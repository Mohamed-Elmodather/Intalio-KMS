using AFC27.KMS.AI.Domain.Entities;

namespace AFC27.KMS.AI.Application.DTOs;

/// <summary>
/// Semantic search request
/// </summary>
public class SemanticSearchRequest
{
    public string Query { get; set; } = string.Empty;
    public SearchScope Scope { get; set; } = SearchScope.All;
    public List<string>? EntityTypes { get; set; }
    public Dictionary<string, object>? Filters { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public double MinScore { get; set; } = 0.5;
    public bool IncludeChunks { get; set; }
    public bool HighlightMatches { get; set; } = true;
}

/// <summary>
/// Semantic search response
/// </summary>
public class SemanticSearchResponse
{
    public string Query { get; set; } = string.Empty;
    public int TotalResults { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int ProcessingTimeMs { get; set; }

    public List<SemanticSearchResultDto> Results { get; set; } = new();
    public List<string>? SuggestedQueries { get; set; }
    public Dictionary<string, int>? FacetCounts { get; set; }
}

/// <summary>
/// Semantic search result DTO
/// </summary>
public class SemanticSearchResultDto
{
    public Guid EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string? Snippet { get; set; }
    public string? SnippetAr { get; set; }
    public double SimilarityScore { get; set; }
    public int Rank { get; set; }
    public string? Url { get; set; }
    public string? ThumbnailUrl { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public List<string>? HighlightedChunks { get; set; }
    public DateTime? LastModified { get; set; }
}

/// <summary>
/// Question answering request
/// </summary>
public class QuestionAnsweringRequest
{
    public string Question { get; set; } = string.Empty;
    public Guid? SessionId { get; set; }
    public QASessionType? SessionType { get; set; }

    public List<Guid>? ContextDocumentIds { get; set; }
    public string? ContextEntityType { get; set; }
    public Guid? ContextEntityId { get; set; }

    public int MaxSources { get; set; } = 5;
    public double MinSourceScore { get; set; } = 0.6;
    public string? Language { get; set; }
    public bool IncludeSources { get; set; } = true;
}

/// <summary>
/// Question answering response
/// </summary>
public class QuestionAnsweringResponse
{
    public Guid SessionId { get; set; }
    public Guid ExchangeId { get; set; }

    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string? Language { get; set; }
    public double ConfidenceScore { get; set; }

    public List<QASourceDto> Sources { get; set; } = new();
    public List<string>? SuggestedFollowUps { get; set; }

