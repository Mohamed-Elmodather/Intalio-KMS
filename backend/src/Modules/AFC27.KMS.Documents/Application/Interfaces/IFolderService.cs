using AFC27.KMS.Documents.Application.DTOs;

namespace AFC27.KMS.Documents.Application.Interfaces;

/// <summary>
/// Service interface for folder operations.
/// </summary>
public interface IFolderService
{
    /// <summary>
    /// Gets a folder by ID with permission check.
    /// </summary>
    Task<FolderDto?> GetFolderAsync(
        Guid folderId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets folder contents (subfolders and documents).
    /// </summary>
    Task<FolderContentsDto> GetFolderContentsAsync(
        Guid folderId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new folder.
    /// </summary>
    Task<FolderDto> CreateFolderAsync(
        CreateFolderRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a folder's metadata.
    /// </summary>
    Task<bool> UpdateFolderAsync(
        Guid folderId,
        UpdateFolderRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a folder and optionally its contents.
    /// </summary>
    Task<bool> DeleteFolderAsync(
        Guid folderId,
        bool deleteContents,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Moves a folder to a new parent.
    /// </summary>
    Task<bool> MoveFolderAsync(
        Guid folderId,
        Guid? targetParentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Copies a folder and its contents.
    /// </summary>
    Task<FolderDto?> CopyFolderAsync(
        Guid folderId,
        Guid? targetParentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the breadcrumb path for a folder.
    /// </summary>
    Task<IReadOnlyList<BreadcrumbDto>> GetBreadcrumbAsync(
        Guid folderId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets folder permissions.
    /// </summary>
    Task<IReadOnlyList<FolderPermissionDto>> GetFolderPermissionsAsync(
        Guid folderId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a permission on a folder.
    /// </summary>
    Task<bool> SetFolderPermissionAsync(
        Guid folderId,
        SetPermissionRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);
}
