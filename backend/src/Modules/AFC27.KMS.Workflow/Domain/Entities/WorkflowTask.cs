using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Workflow.Domain.Entities;

/// <summary>
/// An active task in a workflow instance
/// </summary>
public class WorkflowTask : AuditableEntity
{
    public string TaskNumber { get; private set; } = string.Empty;
    public LocalizedString Title { get; private set; } = new();
    public LocalizedString? Description { get; private set; }
    public LocalizedString? Instructions { get; private set; }

    // References
    public Guid ServiceRequestId { get; private set; }
    public ServiceRequest ServiceRequest { get; private set; } = null!;
    public Guid WorkflowStepId { get; private set; }
    public string StepName { get; private set; } = string.Empty;
    public int StepOrder { get; private set; }

    // Task info
    public TaskType Type { get; private set; }
    public TaskStatus Status { get; private set; } = TaskStatus.Pending;
    public TaskPriority Priority { get; private set; } = TaskPriority.Normal;

    // Assignment
    public Guid? AssignedToId { get; private set; }
    public string? AssignedToName { get; private set; }
    public Guid? AssignedGroupId { get; private set; }
    public string? AssignedGroupName { get; private set; }
    public DateTime? AssignedAt { get; private set; }

    // Due date
    public DateTime? DueDate { get; private set; }
    public bool IsOverdue => DueDate.HasValue && DateTime.UtcNow > DueDate.Value && Status == TaskStatus.Pending;

    // Completion
    public string? Outcome { get; private set; }
    public string? Comments { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public Guid? CompletedById { get; private set; }
    public string? CompletedByName { get; private set; }

    // Delegation
    public bool IsDelegated { get; private set; }
    public Guid? DelegatedFromId { get; private set; }
    public string? DelegatedFromName { get; private set; }
    public DateTime? DelegatedAt { get; private set; }
    public string? DelegationReason { get; private set; }

    // Form data
    public Guid? FormSubmissionId { get; private set; }
    public FormSubmission? FormSubmission { get; private set; }

    // Read tracking
    public DateTime? ReadAt { get; private set; }
    public bool IsRead => ReadAt.HasValue;

    // Reminders
    public DateTime? LastReminderSentAt { get; private set; }
    public int ReminderCount { get; private set; }

    private WorkflowTask() { }

    public static WorkflowTask Create(
        Guid serviceRequestId,
        Guid workflowStepId,
        string stepName,
        int stepOrder,
        LocalizedString title,
        TaskType type)
    {
        var task = new WorkflowTask
        {
            Id = Guid.NewGuid(),
            TaskNumber = GenerateTaskNumber(),
            ServiceRequestId = serviceRequestId,
            WorkflowStepId = workflowStepId,
            StepName = stepName,
            StepOrder = stepOrder,
            Title = title,
            Type = type,
            Status = TaskStatus.Pending
        };

        task.AddDomainEvent(new TaskCreatedEvent(task.Id, serviceRequestId));
        return task;
    }

    public void AssignTo(Guid userId, string userName, Guid? groupId = null, string? groupName = null)
    {
        AssignedToId = userId;
        AssignedToName = userName;
        AssignedGroupId = groupId;
        AssignedGroupName = groupName;
        AssignedAt = DateTime.UtcNow;

        AddDomainEvent(new TaskAssignedEvent(Id, userId, ServiceRequestId));
    }

    public void SetDueDate(DateTime dueDate)
    {
        DueDate = dueDate;
    }

    public void MarkAsRead()
    {
        if (!ReadAt.HasValue)
            ReadAt = DateTime.UtcNow;
    }

    public void Start()
    {
        if (Status != TaskStatus.Pending)
            throw new InvalidOperationException("Task must be pending to start");

        Status = TaskStatus.InProgress;
    }

    public void Complete(string outcome, string? comments, Guid userId, string userName)
    {
        Outcome = outcome;
        Comments = comments;
        CompletedAt = DateTime.UtcNow;
        CompletedById = userId;
        CompletedByName = userName;
        Status = TaskStatus.Completed;

        AddDomainEvent(new TaskCompletedEvent(Id, ServiceRequestId, outcome));
    }

    public void Approve(string? comments, Guid userId, string userName)
    {
        Complete("Approved", comments, userId, userName);
    }

    public void Reject(string? comments, Guid userId, string userName)
    {
        Complete("Rejected", comments, userId, userName);
        Status = TaskStatus.Rejected;
    }

    public void Cancel(string reason)
    {
        Status = TaskStatus.Cancelled;
        Comments = reason;
    }

    public void Delegate(Guid toUserId, string toUserName, Guid fromUserId, string fromUserName, string? reason = null)
    {
        DelegatedFromId = fromUserId;
        DelegatedFromName = fromUserName;
        DelegatedAt = DateTime.UtcNow;
        DelegationReason = reason;
        IsDelegated = true;

        AssignedToId = toUserId;
        AssignedToName = toUserName;

        AddDomainEvent(new TaskDelegatedEvent(Id, fromUserId, toUserId));
    }

    public void Escalate(Guid toUserId, string toUserName, string reason)
    {
        Priority = TaskPriority.High;
        AssignTo(toUserId, toUserName);
        Comments = $"Escalated: {reason}";
    }

    public void SendReminder()
    {
        LastReminderSentAt = DateTime.UtcNow;
        ReminderCount++;
    }

    public void SetPriority(TaskPriority priority)
    {
        Priority = priority;
    }

    public void SetFormSubmission(Guid submissionId)
    {
        FormSubmissionId = submissionId;
    }

    private static string GenerateTaskNumber()
    {
        return $"T-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
    }
}

public enum TaskType
{
    Approval,
    Review,
    Action,
    Information,
    Decision,
    Signature
}

public enum TaskStatus
{
    Pending,
    InProgress,
    Completed,
    Rejected,
    Cancelled,
    Expired
}

public enum TaskPriority
{
    Low,
    Normal,
    High,
    Urgent
}

// Domain Events
public record TaskCreatedEvent(Guid TaskId, Guid RequestId) : DomainEvent;
public record TaskAssignedEvent(Guid TaskId, Guid AssigneeId, Guid RequestId) : DomainEvent;
public record TaskCompletedEvent(Guid TaskId, Guid RequestId, string Outcome) : DomainEvent;
public record TaskDelegatedEvent(Guid TaskId, Guid FromUserId, Guid ToUserId) : DomainEvent;
