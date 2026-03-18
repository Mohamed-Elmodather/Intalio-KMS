using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Workflow.Domain.Entities;

/// <summary>
/// A single step execution within a workflow instance.
/// </summary>
public class WorkflowStepInstance : AuditableEntity
{
    public Guid WorkflowInstanceId { get; private set; }
    public WorkflowInstance WorkflowInstance { get; private set; } = null!;

    /// <summary>
    /// Reference back to the step definition this was created from. Nullable in case the definition is later removed.
    /// </summary>
    public Guid? StepDefinitionId { get; private set; }

    public int StepOrder { get; private set; }
    public string StepName { get; private set; } = string.Empty;

    // Assignment
    public Guid? AssigneeId { get; private set; }
    public string? AssigneeName { get; private set; }

    public WorkflowStepStatus Status { get; private set; } = WorkflowStepStatus.Pending;

    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    /// <summary>
    /// The outcome chosen when the step was completed (e.g. "Approved", "Rejected", "Completed").
    /// </summary>
    public string? Outcome { get; private set; }

    /// <summary>
    /// Free-text comments left by the assignee when acting on this step.
    /// </summary>
    public string? Comments { get; private set; }

    public DateTime? DueDate { get; private set; }
    public DateTime? EscalatedAt { get; private set; }

    private WorkflowStepInstance() { }

    public static WorkflowStepInstance Create(
        Guid workflowInstanceId,
        Guid? stepDefinitionId,
        int stepOrder,
        string stepName,
        Guid? assigneeId = null,
        string? assigneeName = null,
        DateTime? dueDate = null)
    {
        return new WorkflowStepInstance
        {
            Id = Guid.NewGuid(),
            WorkflowInstanceId = workflowInstanceId,
            StepDefinitionId = stepDefinitionId,
            StepOrder = stepOrder,
            StepName = stepName,
            AssigneeId = assigneeId,
            AssigneeName = assigneeName,
            Status = WorkflowStepStatus.Pending,
            DueDate = dueDate
        };
    }

    /// <summary>
    /// Transition this step to InProgress.
    /// </summary>
    public void Start()
    {
        if (Status != WorkflowStepStatus.Pending)
            throw new InvalidOperationException("Only pending steps can be started.");

        Status = WorkflowStepStatus.InProgress;
        StartedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Complete this step with an outcome.
    /// </summary>
    public void Complete(string outcome, string? comments = null)
    {
        Outcome = outcome;
        Comments = comments;
        Status = WorkflowStepStatus.Completed;
        CompletedAt = DateTime.UtcNow;

        if (StartedAt is null)
            StartedAt = DateTime.UtcNow;

        AddDomainEvent(new WorkflowStepCompletedEvent(Id, WorkflowInstanceId, outcome));
    }

    /// <summary>
    /// Reject this step.
    /// </summary>
    public void Reject(string reason)
    {
        Outcome = "Rejected";
        Comments = reason;
        Status = WorkflowStepStatus.Rejected;
        CompletedAt = DateTime.UtcNow;

        if (StartedAt is null)
            StartedAt = DateTime.UtcNow;

        AddDomainEvent(new WorkflowStepRejectedEvent(Id, WorkflowInstanceId, reason));
    }

    /// <summary>
    /// Skip this step.
    /// </summary>
    public void Skip()
    {
        Status = WorkflowStepStatus.Skipped;
        CompletedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Mark this step as escalated.
    /// </summary>
    public void Escalate()
    {
        Status = WorkflowStepStatus.Escalated;
        EscalatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Reassign this step to a different user.
    /// </summary>
    public void Reassign(Guid assigneeId, string assigneeName)
    {
        AssigneeId = assigneeId;
        AssigneeName = assigneeName;
    }

    /// <summary>
    /// Update the due date.
    /// </summary>
    public void SetDueDate(DateTime? dueDate)
    {
        DueDate = dueDate;
    }
}

public enum WorkflowStepStatus
{
    Pending,
    InProgress,
    Completed,
    Skipped,
    Escalated,
    Rejected
}

// Domain Events
public record WorkflowStepCompletedEvent(
    Guid StepInstanceId,
    Guid WorkflowInstanceId,
    string Outcome) : DomainEvent;

public record WorkflowStepRejectedEvent(
    Guid StepInstanceId,
    Guid WorkflowInstanceId,
    string Reason) : DomainEvent;
