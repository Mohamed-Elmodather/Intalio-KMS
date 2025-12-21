using AFC27.KMS.Search.Domain.Entities;

namespace AFC27.KMS.Search.Application.DTOs;

/// <summary>
/// Response DTO for search results
/// </summary>
public record SearchResponse
{
    /// <summary>
    /// Original search query
    /// </summary>
    public string Query { get; init; } = string.Empty;

    /// <summary>
    /// Total number of matching results
    /// </summary>
    public long TotalResults { get; init; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Results per page
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalResults / PageSize) : 0;

    /// <summary>
    /// Search execution time in milliseconds
    /// </summary>
    public long ExecutionTimeMs { get; init; }

    /// <summary>
    /// Search results
    /// </summary>
    public List<SearchResultDto> Results { get; init; } = new();

    /// <summary>
    /// Facets for filtering
    /// </summary>
    public SearchFacets? Facets { get; init; }

    /// <summary>
    /// Spelling correction suggestion
    /// </summary>
    public string? SpellingSuggestion { get; init; }

    /// <summary>
    /// Related searches
    /// </summary>
    public List<string> RelatedSearches { get; init; } = new();
}

/// <summary>
/// Individual search result
/// </summary>
public record SearchResultDto
{
    public Guid Id { get; init; }
    public SearchableContentType ContentType { get; init; }
    public Guid SourceId { get; init; }
    public string SourceType { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;
    public string? Summary { get; init; }
    public string? Content { get; init; }

    // Highlighted snippets (with search terms marked)
    public string? HighlightedTitle { get; init; }
    public string? HighlightedSummary { get; init; }
    public string? HighlightedContent { get; init; }

    public string? Category { get; init; }
    public List<string> Tags { get; init; } = new();

    public string? Author { get; init; }
    public Guid? AuthorId { get; init; }
    public string? AuthorAvatar { get; init; }

    public string? Department { get; init; }

    // Document-specific
    public string? FileType { get; init; }
    public long? FileSize { get; init; }
    public string? MimeType { get; init; }

    // Media-specific
    public string? ThumbnailUrl { get; init; }
    public int? Duration { get; init; }

    public DateTime? PublishedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
    public long ViewCount { get; init; }

    /// <summary>
    /// Relevance score (0-1)
    /// </summary>
    public double Score { get; init; }

    /// <summary>
    /// URL to view the content
    /// </summary>
    public string? Url { get; init; }
}

/// <summary>
/// Search facets for filtering
/// </summary>
public record SearchFacets
{
    public List<FacetItem> ContentTypes { get; init; } = new();
    public List<FacetItem> Categories { get; init; } = new();
    public List<FacetItem> Tags { get; init; } = new();
    public List<FacetItem> Authors { get; init; } = new();
    public List<FacetItem> Departments { get; init; } = new();
    public List<FacetItem> FileTypes { get; init; } = new();
    public DateRangeFacet? DateRange { get; init; }
}

/// <summary>
/// Individual facet item
/// </summary>
public record FacetItem
{
    public string Value { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public long Count { get; init; }
    public bool IsSelected { get; init; }
}

/// <summary>
/// Date range facet
/// </summary>
public record DateRangeFacet
{
    public DateTime? MinDate { get; init; }
    public DateTime? MaxDate { get; init; }
    public List<DateBucket> Buckets { get; init; } = new();
}

/// <summary>
/// Date bucket for histogram
/// </summary>
public record DateBucket
{
    public string Label { get; init; } = string.Empty;
    public DateTime From { get; init; }
    public DateTime To { get; init; }
    public long Count { get; init; }
}

/// <summary>
/// Autocomplete suggestion response
/// </summary>
public record SuggestResponse
{
    public string Query { get; init; } = string.Empty;
    public List<SuggestionItem> Suggestions { get; init; } = new();
}

/// <summary>
/// Individual suggestion item
/// </summary>
public record SuggestionItem
{
    public string Text { get; init; } = string.Empty;
    public string HighlightedText { get; init; } = string.Empty;
    public SuggestionType Type { get; init; }
    public SearchableContentType? ContentType { get; init; }
    public Guid? EntityId { get; init; }
    public string? Category { get; init; }
    public long Weight { get; init; }
}

/// <summary>
/// Saved search DTO
/// </summary>
public record SavedSearchDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Query { get; init; } = string.Empty;
    public List<SearchableContentType> ContentTypes { get; init; } = new();
    public string? FiltersJson { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
    public bool NotifyOnNewResults { get; init; }
    public NotificationFrequency NotificationFrequency { get; init; }
    public int ExecutionCount { get; init; }
    public DateTime? LastExecutedAt { get; init; }
    public int LastResultCount { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Search index status
/// </summary>
public record SearchIndexDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public SearchableContentType ContentType { get; init; }
    public IndexStatus Status { get; init; }
    public long DocumentCount { get; init; }
    public string SizeFormatted { get; init; } = string.Empty;
    public DateTime? LastIndexedAt { get; init; }
    public bool IsReindexing { get; init; }
    public int ReindexProgress { get; init; }
}

/// <summary>
/// Search analytics summary
/// </summary>
public record SearchAnalyticsDto
{
    public DateOnly? Date { get; init; }
    public DateTime PeriodStart { get; init; }
    public DateTime PeriodEnd { get; init; }

    public long TotalSearches { get; init; }
    public long UniqueUsers { get; init; }
    public long UniqueQueries { get; init; }

    public double AverageResultsPerSearch { get; init; }
    public double ZeroResultRate { get; init; }
    public double ClickThroughRate { get; init; }
    public double AverageExecutionTimeMs { get; init; }

    public List<TopQueryDto> TopQueries { get; init; } = new();
    public List<TopQueryDto> TopZeroResultQueries { get; init; } = new();
    public List<ContentTypeStatsDto> ContentTypeBreakdown { get; init; } = new();
    public List<TrendDataPoint> SearchTrend { get; init; } = new();
}

/// <summary>
/// Top query statistics
/// </summary>
public record TopQueryDto
{
    public string Query { get; init; } = string.Empty;
    public long SearchCount { get; init; }
    public long ClickCount { get; init; }
    public double ClickThroughRate { get; init; }
    public double AverageResults { get; init; }
}

/// <summary>
/// Content type statistics
/// </summary>
public record ContentTypeStatsDto
{
    public SearchableContentType ContentType { get; init; }
    public string ContentTypeName { get; init; } = string.Empty;
    public long SearchCount { get; init; }
    public double Percentage { get; init; }
}

/// <summary>
/// Trend data point for charts
/// </summary>
public record TrendDataPoint
{
    public DateOnly Date { get; init; }
    public long Value { get; init; }
}

/// <summary>
/// Response for indexing operations
/// </summary>
public record IndexOperationResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public Guid? DocumentId { get; init; }
    public int IndexedCount { get; init; }
    public int FailedCount { get; init; }
    public List<string> Errors { get; init; } = new();
}

/// <summary>
/// Reindex job status
/// </summary>
public record ReindexJobDto
{
    public Guid JobId { get; init; }
    public SearchableContentType? ContentType { get; init; }
    public string Status { get; init; } = string.Empty;
    public int Progress { get; init; }
    public int TotalDocuments { get; init; }
    public int ProcessedDocuments { get; init; }
    public DateTime StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public string? Error { get; init; }
}
