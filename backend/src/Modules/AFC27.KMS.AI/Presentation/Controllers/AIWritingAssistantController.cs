using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AFC27.KMS.AI.Application.DTOs;
using AFC27.KMS.AI.Application.Services;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.AI.Presentation.Controllers;

/// <summary>
/// API controller for inline AI writing assistance.
/// Provides suggest, generate, improve, tone-change, translate, summarize, continue, and streaming endpoints.
/// </summary>
[ApiController]
[Route("api/ai/writing")]
[Authorize]
public class AIWritingAssistantController : ControllerBase
{
    private readonly IWritingAssistantService _writingService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<AIWritingAssistantController> _logger;

    public AIWritingAssistantController(
        IWritingAssistantService writingService,
        ICurrentUser currentUser,
        ILogger<AIWritingAssistantController> logger)
    {
        _writingService = writingService;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <summary>
    /// Suggest improvements for the given text.
    /// Returns the improved text along with a list of specific suggested changes.
    /// </summary>
    [HttpPost("suggest")]
    [ProducesResponseType(typeof(AIWritingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Suggest(
        [FromBody] AIWritingRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { error = "Text is required" });

        _logger.LogInformation("Writing suggest request from user {UserId}", _currentUser.UserId);

        var response = await _writingService.SuggestAsync(
            request, _currentUser.UserId ?? Guid.Empty, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Generate content from a prompt.
    /// Supports optional style, max length, language, and template ID parameters.
    /// </summary>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(AIWritingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Generate(
        [FromBody] AIGenerateRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
            return BadRequest(new { error = "Prompt is required" });

        _logger.LogInformation("Writing generate request from user {UserId}", _currentUser.UserId);

        var response = await _writingService.GenerateAsync(
            request, _currentUser.UserId ?? Guid.Empty, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Improve text grammar, clarity, and structure.
    /// Returns the improved version of the provided text.
    /// </summary>
    [HttpPost("improve")]
    [ProducesResponseType(typeof(AIWritingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Improve(
        [FromBody] AIWritingRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { error = "Text is required" });

        _logger.LogInformation("Writing improve request from user {UserId}", _currentUser.UserId);

        var response = await _writingService.ImproveAsync(
            request, _currentUser.UserId ?? Guid.Empty, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Change the tone of text to match a target audience.
    /// Supported tones: Formal, Casual, Simplified, Technical, Diplomatic, Persuasive.
    /// </summary>
    [HttpPost("change-tone")]
    [ProducesResponseType(typeof(AIWritingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeTone(
        [FromBody] AIToneChangeRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { error = "Text is required" });

        _logger.LogInformation("Writing change-tone request from user {UserId}, tone: {Tone}",
            _currentUser.UserId, request.TargetTone);

        var response = await _writingService.ChangeToneAsync(
            request, _currentUser.UserId ?? Guid.Empty, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Translate text between languages (EN/AR and others).
    /// Auto-detects source language if not provided.
    /// </summary>
    [HttpPost("translate")]
    [ProducesResponseType(typeof(AIWritingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Translate(
        [FromBody] AITranslateRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { error = "Text is required" });

        if (string.IsNullOrWhiteSpace(request.TargetLanguage))
            return BadRequest(new { error = "TargetLanguage is required" });

        _logger.LogInformation("Writing translate request from user {UserId}, target: {Target}",
            _currentUser.UserId, request.TargetLanguage);

        var response = await _writingService.TranslateAsync(
            request, _currentUser.UserId ?? Guid.Empty, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Summarize text into a shorter version.
    /// Supports optional max sentence count and focus area.
    /// </summary>
    [HttpPost("summarize")]
    [ProducesResponseType(typeof(AIWritingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Summarize(
        [FromBody] AISummarizeRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { error = "Text is required" });

        _logger.LogInformation("Writing summarize request from user {UserId}", _currentUser.UserId);

        var response = await _writingService.SummarizeAsync(
            request, _currentUser.UserId ?? Guid.Empty, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Continue writing from the given text position.
    /// Maintains the style, tone, and language of the existing content.
    /// </summary>
    [HttpPost("continue")]
    [ProducesResponseType(typeof(AIWritingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Continue(
        [FromBody] AIContinueRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { error = "Text is required" });

        _logger.LogInformation("Writing continue request from user {UserId}", _currentUser.UserId);

        var response = await _writingService.ContinueAsync(
            request, _currentUser.UserId ?? Guid.Empty, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Stream a writing operation result via Server-Sent Events (SSE).
    /// Supports all writing operations: Generate, Improve, ChangeTone, Translate, Summarize, Continue, Suggest.
    /// </summary>
    [HttpPost("stream")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task Stream(
        [FromBody] AIStreamWritingRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            await Response.WriteAsync("{\"error\":\"Text is required\"}");
            return;
        }

        _logger.LogInformation(
            "Writing stream request from user {UserId}, operation: {Operation}",
            _currentUser.UserId, request.Operation);

        // Set up SSE response
        Response.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Connection = "keep-alive";

        var fullContent = new StringBuilder();

        try
        {
            await foreach (var chunk in _writingService.StreamAsync(
                request, _currentUser.UserId ?? Guid.Empty, cancellationToken))
            {
                fullContent.Append(chunk.Content);

                if (chunk.IsComplete)
                {
                    await SendSSEEvent("done", new
                    {
                        fullContent = fullContent.ToString(),
                        tokenCount = chunk.TokenCount
                    });
                }
                else if (!string.IsNullOrEmpty(chunk.Content))
                {
                    await SendSSEEvent("chunk", new
                    {
                        content = chunk.Content,
                        tokenCount = chunk.TokenCount
                    });
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Writing stream cancelled by user {UserId}", _currentUser.UserId);
            await SendSSEEvent("cancelled", new { message = "Stream cancelled by client" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during writing stream for user {UserId}", _currentUser.UserId);
            await SendSSEEvent("error", new { error = "An error occurred during processing" });
        }
    }

    private async Task SendSSEEvent(string eventType, object data)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(data);
        await Response.WriteAsync($"event: {eventType}\ndata: {json}\n\n");
        await Response.Body.FlushAsync();
    }
}
