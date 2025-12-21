using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Content.Application.DTOs;
using AFC27.KMS.Content.Application.Interfaces;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.Content.Presentation.Controllers;

/// <summary>
/// Controller for block-based content editing operations.
/// </summary>
[ApiController]
[Route("api/articles/{articleId:guid}/blocks")]
[Authorize]
public class BlockEditorController : ControllerBase
{
    private readonly IBlockEditorService _blockEditorService;
    private readonly ICollaborationService _collaborationService;
    private readonly ICurrentUser _currentUser;

    public BlockEditorController(
        IBlockEditorService blockEditorService,
        ICollaborationService collaborationService,
        ICurrentUser currentUser)
    {
        _blockEditorService = blockEditorService;
        _collaborationService = collaborationService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Get all blocks for an article.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ContentBlockDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ContentBlockDto>>> GetBlocks(
        Guid articleId,
        CancellationToken cancellationToken)
    {
        var blocks = await _blockEditorService.GetBlocksAsync(articleId, cancellationToken);
        return Ok(blocks);
    }

    /// <summary>
    /// Create a new block.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ContentBlockDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ContentBlockDto>> CreateBlock(
        Guid articleId,
        [FromBody] CreateBlockRequest request,
        CancellationToken cancellationToken)
    {
        var block = await _blockEditorService.CreateBlockAsync(
            articleId,
            request,
            _currentUser.UserId ?? Guid.Empty,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetBlocks),
            new { articleId },
            block);
    }

    /// <summary>
    /// Update a block's content.
    /// </summary>
    [HttpPut("{blockId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBlock(
        Guid articleId,
        Guid blockId,
        [FromBody] UpdateBlockRequest request,
        CancellationToken cancellationToken)
    {
        var success = await _blockEditorService.UpdateBlockAsync(
            blockId,
            request,
            _currentUser.UserId ?? Guid.Empty,
            cancellationToken);

        if (!success)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Delete a block.
    /// </summary>
    [HttpDelete("{blockId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBlock(
        Guid articleId,
        Guid blockId,
        CancellationToken cancellationToken)
    {
        var success = await _blockEditorService.DeleteBlockAsync(
            blockId,
            _currentUser.UserId ?? Guid.Empty,
            cancellationToken);

        if (!success)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Reorder blocks.
    /// </summary>
    [HttpPut("reorder")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReorderBlocks(
        Guid articleId,
        [FromBody] ReorderBlocksRequest request,
        CancellationToken cancellationToken)
    {
        var success = await _blockEditorService.ReorderBlocksAsync(
            articleId,
            request.BlockIds,
            _currentUser.UserId ?? Guid.Empty,
            cancellationToken);

        if (!success)
            return BadRequest("Failed to reorder blocks");

        return NoContent();
    }

    /// <summary>
    /// Move a block to a new position.
    /// </summary>
    [HttpPut("{blockId:guid}/move")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MoveBlock(
        Guid articleId,
        Guid blockId,
        [FromBody] MoveBlockRequest request,
        CancellationToken cancellationToken)
    {
        var success = await _blockEditorService.MoveBlockAsync(
            blockId,
            request.NewPosition,
            request.NewParentId,
            _currentUser.UserId ?? Guid.Empty,
            cancellationToken);

        if (!success)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Duplicate a block.
    /// </summary>
    [HttpPost("{blockId:guid}/duplicate")]
    [ProducesResponseType(typeof(ContentBlockDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContentBlockDto>> DuplicateBlock(
        Guid articleId,
        Guid blockId,
        CancellationToken cancellationToken)
    {
        var duplicate = await _blockEditorService.DuplicateBlockAsync(
            blockId,
            _currentUser.UserId ?? Guid.Empty,
            cancellationToken);

        if (duplicate == null)
            return NotFound();

        return CreatedAtAction(
            nameof(GetBlocks),
            new { articleId },
            duplicate);
    }

    /// <summary>
    /// Render blocks to HTML.
    /// </summary>
    [HttpGet("render")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> RenderToHtml(
        Guid articleId,
        [FromQuery] string language = "en",
        CancellationToken cancellationToken = default)
    {
        var html = await _blockEditorService.RenderToHtmlAsync(articleId, language, cancellationToken);
        return Ok(html);
    }

    /// <summary>
    /// Import content from HTML.
    /// </summary>
    [HttpPost("import")]
    [ProducesResponseType(typeof(IReadOnlyList<ContentBlockDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<IReadOnlyList<ContentBlockDto>>> ImportFromHtml(
        Guid articleId,
        [FromBody] ImportHtmlRequest request,
        CancellationToken cancellationToken)
    {
        var blocks = await _blockEditorService.ImportFromHtmlAsync(
            articleId,
            request.Html,
            _currentUser.UserId ?? Guid.Empty,
            cancellationToken);

        return CreatedAtAction(nameof(GetBlocks), new { articleId }, blocks);
    }

    /// <summary>
    /// Get collaboration session info for an article.
    /// </summary>
    [HttpGet("collaboration")]
    [ProducesResponseType(typeof(CollaborationSessionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CollaborationSessionDto>> GetCollaborationSession(
        Guid articleId,
        CancellationToken cancellationToken)
    {
        var session = await _collaborationService.GetActiveSessionAsync(articleId, cancellationToken);

        if (session == null)
            return NotFound();

        return Ok(session);
    }
}

/// <summary>
/// Request to import HTML content.
/// </summary>
public record ImportHtmlRequest
{
    public string Html { get; init; } = string.Empty;
}
