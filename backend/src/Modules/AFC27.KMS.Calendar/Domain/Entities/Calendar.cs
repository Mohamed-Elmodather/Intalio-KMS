using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Calendar.Domain.Entities;

/// <summary>
/// Calendar container for organizing events
/// </summary>
public class Calendar : AuditableEntity
{
    public LocalizedString Name { get; set; } = new();
    public LocalizedString? Description { get; set; }
    public CalendarType Type { get; set; }
    public CalendarVisibility Visibility { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }

    // Owner information
    public Guid OwnerId { get; set; }
    public OwnerType OwnerType { get; set; }

    // Settings
    public string TimeZone { get; set; } = "Asia/Riyadh";
    public DayOfWeek WeekStartDay { get; set; } = DayOfWeek.Sunday;
    public bool ShowWeekNumbers { get; set; }
    public string DefaultView { get; set; } = "month";
    public int DefaultReminderMinutes { get; set; } = 30;

    // Sync settings
    public bool SyncEnabled { get; set; }
    public string? ExternalCalendarId { get; set; }
    public CalendarSyncProvider? SyncProvider { get; set; }
    public DateTime? LastSyncAt { get; set; }

    // Status
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Event> Events { get; set; } = new List<Event>();
    public ICollection<CalendarShare> Shares { get; set; } = new List<CalendarShare>();
}

/// <summary>
/// Calendar sharing with users/groups
/// </summary>
public class CalendarShare : AuditableEntity
{
    public Guid CalendarId { get; set; }
    public Calendar Calendar { get; set; } = null!;

    public Guid ShareWithId { get; set; }
    public ShareWithType ShareWithType { get; set; }

    public CalendarPermission Permission { get; set; }
    public bool CanEdit { get; set; }
    public bool CanInvite { get; set; }
    public bool CanSeeDetails { get; set; } = true;
}

/// <summary>
/// Calendar types
/// </summary>
public enum CalendarType
{
    Personal,
    Team,
    Department,
    Organization,
    Project,
    Meeting,
    Holiday,
    Training,
    External
}

/// <summary>
/// Calendar visibility levels
/// </summary>
public enum CalendarVisibility
{
    Private,
    FreeBusyOnly,
    Limited,
    Public
}

/// <summary>
/// Owner type for calendar
/// </summary>
public enum OwnerType
{
    User,
    Team,
    Department,
    Organization
}

/// <summary>
/// Share with type
/// </summary>
public enum ShareWithType
{
    User,
    Group,
    Department,
    Role,
    Everyone
}

/// <summary>
/// Calendar permission levels
/// </summary>
public enum CalendarPermission
{
    FreeBusyOnly,
    ReadOnly,
    ReadWrite,
    FullAccess
}

/// <summary>
/// Calendar sync providers
/// </summary>
public enum CalendarSyncProvider
{
    Outlook,
    Google,
    Apple,
    Exchange
}
