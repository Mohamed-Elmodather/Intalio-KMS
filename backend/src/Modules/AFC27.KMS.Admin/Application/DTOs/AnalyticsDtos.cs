namespace AFC27.KMS.Admin.Application.DTOs;

/// <summary>
/// Usage metrics for the platform.
/// </summary>
public record UsageMetricsDto
{
    public DateTime PeriodStart { get; init; }
    public DateTime PeriodEnd { get; init; }
    public int TotalActiveUsers { get; init; }
    public int TotalLogins { get; init; }
    public int TotalPageViews { get; init; }
    public int TotalApiCalls { get; init; }
    public double AverageSessionDurationMinutes { get; init; }
    public IReadOnlyList<DailyUsageDto> DailyBreakdown { get; init; } = Array.Empty<DailyUsageDto>();
}

public record DailyUsageDto
{
    public DateTime Date { get; init; }
    public int ActiveUsers { get; init; }
    public int Logins { get; init; }
    public int PageViews { get; init; }
    public int ApiCalls { get; init; }
}

/// <summary>
/// Engagement trends across content types.
/// </summary>
public record EngagementTrendsDto
{
    public DateTime PeriodStart { get; init; }
    public DateTime PeriodEnd { get; init; }
    public IReadOnlyList<ContentEngagementDto> ContentEngagement { get; init; } = Array.Empty<ContentEngagementDto>();
    public IReadOnlyList<TopContentDto> MostViewedContent { get; init; } = Array.Empty<TopContentDto>();
    public IReadOnlyList<TopContentDto> MostSharedContent { get; init; } = Array.Empty<TopContentDto>();
    public IReadOnlyList<TopContributorDto> TopContributors { get; init; } = Array.Empty<TopContributorDto>();
}

public record ContentEngagementDto
{
    public string ContentType { get; init; } = string.Empty;
    public int Views { get; init; }
    public int Edits { get; init; }
    public int Shares { get; init; }
    public int Comments { get; init; }
    public int Downloads { get; init; }
}

public record TopContentDto
{
    public Guid ContentId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public int Count { get; init; }
    public string? AuthorName { get; init; }
}

public record TopContributorDto
{
    public Guid UserId { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public int Contributions { get; init; }
    public int ArticlesCreated { get; init; }
    public int DocumentsUploaded { get; init; }
    public int CommentsPosted { get; init; }
}

/// <summary>
/// Search analytics data.
/// </summary>
public record SearchAnalyticsDto
{
    public DateTime PeriodStart { get; init; }
    public DateTime PeriodEnd { get; init; }
    public int TotalSearches { get; init; }
    public int UniqueSearchers { get; init; }
    public double AverageResultsReturned { get; init; }
    public double SearchSuccessRate { get; init; }
    public IReadOnlyList<TopSearchTermDto> TopSearchTerms { get; init; } = Array.Empty<TopSearchTermDto>();
    public IReadOnlyList<FailedSearchDto> TopFailedSearches { get; init; } = Array.Empty<FailedSearchDto>();
    public IReadOnlyList<DailySearchCountDto> DailyBreakdown { get; init; } = Array.Empty<DailySearchCountDto>();
}

public record TopSearchTermDto
{
    public string Term { get; init; } = string.Empty;
    public int Count { get; init; }
    public double AverageResults { get; init; }
}

public record FailedSearchDto
{
    public string Term { get; init; } = string.Empty;
    public int Count { get; init; }
}

public record DailySearchCountDto
{
    public DateTime Date { get; init; }
    public int Count { get; init; }
    public int UniqueUsers { get; init; }
}

/// <summary>
/// AI usage statistics.
/// </summary>
public record AIUsageStatsDto
{
    public DateTime PeriodStart { get; init; }
    public DateTime PeriodEnd { get; init; }
    public int TotalAIRequests { get; init; }
    public int UniqueAIUsers { get; init; }
    public int TotalTokensConsumed { get; init; }
    public double AverageResponseTimeMs { get; init; }
    public double SuccessRate { get; init; }
    public IReadOnlyList<AIOperationStatsDto> OperationBreakdown { get; init; } = Array.Empty<AIOperationStatsDto>();
    public IReadOnlyList<DailyAIUsageDto> DailyBreakdown { get; init; } = Array.Empty<DailyAIUsageDto>();
}

public record AIOperationStatsDto
{
    public string Operation { get; init; } = string.Empty;
    public int Count { get; init; }
    public int TotalTokens { get; init; }
    public double AverageResponseTimeMs { get; init; }
    public double SuccessRate { get; init; }
}

public record DailyAIUsageDto
{
    public DateTime Date { get; init; }
    public int Requests { get; init; }
    public int TokensConsumed { get; init; }
    public int UniqueUsers { get; init; }
}

/// <summary>
/// Content growth statistics.
/// </summary>
public record ContentGrowthDto
{
    public DateTime PeriodStart { get; init; }
    public DateTime PeriodEnd { get; init; }
    public int TotalDocuments { get; init; }
    public int TotalArticles { get; init; }
    public int NewDocuments { get; init; }
    public int NewArticles { get; init; }
    public long TotalStorageBytes { get; init; }
    public long StorageGrowthBytes { get; init; }
    public IReadOnlyList<ContentGrowthPointDto> GrowthTimeline { get; init; } = Array.Empty<ContentGrowthPointDto>();
    public IReadOnlyList<ContentTypeBreakdownDto> TypeBreakdown { get; init; } = Array.Empty<ContentTypeBreakdownDto>();
}

public record ContentGrowthPointDto
{
    public DateTime Date { get; init; }
    public int CumulativeDocuments { get; init; }
    public int CumulativeArticles { get; init; }
    public long CumulativeStorageBytes { get; init; }
}

public record ContentTypeBreakdownDto
{
    public string ContentType { get; init; } = string.Empty;
    public int Count { get; init; }
    public long StorageBytes { get; init; }
}

/// <summary>
/// Analytics query time range.
/// </summary>
public record AnalyticsTimeRange
{
    public DateTime From { get; init; }
    public DateTime To { get; init; }
    public string? Granularity { get; init; } // "daily", "weekly", "monthly"
}
