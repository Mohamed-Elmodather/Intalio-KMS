using System;
using System.Collections.Generic;

namespace AFC27.KMS.WebApi.Integration.Meeting.Models;

/// <summary>
/// Request to create a meeting
/// </summary>
public class CreateMeetingRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string TimeZone { get; set; } = "UTC";
    public MeetingType Type { get; set; } = MeetingType.Virtual;
    public string? Location { get; set; }
    public string? VirtualMeetingUrl { get; set; }
    public List<MeetingParticipant> Participants { get; set; } = new();
    public MeetingSettings Settings { get; set; } = new();
    public List<MeetingAgendaItem>? Agenda { get; set; }
    public List<Guid>? AttachedDocumentIds { get; set; }
    public Guid? RelatedCommitteeId { get; set; }
    public string? RecurrencePattern { get; set; }
}

public enum MeetingType
{
    Virtual,
    InPerson,
    Hybrid
}

/// <summary>
/// Meeting participant information
/// </summary>
public class MeetingParticipant
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ParticipantRole Role { get; set; } = ParticipantRole.Attendee;
    public bool IsRequired { get; set; } = true;
    public ResponseStatus ResponseStatus { get; set; } = ResponseStatus.Pending;
}

public enum ParticipantRole
{
    Organizer,
    Chairperson,
    Secretary,
    Presenter,
    Attendee,
    Guest
}

public enum ResponseStatus
{
    Pending,
    Accepted,
    Declined,
    Tentative,
    NoResponse
}

/// <summary>
/// Meeting settings
/// </summary>
public class MeetingSettings
{
    public bool EnableRecording { get; set; } = false;
    public bool EnableTranscription { get; set; } = false;
    public bool EnableChat { get; set; } = true;
    public bool RequirePassword { get; set; } = false;
    public string? Password { get; set; }
    public bool EnableWaitingRoom { get; set; } = true;
    public bool AllowGuestAccess { get; set; } = false;
    public int? MaxParticipants { get; set; }
    public bool AutoRecordMinutes { get; set; } = true;
    public List<string>? AllowedDomains { get; set; }
}

/// <summary>
/// Meeting agenda item
/// </summary>
public class MeetingAgendaItem
{
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
    public string? PresenterEmail { get; set; }
    public AgendaItemType Type { get; set; } = AgendaItemType.Discussion;
    public List<Guid>? AttachedDocumentIds { get; set; }
}

public enum AgendaItemType
{
    Information,
    Discussion,
    Decision,
    Action,
    Presentation,
    Break
}

/// <summary>
/// Response when creating a meeting
/// </summary>
public class MeetingResponse
{
    public string MeetingId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public MeetingStatus Status { get; set; }
    public string? JoinUrl { get; set; }
    public string? HostUrl { get; set; }
    public string? DialInNumber { get; set; }
    public string? DialInPin { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<MeetingParticipant> Participants { get; set; } = new();
}

public enum MeetingStatus
{
    Scheduled,
    InProgress,
    Completed,
    Cancelled,
    Rescheduled
}

/// <summary>
/// Meeting minutes model
/// </summary>
public class MeetingMinutes
{
    public string MeetingId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime MeetingDate { get; set; }
    public List<string> Attendees { get; set; } = new();
    public List<string> Absentees { get; set; } = new();
    public List<MinutesItem> Items { get; set; } = new();
    public List<ActionItem> ActionItems { get; set; } = new();
    public List<Decision> Decisions { get; set; } = new();
    public DateTime? NextMeetingDate { get; set; }
    public string? TranscriptUrl { get; set; }
    public string? RecordingUrl { get; set; }
    public MinutesStatus Status { get; set; } = MinutesStatus.Draft;
}

public enum MinutesStatus
{
    Draft,
    PendingApproval,
    Approved,
    Published
}

/// <summary>
/// Minutes item
/// </summary>
public class MinutesItem
{
    public int Order { get; set; }
    public string AgendaItemTitle { get; set; } = string.Empty;
    public string Discussion { get; set; } = string.Empty;
    public string? Outcome { get; set; }
    public List<string>? Notes { get; set; }
}

/// <summary>
/// Action item from meeting
/// </summary>
public class ActionItem
{
    public string ActionId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AssigneeEmail { get; set; } = string.Empty;
    public string AssigneeName { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public ActionItemPriority Priority { get; set; } = ActionItemPriority.Medium;
    public ActionItemStatus Status { get; set; } = ActionItemStatus.Open;
}

public enum ActionItemPriority
{
    Low,
    Medium,
    High,
    Critical
}

public enum ActionItemStatus
{
    Open,
    InProgress,
    Completed,
    Cancelled,
    Overdue
}

/// <summary>
/// Decision made in meeting
/// </summary>
public class Decision
{
    public string DecisionId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Rationale { get; set; }
    public List<string> ApprovedBy { get; set; } = new();
    public DateTime DecisionDate { get; set; }
    public DecisionStatus Status { get; set; } = DecisionStatus.Approved;
}

public enum DecisionStatus
{
    Proposed,
    Approved,
    Rejected,
    Deferred,
    Superseded
}

/// <summary>
/// Meeting recording information
/// </summary>
public class MeetingRecording
{
    public string RecordingId { get; set; } = string.Empty;
    public string MeetingId { get; set; } = string.Empty;
    public string RecordingUrl { get; set; } = string.Empty;
    public string? TranscriptUrl { get; set; }
    public long FileSizeBytes { get; set; }
    public int DurationSeconds { get; set; }
    public DateTime RecordedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// Webhook payload for meeting events
/// </summary>
public class MeetingWebhookPayload
{
    public string MeetingId { get; set; } = string.Empty;
    public string Event { get; set; } = string.Empty;
    public MeetingStatus Status { get; set; }
    public MeetingParticipant? Participant { get; set; }
    public DateTime Timestamp { get; set; }
    public string Signature { get; set; } = string.Empty;
    public Dictionary<string, object>? AdditionalData { get; set; }
}
