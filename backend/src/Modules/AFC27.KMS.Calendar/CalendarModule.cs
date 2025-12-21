using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AFC27.KMS.Calendar;

/// <summary>
/// Calendar module registration and configuration
/// </summary>
public static class CalendarModule
{
    /// <summary>
    /// Add Calendar module services to DI container
    /// </summary>
    public static IServiceCollection AddCalendarModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure Calendar options
        services.Configure<CalendarOptions>(
            configuration.GetSection("Calendar"));

        // Configure Working Hours options
        services.Configure<WorkingHoursOptions>(
            configuration.GetSection("Calendar:WorkingHours"));

        // Configure Time Off options
        services.Configure<TimeOffOptions>(
            configuration.GetSection("Calendar:TimeOff"));

        // TODO: Register services
        // services.AddScoped<ICalendarService, CalendarService>();
        // services.AddScoped<IEventService, EventService>();
        // services.AddScoped<IHolidayService, HolidayService>();
        // services.AddScoped<ITimeOffService, TimeOffService>();
        // services.AddScoped<IRecurrenceService, RecurrenceService>();
        // services.AddScoped<IReminderService, ReminderService>();
        // services.AddScoped<IFreeBusyService, FreeBusyService>();

        // TODO: Register repositories
        // services.AddScoped<ICalendarRepository, CalendarRepository>();
        // services.AddScoped<IEventRepository, EventRepository>();
        // services.AddScoped<IAttendeeRepository, AttendeeRepository>();
        // services.AddScoped<IHolidayRepository, HolidayRepository>();
        // services.AddScoped<ITimeOffRepository, TimeOffRepository>();
        // services.AddScoped<IWorkingHoursRepository, WorkingHoursRepository>();

        // TODO: Register external sync services
        // services.AddScoped<IOutlookCalendarSync, OutlookCalendarSync>();
        // services.AddScoped<IGoogleCalendarSync, GoogleCalendarSync>();

        // Configure authorization policies
        services.AddAuthorizationBuilder()
            .AddPolicy("CanManageCalendar", policy =>
                policy.RequireClaim("permission", "calendar:manage"))
            .AddPolicy("CanCreateEvents", policy =>
                policy.RequireClaim("permission", "calendar:create-events"))
            .AddPolicy("CanApproveTimeOff", policy =>
                policy.RequireClaim("permission", "calendar:approve-timeoff"))
            .AddPolicy("CanManageHolidays", policy =>
                policy.RequireClaim("permission", "calendar:manage-holidays"));

        return services;
    }
}

/// <summary>
/// Calendar module configuration options
/// </summary>
public class CalendarOptions
{
    /// <summary>
    /// Enable calendar module
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Default time zone
    /// </summary>
    public string DefaultTimeZone { get; set; } = "Asia/Riyadh";

    /// <summary>
    /// Week start day (0 = Sunday for Saudi Arabia)
    /// </summary>
    public DayOfWeek WeekStartDay { get; set; } = DayOfWeek.Sunday;

    /// <summary>
    /// Default reminder minutes before event
    /// </summary>
    public int DefaultReminderMinutes { get; set; } = 30;

    /// <summary>
    /// Maximum attendees per event
    /// </summary>
    public int MaxAttendeesPerEvent { get; set; } = 500;

    /// <summary>
    /// Maximum recurring instances to generate
    /// </summary>
    public int MaxRecurringInstances { get; set; } = 365;

    /// <summary>
    /// Enable external calendar sync
    /// </summary>
    public bool EnableExternalSync { get; set; } = true;

    /// <summary>
    /// Sync interval in minutes
    /// </summary>
    public int SyncIntervalMinutes { get; set; } = 15;

    /// <summary>
    /// Enable RSVP for all events by default
    /// </summary>
    public bool DefaultAllowRsvp { get; set; } = true;

    /// <summary>
    /// Send invitation emails automatically
    /// </summary>
    public bool AutoSendInvitations { get; set; } = true;

