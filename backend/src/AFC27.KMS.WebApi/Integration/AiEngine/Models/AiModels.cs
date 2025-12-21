using System;
using System.Collections.Generic;

namespace AFC27.KMS.WebApi.Integration.AiEngine.Models;

/// <summary>
/// Request for document classification
/// </summary>
public class DocumentClassificationRequest
{
    public Guid DocumentId { get; set; }
    public string? DocumentUrl { get; set; }
    public byte[]? DocumentContent { get; set; }
    public string? ContentType { get; set; }
    public string? FileName { get; set; }
    public List<string>? CandidateCategories { get; set; }
    public ClassificationOptions Options { get; set; } = new();
}

/// <summary>
/// Classification options
/// </summary>
public class ClassificationOptions
{
    public bool IncludeConfidenceScores { get; set; } = true;
    public int MaxCategories { get; set; } = 5;
    public double MinConfidenceThreshold { get; set; } = 0.5;
    public bool ExtractKeywords { get; set; } = true;
    public bool GenerateSummary { get; set; } = true;
    public string Language { get; set; } = "en";
}

/// <summary>
/// Document classification result
/// </summary>
public class DocumentClassificationResult
{
    public Guid DocumentId { get; set; }
    public List<CategoryPrediction> Categories { get; set; } = new();
    public List<string> Keywords { get; set; } = new();
    public string? Summary { get; set; }
    public string DetectedLanguage { get; set; } = string.Empty;
    public DocumentMetadata ExtractedMetadata { get; set; } = new();
    public double ProcessingTimeMs { get; set; }
}

/// <summary>
/// Category prediction with confidence
/// </summary>
public class CategoryPrediction
{
    public string CategoryId { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string? ParentCategory { get; set; }
}

/// <summary>
/// Extracted document metadata
/// </summary>
public class DocumentMetadata
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? DocumentType { get; set; }
    public int? PageCount { get; set; }
    public int? WordCount { get; set; }
    public List<string>? Entities { get; set; }
    public List<string>? Topics { get; set; }
}

/// <summary>
/// Request for content summarization
/// </summary>
public class SummarizationRequest
{
    public string Content { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public SummarizationType Type { get; set; } = SummarizationType.Extractive;
    public int MaxLength { get; set; } = 500;
    public string Language { get; set; } = "en";
    public bool IncludeKeyPoints { get; set; } = true;
}

public enum SummarizationType
{
    Extractive,
    Abstractive,
    Bullet
}

/// <summary>
/// Summarization result
/// </summary>
public class SummarizationResult
{
    public string Summary { get; set; } = string.Empty;
    public List<string>? KeyPoints { get; set; }
    public int OriginalLength { get; set; }
    public int SummaryLength { get; set; }
    public double CompressionRatio { get; set; }
}

/// <summary>
/// Request for semantic search
/// </summary>
public class SemanticSearchRequest
{
    public string Query { get; set; } = string.Empty;
    public List<string>? DocumentIds { get; set; }
    public List<string>? Categories { get; set; }
    public SearchFilters? Filters { get; set; }
    public int MaxResults { get; set; } = 10;
    public double MinRelevanceScore { get; set; } = 0.5;
    public bool IncludeSnippets { get; set; } = true;
}

/// <summary>
/// Search filters
/// </summary>
public class SearchFilters
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public List<string>? DocumentTypes { get; set; }
    public List<string>? Authors { get; set; }
    public List<string>? Tags { get; set; }
}

/// <summary>
/// Semantic search result
/// </summary>
public class SemanticSearchResult
{
    public string Query { get; set; } = string.Empty;
    public int TotalResults { get; set; }
    public List<SearchResultItem> Results { get; set; } = new();
    public List<string>? SuggestedQueries { get; set; }
    public double ProcessingTimeMs { get; set; }
}

/// <summary>
/// Individual search result item
/// </summary>
public class SearchResultItem
{
    public Guid DocumentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Snippet { get; set; }
    public double RelevanceScore { get; set; }
    public List<string>? HighlightedTerms { get; set; }
    public string? DocumentType { get; set; }
    public DateTime? LastModified { get; set; }
}

/// <summary>
/// Request for content recommendations
/// </summary>
public class RecommendationRequest
{
    public Guid? UserId { get; set; }
    public Guid? DocumentId { get; set; }
    public List<string>? UserInterests { get; set; }
    public List<Guid>? RecentlyViewedDocuments { get; set; }
    public RecommendationType Type { get; set; } = RecommendationType.Similar;
    public int MaxRecommendations { get; set; } = 10;
}

public enum RecommendationType
{
    Similar,
    Personalized,
    Trending,
    Related
}

/// <summary>
/// Recommendation result
/// </summary>
public class RecommendationResult
{
    public List<RecommendedItem> Recommendations { get; set; } = new();
    public string RecommendationType { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
}

/// <summary>
/// Recommended item
/// </summary>
public class RecommendedItem
{
    public Guid DocumentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public double Score { get; set; }
    public string? Reason { get; set; }
    public string? Category { get; set; }
}

/// <summary>
/// Request for entity extraction
/// </summary>
public class EntityExtractionRequest
{
    public string Content { get; set; } = string.Empty;
    public List<EntityType>? EntityTypes { get; set; }
    public string Language { get; set; } = "en";
    public bool IncludePositions { get; set; } = false;
}

public enum EntityType
{
    Person,
    Organization,
    Location,
    Date,
    Money,
    Email,
    Phone,
    Event,
    Product,
    Custom
}

/// <summary>
/// Entity extraction result
/// </summary>
public class EntityExtractionResult
{
    public List<ExtractedEntity> Entities { get; set; } = new();
    public int TotalEntitiesFound { get; set; }
}

/// <summary>
/// Extracted entity
/// </summary>
public class ExtractedEntity
{
    public string Text { get; set; } = string.Empty;
    public EntityType Type { get; set; }
    public double Confidence { get; set; }
    public int? StartPosition { get; set; }
    public int? EndPosition { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Request for document quality analysis
/// </summary>
public class QualityAnalysisRequest
{
    public Guid DocumentId { get; set; }
    public string? DocumentUrl { get; set; }
    public byte[]? DocumentContent { get; set; }
    public List<QualityCheck>? ChecksToPerform { get; set; }
}

public enum QualityCheck
{
    Readability,
    Grammar,
    Spelling,
    Formatting,
    Completeness,
    Consistency,
    Accessibility,
    Compliance
}

/// <summary>
/// Quality analysis result
/// </summary>
public class QualityAnalysisResult
{
    public Guid DocumentId { get; set; }
    public double OverallScore { get; set; }
    public List<QualityIssue> Issues { get; set; } = new();
    public Dictionary<QualityCheck, double> CheckScores { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
}

/// <summary>
/// Quality issue found
/// </summary>
public class QualityIssue
{
    public string IssueId { get; set; } = string.Empty;
    public QualityCheck Category { get; set; }
    public IssueSeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Suggestion { get; set; }
}

public enum IssueSeverity
{
    Info,
    Warning,
    Error,
    Critical
}

/// <summary>
/// AI Engine webhook payload
/// </summary>
public class AiEngineWebhookPayload
{
    public string JobId { get; set; } = string.Empty;
    public string JobType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid? DocumentId { get; set; }
    public object? Result { get; set; }
    public DateTime Timestamp { get; set; }
    public string Signature { get; set; } = string.Empty;
}
