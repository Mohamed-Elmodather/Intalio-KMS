using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Workflow.Application.DTOs;
using AFC27.KMS.Workflow.Domain.Entities;

namespace AFC27.KMS.Workflow.Application.Services;

/// <summary>
/// Generalized workflow engine that can orchestrate any entity through a
/// workflow definition's steps.
/// </summary>
public class WorkflowEngineService : IWorkflowEngineService
{
    private readonly DbContext _dbContext;
    private readonly ILogger<WorkflowEngineService> _logger;

    public WorkflowEngineService(
        DbContext dbContext,
        ILogger<WorkflowEngineService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    // ───────────────────────────── Start ─────────────────────────────

    public async Task<WorkflowInstanceDto> StartWorkflowAsync(
        Guid definitionId,
        string targetEntityType,
        Guid targetEntityId,
        Guid initiatorId,
        CancellationToken cancellationToken = default)
    {
        var definition = await _dbContext.Set<WorkflowDefinition>()
            .Include(d => d.Steps)
            .FirstOrDefaultAsync(d => d.Id == definitionId, cancellationToken)
            ?? throw new InvalidOperationException($"Workflow definition {definitionId} not found.");

        if (definition.Status != WorkflowStatus.Published)
            throw new InvalidOperationException("Only published workflow definitions can be started.");

        if (definition.Steps.Count == 0)
            throw new InvalidOperationException("Workflow definition has no steps.");

        // Resolve initiator display name (best-effort; falls back to Id string)
        var initiatorName = await ResolveUserNameAsync(initiatorId, cancellationToken)
                            ?? initiatorId.ToString();

        var instance = WorkflowInstance.Create(
            definitionId,
            targetEntityType,
            targetEntityId,
            initiatorId,
            initiatorName);

        _dbContext.Set<WorkflowInstance>().Add(instance);

        // Create step instances for every step in the definition
        var orderedSteps = definition.Steps.OrderBy(s => s.Order).ToList();
        foreach (var stepDef in orderedSteps)
        {
            var assigneeId = stepDef.AssignedUserId;
            var assigneeName = assigneeId.HasValue
                ? await ResolveUserNameAsync(assigneeId.Value, cancellationToken)
                : null;

            DateTime? dueDate = stepDef.TimeoutHours.HasValue
                ? DateTime.UtcNow.AddHours(stepDef.TimeoutHours.Value)
                : null;

            var stepInstance = WorkflowStepInstance.Create(
                instance.Id,
                stepDef.Id,
                stepDef.Order,
                stepDef.Name.English,
                assigneeId,
                assigneeName,
                dueDate);

            instance.AddStepInstance(stepInstance);
            _dbContext.Set<WorkflowStepInstance>().Add(stepInstance);
        }

        // Activate the first step
        var firstStep = instance.StepInstances
            .OrderBy(s => s.StepOrder)
            .First();
        firstStep.Start();
        instance.SetCurrentStep(firstStep.Id);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Workflow instance {InstanceId} started for {EntityType}/{EntityId} using definition {DefinitionId}",
            instance.Id, targetEntityType, targetEntityId, definitionId);

        return await GetWorkflowInstanceAsync(instance.Id, cancellationToken)
               ?? throw new InvalidOperationException("Failed to retrieve newly created instance.");
    }

    // ───────────────────────────── Advance ─────────────────────────────

    public async Task<WorkflowInstanceDto> AdvanceStepAsync(
        Guid instanceId,
        string outcome,
        string? comments,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var instance = await LoadInstanceWithStepsAsync(instanceId, cancellationToken);

        EnsureActive(instance);

        var currentStep = GetCurrentStep(instance);
        currentStep.Complete(outcome, comments);

        // Move to next step or complete
        AdvanceToNextStepOrComplete(instance, currentStep);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Workflow instance {InstanceId} step {StepName} completed with outcome '{Outcome}'",
            instanceId, currentStep.StepName, outcome);

