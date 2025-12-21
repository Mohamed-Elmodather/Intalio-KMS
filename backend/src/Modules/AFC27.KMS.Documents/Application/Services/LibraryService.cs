using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Documents.Application.DTOs;
using AFC27.KMS.Documents.Application.Interfaces;
using AFC27.KMS.Documents.Domain.Entities;
using AFC27.KMS.SharedKernel.Domain;
using PermissionLevel = AFC27.KMS.Documents.Domain.Entities.PermissionLevel;

namespace AFC27.KMS.Documents.Application.Services;

/// <summary>
/// Service for managing document libraries.
/// </summary>
public class LibraryService : ILibraryService
{
    private readonly DbContext _dbContext;
    private readonly IPermissionService _permissionService;
    private readonly ILogger<LibraryService> _logger;

    public LibraryService(
        DbContext dbContext,
        IPermissionService permissionService,
        ILogger<LibraryService> logger)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
        _logger = logger;
    }

    public async Task<IReadOnlyList<LibrarySummaryDto>> GetLibrariesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Get accessible library IDs
        var accessibleIds = await _permissionService.GetAccessibleLibraryIdsAsync(
            userId, PermissionLevel.Read, cancellationToken);

        var libraries = await _dbContext.Set<DocumentLibrary>()
            .AsNoTracking()
            .Where(l => !l.IsDeleted && accessibleIds.Contains(l.Id))
            .Select(l => new LibrarySummaryDto
            {
                Id = l.Id,
                Name = l.Name.English,
                NameArabic = l.Name.Arabic,
                IconName = l.IconName,
                Color = l.Color,
                Type = l.Type.ToString(),
                IsPublic = l.IsPublic,
                DocumentCount = l.Documents.Count(d => !d.IsDeleted),
                TotalSize = l.Documents.Where(d => !d.IsDeleted).Sum(d => d.FileSize)
            })
            .OrderBy(l => l.Name)
            .ToListAsync(cancellationToken);

        return libraries;
    }

    public async Task<LibraryDto?> GetLibraryAsync(
        Guid libraryId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Check access
        var hasAccess = await _permissionService.HasLibraryAccessAsync(
            libraryId, userId, PermissionLevel.Read, cancellationToken);

        if (!hasAccess)
        {
            _logger.LogWarning("User {UserId} denied access to library {LibraryId}", userId, libraryId);
            return null;
        }

        var library = await _dbContext.Set<DocumentLibrary>()
            .AsNoTracking()
            .Where(l => l.Id == libraryId && !l.IsDeleted)
            .Select(l => new LibraryDto
            {
                Id = l.Id,
                Name = l.Name.English,
                NameArabic = l.Name.Arabic,
                Description = l.Description != null ? l.Description.English : null,
                DescriptionArabic = l.Description != null ? l.Description.Arabic : null,
                IconName = l.IconName,
                Color = l.Color,
                Type = l.Type.ToString(),
                IsPublic = l.IsPublic,
                RequiresApproval = l.RequiresApproval,
                EnableVersioning = l.EnableVersioning,
                MaxVersions = l.MaxVersions,
                MaxFileSize = l.MaxFileSize,
                AllowedExtensions = l.AllowedExtensions,
                OwnerId = l.OwnerId,
                OwnerName = l.OwnerName,
                DocumentCount = l.Documents.Count(d => !d.IsDeleted),
                FolderCount = l.Folders.Count(f => !f.IsDeleted),
                TotalSize = l.Documents.Where(d => !d.IsDeleted).Sum(d => d.FileSize),
                CreatedAt = l.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        return library;
    }

    public async Task<LibraryDto> CreateLibraryAsync(
        CreateLibraryRequest request,
        Guid userId,
        string userName,
        CancellationToken cancellationToken = default)
    {
        var name = LocalizedString.Create(request.Name, request.NameArabic);
        var libraryType = Enum.Parse<LibraryType>(request.Type, true);

        var library = DocumentLibrary.Create(name, libraryType, userId, userName, request.IsPublic);

        // Set optional properties
        if (!string.IsNullOrEmpty(request.Description))
        {
            var description = LocalizedString.Create(request.Description, request.DescriptionArabic);
            library.Update(name, description);
        }

        library.SetVersioningOptions(request.EnableVersioning, request.MaxVersions);
        library.SetFileRestrictions(request.MaxFileSize, request.AllowedExtensions);
        library.SetApprovalRequired(request.RequiresApproval);

        _dbContext.Set<DocumentLibrary>().Add(library);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Library {LibraryId} '{Name}' created by user {UserId}",
            library.Id, request.Name, userId);

        return new LibraryDto
        {
            Id = library.Id,
            Name = library.Name.English,
            NameArabic = library.Name.Arabic,
            Description = library.Description?.English,
            DescriptionArabic = library.Description?.Arabic,
            IconName = library.IconName,
            Color = library.Color,
            Type = library.Type.ToString(),
            IsPublic = library.IsPublic,
            RequiresApproval = library.RequiresApproval,
            EnableVersioning = library.EnableVersioning,
            MaxVersions = library.MaxVersions,
            MaxFileSize = library.MaxFileSize,
            AllowedExtensions = library.AllowedExtensions,
            OwnerId = library.OwnerId,
            OwnerName = library.OwnerName,
            DocumentCount = 0,
            FolderCount = 0,
            TotalSize = 0,
            CreatedAt = library.CreatedAt
        };
    }

    public async Task<bool> UpdateLibraryAsync(
        Guid libraryId,
        UpdateLibraryRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Check write access
        var hasAccess = await _permissionService.HasLibraryAccessAsync(
            libraryId, userId, PermissionLevel.Write, cancellationToken);

        if (!hasAccess)
        {
            _logger.LogWarning("User {UserId} denied write access to library {LibraryId}", userId, libraryId);
            return false;
        }

        var library = await _dbContext.Set<DocumentLibrary>()
            .FirstOrDefaultAsync(l => l.Id == libraryId && !l.IsDeleted, cancellationToken);

        if (library == null)
            return false;

        var name = LocalizedString.Create(request.Name, request.NameArabic);
        var description = !string.IsNullOrEmpty(request.Description)
            ? LocalizedString.Create(request.Description, request.DescriptionArabic)
            : null;

        library.Update(name, description);
        library.SetPublic(request.IsPublic);
        library.SetApprovalRequired(request.RequiresApproval);
        library.SetVersioningOptions(request.EnableVersioning, request.MaxVersions);
        library.SetFileRestrictions(request.MaxFileSize, request.AllowedExtensions);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Library {LibraryId} updated by user {UserId}", libraryId, userId);

        return true;
    }

    public async Task<bool> DeleteLibraryAsync(
        Guid libraryId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Check if user is owner or has manage permission
        var isOwner = await _permissionService.IsLibraryOwnerAsync(libraryId, userId, cancellationToken);
        var hasManageAccess = await _permissionService.HasLibraryAccessAsync(
            libraryId, userId, PermissionLevel.Manage, cancellationToken);

        if (!isOwner && !hasManageAccess)
        {
            _logger.LogWarning("User {UserId} denied delete access to library {LibraryId}", userId, libraryId);
            return false;
        }

        var library = await _dbContext.Set<DocumentLibrary>()
            .Include(l => l.Folders)
            .Include(l => l.Documents)
            .FirstOrDefaultAsync(l => l.Id == libraryId && !l.IsDeleted, cancellationToken);

        if (library == null)
            return false;

        // Soft delete library and all contents
        library.Delete();

        foreach (var folder in library.Folders)
        {
            folder.Delete();
        }

        foreach (var document in library.Documents)
        {
            document.Delete();
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Library {LibraryId} deleted by user {UserId}", libraryId, userId);

        return true;
    }

    public async Task<IReadOnlyList<FolderTreeDto>> GetFolderTreeAsync(
        Guid libraryId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Check access
        var hasAccess = await _permissionService.HasLibraryAccessAsync(
            libraryId, userId, PermissionLevel.Read, cancellationToken);

        if (!hasAccess)
            return Array.Empty<FolderTreeDto>();

        var folders = await _dbContext.Set<Folder>()
            .AsNoTracking()
            .Where(f => f.LibraryId == libraryId && !f.IsDeleted)
            .Select(f => new FolderTreeItem
            {
                Id = f.Id,
                Name = f.Name.English,
                NameArabic = f.Name.Arabic,
                ParentId = f.ParentId,
                IconName = f.IconName,
                DocumentCount = f.Documents.Count(d => !d.IsDeleted)
            })
            .ToListAsync(cancellationToken);

        // Build tree structure
        var rootFolders = folders
            .Where(f => f.ParentId == null)
            .Select(f => BuildFolderTree(f, folders))
            .OrderBy(f => f.Name)
            .ToList();

        return rootFolders;
    }

    private static FolderTreeDto BuildFolderTree(
        FolderTreeItem folder,
        List<FolderTreeItem> allFolders)
    {
        var children = allFolders
            .Where(f => f.ParentId == folder.Id)
            .Select(f => BuildFolderTree(f, allFolders))
            .OrderBy(f => f.Name)
            .ToList();

        return new FolderTreeDto
        {
            Id = folder.Id,
            Name = folder.Name,
            NameArabic = folder.NameArabic,
            ParentId = folder.ParentId,
            IconName = folder.IconName,
            DocumentCount = folder.DocumentCount,
            Children = children
        };
    }

    // Internal class for folder tree building
    private class FolderTreeItem
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = "";
        public string? NameArabic { get; init; }
        public Guid? ParentId { get; init; }
        public string? IconName { get; init; }
        public int DocumentCount { get; init; }
    }

    public async Task<LibraryStatsDto> GetLibraryStatsAsync(
        Guid libraryId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Check access
        var hasAccess = await _permissionService.HasLibraryAccessAsync(
            libraryId, userId, PermissionLevel.Read, cancellationToken);

        if (!hasAccess)
        {
            return new LibraryStatsDto { LibraryId = libraryId };
        }

        var documents = await _dbContext.Set<Document>()
            .AsNoTracking()
            .Where(d => d.LibraryId == libraryId && !d.IsDeleted)
            .Select(d => new
            {
                d.FileSize,
                d.Status,
                d.CheckedOutById,
                d.CreatedBy,
                d.FileExtension,
                d.ModifiedAt
            })
            .ToListAsync(cancellationToken);

        var folderCount = await _dbContext.Set<Folder>()
            .AsNoTracking()
            .CountAsync(f => f.LibraryId == libraryId && !f.IsDeleted, cancellationToken);

        var documentsByExtension = documents
            .Where(d => !string.IsNullOrEmpty(d.FileExtension))
            .GroupBy(d => d.FileExtension!.ToLowerInvariant())
            .ToDictionary(g => g.Key, g => g.Count());

        return new LibraryStatsDto
        {
            LibraryId = libraryId,
            TotalDocuments = documents.Count,
            TotalFolders = folderCount,
            TotalSizeBytes = documents.Sum(d => d.FileSize),
            PublishedDocuments = documents.Count(d => d.Status == DocumentStatus.Published),
            DraftDocuments = documents.Count(d => d.Status == DocumentStatus.Draft),
            CheckedOutDocuments = documents.Count(d => d.CheckedOutById != null),
            UniqueContributors = documents.Select(d => d.CreatedBy).Distinct().Count(),
            LastActivityAt = documents.Max(d => (DateTime?)d.ModifiedAt),
            DocumentsByExtension = documentsByExtension
        };
    }

    public async Task<IReadOnlyList<LibraryPermissionDto>> GetLibraryPermissionsAsync(
        Guid libraryId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Check manage access
        var hasAccess = await _permissionService.HasLibraryAccessAsync(
            libraryId, userId, PermissionLevel.Manage, cancellationToken);

        if (!hasAccess)
            return Array.Empty<LibraryPermissionDto>();

        var permissions = await _dbContext.Set<LibraryPermission>()
            .AsNoTracking()
            .Where(lp => lp.LibraryId == libraryId)
            .ToListAsync(cancellationToken);

        var result = new List<LibraryPermissionDto>();

        foreach (var permission in permissions)
        {
            string? userName = null;
            string? groupName = null;
            string? roleName = null;

            // TODO: Implement user/group/role lookup when Identity module is available
            if (permission.UserId.HasValue)
            {
                userName = $"User-{permission.UserId.Value}";
            }

            if (permission.GroupId.HasValue)
            {
                groupName = $"Group-{permission.GroupId.Value}";
            }

            if (permission.RoleId.HasValue)
            {
                roleName = $"Role-{permission.RoleId.Value}";
            }

            result.Add(new LibraryPermissionDto
            {
                Id = permission.Id,
                LibraryId = permission.LibraryId,
                UserId = permission.UserId,
                UserName = userName,
                GroupId = permission.GroupId,
                GroupName = groupName,
                RoleId = permission.RoleId,
                RoleName = roleName,
                AccessLevel = permission.AccessLevel.ToString(),
                GrantedAt = DateTime.UtcNow // LibraryPermission doesn't track this, using current time
            });
        }

        return result;
    }

    public async Task<bool> SetLibraryPermissionAsync(
        Guid libraryId,
        SetPermissionRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Check manage access
        var hasAccess = await _permissionService.HasLibraryAccessAsync(
            libraryId, userId, PermissionLevel.Manage, cancellationToken);

        if (!hasAccess)
        {
            _logger.LogWarning("User {UserId} denied permission management on library {LibraryId}", userId, libraryId);
            return false;
        }

        // Ensure at least one target is specified
        if (!request.UserId.HasValue && !request.GroupId.HasValue && !request.RoleId.HasValue)
        {
            _logger.LogWarning("SetLibraryPermission called without specifying a target user, group, or role");
            return false;
        }

        var accessLevel = Enum.Parse<LibraryAccessLevel>(request.AccessLevel, true);

        // Check for existing permission
        var existingPermission = await _dbContext.Set<LibraryPermission>()
            .FirstOrDefaultAsync(lp =>
                lp.LibraryId == libraryId &&
                lp.UserId == request.UserId &&
                lp.GroupId == request.GroupId &&
                lp.RoleId == request.RoleId,
                cancellationToken);

        if (existingPermission != null)
        {
            // Update existing permission - we need to use reflection or recreate since LibraryPermission doesn't have an Update method
            _dbContext.Set<LibraryPermission>().Remove(existingPermission);
        }

        // Create new permission
        var permission = new LibraryPermission
        {
            LibraryId = libraryId,
            UserId = request.UserId,
            GroupId = request.GroupId,
            RoleId = request.RoleId,
            AccessLevel = accessLevel
        };

        _dbContext.Set<LibraryPermission>().Add(permission);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Permission set on library {LibraryId} by user {UserId}", libraryId, userId);

        return true;
    }

    public async Task<bool> RemoveLibraryPermissionAsync(
        Guid libraryId,
        Guid permissionId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Check manage access
        var hasAccess = await _permissionService.HasLibraryAccessAsync(
            libraryId, userId, PermissionLevel.Manage, cancellationToken);

        if (!hasAccess)
        {
            _logger.LogWarning("User {UserId} denied permission management on library {LibraryId}", userId, libraryId);
            return false;
        }

        var permission = await _dbContext.Set<LibraryPermission>()
            .FirstOrDefaultAsync(lp => lp.Id == permissionId && lp.LibraryId == libraryId, cancellationToken);

        if (permission == null)
            return false;

        _dbContext.Set<LibraryPermission>().Remove(permission);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Permission {PermissionId} removed from library {LibraryId} by user {UserId}",
            permissionId, libraryId, userId);

        return true;
    }
}

/// <summary>
/// Extended LibraryPermission class with settable properties for creation.
/// This is a workaround since the domain entity has private setters.
/// </summary>
file class LibraryPermission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LibraryId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? RoleId { get; set; }
    public Guid? GroupId { get; set; }
    public LibraryAccessLevel AccessLevel { get; set; }
}
