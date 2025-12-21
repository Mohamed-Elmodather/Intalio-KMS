using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AFC27.KMS.WebApi.Features.QualityAssurance.Models;

namespace AFC27.KMS.WebApi.Features.QualityAssurance.Services;

/// <summary>
/// Implementation of document quality assurance service
/// </summary>
public class QualityAssuranceService : IQualityAssuranceService
{
    private readonly ILogger<QualityAssuranceService> _logger;

    // In-memory storage for demo - replace with repository in production
    private static readonly List<QualityReview> _reviews = new();
    private static readonly List<ReviewComment> _comments = new();
    private static readonly List<QualityChecklist> _checklists = new();

    public QualityAssuranceService(ILogger<QualityAssuranceService> logger)
    {
        _logger = logger;

        if (!_checklists.Any())
        {
            SeedDefaultChecklists();
        }
    }

    public async Task<QualityReview> CreateReviewAsync(
        CreateQualityReviewRequest request,
        Guid requestedBy,
        CancellationToken cancellationToken = default)
    {
        // Get checklist criteria
        var checklist = request.ChecklistId.HasValue
            ? await GetChecklistAsync(request.ChecklistId.Value, cancellationToken)
            : await GetDefaultChecklistAsync("General", cancellationToken);

        var review = new QualityReview
        {
            Id = Guid.NewGuid(),
            DocumentId = request.DocumentId,
            DocumentTitle = $"Document {request.DocumentId.ToString()[..8]}",
            DocumentVersion = 1,
            Type = request.Type,
            Status = request.AssignToUserId.HasValue ? ReviewStatus.Assigned : ReviewStatus.Pending,
            RequestedBy = requestedBy,
            RequestedAt = DateTime.UtcNow,
            AssignedTo = request.AssignToUserId,
            AssignedAt = request.AssignToUserId.HasValue ? DateTime.UtcNow : null,
            DueDate = request.DueDate,
            Priority = request.Priority,
            Criteria = checklist?.Criteria.Select(c => new QualityCriterion
            {
                Id = Guid.NewGuid(),
                Category = c.Category,
                Name = c.Name,
                Description = c.Description,
                Weight = c.Weight,
                Type = c.Type,
                IsMandatory = c.IsMandatory,
                Guidelines = c.Guidelines
            }).ToList() ?? new List<QualityCriterion>()
        };

        _reviews.Add(review);

        _logger.LogInformation(
            "Created quality review {ReviewId} for document {DocumentId}",
            review.Id, request.DocumentId);

        return review;
    }

    public Task<QualityReview?> GetReviewAsync(
        Guid reviewId,
        CancellationToken cancellationToken = default)
    {
        var review = _reviews.FirstOrDefault(r => r.Id == reviewId);
        if (review != null)
        {
            review.Comments = _comments.Where(c => c.ReviewId == reviewId).ToList();
        }
        return Task.FromResult(review);
    }

    public Task<(List<QualityReviewSummary> Reviews, int TotalCount)> GetReviewsAsync(
        ReviewStatus? status = null,
        ReviewType? type = null,
        Guid? assignedTo = null,
        Guid? documentId = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _reviews.AsEnumerable();

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        if (type.HasValue)
            query = query.Where(r => r.Type == type.Value);

        if (assignedTo.HasValue)
            query = query.Where(r => r.AssignedTo == assignedTo.Value);

        if (documentId.HasValue)
            query = query.Where(r => r.DocumentId == documentId.Value);

        var totalCount = query.Count();

        var reviews = query
            .OrderByDescending(r => r.RequestedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToSummary)
            .ToList();

        return Task.FromResult((reviews, totalCount));
    }

    public Task<QualityReview> AssignReviewAsync(
        Guid reviewId,
        Guid assignToUserId,
        CancellationToken cancellationToken = default)
    {
        var review = _reviews.FirstOrDefault(r => r.Id == reviewId)
            ?? throw new KeyNotFoundException($"Review {reviewId} not found");

        review.AssignedTo = assignToUserId;
        review.AssignedAt = DateTime.UtcNow;
        review.Status = ReviewStatus.Assigned;

        _logger.LogInformation(
            "Assigned review {ReviewId} to user {UserId}",
            reviewId, assignToUserId);

        return Task.FromResult(review);
    }

    public Task<QualityReview> StartReviewAsync(
        Guid reviewId,
        Guid reviewerId,
        CancellationToken cancellationToken = default)
    {
        var review = _reviews.FirstOrDefault(r => r.Id == reviewId)
            ?? throw new KeyNotFoundException($"Review {reviewId} not found");

        if (review.AssignedTo != reviewerId)
            throw new InvalidOperationException("Review is not assigned to this user");

        review.StartedAt = DateTime.UtcNow;
        review.Status = ReviewStatus.InProgress;

        _logger.LogInformation(
            "Started review {ReviewId} by user {UserId}",
            reviewId, reviewerId);

        return Task.FromResult(review);
    }

