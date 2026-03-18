namespace AFC27.KMS.AI.Application.DTOs;

/// <summary>
/// Request to translate all blocks of an article between EN and AR.
/// </summary>
public record TranslateArticleRequest
{
    /// <summary>
    /// Target language code ("en" or "ar").
    /// </summary>
    public string TargetLanguage { get; init; } = string.Empty;

    /// <summary>
    /// Source language code. Auto-detected if not provided.
    /// </summary>
    public string? SourceLanguage { get; init; }

    /// <summary>
    /// Preserve formatting and HTML structure within blocks.
    /// </summary>
    public bool PreserveFormatting { get; init; } = true;

    /// <summary>
    /// Optional domain hint for better translation (e.g., "sports", "legal", "technical").
    /// </summary>
    public string? Domain { get; init; }
}

/// <summary>
/// Request to translate a set of content blocks.
/// </summary>
public record TranslateBlocksRequest
{
    /// <summary>
    /// The content blocks to translate.
    /// </summary>
    public IReadOnlyList<ContentBlockDto> Blocks { get; init; } = new List<ContentBlockDto>();

    /// <summary>
    /// Target language code ("en" or "ar").
    /// </summary>
    public string TargetLanguage { get; init; } = string.Empty;

    /// <summary>
    /// Source language code. Auto-detected if not provided.
    /// </summary>
    public string? SourceLanguage { get; init; }

    /// <summary>
    /// Preserve formatting and HTML structure within blocks.
    /// </summary>
    public bool PreserveFormatting { get; init; } = true;

    /// <summary>
    /// Optional domain hint for better translation.
    /// </summary>
    public string? Domain { get; init; }
}

/// <summary>
/// Request to translate arbitrary text.
/// </summary>
public record TranslateTextRequest
{
    /// <summary>
    /// The text to translate.
    /// </summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Target language code ("en" or "ar").
    /// </summary>
    public string TargetLanguage { get; init; } = string.Empty;

    /// <summary>
    /// Source language code. Auto-detected if not provided.
    /// </summary>
    public string? SourceLanguage { get; init; }

    /// <summary>
    /// Preserve formatting and HTML structure.
    /// </summary>
    public bool PreserveFormatting { get; init; } = true;

    /// <summary>
    /// Optional domain hint for better translation.
    /// </summary>
    public string? Domain { get; init; }
}

/// <summary>
/// Response for translation operations.
/// </summary>
public record TranslationResponse
{
    /// <summary>
    /// Whether the translation was successful.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Detected source language (if auto-detected).
    /// </summary>
    public string? DetectedSourceLanguage { get; init; }

    /// <summary>
    /// Target language that was translated to.
    /// </summary>
    public string TargetLanguage { get; init; } = string.Empty;

    /// <summary>
    /// Translated text (for text translation requests).
    /// </summary>
    public string? TranslatedText { get; init; }

    /// <summary>
    /// Translated blocks (for article/block translation requests).
    /// </summary>
    public IReadOnlyList<BlockTranslationResult>? TranslatedBlocks { get; init; }

    /// <summary>
    /// Confidence score for the translation (0.0 to 1.0).
    /// </summary>
    public double? Confidence { get; init; }

    /// <summary>
    /// Number of tokens used.
    /// </summary>
    public int TokensUsed { get; init; }

    /// <summary>
    /// Processing time in milliseconds.
    /// </summary>
    public int ProcessingTimeMs { get; init; }

    /// <summary>
    /// Error message if translation failed.
    /// </summary>
    public string? Error { get; init; }
}

/// <summary>
/// Result for a single translated content block.
/// </summary>
public record BlockTranslationResult
{
    /// <summary>
    /// The block identifier.
    /// </summary>
    public string BlockId { get; init; } = string.Empty;

    /// <summary>
    /// The block type (e.g., "paragraph", "heading", "list").
    /// </summary>
    public string BlockType { get; init; } = string.Empty;

    /// <summary>
    /// The original content of the block.
    /// </summary>
    public string OriginalContent { get; init; } = string.Empty;

    /// <summary>
    /// The translated content of the block.
    /// </summary>
    public string TranslatedContent { get; init; } = string.Empty;

    /// <summary>
    /// Confidence score for this block's translation.
    /// </summary>
    public double? Confidence { get; init; }
}

/// <summary>
/// A content block within an article or page.
/// </summary>
public record ContentBlockDto
{
    /// <summary>
    /// The block identifier.
    /// </summary>
    public string BlockId { get; init; } = string.Empty;

    /// <summary>
    /// The block type (e.g., "paragraph", "heading", "list", "table").
    /// </summary>
    public string BlockType { get; init; } = string.Empty;

    /// <summary>
    /// The content of the block (may contain HTML).
    /// </summary>
    public string Content { get; init; } = string.Empty;
}
