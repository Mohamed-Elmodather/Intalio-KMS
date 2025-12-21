using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AFC27.KMS.WebApi.Features.Learning.Models;

namespace AFC27.KMS.WebApi.Features.Learning.Services;

/// <summary>
/// Implementation of learning management service
/// </summary>
public class LearningService : ILearningService
{
    private readonly ILogger<LearningService> _logger;
    private static readonly List<LearningPath> _learningPaths = new();
    private static readonly List<LearningEnrollment> _enrollments = new();
    private static readonly List<Quiz> _quizzes = new();
    private static readonly List<QuizAttempt> _attempts = new();
    private static readonly List<Certificate> _certificates = new();
    private static int _certificateCounter = 1000;

    public LearningService(ILogger<LearningService> logger)
    {
        _logger = logger;
        if (!_learningPaths.Any()) SeedDemoData();
    }

    public Task<LearningPath> CreateLearningPathAsync(CreateLearningPathRequest request, Guid createdBy, CancellationToken cancellationToken = default)
    {
        var path = new LearningPath
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            ThumbnailUrl = request.ThumbnailUrl,
            Category = request.Category,
            Difficulty = request.Difficulty,
            LearningObjectives = request.LearningObjectives ?? new List<string>(),
            Prerequisites = request.Prerequisites ?? new List<string>(),
            Tags = request.Tags ?? new List<string>(),
            IsMandatory = request.IsMandatory,
            DueDate = request.DueDate,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
        _learningPaths.Add(path);
        _logger.LogInformation("Created learning path {PathId}: {Title}", path.Id, path.Title);
        return Task.FromResult(path);
    }

    public Task<LearningPath?> GetLearningPathAsync(Guid pathId, CancellationToken cancellationToken = default) =>
        Task.FromResult(_learningPaths.FirstOrDefault(p => p.Id == pathId));

    public Task<(List<LearningPathSummary> Paths, int TotalCount)> GetLearningPathsAsync(string? category = null, DifficultyLevel? difficulty = null, Guid? userId = null, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var query = _learningPaths.Where(p => p.IsPublished);
        if (!string.IsNullOrEmpty(category)) query = query.Where(p => p.Category == category);
        if (difficulty.HasValue) query = query.Where(p => p.Difficulty == difficulty.Value);

        var totalCount = query.Count();
        var paths = query.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(p => ToSummary(p, userId ?? Guid.Empty)).ToList();

        return Task.FromResult((paths, totalCount));
    }

    public Task<LearningPath> PublishLearningPathAsync(Guid pathId, CancellationToken cancellationToken = default)
    {
        var path = _learningPaths.FirstOrDefault(p => p.Id == pathId) ?? throw new KeyNotFoundException();
        path.IsPublished = true;
        path.EstimatedHours = path.Items.Sum(i => i.DurationMinutes) / 60;
        return Task.FromResult(path);
    }

    public Task<LearningPath> AddItemToPathAsync(Guid pathId, LearningPathItem item, CancellationToken cancellationToken = default)
    {
        var path = _learningPaths.FirstOrDefault(p => p.Id == pathId) ?? throw new KeyNotFoundException();
        item.Id = Guid.NewGuid();
        item.Order = path.Items.Count;
        path.Items.Add(item);
        return Task.FromResult(path);
    }