    public Task<QualityReview> SubmitReviewAsync(
        SubmitReviewRequest request,
        Guid reviewerId,
        CancellationToken cancellationToken = default)
    {
        var review = _reviews.FirstOrDefault(r => r.Id == request.ReviewId)
            ?? throw new KeyNotFoundException($"Review {request.ReviewId} not found");

        // Update criterion scores
        foreach (var score in request.CriterionScores)
        {
            var criterion = review.Criteria.FirstOrDefault(c => c.Id == score.CriterionId);
            if (criterion != null)
            {
                criterion.Score = score.Score;
                criterion.Feedback = score.Feedback;
            }
        }

        // Calculate final score
        review.FinalScore = CalculateScore(review.Criteria);
        review.Recommendation = request.Recommendation;
        review.RequiresRevision = request.RequiresRevision;
        review.RevisionNotes = request.RevisionNotes ?? new List<string>();
        review.CompletedAt = DateTime.UtcNow;
        review.Status = request.Approved ? ReviewStatus.Completed : ReviewStatus.Rejected;

        _logger.LogInformation(
            "Submitted review {ReviewId} with score {Score}, approved: {Approved}",
            request.ReviewId, review.FinalScore?.OverallScore, request.Approved);

        return Task.FromResult(review);
    }

    public Task CancelReviewAsync(
        Guid reviewId,
        string reason,
        Guid cancelledBy,
        CancellationToken cancellationToken = default)
    {
        var review = _reviews.FirstOrDefault(r => r.Id == reviewId)
            ?? throw new KeyNotFoundException($"Review {reviewId} not found");

        review.Status = ReviewStatus.Cancelled;
        review.RevisionNotes.Add($"Cancelled: {reason}");

        _logger.LogInformation(
            "Cancelled review {ReviewId}: {Reason}",
            reviewId, reason);

        return Task.CompletedTask;
    }

    public Task<ReviewComment> AddCommentAsync(
        Guid reviewId,
        string content,
        CommentType type,
        string? location,
        Guid authorId,
        string authorName,
        Guid? parentCommentId = null,
        CancellationToken cancellationToken = default)
    {
        var comment = new ReviewComment
        {
            Id = Guid.NewGuid(),
            ReviewId = reviewId,
            AuthorId = authorId,
            AuthorName = authorName,
            Content = content,
            Location = location,
            Type = type,
            CreatedAt = DateTime.UtcNow,
            ParentCommentId = parentCommentId
        };

        _comments.Add(comment);
        return Task.FromResult(comment);
    }

    public Task<List<ReviewComment>> GetCommentsAsync(
        Guid reviewId,
        CancellationToken cancellationToken = default)
    {
        var comments = _comments
            .Where(c => c.ReviewId == reviewId)
            .OrderBy(c => c.CreatedAt)
            .ToList();

        return Task.FromResult(comments);
    }

    public Task ResolveCommentAsync(
        Guid commentId,
        Guid resolvedBy,
        CancellationToken cancellationToken = default)
    {
        var comment = _comments.FirstOrDefault(c => c.Id == commentId);
        if (comment != null)
        {
            comment.IsResolved = true;
        }
        return Task.CompletedTask;
    }

    public Task<QualityChecklist> CreateChecklistAsync(
        QualityChecklist checklist,
        Guid createdBy,
        CancellationToken cancellationToken = default)
    {
        checklist.Id = Guid.NewGuid();
        checklist.CreatedAt = DateTime.UtcNow;
        checklist.CreatedBy = createdBy;

        _checklists.Add(checklist);

        _logger.LogInformation(
            "Created quality checklist {ChecklistId}: {Name}",
            checklist.Id, checklist.Name);

        return Task.FromResult(checklist);
    }

    public Task<QualityChecklist?> GetChecklistAsync(
        Guid checklistId,
        CancellationToken cancellationToken = default)
    {
        var checklist = _checklists.FirstOrDefault(c => c.Id == checklistId);
        return Task.FromResult(checklist);
    }

