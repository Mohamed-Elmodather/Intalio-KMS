using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Search.Domain.Entities;

/// <summary>
/// Tracks individual search queries for analytics
/// </summary>
public class SearchQuery : Entity
{
    public string Query { get; private set; } = string.Empty;
    public string? NormalizedQuery { get; private set; }
    public string Language { get; private set; } = "en";
    public Guid? UserId { get; private set; }
    public string? SessionId { get; private set; }

    // Search context
    public List<SearchableContentType> ContentTypes { get; private set; } = new();
    public string? FiltersJson { get; private set; }
    public int Page { get; private set; } = 1;
    public int PageSize { get; private set; } = 20;

    // Results
    public int TotalResults { get; private set; }
    public int ResultsReturned { get; private set; }
    public long ExecutionTimeMs { get; private set; }
    public bool HasResults => TotalResults > 0;

    // User interaction
    public int ClickedResultPosition { get; private set; }
    public Guid? ClickedDocumentId { get; private set; }
    public DateTime? ClickedAt { get; private set; }
    public bool WasSuccessful { get; private set; }

    // Timestamp
    public DateTime SearchedAt { get; private set; }

    private SearchQuery() { }

    public static SearchQuery Create(
        string query,
        string language,
        Guid? userId,
        string? sessionId,
        List<SearchableContentType> contentTypes)
    {
        return new SearchQuery
        {
            Id = Guid.NewGuid(),
            Query = query,
            NormalizedQuery = NormalizeQuery(query),
            Language = language,
            UserId = userId,
            SessionId = sessionId,
            ContentTypes = contentTypes,
            SearchedAt = DateTime.UtcNow
        };
    }

    public void SetResults(int totalResults, int resultsReturned, long executionTimeMs)
    {
        TotalResults = totalResults;
        ResultsReturned = resultsReturned;
        ExecutionTimeMs = executionTimeMs;
    }

    public void RecordClick(int position, Guid documentId)
    {
        ClickedResultPosition = position;
        ClickedDocumentId = documentId;
        ClickedAt = DateTime.UtcNow;
        WasSuccessful = true;
    }

    public void MarkAsSuccessful()
    {
        WasSuccessful = true;
    }

    private static string NormalizeQuery(string query)
    {
        return query.ToLowerInvariant().Trim();
    }
}

/// <summary>
/// Aggregated search term statistics
/// </summary>
public class SearchTermStats : Entity
{
    public string Term { get; private set; } = string.Empty;
    public string NormalizedTerm { get; private set; } = string.Empty;
    public string Language { get; private set; } = "en";

    public long SearchCount { get; private set; }
    public long ClickCount { get; private set; }
    public double ClickThroughRate => SearchCount > 0 ? (double)ClickCount / SearchCount : 0;

    public long TotalResultsSum { get; private set; }
    public double AverageResults => SearchCount > 0 ? (double)TotalResultsSum / SearchCount : 0;

    public int ZeroResultCount { get; private set; }
    public double ZeroResultRate => SearchCount > 0 ? (double)ZeroResultCount / SearchCount : 0;

    public DateTime FirstSearchedAt { get; private set; }
    public DateTime LastSearchedAt { get; private set; }

    // Trending calculation
    public long SearchCountLast24h { get; private set; }
    public long SearchCountLast7d { get; private set; }
    public long SearchCountLast30d { get; private set; }
    public double TrendScore { get; private set; }

    private SearchTermStats() { }

    public static SearchTermStats Create(string term, string language)
    {
        var normalized = term.ToLowerInvariant().Trim();
        return new SearchTermStats
        {
            Id = Guid.NewGuid(),
            Term = term,
            NormalizedTerm = normalized,
            Language = language,
            FirstSearchedAt = DateTime.UtcNow,
            LastSearchedAt = DateTime.UtcNow
        };
    }

    public void RecordSearch(int resultCount)
    {
        SearchCount++;
        TotalResultsSum += resultCount;
        LastSearchedAt = DateTime.UtcNow;
        if (resultCount == 0) ZeroResultCount++;
    }

    public void RecordClick()
    {
        ClickCount++;
    }

    public void UpdateTrendingStats(long last24h, long last7d, long last30d)
    {
        SearchCountLast24h = last24h;
        SearchCountLast7d = last7d;
        SearchCountLast30d = last30d;

        // Calculate trend score (higher weight for recent searches)
        TrendScore = (last24h * 10) + (last7d * 3) + last30d;
    }
}

