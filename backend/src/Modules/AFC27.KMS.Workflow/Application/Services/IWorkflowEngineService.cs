using AFC27.KMS.Workflow.Application.DTOs;

namespace AFC27.KMS.Workflow.Application.Services;

/// <summary>
/// General-purpose workflow engine that can drive approval / review workflows
/// for any target entity (ServiceRequest, Article, Document, LessonLearned, etc.).
/// </summary>
public interface IWorkflowEngineService
{
    /// <summary>
    /// Start a new workflow instance from a published definition.
    /// </summary>
    Task<WorkflowInstanceDto> StartWorkflowAsync(
        Guid definitionId,
        string targetEntityType,
        Guid targetEntityId,
        Guid initiatorId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Advance (complete) the current step of the workflow with an outcome.
    /// Automatically moves to the next step or completes the workflow.
    /// </summary>
    Task<WorkflowInstanceDto> AdvanceStepAsync(
        Guid instanceId,
        string outcome,
        string? comments,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reject the current step. Depending on workflow configuration this may
    /// cancel the entire workflow or return to a previous step.
    /// </summary>
    Task<WorkflowInstanceDto> RejectStepAsync(
        Guid instanceId,
        string reason,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Skip the current step and move to the next one.
    /// </summary>
    Task<WorkflowInstanceDto> SkipStepAsync(
        Guid instanceId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancel a running workflow instance.
    /// </summary>
    Task<WorkflowInstanceDto> CancelWorkflowAsync(
        Guid instanceId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve a single workflow instance by its ID.
    /// </summary>
    Task<WorkflowInstanceDto?> GetWorkflowInstanceAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve all workflow instances associated with a given entity.
    /// </summary>
    Task<List<WorkflowInstanceSummaryDto>> GetWorkflowsForEntityAsync(
        string entityType,
        Guid entityId,
        CancellationToken cancellationToken = default);
}
