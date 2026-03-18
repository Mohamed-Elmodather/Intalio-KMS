using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AFC27.KMS.AI.Application.Services;
using AFC27.KMS.AI.Domain.Entities;

namespace AFC27.KMS.AI;

/// <summary>
/// AI module configuration
/// </summary>
public static class AIModule
{
    /// <summary>
    /// Add AI module services
    /// </summary>
    public static IServiceCollection AddAIModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure options
        services.Configure<AIOptions>(configuration.GetSection("AI"));
        services.Configure<IntalioAIOptions>(configuration.GetSection("AI:Intalio"));
        services.Configure<AzureOpenAIOptions>(configuration.GetSection("AI:AzureOpenAI"));
        services.Configure<AzureCognitiveServicesOptions>(configuration.GetSection("AI:AzureCognitiveServices"));
        services.Configure<SemanticSearchOptions>(configuration.GetSection("AI:SemanticSearch"));
        services.Configure<QuotaOptions>(configuration.GetSection("AI:Quota"));

        // Register writing assistant service
        services.AddScoped<IWritingAssistantService, WritingAssistantService>();

        // Register services
        // services.AddScoped<IAIJobService, AIJobService>();
        // services.AddScoped<ITranscriptionService, TranscriptionService>();
        // services.AddScoped<ISummarizationService, SummarizationService>();
        // services.AddScoped<ISemanticSearchService, SemanticSearchService>();
        // services.AddScoped<IQAService, QAService>();
        // services.AddScoped<IContentClassificationService, ContentClassificationService>();
        // services.AddScoped<IKnowledgeGraphService, KnowledgeGraphService>();

        // Register AI providers
        // services.AddScoped<IAIProvider, IntalioAIProvider>();
        // services.AddScoped<IAIProvider, AzureOpenAIProvider>();
        // services.AddScoped<IAIProvider, AzureCognitiveServicesProvider>();

        // Register repositories
        // services.AddScoped<IAIJobRepository, AIJobRepository>();
        // services.AddScoped<ITranscriptionRepository, TranscriptionRepository>();
        // services.AddScoped<ISummaryRepository, SummaryRepository>();
        // services.AddScoped<IVectorEmbeddingRepository, VectorEmbeddingRepository>();
        // services.AddScoped<IPromptTemplateRepository, PromptTemplateRepository>();

        // Register background services
        // services.AddHostedService<AIJobProcessorService>();
        // services.AddHostedService<VectorIndexingService>();

        // Configure authorization policies
        services.AddAuthorizationBuilder()
            .AddPolicy("AIAdmin", policy =>
                policy.RequireRole("Admin", "AIAdmin"));

        return services;
    }
}

/// <summary>
/// AI module options
/// </summary>
public class AIOptions
{
    /// <summary>
    /// Enable AI features
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Default AI provider
    /// </summary>
    public AIProvider DefaultProvider { get; set; } = AIProvider.IntalioAI;

    /// <summary>
    /// Maximum concurrent jobs per user
    /// </summary>
    public int MaxConcurrentJobsPerUser { get; set; } = 5;

    /// <summary>
    /// Job timeout in minutes
    /// </summary>
    public int JobTimeoutMinutes { get; set; } = 30;

    /// <summary>
    /// Maximum retry attempts for failed jobs
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Retry delay in seconds
    /// </summary>
    public int RetryDelaySeconds { get; set; } = 60;

    /// <summary>
    /// Job result retention days
    /// </summary>
    public int ResultRetentionDays { get; set; } = 90;

    /// <summary>
    /// Enable job approval workflow for sensitive operations
    /// </summary>
    public bool RequireApprovalForBroadcast { get; set; } = true;

    /// <summary>
    /// Supported languages
    /// </summary>
    public List<string> SupportedLanguages { get; set; } = new() { "ar", "en", "fr" };

    /// <summary>
    /// Default output language
    /// </summary>
    public string DefaultLanguage { get; set; } = "en";
}

/// <summary>
/// Intalio AI Engine options
/// </summary>
public class IntalioAIOptions
{
    /// <summary>
    /// Enable Intalio AI provider
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Intalio AI API endpoint
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// API key for authentication
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// API version
    /// </summary>
    public string ApiVersion { get; set; } = "v1";

    /// <summary>
    /// Tenant ID for multi-tenant environments
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Request timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 120;

    /// <summary>
    /// Enable Arabic language optimization
    /// </summary>
    public bool ArabicOptimization { get; set; } = true;

    /// <summary>
    /// Transcription model ID
    /// </summary>
    public string TranscriptionModel { get; set; } = "intalio-transcribe-v1";

    /// <summary>
    /// Summarization model ID
    /// </summary>
    public string SummarizationModel { get; set; } = "intalio-summarize-v1";

    /// <summary>
    /// Classification model ID
    /// </summary>
    public string ClassificationModel { get; set; } = "intalio-classify-v1";

    /// <summary>
    /// Embedding model ID
    /// </summary>
    public string EmbeddingModel { get; set; } = "intalio-embed-v1";

    /// <summary>
    /// Generation model ID
    /// </summary>
    public string GenerationModel { get; set; } = "intalio-generate-v1";
}

