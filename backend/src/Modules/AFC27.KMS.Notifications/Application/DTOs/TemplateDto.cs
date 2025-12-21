using AFC27.KMS.Notifications.Domain.Entities;

namespace AFC27.KMS.Notifications.Application.DTOs;

/// <summary>
/// Notification template DTO
/// </summary>
public class NotificationTemplateDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public NotificationType Type { get; set; }
    public NotificationCategory Category { get; set; }

    public string TitleTemplate { get; set; } = string.Empty;
    public string TitleTemplateAr { get; set; } = string.Empty;
    public string MessageTemplate { get; set; } = string.Empty;
    public string MessageTemplateAr { get; set; } = string.Empty;

    public string? Icon { get; set; }
    public string? IconColor { get; set; }
    public string? ActionUrlPattern { get; set; }
    public NotificationPriority DefaultPriority { get; set; }

    public List<DeliveryChannel> SupportedChannels { get; set; } = new();
    public List<DeliveryChannel> DefaultChannels { get; set; } = new();
    public List<TemplatePlaceholderDto> Placeholders { get; set; } = new();

    public bool IsActive { get; set; }
    public bool IsSystem { get; set; }
    public int Version { get; set; }
}

/// <summary>
/// Template placeholder DTO
/// </summary>
public class TemplatePlaceholderDto
{
    public string Key { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? DefaultValue { get; set; }
    public bool IsRequired { get; set; }
    public string DataType { get; set; } = "string";
}

/// <summary>
/// Create/update template request
/// </summary>
public class CreateTemplateRequest
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public NotificationType Type { get; set; }
    public NotificationCategory Category { get; set; }

    public string TitleTemplateEn { get; set; } = string.Empty;
    public string TitleTemplateAr { get; set; } = string.Empty;
    public string MessageTemplateEn { get; set; } = string.Empty;
    public string MessageTemplateAr { get; set; } = string.Empty;
    public string? SummaryTemplateEn { get; set; }
    public string? SummaryTemplateAr { get; set; }

    public string? EmailSubjectEn { get; set; }
    public string? EmailSubjectAr { get; set; }
    public string? EmailHtmlTemplate { get; set; }
    public string? EmailTextTemplate { get; set; }
    public string? PushBodyEn { get; set; }
    public string? PushBodyAr { get; set; }

    public string? Icon { get; set; }
    public string? IconColor { get; set; }
    public string? ActionUrlPattern { get; set; }
    public NotificationPriority DefaultPriority { get; set; } = NotificationPriority.Normal;

    public List<DeliveryChannel>? SupportedChannels { get; set; }
    public List<DeliveryChannel>? DefaultChannels { get; set; }
    public List<TemplatePlaceholderDto>? Placeholders { get; set; }
}

/// <summary>
/// Email template DTO
/// </summary>
public class EmailTemplateDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public string Subject { get; set; } = string.Empty;
    public string SubjectAr { get; set; } = string.Empty;
    public string HtmlContent { get; set; } = string.Empty;
    public string? TextContent { get; set; }
    public string? Preheader { get; set; }
    public string? PreheaderAr { get; set; }

    public EmailTemplateCategory Category { get; set; }
    public string? LayoutTemplate { get; set; }
    public List<TemplatePlaceholderDto> Placeholders { get; set; } = new();

    public bool IsActive { get; set; }
    public bool IsSystem { get; set; }
    public int Version { get; set; }
    public int SendCount { get; set; }
    public DateTime? LastSentAt { get; set; }
}

/// <summary>
/// Create email template request
/// </summary>
public class CreateEmailTemplateRequest
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public string SubjectEn { get; set; } = string.Empty;
    public string SubjectAr { get; set; } = string.Empty;
    public string HtmlContent { get; set; } = string.Empty;
    public string? TextContent { get; set; }
    public string? PreheaderEn { get; set; }
    public string? PreheaderAr { get; set; }

    public EmailTemplateCategory Category { get; set; }
    public string? LayoutTemplate { get; set; }
    public List<TemplatePlaceholderDto>? Placeholders { get; set; }
}

/// <summary>
/// Preview template request
/// </summary>
public class PreviewTemplateRequest
{
    public string TemplateKey { get; set; } = string.Empty;
    public Dictionary<string, object> Placeholders { get; set; } = new();
    public string Language { get; set; } = "en";
}

/// <summary>
/// Template preview result
/// </summary>
public class TemplatePreviewDto
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? EmailSubject { get; set; }
    public string? EmailHtml { get; set; }
    public string? EmailText { get; set; }
    public string? PushBody { get; set; }
}

/// <summary>
/// Send test notification request
/// </summary>
public class SendTestNotificationRequest
{
    public string TemplateKey { get; set; } = string.Empty;
    public DeliveryChannel Channel { get; set; }
    public Dictionary<string, object> Placeholders { get; set; } = new();
    public string? TestEmail { get; set; }
}

/// <summary>
/// Delivery statistics
/// </summary>
public class DeliveryStatsDto
{
    public DeliveryChannel Channel { get; set; }
    public int TotalSent { get; set; }
    public int Delivered { get; set; }
    public int Failed { get; set; }
    public int Opened { get; set; }
    public int Clicked { get; set; }
    public double DeliveryRate { get; set; }
    public double OpenRate { get; set; }
    public double ClickRate { get; set; }
}

/// <summary>
/// Notification analytics
/// </summary>
public class NotificationAnalyticsDto
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public int TotalSent { get; set; }
    public int TotalDelivered { get; set; }
    public int TotalRead { get; set; }
    public Dictionary<NotificationType, int> ByType { get; set; } = new();
    public Dictionary<NotificationCategory, int> ByCategory { get; set; } = new();
    public List<DeliveryStatsDto> ByChannel { get; set; } = new();
    public List<DailyNotificationStatsDto> DailyStats { get; set; } = new();
}

/// <summary>
/// Daily notification statistics
/// </summary>
public class DailyNotificationStatsDto
{
    public DateTime Date { get; set; }
    public int Sent { get; set; }
    public int Delivered { get; set; }
    public int Read { get; set; }
}
