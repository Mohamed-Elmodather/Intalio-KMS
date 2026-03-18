namespace AFC27.KMS.Admin.Application.DTOs;

/// <summary>
/// Automation rule response DTO (Phase 8E).
/// </summary>
public record AutomationRuleDto
{
    public Guid Id { get; init; }
    public Guid OwnerId { get; init; }
    public string OwnerName { get; init; } = string.Empty;
    public Guid? SpaceId { get; init; }
    public string? SpaceName { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string TriggerEvent { get; init; } = string.Empty;
    public string Conditions { get; init; } = "[]";
    public string Actions { get; init; } = "[]";
    public bool IsActive { get; init; }
    public int ExecutionCount { get; init; }
    public DateTime? LastTriggeredAt { get; init; }
    public string? LastError { get; init; }
    public int Priority { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
}

/// <summary>
/// Automation rule summary for lists.
/// </summary>
public record AutomationRuleSummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string TriggerEvent { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public int ExecutionCount { get; init; }
    public DateTime? LastTriggeredAt { get; init; }
    public string OwnerName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Create automation rule request.
/// </summary>
public record CreateAutomationRuleRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string TriggerEvent { get; init; } = string.Empty;
    public string Conditions { get; init; } = "[]";
    public string Actions { get; init; } = "[]";
    public Guid? SpaceId { get; init; }
    public int Priority { get; init; } = 100;
}

/// <summary>
/// Update automation rule request.
/// </summary>
public record UpdateAutomationRuleRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string TriggerEvent { get; init; } = string.Empty;
    public string Conditions { get; init; } = "[]";
    public string Actions { get; init; } = "[]";
    public Guid? SpaceId { get; init; }
    public int Priority { get; init; } = 100;
}

/// <summary>
/// Automation rule execution log entry.
/// </summary>
public record AutomationRuleExecutionDto
{
    public Guid Id { get; init; }
    public Guid RuleId { get; init; }
    public string RuleName { get; init; } = string.Empty;
    public string TriggerEvent { get; init; } = string.Empty;
    public Guid? TriggerEntityId { get; init; }
    public string? TriggerEntityType { get; init; }
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public int DurationMs { get; init; }
    public DateTime ExecutedAt { get; init; }
}

/// <summary>
/// Available trigger events metadata for the UI.
/// </summary>
public record TriggerEventMetadataDto
{
    public string EventName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Category { get; init; } = string.Empty;
    public IReadOnlyList<string> AvailableProperties { get; init; } = Array.Empty<string>();
}
