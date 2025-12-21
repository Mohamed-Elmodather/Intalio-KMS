using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.AI.Domain.Entities;

/// <summary>
/// Vector embedding for semantic search
/// </summary>
public class VectorEmbedding : AuditableEntity
{
    public Guid SourceEntityId { get; set; }
    public string SourceEntityType { get; set; } = string.Empty;
    public string? SourceField { get; set; }

    public string EmbeddingModel { get; set; } = string.Empty;
    public int Dimensions { get; set; }
    public float[] Vector { get; set; } = Array.Empty<float>();

    public string? ChunkId { get; set; }
    public int? ChunkIndex { get; set; }
    public string? ChunkText { get; set; }
    public int? TokenCount { get; set; }

    public string? Language { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();

    public DateTime IndexedAt { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Semantic search query log
/// </summary>
public class SemanticSearchQuery : AuditableEntity
{
    public Guid UserId { get; set; }
    public string QueryText { get; set; } = string.Empty;
    public string? QueryLanguage { get; set; }

    public float[] QueryVector { get; set; } = Array.Empty<float>();
    public string EmbeddingModel { get; set; } = string.Empty;

    public SearchScope Scope { get; set; }
    public List<string>? EntityTypes { get; set; }
    public Dictionary<string, object>? Filters { get; set; }

    public int ResultCount { get; set; }
    public int ProcessingTimeMs { get; set; }
    public List<SemanticSearchResult> Results { get; set; } = new();

    public Guid? ClickedResultId { get; set; }
    public int? ClickedResultRank { get; set; }
    public bool WasHelpful { get; set; }
    public string? UserFeedback { get; set; }
}

/// <summary>
/// Search scope
/// </summary>
public enum SearchScope
{
    All,
    Content,
    Documents,
    Media,
    Discussions,
    LessonsLearned,
    Events,
    Tasks
}

/// <summary>
/// Semantic search result
/// </summary>
public class SemanticSearchResult
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
    public Dictionary<string, object>? Metadata { get; set; }
    public List<string>? MatchedChunks { get; set; }
}

/// <summary>
/// Question answering session
/// </summary>
public class QASession : AuditableEntity
{
    public Guid UserId { get; set; }
    public string SessionTitle { get; set; } = string.Empty;
    public QASessionType Type { get; set; }

    public List<Guid>? ContextDocumentIds { get; set; }
    public string? ContextEntityType { get; set; }
    public Guid? ContextEntityId { get; set; }

    public List<QAExchange> Exchanges { get; set; } = new();
    public int TotalQuestions { get; set; }
    public int TotalTokensUsed { get; set; }

    public bool IsActive { get; set; }
    public DateTime? LastActivityAt { get; set; }
}

/// <summary>
/// QA session type
/// </summary>
public enum QASessionType
{
    General,
    DocumentQA,
    KnowledgeBase,
    CodeAssistant,
    WritingAssistant
}

/// <summary>
/// Question and answer exchange
/// </summary>
public class QAExchange
{
    public Guid Id { get; set; }
    public int SequenceNumber { get; set; }

    public string Question { get; set; } = string.Empty;
    public string? QuestionLanguage { get; set; }
    public DateTime QuestionTime { get; set; }

    public string Answer { get; set; } = string.Empty;
    public string? AnswerLanguage { get; set; }
    public DateTime AnswerTime { get; set; }

    public List<QASource> Sources { get; set; } = new();
    public double ConfidenceScore { get; set; }
    public int TokensUsed { get; set; }
    public int ProcessingTimeMs { get; set; }

    public QAFeedback? Feedback { get; set; }
}

/// <summary>
/// Source citation for QA
/// </summary>
public class QASource
{
    public Guid EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Excerpt { get; set; }
    public double RelevanceScore { get; set; }
}

/// <summary>
/// User feedback on QA
/// </summary>
public class QAFeedback
{
    public bool IsHelpful { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public List<string>? IssueTypes { get; set; }
    public DateTime FeedbackTime { get; set; }
}

/// <summary>
/// AI-powered smart suggestion
/// </summary>
public class SmartSuggestion : AuditableEntity
{
    public Guid UserId { get; set; }
    public SuggestionType Type { get; set; }
    public SuggestionContext Context { get; set; }

    public Guid? SourceEntityId { get; set; }
    public string? SourceEntityType { get; set; }

    public LocalizedString Title { get; set; } = null!;
    public LocalizedString? Description { get; set; }
    public string? ActionUrl { get; set; }

    public double RelevanceScore { get; set; }
    public string? Reasoning { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }

    public SuggestionStatus Status { get; set; }
    public bool WasAccepted { get; set; }
    public bool WasDismissed { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// Suggestion types
/// </summary>
public enum SuggestionType
{
    RelatedContent,
    SimilarDocument,
    RecommendedExpert,
    SuggestedTag,
    SuggestedCategory,
    RelatedTask,
    RelevantEvent,
    LearningResource,
    ActionRecommendation,
    ContentImprovement,
    CollaborationOpportunity
}

/// <summary>
/// Suggestion context
/// </summary>
public enum SuggestionContext
{
    Browsing,
    Editing,
    Searching,
    Creating,
    Reviewing,
    Dashboard
}

/// <summary>
/// Suggestion status
/// </summary>
public enum SuggestionStatus
{
    Active,
    Viewed,
    Accepted,
    Dismissed,
    Expired
}

/// <summary>
/// Knowledge graph entity
/// </summary>
public class KnowledgeGraphEntity : AuditableEntity
{
    public string EntityUri { get; set; } = string.Empty;
    public KnowledgeEntityType Type { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }

    public Guid? LinkedEntityId { get; set; }
    public string? LinkedEntityType { get; set; }

    public Dictionary<string, object> Properties { get; set; } = new();
    public List<string> Aliases { get; set; } = new();
    public List<string> AliasesAr { get; set; } = new();

    public bool IsVerified { get; set; }
    public int ReferenceCount { get; set; }
}

/// <summary>
/// Knowledge entity types
/// </summary>
public enum KnowledgeEntityType
{
    Person,
    Organization,
    Location,
    Event,
    Concept,
    Topic,
    Project,
    Document,
    Process,
    Skill,
    Technology
}

/// <summary>
/// Knowledge graph relationship
/// </summary>
public class KnowledgeGraphRelation : AuditableEntity
{
    public Guid SourceEntityId { get; set; }
    public Guid TargetEntityId { get; set; }

    public KnowledgeRelationType RelationType { get; set; }
    public string? CustomRelationType { get; set; }

    public double Weight { get; set; }
    public double ConfidenceScore { get; set; }

    public Dictionary<string, object>? Properties { get; set; }
    public bool IsInferred { get; set; }
    public bool IsVerified { get; set; }

    public Guid? SourceDocumentId { get; set; }
    public string? Evidence { get; set; }
}

/// <summary>
/// Knowledge relationship types
/// </summary>
public enum KnowledgeRelationType
{
    RelatedTo,
    PartOf,
    Contains,
    CreatedBy,
    OwnedBy,
    LocatedIn,
    WorksFor,
    MemberOf,
    ResponsibleFor,
    DependsOn,
    Precedes,
    Follows,
    SimilarTo,
    DifferentFrom,
    DerivedFrom,
    References,
    Mentions,
    Custom
}

/// <summary>
/// AI prompt template
/// </summary>
public class AIPromptTemplate : AuditableEntity
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AIJobType JobType { get; set; }

    public string SystemPrompt { get; set; } = string.Empty;
    public string UserPromptTemplate { get; set; } = string.Empty;
    public string? OutputFormat { get; set; }

    public List<PromptVariable> Variables { get; set; } = new();
    public Dictionary<string, object> DefaultParameters { get; set; } = new();

    public string? ExampleInput { get; set; }
    public string? ExampleOutput { get; set; }

    public bool IsActive { get; set; }
    public bool IsSystem { get; set; }
    public int Version { get; set; }
    public int UsageCount { get; set; }
    public double AverageRating { get; set; }
}

/// <summary>
/// Prompt variable definition
/// </summary>
public class PromptVariable
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PromptVariableType Type { get; set; }
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public List<string>? AllowedValues { get; set; }
    public string? ValidationPattern { get; set; }
}

/// <summary>
/// Prompt variable types
/// </summary>
public enum PromptVariableType
{
    Text,
    Number,
    Boolean,
    Date,
    List,
    Entity,
    File
}