    public Task<List<QualityChecklist>> GetChecklistsAsync(
        string? documentType = null,
        bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        var query = _checklists.AsEnumerable();

        if (activeOnly)
            query = query.Where(c => c.IsActive);

        if (!string.IsNullOrEmpty(documentType))
            query = query.Where(c => c.DocumentType.Equals(documentType, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(query.ToList());
    }

    public Task<QualityChecklist> GetDefaultChecklistAsync(
        string documentType,
        CancellationToken cancellationToken = default)
    {
        var checklist = _checklists.FirstOrDefault(c =>
            c.IsDefault &&
            c.IsActive &&
            (c.DocumentType.Equals(documentType, StringComparison.OrdinalIgnoreCase) ||
             c.DocumentType == "General"));

        return Task.FromResult(checklist ?? _checklists.First());
    }

    public QualityScore CalculateScore(List<QualityCriterion> criteria)
    {
        var scoredCriteria = criteria.Where(c => c.Score.HasValue).ToList();

        if (!scoredCriteria.Any())
        {
            return new QualityScore
            {
                OverallScore = 0,
                Grade = QualityGrade.Poor
            };
        }

        // Calculate weighted average
        var totalWeight = scoredCriteria.Sum(c => c.Weight);
        var weightedSum = scoredCriteria.Sum(c => c.Score!.Value * c.Weight);
        var overallScore = totalWeight > 0 ? weightedSum / totalWeight : 0;

        // Calculate category scores
        var categoryScores = scoredCriteria
            .GroupBy(c => c.Category)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(c => c.Score!.Value * c.Weight) / g.Sum(c => c.Weight));

        // Identify issues
        var issues = scoredCriteria
            .Where(c => c.Score < 60)
            .Select(c => new QualityIssue
            {
                Id = Guid.NewGuid(),
                Category = c.Category,
                Severity = c.Score < 40 ? IssueSeverity.Critical :
                          c.Score < 50 ? IssueSeverity.Major : IssueSeverity.Minor,
                Description = $"{c.Name} scored below acceptable threshold",
                Suggestion = c.Feedback
            })
            .ToList();

        // Identify strengths
        var strengths = scoredCriteria
            .Where(c => c.Score >= 90)
            .Select(c => c.Name)
            .ToList();

        // Identify improvements
        var improvements = scoredCriteria
            .Where(c => c.Score >= 60 && c.Score < 75)
            .Select(c => c.Name)
            .ToList();

        return new QualityScore
        {
            OverallScore = overallScore,
            Grade = GetGrade(overallScore),
            CategoryScores = categoryScores,
            Issues = issues,
            Strengths = strengths,
            Improvements = improvements
        };
    }

    public Task<QualityDashboardSummary> GetDashboardSummaryAsync(
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _reviews.AsEnumerable();
        if (userId.HasValue)
            query = query.Where(r => r.AssignedTo == userId.Value);

        var reviews = query.ToList();
        var completedThisMonth = reviews.Where(r =>
            r.Status == ReviewStatus.Completed &&
            r.CompletedAt >= DateTime.UtcNow.AddDays(-30));

        var summary = new QualityDashboardSummary
        {
            PendingReviews = reviews.Count(r => r.Status == ReviewStatus.Pending || r.Status == ReviewStatus.Assigned),
            InProgressReviews = reviews.Count(r => r.Status == ReviewStatus.InProgress),
            CompletedThisMonth = completedThisMonth.Count(),
            AverageScore = completedThisMonth
                .Where(r => r.FinalScore != null)
                .Select(r => r.FinalScore!.OverallScore)
                .DefaultIfEmpty(0)
                .Average(),
            ReviewsByType = reviews
                .GroupBy(r => r.Type)
                .Select(g => new ReviewByType
                {
                    Type = g.Key,
                    Count = g.Count(),
                    AverageScore = g.Where(r => r.FinalScore != null)
                        .Select(r => r.FinalScore!.OverallScore)
                        .DefaultIfEmpty(0)
                        .Average()
                })
                .ToList(),
            ScoreDistribution = GetScoreDistribution(reviews)
        };

        return Task.FromResult(summary);
    }

    public Task<List<QualityReviewSummary>> GetMyReviewsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var reviews = _reviews
            .Where(r => r.AssignedTo == userId && r.Status != ReviewStatus.Completed)
            .OrderByDescending(r => r.Priority)
            .ThenBy(r => r.DueDate)
            .Select(ToSummary)
            .ToList();

        return Task.FromResult(reviews);
    }

    public Task<List<QualityReviewSummary>> GetOverdueReviewsAsync(
        CancellationToken cancellationToken = default)
    {
        var reviews = _reviews
            .Where(r =>
                r.DueDate.HasValue &&
                r.DueDate < DateTime.UtcNow &&
                r.Status != ReviewStatus.Completed &&
                r.Status != ReviewStatus.Cancelled)
            .OrderBy(r => r.DueDate)
            .Select(ToSummary)
            .ToList();

        return Task.FromResult(reviews);
    }

    public Task<List<QualityReviewSummary>> GetDocumentQualityHistoryAsync(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        var reviews = _reviews
            .Where(r => r.DocumentId == documentId)
            .OrderByDescending(r => r.RequestedAt)
            .Select(ToSummary)
            .ToList();

        return Task.FromResult(reviews);
    }

    public Task<double?> GetDocumentAverageScoreAsync(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        var scores = _reviews
            .Where(r => r.DocumentId == documentId && r.FinalScore != null)
            .Select(r => r.FinalScore!.OverallScore)
            .ToList();

        if (!scores.Any())
            return Task.FromResult<double?>(null);

        return Task.FromResult<double?>(scores.Average());
    }

