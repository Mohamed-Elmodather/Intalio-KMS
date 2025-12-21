using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Collaboration.Application.DTOs;
using AFC27.KMS.Contracts.Common;

namespace AFC27.KMS.Collaboration.Presentation.Controllers;

/// <summary>
/// Discussion and forum management controller.
/// </summary>
[ApiController]
[Route("api/collaboration/discussions")]
[Authorize]
public class DiscussionsController : ControllerBase
{
    private readonly ILogger<DiscussionsController> _logger;

    public DiscussionsController(ILogger<DiscussionsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of discussions.
    /// </summary>
    [HttpGet]
    public ActionResult<ApiResponse<PagedResult<DiscussionSummaryDto>>> GetDiscussions([FromQuery] DiscussionFilterRequest filter)
    {
        var discussions = new List<DiscussionSummaryDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Stadium Security Protocols Update",
                TitleArabic = "تحديث بروتوكولات أمن الملاعب",
                ContentPreview = "We need to review and update our security protocols for the upcoming matches...",
                CommunityId = Guid.NewGuid(),
                CommunityName = "Stadium Operations",
                AuthorId = Guid.NewGuid(),
                AuthorName = "Mohammed Al-Rashid",
                AuthorAvatarUrl = "/avatars/mohammed.jpg",
                Type = "Discussion",
                Status = "Active",
                IsPinned = true,
                ViewCount = 234,
                ReplyCount = 18,
                LikeCount = 45,
                LastActivityAt = DateTime.UtcNow.AddHours(-2),
                LastReplyByName = "Sara Ali",
                Tags = new[] { "Security", "Protocols" },
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "How to handle VIP arrivals?",
                TitleArabic = "كيفية التعامل مع وصول كبار الشخصيات؟",
                ContentPreview = "I have a question about the proper procedure for VIP arrivals at venue entrances...",
                CommunityId = Guid.NewGuid(),
                CommunityName = "Stadium Operations",
                AuthorId = Guid.NewGuid(),
                AuthorName = "Fatima Al-Saud",
                Type = "Question",
                Status = "Answered",
                ViewCount = 156,
                ReplyCount = 8,
                LikeCount = 12,
                LastActivityAt = DateTime.UtcNow.AddHours(-5),
                LastReplyByName = "Ahmed Hassan",
                Tags = new[] { "VIP", "Procedures" },
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "New Volunteer Training Schedule",
                TitleArabic = "جدول تدريب المتطوعين الجديد",
                ContentPreview = "Please note the updated training schedule for all new volunteers joining in January...",
                CommunityId = Guid.NewGuid(),
                CommunityName = "Volunteer Community",
                AuthorId = Guid.NewGuid(),
                AuthorName = "Sara Ali",
                Type = "Announcement",
                Status = "Active",
                IsPinned = true,
                ViewCount = 890,
                ReplyCount = 45,
                LikeCount = 234,
                LastActivityAt = DateTime.UtcNow.AddHours(-8),
                Tags = new[] { "Training", "Schedule" },
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Which stadium has the best facilities?",
                TitleArabic = "أي ملعب لديه أفضل المرافق؟",
                CommunityId = Guid.NewGuid(),
                CommunityName = "Volunteer Community",
                AuthorId = Guid.NewGuid(),
                AuthorName = "Ahmed Hassan",
                Type = "Poll",
                Status = "Active",
                ViewCount = 567,
                ReplyCount = 12,
                LikeCount = 89,
                LastActivityAt = DateTime.UtcNow.AddHours(-12),
                Tags = new[] { "Stadiums", "Poll" },
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        var result = PagedResult<DiscussionSummaryDto>.Create(discussions, 450, filter.Page, filter.PageSize);
        return Ok(ApiResponse<PagedResult<DiscussionSummaryDto>>.Ok(result));
    }

    /// <summary>
    /// Get trending discussions.
    /// </summary>
    [HttpGet("trending")]
    public ActionResult<ApiResponse<IReadOnlyList<DiscussionSummaryDto>>> GetTrendingDiscussions([FromQuery] int limit = 5)
    {
        var discussions = new List<DiscussionSummaryDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "New Volunteer Training Schedule",
                CommunityName = "Volunteer Community",
                AuthorName = "Sara Ali",
                Type = "Announcement",
                ViewCount = 890,
                ReplyCount = 45,
                LikeCount = 234,
                LastActivityAt = DateTime.UtcNow.AddHours(-8),
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        return Ok(ApiResponse<IReadOnlyList<DiscussionSummaryDto>>.Ok(discussions));
    }

    /// <summary>
    /// Get unanswered questions.
    /// </summary>
    [HttpGet("unanswered")]
    public ActionResult<ApiResponse<PagedResult<DiscussionSummaryDto>>> GetUnansweredQuestions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var discussions = new List<DiscussionSummaryDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Issue with badge printing system",
                CommunityName = "IT & Technology",
                AuthorName = "Technical Support",
                Type = "Question",
                Status = "Active",
                ViewCount = 45,
                ReplyCount = 2,
                LastActivityAt = DateTime.UtcNow.AddHours(-4),
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        var result = PagedResult<DiscussionSummaryDto>.Create(discussions, 12, page, pageSize);
        return Ok(ApiResponse<PagedResult<DiscussionSummaryDto>>.Ok(result));
    }

