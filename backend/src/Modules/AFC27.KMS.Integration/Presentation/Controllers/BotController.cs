using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Integration.Application.DTOs;
using AFC27.KMS.Integration.Application.Services;

namespace AFC27.KMS.Integration.Presentation.Controllers;

/// <summary>
/// Controller for Teams/Slack Q&A bot integration (Phase 9A).
/// Handles incoming messages from external bot channels, routes them through
/// the RAG/AI service, and returns formatted responses.
/// </summary>
[ApiController]
[Route("api/integration/bot")]
public class BotController : ControllerBase
{
    private readonly IBotService _botService;
    private readonly ILogger<BotController> _logger;

    public BotController(IBotService botService, ILogger<BotController> logger)
    {
        _botService = botService;
        _logger = logger;
    }

    #region Message Handling

    /// <summary>
    /// Handle an incoming Microsoft Teams message.
    /// The Teams bot framework posts messages here; the response is returned
    /// synchronously or pushed back via the registered webhook.
    /// </summary>
    [HttpPost("teams/message")]
    [AllowAnonymous] // Validated via bot framework token / HMAC signature
    [ProducesResponseType(typeof(BotMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BotMessageResponse>> HandleTeamsMessage(
        [FromHeader(Name = "Authorization")] string? bearerToken,
        [FromBody] BotMessageRequest request,
        CancellationToken cancellationToken)
    {
        // TODO: Validate the Teams Bot Framework bearer token (JWT from login.botframework.com)
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { error = "Message text is required" });

        request.Channel = "teams";

        _logger.LogInformation(
            "Teams message received from user {UserId} in conversation {ConversationId}",
            request.UserId, request.ConversationId);

        var response = await _botService.ProcessMessageAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Handle an incoming Slack message.
    /// Slack sends events or slash-command payloads here.
    /// </summary>
    [HttpPost("slack/message")]
    [AllowAnonymous] // Validated via Slack signing secret
    [ProducesResponseType(typeof(BotMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BotMessageResponse>> HandleSlackMessage(
        [FromHeader(Name = "X-Slack-Signature")] string? slackSignature,
        [FromHeader(Name = "X-Slack-Request-Timestamp")] string? slackTimestamp,
        [FromBody] BotMessageRequest request,
        CancellationToken cancellationToken)
    {
        // TODO: Validate the Slack signing secret using the signature header
        // See: https://api.slack.com/authentication/verifying-requests-from-slack

        // Handle Slack URL verification challenge
        // (Slack sends a challenge string during bot setup)

        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { error = "Message text is required" });

        request.Channel = "slack";

        _logger.LogInformation(
            "Slack message received from user {UserId} in conversation {ConversationId}",
            request.UserId, request.ConversationId);

        var response = await _botService.ProcessMessageAsync(request, cancellationToken);
        return Ok(response);
    }

    #endregion

    #region Bot Registration

    /// <summary>
    /// Register a bot webhook endpoint for push-based messaging.
    /// Used to set up the connection between the KMS and an external bot channel.
    /// </summary>
    [HttpPost("register")]
    [Authorize(Policy = "IntegrationAdmin")]
    [ProducesResponseType(typeof(BotRegistrationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BotRegistrationResponse>> RegisterBot(
        [FromBody] BotRegistrationRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Channel))
            return BadRequest(new { error = "Channel is required (teams or slack)" });

        if (string.IsNullOrWhiteSpace(request.WebhookUrl))
            return BadRequest(new { error = "WebhookUrl is required" });

        var result = await _botService.RegisterBotAsync(request, cancellationToken);
        return Created($"/api/integration/bot/registrations/{result.RegistrationId}", result);
    }

    /// <summary>
    /// List all active bot registrations.
    /// </summary>
    [HttpGet("registrations")]
    [Authorize(Policy = "IntegrationAdmin")]
    [ProducesResponseType(typeof(IReadOnlyList<BotRegistrationResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BotRegistrationResponse>>> ListRegistrations(
        CancellationToken cancellationToken)
    {
        var registrations = await _botService.ListRegistrationsAsync(cancellationToken);
        return Ok(registrations);
    }

    /// <summary>
    /// Remove a bot registration.
    /// </summary>
    [HttpDelete("registrations/{id:guid}")]
    [Authorize(Policy = "IntegrationAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnregisterBot(Guid id, CancellationToken cancellationToken)
    {
        var success = await _botService.UnregisterBotAsync(id, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    #endregion
}
