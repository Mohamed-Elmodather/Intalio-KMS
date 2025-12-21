using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Workflow.Domain.Entities;

/// <summary>
/// A request submitted for a service
/// </summary>
public class ServiceRequest : AuditableEntity
{
    public string RequestNumber { get; private set; } = string.Empty;
    public LocalizedString Title { get; private set; } = new();
    public LocalizedString? Description { get; private set; }

    public Guid ServiceId { get; private set; }
    public Service Service { get; private set; } = null!;

    public Guid RequesterId { get; private set; }
    public string RequesterName { get; private set; } = string.Empty;
    public string? RequesterEmail { get; private set; }
    public Guid? RequesterDepartmentId { get; private set; }

    public RequestStatus Status { get; private set; } = RequestStatus.Draft;
    public ServicePriority Priority { get; private set; } = ServicePriority.Normal;

    // Assignment
    public Guid? AssignedToId { get; private set; }
    public string? AssignedToName { get; private set; }
    public Guid? AssignedGroupId { get; private set; }
    public DateTime? AssignedAt { get; private set; }

    // Form data
    public Guid? FormSubmissionId { get; private set; }
    public FormSubmission? FormSubmission { get; private set; }

    // Workflow tracking
    public Guid? WorkflowInstanceId { get; private set; }
    public string? CurrentStepName { get; private set; }
    public int CurrentStepOrder { get; private set; }

    // SLA tracking
    public DateTime? SlaResponseDue { get; private set; }
    public DateTime? SlaResolutionDue { get; private set; }
    public DateTime? FirstResponseAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public bool IsSlaResponseBreached { get; private set; }
    public bool IsSlaResolutionBreached { get; private set; }

    // Resolution
    public string? ResolutionNotes { get; private set; }
    public ResolutionType? Resolution { get; private set; }

    // Related items
    public List<RequestComment> Comments { get; private set; } = new();
    public List<RequestAttachment> Attachments { get; private set; } = new();
    public List<RequestActivity> Activities { get; private set; } = new();
    public List<WorkflowTask> Tasks { get; private set; } = new();

    // Metadata
    public Dictionary<string, string> Metadata { get; private set; } = new();

    private ServiceRequest() { }

    public static ServiceRequest Create(
        Guid serviceId,
        Guid requesterId,
        string requesterName,
        LocalizedString title,
        ServicePriority priority = ServicePriority.Normal)
    {
        var request = new ServiceRequest
        {
            Id = Guid.NewGuid(),
            RequestNumber = GenerateRequestNumber(),
            ServiceId = serviceId,
            RequesterId = requesterId,
            RequesterName = requesterName,
            Title = title,
            Priority = priority,
            Status = RequestStatus.Draft
        };

        request.AddDomainEvent(new ServiceRequestCreatedEvent(request.Id, serviceId, requesterId));
        return request;
    }

    public void Submit()
    {
        if (Status != RequestStatus.Draft)
            throw new InvalidOperationException("Only draft requests can be submitted");

        Status = RequestStatus.Submitted;
        AddDomainEvent(new ServiceRequestSubmittedEvent(Id, ServiceId));
    }

    public void StartProcessing()
    {
        Status = RequestStatus.InProgress;
        if (FirstResponseAt == null)
            FirstResponseAt = DateTime.UtcNow;
    }

    public void AssignTo(Guid userId, string userName, Guid? groupId = null)
    {
        AssignedToId = userId;
        AssignedToName = userName;
        AssignedGroupId = groupId;
        AssignedAt = DateTime.UtcNow;

        if (Status == RequestStatus.Submitted)
            StartProcessing();

        AddActivity(RequestActivityType.Assigned, $"Assigned to {userName}");
        AddDomainEvent(new ServiceRequestAssignedEvent(Id, userId));
    }

    public void UpdateStatus(RequestStatus status, string? notes = null)
    {
        var oldStatus = Status;
        Status = status;

        AddActivity(RequestActivityType.StatusChanged, $"Status changed from {oldStatus} to {status}", notes);

        if (status == RequestStatus.Resolved)
        {
            ResolvedAt = DateTime.UtcNow;
        }
        else if (status == RequestStatus.Closed)
        {
            ClosedAt = DateTime.UtcNow;
        }
    }

    public void Resolve(ResolutionType resolution, string? notes = null)
    {
        Resolution = resolution;
        ResolutionNotes = notes;
        ResolvedAt = DateTime.UtcNow;
        Status = RequestStatus.Resolved;

        AddActivity(RequestActivityType.Resolved, $"Resolved: {resolution}", notes);
        AddDomainEvent(new ServiceRequestResolvedEvent(Id, resolution));
    }

    public void Close()
    {
        ClosedAt = DateTime.UtcNow;
        Status = RequestStatus.Closed;
        AddActivity(RequestActivityType.Closed, "Request closed");
    }

    public void Reopen(string reason)
    {
        Status = RequestStatus.InProgress;
        ResolvedAt = null;
        ClosedAt = null;
        Resolution = null;

        AddActivity(RequestActivityType.Reopened, "Request reopened", reason);
    }

