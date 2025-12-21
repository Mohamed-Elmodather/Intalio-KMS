namespace AFC27.KMS.Identity.Application.DTOs;

/// <summary>
/// Role response DTO.
/// </summary>
public record RoleDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public bool IsSystemRole { get; init; }
    public int UserCount { get; init; }
    public IReadOnlyList<PermissionDto> Permissions { get; init; } = Array.Empty<PermissionDto>();
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Permission response DTO.
/// </summary>
public record PermissionDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Resource { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public string? Description { get; init; }
}

/// <summary>
/// Create role request.
/// </summary>
public record CreateRoleRequest
{
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public IReadOnlyList<Guid>? PermissionIds { get; init; }
}

/// <summary>
/// Update role request.
/// </summary>
public record UpdateRoleRequest
{
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
}

/// <summary>
/// Assign role to user request.
/// </summary>
public record AssignRoleRequest
{
    public Guid UserId { get; init; }
    public Guid RoleId { get; init; }
}

/// <summary>
/// Update role permissions request.
/// </summary>
public record UpdateRolePermissionsRequest
{
    public IReadOnlyList<Guid> PermissionIds { get; init; } = Array.Empty<Guid>();
}
