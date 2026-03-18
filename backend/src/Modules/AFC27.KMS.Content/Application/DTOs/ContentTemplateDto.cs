namespace AFC27.KMS.Content.Application.DTOs;

/// <summary>
/// Content template response DTO.
/// </summary>
public record ContentTemplateDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string Structure { get; init; } = "{}";
    public string Category { get; init; } = string.Empty;
    public bool IsPublic { get; init; }
    public bool IsSystem { get; init; }
    public Guid CreatorId { get; init; }
    public string CreatorName { get; init; } = string.Empty;
    public string? ThumbnailUrl { get; init; }
    public int UsageCount { get; init; }
    public string? Tags { get; init; }

    // Phase 3E: Review cycle fields
    public int ReviewIntervalDays { get; init; }
    public DateTime? LastReviewedAt { get; init; }
    public DateTime? NextReviewDue { get; init; }
    public string? LastReviewedByName { get; init; }
    public string ReviewStatus { get; init; } = "NotScheduled";

    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
}

/// <summary>
/// Content template summary for lists.
/// </summary>
public record ContentTemplateSummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string Category { get; init; } = string.Empty;
    public bool IsPublic { get; init; }
    public bool IsSystem { get; init; }
    public string CreatorName { get; init; } = string.Empty;
    public string? ThumbnailUrl { get; init; }
    public int UsageCount { get; init; }
    public string ReviewStatus { get; init; } = "NotScheduled";
    public DateTime? NextReviewDue { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Create content template request.
/// </summary>
public record CreateContentTemplateRequest
{
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string Structure { get; init; } = "{}";
    public string Category { get; init; } = "General";
    public bool IsPublic { get; init; } = false;
    public string? ThumbnailUrl { get; init; }
    public string? Tags { get; init; }
    public int ReviewIntervalDays { get; init; } = 0;
}

/// <summary>
/// Update content template request.
/// </summary>
public record UpdateContentTemplateRequest
{
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string Structure { get; init; } = "{}";
    public string Category { get; init; } = "General";
    public bool IsPublic { get; init; } = false;
    public string? ThumbnailUrl { get; init; }
    public string? Tags { get; init; }
}

/// <summary>
/// Content template filter request.
/// </summary>
public record ContentTemplateFilterRequest
{
    public string? Search { get; init; }
    public string? Category { get; init; }
    public bool? IsPublic { get; init; }
    public Guid? CreatorId { get; init; }
    public string? ReviewStatus { get; init; }
    public string SortBy { get; init; } = "Name";
    public bool SortDescending { get; init; } = false;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

/// <summary>
/// Request to review a template (Phase 3E).
/// </summary>
public record ReviewTemplateRequest
{
    /// <summary>
    /// Optional notes about the review.
    /// </summary>
    public string? ReviewNotes { get; init; }

    /// <summary>
    /// Optionally update the review interval.
    /// </summary>
    public int? NewReviewIntervalDays { get; init; }
}

/// <summary>
/// Request to set the review interval for a template (Phase 3E).
/// </summary>
public record SetReviewIntervalRequest
{
    public int ReviewIntervalDays { get; init; }
}
