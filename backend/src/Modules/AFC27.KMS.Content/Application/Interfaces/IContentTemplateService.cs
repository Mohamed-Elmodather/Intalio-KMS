using AFC27.KMS.Content.Application.DTOs;
using AFC27.KMS.Contracts.Common;

namespace AFC27.KMS.Content.Application.Interfaces;

/// <summary>
/// Service interface for managing content templates.
/// </summary>
public interface IContentTemplateService
{
    /// <summary>
    /// Get a paginated list of templates.
    /// </summary>
    Task<PagedResult<ContentTemplateSummaryDto>> GetTemplatesAsync(
        ContentTemplateFilterRequest filter,
        Guid currentUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a template by ID.
    /// </summary>
    Task<ContentTemplateDto?> GetTemplateByIdAsync(
        Guid templateId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new template.
    /// </summary>
    Task<ContentTemplateDto> CreateTemplateAsync(
        CreateContentTemplateRequest request,
        Guid creatorId,
        string creatorName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing template.
    /// </summary>
    Task<bool> UpdateTemplateAsync(
        Guid templateId,
        UpdateContentTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a template.
    /// </summary>
    Task<bool> DeleteTemplateAsync(
        Guid templateId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Increment the usage count of a template.
    /// </summary>
    Task<bool> IncrementUsageAsync(
        Guid templateId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get template categories.
    /// </summary>
    Task<IReadOnlyList<string>> GetCategoriesAsync(
        CancellationToken cancellationToken = default);

    // Phase 3E: Review cycle operations

    /// <summary>
    /// Mark a template as reviewed.
    /// </summary>
    Task<bool> ReviewTemplateAsync(
        Guid templateId,
        Guid reviewerId,
        string reviewerName,
        ReviewTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the review interval for a template.
    /// </summary>
    Task<bool> SetReviewIntervalAsync(
        Guid templateId,
        int intervalDays,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get templates whose review is overdue.
    /// </summary>
    Task<IReadOnlyList<ContentTemplateSummaryDto>> GetOverdueTemplatesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get templates whose review is due soon.
    /// </summary>
    Task<IReadOnlyList<ContentTemplateSummaryDto>> GetDueSoonTemplatesAsync(
        int daysThreshold = 7,
        CancellationToken cancellationToken = default);
}
