using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Workflow.Domain.Entities;

/// <summary>
/// Definition of a workflow template
/// </summary>
public class WorkflowDefinition : AuditableEntity
{
    public LocalizedString Name { get; private set; } = new();
    public LocalizedString? Description { get; private set; }
    public string Version { get; private set; } = "1.0";

    public WorkflowStatus Status { get; private set; } = WorkflowStatus.Draft;
    public WorkflowType Type { get; private set; } = WorkflowType.Sequential;
    public WorkflowScope Scope { get; private set; } = WorkflowScope.ServiceRequest;

    public List<WorkflowStepDefinition> Steps { get; private set; } = new();

    // Navigation to instances
    public ICollection<WorkflowInstance> Instances { get; private set; } = new List<WorkflowInstance>();

    // Metadata
    public bool IsDefault { get; private set; }
    public int ActiveInstanceCount { get; private set; }

    private WorkflowDefinition() { }

    public static WorkflowDefinition Create(
        LocalizedString name,
        WorkflowType type = WorkflowType.Sequential,
        WorkflowScope scope = WorkflowScope.ServiceRequest)
    {
        return new WorkflowDefinition
        {
            Id = Guid.NewGuid(),
            Name = name,
            Type = type,
            Scope = scope,
            Status = WorkflowStatus.Draft
        };
    }

    public void SetScope(WorkflowScope scope) => Scope = scope;

    public void Update(LocalizedString name, LocalizedString? description)
    {
        Name = name;
        Description = description;
    }

    public void AddStep(WorkflowStepDefinition step)
    {
        step.SetOrder(Steps.Count + 1);
        Steps.Add(step);
    }

    public void RemoveStep(Guid stepId)
    {
        var step = Steps.FirstOrDefault(s => s.Id == stepId);
        if (step != null)
        {
            Steps.Remove(step);
            ReorderSteps();
        }
    }

    public void ReorderSteps()
    {
        for (int i = 0; i < Steps.Count; i++)
        {
            Steps[i].SetOrder(i + 1);
        }
    }

    public void Publish()
    {
        if (Steps.Count == 0)
            throw new InvalidOperationException("Workflow must have at least one step");

        Status = WorkflowStatus.Published;
        Version = IncrementVersion();
    }

    public void Unpublish() => Status = WorkflowStatus.Draft;
    public void Archive() => Status = WorkflowStatus.Archived;
    public void SetAsDefault(bool isDefault) => IsDefault = isDefault;

    private string IncrementVersion()
    {
        var parts = Version.Split('.');
        if (parts.Length == 2 && int.TryParse(parts[1], out int minor))
        {
            return $"{parts[0]}.{minor + 1}";
        }
        return "1.0";
    }
}

/// <summary>
/// Definition of a single step in a workflow
/// </summary>
public class WorkflowStepDefinition : Entity
{
    public Guid WorkflowDefinitionId { get; private set; }
    public LocalizedString Name { get; private set; } = new();
    public LocalizedString? Description { get; private set; }

    public int Order { get; private set; }
    public StepType Type { get; private set; }
    public StepAction Action { get; private set; }

    // Assignment
    public AssignmentType AssignmentType { get; private set; } = AssignmentType.Manual;
    public Guid? AssignedRoleId { get; private set; }
    public Guid? AssignedUserId { get; private set; }
    public Guid? AssignedGroupId { get; private set; }

    // Configuration
    public int? TimeoutHours { get; private set; }
    public bool AllowDelegation { get; private set; } = true;
    public bool RequireComment { get; private set; }
    public bool AllowAttachments { get; private set; } = true;

    // Conditional logic (JSON)
    public string? ConditionJson { get; private set; }

    // Outcomes
    public List<StepOutcome> Outcomes { get; private set; } = new();

    // Form for this step (optional)
    public Guid? StepFormId { get; private set; }

    private WorkflowStepDefinition() { }

    public static WorkflowStepDefinition Create(
        Guid workflowId,
        LocalizedString name,
        StepType type,
        StepAction action)
    {
        return new WorkflowStepDefinition
        {
            Id = Guid.NewGuid(),
            WorkflowDefinitionId = workflowId,
            Name = name,
            Type = type,
            Action = action
        };
    }

    public void Update(LocalizedString name, LocalizedString? description)
    {
        Name = name;
        Description = description;
    }

    public void SetOrder(int order) => Order = order;

    public void SetAssignment(
        AssignmentType type,
        Guid? roleId = null,
        Guid? userId = null,
        Guid? groupId = null)
    {
        AssignmentType = type;
        AssignedRoleId = roleId;
        AssignedUserId = userId;
        AssignedGroupId = groupId;
    }

    public void SetTimeout(int? hours) => TimeoutHours = hours;
    public void SetRequireComment(bool require) => RequireComment = require;
    public void SetAllowDelegation(bool allow) => AllowDelegation = allow;
    public void SetCondition(string? conditionJson) => ConditionJson = conditionJson;
    public void SetStepForm(Guid? formId) => StepFormId = formId;

    public void AddOutcome(StepOutcome outcome) => Outcomes.Add(outcome);
    public void ClearOutcomes() => Outcomes.Clear();
}

/// <summary>
/// Possible outcome of a workflow step
/// </summary>
public class StepOutcome
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? NextStepName { get; set; }
    public bool IsTerminal { get; set; }
    public RequestStatus? ResultStatus { get; set; }
}

public enum WorkflowStatus
{
    Draft,
    Published,
    Archived
}

public enum WorkflowType
{
    Sequential,
    Parallel,
    Conditional,
    StateMachine
}

public enum StepType
{
    Start,
    Task,
    Approval,
    Review,
    Notification,
    Condition,
    Parallel,
    End
}

public enum StepAction
{
    Approve,
    Reject,
    Review,
    Complete,
    Forward,
    Escalate,
    Custom
}

public enum AssignmentType
{
    Manual,
    Role,
    User,
    Group,
    Requester,
    RequesterManager,
    PreviousAssignee,
    RoundRobin,
    LeastBusy
}

public enum WorkflowScope
{
    ServiceRequest,
    ContentApproval,
    DocumentReview,
    KnowledgeVerification,
    LessonLearnedReview,
    Custom
}
