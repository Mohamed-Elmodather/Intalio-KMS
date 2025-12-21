namespace AFC27.KMS.Identity.Application.DTOs;

/// <summary>
/// Department response DTO.
/// </summary>
public record DepartmentDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public int SortOrder { get; init; }
    public Guid? ParentId { get; init; }
    public string? ParentName { get; init; }
    public Guid? ManagerId { get; init; }
    public string? ManagerName { get; init; }
    public int MemberCount { get; init; }
    public IReadOnlyList<DepartmentSummaryDto> Children { get; init; } = Array.Empty<DepartmentSummaryDto>();
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Lightweight department summary.
/// </summary>
public record DepartmentSummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public int MemberCount { get; init; }
}

/// <summary>
/// Organization chart node.
/// </summary>
public record OrgChartNodeDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? AvatarUrl { get; init; }
    public string? JobTitle { get; init; }
    public string? DepartmentName { get; init; }
    public IReadOnlyList<OrgChartNodeDto> Children { get; init; } = Array.Empty<OrgChartNodeDto>();
}

/// <summary>
/// Create department request.
/// </summary>
public record CreateDepartmentRequest
{
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public Guid? ParentId { get; init; }
    public Guid? ManagerId { get; init; }
}

/// <summary>
/// Update department request.
/// </summary>
public record UpdateDepartmentRequest
{
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public Guid? ParentId { get; init; }
    public Guid? ManagerId { get; init; }
    public int SortOrder { get; init; }
}

/// <summary>
/// Group response DTO.
/// </summary>
public record GroupDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string Type { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public Guid OwnerId { get; init; }
    public string OwnerName { get; init; } = string.Empty;
    public int MemberCount { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Create group request.
/// </summary>
public record CreateGroupRequest
{
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string Type { get; init; } = "Team";
}

/// <summary>
/// Group member DTO.
/// </summary>
public record GroupMemberDto
{
    public Guid UserId { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public DateTime JoinedAt { get; init; }
}

/// <summary>
/// Add group member request.
/// </summary>
public record AddGroupMemberRequest
{
    public Guid UserId { get; init; }
    public string Role { get; init; } = "Member";
}