    /// <summary>
    /// Get discussion by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public ActionResult<ApiResponse<DiscussionDto>> GetDiscussion(Guid id)
    {
        var discussion = new DiscussionDto
        {
            Id = id,
            Title = "Stadium Security Protocols Update",
            TitleArabic = "تحديث بروتوكولات أمن الملاعب",
            Content = @"<h2>Security Protocol Review</h2>
<p>Team, we need to review and update our security protocols for the upcoming matches. Key areas to address:</p>
<ul>
<li>Entry point screening procedures</li>
<li>VIP area access control</li>
<li>Emergency evacuation routes</li>
<li>Communication protocols during incidents</li>
</ul>
<p>Please share your feedback and suggestions below. @Sara.Ali can you coordinate with the security team?</p>",
            ContentArabic = "<h2>مراجعة بروتوكولات الأمن</h2><p>فريق العمل، نحتاج إلى مراجعة وتحديث بروتوكولات الأمن الخاصة بنا...</p>",
            CommunityId = Guid.NewGuid(),
            CommunityName = "Stadium Operations",
            AuthorId = Guid.NewGuid(),
            AuthorName = "Mohammed Al-Rashid",
            AuthorAvatarUrl = "/avatars/mohammed.jpg",
            AuthorJobTitle = "Operations Director",
            Type = "Discussion",
            Status = "Active",
            IsPinned = true,
            IsLocked = false,
            IsAnnouncement = false,
            ViewCount = 234,
            ReplyCount = 18,
            LikeCount = 45,
            IsLikedByCurrentUser = true,
            IsFollowedByCurrentUser = true,
            LastActivityAt = DateTime.UtcNow.AddHours(-2),
            LastReplyByName = "Sara Ali",
            Tags = new[] { "Security", "Protocols", "Urgent" },
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            UpdatedAt = DateTime.UtcNow.AddDays(-2)
        };

        return Ok(ApiResponse<DiscussionDto>.Ok(discussion));
    }

    /// <summary>
    /// Create a new discussion.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<DiscussionDto>>> CreateDiscussion([FromBody] CreateDiscussionRequest request)
    {
        _logger.LogInformation("Creating discussion {Title} in community {CommunityId}", request.Title, request.CommunityId);

        await Task.Delay(100);

        var discussion = new DiscussionDto
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            TitleArabic = request.TitleArabic,
            Content = request.Content,
            ContentArabic = request.ContentArabic,
            CommunityId = request.CommunityId,
            CommunityName = "Stadium Operations",
            AuthorId = Guid.NewGuid(),
            AuthorName = "Current User",
            Type = request.Type,
            Status = "Active",
            Tags = request.Tags?.ToArray() ?? Array.Empty<string>(),
            CreatedAt = DateTime.UtcNow
        };

