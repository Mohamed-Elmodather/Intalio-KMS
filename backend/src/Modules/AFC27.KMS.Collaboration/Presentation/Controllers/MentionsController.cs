using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Collaboration.Application.DTOs;
using AFC27.KMS.Contracts.Common;

namespace AFC27.KMS.Collaboration.Presentation.Controllers;

/// <summary>
/// Controller for managing @mentions in article body content (Phase 8A).
/// Supports in-content mentions independent of comments.
/// </summary>
[ApiController]
[Route("api/collaboration/mentions")]
[Authorize]
public class MentionsController : ControllerBase
{
    private readonly ILogger<MentionsController> _logger;

    public MentionsController(ILogger<MentionsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Create an in-content mention (e.g. @mention inside an article body block).
    /// Unlike comment mentions, these are standalone entities tied to a content block.
    /// </summary>
    [HttpPost("in-content")]
    public async Task<ActionResult<ApiResponse<InContentMentionDto>>> CreateInContentMention(
        [FromBody] CreateInContentMentionRequest request)
    {
        _logger.LogInformation(
            "Creating in-content mention for user {MentionedUserId} in {EntityType} {EntityId}, block {BlockId}",
            request.MentionedUserId, request.TargetEntityType, request.TargetEntityId, request.BlockId);

        await Task.Delay(50);

        // In a real implementation, this would:
        // 1. Validate the target entity and block exist
        // 2. Create the Mention entity using Mention.CreateInContent(...)
        // 3. Persist via repository
        // 4. Domain event triggers notification to mentioned user

        var currentUserId = Guid.NewGuid(); // Would come from ICurrentUser
        var mention = new InContentMentionDto
        {
            Id = Guid.NewGuid(),
            MentionedUserId = request.MentionedUserId,
            MentionedUserName = "Mentioned User",
            MentionedUserAvatarUrl = "/avatars/mentioned-user.jpg",
            MentionedUserJobTitle = "Specialist",
            MentionedByUserId = currentUserId,
            MentionedByUserName = "Current User",
            TargetEntityType = request.TargetEntityType,
            TargetEntityId = request.TargetEntityId,
            BlockId = request.BlockId,
            StartIndex = request.StartIndex,
            EndIndex = request.EndIndex,
            MentionContext = "article-body",
            CreatedAt = DateTime.UtcNow
        };

        return CreatedAtAction(
            nameof(GetInContentMentions),
            new { targetEntityType = request.TargetEntityType, targetEntityId = request.TargetEntityId },
            ApiResponse<InContentMentionDto>.Ok(mention));
    }

    /// <summary>
    /// Get all in-content mentions for a given entity (e.g. all @mentions in an article body).
    /// </summary>
    [HttpGet("in-content")]
    public ActionResult<ApiResponse<IReadOnlyList<InContentMentionDto>>> GetInContentMentions(
        [FromQuery] string targetEntityType,
        [FromQuery] Guid targetEntityId,
        [FromQuery] Guid? blockId = null)
    {
        var mentions = new List<InContentMentionDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                MentionedUserId = Guid.NewGuid(),
                MentionedUserName = "Sara Ali",
                MentionedUserAvatarUrl = "/avatars/sara.jpg",
                MentionedUserJobTitle = "Project Manager",
                MentionedByUserId = Guid.NewGuid(),
                MentionedByUserName = "Mohammed Al-Rashid",
                TargetEntityType = targetEntityType,
                TargetEntityId = targetEntityId,
                BlockId = blockId ?? Guid.NewGuid(),
                StartIndex = 25,
                EndIndex = 34,
                MentionContext = "article-body",
                CreatedAt = DateTime.UtcNow.AddHours(-3)
            },
            new()
            {
                Id = Guid.NewGuid(),
                MentionedUserId = Guid.NewGuid(),
                MentionedUserName = "Ahmed Hassan",
                MentionedUserAvatarUrl = null,
                MentionedUserJobTitle = "Coordinator",
                MentionedByUserId = Guid.NewGuid(),
                MentionedByUserName = "Mohammed Al-Rashid",
                TargetEntityType = targetEntityType,
                TargetEntityId = targetEntityId,
                BlockId = blockId ?? Guid.NewGuid(),
                StartIndex = 100,
                EndIndex = 113,
                MentionContext = "article-body",
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            }
        };

        if (blockId.HasValue)
        {
            mentions = mentions.Where(m => m.BlockId == blockId.Value).ToList();
        }

        return Ok(ApiResponse<IReadOnlyList<InContentMentionDto>>.Ok(mentions));
    }

    /// <summary>
    /// Delete an in-content mention (e.g. when the @mention text is removed from the block).
    /// </summary>
    [HttpDelete("in-content/{id:guid}")]
    public async Task<ActionResult<ApiResponse>> DeleteInContentMention(Guid id)
    {
        _logger.LogInformation("Deleting in-content mention {MentionId}", id);

        await Task.Delay(50);

        return Ok(ApiResponse.Ok("In-content mention deleted"));
    }

    /// <summary>
    /// Get all mentions (in-content and comment) for the current user.
    /// Useful for a "Mentions" notification feed.
    /// </summary>
    [HttpGet("my-mentions")]
    public ActionResult<ApiResponse<IReadOnlyList<InContentMentionDto>>> GetMyMentions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var mentions = new List<InContentMentionDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                MentionedUserId = Guid.NewGuid(),
                MentionedUserName = "Current User",
                MentionedByUserId = Guid.NewGuid(),
                MentionedByUserName = "Sara Ali",
                TargetEntityType = "Article",
                TargetEntityId = Guid.NewGuid(),
                BlockId = Guid.NewGuid(),
                StartIndex = 10,
                EndIndex = 22,
                MentionContext = "article-body",
                CreatedAt = DateTime.UtcNow.AddHours(-1)
            }
        };

        return Ok(ApiResponse<IReadOnlyList<InContentMentionDto>>.Ok(mentions));
    }
}
