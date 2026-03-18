using AFC27.KMS.Workflow.Domain.Entities;

namespace AFC27.KMS.Workflow.Application.DTOs;

/// <summary>
/// Full workflow instance DTO with step details.
/// </summary>
public record WorkflowInstanceDto
{
    public Guid Id { get; init; }
    public Guid WorkflowDefinitionId { get; init; }
    public string WorkflowDefinitionName { get; init; } = string.Empty;
    public string TargetEntityType { get; init; } = string.Empty;
    public Guid TargetEntityId { get; init; }
    public Guid? CurrentStepId { get; init; }
    public string? CurrentStepName { get; init; }
    public WorkflowInstanceStatus Status { get; init; }
    public DateTime StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public Guid InitiatedById { get; init; }
    public string InitiatedByName { get; init; } = string.Empty;
    public List<WorkflowStepInstanceDto> StepInstances { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
}

/// <summary>
/// Summary DTO for listing workflow instances.
/// </summary>
public record WorkflowInstanceSummaryDto
{
    public Guid Id { get; init; }
    public string WorkflowDefinitionName { get; init; } = string.Empty;
    public string TargetEntityType { get; init; } = string.Empty;
    public Guid TargetEntityId { get; init; }
    public string? CurrentStepName { get; init; }
    public WorkflowInstanceStatus Status { get; init; }
    public DateTime StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public string InitiatedByName { get; init; } = string.Empty;
}

/// <summary>
/// Step instance DTO.
/// </summary>
public record WorkflowStepInstanceDto
{
    public Guid Id { get; init; }
    public Guid WorkflowInstanceId { get; init; }
    public Guid? StepDefinitionId { get; init; }
    public int StepOrder { get; init; }
    public string StepName { get; init; } = string.Empty;
    public Guid? AssigneeId { get; init; }
    public string? AssigneeName { get; init; }
    public WorkflowStepStatus Status { get; init; }
    public DateTime? StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public string? Outcome { get; init; }
    public string? Comments { get; init; }
    public DateTime? DueDate { get; init; }
    public DateTime? EscalatedAt { get; init; }
}

/// <summary>
/// Request to start a new workflow instance.
/// </summary>
public record StartWorkflowRequest
{
    public Guid WorkflowDefinitionId { get; init; }
    public string TargetEntityType { get; init; } = string.Empty;
    public Guid TargetEntityId { get; init; }
}

/// <summary>
/// Request to advance / complete the current step of a workflow.
/// </summary>
public record AdvanceStepRequest
{
    public string Outcome { get; init; } = string.Empty;
    public string? Comments { get; init; }
}

/// <summary>
/// Request to reject the current step of a workflow.
/// </summary>
public record RejectStepRequest
{
    public string Reason { get; init; } = string.Empty;
}

/// <summary>
/// Filter for querying workflow instances.
/// </summary>
public record WorkflowInstanceFilterDto
{
    public string? TargetEntityType { get; init; }
    public Guid? TargetEntityId { get; init; }
    public Guid? WorkflowDefinitionId { get; init; }
    public WorkflowInstanceStatus? Status { get; init; }
    public Guid? InitiatedById { get; init; }
    public DateTime? StartedFrom { get; init; }
    public DateTime? StartedTo { get; init; }
    public string SortBy { get; init; } = "startedAt";
    public bool SortDescending { get; init; } = true;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
