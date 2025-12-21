using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Notifications.Domain.Entities;

/// <summary>
/// Notification template for consistent messaging
/// </summary>
public class NotificationTemplate : AuditableEntity
{
    /// <summary>
    /// Unique template key
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Template name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Template description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Notification type this template is for
    /// </summary>
    public NotificationType Type { get; set; }

    /// <summary>
    /// Category
    /// </summary>
    public NotificationCategory Category { get; set; }

    /// <summary>
    /// Title template (supports placeholders)
    /// </summary>
    public LocalizedString TitleTemplate { get; set; } = new();

    /// <summary>
    /// Message/body template
    /// </summary>
    public LocalizedString MessageTemplate { get; set; } = new();

    /// <summary>
    /// Short summary template for push
    /// </summary>
    public LocalizedString? SummaryTemplate { get; set; }

    /// <summary>
    /// Email subject template
    /// </summary>
    public LocalizedString? EmailSubjectTemplate { get; set; }

    /// <summary>
    /// Email HTML body template
    /// </summary>
    public string? EmailHtmlTemplate { get; set; }

    /// <summary>
    /// Email plain text template
    /// </summary>
    public string? EmailTextTemplate { get; set; }

    /// <summary>
    /// Push notification body template
    /// </summary>
    public LocalizedString? PushBodyTemplate { get; set; }

    /// <summary>
    /// SMS body template
    /// </summary>
    public LocalizedString? SmsTemplate { get; set; }

    /// <summary>
    /// Default icon
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Default icon color
    /// </summary>
    public string? IconColor { get; set; }

    /// <summary>
    /// Default action URL pattern
    /// </summary>
    public string? ActionUrlPattern { get; set; }

    /// <summary>
    /// Default priority
    /// </summary>
    public NotificationPriority DefaultPriority { get; set; } = NotificationPriority.Normal;

    /// <summary>
    /// Channels this template supports
    /// </summary>
    public List<DeliveryChannel> SupportedChannels { get; set; } = new()
    {
        DeliveryChannel.InApp,
        DeliveryChannel.Email
    };

    /// <summary>
    /// Default channels to use
    /// </summary>
    public List<DeliveryChannel> DefaultChannels { get; set; } = new()
    {
        DeliveryChannel.InApp
    };

    /// <summary>
    /// Available placeholders
    /// </summary>
    public List<TemplatePlaceholder> Placeholders { get; set; } = new();

    /// <summary>
    /// Is template active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Is system template (cannot be deleted)
    /// </summary>
    public bool IsSystem { get; set; }

    /// <summary>
    /// Template version
    /// </summary>
    public int Version { get; set; } = 1;
}

/// <summary>
/// Template placeholder definition
/// </summary>
public class TemplatePlaceholder
{
    public string Key { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? DefaultValue { get; set; }
    public bool IsRequired { get; set; }
    public string DataType { get; set; } = "string";
}

/// <summary>
/// Email template with full styling
/// </summary>
public class EmailTemplate : AuditableEntity
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>
    /// Email subject
    /// </summary>
    public LocalizedString Subject { get; set; } = new();

    /// <summary>
    /// HTML content
    /// </summary>
    public string HtmlContent { get; set; } = string.Empty;

    /// <summary>
    /// Plain text content
    /// </summary>
    public string? TextContent { get; set; }

    /// <summary>
    /// Preheader text
    /// </summary>
    public LocalizedString? Preheader { get; set; }

    /// <summary>
    /// Template category
    /// </summary>
    public EmailTemplateCategory Category { get; set; }

    /// <summary>
    /// Base layout template to use
    /// </summary>
    public string? LayoutTemplate { get; set; }

    /// <summary>
    /// Available placeholders
    /// </summary>
    public List<TemplatePlaceholder> Placeholders { get; set; } = new();

    /// <summary>
    /// Is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Is system template
    /// </summary>
    public bool IsSystem { get; set; }

    /// <summary>
    /// Version
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// Last sent date
    /// </summary>
    public DateTime? LastSentAt { get; set; }

    /// <summary>
    /// Send count
    /// </summary>
    public int SendCount { get; set; }
}

/// <summary>
/// Email template categories
/// </summary>
public enum EmailTemplateCategory
{
    Transactional,
    Notification,
    Marketing,
    System,
    Digest,
    Welcome,
    PasswordReset,
    Invitation
}