    public int TokensUsed { get; set; }
    public int ProcessingTimeMs { get; set; }
}

/// <summary>
/// QA source DTO
/// </summary>
public class QASourceDto
{
    public Guid EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Excerpt { get; set; }
    public double RelevanceScore { get; set; }
}

/// <summary>
/// QA session DTO
/// </summary>
public class QASessionDto
{
    public Guid Id { get; set; }
    public string SessionTitle { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;

    public int TotalQuestions { get; set; }
    public int TotalTokensUsed { get; set; }

    public List<QAExchangeDto> RecentExchanges { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? LastActivityAt { get; set; }
}

/// <summary>
/// QA exchange DTO
/// </summary>
public class QAExchangeDto
{
    public Guid Id { get; set; }
    public int SequenceNumber { get; set; }

    public string Question { get; set; } = string.Empty;
    public DateTime QuestionTime { get; set; }

    public string Answer { get; set; } = string.Empty;
    public DateTime AnswerTime { get; set; }

    public List<QASourceDto> Sources { get; set; } = new();
    public double ConfidenceScore { get; set; }
    public QAFeedbackDto? Feedback { get; set; }
}

/// <summary>
/// QA feedback DTO
/// </summary>
public class QAFeedbackDto
{
    public bool IsHelpful { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
}

/// <summary>
/// Submit QA feedback request
/// </summary>
public class SubmitQAFeedbackRequest
{
    public Guid SessionId { get; set; }
    public Guid ExchangeId { get; set; }
    public bool IsHelpful { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public List<string>? IssueTypes { get; set; }
}

/// <summary>
/// Smart suggestion DTO
/// </summary>
public class SmartSuggestionDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;

    public Guid? SourceEntityId { get; set; }
    public string? SourceEntityType { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public string? ActionUrl { get; set; }

    public double RelevanceScore { get; set; }
    public string? Reasoning { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Get suggestions request
/// </summary>
public class GetSuggestionsRequest
{
    public SuggestionContext Context { get; set; }
    public Guid? CurrentEntityId { get; set; }
    public string? CurrentEntityType { get; set; }
    public string? CurrentText { get; set; }
    public List<SuggestionType>? Types { get; set; }
    public int MaxResults { get; set; } = 5;
}

/// <summary>
/// Knowledge graph entity DTO
/// </summary>
public class KnowledgeGraphEntityDto
{
    public Guid Id { get; set; }
    public string EntityUri { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }

    public Guid? LinkedEntityId { get; set; }
    public string? LinkedEntityType { get; set; }

    public Dictionary<string, object>? Properties { get; set; }
    public List<string>? Aliases { get; set; }
    public bool IsVerified { get; set; }
    public int ReferenceCount { get; set; }

    public List<KnowledgeGraphRelationDto>? Relations { get; set; }
}

/// <summary>
/// Knowledge graph relation DTO
/// </summary>
public class KnowledgeGraphRelationDto
{
    public Guid Id { get; set; }
    public Guid SourceEntityId { get; set; }
    public string? SourceEntityName { get; set; }
    public Guid TargetEntityId { get; set; }
    public string? TargetEntityName { get; set; }

    public string RelationType { get; set; } = string.Empty;
    public double Weight { get; set; }
    public double ConfidenceScore { get; set; }
    public bool IsInferred { get; set; }
}

/// <summary>
/// Knowledge graph query request
/// </summary>
public class KnowledgeGraphQueryRequest
{
    public string? SearchText { get; set; }
    public Guid? EntityId { get; set; }
    public KnowledgeEntityType? EntityType { get; set; }
    public int MaxDepth { get; set; } = 2;
    public int MaxNodes { get; set; } = 50;
    public List<string>? RelationTypes { get; set; }
    public bool IncludeInferred { get; set; } = true;
}

/// <summary>
/// Knowledge graph response
/// </summary>
public class KnowledgeGraphResponse
{
    public List<KnowledgeGraphEntityDto> Nodes { get; set; } = new();
    public List<KnowledgeGraphRelationDto> Edges { get; set; } = new();
    public Dictionary<string, int>? NodeTypeCounts { get; set; }
    public Dictionary<string, int>? RelationTypeCounts { get; set; }
}

/// <summary>
/// AI prompt template DTO
/// </summary>
public class AIPromptTemplateDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string JobType { get; set; } = string.Empty;

    public string SystemPrompt { get; set; } = string.Empty;
    public string UserPromptTemplate { get; set; } = string.Empty;
    public string? OutputFormat { get; set; }

    public List<PromptVariableDto> Variables { get; set; } = new();
    public string? ExampleInput { get; set; }
    public string? ExampleOutput { get; set; }

    public bool IsActive { get; set; }
    public bool IsSystem { get; set; }
    public int Version { get; set; }
    public int UsageCount { get; set; }
    public double AverageRating { get; set; }
}

/// <summary>
/// Prompt variable DTO
/// </summary>
public class PromptVariableDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public List<string>? AllowedValues { get; set; }
}

/// <summary>
/// Create/update prompt template request
/// </summary>
public class CreatePromptTemplateRequest
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AIJobType JobType { get; set; }

    public string SystemPrompt { get; set; } = string.Empty;
    public string UserPromptTemplate { get; set; } = string.Empty;
    public string? OutputFormat { get; set; }

    public List<PromptVariableDto>? Variables { get; set; }
    public Dictionary<string, object>? DefaultParameters { get; set; }
    public string? ExampleInput { get; set; }
    public string? ExampleOutput { get; set; }
}

/// <summary>
/// Execute prompt template request
/// </summary>
public class ExecutePromptRequest
{
    public string TemplateKey { get; set; } = string.Empty;
    public Dictionary<string, object> Variables { get; set; } = new();
    public string? Language { get; set; }
    public bool WaitForCompletion { get; set; } = true;
}

/// <summary>
/// Index content for semantic search request
/// </summary>
public class IndexContentRequest
{
    public Guid EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ContentAr { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public bool ChunkContent { get; set; } = true;
    public int? ChunkSize { get; set; }
}

/// <summary>
/// Batch index request
/// </summary>
public class BatchIndexRequest
{
    public List<IndexContentRequest> Items { get; set; } = new();
    public bool ReplaceExisting { get; set; } = true;
}

/// <summary>
/// Index status DTO
/// </summary>
public class IndexStatusDto
{
    public int TotalDocuments { get; set; }
    public int TotalChunks { get; set; }
    public long TotalVectors { get; set; }
    public DateTime? LastIndexedAt { get; set; }
    public Dictionary<string, int> DocumentsByType { get; set; } = new();
    public bool IsHealthy { get; set; }
    public string? Status { get; set; }
}
