using AFC27.KMS.Content.Application.DTOs;
using AFC27.KMS.Content.Domain.Entities;

namespace AFC27.KMS.Content.Application.Interfaces;

/// <summary>
/// Service interface for space-level permission management and access checks.
/// </summary>
public interface ISpacePermissionService
{
    /// <summary>
    /// Check if a user has the required access level on a space.
    /// </summary>
    Task<bool> HasAccessAsync(
        Guid spaceId, Guid userId, SpaceAccessLevel requiredLevel, CancellationToken ct = default);

    /// <summary>
    /// Get the effective permissions for a user on a specific space.
    /// </summary>
    Task<SpaceUserPermissionsDto> GetUserPermissionsAsync(
        Guid spaceId, Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Grant a permission on a space for a user, group, or role.
    /// </summary>
    Task<SpacePermissionDto?> GrantPermissionAsync(
        GrantSpacePermissionRequest request, Guid grantedByUserId, CancellationToken ct = default);

    /// <summary>
    /// Revoke a specific permission from a space.
    /// </summary>
    Task<bool> RevokePermissionAsync(
        Guid spaceId, Guid permissionId, Guid revokedByUserId, CancellationToken ct = default);

    /// <summary>
    /// Get all permissions configured on a space.
    /// </summary>
    Task<IReadOnlyList<SpacePermissionDto>> GetSpacePermissionsAsync(
        Guid spaceId, CancellationToken ct = default);

    /// <summary>
    /// Check if a user has access to content within a space.
    /// Content inherits space-level permissions unless explicitly overridden.
    /// </summary>
    Task<bool> HasContentAccessAsync(
        Guid spaceId, Guid userId, SpaceAccessLevel requiredLevel, CancellationToken ct = default);
}