        return CreatedAtAction(nameof(GetDiscussion), new { id = discussion.Id }, ApiResponse<DiscussionDto>.Ok(discussion));
    }

    /// <summary>
    /// Update a discussion.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> UpdateDiscussion(Guid id, [FromBody] UpdateDiscussionRequest request)
    {
        _logger.LogInformation("Updating discussion {DiscussionId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Discussion updated successfully"));
    }

    /// <summary>
    /// Delete a discussion.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> DeleteDiscussion(Guid id)
    {
        _logger.LogInformation("Deleting discussion {DiscussionId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Discussion deleted successfully"));
    }

    /// <summary>
    /// Like a discussion.
    /// </summary>
    [HttpPost("{id:guid}/like")]
    public async Task<ActionResult<ApiResponse>> LikeDiscussion(Guid id)
    {
        _logger.LogInformation("User liking discussion {DiscussionId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Discussion liked"));
    }

    /// <summary>
    /// Unlike a discussion.
    /// </summary>
    [HttpDelete("{id:guid}/like")]
    public async Task<ActionResult<ApiResponse>> UnlikeDiscussion(Guid id)
    {
        _logger.LogInformation("User unliking discussion {DiscussionId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Discussion unliked"));
    }

    /// <summary>
    /// Follow a discussion.
    /// </summary>
    [HttpPost("{id:guid}/follow")]
    public async Task<ActionResult<ApiResponse>> FollowDiscussion(Guid id)
    {
        _logger.LogInformation("User following discussion {DiscussionId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Now following discussion"));
    }

    /// <summary>
    /// Unfollow a discussion.
    /// </summary>
    [HttpDelete("{id:guid}/follow")]
    public async Task<ActionResult<ApiResponse>> UnfollowDiscussion(Guid id)
    {
        _logger.LogInformation("User unfollowing discussion {DiscussionId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Unfollowed discussion"));
    }

    /// <summary>
    /// Pin/unpin a discussion.
    /// </summary>
    [HttpPost("{id:guid}/pin")]
    public async Task<ActionResult<ApiResponse>> TogglePin(Guid id, [FromBody] bool isPinned)
    {
        _logger.LogInformation("Setting discussion {DiscussionId} pinned: {IsPinned}", id, isPinned);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok(isPinned ? "Discussion pinned" : "Discussion unpinned"));
    }

    /// <summary>
    /// Lock/unlock a discussion.
    /// </summary>
    [HttpPost("{id:guid}/lock")]
    public async Task<ActionResult<ApiResponse>> ToggleLock(Guid id, [FromBody] bool isLocked)
    {
        _logger.LogInformation("Setting discussion {DiscussionId} locked: {IsLocked}", id, isLocked);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok(isLocked ? "Discussion locked" : "Discussion unlocked"));
    }

    /// <summary>
    /// Mark question as answered.
    /// </summary>
    [HttpPost("{id:guid}/mark-answered")]
    public async Task<ActionResult<ApiResponse>> MarkAsAnswered(Guid id, [FromBody] Guid answerId)
    {
        _logger.LogInformation("Marking discussion {DiscussionId} as answered with comment {AnswerId}", id, answerId);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Question marked as answered"));
    }

    /// <summary>
    /// Get discussion comments.
    /// </summary>
    [HttpGet("{id:guid}/comments")]
    public ActionResult<ApiResponse<IReadOnlyList<CommentDto>>> GetComments(Guid id)
    {
        var comments = new List<CommentDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Content = "Great initiative! I'll coordinate with the security team right away. We should also consider adding metal detector protocols at all entry points.",
                TargetType = "Discussion",
                TargetId = id,
                AuthorId = Guid.NewGuid(),
                AuthorName = "Sara Ali",
                AuthorAvatarUrl = "/avatars/sara.jpg",
                AuthorJobTitle = "Project Manager",
                LikeCount = 12,
                ReplyCount = 3,
                IsLikedByCurrentUser = true,
                Mentions = new List<MentionDto>(),
                Replies = new List<CommentDto>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Content = "Agreed. Let's schedule a meeting this week to discuss the details.",
                        AuthorId = Guid.NewGuid(),
                        AuthorName = "Mohammed Al-Rashid",
                        AuthorJobTitle = "Operations Director",
                        LikeCount = 5,
                        CreatedAt = DateTime.UtcNow.AddHours(-4)
                    }
                },
                CreatedAt = DateTime.UtcNow.AddHours(-6)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Content = "We should also update the emergency evacuation signage. I noticed some signs are outdated.",
                TargetType = "Discussion",
                TargetId = id,
                AuthorId = Guid.NewGuid(),
                AuthorName = "Ahmed Hassan",
                AuthorJobTitle = "Operations Coordinator",
                LikeCount = 8,
                ReplyCount = 1,
                CreatedAt = DateTime.UtcNow.AddHours(-4)
            }
        };

        return Ok(ApiResponse<IReadOnlyList<CommentDto>>.Ok(comments));
    }
}