    public Task<LearningEnrollment> EnrollAsync(Guid pathId, Guid userId, CancellationToken cancellationToken = default)
    {
        var path = _learningPaths.FirstOrDefault(p => p.Id == pathId) ?? throw new KeyNotFoundException();
        var existing = _enrollments.FirstOrDefault(e => e.LearningPathId == pathId && e.UserId == userId);
        if (existing != null) return Task.FromResult(existing);

        var enrollment = new LearningEnrollment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            LearningPathId = pathId,
            LearningPathTitle = path.Title,
            EnrolledAt = DateTime.UtcNow,
            Status = EnrollmentStatus.InProgress,
            ItemProgress = path.Items.Select(i => new ItemProgress { ItemId = i.Id }).ToList()
        };
        _enrollments.Add(enrollment);
        path.EnrollmentCount++;
        return Task.FromResult(enrollment);
    }

    public Task<LearningEnrollment?> GetEnrollmentAsync(Guid pathId, Guid userId, CancellationToken cancellationToken = default) =>
        Task.FromResult(_enrollments.FirstOrDefault(e => e.LearningPathId == pathId && e.UserId == userId));

    public Task<List<LearningEnrollment>> GetUserEnrollmentsAsync(Guid userId, EnrollmentStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _enrollments.Where(e => e.UserId == userId);
        if (status.HasValue) query = query.Where(e => e.Status == status.Value);
        return Task.FromResult(query.ToList());
    }

    public async Task<LearningEnrollment> UpdateProgressAsync(Guid enrollmentId, Guid itemId, bool completed, double? score = null, CancellationToken cancellationToken = default)
    {
        var enrollment = _enrollments.FirstOrDefault(e => e.Id == enrollmentId) ?? throw new KeyNotFoundException();
        var progress = enrollment.ItemProgress.FirstOrDefault(p => p.ItemId == itemId);
        if (progress != null)
        {
            progress.IsCompleted = completed;
            progress.CompletedAt = completed ? DateTime.UtcNow : null;
            progress.Score = score;
        }

        var completedCount = enrollment.ItemProgress.Count(p => p.IsCompleted);
        enrollment.ProgressPercentage = (double)completedCount / enrollment.ItemProgress.Count * 100;

        if (enrollment.ProgressPercentage >= 100)
        {
            enrollment.Status = EnrollmentStatus.Completed;
            enrollment.CompletedAt = DateTime.UtcNow;
            var cert = await IssueCertificateAsync(enrollment.UserId, enrollment.LearningPathId, cancellationToken);
            enrollment.CertificateId = cert.Id;
        }

        return enrollment;
    }

    public Task<Quiz> CreateQuizAsync(Quiz quiz, Guid createdBy, CancellationToken cancellationToken = default)
    {
        quiz.Id = Guid.NewGuid();
        quiz.CreatedBy = createdBy;
        quiz.CreatedAt = DateTime.UtcNow;
        foreach (var q in quiz.Questions) { q.Id = Guid.NewGuid(); foreach (var a in q.Answers) a.Id = Guid.NewGuid(); }
        _quizzes.Add(quiz);
        return Task.FromResult(quiz);
    }

    public Task<Quiz?> GetQuizAsync(Guid quizId, CancellationToken cancellationToken = default) =>
        Task.FromResult(_quizzes.FirstOrDefault(q => q.Id == quizId));

    public Task<QuizAttempt> StartQuizAttemptAsync(Guid quizId, Guid userId, CancellationToken cancellationToken = default)
    {
        var quiz = _quizzes.FirstOrDefault(q => q.Id == quizId) ?? throw new KeyNotFoundException();
        var existingAttempts = _attempts.Count(a => a.QuizId == quizId && a.UserId == userId);
        if (existingAttempts >= quiz.MaxAttempts) throw new InvalidOperationException("Max attempts exceeded");

        var attempt = new QuizAttempt
        {
            Id = Guid.NewGuid(),
            QuizId = quizId,
            UserId = userId,
            AttemptNumber = existingAttempts + 1,
            StartedAt = DateTime.UtcNow
        };
        _attempts.Add(attempt);
        return Task.FromResult(attempt);
    }

    public Task<QuizAttempt> SubmitQuizAttemptAsync(Guid attemptId, List<QuestionResponse> responses, CancellationToken cancellationToken = default)
    {
        var attempt = _attempts.FirstOrDefault(a => a.Id == attemptId) ?? throw new KeyNotFoundException();
        var quiz = _quizzes.FirstOrDefault(q => q.Id == attempt.QuizId)!;

        int totalPoints = 0, earnedPoints = 0;
        foreach (var response in responses)
        {
            var question = quiz.Questions.FirstOrDefault(q => q.Id == response.QuestionId);
            if (question == null) continue;
            totalPoints += question.Points;
            var correctAnswers = question.Answers.Where(a => a.IsCorrect).Select(a => a.Id).ToHashSet();
            response.IsCorrect = response.SelectedAnswerIds != null && response.SelectedAnswerIds.ToHashSet().SetEquals(correctAnswers);
            response.PointsEarned = response.IsCorrect ? question.Points : 0;
            earnedPoints += response.PointsEarned;
        }

        attempt.Responses = responses;
        attempt.Score = totalPoints > 0 ? (double)earnedPoints / totalPoints * 100 : 0;
        attempt.Passed = attempt.Score >= quiz.PassingScore;
        attempt.CompletedAt = DateTime.UtcNow;

        return Task.FromResult(attempt);
    }

    public Task<List<QuizAttempt>> GetUserQuizAttemptsAsync(Guid quizId, Guid userId, CancellationToken cancellationToken = default) =>
        Task.FromResult(_attempts.Where(a => a.QuizId == quizId && a.UserId == userId).OrderByDescending(a => a.StartedAt).ToList());

    public Task<Certificate> IssueCertificateAsync(Guid userId, Guid learningPathId, CancellationToken cancellationToken = default)
    {
        var path = _learningPaths.FirstOrDefault(p => p.Id == learningPathId) ?? throw new KeyNotFoundException();
        var cert = new Certificate
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            UserName = "User",
            LearningPathId = learningPathId,
            LearningPathTitle = path.Title,
            CertificateNumber = $"AFC27-CERT-{DateTime.UtcNow:yyyyMMdd}-{Interlocked.Increment(ref _certificateCounter)}",
            IssuedAt = DateTime.UtcNow,
            Status = CertificateStatus.Active,
            VerificationUrl = $"/api/learning/certificates/verify/{Guid.NewGuid()}"
        };
        _certificates.Add(cert);
        _logger.LogInformation("Issued certificate {CertNumber} for path {PathId}", cert.CertificateNumber, learningPathId);
        return Task.FromResult(cert);
    }

    public Task<Certificate?> GetCertificateAsync(Guid certificateId, CancellationToken cancellationToken = default) =>
        Task.FromResult(_certificates.FirstOrDefault(c => c.Id == certificateId));

    public Task<List<Certificate>> GetUserCertificatesAsync(Guid userId, CancellationToken cancellationToken = default) =>
        Task.FromResult(_certificates.Where(c => c.UserId == userId).OrderByDescending(c => c.IssuedAt).ToList());

    public Task<bool> VerifyCertificateAsync(string certificateNumber, CancellationToken cancellationToken = default) =>
        Task.FromResult(_certificates.Any(c => c.CertificateNumber == certificateNumber && c.Status == CertificateStatus.Active));

    public async Task<LearningDashboard> GetUserDashboardAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var enrollments = await GetUserEnrollmentsAsync(userId, null, cancellationToken);
        var certificates = await GetUserCertificatesAsync(userId, cancellationToken);

        return new LearningDashboard
        {
            EnrolledPaths = enrollments.Count,
            CompletedPaths = enrollments.Count(e => e.Status == EnrollmentStatus.Completed),
            CertificatesEarned = certificates.Count,
            TotalLearningHours = enrollments.Sum(e => e.ItemProgress.Sum(p => p.TimeSpentMinutes)) / 60,
            OverallProgress = enrollments.Any() ? enrollments.Average(e => e.ProgressPercentage) : 0,
            CurrentEnrollments = enrollments.Where(e => e.Status == EnrollmentStatus.InProgress).Take(5).ToList(),
            RecentCertificates = certificates.Take(5).ToList(),
            MandatoryPaths = _learningPaths.Where(p => p.IsMandatory && p.IsPublished).Select(p => ToSummary(p, userId)).ToList()
        };
    }

    public Task<List<string>> GetCategoriesAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(_learningPaths.Where(p => p.IsPublished).Select(p => p.Category).Distinct().OrderBy(c => c).ToList());

    private LearningPathSummary ToSummary(LearningPath path, Guid userId)
    {
        var enrollment = _enrollments.FirstOrDefault(e => e.LearningPathId == path.Id && e.UserId == userId);
        return new LearningPathSummary
        {
            Id = path.Id,
            Title = path.Title,
            Description = path.Description,
            ThumbnailUrl = path.ThumbnailUrl,
            Category = path.Category,
            Difficulty = path.Difficulty,
            EstimatedHours = path.EstimatedHours,
            ItemCount = path.Items.Count,
            EnrollmentCount = path.EnrollmentCount,
            AverageRating = path.AverageRating,
            IsEnrolled = enrollment != null,
            Progress = enrollment?.ProgressPercentage,
            IsMandatory = path.IsMandatory
        };
    }

    private void SeedDemoData()
    {
        _learningPaths.Add(new LearningPath
        {
            Id = Guid.NewGuid(),
            Title = "AFC 2027 Knowledge Management Fundamentals",
            Description = "Essential training for understanding and using the KMS platform effectively",
            Category = "Platform Training",
            Difficulty = DifficultyLevel.Beginner,
            EstimatedHours = 4,
            IsPublished = true,
            IsMandatory = true,
            LearningObjectives = new List<string>
            {
                "Navigate the KMS platform efficiently",
                "Create and manage documents",
                "Understand document workflows",
                "Use search and discovery features"
            },
            Items = new List<LearningPathItem>
            {
                new() { Id = Guid.NewGuid(), Order = 0, Type = LearningItemType.Course, Title = "Platform Overview", DurationMinutes = 30 },
                new() { Id = Guid.NewGuid(), Order = 1, Type = LearningItemType.Course, Title = "Document Management", DurationMinutes = 45 },
                new() { Id = Guid.NewGuid(), Order = 2, Type = LearningItemType.Quiz, Title = "Knowledge Check", DurationMinutes = 15, PassingScore = 70 }
            },
            CreatedBy = Guid.Empty,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            EnrollmentCount = 145,
            AverageRating = 4.5
        });
    }
}
