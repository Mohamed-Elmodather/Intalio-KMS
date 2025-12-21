using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Collaboration.Application.DTOs;
using AFC27.KMS.Contracts.Common;

namespace AFC27.KMS.Collaboration.Presentation.Controllers;

/// <summary>
/// Community management controller.
/// </summary>
[ApiController]
[Route("api/collaboration/communities")]
[Authorize]
public class CommunitiesController : ControllerBase
{
    private readonly ILogger<CommunitiesController> _logger;

    public CommunitiesController(ILogger<CommunitiesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of communities.
    /// </summary>
    [HttpGet]
    public ActionResult<ApiResponse<PagedResult<CommunitySummaryDto>>> GetCommunities([FromQuery] CommunityFilterRequest filter)
    {
        var communities = new List<CommunitySummaryDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Stadium Operations",
                NameArabic = "عمليات الملاعب",
                Description = "Coordination hub for all stadium operations teams",
                Slug = "stadium-operations",
                IconUrl = "/icons/stadium.png",
                Type = "WorkingGroup",
                Visibility = "Private",
                MemberCount = 156,
                DiscussionCount = 89,
                IsMember = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Volunteer Community",
                NameArabic = "مجتمع المتطوعين",
                Description = "Connect with fellow volunteers and stay updated",
                Slug = "volunteer-community",
                IconUrl = "/icons/volunteers.png",
                Type = "General",
                Visibility = "Public",
                MemberCount = 2500,
                DiscussionCount = 456,
                IsMember = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "IT & Technology",
                NameArabic = "تقنية المعلومات",
                Description = "Technical discussions and support",
                Slug = "it-technology",
                IconUrl = "/icons/tech.png",
                Type = "Department",
                Visibility = "Private",
                MemberCount = 85,
                DiscussionCount = 234,
                IsMember = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Media & Communications",
                NameArabic = "الإعلام والاتصالات",
                Description = "Media team collaboration space",
                Slug = "media-communications",
                Type = "Department",
                Visibility = "Private",
                MemberCount = 45,
                DiscussionCount = 123,
                IsMember = false
            }
        };

