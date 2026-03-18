using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Domain.Entities;

namespace AFC27.KMS.Admin.Application.Interfaces;

/// <summary>
/// Service interface for delegated space administration management.
/// </summary>
public interface IDelegatedAdminService
{
    /// <summary>
    /// Create a delegated admin assignment for a space.
    /// </summary>
    Task<DelegatedAdminDto?> CreateAsync(
        CreateDelegatedAdminRequest request,
        Guid grantedByUserId,
        string grantedByUserName,
        CancellationToken ct = default);

    /// <summary>
    /// Get a delegated admin assignment by ID.
    /// </summary>
    Task<DelegatedAdminDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// List delegated admins for a space.
    /// </summary>
    Task<IReadOnlyList<DelegatedAdminDto>> GetBySpaceAsync(
        Guid spaceId, bool includeRevoked = false, CancellationToken ct = default);

    /// <summary>
    /// List delegated admin assignments for a user.
    /// </summary>
    Task<IReadOnlyList<DelegatedAdminDto>> GetByUserAsync(
        Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Update the scopes of a delegated admin assignment.
    /// </summary>
    Task<bool> UpdateScopesAsync(
        Guid id, UpdateDelegatedAdminScopesRequest request, Guid updatedByUserId, CancellationToken ct = default);

    /// <summary>
    /// Revoke a delegated admin assignment.
    /// </summary>
    Task<bool> RevokeAsync(Guid id, Guid revokedByUserId, CancellationToken ct = default);

    /// <summary>
    /// Check if a user has a specific delegated admin scope on a space.
    /// </summary>
    Task<bool> HasDelegatedScopeAsync(
        Guid spaceId, Guid userId, DelegatedAdminScope scope, CancellationToken ct = default);
}
