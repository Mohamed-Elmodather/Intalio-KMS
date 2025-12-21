using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Notifications.Domain.Entities;

/// <summary>
/// User notification preferences
/// </summary>
public class NotificationPreferences : AuditableEntity
{
    /// <summary>
    /// User ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Global enable/disable notifications
    /// </summary>
    public bool NotificationsEnabled { get; set; } = true;

    /// <summary>
    /// Enable email notifications
    /// </summary>
    public bool EmailEnabled { get; set; } = true;

    /// <summary>
    /// Enable push notifications
    /// </summary>
    public bool PushEnabled { get; set; } = true;

    /// <summary>
    /// Enable SMS notifications
    /// </summary>
    public bool SmsEnabled { get; set; } = false;

    /// <summary>
    /// Enable in-app notifications
    /// </summary>
    public bool InAppEnabled { get; set; } = true;

    /// <summary>
    /// Email digest frequency
    /// </summary>
    public DigestFrequency EmailDigestFrequency { get; set; } = DigestFrequency.Instant;

    /// <summary>
    /// Preferred digest time (hour of day)
    /// </summary>
    public int DigestHour { get; set; } = 9;

    /// <summary>
    /// Time zone for digest
    /// </summary>
    public string TimeZone { get; set; } = "Asia/Riyadh";

    /// <summary>
    /// Quiet hours enabled
    /// </summary>
    public bool QuietHoursEnabled { get; set; }

    /// <summary>
    /// Quiet hours start
    /// </summary>
    public TimeSpan QuietHoursStart { get; set; } = new(22, 0, 0);

    /// <summary>
    /// Quiet hours end
    /// </summary>
    public TimeSpan QuietHoursEnd { get; set; } = new(7, 0, 0);

    /// <summary>
    /// Allow urgent notifications during quiet hours
    /// </summary>
    public bool AllowUrgentDuringQuietHours { get; set; } = true;

    /// <summary>
    /// Preferred language for notifications
    /// </summary>
    public string PreferredLanguage { get; set; } = "en";

    /// <summary>
    /// Per-category settings
    /// </summary>
    public List<CategoryPreference> CategoryPreferences { get; set; } = new();

    /// <summary>
    /// Per-type settings
    /// </summary>
    public List<TypePreference> TypePreferences { get; set; } = new();

    /// <summary>
    /// Muted entities (don't notify about these)
    /// </summary>
    public List<MutedEntity> MutedEntities { get; set; } = new();
}

/// <summary>
/// Category-level preference
/// </summary>
public class CategoryPreference
{
    public NotificationCategory Category { get; set; }
    public bool Enabled { get; set; } = true;
    public List<DeliveryChannel> Channels { get; set; } = new();
}

/// <summary>
/// Type-level preference
/// </summary>
public class TypePreference
{
    public NotificationType Type { get; set; }
    public bool Enabled { get; set; } = true;
    public List<DeliveryChannel> Channels { get; set; } = new();
}

/// <summary>
/// Muted entity (e.g., mute notifications from a specific discussion)
/// </summary>
public class MutedEntity
{
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public DateTime MutedAt { get; set; }
    public DateTime? MutedUntil { get; set; }
}

/// <summary>
/// Digest frequency options
/// </summary>
public enum DigestFrequency
{
    Instant,
    Hourly,
    Daily,
    Weekly,
    Never
}

/// <summary>
/// Push notification device registration
/// </summary>
public class DeviceRegistration : AuditableEntity
{
    /// <summary>
    /// User ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Device token/registration ID
    /// </summary>
    public string DeviceToken { get; set; } = string.Empty;

    /// <summary>
    /// Device platform
    /// </summary>
    public DevicePlatform Platform { get; set; }

    /// <summary>
    /// Device name
    /// </summary>
    public string? DeviceName { get; set; }

    /// <summary>
    /// Device model
    /// </summary>
    public string? DeviceModel { get; set; }

    /// <summary>
    /// OS version
    /// </summary>
    public string? OsVersion { get; set; }

    /// <summary>
    /// App version
    /// </summary>
    public string? AppVersion { get; set; }

    /// <summary>
    /// Browser info (for web push)
    /// </summary>
    public string? BrowserInfo { get; set; }

    /// <summary>
    /// Push subscription endpoint (for web push)
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// P256dh key (for web push)
    /// </summary>
    public string? P256dhKey { get; set; }

    /// <summary>
    /// Auth key (for web push)
    /// </summary>
    public string? AuthKey { get; set; }

    /// <summary>
    /// Is device active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Last active date
    /// </summary>
    public DateTime? LastActiveAt { get; set; }

    /// <summary>
    /// Last push sent
    /// </summary>
    public DateTime? LastPushAt { get; set; }

    /// <summary>
    /// Failed push count
    /// </summary>
    public int FailedPushCount { get; set; }
}

/// <summary>
/// Device platforms
/// </summary>
public enum DevicePlatform
{
    Web,
    iOS,
    Android,
    Windows,
    MacOS
}

/// <summary>
/// Notification digest (batched notifications)
/// </summary>
public class NotificationDigest : AuditableEntity
{
    public Guid UserId { get; set; }
    public DigestFrequency Frequency { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public int NotificationCount { get; set; }
    public DigestStatus Status { get; set; }
    public DateTime? SentAt { get; set; }
    public List<Guid> NotificationIds { get; set; } = new();

    /// <summary>
    /// Summary by category
    /// </summary>
    public Dictionary<NotificationCategory, int> CategorySummary { get; set; } = new();
}

/// <summary>
/// Digest status
/// </summary>
public enum DigestStatus
{
    Pending,
    Sent,
    Failed,
    Skipped
}

/// <summary>
/// Notification subscription (subscribe to entity notifications)
/// </summary>
public class NotificationSubscription : AuditableEntity
{
    public Guid UserId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public List<DeliveryChannel> Channels { get; set; } = new();
    public bool IsActive { get; set; } = true;
}