        return await GetWorkflowInstanceAsync(instanceId, cancellationToken)
               ?? throw new InvalidOperationException("Instance not found after advance.");
    }

    // ───────────────────────────── Reject ─────────────────────────────

    public async Task<WorkflowInstanceDto> RejectStepAsync(
        Guid instanceId,
        string reason,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var instance = await LoadInstanceWithStepsAsync(instanceId, cancellationToken);

        EnsureActive(instance);

        var currentStep = GetCurrentStep(instance);
        currentStep.Reject(reason);

        // On rejection we cancel the entire workflow (sequential model).
        // Future enhancement: configurable rejection behaviour per step definition.
        CancelRemainingSteps(instance);
        instance.Cancel();

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Workflow instance {InstanceId} rejected at step {StepName}: {Reason}",
            instanceId, currentStep.StepName, reason);

        return await GetWorkflowInstanceAsync(instanceId, cancellationToken)
               ?? throw new InvalidOperationException("Instance not found after reject.");
    }

    // ───────────────────────────── Skip ─────────────────────────────

    public async Task<WorkflowInstanceDto> SkipStepAsync(
        Guid instanceId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var instance = await LoadInstanceWithStepsAsync(instanceId, cancellationToken);

        EnsureActive(instance);

        var currentStep = GetCurrentStep(instance);
        currentStep.Skip();

        AdvanceToNextStepOrComplete(instance, currentStep);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Workflow instance {InstanceId} step {StepName} skipped",
            instanceId, currentStep.StepName);

        return await GetWorkflowInstanceAsync(instanceId, cancellationToken)
               ?? throw new InvalidOperationException("Instance not found after skip.");
    }

    // ───────────────────────────── Cancel ─────────────────────────────

    public async Task<WorkflowInstanceDto> CancelWorkflowAsync(
        Guid instanceId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var instance = await LoadInstanceWithStepsAsync(instanceId, cancellationToken);

        EnsureActive(instance);

        CancelRemainingSteps(instance);
        instance.Cancel();

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Workflow instance {InstanceId} cancelled by user {UserId}", instanceId, userId);

        return await GetWorkflowInstanceAsync(instanceId, cancellationToken)
               ?? throw new InvalidOperationException("Instance not found after cancel.");
    }

    // ───────────────────────────── Query ─────────────────────────────

    public async Task<WorkflowInstanceDto?> GetWorkflowInstanceAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var instance = await _dbContext.Set<WorkflowInstance>()
            .AsNoTracking()
            .Include(i => i.WorkflowDefinition)
            .Include(i => i.StepInstances)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        if (instance is null)
            return null;

        return MapToDto(instance);
    }

    public async Task<List<WorkflowInstanceSummaryDto>> GetWorkflowsForEntityAsync(
        string entityType,
        Guid entityId,
        CancellationToken cancellationToken = default)
    {
        var instances = await _dbContext.Set<WorkflowInstance>()
            .AsNoTracking()
            .Include(i => i.WorkflowDefinition)
            .Include(i => i.StepInstances)
            .Where(i => i.TargetEntityType == entityType && i.TargetEntityId == entityId)
            .OrderByDescending(i => i.StartedAt)
            .ToListAsync(cancellationToken);

        return instances.Select(MapToSummaryDto).ToList();
    }

    // ───────────────────────────── Helpers ─────────────────────────────

    private async Task<WorkflowInstance> LoadInstanceWithStepsAsync(
        Guid instanceId, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<WorkflowInstance>()
            .Include(i => i.StepInstances)
            .Include(i => i.WorkflowDefinition)
            .FirstOrDefaultAsync(i => i.Id == instanceId, cancellationToken)
            ?? throw new InvalidOperationException($"Workflow instance {instanceId} not found.");
    }

    private static void EnsureActive(WorkflowInstance instance)
    {
        if (instance.Status != WorkflowInstanceStatus.Active)
            throw new InvalidOperationException(
                $"Workflow instance {instance.Id} is not active (status: {instance.Status}).");
    }

    private static WorkflowStepInstance GetCurrentStep(WorkflowInstance instance)
    {
        if (instance.CurrentStepId is null)
            throw new InvalidOperationException("Workflow instance has no current step.");

        return instance.StepInstances.FirstOrDefault(s => s.Id == instance.CurrentStepId)
               ?? throw new InvalidOperationException(
                   $"Current step {instance.CurrentStepId} not found in instance step collection.");
    }

    /// <summary>
    /// Find the next pending step and activate it, or complete the workflow if none remain.
    /// </summary>
    private static void AdvanceToNextStepOrComplete(
        WorkflowInstance instance,
        WorkflowStepInstance completedStep)
    {
        var nextStep = instance.StepInstances
            .Where(s => s.StepOrder > completedStep.StepOrder
                        && s.Status == WorkflowStepStatus.Pending)
            .OrderBy(s => s.StepOrder)
            .FirstOrDefault();

        if (nextStep is not null)
        {
            nextStep.Start();
            instance.SetCurrentStep(nextStep.Id);
        }
        else
        {
            // No more steps -- workflow is done.
            instance.Complete();
        }
    }

    private static void CancelRemainingSteps(WorkflowInstance instance)
    {
        foreach (var step in instance.StepInstances)
        {
            if (step.Status == WorkflowStepStatus.Pending
                || step.Status == WorkflowStepStatus.InProgress)
            {
                step.Skip();
            }
        }
    }

    /// <summary>
    /// Best-effort user name resolution. Returns null when the Identity module
    /// tables are not available or the user is not found.
    /// </summary>
    private async Task<string?> ResolveUserNameAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            // Try to resolve from the Users table if it exists in this DbContext.
            // This avoids a hard dependency on the Identity module.
            var userEntityType = _dbContext.Model.FindEntityType("AFC27.KMS.Identity.Domain.Entities.User");
            if (userEntityType is null)
                return null;

            var user = await _dbContext.FindAsync(userEntityType.ClrType, new object[] { userId }, cancellationToken);
            if (user is null)
                return null;

            var displayNameProp = userEntityType.ClrType.GetProperty("DisplayName");
            return displayNameProp?.GetValue(user) as string;
        }
        catch
        {
            return null;
        }
    }

    // ───────────────────────────── Mapping ─────────────────────────────

    private static WorkflowInstanceDto MapToDto(WorkflowInstance instance)
    {
        var currentStepName = instance.CurrentStepId.HasValue
            ? instance.StepInstances.FirstOrDefault(s => s.Id == instance.CurrentStepId)?.StepName
            : null;

        return new WorkflowInstanceDto
        {
            Id = instance.Id,
            WorkflowDefinitionId = instance.WorkflowDefinitionId,
            WorkflowDefinitionName = instance.WorkflowDefinition?.Name?.English ?? string.Empty,
            TargetEntityType = instance.TargetEntityType,
            TargetEntityId = instance.TargetEntityId,
            CurrentStepId = instance.CurrentStepId,
            CurrentStepName = currentStepName,
            Status = instance.Status,
            StartedAt = instance.StartedAt,
            CompletedAt = instance.CompletedAt,
            InitiatedById = instance.InitiatedById,
            InitiatedByName = instance.InitiatedByName,
            StepInstances = instance.StepInstances
                .OrderBy(s => s.StepOrder)
                .Select(MapStepToDto)
                .ToList(),
            CreatedAt = instance.CreatedAt,
            ModifiedAt = instance.ModifiedAt
        };
    }

    private static WorkflowInstanceSummaryDto MapToSummaryDto(WorkflowInstance instance)
    {
        var currentStepName = instance.CurrentStepId.HasValue
            ? instance.StepInstances.FirstOrDefault(s => s.Id == instance.CurrentStepId)?.StepName
            : null;

        return new WorkflowInstanceSummaryDto
        {
            Id = instance.Id,
            WorkflowDefinitionName = instance.WorkflowDefinition?.Name?.English ?? string.Empty,
            TargetEntityType = instance.TargetEntityType,
            TargetEntityId = instance.TargetEntityId,
            CurrentStepName = currentStepName,
            Status = instance.Status,
            StartedAt = instance.StartedAt,
            CompletedAt = instance.CompletedAt,
            InitiatedByName = instance.InitiatedByName
        };
    }

    private static WorkflowStepInstanceDto MapStepToDto(WorkflowStepInstance step)
    {
        return new WorkflowStepInstanceDto
        {
            Id = step.Id,
            WorkflowInstanceId = step.WorkflowInstanceId,
            StepDefinitionId = step.StepDefinitionId,
            StepOrder = step.StepOrder,
            StepName = step.StepName,
            AssigneeId = step.AssigneeId,
            AssigneeName = step.AssigneeName,
            Status = step.Status,
            StartedAt = step.StartedAt,
            CompletedAt = step.CompletedAt,
            Outcome = step.Outcome,
            Comments = step.Comments,
            DueDate = step.DueDate,
            EscalatedAt = step.EscalatedAt
        };
    }
}
