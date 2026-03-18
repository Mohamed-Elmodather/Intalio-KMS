using AFC27.KMS.Admin.Application.DTOs;

namespace AFC27.KMS.Admin.Application.Interfaces;

/// <summary>
/// Service interface for guest/external access management.
/// </summary>
public interface IGuestAccessService
{
    /// <summary>
    /// Create a guest access invitation.
    /// </summary>
    Task<GuestAccessDto> CreateAsync(
        CreateGuestAccessRequest request,
        Guid grantedByUserId,
        string grantedByName,
        CancellationToken ct = default);

    /// <summary>
    /// Get a guest access by ID.
    /// </summary>
    Task<GuestAccessDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Get a guest access by token.
    /// </summary>
    Task<GuestAccessDto?> GetByTokenAsync(string token, CancellationToken ct = default);

    /// <summary>
    /// List guest access entries with filtering.
    /// </summary>
    Task<PagedResult<GuestAccessDto>> ListAsync(
        GuestAccessFilterRequest filter, CancellationToken ct = default);

    /// <summary>
    /// Revoke a guest access.
    /// </summary>
    Task<bool> RevokeAsync(Guid id, Guid revokedByUserId, CancellationToken ct = default);

    /// <summary>
    /// Extend a guest access expiration.
    /// </summary>
    Task<bool> ExtendAsync(Guid id, ExtendGuestAccessRequest request, CancellationToken ct = default);

    /// <summary>
    /// Record an access event for a guest token.
    /// </summary>
    Task<bool> RecordAccessAsync(string token, CancellationToken ct = default);

    /// <summary>
    /// Validate a guest access token and return the access details if valid.
    /// </summary>
    Task<GuestAccessDto?> ValidateTokenAsync(string token, CancellationToken ct = default);

    /// <summary>
    /// Deactivate expired guest access entries (for background job).
    /// </summary>
    Task<int> DeactivateExpiredAsync(CancellationToken ct = default);
}
