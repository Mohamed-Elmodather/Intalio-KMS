using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Workflow.Application.DTOs;
using AFC27.KMS.Workflow.Domain.Entities;
using TaskStatus = AFC27.KMS.Workflow.Domain.Entities.TaskStatus;

namespace AFC27.KMS.Workflow.Presentation.Controllers;

/// <summary>
/// Controller for workflow task management
/// </summary>
[ApiController]
[Route("api/workflow/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    /// <summary>
    /// Get my task inbox
    /// </summary>
    [HttpGet("inbox")]
    [ProducesResponseType(typeof(IEnumerable<TaskSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TaskSummaryDto>>> GetInbox([FromQuery] TaskFilterDto filter)
    {
        // TODO: Return tasks assigned to current user
        var tasks = new List<TaskSummaryDto>();
        return Ok(tasks);
    }

    /// <summary>
    /// Get inbox statistics
    /// </summary>
    [HttpGet("inbox/stats")]
    [ProducesResponseType(typeof(TaskInboxStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TaskInboxStatsDto>> GetInboxStats()
    {
        // TODO: Return inbox statistics
        var stats = new TaskInboxStatsDto
        {
            TotalPending = 0,
            Overdue = 0,
            DueToday = 0,
            DueThisWeek = 0,
            Unread = 0
        };

        return Ok(stats);
    }

    /// <summary>
    /// Get all tasks (admin)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "CanViewAllTasks")]
    [ProducesResponseType(typeof(IEnumerable<TaskSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TaskSummaryDto>>> GetAllTasks([FromQuery] TaskFilterDto filter)
    {
        // TODO: Return all tasks
        var tasks = new List<TaskSummaryDto>();
        return Ok(tasks);
    }

    /// <summary>
    /// Get task by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WorkflowTaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkflowTaskDto>> GetTask(Guid id)
    {
        // TODO: Return task
        return NotFound();
    }

    /// <summary>
    /// Get task by task number
    /// </summary>
    [HttpGet("by-number/{taskNumber}")]
    [ProducesResponseType(typeof(WorkflowTaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkflowTaskDto>> GetTaskByNumber(string taskNumber)
    {
        // TODO: Return task by number
        return NotFound();
    }

    /// <summary>
    /// Get tasks for a service request
    /// </summary>
    [HttpGet("by-request/{requestId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<TaskSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TaskSummaryDto>>> GetTasksForRequest(Guid requestId)
    {
        // TODO: Return tasks for request
        var tasks = new List<TaskSummaryDto>();
        return Ok(tasks);
    }

    /// <summary>
    /// Mark task as read
    /// </summary>
    [HttpPost("{id:guid}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        // TODO: Mark task as read
        return NoContent();
    }

    /// <summary>
    /// Start working on a task
    /// </summary>
    [HttpPost("{id:guid}/start")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartTask(Guid id)
    {
        // TODO: Start task
        return NoContent();
    }

    /// <summary>
    /// Complete a task
    /// </summary>
    [HttpPost("{id:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompleteTask(Guid id, [FromBody] CompleteTaskRequest request)
    {
        // TODO: Complete task
        return NoContent();
    }

    /// <summary>
    /// Approve a task
    /// </summary>
    [HttpPost("{id:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ApproveTask(Guid id, [FromBody] string? comments = null)
    {
        // TODO: Approve task
        return NoContent();
    }

    /// <summary>
    /// Reject a task
    /// </summary>
    [HttpPost("{id:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RejectTask(Guid id, [FromBody] string comments)
    {
        // TODO: Reject task
        return NoContent();
    }

    /// <summary>
    /// Delegate a task to another user
    /// </summary>
    [HttpPost("{id:guid}/delegate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DelegateTask(Guid id, [FromBody] DelegateTaskRequest request)
    {
        // TODO: Delegate task
        return NoContent();
    }

    /// <summary>
    /// Escalate a task
    /// </summary>
    [HttpPost("{id:guid}/escalate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> EscalateTask(
        Guid id,
        [FromQuery] Guid toUserId,
        [FromBody] string reason)
    {
        // TODO: Escalate task
        return NoContent();
    }

    /// <summary>
    /// Claim an unassigned task
    /// </summary>
    [HttpPost("{id:guid}/claim")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ClaimTask(Guid id)
    {
        // TODO: Claim task
        return NoContent();
    }

    /// <summary>
    /// Release a claimed task
    /// </summary>
    [HttpPost("{id:guid}/release")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ReleaseTask(Guid id)
    {
        // TODO: Release task
        return NoContent();
    }

    /// <summary>
    /// Get available outcomes for a task
    /// </summary>
    [HttpGet("{id:guid}/outcomes")]
    [ProducesResponseType(typeof(IEnumerable<StepOutcomeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StepOutcomeDto>>> GetOutcomes(Guid id)
    {
        // TODO: Return available outcomes
        var outcomes = new List<StepOutcomeDto>
        {
            new() { Name = "Approved", Label = "Approve" },
            new() { Name = "Rejected", Label = "Reject" },
            new() { Name = "NeedMoreInfo", Label = "Need More Information" }
        };

        return Ok(outcomes);
    }

    /// <summary>
    /// Bulk approve tasks
    /// </summary>
    [HttpPost("bulk/approve")]
    [ProducesResponseType(typeof(BulkTaskResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BulkTaskResultDto>> BulkApprove(
        [FromBody] List<Guid> taskIds,
        [FromQuery] string? comments = null)
    {
        // TODO: Bulk approve
        var result = new BulkTaskResultDto
        {
            Processed = taskIds.Count,
            Succeeded = taskIds.Count,
            Failed = 0
        };

        return Ok(result);
    }

    /// <summary>
    /// Bulk reject tasks
    /// </summary>
    [HttpPost("bulk/reject")]
    [ProducesResponseType(typeof(BulkTaskResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BulkTaskResultDto>> BulkReject(
        [FromBody] List<Guid> taskIds,
        [FromQuery] string comments)
    {
        // TODO: Bulk reject
        var result = new BulkTaskResultDto
        {
            Processed = taskIds.Count,
            Succeeded = taskIds.Count,
            Failed = 0
        };

        return Ok(result);
    }

    /// <summary>
    /// Get task types
    /// </summary>
    [HttpGet("types")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<object>> GetTaskTypes()
    {
        var types = Enum.GetValues<TaskType>()
            .Select(t => new { Value = (int)t, Name = t.ToString() });
        return Ok(types);
    }

    /// <summary>
    /// Get task statuses
    /// </summary>
    [HttpGet("statuses")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<object>> GetTaskStatuses()
    {
        var statuses = Enum.GetValues<TaskStatus>()
            .Select(s => new { Value = (int)s, Name = s.ToString() });
        return Ok(statuses);
    }
}

/// <summary>
/// Result of bulk task operation
/// </summary>
public record BulkTaskResultDto
{
    public int Processed { get; init; }
    public int Succeeded { get; init; }
    public int Failed { get; init; }
    public List<string> Errors { get; init; } = new();
}
