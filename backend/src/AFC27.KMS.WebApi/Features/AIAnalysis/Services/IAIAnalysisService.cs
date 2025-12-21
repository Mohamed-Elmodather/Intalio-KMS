using AFC27.KMS.WebApi.Features.AIAnalysis.Models;

namespace AFC27.KMS.WebApi.Features.AIAnalysis.Services;

/// <summary>
/// Interface for AI-powered document analysis service
/// </summary>
public interface IAIAnalysisService
{
    // Document Analysis
    Task<DocumentAnalysis> AnalyzeDocumentAsync(Guid documentId, AnalyzeDocumentRequest request, CancellationToken cancellationToken = default);
    Task<DocumentAnalysis?> GetDocumentAnalysisAsync(Guid documentId, CancellationToken cancellationToken = default);
    Task<List<DocumentAnalysis>> GetRecentAnalysesAsync(int count, CancellationToken cancellationToken = default);

    // Entity Extraction
    Task<List<ExtractedEntity>> GetDocumentEntitiesAsync(Guid documentId, EntityType? type, CancellationToken cancellationToken = default);
    Task<List<ExtractedEntity>> ExtractEntitiesFromTextAsync(string text, CancellationToken cancellationToken = default);

    // Sentiment Analysis
    Task<SentimentAnalysis> AnalyzeSentimentAsync(Guid contentId, ContentType contentType, string text, CancellationToken cancellationToken = default);
    Task<SentimentAnalysis?> GetSentimentAsync(Guid contentId, CancellationToken cancellationToken = default);

    // Tag Suggestions
    Task<List<TagSuggestion>> GetTagSuggestionsAsync(Guid documentId, CancellationToken cancellationToken = default);
    Task<List<TagSuggestion>> SuggestTagsFromTextAsync(string text, int maxTags, CancellationToken cancellationToken = default);
    Task ApplyTagSuggestionsAsync(Guid documentId, List<string> tags, CancellationToken cancellationToken = default);

    // Summarization
    Task<string> GenerateSummaryAsync(Guid documentId, int? maxLength, string? targetLanguage, CancellationToken cancellationToken = default);
    Task<string> SummarizeTextAsync(string text, int? maxLength, CancellationToken cancellationToken = default);

    // Semantic Search
    Task<List<SemanticSearchResult>> SemanticSearchAsync(SemanticSearchRequest request, CancellationToken cancellationToken = default);
    Task<List<SimilarContent>> FindSimilarContentAsync(Guid contentId, ContentType contentType, int maxResults, CancellationToken cancellationToken = default);

    // Transcription
    Task<TranscriptionResult> StartTranscriptionAsync(TranscribeRequest request, CancellationToken cancellationToken = default);
    Task<TranscriptionResult?> GetTranscriptionStatusAsync(Guid transcriptionId, CancellationToken cancellationToken = default);
    Task<TranscriptionResult?> GetTranscriptionBySourceAsync(Guid sourceId, TranscriptionSourceType sourceType, CancellationToken cancellationToken = default);

    // Batch Processing
    Task<Guid> StartBatchAnalysisAsync(BatchAnalysisRequest request, CancellationToken cancellationToken = default);
    Task<BatchAnalysisStatus?> GetBatchStatusAsync(Guid batchId, CancellationToken cancellationToken = default);

    // Recommendations
    Task<List<ContentRecommendation>> GetRecommendationsAsync(Guid userId, int maxResults, CancellationToken cancellationToken = default);
    Task<List<ContentRecommendation>> GetContentBasedRecommendationsAsync(Guid contentId, ContentType contentType, int maxResults, CancellationToken cancellationToken = default);

    // Dashboard
    Task<AIInsightsDashboard> GetInsightsDashboardAsync(CancellationToken cancellationToken = default);
}
