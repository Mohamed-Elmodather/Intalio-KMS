using AFC27.KMS.Workflow.Domain.Entities;
using TaskStatus = AFC27.KMS.Workflow.Domain.Entities.TaskStatus;

namespace AFC27.KMS.Workflow.Application.DTOs;

/// <summary>
/// Workflow definition DTO
/// </summary>
public record WorkflowDefinitionDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Version { get; init; } = string.Empty;
    public WorkflowStatus Status { get; init; }
    public WorkflowType Type { get; init; }
    public bool IsDefault { get; init; }
    public int ActiveInstanceCount { get; init; }
    public List<WorkflowStepDto> Steps { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
}

/// <summary>
/// Workflow step DTO
/// </summary>
public record WorkflowStepDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Order { get; init; }
    public StepType Type { get; init; }
    public StepAction Action { get; init; }
    public AssignmentType AssignmentType { get; init; }
    public Guid? AssignedRoleId { get; init; }
    public string? AssignedRoleName { get; init; }
    public Guid? AssignedUserId { get; init; }
    public string? AssignedUserName { get; init; }
    public Guid? AssignedGroupId { get; init; }
    public string? AssignedGroupName { get; init; }
    public int? TimeoutHours { get; init; }
    public bool AllowDelegation { get; init; }
    public bool RequireComment { get; init; }
    public List<StepOutcomeDto> Outcomes { get; init; } = new();
}

/// <summary>
/// Step outcome DTO
/// </summary>
public record StepOutcomeDto
{
    public string Name { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string? NextStepName { get; init; }
    public bool IsTerminal { get; init; }
}

/// <summary>
/// Create workflow request
/// </summary>
public record CreateWorkflowRequest
{
    public string NameEn { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string? DescriptionEn { get; init; }
    public string? DescriptionAr { get; init; }
    public WorkflowType Type { get; init; } = WorkflowType.Sequential;
}

/// <summary>
/// Create workflow step request
/// </summary>
public record CreateWorkflowStepRequest
{
    public string NameEn { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string? DescriptionEn { get; init; }
    public string? DescriptionAr { get; init; }
    public StepType Type { get; init; }
    public StepAction Action { get; init; }
    public AssignmentType AssignmentType { get; init; } = AssignmentType.Manual;
    public Guid? AssignedRoleId { get; init; }
    public Guid? AssignedUserId { get; init; }
    public Guid? AssignedGroupId { get; init; }
    public int? TimeoutHours { get; init; }
    public bool AllowDelegation { get; init; } = true;
    public bool RequireComment { get; init; }
    public List<StepOutcomeDto> Outcomes { get; init; } = new();
}

/// <summary>
/// Task DTO
/// </summary>
public record WorkflowTaskDto
{
    public Guid Id { get; init; }
    public string TaskNumber { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Instructions { get; init; }

    public Guid ServiceRequestId { get; init; }
    public string RequestNumber { get; init; } = string.Empty;
    public string RequestTitle { get; init; } = string.Empty;
    public string ServiceName { get; init; } = string.Empty;

    public string StepName { get; init; } = string.Empty;
    public int StepOrder { get; init; }

    public TaskType Type { get; init; }
    public TaskStatus Status { get; init; }
    public TaskPriority Priority { get; init; }

    public Guid? AssignedToId { get; init; }
    public string? AssignedToName { get; init; }
    public string? AssignedGroupName { get; init; }
    public DateTime? AssignedAt { get; init; }

    public DateTime? DueDate { get; init; }
    public bool IsOverdue { get; init; }
    public bool IsRead { get; init; }

    public string? Outcome { get; init; }
    public string? Comments { get; init; }
    public DateTime? CompletedAt { get; init; }
    public string? CompletedByName { get; init; }

    public bool IsDelegated { get; init; }
    public string? DelegatedFromName { get; init; }

    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Task summary for inbox
/// </summary>
public record TaskSummaryDto
{
    public Guid Id { get; init; }
    public string TaskNumber { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string RequestNumber { get; init; } = string.Empty;
    public string ServiceName { get; init; } = string.Empty;
    public TaskType Type { get; init; }
    public TaskStatus Status { get; init; }
    public TaskPriority Priority { get; init; }
    public DateTime? DueDate { get; init; }
    public bool IsOverdue { get; init; }
    public bool IsRead { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Complete task request
/// </summary>
public record CompleteTaskRequest
{
    public string Outcome { get; init; } = string.Empty;
    public string? Comments { get; init; }
    public string? FormDataJson { get; init; }
}

/// <summary>
/// Delegate task request
/// </summary>
public record DelegateTaskRequest
{
    public Guid ToUserId { get; init; }
    public string? Reason { get; init; }
}

/// <summary>
/// Task filter
/// </summary>
public record TaskFilterDto
{
    public TaskStatus? Status { get; init; }
    public TaskType? Type { get; init; }
    public TaskPriority? Priority { get; init; }
    public bool? IsOverdue { get; init; }
    public bool? IsRead { get; init; }
    public Guid? ServiceId { get; init; }
    public DateTime? DueFrom { get; init; }
    public DateTime? DueTo { get; init; }
    public string SortBy { get; init; } = "dueDate";
    public bool SortDescending { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

/// <summary>
/// Task inbox stats
/// </summary>
public record TaskInboxStatsDto
{
    public int TotalPending { get; init; }
    public int Overdue { get; init; }
    public int DueToday { get; init; }
    public int DueThisWeek { get; init; }
    public int Unread { get; init; }
    public Dictionary<TaskType, int> ByType { get; init; } = new();
    public Dictionary<TaskPriority, int> ByPriority { get; init; } = new();
}
