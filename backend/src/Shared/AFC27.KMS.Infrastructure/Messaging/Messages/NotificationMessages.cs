namespace AFC27.KMS.Infrastructure.Messaging.Messages;

/// <summary>
/// Message for sending notifications to users.
/// </summary>
public record SendNotificationMessage
{
    /// <summary>
    /// Target user ID.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Notification type for categorization.
    /// </summary>
    public NotificationType Type { get; init; }

    /// <summary>
    /// Notification title (English).
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Notification title (Arabic).
    /// </summary>
    public string? TitleArabic { get; init; }

    /// <summary>
    /// Notification body (English).
    /// </summary>
    public string Body { get; init; } = string.Empty;

    /// <summary>
    /// Notification body (Arabic).
    /// </summary>
    public string? BodyArabic { get; init; }

    /// <summary>
    /// Delivery channels for this notification.
    /// </summary>
    public List<NotificationChannel> Channels { get; init; } = new() { NotificationChannel.InApp };

    /// <summary>
    /// Priority level.
    /// </summary>
    public NotificationPriority Priority { get; init; } = NotificationPriority.Normal;

    /// <summary>
    /// Related entity type (for deep linking).
    /// </summary>
    public string? RelatedEntityType { get; init; }

    /// <summary>
    /// Related entity ID (for deep linking).
    /// </summary>
    public Guid? RelatedEntityId { get; init; }

    /// <summary>
    /// Action URL for the notification.
    /// </summary>
    public string? ActionUrl { get; init; }

    /// <summary>
    /// Additional data for the notification.
    /// </summary>
    public Dictionary<string, string> Data { get; init; } = new();

    /// <summary>
    /// Timestamp when notification should be sent.
    /// </summary>
    public DateTime? ScheduledAt { get; init; }

    /// <summary>
    /// Expiration time after which notification should not be delivered.
    /// </summary>
    public DateTime? ExpiresAt { get; init; }
}

/// <summary>
/// Message for sending bulk notifications.
/// </summary>
public record SendBulkNotificationMessage
{
    /// <summary>
    /// List of target user IDs.
    /// </summary>
    public List<Guid> UserIds { get; init; } = new();

    /// <summary>
    /// Target a specific group instead of individual users.
    /// </summary>
    public Guid? GroupId { get; init; }

    /// <summary>
    /// Target all users in a department.
    /// </summary>
    public Guid? DepartmentId { get; init; }

    /// <summary>
    /// Notification type.
    /// </summary>
    public NotificationType Type { get; init; }

    /// <summary>
    /// Notification title (English).
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Notification title (Arabic).
    /// </summary>
    public string? TitleArabic { get; init; }

    /// <summary>
    /// Notification body (English).
    /// </summary>
    public string Body { get; init; } = string.Empty;

    /// <summary>
    /// Notification body (Arabic).
    /// </summary>
    public string? BodyArabic { get; init; }

    /// <summary>
    /// Delivery channels.
    /// </summary>
    public List<NotificationChannel> Channels { get; init; } = new() { NotificationChannel.InApp };

    /// <summary>
    /// Priority level.
    /// </summary>
    public NotificationPriority Priority { get; init; } = NotificationPriority.Normal;

    /// <summary>
    /// Related entity information.
    /// </summary>
    public string? RelatedEntityType { get; init; }
    public Guid? RelatedEntityId { get; init; }
    public string? ActionUrl { get; init; }
}

/// <summary>
/// Message for email notification.
/// </summary>
public record SendEmailMessage
{
    public string To { get; init; } = string.Empty;
    public List<string>? Cc { get; init; }
    public List<string>? Bcc { get; init; }
    public string Subject { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public bool IsHtml { get; init; } = true;
    public string? TemplateName { get; init; }
    public Dictionary<string, string> TemplateData { get; init; } = new();
    public List<EmailAttachment>? Attachments { get; init; }
    public NotificationPriority Priority { get; init; } = NotificationPriority.Normal;
}

/// <summary>
/// Email attachment info.
/// </summary>
public record EmailAttachment
{
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public string StoragePath { get; init; } = string.Empty;
}

/// <summary>
/// Notification type categories.
/// </summary>
public enum NotificationType
{
    System,
    Document,
    Content,
    Workflow,
    Task,
    Comment,
    Mention,
    Share,
    Approval,
    Calendar,
    AI
}

/// <summary>
/// Notification delivery channels.
/// </summary>
public enum NotificationChannel
{
    InApp,
    Email,
    Push,
    SMS
}

/// <summary>
/// Notification priority levels.
/// </summary>
public enum NotificationPriority
{
    Low,
    Normal,
    High,
    Urgent
}
