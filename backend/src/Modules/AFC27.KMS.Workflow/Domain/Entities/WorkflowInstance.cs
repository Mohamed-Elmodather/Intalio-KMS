using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Workflow.Domain.Entities;

/// <summary>
/// A running instance of a workflow definition, tracking progress for a target entity.
/// </summary>
public class WorkflowInstance : AuditableEntity
{
    public Guid WorkflowDefinitionId { get; private set; }
    public WorkflowDefinition WorkflowDefinition { get; private set; } = null!;

    /// <summary>
    /// The type of entity this workflow is running against (Article, Document, ServiceRequest, LessonLearned, etc.)
    /// </summary>
    public string TargetEntityType { get; private set; } = string.Empty;

    /// <summary>
    /// The ID of the entity this workflow is running against.
    /// </summary>
    public Guid TargetEntityId { get; private set; }

    /// <summary>
    /// The step instance that is currently active. Null when completed or cancelled.
    /// </summary>
    public Guid? CurrentStepId { get; private set; }

    public WorkflowInstanceStatus Status { get; private set; } = WorkflowInstanceStatus.Active;

    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public Guid InitiatedById { get; private set; }
    public string InitiatedByName { get; private set; } = string.Empty;

    /// <summary>
    /// All step instances belonging to this workflow run.
    /// </summary>
    public ICollection<WorkflowStepInstance> StepInstances { get; private set; } = new List<WorkflowStepInstance>();

    private WorkflowInstance() { }

    public static WorkflowInstance Create(
        Guid workflowDefinitionId,
        string targetEntityType,
        Guid targetEntityId,
        Guid initiatedById,
        string initiatedByName)
    {
        var instance = new WorkflowInstance
        {
            Id = Guid.NewGuid(),
            WorkflowDefinitionId = workflowDefinitionId,
            TargetEntityType = targetEntityType,
            TargetEntityId = targetEntityId,
            Status = WorkflowInstanceStatus.Active,
            StartedAt = DateTime.UtcNow,
            InitiatedById = initiatedById,
            InitiatedByName = initiatedByName
        };

        instance.AddDomainEvent(new WorkflowInstanceStartedEvent(
            instance.Id, workflowDefinitionId, targetEntityType, targetEntityId));

        return instance;
    }

    /// <summary>
    /// Set the currently active step.
    /// </summary>
    public void SetCurrentStep(Guid? stepInstanceId)
    {
        CurrentStepId = stepInstanceId;
    }

    /// <summary>
    /// Mark this workflow instance as completed.
    /// </summary>
    public void Complete()
    {
        Status = WorkflowInstanceStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        CurrentStepId = null;

        AddDomainEvent(new WorkflowInstanceCompletedEvent(Id, TargetEntityType, TargetEntityId));
    }

    /// <summary>
    /// Cancel this workflow instance.
    /// </summary>
    public void Cancel()
    {
        Status = WorkflowInstanceStatus.Cancelled;
        CompletedAt = DateTime.UtcNow;
        CurrentStepId = null;

        AddDomainEvent(new WorkflowInstanceCancelledEvent(Id, TargetEntityType, TargetEntityId));
    }

    /// <summary>
    /// Mark this workflow instance as timed out.
    /// </summary>
    public void TimeOut()
    {
        Status = WorkflowInstanceStatus.TimedOut;
        CompletedAt = DateTime.UtcNow;
        CurrentStepId = null;
    }

    /// <summary>
    /// Add a step instance to this workflow run.
    /// </summary>
    public void AddStepInstance(WorkflowStepInstance stepInstance)
    {
        StepInstances.Add(stepInstance);
    }
}

public enum WorkflowInstanceStatus
{
    Active,
    Completed,
    Cancelled,
    TimedOut
}

// Domain Events
public record WorkflowInstanceStartedEvent(
    Guid InstanceId,
    Guid DefinitionId,
    string TargetEntityType,
    Guid TargetEntityId) : DomainEvent;

public record WorkflowInstanceCompletedEvent(
    Guid InstanceId,
    string TargetEntityType,
    Guid TargetEntityId) : DomainEvent;

public record WorkflowInstanceCancelledEvent(
    Guid InstanceId,
    string TargetEntityType,
    Guid TargetEntityId) : DomainEvent;
