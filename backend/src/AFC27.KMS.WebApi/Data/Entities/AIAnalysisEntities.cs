using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.WebApi.Data.Entities;

/// <summary>
/// Document analysis result entity
/// </summary>
public class DocumentAnalysisEntity : AuditableEntity
{
    public Guid DocumentId { get; set; }
    public string? Summary { get; set; }
    public string? SummaryAr { get; set; }
    public string? Language { get; set; }
    public double? LanguageConfidence { get; set; }
    public int? WordCount { get; set; }
    public int? PageCount { get; set; }
    public double? ReadabilityScore { get; set; }
    public string? SentimentLabel { get; set; } // Positive, Negative, Neutral, Mixed
    public double? SentimentScore { get; set; }
    public double? SentimentPositive { get; set; }
    public double? SentimentNegative { get; set; }
    public double? SentimentNeutral { get; set; }
    public string? TopicsJson { get; set; } // JSON array of topics
    public string? KeyPhrasesJson { get; set; } // JSON array of key phrases
    public string? SuggestedTagsJson { get; set; } // JSON array of suggested tags
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
    public string? AnalysisVersion { get; set; }
    public bool IsProcessing { get; set; }
    public string? ErrorMessage { get; set; }

    // Navigation
    public ICollection<ExtractedEntityRecord> ExtractedEntities { get; set; } = new List<ExtractedEntityRecord>();
}

/// <summary>
/// Extracted entity (NER) record
/// </summary>
public class ExtractedEntityRecord : AuditableEntity
{
    public Guid DocumentAnalysisId { get; set; }
    public Guid DocumentId { get; set; }
    public string EntityType { get; set; } = string.Empty; // Person, Organization, Location, Date, etc.
    public string Value { get; set; } = string.Empty;
    public string? NormalizedValue { get; set; }
    public double Confidence { get; set; }
    public int? StartOffset { get; set; }
    public int? EndOffset { get; set; }
    public string? Context { get; set; } // Surrounding text
    public string? MetadataJson { get; set; }

    // Navigation
    public DocumentAnalysisEntity? DocumentAnalysis { get; set; }
}

/// <summary>
/// Batch analysis job entity
/// </summary>
public class BatchAnalysisJobEntity : AuditableEntity
{
    public string JobName { get; set; } = string.Empty;
    public BatchAnalysisStatusEnum Status { get; set; } = BatchAnalysisStatusEnum.Queued;
    public int TotalDocuments { get; set; }
    public int ProcessedDocuments { get; set; }
    public int FailedDocuments { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public string? DocumentIdsJson { get; set; } // JSON array of document IDs
    public Guid InitiatedById { get; set; }
    public string InitiatedByName { get; set; } = string.Empty;
}

/// <summary>
/// Transcription result entity
/// </summary>
public class TranscriptionResultEntity : AuditableEntity
{
    public Guid DocumentId { get; set; }
    public string FullText { get; set; } = string.Empty;
    public string? Language { get; set; }
    public double? Confidence { get; set; }
    public TimeSpan? Duration { get; set; }
    public int? SpeakerCount { get; set; }
    public string? SegmentsJson { get; set; } // JSON array of segments with timestamps
    public string? SpeakersJson { get; set; } // JSON array of identified speakers
    public DateTime TranscribedAt { get; set; } = DateTime.UtcNow;
    public bool IsProcessing { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Content recommendation entity
/// </summary>
public class ContentRecommendationEntity : AuditableEntity
{
    public Guid UserId { get; set; }
    public Guid RecommendedDocumentId { get; set; }
    public string RecommendationType { get; set; } = string.Empty; // Similar, Trending, Based on history, etc.
    public double RelevanceScore { get; set; }
    public string? ReasonJson { get; set; } // JSON with recommendation reasons
    public bool IsViewed { get; set; }
    public bool IsDismissed { get; set; }
    public DateTime? ViewedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}

// Enums
public enum BatchAnalysisStatusEnum
{
    Queued,
    Processing,
    Completed,
    Failed,
    Cancelled
}
