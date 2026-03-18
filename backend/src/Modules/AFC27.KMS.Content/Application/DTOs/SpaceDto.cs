namespace AFC27.KMS.Content.Application.DTOs;

/// <summary>
/// Full space response DTO.
/// </summary>
public record SpaceDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string SpaceType { get; init; } = string.Empty;
    public Guid OwnerId { get; init; }
    public string OwnerName { get; init; } = string.Empty;
    public string? IconName { get; init; }
    public string? Color { get; init; }
    public bool IsPublic { get; init; }
    public bool IsArchived { get; init; }
    public Guid? ParentSpaceId { get; init; }
    public string? ParentSpaceName { get; init; }
    public int MemberCount { get; init; }
    public int ContentCount { get; init; }
    public int ChildSpaceCount { get; init; }
    public IReadOnlyList<SpaceMemberDto> Members { get; init; } = Array.Empty<SpaceMemberDto>();
    public IReadOnlyList<SpaceSummaryDto> ChildSpaces { get; init; } = Array.Empty<SpaceSummaryDto>();
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// Space summary for lists and navigation.
/// </summary>
public record SpaceSummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string SpaceType { get; init; } = string.Empty;
    public string OwnerName { get; init; } = string.Empty;
    public string? IconName { get; init; }
    public string? Color { get; init; }
    public bool IsPublic { get; init; }
    public bool IsArchived { get; init; }
    public int MemberCount { get; init; }
    public int ContentCount { get; init; }
    public int ChildSpaceCount { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Request to create a new space.
/// </summary>
public record CreateSpaceRequest
{
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string SpaceType { get; init; } = "Team";
    public bool IsPublic { get; init; }
    public string? IconName { get; init; }
    public string? Color { get; init; }
    public Guid? ParentSpaceId { get; init; }
}

/// <summary>
/// Request to update an existing space.
/// </summary>
public record UpdateSpaceRequest
{
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string? IconName { get; init; }
    public string? Color { get; init; }
    public bool IsPublic { get; init; }
}

/// <summary>
/// Request to add a member to a space.
/// </summary>
public record AddSpaceMemberRequest
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Role { get; init; } = "Member";
}

/// <summary>
/// Request to change a member's role.
/// </summary>
public record ChangeSpaceMemberRoleRequest
{
    public string Role { get; init; } = "Member";
}

/// <summary>
/// Space member response DTO.
/// </summary>
public record SpaceMemberDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public DateTime JoinedAt { get; init; }
}

/// <summary>
/// Filter/query parameters for listing spaces.
/// </summary>
public record SpaceFilterRequest
{
    public string? Search { get; init; }
    public string? SpaceType { get; init; }
    public bool? IsPublic { get; init; }
    public bool? IsArchived { get; init; }
    public Guid? ParentSpaceId { get; init; }
    public Guid? OwnerId { get; init; }
    public string SortBy { get; init; } = "CreatedAt";
    public bool SortDescending { get; init; } = true;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
