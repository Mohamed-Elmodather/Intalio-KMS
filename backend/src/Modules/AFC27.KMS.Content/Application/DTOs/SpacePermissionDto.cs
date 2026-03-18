namespace AFC27.KMS.Content.Application.DTOs;

/// <summary>
/// Represents a single permission grant on a space.
/// </summary>
public record SpacePermissionDto
{
    public Guid Id { get; init; }
    public Guid SpaceId { get; init; }
    public Guid? UserId { get; init; }
    public Guid? GroupId { get; init; }
    public Guid? RoleId { get; init; }
    public string AccessLevel { get; init; } = string.Empty;
}

/// <summary>
/// Comprehensive view of a user's effective permissions on a space.
/// </summary>
public record SpaceUserPermissionsDto
{
    public Guid SpaceId { get; init; }
    public Guid UserId { get; init; }
    public bool HasAccess { get; init; }
    public string? EffectiveAccessLevel { get; init; }
    public bool IsOwner { get; init; }
    public string? MemberRole { get; init; }
    public string? Source { get; init; }
    public IReadOnlyList<SpacePermissionDto> ExplicitPermissions { get; init; } = Array.Empty<SpacePermissionDto>();
    public bool CanRead { get; init; }
    public bool CanWrite { get; init; }
    public bool CanManage { get; init; }
    public bool CanAdmin { get; init; }
}

/// <summary>
/// Request to grant a permission on a space.
/// At least one of UserId, GroupId, or RoleId must be specified.
/// </summary>
public record GrantSpacePermissionRequest
{
    public Guid SpaceId { get; init; }
    public Guid? UserId { get; init; }
    public Guid? GroupId { get; init; }
    public Guid? RoleId { get; init; }
    public string AccessLevel { get; init; } = "Read";
}
