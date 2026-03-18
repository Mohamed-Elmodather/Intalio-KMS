namespace AFC27.KMS.Content.Application.DTOs;

/// <summary>
/// Health score details for a single article.
/// </summary>
public record ContentHealthDto
{
    public Guid ArticleId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? TitleArabic { get; init; }
    public string Slug { get; init; } = string.Empty;
    public double? HealthScore { get; init; }
    public DateTime? HealthScoreCalculatedAt { get; init; }
    public string Status { get; init; } = string.Empty;
    public string VerificationStatus { get; init; } = string.Empty;
    public string? OwnerName { get; init; }
    public string? CategoryName { get; init; }
    public DateTime? PublishedAt { get; init; }
    public DateTime? LastVerifiedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public int ViewCount { get; init; }

    /// <summary>
    /// Breakdown of individual score components.
    /// </summary>
    public HealthScoreBreakdown? Breakdown { get; init; }
}

/// <summary>
/// Breakdown of the health score into its weighted components.
/// </summary>
public record HealthScoreBreakdown
{
    /// <summary>
    /// How recently the content was updated (weight: 0.3).
    /// </summary>
    public double Freshness { get; init; }

    /// <summary>
    /// Verification status score (weight: 0.3).
    /// </summary>
    public double Verification { get; init; }

    /// <summary>
    /// User engagement score based on views (weight: 0.2).
    /// </summary>
    public double Engagement { get; init; }

    /// <summary>
    /// Content completeness (both languages, summary, category, tags) (weight: 0.1).
    /// </summary>
    public double Completeness { get; init; }

    /// <summary>
    /// Content quality indicators (weight: 0.1).
    /// </summary>
    public double Quality { get; init; }
}

/// <summary>
/// Aggregated health metrics across all content.
/// </summary>
public record ContentHealthSummaryDto
{
    public int TotalArticles { get; init; }
    public double AverageHealthScore { get; init; }
    public int HealthyCount { get; init; }
    public int NeedsAttentionCount { get; init; }
    public int StaleCount { get; init; }
    public int UnscoreCount { get; init; }

    /// <summary>
    /// Distribution of health scores in 0.1 buckets (0.0-0.1, 0.1-0.2, etc.).
    /// </summary>
    public IReadOnlyList<HealthBucket> Distribution { get; init; } = Array.Empty<HealthBucket>();
}

/// <summary>
/// A bucket in the health score distribution.
/// </summary>
public record HealthBucket
{
    public double RangeStart { get; init; }
    public double RangeEnd { get; init; }
    public int Count { get; init; }
}

/// <summary>
/// Stale content that needs attention.
/// </summary>
public record StaleContentDto
{
    public Guid ArticleId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? TitleArabic { get; init; }
    public string Slug { get; init; } = string.Empty;
    public double? HealthScore { get; init; }
    public string Status { get; init; } = string.Empty;
    public string VerificationStatus { get; init; } = string.Empty;
    public string? OwnerName { get; init; }
    public Guid? OwnerId { get; init; }
    public string? CategoryName { get; init; }
    public DateTime? LastVerifiedAt { get; init; }
    public DateTime? PublishedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public int ViewCount { get; init; }
    public int DaysSinceUpdate { get; init; }

    /// <summary>
    /// Primary reason the content is considered stale.
    /// </summary>
    public string StaleReason { get; init; } = string.Empty;
}