    public void Cancel(string reason)
    {
        Status = RequestStatus.Cancelled;
        AddActivity(RequestActivityType.Cancelled, "Request cancelled", reason);
    }

    public void SetSla(DateTime? responseDue, DateTime? resolutionDue)
    {
        SlaResponseDue = responseDue;
        SlaResolutionDue = resolutionDue;
    }

    public void CheckSlaBreaches()
    {
        var now = DateTime.UtcNow;

        if (SlaResponseDue.HasValue && FirstResponseAt == null && now > SlaResponseDue.Value)
        {
            IsSlaResponseBreached = true;
        }

        if (SlaResolutionDue.HasValue && ResolvedAt == null && now > SlaResolutionDue.Value)
        {
            IsSlaResolutionBreached = true;
        }
    }

    public void SetFormSubmission(Guid submissionId)
    {
        FormSubmissionId = submissionId;
    }

    public void UpdateWorkflowStep(string stepName, int stepOrder)
    {
        CurrentStepName = stepName;
        CurrentStepOrder = stepOrder;
    }

    public void AddComment(string content, Guid authorId, string authorName, bool isInternal = false)
    {
        var comment = RequestComment.Create(Id, content, authorId, authorName, isInternal);
        Comments.Add(comment);
        AddActivity(RequestActivityType.CommentAdded, isInternal ? "Internal comment added" : "Comment added");
    }

    public void AddAttachment(string fileName, string filePath, long fileSize, string mimeType, Guid uploadedById)
    {
        var attachment = RequestAttachment.Create(Id, fileName, filePath, fileSize, mimeType, uploadedById);
        Attachments.Add(attachment);
        AddActivity(RequestActivityType.AttachmentAdded, $"Attachment added: {fileName}");
    }

    private void AddActivity(RequestActivityType type, string description, string? details = null)
    {
        var activity = RequestActivity.Create(Id, type, description, details);
        Activities.Add(activity);
    }

    private static string GenerateRequestNumber()
    {
        return $"SR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }
}

public enum RequestStatus
{
    Draft,
    Submitted,
    Pending,
    InProgress,
    OnHold,
    PendingApproval,
    Resolved,
    Closed,
    Cancelled,
    Rejected
}

public enum ResolutionType
{
    Completed,
    Resolved,
    WorkAround,
    Duplicate,
    CannotReproduce,
    WontFix,
    OutOfScope
}

/// <summary>
/// Comment on a service request
/// </summary>
public class RequestComment : Entity
{
    public Guid RequestId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public Guid AuthorId { get; private set; }
    public string AuthorName { get; private set; } = string.Empty;
    public bool IsInternal { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? EditedAt { get; private set; }

    private RequestComment() { }

    public static RequestComment Create(
        Guid requestId,
        string content,
        Guid authorId,
        string authorName,
        bool isInternal = false)
    {
        return new RequestComment
        {
            Id = Guid.NewGuid(),
            RequestId = requestId,
            Content = content,
            AuthorId = authorId,
            AuthorName = authorName,
            IsInternal = isInternal,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Edit(string content)
    {
        Content = content;
        EditedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Attachment on a service request
/// </summary>
public class RequestAttachment : Entity
{
    public Guid RequestId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public string MimeType { get; private set; } = string.Empty;
    public Guid UploadedById { get; private set; }
    public DateTime UploadedAt { get; private set; }

    private RequestAttachment() { }

    public static RequestAttachment Create(
        Guid requestId,
        string fileName,
        string filePath,
        long fileSize,
        string mimeType,
        Guid uploadedById)
    {
        return new RequestAttachment
        {
            Id = Guid.NewGuid(),
            RequestId = requestId,
            FileName = fileName,
            FilePath = filePath,
            FileSize = fileSize,
            MimeType = mimeType,
            UploadedById = uploadedById,
            UploadedAt = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Activity log entry for a service request
/// </summary>
public class RequestActivity : Entity
{
    public Guid RequestId { get; private set; }
    public RequestActivityType Type { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string? Details { get; private set; }
    public DateTime OccurredAt { get; private set; }

    private RequestActivity() { }

    public static RequestActivity Create(
        Guid requestId,
        RequestActivityType type,
        string description,
        string? details = null)
    {
        return new RequestActivity
        {
            Id = Guid.NewGuid(),
            RequestId = requestId,
            Type = type,
            Description = description,
            Details = details,
            OccurredAt = DateTime.UtcNow
        };
    }
}

public enum RequestActivityType
{
    Created,
    Submitted,
    StatusChanged,
    Assigned,
    CommentAdded,
    AttachmentAdded,
    Resolved,
    Closed,
    Reopened,
    Cancelled,
    Escalated,
    WorkflowStep
}

// Domain Events
public record ServiceRequestCreatedEvent(Guid RequestId, Guid ServiceId, Guid RequesterId) : DomainEvent;
public record ServiceRequestSubmittedEvent(Guid RequestId, Guid ServiceId) : DomainEvent;
public record ServiceRequestAssignedEvent(Guid RequestId, Guid AssigneeId) : DomainEvent;
public record ServiceRequestResolvedEvent(Guid RequestId, ResolutionType Resolution) : DomainEvent;
