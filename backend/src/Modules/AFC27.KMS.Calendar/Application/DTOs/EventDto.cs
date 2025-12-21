using AFC27.KMS.Calendar.Domain.Entities;

namespace AFC27.KMS.Calendar.Application.DTOs;

/// <summary>
/// Full event DTO
/// </summary>
public class EventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public EventType Type { get; set; }
    public EventStatus Status { get; set; }

    public Guid CalendarId { get; set; }
    public string? CalendarName { get; set; }
    public string? CalendarColor { get; set; }

    // Timing
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsAllDay { get; set; }
    public string TimeZone { get; set; } = "Asia/Riyadh";
    public string? Duration { get; set; }

    // Location
    public string? Location { get; set; }
    public string? LocationAr { get; set; }
    public string? LocationUrl { get; set; }
    public bool IsOnline { get; set; }
    public string? OnlineMeetingUrl { get; set; }
    public OnlineMeetingProvider? MeetingProvider { get; set; }

    // Organizer
    public Guid OrganizerId { get; set; }
    public string? OrganizerName { get; set; }
    public string? OrganizerEmail { get; set; }
    public string? OrganizerAvatar { get; set; }

    // Recurrence
    public bool IsRecurring { get; set; }
    public RecurrenceRuleDto? RecurrenceRule { get; set; }
    public Guid? ParentEventId { get; set; }

    // Visual
    public string? Color { get; set; }
    public string? Icon { get; set; }

    // Settings
    public EventVisibility Visibility { get; set; }
    public bool ShowAsBusy { get; set; }
    public bool AllowRsvp { get; set; }
    public int? MaxAttendees { get; set; }
    public bool RequiresApproval { get; set; }
    public DateTime? RsvpDeadline { get; set; }

    // Categories
    public string? Category { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> AttachmentUrls { get; set; } = new();

    // Attendees
    public List<AttendeeDto> Attendees { get; set; } = new();
    public int AttendeeCount { get; set; }
    public int AcceptedCount { get; set; }
    public int DeclinedCount { get; set; }
    public int PendingCount { get; set; }

    // Reminders
    public List<ReminderDto> Reminders { get; set; } = new();

    // Current user's status
    public RsvpStatus? MyRsvpStatus { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool IsOrganizer { get; set; }

    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}

/// <summary>
/// Event summary for calendar view
/// </summary>
public class EventSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public EventType Type { get; set; }
    public EventStatus Status { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsAllDay { get; set; }

    public string? Location { get; set; }
    public bool IsOnline { get; set; }

    public string? Color { get; set; }
    public Guid CalendarId { get; set; }
    public string? CalendarColor { get; set; }

    public bool IsRecurring { get; set; }
    public bool ShowAsBusy { get; set; }

    public RsvpStatus? MyRsvpStatus { get; set; }
    public int AttendeeCount { get; set; }
}

/// <summary>
/// Create event request
/// </summary>
public class CreateEventRequest
{
    public string TitleEn { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public EventType Type { get; set; }

    public Guid CalendarId { get; set; }

    // Timing
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsAllDay { get; set; }
    public string TimeZone { get; set; } = "Asia/Riyadh";

    // Location
    public string? LocationEn { get; set; }
    public string? LocationAr { get; set; }
    public string? LocationUrl { get; set; }
    public bool IsOnline { get; set; }
    public OnlineMeetingProvider? MeetingProvider { get; set; }
    public bool CreateOnlineMeeting { get; set; }

    // Visual
    public string? Color { get; set; }

    // Settings
    public EventVisibility Visibility { get; set; } = EventVisibility.Default;
    public bool ShowAsBusy { get; set; } = true;
    public bool AllowRsvp { get; set; } = true;
    public int? MaxAttendees { get; set; }
    public bool RequiresApproval { get; set; }
    public DateTime? RsvpDeadline { get; set; }

    // Categories
    public string? Category { get; set; }
    public List<string> Tags { get; set; } = new();

    // Recurrence
    public CreateRecurrenceRequest? Recurrence { get; set; }

    // Attendees
    public List<AddAttendeeRequest> Attendees { get; set; } = new();

    // Reminders
    public List<CreateReminderRequest> Reminders { get; set; } = new();

    // Notifications
    public bool SendInvitations { get; set; } = true;
}

/// <summary>
/// Update event request
/// </summary>
public class UpdateEventRequest : CreateEventRequest
{
    public UpdateScope UpdateScope { get; set; } = UpdateScope.ThisEvent;
    public bool NotifyAttendees { get; set; } = true;
}

/// <summary>
/// Scope for updating recurring events
/// </summary>
public enum UpdateScope
{
    ThisEvent,
    ThisAndFuture,
    AllEvents
}

/// <summary>
/// Event filter/query request
/// </summary>
public class EventFilterRequest
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<Guid>? CalendarIds { get; set; }
    public EventType? Type { get; set; }
    public EventStatus? Status { get; set; }
    public string? Search { get; set; }
    public bool IncludeDeclined { get; set; }
    public bool ExpandRecurring { get; set; } = true;
    public string? TimeZone { get; set; }
}

/// <summary>
/// Recurrence rule DTO
/// </summary>
public class RecurrenceRuleDto
{
    public RecurrenceFrequency Frequency { get; set; }
    public int Interval { get; set; } = 1;
    public RecurrenceEndType EndType { get; set; }
    public DateTime? EndDate { get; set; }
    public int? OccurrenceCount { get; set; }
    public List<DayOfWeek> ByDay { get; set; } = new();
    public List<int> ByMonthDay { get; set; } = new();
    public WeekOfMonth? WeekOfMonth { get; set; }
    public List<int> ByMonth { get; set; } = new();
    public DayOfWeek WeekStart { get; set; } = DayOfWeek.Sunday;
    public string? RRule { get; set; }
    public string? HumanReadable { get; set; }
}

/// <summary>
/// Create recurrence request
/// </summary>
public class CreateRecurrenceRequest
{
    public RecurrenceFrequency Frequency { get; set; }
    public int Interval { get; set; } = 1;
    public RecurrenceEndType EndType { get; set; } = RecurrenceEndType.Never;
    public DateTime? EndDate { get; set; }
    public int? OccurrenceCount { get; set; }
    public List<DayOfWeek>? ByDay { get; set; }
    public List<int>? ByMonthDay { get; set; }
    public WeekOfMonth? WeekOfMonth { get; set; }
    public List<int>? ByMonth { get; set; }
}

/// <summary>
/// Reminder DTO
/// </summary>
public class ReminderDto
{
    public Guid Id { get; set; }
    public ReminderType Type { get; set; }
    public ReminderMethod Method { get; set; }
    public int MinutesBefore { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public bool IsSent { get; set; }
}

/// <summary>
/// Create reminder request
/// </summary>
public class CreateReminderRequest
{
    public ReminderMethod Method { get; set; } = ReminderMethod.Email;
    public int MinutesBefore { get; set; } = 30;
}

/// <summary>
/// Free/busy time slot
/// </summary>
public class FreeBusySlotDto
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public FreeBusyStatus Status { get; set; }
    public string? EventTitle { get; set; }
}

/// <summary>
/// Free/busy status
/// </summary>
public enum FreeBusyStatus
{
    Free,
    Tentative,
    Busy,
    OutOfOffice
}

/// <summary>
/// Free/busy query request
/// </summary>
public class FreeBusyRequest
{
    public List<Guid> UserIds { get; set; } = new();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? TimeZone { get; set; }
}

/// <summary>
/// Free/busy response
/// </summary>
public class FreeBusyResponseDto
{
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public List<FreeBusySlotDto> Slots { get; set; } = new();
}

/// <summary>
/// Suggested meeting times
/// </summary>
public class MeetingTimeSuggestionDto
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int AvailableAttendeesCount { get; set; }
    public int TotalAttendeesCount { get; set; }
    public double AvailabilityScore { get; set; }
    public List<string> UnavailableAttendees { get; set; } = new();
}

/// <summary>
/// Find meeting times request
/// </summary>
public class FindMeetingTimesRequest
{
    public List<Guid> AttendeeIds { get; set; } = new();
    public int DurationMinutes { get; set; } = 60;
    public DateTime SearchStart { get; set; }
    public DateTime SearchEnd { get; set; }
    public TimeSpan? PreferredStartTime { get; set; }
    public TimeSpan? PreferredEndTime { get; set; }
    public int MaxSuggestions { get; set; } = 5;
}
