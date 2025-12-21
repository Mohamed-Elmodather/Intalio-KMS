using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Collaboration.Application.DTOs;
using AFC27.KMS.Contracts.Common;

namespace AFC27.KMS.Collaboration.Presentation.Controllers;

/// <summary>
/// Comments management controller.
/// </summary>
[ApiController]
[Route("api/collaboration/comments")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ILogger<CommentsController> _logger;

    public CommentsController(ILogger<CommentsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get comments for a target (discussion, article, document, etc.).
    /// </summary>
    [HttpGet]
    public ActionResult<ApiResponse<IReadOnlyList<CommentDto>>> GetComments(
        [FromQuery] string targetType,
        [FromQuery] Guid targetId)
    {
        var comments = new List<CommentDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Content = "Great article! Very informative and well-written.",
                TargetType = targetType,
                TargetId = targetId,
                AuthorId = Guid.NewGuid(),
                AuthorName = "Sara Ali",
                AuthorAvatarUrl = "/avatars/sara.jpg",
                AuthorJobTitle = "Project Manager",
                LikeCount = 5,
                ReplyCount = 2,
                IsLikedByCurrentUser = false,
                Replies = new List<CommentDto>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Content = "Thank you! Glad you found it useful.",
                        AuthorId = Guid.NewGuid(),
                        AuthorName = "Mohammed Al-Rashid",
                        AuthorJobTitle = "Operations Director",
                        LikeCount = 2,
                        CreatedAt = DateTime.UtcNow.AddHours(-2)
                    }
                },
                CreatedAt = DateTime.UtcNow.AddHours(-5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Content = "Could you provide more details about the security protocols mentioned?",
                TargetType = targetType,
                TargetId = targetId,
                AuthorId = Guid.NewGuid(),
                AuthorName = "Ahmed Hassan",
                AuthorJobTitle = "Coordinator",
                LikeCount = 3,
                ReplyCount = 0,
                CreatedAt = DateTime.UtcNow.AddHours(-3)
            }
        };

        return Ok(ApiResponse<IReadOnlyList<CommentDto>>.Ok(comments));
    }

    /// <summary>
    /// Get comment by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public ActionResult<ApiResponse<CommentDto>> GetComment(Guid id)
    {
        var comment = new CommentDto
        {
            Id = id,
            Content = "Great article! Very informative and well-written.",
            TargetType = "Article",
            TargetId = Guid.NewGuid(),
            AuthorId = Guid.NewGuid(),
            AuthorName = "Sara Ali",
            AuthorAvatarUrl = "/avatars/sara.jpg",
            AuthorJobTitle = "Project Manager",
            LikeCount = 5,
            ReplyCount = 2,
            CreatedAt = DateTime.UtcNow.AddHours(-5)
        };

        return Ok(ApiResponse<CommentDto>.Ok(comment));
    }

    /// <summary>
    /// Create a new comment.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CommentDto>>> CreateComment([FromBody] CreateCommentRequest request)
    {
        _logger.LogInformation("Creating comment on {TargetType} {TargetId}", request.TargetType, request.TargetId);

        await Task.Delay(100);

        var comment = new CommentDto
        {
            Id = Guid.NewGuid(),
            Content = request.Content,
            ContentArabic = request.ContentArabic,
            TargetType = request.TargetType,
            TargetId = request.TargetId,
            ParentId = request.ParentId,
            AuthorId = Guid.NewGuid(),
            AuthorName = "Current User",
            LikeCount = 0,
            ReplyCount = 0,
            Mentions = request.Mentions?.Select(m => new MentionDto
            {
                UserId = m.UserId,
                UserName = "Mentioned User",
                StartIndex = m.StartIndex,
                EndIndex = m.EndIndex
            }).ToList() ?? new List<MentionDto>(),
            CreatedAt = DateTime.UtcNow
        };

        return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, ApiResponse<CommentDto>.Ok(comment));
    }

    /// <summary>
    /// Update a comment.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> UpdateComment(Guid id, [FromBody] UpdateCommentRequest request)
    {
        _logger.LogInformation("Updating comment {CommentId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Comment updated successfully"));
    }

    /// <summary>
    /// Delete a comment.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> DeleteComment(Guid id)
    {
        _logger.LogInformation("Deleting comment {CommentId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Comment deleted successfully"));
    }

    /// <summary>
    /// Like a comment.
    /// </summary>
    [HttpPost("{id:guid}/like")]
    public async Task<ActionResult<ApiResponse>> LikeComment(Guid id)
    {
        _logger.LogInformation("User liking comment {CommentId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Comment liked"));
    }

    /// <summary>
    /// Unlike a comment.
    /// </summary>
    [HttpDelete("{id:guid}/like")]
    public async Task<ActionResult<ApiResponse>> UnlikeComment(Guid id)
    {
        _logger.LogInformation("User unliking comment {CommentId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Comment unliked"));
    }

    /// <summary>
    /// Mark comment as accepted answer.
    /// </summary>
    [HttpPost("{id:guid}/accept-answer")]
    public async Task<ActionResult<ApiResponse>> AcceptAsAnswer(Guid id)
    {
        _logger.LogInformation("Marking comment {CommentId} as accepted answer", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Comment marked as accepted answer"));
    }

    /// <summary>
    /// Get comment replies.
    /// </summary>
    [HttpGet("{id:guid}/replies")]
    public ActionResult<ApiResponse<IReadOnlyList<CommentDto>>> GetReplies(Guid id)
    {
        var replies = new List<CommentDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Content = "Thank you for your feedback!",
                ParentId = id,
                AuthorId = Guid.NewGuid(),
                AuthorName = "Mohammed Al-Rashid",
                LikeCount = 2,
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            }
        };

        return Ok(ApiResponse<IReadOnlyList<CommentDto>>.Ok(replies));
    }
}

