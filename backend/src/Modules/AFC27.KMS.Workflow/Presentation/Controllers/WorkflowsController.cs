using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Workflow.Application.DTOs;
using AFC27.KMS.Workflow.Domain.Entities;

namespace AFC27.KMS.Workflow.Presentation.Controllers;

/// <summary>
/// Controller for workflow definition management
/// </summary>
[ApiController]
[Route("api/workflow/definitions")]
[Authorize(Policy = "CanManageWorkflow")]
public class WorkflowsController : ControllerBase
{
    /// <summary>
    /// Get all workflow definitions
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WorkflowDefinitionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WorkflowDefinitionDto>>> GetWorkflows(
        [FromQuery] WorkflowStatus? status = null)
    {
        // TODO: Return workflows
        var workflows = new List<WorkflowDefinitionDto>();
        return Ok(workflows);
    }

    /// <summary>
    /// Get workflow by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WorkflowDefinitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkflowDefinitionDto>> GetWorkflow(Guid id)
    {
        // TODO: Return workflow
        return NotFound();
    }

    /// <summary>
    /// Create a new workflow definition
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(WorkflowDefinitionDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<WorkflowDefinitionDto>> CreateWorkflow([FromBody] CreateWorkflowRequest request)
    {
        // TODO: Create workflow
        var workflow = new WorkflowDefinitionDto
        {
            Id = Guid.NewGuid(),
            Name = request.NameEn,
            Description = request.DescriptionEn,
            Type = request.Type,
            Status = WorkflowStatus.Draft,
            Version = "1.0",
            Steps = new List<WorkflowStepDto>(),
            CreatedAt = DateTime.UtcNow
        };

        return CreatedAtAction(nameof(GetWorkflow), new { id = workflow.Id }, workflow);
    }

    /// <summary>
    /// Update a workflow definition
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(WorkflowDefinitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkflowDefinitionDto>> UpdateWorkflow(
        Guid id,
        [FromBody] CreateWorkflowRequest request)
    {
        // TODO: Update workflow
        return NotFound();
    }

    /// <summary>
    /// Add a step to workflow
    /// </summary>
    [HttpPost("{id:guid}/steps")]
    [ProducesResponseType(typeof(WorkflowStepDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<WorkflowStepDto>> AddStep(
        Guid id,
        [FromBody] CreateWorkflowStepRequest request)
    {
        // TODO: Add step
        var step = new WorkflowStepDto
        {
            Id = Guid.NewGuid(),
            Name = request.NameEn,
            Type = request.Type,
            Action = request.Action,
            AssignmentType = request.AssignmentType,
            Order = 1
        };

        return CreatedAtAction(nameof(GetWorkflow), new { id }, step);
    }

    /// <summary>
    /// Update a workflow step
    /// </summary>
    [HttpPut("{id:guid}/steps/{stepId:guid}")]
    [ProducesResponseType(typeof(WorkflowStepDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkflowStepDto>> UpdateStep(
        Guid id,
        Guid stepId,
        [FromBody] CreateWorkflowStepRequest request)
    {
        // TODO: Update step
        return NotFound();
    }

    /// <summary>
    /// Remove a step from workflow
    /// </summary>
    [HttpDelete("{id:guid}/steps/{stepId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveStep(Guid id, Guid stepId)
    {
        // TODO: Remove step
        return NoContent();
    }

    /// <summary>
    /// Reorder workflow steps
    /// </summary>
    [HttpPost("{id:guid}/steps/reorder")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ReorderSteps(Guid id, [FromBody] List<Guid> stepIds)
    {
        // TODO: Reorder steps
        return NoContent();
    }

    /// <summary>
    /// Publish a workflow
    /// </summary>
    [HttpPost("{id:guid}/publish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PublishWorkflow(Guid id)
    {
        // TODO: Publish workflow
        return NoContent();
    }

    /// <summary>
    /// Unpublish a workflow
    /// </summary>
    [HttpPost("{id:guid}/unpublish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UnpublishWorkflow(Guid id)
    {
        // TODO: Unpublish workflow
        return NoContent();
    }

    /// <summary>
    /// Archive a workflow
    /// </summary>
    [HttpPost("{id:guid}/archive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ArchiveWorkflow(Guid id)
    {
        // TODO: Archive workflow
        return NoContent();
    }

    /// <summary>
    /// Clone a workflow
    /// </summary>
    [HttpPost("{id:guid}/clone")]
    [ProducesResponseType(typeof(WorkflowDefinitionDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<WorkflowDefinitionDto>> CloneWorkflow(Guid id, [FromQuery] string? newName = null)
    {
        // TODO: Clone workflow
        var workflow = new WorkflowDefinitionDto
        {
            Id = Guid.NewGuid(),
            Name = newName ?? "Copy of Workflow",
            Status = WorkflowStatus.Draft,
            Version = "1.0",
            CreatedAt = DateTime.UtcNow
        };

        return CreatedAtAction(nameof(GetWorkflow), new { id = workflow.Id }, workflow);
    }

    /// <summary>
    /// Set as default workflow
    /// </summary>
    [HttpPost("{id:guid}/set-default")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetAsDefault(Guid id)
    {
        // TODO: Set as default
        return NoContent();
    }

    /// <summary>
    /// Delete a workflow
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteWorkflow(Guid id)
    {
        // TODO: Delete workflow (only if no active instances)
        return NoContent();
    }

    /// <summary>
    /// Get workflow types
    /// </summary>
    [HttpGet("types")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<object>> GetWorkflowTypes()
    {
        var types = Enum.GetValues<WorkflowType>()
            .Select(t => new { Value = (int)t, Name = t.ToString() });
        return Ok(types);
    }

    /// <summary>
    /// Get step types
    /// </summary>
    [HttpGet("step-types")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<object>> GetStepTypes()
    {
        var types = Enum.GetValues<StepType>()
            .Select(t => new { Value = (int)t, Name = t.ToString() });
        return Ok(types);
    }

    /// <summary>
    /// Get assignment types
    /// </summary>
    [HttpGet("assignment-types")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<object>> GetAssignmentTypes()
    {
        var types = Enum.GetValues<AssignmentType>()
            .Select(t => new { Value = (int)t, Name = t.ToString() });
        return Ok(types);
    }
}
