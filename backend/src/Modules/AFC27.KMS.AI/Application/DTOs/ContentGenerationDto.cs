namespace AFC27.KMS.AI.Application.DTOs;

/// <summary>
/// Request to generate a complete article from a topic, prompt, and optional outline.
/// </summary>
public record GenerateArticleRequest
{
    /// <summary>
    /// The topic or subject of the article.
    /// </summary>
    public string Topic { get; init; } = string.Empty;

    /// <summary>
    /// Optional detailed prompt or instructions for generation.
    /// </summary>
    public string? Prompt { get; init; }

    /// <summary>
    /// Optional outline with section headings and brief descriptions.
    /// </summary>
    public IReadOnlyList<ArticleOutlineSection>? Outline { get; init; }

    /// <summary>
    /// Writing style (e.g., "professional", "journalistic", "technical", "creative").
    /// </summary>
    public string? Style { get; init; }

    /// <summary>
    /// Optional template ID from the prompt template library.
    /// </summary>
    public Guid? TemplateId { get; init; }

    /// <summary>
    /// Maximum number of sections to generate.
    /// </summary>
    public int MaxSections { get; init; } = 5;

    /// <summary>
    /// Target language for the generated article ("en" or "ar").
    /// </summary>
    public string? Language { get; init; }

    /// <summary>
    /// Optional context or reference material to guide generation.
    /// </summary>
    public string? Context { get; init; }

    /// <summary>
    /// Target audience description (e.g., "general public", "technical staff", "executives").
    /// </summary>
    public string? TargetAudience { get; init; }

    /// <summary>
    /// Target word count for the entire article. Defaults to no limit.
    /// </summary>
    public int? TargetWordCount { get; init; }
}

/// <summary>
/// A section in an article outline.
/// </summary>
public record ArticleOutlineSection
{
    /// <summary>
    /// The section heading.
    /// </summary>
    public string Heading { get; init; } = string.Empty;

    /// <summary>
    /// Brief description or key points for this section.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Section order/position.
    /// </summary>
    public int Order { get; init; }
}

/// <summary>
/// Response containing a generated article.
/// </summary>
public record GenerateArticleResponse
{
    /// <summary>
    /// The generated article title.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// The generated article sections.
    /// </summary>
    public IReadOnlyList<GeneratedSection> Sections { get; init; } = new List<GeneratedSection>();

    /// <summary>
    /// A generated executive summary of the article.
    /// </summary>
    public string Summary { get; init; } = string.Empty;

    /// <summary>
    /// Suggested tags/keywords for the article.
    /// </summary>
    public IReadOnlyList<string> SuggestedTags { get; init; } = new List<string>();

    /// <summary>
    /// Suggested category for the article.
    /// </summary>
    public string? SuggestedCategory { get; init; }

    /// <summary>
    /// Total word count of the generated article.
    /// </summary>
    public int WordCount { get; init; }

    /// <summary>
    /// Number of tokens used for generation.
    /// </summary>
    public int TokensUsed { get; init; }

    /// <summary>
    /// Processing time in milliseconds.
    /// </summary>
    public int ProcessingTimeMs { get; init; }
}

/// <summary>
/// A generated section within an article.
/// </summary>
public record GeneratedSection
{
    /// <summary>
    /// The section heading.
    /// </summary>
    public string Heading { get; init; } = string.Empty;

    /// <summary>
    /// The section content (may contain HTML).
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Section order/position.
    /// </summary>
    public int Order { get; init; }

    /// <summary>
    /// Word count for this section.
    /// </summary>
    public int WordCount { get; init; }
}
