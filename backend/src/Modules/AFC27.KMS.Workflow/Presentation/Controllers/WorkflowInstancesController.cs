using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Workflow.Application.DTOs;
using AFC27.KMS.Workflow.Application.Services;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.Workflow.Presentation.Controllers;

/// <summary>
/// Controller for managing generalized workflow instances.
/// </summary>
[ApiController]
[Route("api/workflow/instances")]
[Authorize]
public class WorkflowInstancesController : ControllerBase
{
    private readonly IWorkflowEngineService _workflowEngine;
    private readonly ICurrentUser _currentUser;

    public WorkflowInstancesController(
        IWorkflowEngineService workflowEngine,
        ICurrentUser currentUser)
    {
        _workflowEngine = workflowEngine;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Start a new workflow instance from a published definition.
    /// </summary>
    [HttpPost("start")]
    [ProducesResponseType(typeof(WorkflowInstanceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WorkflowInstanceDto>> StartWorkflow(
        [FromBody] StartWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        try
        {
            var instance = await _workflowEngine.StartWorkflowAsync(
                request.WorkflowDefinitionId,
                request.TargetEntityType,
                request.TargetEntityId,
                _currentUser.UserId.Value,
                cancellationToken);

            return CreatedAtAction(nameof(GetInstance), new { id = instance.Id }, instance);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get a workflow instance by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WorkflowInstanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkflowInstanceDto>> GetInstance(
        Guid id,
        CancellationToken cancellationToken)
    {
        var instance = await _workflowEngine.GetWorkflowInstanceAsync(id, cancellationToken);
        if (instance is null)
            return NotFound();

        return Ok(instance);
    }

    /// <summary>
    /// Get all workflow instances for a specific entity.
    /// </summary>
    [HttpGet("by-entity")]
    [ProducesResponseType(typeof(List<WorkflowInstanceSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<WorkflowInstanceSummaryDto>>> GetWorkflowsForEntity(
        [FromQuery] string entityType,
        [FromQuery] Guid entityId,
        CancellationToken cancellationToken)
    {
        var instances = await _workflowEngine.GetWorkflowsForEntityAsync(
            entityType, entityId, cancellationToken);

        return Ok(instances);
    }

    /// <summary>
    /// Advance (complete) the current step of a workflow instance.
    /// </summary>
    [HttpPost("{id:guid}/advance")]
    [ProducesResponseType(typeof(WorkflowInstanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkflowInstanceDto>> AdvanceStep(
        Guid id,
        [FromBody] AdvanceStepRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        try
        {
            var instance = await _workflowEngine.AdvanceStepAsync(
                id, request.Outcome, request.Comments,
                _currentUser.UserId.Value, cancellationToken);

            return Ok(instance);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Reject the current step of a workflow instance.
    /// </summary>
    [HttpPost("{id:guid}/reject")]
    [ProducesResponseType(typeof(WorkflowInstanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WorkflowInstanceDto>> RejectStep(
        Guid id,
        [FromBody] RejectStepRequest request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        try
        {
            var instance = await _workflowEngine.RejectStepAsync(
                id, request.Reason,
                _currentUser.UserId.Value, cancellationToken);

            return Ok(instance);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Skip the current step of a workflow instance.
    /// </summary>
    [HttpPost("{id:guid}/skip")]
    [ProducesResponseType(typeof(WorkflowInstanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WorkflowInstanceDto>> SkipStep(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        try
        {
            var instance = await _workflowEngine.SkipStepAsync(
                id, _currentUser.UserId.Value, cancellationToken);

            return Ok(instance);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Cancel a running workflow instance.
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(WorkflowInstanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WorkflowInstanceDto>> CancelWorkflow(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        try
        {
            var instance = await _workflowEngine.CancelWorkflowAsync(
                id, _currentUser.UserId.Value, cancellationToken);

            return Ok(instance);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
