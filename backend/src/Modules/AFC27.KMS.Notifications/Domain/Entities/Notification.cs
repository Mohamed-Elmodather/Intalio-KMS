using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Notifications.Domain.Entities;

/// <summary>
/// User notification entity
/// </summary>
public class Notification : AuditableEntity
{
    /// <summary>
    /// Recipient user ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Notification type
    /// </summary>
    public NotificationType Type { get; set; }

    /// <summary>
    /// Notification category for grouping
    /// </summary>
    public NotificationCategory Category { get; set; }

    /// <summary>
    /// Priority level
    /// </summary>
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    /// <summary>
    /// Notification title
    /// </summary>
    public LocalizedString Title { get; set; } = new();

    /// <summary>
    /// Notification message/body
    /// </summary>
    public LocalizedString Message { get; set; } = new();

    /// <summary>
    /// Short summary for push notifications
    /// </summary>
    public LocalizedString? Summary { get; set; }

    /// <summary>
    /// Icon to display
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Icon color/style
    /// </summary>
    public string? IconColor { get; set; }

    /// <summary>
    /// Image URL for rich notifications
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Action URL when clicked
    /// </summary>
    public string? ActionUrl { get; set; }

    /// <summary>
    /// Additional action buttons
    /// </summary>
    public List<NotificationAction> Actions { get; set; } = new();

    /// <summary>
    /// Related entity type (Article, Document, Task, etc.)
    /// </summary>
    public string? EntityType { get; set; }

    /// <summary>
    /// Related entity ID
    /// </summary>
    public Guid? EntityId { get; set; }

    /// <summary>
    /// Actor who triggered the notification
    /// </summary>
    public Guid? ActorId { get; set; }

    /// <summary>
    /// Actor display name
    /// </summary>
    public string? ActorName { get; set; }

    /// <summary>
    /// Actor avatar URL
    /// </summary>
    public string? ActorAvatarUrl { get; set; }

    /// <summary>
    /// Additional metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Read status
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// When the notification was read
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Archived status
    /// </summary>
    public bool IsArchived { get; set; }

    /// <summary>
    /// When the notification was archived
    /// </summary>
    public DateTime? ArchivedAt { get; set; }

    /// <summary>
    /// Delivery status for each channel
    /// </summary>
    public List<NotificationDelivery> Deliveries { get; set; } = new();

    /// <summary>
    /// Expiration date (auto-archive after)
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Group key for batching similar notifications
    /// </summary>
    public string? GroupKey { get; set; }

    /// <summary>
    /// Count of grouped notifications
    /// </summary>
    public int GroupCount { get; set; } = 1;
}

/// <summary>
/// Notification action button
/// </summary>
public class NotificationAction
{
    public string Label { get; set; } = string.Empty;
    public string LabelAr { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string Style { get; set; } = "default"; // default, primary, danger
}

/// <summary>
/// Delivery status per channel
/// </summary>
public class NotificationDelivery : Entity
{
    public Guid NotificationId { get; set; }
    public Notification Notification { get; set; } = null!;

    public DeliveryChannel Channel { get; set; }
    public DeliveryStatus Status { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public string? FailureReason { get; set; }
    public int RetryCount { get; set; }
    public string? ExternalId { get; set; }
}

/// <summary>
/// Notification types
/// </summary>
public enum NotificationType
{
    // Content
    ContentPublished,
    ContentUpdated,
    ContentMentioned,
    ContentCommented,
    ContentShared,
    ContentLiked,

    // Documents
    DocumentUploaded,
    DocumentShared,
    DocumentCheckedOut,
    DocumentCheckedIn,
    DocumentVersionCreated,

    // Collaboration
    CommunityInvite,
    CommunityJoined,
    DiscussionReplied,
    DiscussionMentioned,
    DiscussionAnswerAccepted,
    LessonLearnedApproved,

    // Lessons Learned lifecycle
    LessonSubmitted,
    LessonRejected,
    LessonPublished,
    LessonActionAssigned,
    LessonActionOverdue,
    LessonActionCompleted,
    LessonActionsAllComplete,
    LessonVerified,

    // Knowledge Verification
    VerificationDue,
    VerificationOverdue,
    VerificationCompleted,

    // Workflow
    TaskAssigned,
    TaskDueSoon,
    TaskOverdue,
    TaskCompleted,
    RequestSubmitted,
    RequestApproved,
    RequestRejected,
    RequestComment,

    // Calendar
    EventInvitation,
    EventReminder,
    EventUpdated,
    EventCancelled,
    RsvpReceived,
    TimeOffApproved,
    TimeOffRejected,

    // System
    SystemAnnouncement,
    SystemMaintenance,
    SecurityAlert,
    PasswordExpiring,
    LoginFromNewDevice,

    // Social
    NewFollower,
    UserMentioned,
    BirthdayReminder,
    WorkAnniversary
}

/// <summary>
/// Notification categories
/// </summary>
public enum NotificationCategory
{
    Content,
    Documents,
    Collaboration,
    Workflow,
    Calendar,
    Social,
    System,
    Security
}

/// <summary>
/// Notification priority
/// </summary>
public enum NotificationPriority
{
    Low,
    Normal,
    High,
    Urgent
}

/// <summary>
/// Delivery channels
/// </summary>
public enum DeliveryChannel
{
    InApp,
    Email,
    Push,
    SMS,
    Teams,
    Slack
}

/// <summary>
/// Delivery status
/// </summary>
public enum DeliveryStatus
{
    Pending,
    Queued,
    Sent,
    Delivered,
    Failed,
    Bounced,
    Opened,
    Clicked
}
