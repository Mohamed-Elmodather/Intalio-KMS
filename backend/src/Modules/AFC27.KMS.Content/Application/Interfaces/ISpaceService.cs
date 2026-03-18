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
}
