using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Calendar.Domain.Entities;

/// <summary>
/// Event attendee with RSVP status
/// </summary>
public class EventAttendee : AuditableEntity
{
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;

    // Attendee info
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Department { get; set; }

    // RSVP
    public RsvpStatus RsvpStatus { get; set; } = RsvpStatus.Pending;
    public DateTime? RsvpDate { get; set; }
    public string? RsvpComment { get; set; }
    public int? GuestCount { get; set; }

    // Role
    public AttendeeRole Role { get; set; } = AttendeeRole.Required;
    public bool IsOrganizer { get; set; }
    public bool CanModify { get; set; }
    public bool CanInviteOthers { get; set; }

    // Notifications
    public bool NotificationSent { get; set; }
    public DateTime? NotificationSentAt { get; set; }
    public bool ReminderSent { get; set; }
    public DateTime? ReminderSentAt { get; set; }

    // Check-in
    public bool CheckedIn { get; set; }
    public DateTime? CheckInTime { get; set; }
    public string? CheckInMethod { get; set; }

    // Feedback
    public int? Rating { get; set; }
    public string? Feedback { get; set; }
    public DateTime? FeedbackDate { get; set; }

    // External reference
    public string? ExternalAttendeeId { get; set; }
}

/// <summary>
/// RSVP status options
/// </summary>
public enum RsvpStatus
{
    Pending,
    Accepted,
    Declined,
    Tentative,
    NoResponse
}

/// <summary>
/// Attendee role types
/// </summary>
public enum AttendeeRole
{
    Required,
    Optional,
    Resource,
    Presenter,
    Moderator,
    Observer
}

/// <summary>
/// Event reminder configuration
/// </summary>
public class EventReminder : Entity
{
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;

    public Guid? UserId { get; set; }

    public ReminderType Type { get; set; }
    public ReminderMethod Method { get; set; }
    public int MinutesBefore { get; set; }

    public bool IsSent { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? ScheduledAt { get; set; }
}

/// <summary>
/// Reminder types
/// </summary>
public enum ReminderType
{
    Default,
    Custom,
    AllDay
}

/// <summary>
/// Reminder delivery methods
/// </summary>
public enum ReminderMethod
{
    Email,
    Push,
    InApp,
    SMS,
    Popup
}
