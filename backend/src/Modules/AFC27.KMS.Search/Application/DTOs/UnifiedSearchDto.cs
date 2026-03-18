using AFC27.KMS.Search.Domain.Entities;

namespace AFC27.KMS.Search.Application.DTOs;

/// <summary>
/// Request for unified search with optional AI-generated answer.
/// </summary>
public record UnifiedSearchRequest
{
    /// <summary>
    /// The search query string.
    /// </summary>
    public string Query { get; init; } = string.Empty;

    /// <summary>
    /// Language preference ("en" or "ar").
    /// </summary>
    public string Language { get; init; } = "en";

    /// <summary>
    /// Whether to include an AI-generated answer alongside search results.
    /// </summary>
    public bool IncludeAIAnswer { get; init; } = false;

    /// <summary>
    /// Content types to search (empty = all).
    /// </summary>
    public List<SearchableContentType> ContentTypes { get; init; } = new();

    /// <summary>
    /// Filter by category.
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Filter by tags (any match).
    /// </summary>
    public List<string> Tags { get; init; } = new();

    /// <summary>
    /// Filter by date range start.
    /// </summary>
    public DateTime? DateFrom { get; init; }

    /// <summary>
    /// Filter by date range end.
    /// </summary>
    public DateTime? DateTo { get; init; }

    /// <summary>
    /// Sort field (relevance, date, title, views).
    /// </summary>
    public string SortBy { get; init; } = "relevance";

    /// <summary>
    /// Sort direction.
    /// </summary>
    public SortDirection SortDirection { get; init; } = SortDirection.Descending;

    /// <summary>
    /// Page number (1-based).
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Results per page.
    /// </summary>
    public int PageSize { get; init; } = 20;

    /// <summary>
    /// Include facets in response.
    /// </summary>
    public bool IncludeFacets { get; init; } = true;

    /// <summary>
    /// Enable fuzzy matching for typo tolerance.
    /// </summary>
    public bool FuzzyMatching { get; init; } = true;

    /// <summary>
    /// Session ID for analytics tracking.
    /// </summary>
    public string? SessionId { get; init; }
}

/// <summary>
/// Response for unified search, containing both search results and an optional AI answer.
/// </summary>
public record UnifiedSearchResponse
{
    /// <summary>
    /// Original search query.
    /// </summary>
    public string Query { get; init; } = string.Empty;

    /// <summary>
    /// AI-generated answer, if requested and available.
    /// </summary>
    public AIAnswerDto? AIAnswer { get; init; }

    /// <summary>
    /// Traditional search results.
    /// </summary>
    public List<SearchResultDto> SearchResults { get; init; } = new();

    /// <summary>
    /// Total number of matching results.
    /// </summary>
    public long TotalResults { get; init; }

    /// <summary>
    /// Current page number.
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Results per page.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Total number of pages.
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalResults / PageSize) : 0;

    /// <summary>
    /// Search execution time in milliseconds.
    /// </summary>
    public long ExecutionTimeMs { get; init; }

    /// <summary>
    /// Facets for filtering.
    /// </summary>
    public SearchFacets? Facets { get; init; }

    /// <summary>
    /// Spelling correction suggestion.
    /// </summary>
    public string? SpellingSuggestion { get; init; }

    /// <summary>
    /// Related searches.
    /// </summary>
    public List<string> RelatedSearches { get; init; } = new();
}

/// <summary>
/// AI-generated answer for a search query, powered by RAG.
/// </summary>
public record AIAnswerDto
{
    /// <summary>
    /// The AI-generated answer text.
    /// </summary>
    public string Answer { get; init; } = string.Empty;

    /// <summary>
    /// Confidence score for the answer (0.0 to 1.0).
    /// </summary>
    public double Confidence { get; init; }

    /// <summary>
    /// Source citations that support the answer.
    /// </summary>
    public IReadOnlyList<AnswerCitationDto> Citations { get; init; } = new List<AnswerCitationDto>();

    /// <summary>
    /// Suggested follow-up questions.
    /// </summary>
    public IReadOnlyList<string> FollowUpQuestions { get; init; } = new List<string>();

    /// <summary>
    /// Processing time for the AI answer in milliseconds.
    /// </summary>
    public int ProcessingTimeMs { get; init; }

    /// <summary>
    /// Number of tokens used.
    /// </summary>
    public int TokensUsed { get; init; }
}

/// <summary>
/// A citation reference from an AI-generated answer.
/// </summary>
public record AnswerCitationDto
{
    /// <summary>
    /// The source document ID.
    /// </summary>
    public Guid DocumentId { get; init; }

    /// <summary>
    /// The title of the cited source.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// The relevant snippet from the source.
    /// </summary>
    public string Snippet { get; init; } = string.Empty;

    /// <summary>
    /// URL to the source document.
    /// </summary>
    public string? Url { get; init; }

    /// <summary>
    /// Relevance score for this citation.
    /// </summary>
    public double RelevanceScore { get; init; }

    /// <summary>
    /// Content type of the cited source.
    /// </summary>
    public SearchableContentType ContentType { get; init; }
}
