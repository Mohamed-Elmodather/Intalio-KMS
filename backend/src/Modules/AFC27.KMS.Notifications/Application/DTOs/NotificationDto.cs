using AFC27.KMS.Notifications.Domain.Entities;

namespace AFC27.KMS.Notifications.Application.DTOs;

/// <summary>
/// Notification DTO
/// </summary>
public class NotificationDto
{
    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
    public NotificationCategory Category { get; set; }
    public NotificationPriority Priority { get; set; }

    public string Title { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string MessageAr { get; set; } = string.Empty;
    public string? Summary { get; set; }

    public string? Icon { get; set; }
    public string? IconColor { get; set; }
    public string? ImageUrl { get; set; }
    public string? ActionUrl { get; set; }
    public List<NotificationActionDto> Actions { get; set; } = new();

    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }

    public Guid? ActorId { get; set; }
    public string? ActorName { get; set; }
    public string? ActorAvatarUrl { get; set; }

    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsArchived { get; set; }

    public string? GroupKey { get; set; }
    public int GroupCount { get; set; }

    public DateTime CreatedAt { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
}

/// <summary>
/// Notification action DTO
/// </summary>
public class NotificationActionDto
{
    public string Label { get; set; } = string.Empty;
    public string LabelAr { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string Style { get; set; } = "default";
}

/// <summary>
/// Create notification request (internal)
/// </summary>
public class CreateNotificationRequest
{
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    public string TitleEn { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string MessageEn { get; set; } = string.Empty;
    public string MessageAr { get; set; } = string.Empty;
    public string? SummaryEn { get; set; }
    public string? SummaryAr { get; set; }

    public string? Icon { get; set; }
    public string? IconColor { get; set; }
    public string? ImageUrl { get; set; }
    public string? ActionUrl { get; set; }
    public List<NotificationActionDto>? Actions { get; set; }

    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }

    public Guid? ActorId { get; set; }
    public string? ActorName { get; set; }
    public string? ActorAvatarUrl { get; set; }

    public Dictionary<string, object>? Metadata { get; set; }
    public List<DeliveryChannel>? Channels { get; set; }
    public string? GroupKey { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// Send notification using template
/// </summary>
public class SendTemplatedNotificationRequest
{
    public string TemplateKey { get; set; } = string.Empty;
    public List<Guid> RecipientIds { get; set; } = new();
    public Dictionary<string, object> Placeholders { get; set; } = new();
    public string? ActionUrl { get; set; }
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public Guid? ActorId { get; set; }
    public List<DeliveryChannel>? Channels { get; set; }
    public bool RespectUserPreferences { get; set; } = true;
}

/// <summary>
/// Broadcast notification to multiple users
/// </summary>
public class BroadcastNotificationRequest
{
    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    public string TitleEn { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string MessageEn { get; set; } = string.Empty;
    public string MessageAr { get; set; } = string.Empty;

    public string? ActionUrl { get; set; }
    public string? Icon { get; set; }

    public BroadcastTarget Target { get; set; }
    public List<Guid>? UserIds { get; set; }
    public List<Guid>? GroupIds { get; set; }
    public List<Guid>? DepartmentIds { get; set; }
    public List<string>? Roles { get; set; }

    public List<DeliveryChannel>? Channels { get; set; }
    public DateTime? ScheduledAt { get; set; }
}

/// <summary>
/// Broadcast target type
/// </summary>
public enum BroadcastTarget
{
    AllUsers,
    SpecificUsers,
    Groups,
    Departments,
    Roles
}

/// <summary>
/// Notification filter request
/// </summary>
public class NotificationFilterRequest
{
    public NotificationCategory? Category { get; set; }
    public NotificationType? Type { get; set; }
    public bool? IsRead { get; set; }
    public bool IncludeArchived { get; set; }
    public DateTime? Since { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// Notification statistics
/// </summary>
public class NotificationStatsDto
{
    public int TotalCount { get; set; }
    public int UnreadCount { get; set; }
    public int TodayCount { get; set; }
    public Dictionary<NotificationCategory, int> ByCategory { get; set; } = new();
    public Dictionary<NotificationPriority, int> ByPriority { get; set; } = new();
}

/// <summary>
/// Mark notifications request
/// </summary>
public class MarkNotificationsRequest
{
    public List<Guid>? NotificationIds { get; set; }
    public bool MarkAll { get; set; }
    public NotificationCategory? Category { get; set; }
}

/// <summary>
/// Real-time notification for SignalR
/// </summary>
public class RealTimeNotificationDto
{
    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
    public NotificationCategory Category { get; set; }
    public NotificationPriority Priority { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? IconColor { get; set; }
    public string? ActionUrl { get; set; }
    public string? ActorName { get; set; }
    public string? ActorAvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