/// <summary>
/// Azure OpenAI options
/// </summary>
public class AzureOpenAIOptions
{
    /// <summary>
    /// Enable Azure OpenAI provider
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Azure OpenAI endpoint
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// API key
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// API version
    /// </summary>
    public string ApiVersion { get; set; } = "2024-02-01";

    /// <summary>
    /// GPT deployment name
    /// </summary>
    public string GptDeployment { get; set; } = "gpt-4";

    /// <summary>
    /// Embedding deployment name
    /// </summary>
    public string EmbeddingDeployment { get; set; } = "text-embedding-ada-002";

    /// <summary>
    /// Whisper deployment name (for transcription)
    /// </summary>
    public string WhisperDeployment { get; set; } = "whisper";

    /// <summary>
    /// Max tokens for completion
    /// </summary>
    public int MaxTokens { get; set; } = 4096;

    /// <summary>
    /// Temperature for generation
    /// </summary>
    public double Temperature { get; set; } = 0.7;
}

/// <summary>
/// Azure Cognitive Services options
/// </summary>
public class AzureCognitiveServicesOptions
{
    /// <summary>
    /// Enable Azure Cognitive Services
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Speech service endpoint
    /// </summary>
    public string SpeechEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Speech service key
    /// </summary>
    public string SpeechKey { get; set; } = string.Empty;

    /// <summary>
    /// Speech region
    /// </summary>
    public string SpeechRegion { get; set; } = string.Empty;

    /// <summary>
    /// Language service endpoint
    /// </summary>
    public string LanguageEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Language service key
    /// </summary>
    public string LanguageKey { get; set; } = string.Empty;

    /// <summary>
    /// Translator endpoint
    /// </summary>
    public string TranslatorEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Translator key
    /// </summary>
    public string TranslatorKey { get; set; } = string.Empty;

    /// <summary>
    /// Form recognizer endpoint
    /// </summary>
    public string FormRecognizerEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Form recognizer key
    /// </summary>
    public string FormRecognizerKey { get; set; } = string.Empty;
}

/// <summary>
/// Semantic search options
/// </summary>
public class SemanticSearchOptions
{
    /// <summary>
    /// Enable semantic search
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Vector database provider (Elasticsearch, Pinecone, Qdrant, Milvus)
    /// </summary>
    public string VectorProvider { get; set; } = "Elasticsearch";

    /// <summary>
    /// Elasticsearch endpoint for vectors
    /// </summary>
    public string ElasticsearchEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Vector index name
    /// </summary>
    public string VectorIndexName { get; set; } = "kms-vectors";

    /// <summary>
    /// Embedding dimensions
    /// </summary>
    public int EmbeddingDimensions { get; set; } = 1536;

    /// <summary>
    /// Chunk size for document splitting
    /// </summary>
    public int ChunkSize { get; set; } = 500;

    /// <summary>
    /// Chunk overlap
    /// </summary>
    public int ChunkOverlap { get; set; } = 50;

    /// <summary>
    /// Minimum similarity score for results
    /// </summary>
    public double MinSimilarityScore { get; set; } = 0.5;

    /// <summary>
    /// Maximum results per query
    /// </summary>
    public int MaxResults { get; set; } = 100;

    /// <summary>
    /// Enable hybrid search (vector + keyword)
    /// </summary>
    public bool EnableHybridSearch { get; set; } = true;

    /// <summary>
    /// Keyword search weight in hybrid mode
    /// </summary>
    public double KeywordWeight { get; set; } = 0.3;

    /// <summary>
    /// Vector search weight in hybrid mode
    /// </summary>
    public double VectorWeight { get; set; } = 0.7;

    /// <summary>
    /// Auto-index new content
    /// </summary>
    public bool AutoIndexContent { get; set; } = true;

    /// <summary>
    /// Content types to auto-index
    /// </summary>
    public List<string> AutoIndexTypes { get; set; } = new()
    {
        "Article", "Document", "Discussion", "LessonLearned", "Event"
    };
}

/// <summary>
/// AI quota options
/// </summary>
public class QuotaOptions
{
    /// <summary>
    /// Enable quota enforcement
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Default daily request limit
    /// </summary>
    public int DefaultDailyLimit { get; set; } = 100;

    /// <summary>
    /// Default monthly request limit
    /// </summary>
    public int DefaultMonthlyLimit { get; set; } = 2000;

    /// <summary>
    /// Default monthly token limit
    /// </summary>
    public int DefaultMonthlyTokenLimit { get; set; } = 100000;

    /// <summary>
    /// Default monthly transcription minutes
    /// </summary>
    public int DefaultMonthlyTranscriptionMinutes { get; set; } = 60;

    /// <summary>
    /// Warn at percentage
    /// </summary>
    public int WarnAtPercentage { get; set; } = 80;

    /// <summary>
    /// Block at percentage
    /// </summary>
    public int BlockAtPercentage { get; set; } = 100;

    /// <summary>
    /// Exempt roles from quota
    /// </summary>
    public List<string> ExemptRoles { get; set; } = new() { "Admin" };
}
