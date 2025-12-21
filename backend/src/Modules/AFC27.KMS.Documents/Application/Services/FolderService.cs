using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Documents.Application.DTOs;
using AFC27.KMS.Documents.Application.Interfaces;
using AFC27.KMS.Documents.Domain.Entities;
using AFC27.KMS.SharedKernel.Domain;
using PermissionLevel = AFC27.KMS.Documents.Domain.Entities.PermissionLevel;

namespace AFC27.KMS.Documents.Application.Services;

/// <summary>
/// Service for managing folders within document libraries.
/// </summary>
public class FolderService : IFolderService
{
    private readonly DbContext _dbContext;
    private readonly IPermissionService _permissionService;
    private readonly ILogger<FolderService> _logger;

    public FolderService(
        DbContext dbContext,
        IPermissionService permissionService,
        ILogger<FolderService> logger)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
        _logger = logger;
    }

    public async Task<FolderDto?> GetFolderAsync(
        Guid folderId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Check access
        var hasAccess = await _permissionService.HasFolderAccessAsync(
            folderId, userId, PermissionLevel.Read, cancellationToken);

        if (!hasAccess)
        {
            _logger.LogWarning("User {UserId} denied access to folder {FolderId}", userId, folderId);
            return null;
        }

        var folder = await _dbContext.Set<Folder>()
            .AsNoTracking()
            .Include(f => f.Children.Where(c => !c.IsDeleted))
            .Where(f => f.Id == folderId && !f.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (folder == null)
            return null;

        var documentCount = await _dbContext.Set<Document>()
            .AsNoTracking()
            .CountAsync(d => d.FolderId == folderId && !d.IsDeleted, cancellationToken);

        return new FolderDto
        {
            Id = folder.Id,
            Name = folder.Name.English,
            NameArabic = folder.Name.Arabic,
            Description = folder.Description?.English,
            DescriptionArabic = folder.Description?.Arabic,
            LibraryId = folder.LibraryId,
            ParentId = folder.ParentId,
            Path = folder.Path,
            IconName = folder.IconName,
            Color = folder.Color,
            DocumentCount = documentCount,
            ChildFolderCount = folder.Children.Count,
            Children = folder.Children.Select(c => new FolderSummaryDto
            {
                Id = c.Id,
                Name = c.Name.English,
                NameArabic = c.Name.Arabic,
                IconName = c.IconName,
                Color = c.Color,
                DocumentCount = 0, // Will be calculated on demand
                ChildFolderCount = 0
            }).ToList(),
            CreatedAt = folder.CreatedAt
        };
    }

    public async Task<FolderContentsDto> GetFolderContentsAsync(
        Guid folderId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var folder = await GetFolderAsync(folderId, userId, cancellationToken);
        if (folder == null)
        {
            return new FolderContentsDto
            {
                Folder = new FolderDto(),
                Subfolders = Array.Empty<FolderSummaryDto>(),
                Documents = Array.Empty<DocumentSummaryDto>(),
                Breadcrumbs = Array.Empty<BreadcrumbDto>()
            };
        }

        // Get subfolders with counts
        var subfolders = await _dbContext.Set<Folder>()
            .AsNoTracking()
            .Where(f => f.ParentId == folderId && !f.IsDeleted)
            .Select(f => new FolderSummaryDto
            {
                Id = f.Id,
                Name = f.Name.English,
                NameArabic = f.Name.Arabic,
                IconName = f.IconName,
                Color = f.Color,
                DocumentCount = f.Documents.Count(d => !d.IsDeleted),
                ChildFolderCount = f.Children.Count(c => !c.IsDeleted)
            })
            .OrderBy(f => f.Name)
            .ToListAsync(cancellationToken);

        // Get documents
        var documents = await _dbContext.Set<Document>()
            .AsNoTracking()
            .Where(d => d.FolderId == folderId && !d.IsDeleted)
            .Select(d => new DocumentSummaryDto
            {
                Id = d.Id,
                Name = d.Name,
                NameArabic = d.NameArabic,
                FileName = d.FileName,
                FileExtension = d.FileExtension,
                Extension = d.FileExtension,
                FileSize = d.FileSize,
                MimeType = d.ContentType,
                ThumbnailUrl = d.ThumbnailPath,
                Status = d.Status.ToString(),
                Version = d.GetVersionString(),
                IsCheckedOut = d.CheckedOutById != null,
                CheckedOutByName = d.CheckedOutByName,
                CreatedByName = "", // TODO: Add CreatedByName to Document entity
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.ModifiedAt,
                ModifiedAt = d.ModifiedAt ?? d.CreatedAt
            })
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);

        // Get breadcrumbs
        var breadcrumbs = await GetBreadcrumbAsync(folderId, cancellationToken);

        return new FolderContentsDto
        {
            Folder = folder,
            Subfolders = subfolders,
            Documents = documents,
            Breadcrumbs = breadcrumbs
        };
    }

    public async Task<FolderDto> CreateFolderAsync(
        CreateFolderRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Check library access
        var hasLibraryAccess = await _permissionService.HasLibraryAccessAsync(
            request.LibraryId, userId, PermissionLevel.Write, cancellationToken);

        if (!hasLibraryAccess)
        {
            throw new UnauthorizedAccessException("User does not have write access to the library");
        }

        // If parent folder specified, check access and get path
        string? parentPath = null;
        if (request.ParentId.HasValue)
        {
            var hasParentAccess = await _permissionService.HasFolderAccessAsync(
                request.ParentId.Value, userId, PermissionLevel.Write, cancellationToken);

            if (!hasParentAccess)
            {
                throw new UnauthorizedAccessException("User does not have write access to the parent folder");
            }

            var parentFolder = await _dbContext.Set<Folder>()
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == request.ParentId.Value && !f.IsDeleted, cancellationToken);

            if (parentFolder == null)
            {
                throw new ArgumentException("Parent folder not found");
            }

            parentPath = parentFolder.Path;
        }

        var name = LocalizedString.Create(request.Name, request.NameArabic);
        var folder = Folder.Create(name, request.LibraryId, request.ParentId, parentPath);

        if (!string.IsNullOrEmpty(request.Description))
        {
            var description = LocalizedString.Create(request.Description, request.DescriptionArabic);
            folder.Update(name, description);
        }

        _dbContext.Set<Folder>().Add(folder);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Folder {FolderId} '{Name}' created in library {LibraryId} by user {UserId}",
            folder.Id, request.Name, request.LibraryId, userId);

        return new FolderDto
        {
            Id = folder.Id,
            Name = folder.Name.English,
            NameArabic = folder.Name.Arabic,
            Description = folder.Description?.English,
            DescriptionArabic = folder.Description?.Arabic,
            LibraryId = folder.LibraryId,
            ParentId = folder.ParentId,
            Path = folder.Path,
            IconName = folder.IconName,
            Color = folder.Color,
            DocumentCount = 0,
            ChildFolderCount = 0,
            Children = Array.Empty<FolderSummaryDto>(),
            CreatedAt = folder.CreatedAt
        };
    }

    public async Task<bool> UpdateFolderAsync(
        Guid folderId,
        UpdateFolderRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Check write access
        var hasAccess = await _permissionService.HasFolderAccessAsync(
            folderId, userId, PermissionLevel.Write, cancellationToken);

        if (!hasAccess)
        {
            _logger.LogWarning("User {UserId} denied write access to folder {FolderId}", userId, folderId);
            return false;
        }

        var folder = await _dbContext.Set<Folder>()
            .FirstOrDefaultAsync(f => f.Id == folderId && !f.IsDeleted, cancellationToken);

        if (folder == null)
            return false;

        var name = LocalizedString.Create(request.Name, request.NameArabic);
        var description = !string.IsNullOrEmpty(request.Description)
            ? LocalizedString.Create(request.Description, request.DescriptionArabic)
            : null;

        folder.Update(name, description);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Folder {FolderId} updated by user {UserId}", folderId, userId);

        return true;
    }

    public async Task<bool> DeleteFolderAsync(
        Guid folderId,
        bool deleteContents,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Check delete access
        var hasAccess = await _permissionService.HasFolderAccessAsync(
            folderId, userId, PermissionLevel.Delete, cancellationToken);

        if (!hasAccess)
        {
            _logger.LogWarning("User {UserId} denied delete access to folder {FolderId}", userId, folderId);
            return false;
        }

        var folder = await _dbContext.Set<Folder>()
            .Include(f => f.Children)
            .Include(f => f.Documents)
            .FirstOrDefaultAsync(f => f.Id == folderId && !f.IsDeleted, cancellationToken);

        if (folder == null)
            return false;

        // Check if folder has contents and deleteContents is false
        if (!deleteContents && (folder.Children.Any(c => !c.IsDeleted) || folder.Documents.Any(d => !d.IsDeleted)))
        {
            _logger.LogWarning("Cannot delete folder {FolderId} because it has contents", folderId);
            return false;
        }

        // Soft delete folder
        folder.Delete();

        if (deleteContents)
        {
            // Recursively delete subfolders
            await DeleteFolderChildrenAsync(folder, cancellationToken);

            // Delete documents
            foreach (var document in folder.Documents.Where(d => !d.IsDeleted))
            {
                document.Delete();
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Folder {FolderId} deleted by user {UserId}", folderId, userId);

        return true;
    }

    private async Task DeleteFolderChildrenAsync(Folder folder, CancellationToken cancellationToken)
    {
        foreach (var child in folder.Children.Where(c => !c.IsDeleted))
        {
            child.Delete();

            // Load child's children and documents
            await _dbContext.Entry(child)
                .Collection(c => c.Children)
                .LoadAsync(cancellationToken);
            await _dbContext.Entry(child)
                .Collection(c => c.Documents)
                .LoadAsync(cancellationToken);

            foreach (var document in child.Documents.Where(d => !d.IsDeleted))
            {
                document.Delete();
            }

            await DeleteFolderChildrenAsync(child, cancellationToken);
        }
    }

    public async Task<bool> MoveFolderAsync(
        Guid folderId,
        Guid? targetParentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Check write access on source folder
        var hasSourceAccess = await _permissionService.HasFolderAccessAsync(
            folderId, userId, PermissionLevel.Write, cancellationToken);

        if (!hasSourceAccess)
        {
            _logger.LogWarning("User {UserId} denied write access to source folder {FolderId}", userId, folderId);
            return false;
        }

        var folder = await _dbContext.Set<Folder>()
            .FirstOrDefaultAsync(f => f.Id == folderId && !f.IsDeleted, cancellationToken);

        if (folder == null)
            return false;

        // Check target parent access
        string? newParentPath = null;
        if (targetParentId.HasValue)
        {
            var hasTargetAccess = await _permissionService.HasFolderAccessAsync(
                targetParentId.Value, userId, PermissionLevel.Write, cancellationToken);

            if (!hasTargetAccess)
            {
                _logger.LogWarning("User {UserId} denied write access to target folder {FolderId}", userId, targetParentId);
                return false;
            }

            var targetParent = await _dbContext.Set<Folder>()
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == targetParentId.Value && !f.IsDeleted, cancellationToken);

            if (targetParent == null)
                return false;

            // Check that target is in same library
            if (targetParent.LibraryId != folder.LibraryId)
            {
                _logger.LogWarning("Cannot move folder to different library");
                return false;
            }

            // Check that target is not a descendant of source (would create circular reference)
            if (await IsDescendantOfAsync(targetParentId.Value, folderId, cancellationToken))
            {
                _logger.LogWarning("Cannot move folder {FolderId} into its own descendant", folderId);
                return false;
            }

            newParentPath = targetParent.Path;
        }

        var newPath = string.IsNullOrEmpty(newParentPath)
            ? folder.Name.English
            : $"{newParentPath}/{folder.Name.English}";

        folder.MoveTo(targetParentId, newPath);

        // Update paths of all descendants
        await UpdateDescendantPathsAsync(folder, newPath, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Folder {FolderId} moved to parent {TargetParentId} by user {UserId}",
            folderId, targetParentId, userId);

        return true;
    }

    private async Task<bool> IsDescendantOfAsync(Guid folderId, Guid ancestorId, CancellationToken cancellationToken)
    {
        var folder = await _dbContext.Set<Folder>()
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == folderId && !f.IsDeleted, cancellationToken);

        while (folder?.ParentId != null)
        {
            if (folder.ParentId == ancestorId)
                return true;

            folder = await _dbContext.Set<Folder>()
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == folder.ParentId && !f.IsDeleted, cancellationToken);
        }

        return false;
    }

    private async Task UpdateDescendantPathsAsync(Folder folder, string parentPath, CancellationToken cancellationToken)
    {
        var children = await _dbContext.Set<Folder>()
            .Where(f => f.ParentId == folder.Id && !f.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var child in children)
        {
            var newChildPath = $"{parentPath}/{child.Name.English}";
            child.MoveTo(child.ParentId, newChildPath);
            await UpdateDescendantPathsAsync(child, newChildPath, cancellationToken);
        }
    }

    public async Task<FolderDto?> CopyFolderAsync(
        Guid folderId,
        Guid? targetParentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Check read access on source folder
        var hasSourceAccess = await _permissionService.HasFolderAccessAsync(
            folderId, userId, PermissionLevel.Read, cancellationToken);

        if (!hasSourceAccess)
        {
            _logger.LogWarning("User {UserId} denied read access to source folder {FolderId}", userId, folderId);
            return null;
        }

        var sourceFolder = await _dbContext.Set<Folder>()
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == folderId && !f.IsDeleted, cancellationToken);

        if (sourceFolder == null)
            return null;

        // Determine target library and parent path
        Guid targetLibraryId = sourceFolder.LibraryId;
        string? parentPath = null;

        if (targetParentId.HasValue)
        {
            var hasTargetAccess = await _permissionService.HasFolderAccessAsync(
                targetParentId.Value, userId, PermissionLevel.Write, cancellationToken);

            if (!hasTargetAccess)
            {
                _logger.LogWarning("User {UserId} denied write access to target folder {FolderId}", userId, targetParentId);
                return null;
            }

            var targetParent = await _dbContext.Set<Folder>()
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == targetParentId.Value && !f.IsDeleted, cancellationToken);

            if (targetParent == null)
                return null;

            targetLibraryId = targetParent.LibraryId;
            parentPath = targetParent.Path;
        }
        else
        {
            // Check library write access
            var hasLibraryAccess = await _permissionService.HasLibraryAccessAsync(
                targetLibraryId, userId, PermissionLevel.Write, cancellationToken);

            if (!hasLibraryAccess)
            {
                _logger.LogWarning("User {UserId} denied write access to library {LibraryId}", userId, targetLibraryId);
                return null;
            }
        }

        // Create new folder
        var newName = LocalizedString.Create($"{sourceFolder.Name.English} (Copy)", sourceFolder.Name.Arabic != null ? $"{sourceFolder.Name.Arabic} (نسخة)" : null);
        var newFolder = Folder.Create(newName, targetLibraryId, targetParentId, parentPath);

        if (sourceFolder.Description != null)
        {
            newFolder.Update(newName, sourceFolder.Description);
        }

        newFolder.SetIcon(sourceFolder.IconName, sourceFolder.Color);

        _dbContext.Set<Folder>().Add(newFolder);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Folder {SourceFolderId} copied to {NewFolderId} by user {UserId}",
            folderId, newFolder.Id, userId);

        // Note: Documents are not copied in this basic implementation
        // A full implementation would recursively copy all contents

        return await GetFolderAsync(newFolder.Id, userId, cancellationToken);
    }

    public async Task<IReadOnlyList<BreadcrumbDto>> GetBreadcrumbAsync(
        Guid folderId,
        CancellationToken cancellationToken = default)
    {
        var breadcrumbs = new List<BreadcrumbDto>();

        var folder = await _dbContext.Set<Folder>()
            .AsNoTracking()
            .Include(f => f.Library)
            .FirstOrDefaultAsync(f => f.Id == folderId && !f.IsDeleted, cancellationToken);

        if (folder == null)
            return breadcrumbs;

        // Add library as first breadcrumb
        breadcrumbs.Add(new BreadcrumbDto
        {
            Id = folder.LibraryId,
            Name = folder.Library.Name.English,
            NameArabic = folder.Library.Name.Arabic,
            Type = "library"
        });

        // Build folder path from root to current
        var folderPath = new List<BreadcrumbDto>();
        var currentFolder = folder;

        while (currentFolder != null)
        {
            folderPath.Add(new BreadcrumbDto
            {
                Id = currentFolder.Id,
                Name = currentFolder.Name.English,
                NameArabic = currentFolder.Name.Arabic,
                Type = "folder"
            });

            if (currentFolder.ParentId.HasValue)
            {
                currentFolder = await _dbContext.Set<Folder>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Id == currentFolder.ParentId.Value && !f.IsDeleted, cancellationToken);
            }
            else
            {
                currentFolder = null;
            }
        }

        // Reverse to get path from root to current
        folderPath.Reverse();
        breadcrumbs.AddRange(folderPath);

        return breadcrumbs;
    }

    public async Task<IReadOnlyList<FolderPermissionDto>> GetFolderPermissionsAsync(
        Guid folderId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Check manage access
        var hasAccess = await _permissionService.HasFolderAccessAsync(
            folderId, userId, PermissionLevel.Manage, cancellationToken);

        if (!hasAccess)
            return Array.Empty<FolderPermissionDto>();

        var permissions = await _dbContext.Set<FolderPermission>()
            .AsNoTracking()
            .Where(fp => fp.FolderId == folderId && !fp.IsDeleted)
            .ToListAsync(cancellationToken);

        var result = new List<FolderPermissionDto>();

        foreach (var permission in permissions)
        {
            string? userName = null;
            string? groupName = null;

            // TODO: Implement user/group lookup when Identity module is available
            if (permission.UserId.HasValue)
            {
                userName = $"User-{permission.UserId.Value}";
            }

            if (permission.GroupId.HasValue)
            {
                groupName = $"Group-{permission.GroupId.Value}";
            }

            result.Add(new FolderPermissionDto
            {
                Id = permission.Id,
                FolderId = permission.FolderId,
                UserId = permission.UserId,
                UserName = userName,
                GroupId = permission.GroupId,
                GroupName = groupName,
                PermissionLevel = permission.Level.ToString(),
                InheritFromParent = permission.InheritFromParent,
                PropagateToChildren = permission.PropagateToChildren,
                GrantedAt = permission.CreatedAt
            });
        }

        return result;
    }

    public async Task<bool> SetFolderPermissionAsync(
        Guid folderId,
        SetPermissionRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Check manage access
        var hasAccess = await _permissionService.HasFolderAccessAsync(
            folderId, userId, PermissionLevel.Manage, cancellationToken);

        if (!hasAccess)
        {
            _logger.LogWarning("User {UserId} denied permission management on folder {FolderId}", userId, folderId);
            return false;
        }

        // Ensure at least one target is specified
        if (!request.UserId.HasValue && !request.GroupId.HasValue)
        {
            _logger.LogWarning("SetFolderPermission called without specifying a target user or group");
            return false;
        }

        var permissionLevel = Enum.Parse<PermissionLevel>(request.AccessLevel, true);

        // TODO: Get current user's name from Identity module when available
        var grantedByName = $"User-{userId}";

        // Check for existing permission
        var existingPermission = await _dbContext.Set<FolderPermission>()
            .FirstOrDefaultAsync(fp =>
                fp.FolderId == folderId &&
                fp.UserId == request.UserId &&
                fp.GroupId == request.GroupId &&
                !fp.IsDeleted,
                cancellationToken);

        if (existingPermission != null)
        {
            // Update existing permission
            existingPermission.UpdateLevel(permissionLevel);
            existingPermission.SetInheritance(request.InheritFromParent, request.PropagateToChildren);
        }
        else
        {
            // Create new permission
            FolderPermission newPermission;

            if (request.UserId.HasValue)
            {
                newPermission = FolderPermission.CreateForUser(
                    folderId,
                    request.UserId.Value,
                    permissionLevel,
                    userId,
                    grantedByName,
                    request.InheritFromParent,
                    request.PropagateToChildren);
            }
            else
            {
                newPermission = FolderPermission.CreateForGroup(
                    folderId,
                    request.GroupId!.Value,
                    permissionLevel,
                    userId,
                    grantedByName,
                    request.InheritFromParent,
                    request.PropagateToChildren);
            }

            _dbContext.Set<FolderPermission>().Add(newPermission);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Permission set on folder {FolderId} by user {UserId}", folderId, userId);

        return true;
    }
}
