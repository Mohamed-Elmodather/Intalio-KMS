namespace AFC27.KMS.AI.Application.DTOs;

/// <summary>
/// Request to query the AI with injected page/entity context.
/// </summary>
public record ContextQueryRequest
{
    /// <summary>
    /// The user's question or instruction.
    /// </summary>
    public string Query { get; init; } = string.Empty;

    /// <summary>
    /// Type of context being provided (Article, Document, LessonLearned, etc.).
    /// </summary>
    public string ContextType { get; init; } = string.Empty;

    /// <summary>
    /// ID of the entity providing context.
    /// </summary>
    public Guid? ContextEntityId { get; init; }

    /// <summary>
    /// Optional raw text context to inject (used when entity ID is not available).
    /// </summary>
    public string? ContextText { get; init; }

    /// <summary>
    /// Preferred output language (en/ar).
    /// </summary>
    public string? Language { get; init; }
}

/// <summary>
/// Response from a context-aware AI query.
/// </summary>
public record ContextQueryResponse
{
    /// <summary>
    /// The AI-generated answer.
    /// </summary>
    public string Answer { get; init; } = string.Empty;

    /// <summary>
    /// Confidence score (0.0-1.0) of the answer.
    /// </summary>
    public double Confidence { get; init; }

    /// <summary>
    /// Sources used to generate the answer.
    /// </summary>
    public IReadOnlyList<ContextSource> Sources { get; init; } = Array.Empty<ContextSource>();

    /// <summary>
    /// Tokens consumed by the request.
    /// </summary>
    public int TokensUsed { get; init; }
}

/// <summary>
/// A source reference used in a context query response.
/// </summary>
public record ContextSource
{
    /// <summary>
    /// Type of the source entity.
    /// </summary>
    public string EntityType { get; init; } = string.Empty;

    /// <summary>
    /// ID of the source entity.
    /// </summary>
    public Guid EntityId { get; init; }

    /// <summary>
    /// Display title of the source.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Relevant excerpt from the source.
    /// </summary>
    public string? Excerpt { get; init; }
}

/// <summary>
/// Response for article summarization.
/// </summary>
public record ArticleSummarizeResponse
{
    public Guid ArticleId { get; init; }
    public string Summary { get; init; } = string.Empty;
    public IReadOnlyList<string> KeyPoints { get; init; } = Array.Empty<string>();
    public int TokensUsed { get; init; }
}

/// <summary>
/// Response for article explanation.
/// </summary>
public record ArticleExplainResponse
{
    public Guid ArticleId { get; init; }
    public string Explanation { get; init; } = string.Empty;
    public int TokensUsed { get; init; }
}

/// <summary>
/// Response for document key points extraction.
/// </summary>
public record DocumentExtractPointsResponse
{
    public Guid DocumentId { get; init; }
    public IReadOnlyList<string> KeyPoints { get; init; } = Array.Empty<string>();
    public string? Summary { get; init; }
    public int TokensUsed { get; init; }
}
