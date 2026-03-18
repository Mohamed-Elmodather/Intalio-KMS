using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Content.Application.DTOs;
using AFC27.KMS.Content.Application.Interfaces;
using AFC27.KMS.Content.Domain.Entities;

namespace AFC27.KMS.Content.Application.Services;

/// <summary>
/// Service for managing and checking space-level permissions.
/// Implements permission inheritance: Space permissions cascade to all content within
/// unless explicitly overridden.
/// </summary>
public class SpacePermissionService : ISpacePermissionService
{
    private readonly DbContext _dbContext;
    private readonly ILogger<SpacePermissionService> _logger;

    public SpacePermissionService(DbContext dbContext, ILogger<SpacePermissionService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    // ========================================
    // Access Checks
    // ========================================

    /// <summary>
    /// Check if a user has the required access level on a space.
    /// Evaluates: owner status, membership role, explicit permissions (user/group/role), and public visibility.
    /// </summary>
    public async Task<bool> HasAccessAsync(
        Guid spaceId, Guid userId, SpaceAccessLevel requiredLevel, CancellationToken ct = default)
    {
        var space = await _dbContext.Set<Space>()
            .AsNoTracking()
            .Include(s => s.Members)
            .Include(s => s.Permissions)
            .FirstOrDefaultAsync(s => s.Id == spaceId && !s.IsDeleted, ct);

        if (space == null)
            return false;

        // Owner always has full access
        if (space.OwnerId == userId)
            return true;

        // Check membership-based access
        var member = space.Members.FirstOrDefault(m => m.UserId == userId);
        if (member != null)
        {
            var memberAccessLevel = MapMemberRoleToAccessLevel(member.Role);
            if (memberAccessLevel >= requiredLevel)
                return true;
        }

        // Check explicit permission grants (user-level)
        var userPermission = space.Permissions
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.AccessLevel)
            .FirstOrDefault();

        if (userPermission != null && userPermission.AccessLevel >= requiredLevel)
            return true;

        // Check public space read access
        if (space.IsPublic && requiredLevel == SpaceAccessLevel.Read)
            return true;

        // Check parent space permission inheritance
        if (space.ParentSpaceId.HasValue)
        {
            var hasParentAccess = await HasAccessAsync(space.ParentSpaceId.Value, userId, requiredLevel, ct);
            if (hasParentAccess)
                return true;
        }

        _logger.LogDebug(
            "Access denied: User {UserId} lacks {RequiredLevel} access on Space {SpaceId}",
            userId, requiredLevel, spaceId);

        return false;
    }

    /// <summary>
    /// Get the effective permissions for a user on a specific space.
    /// Returns all permission sources: membership, explicit grants, inheritance, and public access.
    /// </summary>
    public async Task<SpaceUserPermissionsDto> GetUserPermissionsAsync(
        Guid spaceId, Guid userId, CancellationToken ct = default)
    {
        var space = await _dbContext.Set<Space>()
            .AsNoTracking()
            .Include(s => s.Members)
            .Include(s => s.Permissions)
            .FirstOrDefaultAsync(s => s.Id == spaceId && !s.IsDeleted, ct);

        if (space == null)
        {
            return new SpaceUserPermissionsDto
            {
                SpaceId = spaceId,
                UserId = userId,
                HasAccess = false
            };
        }

        var isOwner = space.OwnerId == userId;
        var member = space.Members.FirstOrDefault(m => m.UserId == userId);
        var explicitPermissions = space.Permissions
            .Where(p => p.UserId == userId)
            .Select(p => new SpacePermissionDto
            {
                Id = p.Id,
                SpaceId = p.SpaceId,
                UserId = p.UserId,
                GroupId = p.GroupId,
                RoleId = p.RoleId,
                AccessLevel = p.AccessLevel.ToString()
            })
            .ToList();

        // Determine effective access level
        var effectiveLevel = SpaceAccessLevel.Read; // default for public
        var hasAccess = false;
        var source = "None";

        if (isOwner)
        {
            effectiveLevel = SpaceAccessLevel.Admin;
            hasAccess = true;
            source = "Owner";
        }
        else if (member != null)
        {
            effectiveLevel = MapMemberRoleToAccessLevel(member.Role);
            hasAccess = true;
            source = $"Member ({member.Role})";
        }
        else if (explicitPermissions.Any())
        {
            var maxLevel = space.Permissions
                .Where(p => p.UserId == userId)
                .Max(p => p.AccessLevel);
            effectiveLevel = maxLevel;
            hasAccess = true;
            source = "ExplicitGrant";
        }
        else if (space.IsPublic)
        {
            effectiveLevel = SpaceAccessLevel.Read;
            hasAccess = true;
            source = "PublicSpace";
        }

        return new SpaceUserPermissionsDto
        {
            SpaceId = spaceId,
            UserId = userId,
            HasAccess = hasAccess,
            EffectiveAccessLevel = effectiveLevel.ToString(),
            IsOwner = isOwner,
            MemberRole = member?.Role.ToString(),
            Source = source,
            ExplicitPermissions = explicitPermissions,
            CanRead = hasAccess,
            CanWrite = hasAccess && effectiveLevel >= SpaceAccessLevel.Write,
            CanManage = hasAccess && effectiveLevel >= SpaceAccessLevel.Manage,
            CanAdmin = hasAccess && effectiveLevel >= SpaceAccessLevel.Admin
        };
    }

