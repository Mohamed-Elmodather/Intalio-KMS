namespace AFC27.KMS.WebApi.Features.AIAnalysis.Models;

/// <summary>
/// Document AI analysis result
/// </summary>
public class DocumentAnalysis
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public string DocumentTitle { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? SummaryAr { get; set; }
    public string DetectedLanguage { get; set; } = "en";
    public double SentimentScore { get; set; }
    public SentimentLabel SentimentLabel { get; set; }
    public List<string> SuggestedTags { get; set; } = new();
    public List<string> KeyPhrases { get; set; } = new();
    public List<ExtractedEntity> Entities { get; set; } = new();
    public List<string> Topics { get; set; } = new();
    public double ReadabilityScore { get; set; }
    public int WordCount { get; set; }
    public int EstimatedReadTimeMinutes { get; set; }
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
    public AnalysisStatus Status { get; set; } = AnalysisStatus.Completed;
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Extracted named entity
/// </summary>
public class ExtractedEntity
{
    public Guid Id { get; set; }
    public Guid? DocumentId { get; set; }
    public Guid? ArticleId { get; set; }
    public EntityType Type { get; set; }
    public string Value { get; set; } = string.Empty;
    public string? NormalizedValue { get; set; }
    public double Confidence { get; set; }
    public int StartOffset { get; set; }
    public int EndOffset { get; set; }
    public int OccurrenceCount { get; set; } = 1;
    public string? Context { get; set; }
}

/// <summary>
/// Content sentiment analysis result
/// </summary>
public class SentimentAnalysis
{
    public Guid Id { get; set; }
    public Guid ContentId { get; set; }
    public ContentType ContentType { get; set; }
    public double Score { get; set; }
    public SentimentLabel Label { get; set; }
    public double PositiveScore { get; set; }
    public double NeutralScore { get; set; }
    public double NegativeScore { get; set; }
    public List<SentimentSentence> Sentences { get; set; } = new();
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
}

public class SentimentSentence
{
    public int Index { get; set; }
    public string Text { get; set; } = string.Empty;
    public SentimentLabel Label { get; set; }
    public double Score { get; set; }
}

/// <summary>
/// Tag suggestion with confidence
/// </summary>
public class TagSuggestion
{
    public string Tag { get; set; } = string.Empty;
    public string? TagAr { get; set; }
    public double Confidence { get; set; }
    public string Source { get; set; } = "AI";
    public bool IsApplied { get; set; }
}

/// <summary>
/// Semantic search result
/// </summary>
public class SemanticSearchResult
{
    public Guid ContentId { get; set; }
    public ContentType ContentType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Snippet { get; set; }
    public double RelevanceScore { get; set; }
    public List<string> MatchedPhrases { get; set; } = new();
    public string? HighlightedContent { get; set; }
}

/// <summary>
/// Similar content recommendation
/// </summary>
public class SimilarContent
{
    public Guid ContentId { get; set; }
    public ContentType ContentType { get; set; }
    public string Title { get; set; } = string.Empty;
    public double SimilarityScore { get; set; }
    public List<string> CommonTopics { get; set; } = new();
    public List<string> CommonEntities { get; set; } = new();
}

/// <summary>
/// Transcription result
/// </summary>
public class TranscriptionResult
{
    public Guid Id { get; set; }
    public Guid SourceId { get; set; }
    public TranscriptionSourceType SourceType { get; set; }
    public string ExternalJobId { get; set; } = string.Empty;
    public TranscriptionStatus Status { get; set; }
    public string? TranscribedText { get; set; }
    public string? Language { get; set; }
    public double Confidence { get; set; }
    public TimeSpan Duration { get; set; }
    public List<TranscriptionSegment> Segments { get; set; } = new();
    public List<TranscriptionSpeaker> Speakers { get; set; } = new();
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
}

public class TranscriptionSegment
{
    public int Index { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? SpeakerId { get; set; }
    public double Confidence { get; set; }
}

public class TranscriptionSpeaker
{
    public string Id { get; set; } = string.Empty;
    public string? Label { get; set; }
    public TimeSpan TotalSpeakingTime { get; set; }
}

/// <summary>
/// AI-powered content recommendation
/// </summary>
public class ContentRecommendation
{
    public Guid ContentId { get; set; }
    public ContentType ContentType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Thumbnail { get; set; }
    public RecommendationType RecommendationType { get; set; }
    public double Score { get; set; }
    public string Reason { get; set; } = string.Empty;
}

// Enums
public enum SentimentLabel
{
    VeryNegative,
    Negative,
    Neutral,
    Positive,
    VeryPositive
}

public enum EntityType
{
    Person,
    Organization,
    Location,
    Date,
    Event,
    Product,
    Money,
    Percentage,
    PhoneNumber,
    Email,
    Url,
    Custom
}

public enum ContentType
{
    Document,
    Article,
    Comment,
    Meeting,
    Transcript
}

public enum AnalysisStatus
{
    Pending,
    Processing,
    Completed,
    Failed
}

public enum TranscriptionSourceType
{
    MeetingRecording,
    AudioFile,
    VideoFile
}

public enum TranscriptionStatus
{
    Pending,
    Processing,
    Completed,
    Failed
}

public enum RecommendationType
{
    Similar,
    Trending,
    RecentlyViewed,
    BasedOnInterests,
    Collaborative
}

// Request Models
public class AnalyzeDocumentRequest
{
    public bool ExtractEntities { get; set; } = true;
    public bool AnalyzeSentiment { get; set; } = true;
    public bool GenerateSummary { get; set; } = true;
    public bool SuggestTags { get; set; } = true;
    public bool ExtractKeyPhrases { get; set; } = true;
    public int? SummaryMaxLength { get; set; }
    public string? TargetLanguage { get; set; }
}

public class SemanticSearchRequest
{
    public string Query { get; set; } = string.Empty;
    public List<ContentType>? ContentTypes { get; set; }
    public int MaxResults { get; set; } = 20;
    public double MinRelevanceScore { get; set; } = 0.5;
    public Dictionary<string, string>? Filters { get; set; }
}

public class TranscribeRequest
{
    public Guid SourceId { get; set; }
    public TranscriptionSourceType SourceType { get; set; }
    public string? FileUrl { get; set; }
    public string? Language { get; set; }
    public bool IdentifySpeakers { get; set; } = true;
    public bool GenerateTimestamps { get; set; } = true;
}

public class BatchAnalysisRequest
{
    public List<Guid> DocumentIds { get; set; } = new();
    public AnalyzeDocumentRequest Options { get; set; } = new();
}

public class BatchAnalysisStatus
{
    public Guid BatchId { get; set; }
    public int TotalDocuments { get; set; }
    public int ProcessedDocuments { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public BatchStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<string> Errors { get; set; } = new();
}

public enum BatchStatus
{
    Queued,
    Processing,
    Completed,
    PartiallyCompleted,
    Failed
}

public class AIInsightsDashboard
{
    public int TotalDocumentsAnalyzed { get; set; }
    public int DocumentsPendingAnalysis { get; set; }
    public List<EntityTypeCount> TopEntityTypes { get; set; } = new();
    public List<TagCount> TopSuggestedTags { get; set; } = new();
    public SentimentDistribution SentimentDistribution { get; set; } = new();
    public List<string> TrendingTopics { get; set; } = new();
    public double AverageReadabilityScore { get; set; }
}

public class EntityTypeCount
{
    public EntityType Type { get; set; }
    public int Count { get; set; }
}

public class TagCount
{
    public string Tag { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class SentimentDistribution
{
    public int Positive { get; set; }
    public int Neutral { get; set; }
    public int Negative { get; set; }
}