/// <summary>
/// Search suggestions based on popular queries
/// </summary>
public class SearchSuggestion : Entity
{
    public string Suggestion { get; private set; } = string.Empty;
    public string NormalizedSuggestion { get; private set; } = string.Empty;
    public string Language { get; private set; } = "en";
    public SuggestionType Type { get; private set; }

    public long Weight { get; private set; } = 1;
    public bool IsActive { get; private set; } = true;
    public bool IsCurated { get; private set; }

    // Related content
    public SearchableContentType? RelatedContentType { get; private set; }
    public Guid? RelatedEntityId { get; private set; }

    private SearchSuggestion() { }

    public static SearchSuggestion Create(
        string suggestion,
        string language,
        SuggestionType type)
    {
        return new SearchSuggestion
        {
            Id = Guid.NewGuid(),
            Suggestion = suggestion,
            NormalizedSuggestion = suggestion.ToLowerInvariant().Trim(),
            Language = language,
            Type = type
        };
    }

    public static SearchSuggestion CreateCurated(
        string suggestion,
        string language,
        SuggestionType type,
        long weight)
    {
        return new SearchSuggestion
        {
            Id = Guid.NewGuid(),
            Suggestion = suggestion,
            NormalizedSuggestion = suggestion.ToLowerInvariant().Trim(),
            Language = language,
            Type = type,
            Weight = weight,
            IsCurated = true
        };
    }

    public void IncrementWeight(int amount = 1)
    {
        Weight += amount;
    }

    public void SetWeight(long weight)
    {
        Weight = Math.Max(0, weight);
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public void SetRelatedEntity(SearchableContentType contentType, Guid entityId)
    {
        RelatedContentType = contentType;
        RelatedEntityId = entityId;
    }
}

public enum SuggestionType
{
    Query,      // Based on popular search queries
    Term,       // Based on indexed terms
    Entity,     // Entity name (person, place, etc.)
    Category,   // Category name
    Tag,        // Tag name
    Curated     // Manually curated suggestion
}

/// <summary>
/// Daily search analytics aggregate
/// </summary>
public class DailySearchStats : Entity
{
    public DateOnly Date { get; private set; }

    public long TotalSearches { get; private set; }
    public long UniqueUsers { get; private set; }
    public long UniqueQueries { get; private set; }

    public long TotalResults { get; private set; }
    public long ZeroResultSearches { get; private set; }
    public double ZeroResultRate => TotalSearches > 0 ? (double)ZeroResultSearches / TotalSearches : 0;

    public long TotalClicks { get; private set; }
    public double OverallCTR => TotalSearches > 0 ? (double)TotalClicks / TotalSearches : 0;

    public double AverageExecutionTimeMs { get; private set; }
    public double AverageResultsPerSearch => TotalSearches > 0 ? (double)TotalResults / TotalSearches : 0;

    // By content type
    public Dictionary<SearchableContentType, long> SearchesByContentType { get; private set; } = new();

    // Top searches
    public List<string> TopQueries { get; private set; } = new();
    public List<string> TopZeroResultQueries { get; private set; } = new();

    private DailySearchStats() { }

    public static DailySearchStats Create(DateOnly date)
    {
        return new DailySearchStats
        {
            Id = Guid.NewGuid(),
            Date = date
        };
    }

    public void UpdateStats(
        long totalSearches,
        long uniqueUsers,
        long uniqueQueries,
        long totalResults,
        long zeroResultSearches,
        long totalClicks,
        double avgExecutionTimeMs)
    {
        TotalSearches = totalSearches;
        UniqueUsers = uniqueUsers;
        UniqueQueries = uniqueQueries;
        TotalResults = totalResults;
        ZeroResultSearches = zeroResultSearches;
        TotalClicks = totalClicks;
        AverageExecutionTimeMs = avgExecutionTimeMs;
    }

    public void SetTopQueries(List<string> queries, List<string> zeroResultQueries)
    {
        TopQueries = queries;
        TopZeroResultQueries = zeroResultQueries;
    }

    public void SetContentTypeBreakdown(Dictionary<SearchableContentType, long> breakdown)
    {
        SearchesByContentType = breakdown;
    }
}