    /// <summary>
    /// Send reminder notifications
    /// </summary>
    public bool SendReminders { get; set; } = true;

    /// <summary>
    /// Enable check-in feature
    /// </summary>
    public bool EnableCheckIn { get; set; } = true;

    /// <summary>
    /// Enable event feedback collection
    /// </summary>
    public bool EnableFeedback { get; set; } = true;

    /// <summary>
    /// Days ahead to show in agenda view
    /// </summary>
    public int AgendaDaysAhead { get; set; } = 14;

    /// <summary>
    /// Enable free/busy information sharing
    /// </summary>
    public bool EnableFreeBusySharing { get; set; } = true;

    /// <summary>
    /// Supported online meeting providers
    /// </summary>
    public List<string> SupportedMeetingProviders { get; set; } = new()
    {
        "MicrosoftTeams", "Zoom", "GoogleMeet", "Webex"
    };
}

/// <summary>
/// Working hours configuration options
/// </summary>
public class WorkingHoursOptions
{
    /// <summary>
    /// Default business hours start
    /// </summary>
    public TimeSpan DefaultStartTime { get; set; } = new TimeSpan(8, 0, 0);

    /// <summary>
    /// Default business hours end
    /// </summary>
    public TimeSpan DefaultEndTime { get; set; } = new TimeSpan(17, 0, 0);

    /// <summary>
    /// Default working days (0 = Sunday, 6 = Saturday)
    /// Saudi Arabia: Sunday to Thursday
    /// </summary>
    public List<int> DefaultWorkingDays { get; set; } = new() { 0, 1, 2, 3, 4 };

    /// <summary>
    /// Allow users to customize their working hours
    /// </summary>
    public bool AllowCustomization { get; set; } = true;

    /// <summary>
    /// Consider working hours when suggesting meeting times
    /// </summary>
    public bool UseForScheduling { get; set; } = true;
}

/// <summary>
/// Time off configuration options
/// </summary>
public class TimeOffOptions
{
    /// <summary>
    /// Enable time off requests
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Require approval for time off
    /// </summary>
    public bool RequireApproval { get; set; } = true;

    /// <summary>
    /// Auto-approve time off after days (0 = disabled)
    /// </summary>
    public int AutoApproveAfterDays { get; set; } = 0;

    /// <summary>
    /// Maximum consecutive vacation days
    /// </summary>
    public int MaxConsecutiveVacationDays { get; set; } = 21;

    /// <summary>
    /// Allow setting out-of-office auto-reply
    /// </summary>
    public bool AllowAutoReply { get; set; } = true;

    /// <summary>
    /// Allow delegation during time off
    /// </summary>
    public bool AllowDelegation { get; set; } = true;

    /// <summary>
    /// Show time off on team calendar
    /// </summary>
    public bool ShowOnTeamCalendar { get; set; } = true;

    /// <summary>
    /// Annual leave allocation by default
    /// </summary>
    public Dictionary<string, int> DefaultLeaveAllocation { get; set; } = new()
    {
        ["Vacation"] = 21,
        ["Sick"] = 10,
        ["Personal"] = 3
    };

    /// <summary>
    /// Allow carry-over of unused days
    /// </summary>
    public bool AllowCarryOver { get; set; } = true;

    /// <summary>
    /// Maximum carry-over days
    /// </summary>
    public int MaxCarryOverDays { get; set; } = 5;
}

/// <summary>
/// Calendar validation constants
/// </summary>
public static class CalendarValidation
{
    public const int MaxTitleLength = 200;
    public const int MaxDescriptionLength = 4000;
    public const int MaxLocationLength = 500;
    public const int MaxAttendeesPerEvent = 500;
    public const int MaxRemindersPerEvent = 5;
    public const int MaxAttachmentsPerEvent = 10;
    public const int MinReminderMinutes = 0;
    public const int MaxReminderMinutes = 40320; // 4 weeks
    public const int MaxRecurrenceYears = 5;
}
