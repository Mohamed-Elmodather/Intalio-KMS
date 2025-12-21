using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Calendar.Domain.Entities;

/// <summary>
/// Calendar event entity
/// </summary>
public class Event : AuditableEntity
{
    public LocalizedString Title { get; set; } = new();
    public LocalizedString? Description { get; set; }
    public EventType Type { get; set; }
    public EventStatus Status { get; set; }

    // Calendar reference
    public Guid CalendarId { get; set; }
    public Calendar Calendar { get; set; } = null!;

    // Timing
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsAllDay { get; set; }
    public string TimeZone { get; set; } = "Asia/Riyadh";

    // Location
    public LocalizedString? Location { get; set; }
    public string? LocationUrl { get; set; }
    public bool IsOnline { get; set; }
    public string? OnlineMeetingUrl { get; set; }
    public OnlineMeetingProvider? MeetingProvider { get; set; }
    public string? MeetingId { get; set; }

    // Organizer
    public Guid OrganizerId { get; set; }
    public string? OrganizerName { get; set; }
    public string? OrganizerEmail { get; set; }

    // Recurrence
    public bool IsRecurring { get; set; }
    public Guid? RecurrenceRuleId { get; set; }
    public RecurrenceRule? RecurrenceRule { get; set; }
    public Guid? ParentEventId { get; set; }
    public Event? ParentEvent { get; set; }
    public DateTime? RecurrenceExceptionDate { get; set; }

    // Visual
    public string? Color { get; set; }
    public string? Icon { get; set; }

    // Settings
    public EventVisibility Visibility { get; set; } = EventVisibility.Default;
    public bool ShowAsBusy { get; set; } = true;
    public bool AllowRsvp { get; set; } = true;
    public int? MaxAttendees { get; set; }
    public bool RequiresApproval { get; set; }
    public DateTime? RsvpDeadline { get; set; }

    // Categories and tags
    public string? Category { get; set; }
    public List<string> Tags { get; set; } = new();

    // Attachments
    public List<string> AttachmentUrls { get; set; } = new();

    // External sync
    public string? ExternalEventId { get; set; }
    public DateTime? LastSyncAt { get; set; }

    // Statistics
    public int ViewCount { get; set; }

    // Navigation
    public ICollection<EventAttendee> Attendees { get; set; } = new List<EventAttendee>();
    public ICollection<EventReminder> Reminders { get; set; } = new List<EventReminder>();
    public ICollection<Event> Exceptions { get; set; } = new List<Event>();
}

/// <summary>
/// Event types
/// </summary>
public enum EventType
{
    Meeting,
    Appointment,
    Conference,
    Workshop,
    Training,
    Webinar,
    Holiday,
    Birthday,
    Anniversary,
    Deadline,
    Reminder,
    Task,
    OutOfOffice,
    Travel,
    Social,
    Sports,
    Other
}

/// <summary>
/// Event status
/// </summary>
public enum EventStatus
{
    Tentative,
    Confirmed,
    Cancelled,
    Postponed,
    Completed
}

/// <summary>
/// Event visibility levels
/// </summary>
public enum EventVisibility
{
    Default,
    Public,
    Private,
    Confidential
}

/// <summary>
/// Online meeting providers
/// </summary>
public enum OnlineMeetingProvider
{
    MicrosoftTeams,
    Zoom,
    GoogleMeet,
    Webex,
    Custom
}
