using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Application.Services;

namespace AFC27.KMS.Admin.Presentation.Controllers;

/// <summary>
/// Controller for managing webhook subscriptions.
/// </summary>
[ApiController]
[Route("api/admin/webhooks")]
[Authorize(Policy = "CanManageUsers")]
public class WebhookController : ControllerBase
{
    private readonly IWebhookService _webhookService;

    public WebhookController(IWebhookService webhookService)
    {
        _webhookService = webhookService;
    }

    /// <summary>
    /// Create a new webhook subscription.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(WebhookSubscriptionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateWebhookRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { error = "Name is required" });
        if (string.IsNullOrWhiteSpace(request.Url))
            return BadRequest(new { error = "URL is required" });
        if (string.IsNullOrWhiteSpace(request.Secret))
            return BadRequest(new { error = "Secret is required" });

        var webhook = await _webhookService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = webhook.Id }, webhook);
    }

    /// <summary>
    /// Get a webhook subscription by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WebhookSubscriptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var webhook = await _webhookService.GetByIdAsync(id, cancellationToken);
        if (webhook == null)
            return NotFound();

        return Ok(webhook);
    }

    /// <summary>
    /// List all webhook subscriptions.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<WebhookSubscriptionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var webhooks = await _webhookService.ListAsync(includeInactive, cancellationToken);
        return Ok(webhooks);
    }

    /// <summary>
    /// Update a webhook subscription.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(WebhookSubscriptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateWebhookRequest request,
        CancellationToken cancellationToken)
    {
        var webhook = await _webhookService.UpdateAsync(id, request, cancellationToken);
        if (webhook == null)
            return NotFound();

        return Ok(webhook);
    }

    /// <summary>
    /// Delete a webhook subscription.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var success = await _webhookService.DeleteAsync(id, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Activate a webhook subscription.
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var success = await _webhookService.ActivateAsync(id, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Deactivate a webhook subscription.
    /// </summary>
    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var success = await _webhookService.DeactivateAsync(id, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Get delivery logs for a webhook subscription.
    /// </summary>
    [HttpGet("{id:guid}/deliveries")]
    [ProducesResponseType(typeof(IReadOnlyList<WebhookDeliveryLogDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDeliveryLogs(
        Guid id,
        [FromQuery] int limit = 50,
        CancellationToken cancellationToken = default)
    {
        var logs = await _webhookService.GetDeliveryLogsAsync(id, limit, cancellationToken);
        return Ok(logs);
    }

    /// <summary>
    /// Test a webhook subscription by sending a test event.
    /// </summary>
    [HttpPost("{id:guid}/test")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Test(
        Guid id,
        [FromBody] TestWebhookRequest? request,
        CancellationToken cancellationToken)
    {
        var success = await _webhookService.TestAsync(id, request ?? new TestWebhookRequest(), cancellationToken);
        return success ? NoContent() : NotFound();
    }
}
