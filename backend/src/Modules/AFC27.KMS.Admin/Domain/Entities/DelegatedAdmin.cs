using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Admin.Domain.Entities;

/// <summary>
/// Scopes that can be delegated to space-level administrators.
/// </summary>
[Flags]
public enum DelegatedAdminScope
{
    None = 0,
    ManageMembers = 1,
    ManageSettings = 2,
    ManageContentPermissions = 4,
    ViewAnalytics = 8,
    ManageContent = 16,
    All = ManageMembers | ManageSettings | ManageContentPermissions | ViewAnalytics | ManageContent
}

/// <summary>
/// Status of a delegated administration assignment.
/// </summary>
public enum DelegatedAdminStatus
{
    Active,
    Revoked,
    Expired
}

/// <summary>
/// Represents a delegated administration assignment.
/// Space owners can delegate specific admin capabilities to users for their space,
/// restricted to the space scope only — delegated admins cannot affect global settings.
/// </summary>
public class DelegatedAdmin : AuditableEntity
{
    /// <summary>
    /// The space this delegation applies to.
    /// </summary>
    public Guid SpaceId { get; private set; }

    /// <summary>
    /// The user receiving delegated admin rights.
    /// </summary>
    public Guid DelegateUserId { get; private set; }

    /// <summary>
    /// Display name of the delegated user.
    /// </summary>
    public string DelegateUserName { get; private set; } = string.Empty;

    /// <summary>
    /// The user who granted the delegation (space owner or admin).
    /// </summary>
    public Guid GrantedByUserId { get; private set; }

    /// <summary>
    /// Display name of the granting user.
    /// </summary>
    public string GrantedByUserName { get; private set; } = string.Empty;

    /// <summary>
    /// Bitwise combination of delegated scopes.
    /// </summary>
    public DelegatedAdminScope Scopes { get; private set; }

    /// <summary>
    /// Optional reason or notes for the delegation.
    /// </summary>
    public string? Reason { get; private set; }

    /// <summary>
    /// Current status of the delegation.
    /// </summary>
    public DelegatedAdminStatus Status { get; private set; }

    /// <summary>
    /// Optional expiration date. Null means no expiration.
    /// </summary>
    public DateTime? ExpiresAt { get; private set; }

    /// <summary>
    /// When the delegation was revoked, if applicable.
    /// </summary>
    public DateTime? RevokedAt { get; private set; }

    /// <summary>
    /// Who revoked the delegation, if applicable.
    /// </summary>
    public Guid? RevokedByUserId { get; private set; }

    private DelegatedAdmin() { }

    public static DelegatedAdmin Create(
        Guid spaceId,
        Guid delegateUserId,
        string delegateUserName,
        Guid grantedByUserId,
        string grantedByUserName,
        DelegatedAdminScope scopes,
        string? reason = null,
        DateTime? expiresAt = null)
    {
        return new DelegatedAdmin
        {
            SpaceId = spaceId,
            DelegateUserId = delegateUserId,
            DelegateUserName = delegateUserName,
            GrantedByUserId = grantedByUserId,
            GrantedByUserName = grantedByUserName,
            Scopes = scopes,
            Reason = reason,
            Status = DelegatedAdminStatus.Active,
            ExpiresAt = expiresAt
        };
    }

    /// <summary>
    /// Revoke this delegation.
    /// </summary>
    public void Revoke(Guid revokedByUserId)
    {
        Status = DelegatedAdminStatus.Revoked;
        RevokedAt = DateTime.UtcNow;
        RevokedByUserId = revokedByUserId;
    }

    /// <summary>
    /// Mark as expired (called by background job or on-demand).
    /// </summary>
    public void MarkExpired()
    {
        Status = DelegatedAdminStatus.Expired;
    }

    /// <summary>
    /// Update the scopes of the delegation.
    /// </summary>
    public void UpdateScopes(DelegatedAdminScope newScopes)
    {
        Scopes = newScopes;
    }

    /// <summary>
    /// Check if this delegation grants a specific scope.
    /// </summary>
    public bool HasScope(DelegatedAdminScope scope)
    {
        return Status == DelegatedAdminStatus.Active
            && (!ExpiresAt.HasValue || ExpiresAt.Value > DateTime.UtcNow)
            && Scopes.HasFlag(scope);
    }

    /// <summary>
    /// Whether this delegation is currently effective.
    /// </summary>
    public bool IsEffective =>
        Status == DelegatedAdminStatus.Active
        && (!ExpiresAt.HasValue || ExpiresAt.Value > DateTime.UtcNow);
}
