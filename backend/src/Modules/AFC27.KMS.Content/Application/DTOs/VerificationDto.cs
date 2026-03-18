namespace AFC27.KMS.Content.Application.DTOs;

/// <summary>
/// DTO representing a single verification record in the audit trail.
/// </summary>
public record VerificationRecordDto
{
    public Guid Id { get; init; }
    public Guid ArticleId { get; init; }
    public string ArticleTitle { get; init; } = string.Empty;
    public Guid VerifiedById { get; init; }
    public string VerifiedByName { get; init; } = string.Empty;
    public DateTime VerifiedAt { get; init; }
    public string? Notes { get; init; }
    public string PreviousStatus { get; init; } = string.Empty;
    public string NewStatus { get; init; } = string.Empty;
    public DateTime? ExpiresAt { get; init; }
}

/// <summary>
/// Request to verify an article's knowledge content is still accurate.
/// </summary>
public record VerifyArticleRequest
{
    public string? Notes { get; init; }
    public int? ReviewIntervalDays { get; init; }
}

/// <summary>
/// Request to assign a knowledge owner to an article.
/// </summary>
public record AssignOwnerRequest
{
    public Guid OwnerId { get; init; }
    public string OwnerName { get; init; } = string.Empty;
}

/// <summary>
/// Dashboard view of the overall knowledge verification state.
/// </summary>
public record VerificationDashboardDto
{
    public VerificationMetricsDto Metrics { get; init; } = new();
    public IReadOnlyList<ArticleVerificationSummaryDto> OverdueArticles { get; init; } = Array.Empty<ArticleVerificationSummaryDto>();
    public IReadOnlyList<ArticleVerificationSummaryDto> DueSoonArticles { get; init; } = Array.Empty<ArticleVerificationSummaryDto>();
    public IReadOnlyList<VerificationRecordDto> RecentVerifications { get; init; } = Array.Empty<VerificationRecordDto>();
}

/// <summary>
/// Aggregate metrics for the verification dashboard.
/// </summary>
public record VerificationMetricsDto
{
    public int TotalArticles { get; init; }
    public int VerifiedCount { get; init; }
    public int UnverifiedCount { get; init; }
    public int DueSoonCount { get; init; }
    public int OverdueCount { get; init; }
    public double VerifiedPercentage { get; init; }
    public int ArticlesWithOwner { get; init; }
    public int ArticlesWithoutOwner { get; init; }
}

/// <summary>
/// Lightweight article summary used in verification lists.
/// </summary>
public record ArticleVerificationSummaryDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? TitleArabic { get; init; }
    public string Slug { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string VerificationStatus { get; init; } = string.Empty;
    public Guid? OwnerId { get; init; }
    public string? OwnerName { get; init; }
    public DateTime? LastVerifiedAt { get; init; }
    public DateTime? NextVerificationDue { get; init; }
    public int ReviewIntervalDays { get; init; }
    public string? CategoryName { get; init; }
}
