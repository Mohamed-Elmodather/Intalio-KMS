namespace AFC27.KMS.WebApi.Features.Meetings.Models;

/// <summary>
/// Represents a link to an external meeting
/// </summary>
public class ExternalMeetingLink
{
    public Guid Id { get; set; }
    public string ExternalMeetingId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime MeetingDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Location { get; set; }
    public string? TeamsLink { get; set; }
    public string ExternalSystemUrl { get; set; } = string.Empty;
    public MeetingStatus Status { get; set; } = MeetingStatus.Scheduled;
    public string? OrganizerEmail { get; set; }
    public string? OrganizerName { get; set; }
    public DateTime LastSyncedAt { get; set; } = DateTime.UtcNow;
    public string? CachedData { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<MeetingAttendee> Attendees { get; set; } = new();
    public List<MeetingDocumentLink> LinkedDocuments { get; set; } = new();
    public List<MeetingAgendaItem> AgendaItems { get; set; } = new();
    public List<MeetingActionItem> ActionItems { get; set; } = new();
}

/// <summary>
/// Links a document to a meeting
/// </summary>
public class MeetingDocumentLink
{
    public Guid Id { get; set; }
    public Guid MeetingLinkId { get; set; }
    public Guid DocumentId { get; set; }
    public string DocumentTitle { get; set; } = string.Empty;
    public DocumentLinkType LinkType { get; set; }
    public DateTime LinkedAt { get; set; } = DateTime.UtcNow;
    public Guid LinkedById { get; set; }
    public string? LinkedByName { get; set; }
}

public class MeetingAttendee
{
    public Guid Id { get; set; }
    public Guid MeetingLinkId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AttendeeRole Role { get; set; } = AttendeeRole.Attendee;
    public AttendeeResponse Response { get; set; } = AttendeeResponse.None;
    public bool IsOptional { get; set; }
}

public class MeetingAgendaItem
{
    public Guid Id { get; set; }
    public Guid MeetingLinkId { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
    public string? PresenterName { get; set; }
    public Guid? RelatedDocumentId { get; set; }
}

public class MeetingActionItem
{
    public Guid Id { get; set; }
    public Guid MeetingLinkId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? AssignedToEmail { get; set; }
    public string? AssignedToName { get; set; }
    public DateTime? DueDate { get; set; }
    public ActionItemStatus Status { get; set; } = ActionItemStatus.Open;
    public ActionItemPriority Priority { get; set; } = ActionItemPriority.Medium;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}

public enum MeetingStatus
{
    Scheduled,
    InProgress,
    Completed,
    Cancelled,
    Postponed
}

public enum DocumentLinkType
{
    Agenda,
    Minutes,
    Presentation,
    Reference,
    Attachment,
    Decision
}

public enum AttendeeRole
{
    Organizer,
    Presenter,
    Attendee,
    Optional
}

public enum AttendeeResponse
{
    None,
    Accepted,
    Tentative,
    Declined
}

public enum ActionItemStatus
{
    Open,
    InProgress,
    Completed,
    Cancelled,
    Deferred
}

public enum ActionItemPriority
{
    Low,
    Medium,
    High,
    Critical
}

// Request/Response Models

public class CreateMeetingLinkRequest
{
    public string ExternalMeetingId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string? Description { get; set; }
    public DateTime MeetingDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Location { get; set; }
    public string? TeamsLink { get; set; }
    public string ExternalSystemUrl { get; set; } = string.Empty;
}

public class LinkDocumentRequest
{
    public Guid DocumentId { get; set; }
    public DocumentLinkType LinkType { get; set; }
}

public class CreateAgendaItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
    public string? PresenterName { get; set; }
    public Guid? RelatedDocumentId { get; set; }
}

public class CreateActionItemRequest
{
    public string Description { get; set; } = string.Empty;
    public string? AssignedToEmail { get; set; }
    public string? AssignedToName { get; set; }
    public DateTime? DueDate { get; set; }
    public ActionItemPriority Priority { get; set; } = ActionItemPriority.Medium;
}

public class UpdateActionItemRequest
{
    public ActionItemStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class MeetingSummary
{
    public Guid Id { get; set; }
    public string ExternalMeetingId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime MeetingDate { get; set; }
    public MeetingStatus Status { get; set; }
    public int DocumentCount { get; set; }
    public int ActionItemCount { get; set; }
    public int PendingActionItems { get; set; }
    public string? OrganizerName { get; set; }
}

public class CalendarEvent
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public MeetingStatus Status { get; set; }
    public string? Location { get; set; }
    public bool HasDocuments { get; set; }
    public bool HasPendingActions { get; set; }
}

public class MeetingSyncResult
{
    public int MeetingsCreated { get; set; }
    public int MeetingsUpdated { get; set; }
    public int MeetingsCancelled { get; set; }
    public List<string> Errors { get; set; } = new();
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}
