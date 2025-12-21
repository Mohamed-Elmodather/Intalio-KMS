using AFC27.KMS.Notifications.Domain.Entities;

namespace AFC27.KMS.Notifications.Application.DTOs;

/// <summary>
/// User notification preferences DTO
/// </summary>
public class NotificationPreferencesDto
{
    public bool NotificationsEnabled { get; set; }
    public bool EmailEnabled { get; set; }
    public bool PushEnabled { get; set; }
    public bool SmsEnabled { get; set; }
    public bool InAppEnabled { get; set; }

    public DigestFrequency EmailDigestFrequency { get; set; }
    public int DigestHour { get; set; }
    public string TimeZone { get; set; } = "Asia/Riyadh";

    public bool QuietHoursEnabled { get; set; }
    public string QuietHoursStart { get; set; } = "22:00";
    public string QuietHoursEnd { get; set; } = "07:00";
    public bool AllowUrgentDuringQuietHours { get; set; }

    public string PreferredLanguage { get; set; } = "en";

    public List<CategoryPreferenceDto> CategoryPreferences { get; set; } = new();
    public List<TypePreferenceDto> TypePreferences { get; set; } = new();
    public List<MutedEntityDto> MutedEntities { get; set; } = new();
}

/// <summary>
/// Category preference DTO
/// </summary>
public class CategoryPreferenceDto
{
    public NotificationCategory Category { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryNameAr { get; set; } = string.Empty;
    public bool Enabled { get; set; }
    public List<DeliveryChannel> Channels { get; set; } = new();
}

/// <summary>
/// Type preference DTO
/// </summary>
public class TypePreferenceDto
{
    public NotificationType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string TypeNameAr { get; set; } = string.Empty;
    public NotificationCategory Category { get; set; }
    public bool Enabled { get; set; }
    public List<DeliveryChannel> Channels { get; set; } = new();
}

/// <summary>
/// Muted entity DTO
/// </summary>
public class MutedEntityDto
{
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string? EntityName { get; set; }
    public DateTime MutedAt { get; set; }
    public DateTime? MutedUntil { get; set; }
}

/// <summary>
/// Update preferences request
/// </summary>
public class UpdatePreferencesRequest
{
    public bool? NotificationsEnabled { get; set; }
    public bool? EmailEnabled { get; set; }
    public bool? PushEnabled { get; set; }
    public bool? SmsEnabled { get; set; }
    public bool? InAppEnabled { get; set; }

    public DigestFrequency? EmailDigestFrequency { get; set; }
    public int? DigestHour { get; set; }
    public string? TimeZone { get; set; }

    public bool? QuietHoursEnabled { get; set; }
    public string? QuietHoursStart { get; set; }
    public string? QuietHoursEnd { get; set; }
    public bool? AllowUrgentDuringQuietHours { get; set; }

    public string? PreferredLanguage { get; set; }
}

/// <summary>
/// Update category preference request
/// </summary>
public class UpdateCategoryPreferenceRequest
{
    public NotificationCategory Category { get; set; }
    public bool Enabled { get; set; }
    public List<DeliveryChannel>? Channels { get; set; }
}

/// <summary>
/// Update type preference request
/// </summary>
public class UpdateTypePreferenceRequest
{
    public NotificationType Type { get; set; }
    public bool Enabled { get; set; }
    public List<DeliveryChannel>? Channels { get; set; }
}

/// <summary>
/// Mute entity request
/// </summary>
public class MuteEntityRequest
{
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public int? MuteForHours { get; set; }
}

/// <summary>
/// Device registration DTO
/// </summary>
public class DeviceRegistrationDto
{
    public Guid Id { get; set; }
    public DevicePlatform Platform { get; set; }
    public string? DeviceName { get; set; }
    public string? DeviceModel { get; set; }
    public string? OsVersion { get; set; }
    public string? BrowserInfo { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastActiveAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Register device request
/// </summary>
public class RegisterDeviceRequest
{
    public string DeviceToken { get; set; } = string.Empty;
    public DevicePlatform Platform { get; set; }
    public string? DeviceName { get; set; }
    public string? DeviceModel { get; set; }
    public string? OsVersion { get; set; }
    public string? AppVersion { get; set; }
    public string? BrowserInfo { get; set; }

    // Web push specific
    public string? Endpoint { get; set; }
    public string? P256dhKey { get; set; }
    public string? AuthKey { get; set; }
}

/// <summary>
/// Subscription DTO
/// </summary>
public class SubscriptionDto
{
    public Guid Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string? EntityName { get; set; }
    public List<DeliveryChannel> Channels { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Subscribe request
/// </summary>
public class SubscribeRequest
{
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public List<DeliveryChannel>? Channels { get; set; }
}

/// <summary>
/// Available notification channels
/// </summary>
public class ChannelInfoDto
{
    public DeliveryChannel Channel { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public bool RequiresSetup { get; set; }
    public bool IsSetUp { get; set; }
}