        var result = PagedResult<CommunitySummaryDto>.Create(communities, 25, filter.Page, filter.PageSize);
        return Ok(ApiResponse<PagedResult<CommunitySummaryDto>>.Ok(result));
    }

    /// <summary>
    /// Get communities the current user is a member of.
    /// </summary>
    [HttpGet("my-communities")]
    public ActionResult<ApiResponse<IReadOnlyList<CommunitySummaryDto>>> GetMyCommunities()
    {
        var communities = new List<CommunitySummaryDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Stadium Operations",
                NameArabic = "عمليات الملاعب",
                Slug = "stadium-operations",
                Type = "WorkingGroup",
                Visibility = "Private",
                MemberCount = 156,
                DiscussionCount = 89,
                IsMember = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "IT & Technology",
                NameArabic = "تقنية المعلومات",
                Slug = "it-technology",
                Type = "Department",
                Visibility = "Private",
                MemberCount = 85,
                DiscussionCount = 234,
                IsMember = true
            }
        };

        return Ok(ApiResponse<IReadOnlyList<CommunitySummaryDto>>.Ok(communities));
    }

    /// <summary>
    /// Get community by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public ActionResult<ApiResponse<CommunityDto>> GetCommunity(Guid id)
    {
        var community = new CommunityDto
        {
            Id = id,
            Name = "Stadium Operations",
            NameArabic = "عمليات الملاعب",
            Description = "Coordination hub for all stadium operations teams. Share updates, discuss challenges, and collaborate on solutions.",
            DescriptionArabic = "مركز التنسيق لجميع فرق عمليات الملاعب. شارك التحديثات وناقش التحديات وتعاون في الحلول.",
            Slug = "stadium-operations",
            CoverImageUrl = "/covers/stadium-ops.jpg",
            IconUrl = "/icons/stadium.png",
            Type = "WorkingGroup",
            Visibility = "Private",
            IsActive = true,
            AllowMemberPosts = true,
            RequirePostApproval = false,
            OwnerId = Guid.NewGuid(),
            OwnerName = "Mohammed Al-Rashid",
            MemberCount = 156,
            DiscussionCount = 89,
            IsMember = true,
            CurrentUserRole = "Member",
            CreatedAt = DateTime.UtcNow.AddMonths(-6)
        };

        return Ok(ApiResponse<CommunityDto>.Ok(community));
    }

    /// <summary>
    /// Get community by slug.
    /// </summary>
    [HttpGet("by-slug/{slug}")]
    public ActionResult<ApiResponse<CommunityDto>> GetCommunityBySlug(string slug)
    {
        return GetCommunity(Guid.NewGuid());
    }

    /// <summary>
    /// Create a new community.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CommunityDto>>> CreateCommunity([FromBody] CreateCommunityRequest request)
    {
        _logger.LogInformation("Creating community {CommunityName}", request.Name);

        await Task.Delay(100);

        var community = new CommunityDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            NameArabic = request.NameArabic,
            Description = request.Description,
            DescriptionArabic = request.DescriptionArabic,
            Slug = request.Name.ToLower().Replace(" ", "-"),
            Type = request.Type,
            Visibility = request.Visibility,
            IsActive = true,
            AllowMemberPosts = request.AllowMemberPosts,
            RequirePostApproval = request.RequirePostApproval,
            OwnerId = Guid.NewGuid(),
            OwnerName = "Current User",
            MemberCount = 1,
            DiscussionCount = 0,
            IsMember = true,
            CurrentUserRole = "Owner",
            CreatedAt = DateTime.UtcNow
        };

        return CreatedAtAction(nameof(GetCommunity), new { id = community.Id }, ApiResponse<CommunityDto>.Ok(community));
    }

    /// <summary>
    /// Update a community.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> UpdateCommunity(Guid id, [FromBody] UpdateCommunityRequest request)
    {
        _logger.LogInformation("Updating community {CommunityId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Community updated successfully"));
    }

    /// <summary>
    /// Delete a community.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> DeleteCommunity(Guid id)
    {
        _logger.LogInformation("Deleting community {CommunityId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Community deleted successfully"));
    }

    /// <summary>
    /// Join a community.
    /// </summary>
    [HttpPost("{id:guid}/join")]
    public async Task<ActionResult<ApiResponse>> JoinCommunity(Guid id)
    {
        _logger.LogInformation("User joining community {CommunityId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Successfully joined the community"));
    }

    /// <summary>
    /// Leave a community.
    /// </summary>
    [HttpPost("{id:guid}/leave")]
    public async Task<ActionResult<ApiResponse>> LeaveCommunity(Guid id)
    {
        _logger.LogInformation("User leaving community {CommunityId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Successfully left the community"));
    }

    /// <summary>
    /// Get community members.
    /// </summary>
    [HttpGet("{id:guid}/members")]
    public ActionResult<ApiResponse<PagedResult<CommunityMemberDto>>> GetMembers(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var members = new List<CommunityMemberDto>
        {
            new()
            {
                UserId = Guid.NewGuid(),
                UserName = "Mohammed Al-Rashid",
                UserAvatarUrl = "/avatars/mohammed.jpg",
                JobTitle = "Operations Director",
                Role = "Owner",
                JoinedAt = DateTime.UtcNow.AddMonths(-6)
            },
            new()
            {
                UserId = Guid.NewGuid(),
                UserName = "Sara Ali",
                UserAvatarUrl = "/avatars/sara.jpg",
                JobTitle = "Project Manager",
                Role = "Admin",
                JoinedAt = DateTime.UtcNow.AddMonths(-5)
            },
            new()
            {
                UserId = Guid.NewGuid(),
                UserName = "Ahmed Hassan",
                JobTitle = "Operations Coordinator",
                Role = "Moderator",
                JoinedAt = DateTime.UtcNow.AddMonths(-4)
            },
            new()
            {
                UserId = Guid.NewGuid(),
                UserName = "Fatima Al-Saud",
                JobTitle = "Team Lead",
                Role = "Member",
                JoinedAt = DateTime.UtcNow.AddMonths(-3)
            }
        };

        var result = PagedResult<CommunityMemberDto>.Create(members, 156, page, pageSize);
        return Ok(ApiResponse<PagedResult<CommunityMemberDto>>.Ok(result));
    }

    /// <summary>
    /// Add member to community.
    /// </summary>
    [HttpPost("{id:guid}/members")]
    public async Task<ActionResult<ApiResponse>> AddMember(Guid id, [FromBody] AddCommunityMemberRequest request)
    {
        _logger.LogInformation("Adding user {UserId} to community {CommunityId}", request.UserId, id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Member added successfully"));
    }

    /// <summary>
    /// Update member role.
    /// </summary>
    [HttpPut("{id:guid}/members/{userId:guid}")]
    public async Task<ActionResult<ApiResponse>> UpdateMemberRole(Guid id, Guid userId, [FromBody] string role)
    {
        _logger.LogInformation("Updating role of user {UserId} in community {CommunityId} to {Role}", userId, id, role);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Member role updated"));
    }

    /// <summary>
    /// Remove member from community.
    /// </summary>
    [HttpDelete("{id:guid}/members/{userId:guid}")]
    public async Task<ActionResult<ApiResponse>> RemoveMember(Guid id, Guid userId)
    {
        _logger.LogInformation("Removing user {UserId} from community {CommunityId}", userId, id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Member removed successfully"));
    }

    /// <summary>
    /// Get community discussions.
    /// </summary>
    [HttpGet("{id:guid}/discussions")]
    public ActionResult<ApiResponse<PagedResult<DiscussionSummaryDto>>> GetDiscussions(
        Guid id,
        [FromQuery] DiscussionFilterRequest filter)
    {
        var discussions = new List<DiscussionSummaryDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Stadium Security Protocols Update",
                TitleArabic = "تحديث بروتوكولات أمن الملاعب",
                ContentPreview = "We need to review and update our security protocols for the upcoming matches...",
                CommunityId = id,
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
            }
        };

        var result = PagedResult<DiscussionSummaryDto>.Create(discussions, 89, filter.Page, filter.PageSize);
        return Ok(ApiResponse<PagedResult<DiscussionSummaryDto>>.Ok(result));
    }
}
