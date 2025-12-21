using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AFC27.KMS.Search;

/// <summary>
/// Search module registration and configuration
/// </summary>
public static class SearchModule
{
    /// <summary>
    /// Add Search module services to DI container
    /// </summary>
    public static IServiceCollection AddSearchModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure Search options
        services.Configure<SearchOptions>(
            configuration.GetSection("Search"));

        // Configure Elasticsearch options
        services.Configure<ElasticsearchOptions>(
            configuration.GetSection("Search:Elasticsearch"));

        // TODO: Register Elasticsearch client
        // services.AddSingleton<IElasticClient>(provider =>
        // {
        //     var options = provider.GetRequiredService<IOptions<ElasticsearchOptions>>().Value;
        //     var settings = new ConnectionSettings(new Uri(options.Url))
        //         .DefaultIndex(options.DefaultIndex)
        //         .EnableApiVersioningHeader();
        //
        //     if (!string.IsNullOrEmpty(options.Username))
        //     {
        //         settings.BasicAuthentication(options.Username, options.Password);
        //     }
        //
        //     return new ElasticClient(settings);
        // });

        // TODO: Register search services
        // services.AddScoped<ISearchService, ElasticsearchSearchService>();
        // services.AddScoped<IIndexingService, ElasticsearchIndexingService>();
        // services.AddScoped<ISuggestionService, ElasticsearchSuggestionService>();
        // services.AddScoped<ISearchAnalyticsService, SearchAnalyticsService>();

        // TODO: Register repositories
        // services.AddScoped<ISavedSearchRepository, SavedSearchRepository>();
        // services.AddScoped<ISearchQueryRepository, SearchQueryRepository>();
        // services.AddScoped<ISearchTermStatsRepository, SearchTermStatsRepository>();

        // Configure authorization policies
        services.AddAuthorizationBuilder()
            .AddPolicy("CanManageSearch", policy =>
                policy.RequireClaim("permission", "search:manage"))
            .AddPolicy("CanViewSearchAnalytics", policy =>
                policy.RequireClaim("permission", "search:analytics"));

        return services;
    }
}

/// <summary>
/// Search module configuration options
/// </summary>
public class SearchOptions
{
    /// <summary>
    /// Enable search functionality
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Minimum query length for search
    /// </summary>
    public int MinQueryLength { get; set; } = 2;

    /// <summary>
    /// Maximum results per page
    /// </summary>
    public int MaxPageSize { get; set; } = 100;

    /// <summary>
    /// Default page size
    /// </summary>
    public int DefaultPageSize { get; set; } = 20;

    /// <summary>
    /// Enable fuzzy matching by default
    /// </summary>
    public bool DefaultFuzzyMatching { get; set; } = true;

    /// <summary>
    /// Fuzziness level (AUTO, 0, 1, 2)
    /// </summary>
    public string Fuzziness { get; set; } = "AUTO";

    /// <summary>
    /// Enable highlighting by default
    /// </summary>
    public bool DefaultHighlighting { get; set; } = true;

    /// <summary>
    /// Maximum snippet length for highlights
    /// </summary>
    public int HighlightFragmentSize { get; set; } = 150;

    /// <summary>
    /// Number of highlight fragments
    /// </summary>
    public int HighlightNumberOfFragments { get; set; } = 3;

    /// <summary>
    /// Highlight pre-tag
    /// </summary>
    public string HighlightPreTag { get; set; } = "<mark>";

    /// <summary>
    /// Highlight post-tag
    /// </summary>
    public string HighlightPostTag { get; set; } = "</mark>";

    /// <summary>
    /// Enable search analytics tracking
    /// </summary>
    public bool EnableAnalytics { get; set; } = true;

    /// <summary>
    /// Days to retain search query history
    /// </summary>
    public int QueryRetentionDays { get; set; } = 90;

    /// <summary>
    /// Enable spelling suggestions
    /// </summary>
    public bool EnableSpellingSuggestions { get; set; } = true;

    /// <summary>
    /// Cache duration for search results (seconds)
    /// </summary>
    public int CacheDurationSeconds { get; set; } = 60;

    /// <summary>
    /// Index refresh interval
    /// </summary>
    public string IndexRefreshInterval { get; set; } = "1s";
}

/// <summary>
/// Elasticsearch connection options
/// </summary>
public class ElasticsearchOptions
{
    /// <summary>
    /// Elasticsearch server URL
    /// </summary>
    public string Url { get; set; } = "http://localhost:9200";

    /// <summary>
    /// Elasticsearch username (optional)
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Elasticsearch password (optional)
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Default index name prefix
    /// </summary>
    public string IndexPrefix { get; set; } = "kms";

    /// <summary>
    /// Default index name
    /// </summary>
    public string DefaultIndex { get; set; } = "kms_content";

    /// <summary>
    /// Number of shards for new indices
    /// </summary>
    public int NumberOfShards { get; set; } = 1;

    /// <summary>
    /// Number of replicas for new indices
    /// </summary>
    public int NumberOfReplicas { get; set; } = 1;

    /// <summary>
    /// Connection timeout in seconds
    /// </summary>
    public int ConnectionTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Enable debug mode (logs all requests)
    /// </summary>
    public bool DebugMode { get; set; } = false;

    /// <summary>
    /// Enable SSL certificate validation
    /// </summary>
    public bool ValidateCertificates { get; set; } = true;

    /// <summary>
    /// Maximum retry attempts
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Bulk indexing batch size
    /// </summary>
    public int BulkIndexBatchSize { get; set; } = 1000;
}

/// <summary>
/// Index names for different content types
/// </summary>
public static class SearchIndices
{
    public const string Articles = "kms_articles";
    public const string Documents = "kms_documents";
    public const string Media = "kms_media";
    public const string Users = "kms_users";
    public const string Communities = "kms_communities";
    public const string Discussions = "kms_discussions";
    public const string LessonsLearned = "kms_lessons_learned";
    public const string Events = "kms_events";
    public const string Services = "kms_services";
    public const string Suggestions = "kms_suggestions";
}
