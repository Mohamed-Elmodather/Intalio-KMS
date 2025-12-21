namespace AFC27.KMS.Infrastructure.Messaging.Messages;

/// <summary>
/// Message requesting AI ingestion of a document for semantic search.
/// </summary>
public record AIIngestionRequestMessage
{
    /// <summary>
    /// Entity ID (Document, Article, etc.).
    /// </summary>
    public Guid EntityId { get; init; }

    /// <summary>
    /// Entity type for polymorphic processing.
    /// </summary>
    public string EntityType { get; init; } = string.Empty;

    /// <summary>
    /// Storage path for document files.
    /// </summary>
    public string? StoragePath { get; init; }

    /// <summary>
    /// Direct content for text-based entities (Articles, etc.).
    /// </summary>
    public string? Content { get; init; }

    /// <summary>
    /// Entity title for context.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// MIME content type (for files).
    /// </summary>
    public string? ContentType { get; init; }

    /// <summary>
    /// Library or category ID for ACL context.
    /// </summary>
    public Guid? ContainerId { get; init; }

    /// <summary>
    /// Language hint for processing.
    /// </summary>
    public string Language { get; init; } = "en";

    /// <summary>
    /// Whether to force re-ingestion if already indexed.
    /// </summary>
    public bool ForceReindex { get; init; } = false;

    /// <summary>
    /// Priority level (0-10, higher is more urgent).
    /// </summary>
    public int Priority { get; init; } = 5;

    /// <summary>
    /// Timestamp when request was created.
    /// </summary>
    public DateTime RequestedAt { get; init; }

    /// <summary>
    /// User who triggered the ingestion.
    /// </summary>
    public Guid RequestedByUserId { get; init; }
}

/// <summary>
/// Message for embedding generation request.
/// </summary>
public record EmbeddingGenerationMessage
{
    /// <summary>
    /// Source entity ID.
    /// </summary>
    public Guid EntityId { get; init; }

    /// <summary>
    /// Entity type.
    /// </summary>
    public string EntityType { get; init; } = string.Empty;

    /// <summary>
    /// Chunk index within the document.
    /// </summary>
    public int ChunkIndex { get; init; }

    /// <summary>
    /// Text content to generate embeddings for.
    /// </summary>
    public string ChunkText { get; init; } = string.Empty;

    /// <summary>
    /// Model to use for embedding generation.
    /// </summary>
    public string Model { get; init; } = "default";
}

/// <summary>
/// Message for AI summary generation.
/// </summary>
public record SummaryGenerationMessage
{
    public Guid EntityId { get; init; }
    public string EntityType { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string Language { get; init; } = "en";
    public int MaxLength { get; init; } = 500;
    public Guid RequestedByUserId { get; init; }
}

/// <summary>
/// Message for AI classification request.
/// </summary>
public record ClassificationRequestMessage
{
    public Guid EntityId { get; init; }
    public string EntityType { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public List<string> AvailableCategories { get; init; } = new();
    public List<string> AvailableTags { get; init; } = new();
    public Guid RequestedByUserId { get; init; }
}
