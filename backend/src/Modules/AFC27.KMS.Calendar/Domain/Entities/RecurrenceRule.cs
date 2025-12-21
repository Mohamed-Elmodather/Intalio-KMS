using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Calendar.Domain.Entities;

/// <summary>
/// Recurrence rule for recurring events (RFC 5545 compatible)
/// </summary>
public class RecurrenceRule : Entity
{
    // Frequency
    public RecurrenceFrequency Frequency { get; set; }
    public int Interval { get; set; } = 1;

    // End conditions
    public RecurrenceEndType EndType { get; set; }
    public DateTime? EndDate { get; set; }
    public int? OccurrenceCount { get; set; }

    // Weekly pattern
    public List<DayOfWeek> ByDay { get; set; } = new();

    // Monthly pattern
    public List<int> ByMonthDay { get; set; } = new();
    public List<int> BySetPosition { get; set; } = new();
    public WeekOfMonth? WeekOfMonth { get; set; }

    // Yearly pattern
    public List<int> ByMonth { get; set; } = new();
    public List<int> ByYearDay { get; set; } = new();
    public List<int> ByWeekNo { get; set; } = new();

    // Week start
    public DayOfWeek WeekStart { get; set; } = DayOfWeek.Sunday;

    // Exceptions
    public List<DateTime> ExceptionDates { get; set; } = new();
    public List<DateTime> AdditionalDates { get; set; } = new();

    // iCalendar format
    public string? RRule { get; set; }

    // Navigation
    public ICollection<Event> Events { get; set; } = new List<Event>();

    /// <summary>
    /// Generate RRULE string (RFC 5545)
    /// </summary>
    public string ToRRule()
    {
        var parts = new List<string>
        {
            $"FREQ={Frequency.ToString().ToUpper()}"
        };

        if (Interval > 1)
            parts.Add($"INTERVAL={Interval}");

        if (EndType == RecurrenceEndType.EndDate && EndDate.HasValue)
            parts.Add($"UNTIL={EndDate.Value:yyyyMMddTHHmmssZ}");
        else if (EndType == RecurrenceEndType.Count && OccurrenceCount.HasValue)
            parts.Add($"COUNT={OccurrenceCount}");

        if (ByDay.Any())
            parts.Add($"BYDAY={string.Join(",", ByDay.Select(DayToRRule))}");

        if (ByMonthDay.Any())
            parts.Add($"BYMONTHDAY={string.Join(",", ByMonthDay)}");

        if (ByMonth.Any())
            parts.Add($"BYMONTH={string.Join(",", ByMonth)}");

        if (BySetPosition.Any())
            parts.Add($"BYSETPOS={string.Join(",", BySetPosition)}");

        parts.Add($"WKST={DayToRRule(WeekStart)}");

        return string.Join(";", parts);
    }

    private static string DayToRRule(DayOfWeek day) => day switch
    {
        DayOfWeek.Sunday => "SU",
        DayOfWeek.Monday => "MO",
        DayOfWeek.Tuesday => "TU",
        DayOfWeek.Wednesday => "WE",
        DayOfWeek.Thursday => "TH",
        DayOfWeek.Friday => "FR",
        DayOfWeek.Saturday => "SA",
        _ => "SU"
    };
}

/// <summary>
/// Recurrence frequency options
/// </summary>
public enum RecurrenceFrequency
{
    Daily,
    Weekly,
    Monthly,
    Yearly
}

/// <summary>
/// Recurrence end type
/// </summary>
public enum RecurrenceEndType
{
    Never,
    EndDate,
    Count
}

/// <summary>
/// Week of month for monthly recurrence
/// </summary>
public enum WeekOfMonth
{
    First = 1,
    Second = 2,
    Third = 3,
    Fourth = 4,
    Last = -1
}

/// <summary>
/// Holiday entity for organization-wide holidays
/// </summary>
public class Holiday : AuditableEntity
{
    public LocalizedString Name { get; set; } = new();
    public LocalizedString? Description { get; set; }

    public DateTime Date { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsRecurring { get; set; }
    public HolidayType Type { get; set; }

    public string? Country { get; set; }
    public string? Region { get; set; }

    public bool IsPublic { get; set; } = true;
    public bool AffectsWorkingHours { get; set; } = true;
    public int Year { get; set; }
}

/// <summary>
/// Holiday types
/// </summary>
public enum HolidayType
{
    National,
    Religious,
    Cultural,
    Company,
    Regional,
    Observance
}

/// <summary>
/// Working hours configuration
/// </summary>
public class WorkingHours : Entity
{
    public Guid? UserId { get; set; }
    public Guid? DepartmentId { get; set; }

    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsWorkingDay { get; set; } = true;

    public string TimeZone { get; set; } = "Asia/Riyadh";
}

/// <summary>
/// Time off / Out of office request
/// </summary>
public class TimeOff : AuditableEntity
{
    public Guid UserId { get; set; }
    public string? UserName { get; set; }

    public TimeOffType Type { get; set; }
    public LocalizedString? Reason { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsAllDay { get; set; } = true;

    public TimeOffStatus Status { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovalComment { get; set; }

    // Auto-reply settings
    public bool SetAutoReply { get; set; }
    public LocalizedString? AutoReplyMessage { get; set; }

    // Delegate
    public Guid? DelegateToId { get; set; }
    public string? DelegateToName { get; set; }
}

/// <summary>
/// Time off types
/// </summary>
public enum TimeOffType
{
    Vacation,
    Sick,
    Personal,
    Maternity,
    Paternity,
    Bereavement,
    Training,
    Conference,
    Remote,
    Other
}

/// <summary>
/// Time off status
/// </summary>
public enum TimeOffStatus
{
    Pending,
    Approved,
    Rejected,
    Cancelled
}
