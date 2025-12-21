using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AFC27.KMS.WebApi.Features.Learning.Models;

namespace AFC27.KMS.WebApi.Features.Learning.Services;

/// <summary>
/// Interface for learning management service
/// </summary>
public interface ILearningService
{
    // Learning Paths
    Task<LearningPath> CreateLearningPathAsync(CreateLearningPathRequest request, Guid createdBy, CancellationToken cancellationToken = default);
    Task<LearningPath?> GetLearningPathAsync(Guid pathId, CancellationToken cancellationToken = default);
    Task<(List<LearningPathSummary> Paths, int TotalCount)> GetLearningPathsAsync(string? category = null, DifficultyLevel? difficulty = null, Guid? userId = null, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<LearningPath> PublishLearningPathAsync(Guid pathId, CancellationToken cancellationToken = default);
    Task<LearningPath> AddItemToPathAsync(Guid pathId, LearningPathItem item, CancellationToken cancellationToken = default);

    // Enrollment
    Task<LearningEnrollment> EnrollAsync(Guid pathId, Guid userId, CancellationToken cancellationToken = default);
    Task<LearningEnrollment?> GetEnrollmentAsync(Guid pathId, Guid userId, CancellationToken cancellationToken = default);
    Task<List<LearningEnrollment>> GetUserEnrollmentsAsync(Guid userId, EnrollmentStatus? status = null, CancellationToken cancellationToken = default);
    Task<LearningEnrollment> UpdateProgressAsync(Guid enrollmentId, Guid itemId, bool completed, double? score = null, CancellationToken cancellationToken = default);

    // Quizzes
    Task<Quiz> CreateQuizAsync(Quiz quiz, Guid createdBy, CancellationToken cancellationToken = default);
    Task<Quiz?> GetQuizAsync(Guid quizId, CancellationToken cancellationToken = default);
    Task<QuizAttempt> StartQuizAttemptAsync(Guid quizId, Guid userId, CancellationToken cancellationToken = default);
    Task<QuizAttempt> SubmitQuizAttemptAsync(Guid attemptId, List<QuestionResponse> responses, CancellationToken cancellationToken = default);
    Task<List<QuizAttempt>> GetUserQuizAttemptsAsync(Guid quizId, Guid userId, CancellationToken cancellationToken = default);

    // Certificates
    Task<Certificate> IssueCertificateAsync(Guid userId, Guid learningPathId, CancellationToken cancellationToken = default);
    Task<Certificate?> GetCertificateAsync(Guid certificateId, CancellationToken cancellationToken = default);
    Task<List<Certificate>> GetUserCertificatesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> VerifyCertificateAsync(string certificateNumber, CancellationToken cancellationToken = default);

    // Dashboard
    Task<LearningDashboard> GetUserDashboardAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<string>> GetCategoriesAsync(CancellationToken cancellationToken = default);
}
