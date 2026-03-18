namespace AFC27.KMS.AI.Application.DTOs;

/// <summary>
/// Base writing assistance request.
/// </summary>
public record AIWritingRequest
{
    /// <summary>
    /// The text to process.
    /// </summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Optional context about the document or content being edited (e.g., article title, section).
    /// </summary>
    public string? Context { get; init; }

    /// <summary>
    /// Optional specific instruction for the AI (e.g., "Make it more concise").
    /// </summary>
    public string? Instruction { get; init; }

    /// <summary>
    /// Language of the input text (e.g., "en", "ar"). Auto-detected if not provided.
    /// </summary>
    public string? Language { get; init; }
}

/// <summary>
/// Writing assistance response.
/// </summary>
public record AIWritingResponse
{
    /// <summary>
    /// The processed result text.
    /// </summary>
    public string Result { get; init; } = string.Empty;

    /// <summary>
    /// Confidence score for the result (0.0 to 1.0).
    /// </summary>
    public double? Confidence { get; init; }

    /// <summary>
    /// List of specific suggested changes, if applicable.
    /// </summary>
    public IReadOnlyList<SuggestedChange>? SuggestedChanges { get; init; }

    /// <summary>
    /// Number of tokens used for this request.
    /// </summary>
    public int TokensUsed { get; init; }

    /// <summary>
    /// Processing time in milliseconds.
    /// </summary>
    public int ProcessingTimeMs { get; init; }
}

/// <summary>
/// A single suggested change within the text.
/// </summary>
public record SuggestedChange
{
    /// <summary>
    /// The original text segment.
    /// </summary>
    public string Original { get; init; } = string.Empty;

    /// <summary>
    /// The suggested replacement.
    /// </summary>
    public string Suggested { get; init; } = string.Empty;

    /// <summary>
    /// Reason for the suggestion.
    /// </summary>
    public string? Reason { get; init; }

    /// <summary>
    /// Category of the change (grammar, clarity, style, etc.).
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Start position in the original text.
    /// </summary>
    public int? StartPosition { get; init; }

    /// <summary>
    /// End position in the original text.
    /// </summary>
    public int? EndPosition { get; init; }
}

/// <summary>
/// Request to change the tone of text.
/// </summary>
public record AIToneChangeRequest
{
    /// <summary>
    /// The text to rewrite.
    /// </summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// The target tone.
    /// </summary>
    public WritingTone TargetTone { get; init; } = WritingTone.Formal;

    /// <summary>
    /// Optional context about the content.
    /// </summary>
    public string? Context { get; init; }

    /// <summary>
    /// Language of the input text.
    /// </summary>
    public string? Language { get; init; }
}

/// <summary>
/// Supported writing tones.
/// </summary>
public enum WritingTone
{
    /// <summary>Professional and formal language.</summary>
    Formal,

    /// <summary>Conversational and approachable language.</summary>
    Casual,

    /// <summary>Simplified language for broader audiences.</summary>
    Simplified,

    /// <summary>Technical and precise language.</summary>
    Technical,

    /// <summary>Diplomatic and balanced language.</summary>
    Diplomatic,

    /// <summary>Persuasive and compelling language.</summary>
    Persuasive
}

/// <summary>
/// Request to translate text.
/// </summary>
public record AITranslateRequest
{
    /// <summary>
    /// The text to translate.
    /// </summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Source language code (e.g., "en", "ar"). Auto-detected if not provided.
    /// </summary>
    public string? SourceLanguage { get; init; }

    /// <summary>
    /// Target language code (e.g., "en", "ar").
    /// </summary>
    public string TargetLanguage { get; init; } = string.Empty;

    /// <summary>
    /// Preserve formatting and structure.
    /// </summary>
    public bool PreserveFormatting { get; init; } = true;

    /// <summary>
    /// Optional domain hint for better translation (e.g., "sports", "legal", "technical").
    /// </summary>
    public string? Domain { get; init; }
}

/// <summary>
/// Request to generate content from a prompt.
/// </summary>
public record AIGenerateRequest
{
    /// <summary>
    /// The prompt describing what to generate.
    /// </summary>
    public string Prompt { get; init; } = string.Empty;

    /// <summary>
    /// Writing style to use (e.g., "professional", "creative", "journalistic").
    /// </summary>
    public string? Style { get; init; }

    /// <summary>
    /// Maximum length of generated content in words.
    /// </summary>
    public int? MaxLength { get; init; }

    /// <summary>
    /// Optional prompt template ID to use from the template library.
    /// </summary>
    public Guid? TemplateId { get; init; }

    /// <summary>
    /// Target language for the generated content.
    /// </summary>
    public string? Language { get; init; }

    /// <summary>
    /// Optional context or reference material to guide generation.
    /// </summary>
    public string? Context { get; init; }
}

/// <summary>
/// Request to summarize text.
/// </summary>
public record AISummarizeRequest
{
    /// <summary>
    /// The text to summarize.
    /// </summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Maximum number of sentences in the summary.
    /// </summary>
    public int? MaxSentences { get; init; }

    /// <summary>
    /// Language of the input text.
    /// </summary>
    public string? Language { get; init; }

    /// <summary>
    /// Optional focus area for the summary.
    /// </summary>
    public string? FocusArea { get; init; }
}

/// <summary>
/// Request to continue writing from a position.
/// </summary>
public record AIContinueRequest
{
    /// <summary>
    /// The existing text to continue from.
    /// </summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Optional context about what the text is about.
    /// </summary>
    public string? Context { get; init; }

    /// <summary>
    /// Direction or guidance for continuation (e.g., "conclude the paragraph", "add examples").
    /// </summary>
    public string? Direction { get; init; }

    /// <summary>
    /// Maximum number of words to generate.
    /// </summary>
    public int? MaxWords { get; init; }

    /// <summary>
    /// Language to continue in.
    /// </summary>
    public string? Language { get; init; }
}

/// <summary>
/// A chunk of streamed AI output.
/// </summary>
public record AIStreamChunk
{
    /// <summary>
    /// The content fragment.
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Whether this is the final chunk.
    /// </summary>
    public bool IsComplete { get; init; }

    /// <summary>
    /// Running token count.
    /// </summary>
    public int TokenCount { get; init; }
}

/// <summary>
/// Request for streaming writing operations.
/// </summary>
public record AIStreamWritingRequest
{
    /// <summary>
    /// The operation to perform.
    /// </summary>
    public WritingOperation Operation { get; init; } = WritingOperation.Generate;

    /// <summary>
    /// The text or prompt to process.
    /// </summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Optional context.
    /// </summary>
    public string? Context { get; init; }

    /// <summary>
    /// Optional instruction.
    /// </summary>
    public string? Instruction { get; init; }

    /// <summary>
    /// Target tone for tone-change operations.
    /// </summary>
    public WritingTone? TargetTone { get; init; }

    /// <summary>
    /// Target language for translation operations.
    /// </summary>
    public string? TargetLanguage { get; init; }

    /// <summary>
    /// Language of the input text.
    /// </summary>
    public string? Language { get; init; }
}

/// <summary>
/// Supported writing operations for streaming.
/// </summary>
public enum WritingOperation
{
    Generate,
    Improve,
    ChangeTone,
    Translate,
    Summarize,
    Continue,
    Suggest
}
