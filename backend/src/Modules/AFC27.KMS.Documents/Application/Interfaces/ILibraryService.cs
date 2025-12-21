using AFC27.KMS.Documents.Application.DTOs;
using AFC27.KMS.Contracts.Common;

namespace AFC27.KMS.Documents.Application.Interfaces;

/// <summary>
/// Service interface for document library operations.
/// </summary>
public interface ILibraryService
{
    /// <summary>
    /// Gets all libraries accessible to the user.
    /// </summary>
    Task<IReadOnlyList<LibrarySummaryDto>> GetLibrariesAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a library by ID with permission check.
    /// </summary>
    Task<LibraryDto?> GetLibraryAsync(
        Guid libraryId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new document library.
    /// </summary>
    Task<LibraryDto> CreateLibraryAsync(
        CreateLibraryRequest request,
        Guid userId,
        string userName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a library's metadata.
    /// </summary>
    Task<bool> UpdateLibraryAsync(
        Guid libraryId,
        UpdateLibraryRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a library and all its contents.
    /// </summary>
    Task<bool> DeleteLibraryAsync(
        Guid libraryId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the folder tree structure for a library.
    /// </summary>
    Task<IReadOnlyList<FolderTreeDto>> GetFolderTreeAsync(
        Guid libraryId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets library statistics (document count, total size, etc.).
    /// </summary>
    Task<LibraryStatsDto> GetLibraryStatsAsync(
        Guid libraryId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets library permissions.
    /// </summary>
    Task<IReadOnlyList<LibraryPermissionDto>> GetLibraryPermissionsAsync(
        Guid libraryId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a permission on a library.
    /// </summary>
    Task<bool> SetLibraryPermissionAsync(
        Guid libraryId,
        SetPermissionRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a permission from a library.
    /// </summary>
    Task<bool> RemoveLibraryPermissionAsync(
        Guid libraryId,
        Guid permissionId,
        Guid userId,
        CancellationToken cancellationToken = default);
}
