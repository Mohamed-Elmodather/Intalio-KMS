using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AFC27.KMS.WebApi.Features.QualityAssurance.Models;

namespace AFC27.KMS.WebApi.Features.QualityAssurance.Services;

/// <summary>
/// Interface for document quality assurance service
/// </summary>
public interface IQualityAssuranceService
{
    // Review Management
    Task<QualityReview> CreateReviewAsync(
        CreateQualityReviewRequest request,
        Guid requestedBy,
        CancellationToken cancellationToken = default);

    Task<QualityReview?> GetReviewAsync(
        Guid reviewId,
        CancellationToken cancellationToken = default);

    Task<(List<QualityReviewSummary> Reviews, int TotalCount)> GetReviewsAsync(
        ReviewStatus? status = null,
        ReviewType? type = null,
        Guid? assignedTo = null,
        Guid? documentId = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<QualityReview> AssignReviewAsync(
        Guid reviewId,
        Guid assignToUserId,
        CancellationToken cancellationToken = default);

    Task<QualityReview> StartReviewAsync(
        Guid reviewId,
        Guid reviewerId,
        CancellationToken cancellationToken = default);

    Task<QualityReview> SubmitReviewAsync(
        SubmitReviewRequest request,
        Guid reviewerId,
        CancellationToken cancellationToken = default);

    Task CancelReviewAsync(
        Guid reviewId,
        string reason,
        Guid cancelledBy,
        CancellationToken cancellationToken = default);

    // Comments
    Task<ReviewComment> AddCommentAsync(
        Guid reviewId,
        string content,
        CommentType type,
        string? location,
        Guid authorId,
        string authorName,
        Guid? parentCommentId = null,
        CancellationToken cancellationToken = default);

    Task<List<ReviewComment>> GetCommentsAsync(
        Guid reviewId,
        CancellationToken cancellationToken = default);

    Task ResolveCommentAsync(
        Guid commentId,
        Guid resolvedBy,
        CancellationToken cancellationToken = default);

    // Checklists
    Task<QualityChecklist> CreateChecklistAsync(
        QualityChecklist checklist,
        Guid createdBy,
        CancellationToken cancellationToken = default);

    Task<QualityChecklist?> GetChecklistAsync(
        Guid checklistId,
        CancellationToken cancellationToken = default);

    Task<List<QualityChecklist>> GetChecklistsAsync(
        string? documentType = null,
        bool activeOnly = true,
        CancellationToken cancellationToken = default);

    Task<QualityChecklist> GetDefaultChecklistAsync(
        string documentType,
        CancellationToken cancellationToken = default);

    // Scoring
    QualityScore CalculateScore(List<QualityCriterion> criteria);

    // Dashboard & Reporting
    Task<QualityDashboardSummary> GetDashboardSummaryAsync(
        Guid? userId = null,
        CancellationToken cancellationToken = default);

    Task<List<QualityReviewSummary>> GetMyReviewsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<List<QualityReviewSummary>> GetOverdueReviewsAsync(
        CancellationToken cancellationToken = default);

    // Document Quality History
    Task<List<QualityReviewSummary>> GetDocumentQualityHistoryAsync(
        Guid documentId,
        CancellationToken cancellationToken = default);

    Task<double?> GetDocumentAverageScoreAsync(
        Guid documentId,
        CancellationToken cancellationToken = default);
}
