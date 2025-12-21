using AFC27.KMS.Calendar.Domain.Entities;

namespace AFC27.KMS.Calendar.Application.DTOs;

/// <summary>
/// Full calendar DTO
/// </summary>
public class CalendarDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public CalendarType Type { get; set; }
    public CalendarVisibility Visibility { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }

    public Guid OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public OwnerType OwnerType { get; set; }

    public string TimeZone { get; set; } = "Asia/Riyadh";
    public DayOfWeek WeekStartDay { get; set; }
    public bool ShowWeekNumbers { get; set; }
    public string DefaultView { get; set; } = "month";
    public int DefaultReminderMinutes { get; set; }

    public bool SyncEnabled { get; set; }
    public CalendarSyncProvider? SyncProvider { get; set; }
    public DateTime? LastSyncAt { get; set; }

    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }

    public int EventCount { get; set; }
    public List<CalendarShareDto> Shares { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}

/// <summary>
/// Calendar summary DTO
/// </summary>
public class CalendarSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public CalendarType Type { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public bool IsDefault { get; set; }
    public int EventCount { get; set; }
    public CalendarPermission? MyPermission { get; set; }
}

/// <summary>
/// Create calendar request
/// </summary>
public class CreateCalendarRequest
{
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public CalendarType Type { get; set; }
    public CalendarVisibility Visibility { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public string TimeZone { get; set; } = "Asia/Riyadh";
    public DayOfWeek WeekStartDay { get; set; } = DayOfWeek.Sunday;
    public bool ShowWeekNumbers { get; set; }
    public string DefaultView { get; set; } = "month";
    public int DefaultReminderMinutes { get; set; } = 30;
    public bool IsDefault { get; set; }
}

/// <summary>
/// Calendar share DTO
/// </summary>
public class CalendarShareDto
{
    public Guid Id { get; set; }
    public Guid ShareWithId { get; set; }
    public string? ShareWithName { get; set; }
    public ShareWithType ShareWithType { get; set; }
    public CalendarPermission Permission { get; set; }
    public bool CanEdit { get; set; }
    public bool CanInvite { get; set; }
    public bool CanSeeDetails { get; set; }
}

/// <summary>
/// Share calendar request
/// </summary>
public class ShareCalendarRequest
{
    public Guid ShareWithId { get; set; }
    public ShareWithType ShareWithType { get; set; }
    public CalendarPermission Permission { get; set; }
    public bool CanEdit { get; set; }
    public bool CanInvite { get; set; }
    public bool CanSeeDetails { get; set; } = true;
}

/// <summary>
/// Calendar filter request
/// </summary>
public class CalendarFilterRequest
{
    public CalendarType? Type { get; set; }
    public bool? IncludeShared { get; set; }
    public bool? ActiveOnly { get; set; }
    public string? Search { get; set; }
}

/// <summary>
/// Sync calendar request
/// </summary>
public class SyncCalendarRequest
{
    public CalendarSyncProvider Provider { get; set; }
    public string? ExternalCalendarId { get; set; }
    public string? AccessToken { get; set; }
    public bool TwoWaySync { get; set; }
}

/// <summary>
/// Calendar settings DTO
/// </summary>
public class CalendarSettingsDto
{
    public string TimeZone { get; set; } = "Asia/Riyadh";
    public DayOfWeek WeekStartDay { get; set; }
    public bool ShowWeekNumbers { get; set; }
    public string DefaultView { get; set; } = "month";
    public int DefaultReminderMinutes { get; set; }
    public bool ShowDeclinedEvents { get; set; }
    public bool ShowWeekends { get; set; } = true;
    public TimeSpan WorkingHoursStart { get; set; } = new(8, 0, 0);
    public TimeSpan WorkingHoursEnd { get; set; } = new(17, 0, 0);
    public List<DayOfWeek> WorkingDays { get; set; } = new() {
        DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday,
        DayOfWeek.Wednesday, DayOfWeek.Thursday
    };
}