    private static QualityGrade GetGrade(double score) => score switch
    {
        >= 90 => QualityGrade.Excellent,
        >= 75 => QualityGrade.Good,
        >= 60 => QualityGrade.Acceptable,
        >= 40 => QualityGrade.NeedsWork,
        _ => QualityGrade.Poor
    };

    private static QualityReviewSummary ToSummary(QualityReview review) => new()
    {
        Id = review.Id,
        DocumentId = review.DocumentId,
        DocumentTitle = review.DocumentTitle,
        Type = review.Type,
        Status = review.Status,
        Priority = review.Priority,
        RequestedAt = review.RequestedAt,
        DueDate = review.DueDate,
        FinalScore = review.FinalScore?.OverallScore,
        Grade = review.FinalScore?.Grade
    };

    private static List<ScoreDistribution> GetScoreDistribution(List<QualityReview> reviews)
    {
        var completed = reviews.Where(r => r.FinalScore != null).ToList();
        var total = completed.Count;

        if (total == 0)
            return new List<ScoreDistribution>();

        return Enum.GetValues<QualityGrade>()
            .Select(grade =>
            {
                var count = completed.Count(r => r.FinalScore!.Grade == grade);
                return new ScoreDistribution
                {
                    Grade = grade,
                    Count = count,
                    Percentage = (double)count / total * 100
                };
            })
            .ToList();
    }

    private void SeedDefaultChecklists()
    {
        _checklists.Add(new QualityChecklist
        {
            Id = Guid.NewGuid(),
            Name = "General Document Quality Checklist",
            Description = "Standard checklist for all document types",
            DocumentType = "General",
            IsDefault = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Criteria = new List<QualityCriterion>
            {
                new() { Id = Guid.NewGuid(), Category = "Content", Name = "Accuracy", Description = "Information is factually correct", Weight = 1.5, Type = CriterionType.Scale, IsMandatory = true },
                new() { Id = Guid.NewGuid(), Category = "Content", Name = "Completeness", Description = "All required information is included", Weight = 1.5, Type = CriterionType.Scale, IsMandatory = true },
                new() { Id = Guid.NewGuid(), Category = "Content", Name = "Clarity", Description = "Content is clear and understandable", Weight = 1.0, Type = CriterionType.Scale, IsMandatory = true },
                new() { Id = Guid.NewGuid(), Category = "Structure", Name = "Organization", Description = "Document is well-organized", Weight = 1.0, Type = CriterionType.Scale, IsMandatory = false },
                new() { Id = Guid.NewGuid(), Category = "Structure", Name = "Formatting", Description = "Consistent formatting throughout", Weight = 0.5, Type = CriterionType.Scale, IsMandatory = false },
                new() { Id = Guid.NewGuid(), Category = "Language", Name = "Grammar", Description = "No grammatical errors", Weight = 0.75, Type = CriterionType.Scale, IsMandatory = true },
                new() { Id = Guid.NewGuid(), Category = "Language", Name = "Spelling", Description = "No spelling errors", Weight = 0.5, Type = CriterionType.Scale, IsMandatory = true },
                new() { Id = Guid.NewGuid(), Category = "Compliance", Name = "Template Adherence", Description = "Follows required template", Weight = 1.0, Type = CriterionType.Boolean, IsMandatory = true },
                new() { Id = Guid.NewGuid(), Category = "Compliance", Name = "Brand Guidelines", Description = "Adheres to brand guidelines", Weight = 0.75, Type = CriterionType.Boolean, IsMandatory = false }
            }
        });

        _checklists.Add(new QualityChecklist
        {
            Id = Guid.NewGuid(),
            Name = "Policy Document Checklist",
            Description = "Specialized checklist for policy documents",
            DocumentType = "Policy",
            IsDefault = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Criteria = new List<QualityCriterion>
            {
                new() { Id = Guid.NewGuid(), Category = "Content", Name = "Policy Clarity", Description = "Policy is clearly stated", Weight = 2.0, Type = CriterionType.Scale, IsMandatory = true },
                new() { Id = Guid.NewGuid(), Category = "Content", Name = "Scope Definition", Description = "Scope is well-defined", Weight = 1.5, Type = CriterionType.Scale, IsMandatory = true },
                new() { Id = Guid.NewGuid(), Category = "Compliance", Name = "Legal Review", Description = "Reviewed by legal team", Weight = 2.0, Type = CriterionType.Boolean, IsMandatory = true },
                new() { Id = Guid.NewGuid(), Category = "Compliance", Name = "Regulatory Alignment", Description = "Aligns with regulations", Weight = 2.0, Type = CriterionType.Scale, IsMandatory = true }
            }
        });
    }
}
