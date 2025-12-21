using AFC27.KMS.Documents.Domain.Entities;

namespace AFC27.KMS.Documents.Application.Interfaces;

/// <summary>
/// Service interface for permission checking and management.
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Checks if a user has access to a library.
    /// </summary>
    Task<bool> HasLibraryAccessAsync(
        Guid libraryId,
        Guid userId,
        PermissionLevel requiredLevel,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has access to a folder.
    /// </summary>
    Task<bool> HasFolderAccessAsync(
        Guid folderId,
        Guid userId,
        PermissionLevel requiredLevel,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has access to a document.
    /// </summary>
    Task<bool> HasDocumentAccessAsync(
        Guid documentId,
        Guid userId,
        PermissionLevel requiredLevel,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the effective permission level for a user on a library.
    /// </summary>
    Task<PermissionLevel> GetEffectiveLibraryPermissionAsync(
        Guid libraryId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the effective permission level for a user on a folder.
    /// </summary>
    Task<PermissionLevel> GetEffectiveFolderPermissionAsync(
        Guid folderId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the effective permission level for a user on a document.
    /// </summary>
    Task<PermissionLevel> GetEffectiveDocumentPermissionAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all user's group memberships for permission resolution.
    /// </summary>
    Task<IReadOnlyList<Guid>> GetUserGroupsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all user's roles for permission resolution.
    /// </summary>
    Task<IReadOnlyList<Guid>> GetUserRolesAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user is the owner of a library.
    /// </summary>
    Task<bool> IsLibraryOwnerAsync(
        Guid libraryId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets IDs of all libraries accessible to a user.
    /// </summary>
    Task<IReadOnlyList<Guid>> GetAccessibleLibraryIdsAsync(
        Guid userId,
        PermissionLevel minimumLevel = PermissionLevel.Read,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets IDs of all folders accessible to a user in a library.
    /// </summary>
    Task<IReadOnlyList<Guid>> GetAccessibleFolderIdsAsync(
        Guid libraryId,
        Guid userId,
        PermissionLevel minimumLevel = PermissionLevel.Read,
        CancellationToken cancellationToken = default);
}
