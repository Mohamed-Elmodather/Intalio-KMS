using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Documents.Application.Interfaces;
using AFC27.KMS.Documents.Domain.Entities;
using PermissionLevel = AFC27.KMS.Documents.Domain.Entities.PermissionLevel;

namespace AFC27.KMS.Documents.Application.Services;

/// <summary>
/// Service for checking and managing document permissions.
/// Implements inheritance from library -> folder -> document.
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly DbContext _dbContext;
    private readonly ILogger<PermissionService> _logger;

    public PermissionService(
        DbContext dbContext,
        ILogger<PermissionService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<bool> HasLibraryAccessAsync(
        Guid libraryId,
        Guid userId,
        PermissionLevel requiredLevel,
        CancellationToken cancellationToken = default)
    {
        var effectiveLevel = await GetEffectiveLibraryPermissionAsync(libraryId, userId, cancellationToken);
        return effectiveLevel.Includes(requiredLevel);
    }

    public async Task<bool> HasFolderAccessAsync(
        Guid folderId,
        Guid userId,
        PermissionLevel requiredLevel,
        CancellationToken cancellationToken = default)
    {
        var effectiveLevel = await GetEffectiveFolderPermissionAsync(folderId, userId, cancellationToken);
        return effectiveLevel.Includes(requiredLevel);
    }

    public async Task<bool> HasDocumentAccessAsync(
        Guid documentId,
        Guid userId,
        PermissionLevel requiredLevel,
        CancellationToken cancellationToken = default)
    {
        var effectiveLevel = await GetEffectiveDocumentPermissionAsync(documentId, userId, cancellationToken);
        return effectiveLevel.Includes(requiredLevel);
    }

    public async Task<PermissionLevel> GetEffectiveLibraryPermissionAsync(
        Guid libraryId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Get user's groups for group-based permissions
        var userGroups = await GetUserGroupsAsync(userId, cancellationToken);
        var userRoles = await GetUserRolesAsync(userId, cancellationToken);

        // Check if user is library owner (full control)
        if (await IsLibraryOwnerAsync(libraryId, userId, cancellationToken))
        {
            return PermissionLevel.FullControl;
        }

        // Check if library is public (read access for all)
        var library = await _dbContext.Set<DocumentLibrary>()
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == libraryId && !l.IsDeleted, cancellationToken);

        if (library == null)
            return PermissionLevel.None;

        if (library.IsPublic)
        {
            // Start with read access for public libraries
            var publicLevel = PermissionLevel.Read;

            // Check for additional explicit permissions
            var explicitLevel = await GetExplicitLibraryPermissionAsync(
                libraryId, userId, userGroups, userRoles, cancellationToken);

            return publicLevel.Combine(explicitLevel);
        }

        // Private library - check explicit permissions only
        return await GetExplicitLibraryPermissionAsync(
            libraryId, userId, userGroups, userRoles, cancellationToken);
    }

    public async Task<PermissionLevel> GetEffectiveFolderPermissionAsync(
        Guid folderId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var userGroups = await GetUserGroupsAsync(userId, cancellationToken);

        // Get folder with parent chain
        var folder = await _dbContext.Set<Folder>()
            .AsNoTracking()
            .Include(f => f.Library)
            .FirstOrDefaultAsync(f => f.Id == folderId && !f.IsDeleted, cancellationToken);

        if (folder == null)
            return PermissionLevel.None;

        // Get library-level permission as base
        var libraryPermission = await GetEffectiveLibraryPermissionAsync(
            folder.LibraryId, userId, cancellationToken);

        // Get folder-specific permissions
        var folderPermissions = await _dbContext.Set<FolderPermission>()
            .AsNoTracking()
            .Where(fp => fp.FolderId == folderId && !fp.IsDeleted)
            .ToListAsync(cancellationToken);

        // Check direct user permission
        var directPermission = folderPermissions
            .FirstOrDefault(fp => fp.UserId == userId);

        if (directPermission != null)
        {
            if (!directPermission.InheritFromParent)
            {
                // Override parent permissions
                return directPermission.Level;
            }
            // Combine with parent
            return libraryPermission.Combine(directPermission.Level);
        }

        // Check group permissions
        var groupPermission = folderPermissions
            .Where(fp => fp.GroupId.HasValue && userGroups.Contains(fp.GroupId.Value))
            .OrderByDescending(fp => fp.Level)
            .FirstOrDefault();

        if (groupPermission != null)
        {
            if (!groupPermission.InheritFromParent)
            {
                return groupPermission.Level;
            }
            return libraryPermission.Combine(groupPermission.Level);
        }

        // Inherit parent permissions if folder allows
        if (folder.ParentId.HasValue)
        {
            return await GetEffectiveFolderPermissionAsync(
                folder.ParentId.Value, userId, cancellationToken);
        }

        // Fall back to library permission
        return libraryPermission;
    }

    public async Task<PermissionLevel> GetEffectiveDocumentPermissionAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var userGroups = await GetUserGroupsAsync(userId, cancellationToken);

        // Get document
        var document = await _dbContext.Set<Document>()
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted, cancellationToken);

        if (document == null)
            return PermissionLevel.None;

        // Get base permission from folder or library
        PermissionLevel basePermission;
        if (document.FolderId.HasValue)
        {
            basePermission = await GetEffectiveFolderPermissionAsync(
                document.FolderId.Value, userId, cancellationToken);
        }
        else
        {
            basePermission = await GetEffectiveLibraryPermissionAsync(
                document.LibraryId, userId, cancellationToken);
        }

        // Check document-specific permissions
        var docPermissions = await _dbContext.Set<DocumentPermission>()
            .AsNoTracking()
            .Where(dp => dp.DocumentId == documentId && !dp.IsDeleted)
            .ToListAsync(cancellationToken);

        // Direct user permission takes precedence
        var directPermission = docPermissions
            .FirstOrDefault(dp => dp.UserId == userId);

        if (directPermission != null)
        {
            return directPermission.Level;
        }

        // Check group permissions
        var groupPermission = docPermissions
            .Where(dp => dp.GroupId.HasValue && userGroups.Contains(dp.GroupId.Value))
            .OrderByDescending(dp => dp.Level)
            .FirstOrDefault();

        if (groupPermission != null)
        {
            return groupPermission.Level;
        }

        // Fall back to base permission
        return basePermission;
    }

    public async Task<IReadOnlyList<Guid>> GetUserGroupsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Query group memberships from Identity module
        var groupIds = await _dbContext.Set<AFC27.KMS.Identity.Domain.Entities.GroupMember>()
            .AsNoTracking()
            .Where(gm => gm.UserId == userId)
            .Select(gm => gm.GroupId)
            .ToListAsync(cancellationToken);

        return groupIds;
    }

    public async Task<IReadOnlyList<Guid>> GetUserRolesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Query role assignments from Identity module
        var roleIds = await _dbContext.Set<AFC27.KMS.Identity.Domain.Entities.UserRole>()
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync(cancellationToken);

        return roleIds;
    }

    public async Task<bool> IsLibraryOwnerAsync(
        Guid libraryId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<DocumentLibrary>()
            .AsNoTracking()
            .AnyAsync(l => l.Id == libraryId && l.OwnerId == userId && !l.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<Guid>> GetAccessibleLibraryIdsAsync(
        Guid userId,
        PermissionLevel minimumLevel = PermissionLevel.Read,
        CancellationToken cancellationToken = default)
    {
        var userGroups = await GetUserGroupsAsync(userId, cancellationToken);
        var userRoles = await GetUserRolesAsync(userId, cancellationToken);

        // Get all libraries
        var libraries = await _dbContext.Set<DocumentLibrary>()
            .AsNoTracking()
            .Where(l => !l.IsDeleted)
            .Select(l => new { l.Id, l.OwnerId, l.IsPublic })
            .ToListAsync(cancellationToken);

        // Get all library permissions for user and their groups
        var libraryPermissions = await _dbContext.Set<LibraryPermission>()
            .AsNoTracking()
            .Where(lp =>
                lp.UserId == userId ||
                (lp.GroupId.HasValue && userGroups.Contains(lp.GroupId.Value)) ||
                (lp.RoleId.HasValue && userRoles.Contains(lp.RoleId.Value)))
            .ToListAsync(cancellationToken);

        var accessibleIds = new List<Guid>();

        foreach (var library in libraries)
        {
            // Owner has full access
            if (library.OwnerId == userId)
            {
                accessibleIds.Add(library.Id);
                continue;
            }

            // Public libraries have read access
            if (library.IsPublic && minimumLevel == PermissionLevel.Read)
            {
                accessibleIds.Add(library.Id);
                continue;
            }

            // Check explicit permissions
            var permission = libraryPermissions
                .Where(lp => lp.LibraryId == library.Id)
                .OrderByDescending(lp => lp.AccessLevel)
                .FirstOrDefault();

            if (permission != null)
            {
                var level = permission.AccessLevel.ToPermissionLevel();
                if (level.Includes(minimumLevel))
                {
                    accessibleIds.Add(library.Id);
                }
            }
        }

        return accessibleIds;
    }

    public async Task<IReadOnlyList<Guid>> GetAccessibleFolderIdsAsync(
        Guid libraryId,
        Guid userId,
        PermissionLevel minimumLevel = PermissionLevel.Read,
        CancellationToken cancellationToken = default)
    {
        // Check library access first
        var libraryAccess = await HasLibraryAccessAsync(libraryId, userId, minimumLevel, cancellationToken);
        if (!libraryAccess)
        {
            return Array.Empty<Guid>();
        }

        // Get all folders in library
        var folders = await _dbContext.Set<Folder>()
            .AsNoTracking()
            .Where(f => f.LibraryId == libraryId && !f.IsDeleted)
            .Select(f => f.Id)
            .ToListAsync(cancellationToken);

        // If user has library-level access, they can access all folders by default
        // unless there are explicit restrictions (not implemented in this version)
        return folders;
    }

    private async Task<PermissionLevel> GetExplicitLibraryPermissionAsync(
        Guid libraryId,
        Guid userId,
        IReadOnlyList<Guid> userGroups,
        IReadOnlyList<Guid> userRoles,
        CancellationToken cancellationToken)
    {
        var permissions = await _dbContext.Set<LibraryPermission>()
            .AsNoTracking()
            .Where(lp => lp.LibraryId == libraryId)
            .ToListAsync(cancellationToken);

        // Check direct user permission first
        var userPermission = permissions.FirstOrDefault(p => p.UserId == userId);
        if (userPermission != null)
        {
            return userPermission.AccessLevel.ToPermissionLevel();
        }

        // Check role permissions
        var rolePermission = permissions
            .Where(p => p.RoleId.HasValue && userRoles.Contains(p.RoleId.Value))
            .OrderByDescending(p => p.AccessLevel)
            .FirstOrDefault();

        if (rolePermission != null)
        {
            return rolePermission.AccessLevel.ToPermissionLevel();
        }

        // Check group permissions
        var groupPermission = permissions
            .Where(p => p.GroupId.HasValue && userGroups.Contains(p.GroupId.Value))
            .OrderByDescending(p => p.AccessLevel)
            .FirstOrDefault();

        if (groupPermission != null)
        {
            return groupPermission.AccessLevel.ToPermissionLevel();
        }

        return PermissionLevel.None;
    }
}
