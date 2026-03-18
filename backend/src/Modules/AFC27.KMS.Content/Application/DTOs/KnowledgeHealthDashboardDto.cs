namespace AFC27.KMS.Content.Application.DTOs;

/// <summary>
/// Comprehensive dashboard data for the Knowledge Health Dashboard.
/// </summary>
public record KnowledgeHealthDashboardDto
{
    /// <summary>
    /// Overall content health summary metrics.
    /// </summary>
    public ContentHealthSummaryDto HealthSummary { get; init; } = new();

    /// <summary>
    /// Breakdown of articles by verification status.
    /// </summary>
    public VerificationStatusBreakdown VerificationBreakdown { get; init; } = new();

    /// <summary>
    /// Monthly freshness heatmap data (last 12 months).
    /// Each entry represents a month with count of articles updated in that period.
    /// </summary>
    public IReadOnlyList<FreshnessHeatmapEntry> FreshnessHeatmap { get; init; } = Array.Empty<FreshnessHeatmapEntry>();

    /// <summary>
    /// Categories with no or very few articles, indicating knowledge gaps.
    /// </summary>
    public IReadOnlyList<CoverageGap> CoverageGaps { get; init; } = Array.Empty<CoverageGap>();

    /// <summary>
    /// Top contributors by article count and verification activity.
    /// </summary>
    public IReadOnlyList<TopContributor> TopContributors { get; init; } = Array.Empty<TopContributor>();

    /// <summary>
    /// Articles with no category, no tags, or no owner (orphaned content).
    /// </summary>
    public IReadOnlyList<OrphanedContent> OrphanedContent { get; init; } = Array.Empty<OrphanedContent>();

    /// <summary>
    /// Health score trend over the last 12 weeks (weekly averages).
    /// </summary>
    public IReadOnlyList<HealthTrendEntry> HealthTrend { get; init; } = Array.Empty<HealthTrendEntry>();
}

/// <summary>
/// Breakdown of articles by verification status.
/// </summary>
public record VerificationStatusBreakdown
{
    public int Verified { get; init; }
    public int Unverified { get; init; }
    public int DueSoon { get; init; }
    public int Overdue { get; init; }
    public double VerifiedPercentage { get; init; }
}

/// <summary>
/// A single entry in the freshness heatmap.
/// </summary>
public record FreshnessHeatmapEntry
{
    public int Year { get; init; }
    public int Month { get; init; }
    public string MonthLabel { get; init; } = string.Empty;
    public int ArticlesUpdated { get; init; }
    public int ArticlesCreated { get; init; }
}

/// <summary>
/// Represents a knowledge coverage gap (category with insufficient content).
/// </summary>
public record CoverageGap
{
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public string? CategoryNameArabic { get; init; }
    public int ArticleCount { get; init; }
    public string Severity { get; init; } = string.Empty; // "Critical", "Warning", "Info"
}

/// <summary>
/// Top contributor details.
/// </summary>
public record TopContributor
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public int ArticleCount { get; init; }
    public int VerificationCount { get; init; }
    public double AverageHealthScore { get; init; }
}

/// <summary>
/// Content that lacks proper categorization, tagging, or ownership.
/// </summary>
public record OrphanedContent
{
    public Guid ArticleId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? TitleArabic { get; init; }
    public string Slug { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// What makes this content "orphaned": "NoCategory", "NoTags", "NoOwner", "NoCategoryOrTags".
    /// </summary>
    public string OrphanReason { get; init; } = string.Empty;
}

/// <summary>
/// A single data point in the health score trend.
/// </summary>
public record HealthTrendEntry
{
    public DateTime WeekStart { get; init; }
    public string WeekLabel { get; init; } = string.Empty;
    public double AverageHealthScore { get; init; }
    public int ArticleCount { get; init; }
}
