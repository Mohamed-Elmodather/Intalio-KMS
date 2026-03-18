using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Admin.Domain.Entities;

/// <summary>
/// Access level for guest/external users.
/// </summary>
public enum GuestAccessLevel
{
    View,
    Comment,
    Edit
}

/// <summary>
/// Types of entities that can be shared with guests.
/// </summary>
public enum GuestEntityType
{
    Space,
    Article,
    Document,
    DocumentLibrary
}

/// <summary>
/// Represents a time-limited access token for external/guest users.
/// Enables sharing specific content with people outside the organization
/// without requiring full user accounts.
/// </summary>
public class GuestAccess : AuditableEntity
{
    /// <summary>
    /// Unique access token used by the guest to authenticate.
    /// </summary>
    public string Token { get; private set; } = string.Empty;

    /// <summary>
    /// The type of entity being shared.
    /// </summary>
    public GuestEntityType EntityType { get; private set; }

    /// <summary>
    /// The ID of the entity being shared.
    /// </summary>
    public Guid EntityId { get; private set; }

    /// <summary>
    /// The user who granted guest access.
    /// </summary>
    public Guid GrantedById { get; private set; }

    /// <summary>
    /// Display name of the granting user.
    /// </summary>
    public string GrantedByName { get; private set; } = string.Empty;

    /// <summary>
    /// Email of the guest receiving access (for tracking and notifications).
    /// </summary>
    public string GuestEmail { get; private set; } = string.Empty;

    /// <summary>
    /// Display name of the guest (optional).
    /// </summary>
    public string? GuestName { get; private set; }

    /// <summary>
    /// When this guest access expires.
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// The level of access granted to the guest.
    /// </summary>
    public GuestAccessLevel AccessLevel { get; private set; }

    /// <summary>
    /// Whether this guest access is currently active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Optional message included in the invitation.
    /// </summary>
    public string? Message { get; private set; }

    /// <summary>
    /// Number of times the guest has accessed the content.
    /// </summary>
    public int AccessCount { get; private set; }

    /// <summary>
    /// Last time the guest accessed the content.
    /// </summary>
    public DateTime? LastAccessedAt { get; private set; }

    /// <summary>
    /// When the access was revoked, if applicable.
    /// </summary>
    public DateTime? RevokedAt { get; private set; }

    /// <summary>
    /// Who revoked the access, if applicable.
    /// </summary>
    public Guid? RevokedByUserId { get; private set; }

    private GuestAccess() { }

    public static GuestAccess Create(
        GuestEntityType entityType,
        Guid entityId,
        Guid grantedById,
        string grantedByName,
        string guestEmail,
        GuestAccessLevel accessLevel,
        DateTime expiresAt,
        string? guestName = null,
        string? message = null)
    {
        return new GuestAccess
        {
            Token = GenerateSecureToken(),
            EntityType = entityType,
            EntityId = entityId,
            GrantedById = grantedById,
            GrantedByName = grantedByName,
            GuestEmail = guestEmail,
            GuestName = guestName,
            ExpiresAt = expiresAt,
            AccessLevel = accessLevel,
            IsActive = true,
            Message = message
        };
    }

    /// <summary>
    /// Record an access event by the guest.
    /// </summary>
    public void RecordAccess()
    {
        AccessCount++;
        LastAccessedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Revoke this guest access.
    /// </summary>
    public void Revoke(Guid revokedByUserId)
    {
        IsActive = false;
        RevokedAt = DateTime.UtcNow;
        RevokedByUserId = revokedByUserId;
    }

    /// <summary>
    /// Deactivate (e.g., when expired).
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Extend the expiration date.
    /// </summary>
    public void ExtendExpiration(DateTime newExpiresAt)
    {
        if (newExpiresAt > ExpiresAt)
            ExpiresAt = newExpiresAt;
    }

    /// <summary>
    /// Whether this access is currently valid (active and not expired).
    /// </summary>
    public bool IsValid => IsActive && ExpiresAt > DateTime.UtcNow;

    /// <summary>
    /// Generates a cryptographically secure URL-safe token.
    /// </summary>
    private static string GenerateSecureToken()
    {
        var bytes = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }
}
