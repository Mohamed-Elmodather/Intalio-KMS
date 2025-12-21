using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.AI.Domain.Entities;

/// <summary>
/// AI processing job
/// </summary>
public class AIJob : AuditableEntity
{
    public AIJobType Type { get; set; }
    public AIJobStatus Status { get; set; }
    public AIProvider Provider { get; set; }
    public string? ModelId { get; set; }

    // Input
    public Guid? SourceEntityId { get; set; }
    public string? SourceEntityType { get; set; }
    public string? InputText { get; set; }
    public string? InputFileUrl { get; set; }
    public string? InputFileMimeType { get; set; }
    public long? InputFileSizeBytes { get; set; }
    public string? InputLanguage { get; set; }

    // Output
    public string? OutputText { get; set; }
    public string? OutputFileUrl { get; set; }
    public string? OutputLanguage { get; set; }
    public string? OutputFormat { get; set; }

    // Processing metadata
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? ProcessingTimeMs { get; set; }
    public int? TokensUsed { get; set; }
    public decimal? CostEstimate { get; set; }

    // Error handling
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }

    // Configuration
    public Dictionary<string, object> Parameters { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();

    // Relations
    public Guid RequestedById { get; set; }
    public Guid? ApprovedById { get; set; }

    // Callback
    public string? CallbackUrl { get; set; }
    public bool CallbackSent { get; set; }
}

/// <summary>
/// AI job types
/// </summary>
public enum AIJobType
{
    // Transcription
    AudioTranscription,
    VideoTranscription,
    LiveTranscription,

    // Summarization
    DocumentSummarization,
    MeetingSummarization,
    ArticleSummarization,
    EmailSummarization,

    // Generation
    MeetingMinutesGeneration,
    ReportGeneration,
    ContentGeneration,
    EmailDraftGeneration,
    TranslationGeneration,

    // Analysis
    SentimentAnalysis,
    EntityExtraction,
    KeywordExtraction,
    TopicClassification,
    ContentClassification,
    LanguageDetection,

    // Document Processing
    DocumentOCR,
    DocumentParsing,
    FormExtraction,
    TableExtraction,
    ImageAnalysis,

    // Search & Retrieval
    SemanticSearch,
    SimilaritySearch,
    QuestionAnswering,

    // Text Processing
    TextToSpeech,
    SpeechToText,
    Paraphrasing,
    GrammarCorrection,

    // Custom
    CustomPrompt
}

/// <summary>
/// AI job status
/// </summary>
public enum AIJobStatus
{
    Pending,
    Queued,
    Processing,
    Completed,
    Failed,
    Cancelled,
    RequiresApproval,
    Approved,
    Rejected
}

/// <summary>
/// AI provider
/// </summary>
public enum AIProvider
{
    IntalioAI,
    AzureOpenAI,
    OpenAI,
    AzureCognitiveServices,
    GoogleCloud,
    AWS,
    Custom
}

/// <summary>
/// Transcription result
/// </summary>
public class TranscriptionResult : AuditableEntity
{
    public Guid JobId { get; set; }
    public AIJob Job { get; set; } = null!;

    public Guid SourceMediaId { get; set; }
    public string? SourceMediaType { get; set; }
    public TimeSpan Duration { get; set; }

    public string FullText { get; set; } = string.Empty;
    public string? FullTextAr { get; set; }
    public string DetectedLanguage { get; set; } = "en";
    public double ConfidenceScore { get; set; }

    public List<TranscriptionSegment> Segments { get; set; } = new();
    public List<TranscriptionSpeaker> Speakers { get; set; } = new();

    public bool HasSpeakerDiarization { get; set; }
    public bool HasTimestamps { get; set; }
    public bool IsEdited { get; set; }
    public Guid? EditedById { get; set; }
    public DateTime? EditedAt { get; set; }
}

/// <summary>
/// Transcription segment with timestamp
/// </summary>
public class TranscriptionSegment
{
    public int SequenceNumber { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? SpeakerId { get; set; }
    public string? SpeakerName { get; set; }
    public double ConfidenceScore { get; set; }
    public List<TranscriptionWord>? Words { get; set; }
}

/// <summary>
/// Individual word with timing
/// </summary>
public class TranscriptionWord
{
    public string Word { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public double Confidence { get; set; }
}

/// <summary>
/// Speaker identification
/// </summary>
public class TranscriptionSpeaker
{
    public string SpeakerId { get; set; } = string.Empty;
    public string? SpeakerName { get; set; }
    public Guid? UserId { get; set; }
    public TimeSpan TotalSpeakingTime { get; set; }
    public int SegmentCount { get; set; }
}

/// <summary>
/// Document summary
/// </summary>
public class DocumentSummary : AuditableEntity
{
    public Guid JobId { get; set; }
    public AIJob Job { get; set; } = null!;