    // ========================================
    // Permission Management
    // ========================================

    /// <summary>
    /// Grant a permission on a space for a user, group, or role.
    /// Only space owners and admins can grant permissions.
    /// </summary>
    public async Task<SpacePermissionDto?> GrantPermissionAsync(
        GrantSpacePermissionRequest request, Guid grantedByUserId, CancellationToken ct = default)
    {
        var space = await _dbContext.Set<Space>()
            .Include(s => s.Members)
            .Include(s => s.Permissions)
            .FirstOrDefaultAsync(s => s.Id == request.SpaceId && !s.IsDeleted, ct);

        if (space == null)
            return null;

        // Verify the granting user has admin-level access
        if (!await HasAccessAsync(request.SpaceId, grantedByUserId, SpaceAccessLevel.Admin, ct))
        {
            _logger.LogWarning(
                "User {UserId} attempted to grant permission on Space {SpaceId} without admin access",
                grantedByUserId, request.SpaceId);
            return null;
        }

        if (!Enum.TryParse<SpaceAccessLevel>(request.AccessLevel, true, out var accessLevel))
            return null;

        // Check for duplicate
        var existing = space.Permissions.FirstOrDefault(p =>
            p.UserId == request.UserId &&
            p.GroupId == request.GroupId &&
            p.RoleId == request.RoleId);

        if (existing != null)
        {
            // Remove old permission so we can replace it
            _dbContext.Set<SpacePermission>().Remove(existing);
        }

        var permission = SpacePermission.Create(
            request.SpaceId,
            accessLevel,
            request.UserId,
            request.GroupId,
            request.RoleId);

        space.Permissions.Add(permission);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Permission {AccessLevel} granted on Space {SpaceId} for User={UserId}, Group={GroupId}, Role={RoleId} by {GrantedBy}",
            accessLevel, request.SpaceId, request.UserId, request.GroupId, request.RoleId, grantedByUserId);

        return new SpacePermissionDto
        {
            Id = permission.Id,
            SpaceId = permission.SpaceId,
            UserId = permission.UserId,
            GroupId = permission.GroupId,
            RoleId = permission.RoleId,
            AccessLevel = permission.AccessLevel.ToString()
        };
    }

    /// <summary>
    /// Revoke a specific permission from a space.
    /// Only space owners and admins can revoke permissions.
    /// </summary>
    public async Task<bool> RevokePermissionAsync(
        Guid spaceId, Guid permissionId, Guid revokedByUserId, CancellationToken ct = default)
    {
        // Verify the revoking user has admin-level access
        if (!await HasAccessAsync(spaceId, revokedByUserId, SpaceAccessLevel.Admin, ct))
        {
            _logger.LogWarning(
                "User {UserId} attempted to revoke permission on Space {SpaceId} without admin access",
                revokedByUserId, spaceId);
            return false;
        }

        var permission = await _dbContext.Set<SpacePermission>()
            .FirstOrDefaultAsync(p => p.Id == permissionId && p.SpaceId == spaceId, ct);

        if (permission == null)
            return false;

        _dbContext.Set<SpacePermission>().Remove(permission);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Permission {PermissionId} revoked from Space {SpaceId} by {RevokedBy}",
            permissionId, spaceId, revokedByUserId);

        return true;
    }

    /// <summary>
    /// Get all permissions configured on a space.
    /// </summary>
    public async Task<IReadOnlyList<SpacePermissionDto>> GetSpacePermissionsAsync(
        Guid spaceId, CancellationToken ct = default)
    {
        var permissions = await _dbContext.Set<SpacePermission>()
            .AsNoTracking()
            .Where(p => p.SpaceId == spaceId)
            .Select(p => new SpacePermissionDto
            {
                Id = p.Id,
                SpaceId = p.SpaceId,
                UserId = p.UserId,
                GroupId = p.GroupId,
                RoleId = p.RoleId,
                AccessLevel = p.AccessLevel.ToString()
            })
            .ToListAsync(ct);

        return permissions;
    }

    /// <summary>
    /// Check if a user has access to content within a space (for use by Articles, Documents, etc.).
    /// This is the main entry point for content-level permission checks.
    /// </summary>
    public async Task<bool> HasContentAccessAsync(
        Guid spaceId, Guid userId, SpaceAccessLevel requiredLevel, CancellationToken ct = default)
    {
        // Content inherits space-level permissions
        return await HasAccessAsync(spaceId, userId, requiredLevel, ct);
    }

    // ========================================
    // Helpers
    // ========================================

    private static SpaceAccessLevel MapMemberRoleToAccessLevel(SpaceMemberRole role)
    {
        return role switch
        {
            SpaceMemberRole.Owner => SpaceAccessLevel.Admin,
            SpaceMemberRole.Admin => SpaceAccessLevel.Admin,
            SpaceMemberRole.Editor => SpaceAccessLevel.Write,
            SpaceMemberRole.Member => SpaceAccessLevel.Read,
            _ => SpaceAccessLevel.Read
        };
    }
}
