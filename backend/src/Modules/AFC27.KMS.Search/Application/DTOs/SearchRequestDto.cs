using AFC27.KMS.Search.Domain.Entities;

namespace AFC27.KMS.Search.Application.DTOs;

/// <summary>
/// Request DTO for global search
/// </summary>
public record SearchRequest
{
    /// <summary>
    /// The search query string
    /// </summary>
    public string Query { get; init; } = string.Empty;

    /// <summary>
    /// Language preference (en/ar)
    /// </summary>
    public string Language { get; init; } = "en";

    /// <summary>
    /// Content types to search (empty = all)
    /// </summary>
    public List<SearchableContentType> ContentTypes { get; init; } = new();

    /// <summary>
    /// Filter by category
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Filter by tags (any match)
    /// </summary>
    public List<string> Tags { get; init; } = new();

    /// <summary>
    /// Filter by author ID
    /// </summary>
    public Guid? AuthorId { get; init; }

    /// <summary>
    /// Filter by department ID
    /// </summary>
    public Guid? DepartmentId { get; init; }

    /// <summary>
    /// Filter by date range start
    /// </summary>
    public DateTime? DateFrom { get; init; }

    /// <summary>
    /// Filter by date range end
    /// </summary>
    public DateTime? DateTo { get; init; }

    /// <summary>
    /// Filter by file type (for documents)
    /// </summary>
    public string? FileType { get; init; }

    /// <summary>
    /// Sort field (relevance, date, title, views)
    /// </summary>
    public string SortBy { get; init; } = "relevance";

    /// <summary>
    /// Sort direction
    /// </summary>
    public SortDirection SortDirection { get; init; } = SortDirection.Descending;

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Results per page
    /// </summary>
    public int PageSize { get; init; } = 20;

    /// <summary>
    /// Include facets in response
    /// </summary>
    public bool IncludeFacets { get; init; } = true;

    /// <summary>
    /// Highlight search terms in results
    /// </summary>
    public bool HighlightResults { get; init; } = true;

    /// <summary>
    /// Enable fuzzy matching for typo tolerance
    /// </summary>
    public bool FuzzyMatching { get; init; } = true;

    /// <summary>
    /// Session ID for analytics tracking
    /// </summary>
    public string? SessionId { get; init; }
}

/// <summary>
/// Request for autocomplete suggestions
/// </summary>
public record SuggestRequest
{
    public string Query { get; init; } = string.Empty;
    public string Language { get; init; } = "en";
    public List<SearchableContentType> ContentTypes { get; init; } = new();
    public int Limit { get; init; } = 10;
}

/// <summary>
/// Request for advanced search with field-specific queries
/// </summary>
public record AdvancedSearchRequest
{
    public string? TitleQuery { get; init; }
    public string? ContentQuery { get; init; }
    public string? AllFieldsQuery { get; init; }
    public string? ExactPhrase { get; init; }
    public List<string> MustIncludeTerms { get; init; } = new();
    public List<string> MustExcludeTerms { get; init; } = new();
    public string Language { get; init; } = "en";
    public List<SearchableContentType> ContentTypes { get; init; } = new();
    public List<string> Categories { get; init; } = new();
    public List<string> Tags { get; init; } = new();
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
    public string SortBy { get; init; } = "relevance";
    public SortDirection SortDirection { get; init; } = SortDirection.Descending;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

/// <summary>
/// Request to record a click on a search result
/// </summary>
public record RecordClickRequest
{
    public string Query { get; init; } = string.Empty;
    public Guid DocumentId { get; init; }
    public int Position { get; init; }
    public string? SessionId { get; init; }
}

/// <summary>
/// Request to save a search
/// </summary>
public record SaveSearchRequest
{
    public string Name { get; init; } = string.Empty;
    public string Query { get; init; } = string.Empty;
    public List<SearchableContentType> ContentTypes { get; init; } = new();
    public string? FiltersJson { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
    public bool NotifyOnNewResults { get; init; }
    public NotificationFrequency NotificationFrequency { get; init; } = NotificationFrequency.Daily;
}

/// <summary>
/// Request to update a saved search
/// </summary>
public record UpdateSavedSearchRequest
{
    public string Name { get; init; } = string.Empty;
    public string Query { get; init; } = string.Empty;
    public string? FiltersJson { get; init; }
    public bool NotifyOnNewResults { get; init; }
    public NotificationFrequency NotificationFrequency { get; init; } = NotificationFrequency.Daily;
}

/// <summary>
/// Request to index a document
/// </summary>
public record IndexDocumentRequest
{
    public SearchableContentType ContentType { get; init; }
    public Guid SourceId { get; init; }
    public string SourceType { get; init; } = string.Empty;
    public string TitleEn { get; init; } = string.Empty;
    public string TitleAr { get; init; } = string.Empty;
    public string? ContentEn { get; init; }
    public string? ContentAr { get; init; }
    public string? SummaryEn { get; init; }
    public string? SummaryAr { get; init; }
    public string? Category { get; init; }
    public List<string> Tags { get; init; } = new();
    public string? Author { get; init; }
    public Guid? AuthorId { get; init; }
    public string? Department { get; init; }
    public Guid? DepartmentId { get; init; }
    public string? FileType { get; init; }
    public long? FileSize { get; init; }
    public string? MimeType { get; init; }
    public DocumentVisibility Visibility { get; init; } = DocumentVisibility.Internal;
    public bool IsPublished { get; init; }
    public List<Guid> AllowedRoleIds { get; init; } = new();
    public List<Guid> AllowedGroupIds { get; init; } = new();
    public List<Guid> AllowedUserIds { get; init; } = new();
}

/// <summary>
/// Request to reindex content
/// </summary>
public record ReindexRequest
{
    public SearchableContentType? ContentType { get; init; }
    public bool FullReindex { get; init; }
    public DateTime? ModifiedSince { get; init; }
}

/// <summary>
/// Filter request for search analytics
/// </summary>
public record SearchAnalyticsFilter
{
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
    public SearchableContentType? ContentType { get; init; }
    public int TopCount { get; init; } = 10;
}
