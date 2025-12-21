namespace AFC27.KMS.SharedKernel.Interfaces;

/// <summary>
/// Interface for accessing the current authenticated user context.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Gets the current user's ID.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Gets the current user's email.
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Gets the current user's display name.
    /// </summary>
    string? DisplayName { get; }

    /// <summary>
    /// Gets the current user's preferred language (en/ar).
    /// </summary>
    string Language { get; }

    /// <summary>
    /// Checks if the user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the current user's roles.
    /// </summary>
    IEnumerable<string> Roles { get; }

    /// <summary>
    /// Gets the current user's permissions.
    /// </summary>
    IEnumerable<string> Permissions { get; }

    /// <summary>
    /// Checks if the user has a specific role.
    /// </summary>
    bool IsInRole(string role);

    /// <summary>
    /// Checks if the user has a specific permission.
    /// </summary>
    bool HasPermission(string permission);
}