/// <summary>
/// Follow management controller.
/// </summary>
[ApiController]
[Route("api/collaboration/follows")]
[Authorize]
public class FollowsController : ControllerBase
{
    private readonly ILogger<FollowsController> _logger;

    public FollowsController(ILogger<FollowsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get items followed by current user.
    /// </summary>
    [HttpGet]
    public ActionResult<ApiResponse<IReadOnlyList<FollowDto>>> GetFollows([FromQuery] string? targetType)
    {
        var follows = new List<FollowDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                TargetType = "Community",
                TargetId = Guid.NewGuid(),
                TargetName = "Stadium Operations",
                NotificationsEnabled = true,
                CreatedAt = DateTime.UtcNow.AddMonths(-2)
            },
            new()
            {
                Id = Guid.NewGuid(),
                TargetType = "User",
                TargetId = Guid.NewGuid(),
                TargetName = "Mohammed Al-Rashid",
                NotificationsEnabled = true,
                CreatedAt = DateTime.UtcNow.AddMonths(-1)
            },
            new()
            {
                Id = Guid.NewGuid(),
                TargetType = "Discussion",
                TargetId = Guid.NewGuid(),
                TargetName = "Stadium Security Protocols Update",
                NotificationsEnabled = false,
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            }
        };

        if (!string.IsNullOrEmpty(targetType))
        {
            follows = follows.Where(f => f.TargetType.Equals(targetType, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        return Ok(ApiResponse<IReadOnlyList<FollowDto>>.Ok(follows));
    }

    /// <summary>
    /// Follow an item.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<FollowDto>>> Follow([FromBody] FollowRequest request)
    {
        _logger.LogInformation("User following {TargetType} {TargetId}", request.TargetType, request.TargetId);

        await Task.Delay(100);

        var follow = new FollowDto
        {
            Id = Guid.NewGuid(),
            TargetType = request.TargetType,
            TargetId = request.TargetId,
            TargetName = "Followed Item",
            NotificationsEnabled = request.NotificationsEnabled,
            CreatedAt = DateTime.UtcNow
        };

        return Ok(ApiResponse<FollowDto>.Ok(follow));
    }

    /// <summary>
    /// Unfollow an item.
    /// </summary>
    [HttpDelete]
    public async Task<ActionResult<ApiResponse>> Unfollow(
        [FromQuery] string targetType,
        [FromQuery] Guid targetId)
    {
        _logger.LogInformation("User unfollowing {TargetType} {TargetId}", targetType, targetId);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Unfollowed successfully"));
    }

    /// <summary>
    /// Update follow notifications setting.
    /// </summary>
    [HttpPut("{id:guid}/notifications")]
    public async Task<ActionResult<ApiResponse>> UpdateNotifications(Guid id, [FromBody] bool enabled)
    {
        _logger.LogInformation("Updating follow {FollowId} notifications: {Enabled}", id, enabled);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Notification settings updated"));
    }

    /// <summary>
    /// Get followers of an item.
    /// </summary>
    [HttpGet("followers")]
    public ActionResult<ApiResponse<IReadOnlyList<object>>> GetFollowers(
        [FromQuery] string targetType,
        [FromQuery] Guid targetId)
    {
        var followers = new List<object>
        {
            new { UserId = Guid.NewGuid(), UserName = "Sara Ali", AvatarUrl = "/avatars/sara.jpg" },
            new { UserId = Guid.NewGuid(), UserName = "Ahmed Hassan", AvatarUrl = (string?)null }
        };

        return Ok(ApiResponse<IReadOnlyList<object>>.Ok(followers));
    }
}
