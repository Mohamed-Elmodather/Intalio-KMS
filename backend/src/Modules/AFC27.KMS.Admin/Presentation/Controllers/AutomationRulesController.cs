using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Application.Services;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.Admin.Presentation.Controllers;

/// <summary>
/// Controller for managing user-configurable automation rules (Phase 8E).
/// Allows users to create trigger-condition-action automations based on domain events.
/// </summary>
[ApiController]
[Route("api/admin/automation-rules")]
[Authorize]
public class AutomationRulesController : ControllerBase
{
    private readonly IAutomationRuleService _automationRuleService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<AutomationRulesController> _logger;

    public AutomationRulesController(
        IAutomationRuleService automationRuleService,
        ICurrentUser currentUser,
        ILogger<AutomationRulesController> logger)
    {
        _automationRuleService = automationRuleService;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <summary>
    /// List automation rules. Optionally filter by owner, space, or active status.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AutomationRuleSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AutomationRuleSummaryDto>>> ListRules(
        [FromQuery] Guid? ownerId = null,
        [FromQuery] Guid? spaceId = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var rules = await _automationRuleService.ListAsync(ownerId, spaceId, isActive, cancellationToken);
        return Ok(rules);
    }

    /// <summary>
    /// Get an automation rule by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AutomationRuleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AutomationRuleDto>> GetRule(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var rule = await _automationRuleService.GetByIdAsync(id, cancellationToken);
        if (rule == null)
            return NotFound();
        return Ok(rule);
    }

    /// <summary>
    /// Create a new automation rule.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AutomationRuleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AutomationRuleDto>> CreateRule(
        [FromBody] CreateAutomationRuleRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Rule name is required.");
        }
        if (string.IsNullOrWhiteSpace(request.TriggerEvent))
        {
            return BadRequest("Trigger event is required.");
        }

        var userId = _currentUser.UserId ?? Guid.Empty;
        var userName = _currentUser.DisplayName ?? "Unknown";

        _logger.LogInformation("Creating automation rule '{Name}' by {UserName}", request.Name, userName);

        var rule = await _automationRuleService.CreateAsync(request, userId, userName, cancellationToken);
        return CreatedAtAction(nameof(GetRule), new { id = rule.Id }, rule);
    }

    /// <summary>
    /// Update an existing automation rule.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AutomationRuleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AutomationRuleDto>> UpdateRule(
        Guid id,
        [FromBody] UpdateAutomationRuleRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating automation rule {RuleId}", id);

        var rule = await _automationRuleService.UpdateAsync(id, request, cancellationToken);
        if (rule == null)
            return NotFound();
        return Ok(rule);
    }

    /// <summary>
    /// Delete an automation rule.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRule(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting automation rule {RuleId}", id);

        var success = await _automationRuleService.DeleteAsync(id, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Activate an automation rule.
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateRule(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Activating automation rule {RuleId}", id);

        var success = await _automationRuleService.ActivateAsync(id, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Deactivate an automation rule.
    /// </summary>
    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateRule(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deactivating automation rule {RuleId}", id);

        var success = await _automationRuleService.DeactivateAsync(id, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Get execution history for a rule.
    /// </summary>
    [HttpGet("{id:guid}/executions")]
    [ProducesResponseType(typeof(IReadOnlyList<AutomationRuleExecutionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AutomationRuleExecutionDto>>> GetExecutionHistory(
        Guid id,
        [FromQuery] int maxResults = 50,
        CancellationToken cancellationToken = default)
    {
        var history = await _automationRuleService.GetExecutionHistoryAsync(id, maxResults, cancellationToken);
        return Ok(history);
    }

    /// <summary>
    /// Get available trigger events with metadata for the rule builder UI.
    /// </summary>
    [HttpGet("trigger-events")]
    [ProducesResponseType(typeof(IReadOnlyList<TriggerEventMetadataDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TriggerEventMetadataDto>>> GetAvailableTriggerEvents(
        CancellationToken cancellationToken = default)
    {
        var events = await _automationRuleService.GetAvailableTriggerEventsAsync(cancellationToken);
        return Ok(events);
    }
}
