using AFC27.KMS.Calendar.Domain.Entities;

namespace AFC27.KMS.Calendar.Application.DTOs;

/// <summary>
/// Attendee DTO
/// </summary>
public class AttendeeDto
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }

    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Avatar { get; set; }
    public string? Department { get; set; }

    public RsvpStatus RsvpStatus { get; set; }
    public DateTime? RsvpDate { get; set; }
    public string? RsvpComment { get; set; }
    public int? GuestCount { get; set; }

    public AttendeeRole Role { get; set; }
    public bool IsOrganizer { get; set; }
    public bool CanModify { get; set; }

    public bool CheckedIn { get; set; }
    public DateTime? CheckInTime { get; set; }
}

/// <summary>
/// Add attendee request
/// </summary>
public class AddAttendeeRequest
{
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public AttendeeRole Role { get; set; } = AttendeeRole.Required;
    public bool CanModify { get; set; }
    public bool CanInviteOthers { get; set; }
}

/// <summary>
/// RSVP request
/// </summary>
public class RsvpRequest
{
    public RsvpStatus Status { get; set; }
    public string? Comment { get; set; }
    public int? GuestCount { get; set; }
    public RsvpScope Scope { get; set; } = RsvpScope.ThisEvent;
}

/// <summary>
/// RSVP scope for recurring events
/// </summary>
public enum RsvpScope
{
    ThisEvent,
    ThisAndFuture,
    AllEvents
}

/// <summary>
/// Check-in request
/// </summary>
public class CheckInRequest
{
    public string? Method { get; set; }
    public string? Location { get; set; }
}

/// <summary>
/// Attendee feedback request
/// </summary>
public class AttendeeFeedbackRequest
{
    public int Rating { get; set; }
    public string? Feedback { get; set; }
}

/// <summary>
/// Holiday DTO
/// </summary>
public class HolidayDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }

    public DateTime Date { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsRecurring { get; set; }
    public HolidayType Type { get; set; }

    public string? Country { get; set; }
    public string? Region { get; set; }

    public bool IsPublic { get; set; }
    public bool AffectsWorkingHours { get; set; }
    public int Year { get; set; }
}

/// <summary>
/// Create holiday request
/// </summary>
public class CreateHolidayRequest
{
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }

    public DateTime Date { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsRecurring { get; set; }
    public HolidayType Type { get; set; }

    public string? Country { get; set; }
    public string? Region { get; set; }

    public bool IsPublic { get; set; } = true;
    public bool AffectsWorkingHours { get; set; } = true;
}

/// <summary>
/// Time off DTO
/// </summary>
public class TimeOffDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserAvatar { get; set; }

    public TimeOffType Type { get; set; }
    public string? Reason { get; set; }
    public string? ReasonAr { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsAllDay { get; set; }
    public int TotalDays { get; set; }

    public TimeOffStatus Status { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovalComment { get; set; }

    public Guid? DelegateToId { get; set; }
    public string? DelegateToName { get; set; }

    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Create time off request
/// </summary>
public class CreateTimeOffRequest
{
    public TimeOffType Type { get; set; }
    public string? ReasonEn { get; set; }
    public string? ReasonAr { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsAllDay { get; set; } = true;

    public bool SetAutoReply { get; set; }
    public string? AutoReplyMessageEn { get; set; }
    public string? AutoReplyMessageAr { get; set; }

    public Guid? DelegateToId { get; set; }
}

/// <summary>
/// Approve/reject time off request
/// </summary>
public class TimeOffApprovalRequest
{
    public bool Approve { get; set; }
    public string? Comment { get; set; }
}

/// <summary>
/// Working hours DTO
/// </summary>
public class WorkingHoursDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsWorkingDay { get; set; }
}

/// <summary>
/// Update working hours request
/// </summary>
public class UpdateWorkingHoursRequest
{
    public List<WorkingHoursDto> WorkingHours { get; set; } = new();
    public string TimeZone { get; set; } = "Asia/Riyadh";
}

/// <summary>
/// Calendar export request
/// </summary>
public class CalendarExportRequest
{
    public List<Guid>? CalendarIds { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ExportFormat Format { get; set; } = ExportFormat.ICalendar;
}

/// <summary>
/// Export format
/// </summary>
public enum ExportFormat
{
    ICalendar,
    Csv,
    Json
}

/// <summary>
/// Calendar import request
/// </summary>
public class CalendarImportRequest
{
    public Guid CalendarId { get; set; }
    public string? ICalendarData { get; set; }
    public string? FileUrl { get; set; }
    public bool MergeWithExisting { get; set; }
}

/// <summary>
/// Import result
/// </summary>
public class ImportResultDto
{
    public int EventsImported { get; set; }
    public int EventsSkipped { get; set; }
    public int EventsUpdated { get; set; }
    public List<string> Errors { get; set; } = new();
}

/// <summary>
/// Calendar statistics
/// </summary>
public class CalendarStatisticsDto
{
    public int TotalEvents { get; set; }
    public int UpcomingEvents { get; set; }
    public int PastEvents { get; set; }
    public int TodayEvents { get; set; }
    public int ThisWeekEvents { get; set; }
    public int RecurringEvents { get; set; }

    public Dictionary<EventType, int> EventsByType { get; set; } = new();
    public Dictionary<RsvpStatus, int> RsvpBreakdown { get; set; } = new();

    public double AverageAttendeeCount { get; set; }
    public double AverageAcceptanceRate { get; set; }
}

/// <summary>
/// Agenda view DTO
/// </summary>
public class AgendaItemDto
{
    public DateTime Date { get; set; }
    public bool IsToday { get; set; }
    public bool IsWeekend { get; set; }
    public bool IsHoliday { get; set; }
    public string? HolidayName { get; set; }
    public List<EventSummaryDto> Events { get; set; } = new();
}
