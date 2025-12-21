using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.WebApi.Data.Entities;

/// <summary>
/// External meeting link entity for database persistence
/// </summary>
public class ExternalMeetingLinkEntity : AuditableEntity
{
    public string ExternalMeetingId { get; set; } = string.Empty;
    public string ExternalSystem { get; set; } = string.Empty; // Teams, Outlook, Google, Zoom
    public string Title { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string? Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Location { get; set; }
    public bool IsOnline { get; set; }
    public string? OnlineMeetingUrl { get; set; }
    public string? RecurrencePattern { get; set; }
    public MeetingStatusEnum Status { get; set; } = MeetingStatusEnum.Scheduled;
    public string? OrganizerEmail { get; set; }
    public string? OrganizerName { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public string? ExternalData { get; set; } // JSON for additional external system data

    // Navigation
    public ICollection<MeetingDocumentLinkEntity> DocumentLinks { get; set; } = new List<MeetingDocumentLinkEntity>();
    public ICollection<MeetingAttendeeEntity> Attendees { get; set; } = new List<MeetingAttendeeEntity>();
    public ICollection<MeetingAgendaItemEntity> AgendaItems { get; set; } = new List<MeetingAgendaItemEntity>();
    public ICollection<MeetingActionItemEntity> ActionItems { get; set; } = new List<MeetingActionItemEntity>();
}

/// <summary>
/// Meeting document link entity
/// </summary>
public class MeetingDocumentLinkEntity : AuditableEntity
{
    public Guid MeetingId { get; set; }
    public Guid DocumentId { get; set; }
    public DocumentLinkTypeEnum LinkType { get; set; } = DocumentLinkTypeEnum.Reference;
    public string? Notes { get; set; }
    public int SortOrder { get; set; }
    public Guid LinkedById { get; set; }
    public string LinkedByName { get; set; } = string.Empty;

    // Navigation
    public ExternalMeetingLinkEntity? Meeting { get; set; }
}

/// <summary>
/// Meeting attendee entity
/// </summary>
public class MeetingAttendeeEntity : AuditableEntity
{
    public Guid MeetingId { get; set; }
    public Guid? UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AttendeeRoleEnum Role { get; set; } = AttendeeRoleEnum.Attendee;
    public AttendeeResponseEnum ResponseStatus { get; set; } = AttendeeResponseEnum.None;
    public bool IsOptional { get; set; }

    // Navigation
    public ExternalMeetingLinkEntity? Meeting { get; set; }
}

/// <summary>
/// Meeting agenda item entity
/// </summary>
public class MeetingAgendaItemEntity : AuditableEntity
{
    public Guid MeetingId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
    public int SortOrder { get; set; }
    public Guid? PresenterUserId { get; set; }
    public string? PresenterName { get; set; }
    public bool IsCompleted { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public ExternalMeetingLinkEntity? Meeting { get; set; }
}

/// <summary>
/// Meeting action item entity
/// </summary>
public class MeetingActionItemEntity : AuditableEntity
{
    public Guid MeetingId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToName { get; set; }
    public DateTime? DueDate { get; set; }
    public ActionItemStatusEnum Status { get; set; } = ActionItemStatusEnum.Pending;
    public ActionItemPriorityEnum Priority { get; set; } = ActionItemPriorityEnum.Medium;
    public DateTime? CompletedAt { get; set; }
    public int SortOrder { get; set; }

    // Navigation
    public ExternalMeetingLinkEntity? Meeting { get; set; }
}

// Enums
public enum MeetingStatusEnum
{
    Scheduled,
    InProgress,
    Completed,
    Cancelled,
    Postponed
}

public enum DocumentLinkTypeEnum
{
    Agenda,
    Presentation,
    Minutes,
    Reference,
    Attachment,
    ActionItem
}

public enum AttendeeRoleEnum
{
    Organizer,
    CoOrganizer,
    Presenter,
    Attendee
}

public enum AttendeeResponseEnum
{
    None,
    Accepted,
    Tentative,
    Declined
}

public enum ActionItemStatusEnum
{
    Pending,
    InProgress,
    Completed,
    Cancelled,
    Deferred
}

public enum ActionItemPriorityEnum
{
    Low,
    Medium,
    High,
    Critical
}
