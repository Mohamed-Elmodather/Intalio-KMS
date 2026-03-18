namespace AFC27.KMS.Integration.Application.DTOs;

/// <summary>
/// Incoming message from a Teams or Slack bot channel.
/// </summary>
public class BotMessageRequest
{
    /// <summary>
    /// Unique identifier of the incoming message from the external platform.
    /// </summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// The channel from which the message originated (e.g., "teams", "slack").
    /// </summary>
    public string Channel { get; set; } = string.Empty;

    /// <summary>
    /// Identifier of the user who sent the message.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the user.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Email address of the user, if available.
    /// </summary>
    public string? UserEmail { get; set; }

    /// <summary>
    /// The channel or conversation identifier (team channel, Slack channel, DM, etc.).
    /// </summary>
    public string? ConversationId { get; set; }

    /// <summary>
    /// The user's question or message text.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// ISO language code preferred by the user (e.g., "en", "ar").
    /// </summary>
    public string Language { get; set; } = "en";

    /// <summary>
    /// Optional thread/reply identifier for threaded conversations.
    /// </summary>
    public string? ThreadId { get; set; }

    /// <summary>
    /// Optional attachments or file references sent with the message.
    /// </summary>
    public List<BotAttachment>? Attachments { get; set; }
}

/// <summary>
/// Attachment sent with a bot message.
/// </summary>
public class BotAttachment
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string? Url { get; set; }
    public long? Size { get; set; }
}

/// <summary>
/// Response sent back to the external bot channel.
/// </summary>
public class BotMessageResponse
{
    /// <summary>
    /// A unique identifier for this response message.
    /// </summary>
    public string ResponseId { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// The original message identifier being replied to.
    /// </summary>
    public string? ReplyToMessageId { get; set; }

    /// <summary>
    /// Plain-text answer content.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Rich/formatted answer (Markdown for Slack, Adaptive Card JSON for Teams).
    /// </summary>
    public string? RichContent { get; set; }

    /// <summary>
    /// Content format indicator: "text", "markdown", "adaptive-card".
    /// </summary>
    public string ContentFormat { get; set; } = "text";

    /// <summary>
    /// Source articles or documents referenced in the answer.
    /// </summary>
    public List<BotSourceReference> Sources { get; set; } = new();

    /// <summary>
    /// Confidence score of the AI-generated answer (0.0 - 1.0).
    /// </summary>
    public double? Confidence { get; set; }

    /// <summary>
    /// Suggested follow-up questions for the user.
    /// </summary>
    public List<string> SuggestedQuestions { get; set; } = new();

    /// <summary>
    /// Processing time in milliseconds.
    /// </summary>
    public int ProcessingTimeMs { get; set; }
}

/// <summary>
/// A knowledge-base source referenced in the bot answer.
/// </summary>
public class BotSourceReference
{
    public Guid ArticleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Url { get; set; }
    public double Relevance { get; set; }
}

/// <summary>
/// Request to register a bot webhook endpoint for push-based messaging.
/// </summary>
public class BotRegistrationRequest
{
    /// <summary>
    /// The channel platform: "teams" or "slack".
    /// </summary>
    public string Channel { get; set; } = string.Empty;

    /// <summary>
    /// Display name for this bot registration.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The webhook URL that the KMS will call to deliver responses.
    /// </summary>
    public string WebhookUrl { get; set; } = string.Empty;

    /// <summary>
    /// Verification token or signing secret used to validate webhook authenticity.
    /// </summary>
    public string? VerificationToken { get; set; }

    /// <summary>
    /// Bot application/client ID on the external platform.
    /// </summary>
    public string? BotAppId { get; set; }

    /// <summary>
    /// Bot application secret on the external platform.
    /// </summary>
    public string? BotAppSecret { get; set; }

    /// <summary>
    /// Tenant or workspace identifier on the external platform.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Allowed channel/conversation IDs the bot should respond in (empty = all).
    /// </summary>
    public List<string> AllowedConversations { get; set; } = new();

    /// <summary>
    /// Default language for responses.
    /// </summary>
    public string DefaultLanguage { get; set; } = "en";
}

/// <summary>
/// Result of a bot registration operation.
/// </summary>
public class BotRegistrationResponse
{
    public Guid RegistrationId { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public DateTime RegisteredAt { get; set; }
}
