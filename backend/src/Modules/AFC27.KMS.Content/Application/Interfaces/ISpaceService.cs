using AFC27.KMS.Content.Application.DTOs;
using AFC27.KMS.Contracts.Common;

namespace AFC27.KMS.Content.Application.Interfaces;

/// <summary>
/// Service interface for Space/Teamspace management.
/// </summary>
public interface ISpaceService
{
    Task<PagedResult<SpaceSummaryDto>> GetSpacesAsync(
        SpaceFilterRequest filter, CancellationToken ct = default);

    Task<SpaceDto?> GetSpaceByIdAsync(Guid id, CancellationToken ct = default);

    Task<SpaceDto> CreateSpaceAsync(
        CreateSpaceRequest request, Guid userId, string userName, CancellationToken ct = default);

    Task<bool> UpdateSpaceAsync(
        Guid id, UpdateSpaceRequest request, Guid userId, CancellationToken ct = default);

    Task<bool> DeleteSpaceAsync(Guid id, Guid userId, CancellationToken ct = default);

    Task<bool> ArchiveSpaceAsync(Guid id, Guid userId, CancellationToken ct = default);

    Task<bool> UnarchiveSpaceAsync(Guid id, Guid userId, CancellationToken ct = default);

    Task<SpaceMemberDto?> AddMemberAsync(
        Guid spaceId, AddSpaceMemberRequest request, Guid userId, CancellationToken ct = default);

    Task<bool> RemoveMemberAsync(
        Guid spaceId, Guid memberUserId, Guid userId, CancellationToken ct = default);

    Task<bool> ChangeMemberRoleAsync(
        Guid spaceId, Guid memberUserId, ChangeSpaceMemberRoleRequest request, Guid userId, CancellationToken ct = default);

    Task<PagedResult<ArticleSummaryDto>> GetSpaceContentAsync(
        Guid spaceId, int page, int pageSize, CancellationToken ct = default);

    Task<IReadOnlyList<SpaceSummaryDto>> GetMySpacesAsync(
        Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Get or create the personal workspace for a user.
    /// Auto-creates a Space with SpaceType.Personal if it does not exist.
    /// Personal spaces are private by default and not visible in the space directory.
    /// </summary>
    Task<SpaceDto> GetOrCreateMyWorkspaceAsync(
        Guid userId, string userName, CancellationToken ct = default);
}
