using AFC27.KMS.Content.Application.DTOs;

namespace AFC27.KMS.Content.Application.Interfaces;

/// <summary>
/// Service interface for the Knowledge Verification Lifecycle.
/// Manages article ownership, periodic verification, and dashboard analytics.
/// </summary>
public interface IVerificationService
{
    /// <summary>
    /// Verify that an article's knowledge content is still accurate and up-to-date.
    /// Resets the verification timer based on the configured review interval.
    /// </summary>
    Task<VerificationRecordDto> VerifyArticleAsync(
        Guid articleId, VerifyArticleRequest request, Guid verifiedById, string verifiedByName,
        CancellationToken ct = default);

    /// <summary>
    /// Assign a knowledge owner responsible for periodic verification of an article.
    /// </summary>
    Task<bool> AssignOwnerAsync(
        Guid articleId, AssignOwnerRequest request, CancellationToken ct = default);

    /// <summary>
    /// Get the verification dashboard with metrics, overdue/due-soon lists, and recent activity.
    /// </summary>
    Task<VerificationDashboardDto> GetDashboardAsync(CancellationToken ct = default);

    /// <summary>
    /// Get all articles whose verification is overdue.
    /// </summary>
    Task<IReadOnlyList<ArticleVerificationSummaryDto>> GetOverdueArticlesAsync(
        CancellationToken ct = default);

    /// <summary>
    /// Get all articles whose verification is due within the next 7 days.
    /// </summary>
    Task<IReadOnlyList<ArticleVerificationSummaryDto>> GetDueSoonArticlesAsync(
        CancellationToken ct = default);

    /// <summary>
    /// Get the full verification history for a specific article.
    /// </summary>
    Task<IReadOnlyList<VerificationRecordDto>> GetVerificationHistoryAsync(
        Guid articleId, CancellationToken ct = default);
}
