using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AFC27.KMS.AI.Application.DTOs;
using AFC27.KMS.AI.Application.Services;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.AI.Presentation.Controllers;

/// <summary>
/// API controller for AI-powered translation of articles, content blocks, and text.
/// Supports EN/AR bidirectional translation with domain-specific terminology.
/// </summary>
[ApiController]
[Route("api/ai/translation")]
[Authorize]
public class TranslationController : ControllerBase
{
    private readonly ITranslationService _translationService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<TranslationController> _logger;

    public TranslationController(
        ITranslationService translationService,
        ICurrentUser currentUser,
        ILogger<TranslationController> logger)
    {
        _translationService = translationService;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <summary>
    /// Translate all blocks of an article between EN and AR.
    /// Fetches the article's content blocks, translates each one, and returns the results.
    /// </summary>
    /// <param name="id">The article ID to translate.</param>
    /// <param name="request">Translation parameters including target language and domain hint.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("article/{id:guid}")]
    [ProducesResponseType(typeof(TranslationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TranslateArticle(
        Guid id,
        [FromBody] TranslateArticleRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TargetLanguage))
            return BadRequest(new { error = "TargetLanguage is required" });

        if (request.TargetLanguage is not ("en" or "ar"))
            return BadRequest(new { error = "TargetLanguage must be 'en' or 'ar'" });

        _logger.LogInformation(
            "Article translation request from user {UserId} for article {ArticleId} to {TargetLanguage}",
            _currentUser.UserId, id, request.TargetLanguage);

        var response = await _translationService.TranslateArticleAsync(
            id, request, _currentUser.UserId ?? Guid.Empty, cancellationToken);

        if (!response.Success && response.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            return NotFound(new { error = $"Article {id} not found" });

        return Ok(response);
    }

    /// <summary>
    /// Translate a set of content blocks between EN and AR.
    /// Each block is translated individually, preserving block type and structure.
    /// </summary>
    /// <param name="request">The blocks to translate with translation parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("blocks")]
    [ProducesResponseType(typeof(TranslationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TranslateBlocks(
        [FromBody] TranslateBlocksRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Blocks == null || request.Blocks.Count == 0)
            return BadRequest(new { error = "At least one block is required" });

        if (string.IsNullOrWhiteSpace(request.TargetLanguage))
            return BadRequest(new { error = "TargetLanguage is required" });

        if (request.TargetLanguage is not ("en" or "ar"))
            return BadRequest(new { error = "TargetLanguage must be 'en' or 'ar'" });

        _logger.LogInformation(
            "Block translation request from user {UserId}, {BlockCount} blocks to {TargetLanguage}",
            _currentUser.UserId, request.Blocks.Count, request.TargetLanguage);

        var response = await _translationService.TranslateBlocksAsync(
            request, _currentUser.UserId ?? Guid.Empty, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Translate arbitrary text between EN and AR.
    /// Supports domain-specific terminology and formatting preservation.
    /// </summary>
    /// <param name="request">The text to translate with translation parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("text")]
    [ProducesResponseType(typeof(TranslationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TranslateText(
        [FromBody] TranslateTextRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { error = "Text is required" });

        if (string.IsNullOrWhiteSpace(request.TargetLanguage))
            return BadRequest(new { error = "TargetLanguage is required" });

        if (request.TargetLanguage is not ("en" or "ar"))
            return BadRequest(new { error = "TargetLanguage must be 'en' or 'ar'" });

        _logger.LogInformation(
            "Text translation request from user {UserId} to {TargetLanguage}",
            _currentUser.UserId, request.TargetLanguage);

        var response = await _translationService.TranslateTextAsync(
            request, _currentUser.UserId ?? Guid.Empty, cancellationToken);

        return Ok(response);
    }
}
