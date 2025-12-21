namespace AFC27.KMS.Collaboration.Application.DTOs;

/// <summary>
/// Community response DTO.
/// </summary>
public record CommunityDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string Slug { get; init; } = string.Empty;
    public string? CoverImageUrl { get; init; }
    public string? IconUrl { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Visibility { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public bool AllowMemberPosts { get; init; }
    public bool RequirePostApproval { get; init; }
    public Guid OwnerId { get; init; }
    public string OwnerName { get; init; } = string.Empty;
    public int MemberCount { get; init; }
    public int DiscussionCount { get; init; }
    public bool IsMember { get; init; }
    public string? CurrentUserRole { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Community summary for lists.
/// </summary>
public record CommunitySummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string Slug { get; init; } = string.Empty;
    public string? IconUrl { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Visibility { get; init; } = string.Empty;
    public int MemberCount { get; init; }
    public int DiscussionCount { get; init; }
    public bool IsMember { get; init; }
}

/// <summary>
/// Create community request.
/// </summary>
public record CreateCommunityRequest
{
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string Type { get; init; } = "General";
    public string Visibility { get; init; } = "Public";
    public bool AllowMemberPosts { get; init; } = true;
    public bool RequirePostApproval { get; init; }
}

/// <summary>
/// Update community request.
/// </summary>
public record UpdateCommunityRequest
{
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string Visibility { get; init; } = "Public";
    public bool AllowMemberPosts { get; init; } = true;
    public bool RequirePostApproval { get; init; }
}

/// <summary>
/// Community member DTO.
/// </summary>
public record CommunityMemberDto
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string? UserAvatarUrl { get; init; }
    public string? JobTitle { get; init; }
    public string Role { get; init; } = string.Empty;
    public DateTime JoinedAt { get; init; }
}

/// <summary>
/// Add community member request.
/// </summary>
public record AddCommunityMemberRequest
{
    public Guid UserId { get; init; }
    public string Role { get; init; } = "Member";
}

/// <summary>
/// Community filter request.
/// </summary>
public record CommunityFilterRequest
{
    public string? Search { get; init; }
    public string? Type { get; init; }
    public string? Visibility { get; init; }
    public bool? IsMember { get; init; }
    public string SortBy { get; init; } = "Name";
    public bool SortDescending { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