    public Guid SourceDocumentId { get; set; }
    public string? SourceDocumentType { get; set; }

    public LocalizedString Title { get; set; } = null!;
    public LocalizedString Summary { get; set; } = null!;
    public LocalizedString? ExecutiveSummary { get; set; }

    public List<string> KeyPoints { get; set; } = new();
    public List<string> KeyPointsAr { get; set; } = new();
    public List<string> Keywords { get; set; } = new();
    public List<ExtractedEntity> Entities { get; set; } = new();
    public List<string> Topics { get; set; } = new();

    public SummaryLength Length { get; set; }
    public int WordCount { get; set; }
    public int OriginalWordCount { get; set; }
    public double CompressionRatio { get; set; }

    public bool IsEdited { get; set; }
    public Guid? EditedById { get; set; }
    public DateTime? EditedAt { get; set; }
}

/// <summary>
/// Summary length options
/// </summary>
public enum SummaryLength
{
    Brief,      // 1-2 sentences
    Short,      // 3-5 sentences
    Medium,     // 1-2 paragraphs
    Long,       // 3-5 paragraphs
    Detailed    // Full summary
}

/// <summary>
/// Extracted entity from content
/// </summary>
public class ExtractedEntity
{
    public string Text { get; set; } = string.Empty;
    public EntityType Type { get; set; }
    public string? NormalizedValue { get; set; }
    public double ConfidenceScore { get; set; }
    public int StartPosition { get; set; }
    public int EndPosition { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Entity types for extraction
/// </summary>
public enum EntityType
{
    Person,
    Organization,
    Location,
    Date,
    Time,
    Money,
    Percentage,
    Email,
    Phone,
    URL,
    Event,
    Product,
    Quantity,
    Custom
}

/// <summary>
/// Meeting minutes generated by AI
/// </summary>
public class MeetingMinutes : AuditableEntity
{
    public Guid JobId { get; set; }
    public AIJob Job { get; set; } = null!;

    public Guid? SourceEventId { get; set; }
    public Guid? SourceRecordingId { get; set; }

    public LocalizedString Title { get; set; } = null!;
    public DateTime MeetingDate { get; set; }
    public TimeSpan Duration { get; set; }
    public string? Location { get; set; }

    public List<MeetingParticipant> Participants { get; set; } = new();
    public List<MeetingAgendaItem> AgendaItems { get; set; } = new();
    public List<MeetingActionItem> ActionItems { get; set; } = new();
    public List<MeetingDecision> Decisions { get; set; } = new();

    public LocalizedString? ExecutiveSummary { get; set; }
    public LocalizedString? DetailedNotes { get; set; }
    public string? NextMeetingDate { get; set; }

    public MeetingMinutesStatus Status { get; set; }
    public bool IsApproved { get; set; }
    public Guid? ApprovedById { get; set; }
    public DateTime? ApprovedAt { get; set; }
}

/// <summary>
/// Meeting minutes status
/// </summary>
public enum MeetingMinutesStatus
{
    Draft,
    PendingReview,
    Approved,
    Distributed
}

/// <summary>
/// Meeting participant
/// </summary>
public class MeetingParticipant
{
    public Guid? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public ParticipantRole ParticipantType { get; set; }
    public bool WasPresent { get; set; }
    public TimeSpan? JoinTime { get; set; }
    public TimeSpan? LeaveTime { get; set; }
}

/// <summary>
/// Participant role in meeting
/// </summary>
public enum ParticipantRole
{
    Organizer,
    Chair,
    Secretary,
    Attendee,
    Guest,
    Observer
}

/// <summary>
/// Meeting agenda item
/// </summary>
public class MeetingAgendaItem
{
    public int SequenceNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string? Discussion { get; set; }
    public string? DiscussionAr { get; set; }
    public string? Presenter { get; set; }
    public TimeSpan? Duration { get; set; }
    public AgendaItemStatus Status { get; set; }
}

/// <summary>
/// Agenda item status
/// </summary>
public enum AgendaItemStatus
{
    NotDiscussed,
    Discussed,
    Deferred,
    Tabled
}

/// <summary>
/// Meeting action item
/// </summary>
public class MeetingActionItem
{
    public string Description { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public Guid? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public DateTime? DueDate { get; set; }
    public ActionItemPriority Priority { get; set; }
    public ActionItemStatus Status { get; set; }
}

/// <summary>
/// Action item priority
/// </summary>
public enum ActionItemPriority
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Action item status
/// </summary>
public enum ActionItemStatus
{
    Open,
    InProgress,
    Completed,
    Cancelled
}

/// <summary>
/// Meeting decision
/// </summary>
public class MeetingDecision
{
    public string Description { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? Context { get; set; }
    public List<string>? ApprovedBy { get; set; }
    public DateTime DecisionDate { get; set; }
}

/// <summary>
/// Content classification result
/// </summary>
public class ContentClassification : AuditableEntity
{
    public Guid JobId { get; set; }
    public AIJob Job { get; set; } = null!;

    public Guid SourceEntityId { get; set; }
    public string SourceEntityType { get; set; } = string.Empty;

    public List<ClassificationLabel> Labels { get; set; } = new();
    public List<string> SuggestedCategories { get; set; } = new();
    public List<string> SuggestedTags { get; set; } = new();
    public string? DetectedLanguage { get; set; }
    public SentimentResult? Sentiment { get; set; }
    public ContentSafetyResult? ContentSafety { get; set; }
}

/// <summary>
/// Classification label with confidence
/// </summary>
public class ClassificationLabel
{
    public string Label { get; set; } = string.Empty;
    public string? LabelAr { get; set; }
    public double ConfidenceScore { get; set; }
    public string? Category { get; set; }
}

/// <summary>
/// Sentiment analysis result
/// </summary>
public class SentimentResult
{
    public SentimentType Overall { get; set; }
    public double PositiveScore { get; set; }
    public double NeutralScore { get; set; }
    public double NegativeScore { get; set; }
    public double ConfidenceScore { get; set; }
    public List<SentimentSegment>? Segments { get; set; }
}

/// <summary>
/// Sentiment type
/// </summary>
public enum SentimentType
{
    VeryNegative,
    Negative,
    Neutral,
    Positive,
    VeryPositive,
    Mixed
}

/// <summary>
/// Sentiment for a text segment
/// </summary>
public class SentimentSegment
{
    public string Text { get; set; } = string.Empty;
    public SentimentType Sentiment { get; set; }
    public double Score { get; set; }
    public int StartPosition { get; set; }
    public int EndPosition { get; set; }
}

/// <summary>
/// Content safety analysis
/// </summary>
public class ContentSafetyResult
{
    public bool IsSafe { get; set; }
    public double HateSpeechScore { get; set; }
    public double ViolenceScore { get; set; }
    public double SexualContentScore { get; set; }
    public double SelfHarmScore { get; set; }
    public List<string>? FlaggedContent { get; set; }
    public List<string>? Recommendations { get; set; }
}

/// <summary>
/// AI model configuration
/// </summary>
public class AIModelConfig : AuditableEntity
{
    public string ModelId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AIProvider Provider { get; set; }
    public AIJobType SupportedJobType { get; set; }

    public string? Endpoint { get; set; }
    public string? ApiVersion { get; set; }
    public int? MaxTokens { get; set; }
    public double? Temperature { get; set; }

    public Dictionary<string, object> DefaultParameters { get; set; } = new();
    public List<string> SupportedLanguages { get; set; } = new();
    public List<string> SupportedFormats { get; set; } = new();

    public decimal? CostPerToken { get; set; }
    public decimal? CostPerMinute { get; set; }
    public int? RateLimitPerMinute { get; set; }

    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public int Priority { get; set; }
}

/// <summary>
/// AI usage quota
/// </summary>
public class AIUsageQuota : AuditableEntity
{
    public Guid? UserId { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? Role { get; set; }

    public AIJobType JobType { get; set; }
    public QuotaPeriod Period { get; set; }

    public int MaxRequests { get; set; }
    public int UsedRequests { get; set; }
    public int? MaxTokens { get; set; }
    public int? UsedTokens { get; set; }
    public int? MaxMinutes { get; set; }
    public int? UsedMinutes { get; set; }
    public decimal? MaxCost { get; set; }
    public decimal? UsedCost { get; set; }

    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public bool IsExceeded { get; set; }
}

/// <summary>
/// Quota period
/// </summary>
public enum QuotaPeriod
{
    Daily,
    Weekly,
    Monthly,
    Quarterly,
    Yearly
}
