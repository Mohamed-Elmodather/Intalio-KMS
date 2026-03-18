namespace AFC27.KMS.Admin.Application.DTOs;

/// <summary>
/// Delegated administration DTO.
/// </summary>
public record DelegatedAdminDto
{
    public Guid Id { get; init; }
    public Guid SpaceId { get; init; }
    public Guid DelegateUserId { get; init; }
    public string DelegateUserName { get; init; } = string.Empty;
    public Guid GrantedByUserId { get; init; }
    public string GrantedByUserName { get; init; } = string.Empty;
    public IReadOnlyList<string> Scopes { get; init; } = Array.Empty<string>();
    public string? Reason { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime? ExpiresAt { get; init; }
    public bool IsEffective { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? RevokedAt { get; init; }
}

/// <summary>
/// Request to create a delegated admin assignment.
/// </summary>
public record CreateDelegatedAdminRequest
{
    public Guid SpaceId { get; init; }
    public Guid DelegateUserId { get; init; }
    public string DelegateUserName { get; init; } = string.Empty;
    public IReadOnlyList<string> Scopes { get; init; } = Array.Empty<string>();
    public string? Reason { get; init; }
    public DateTime? ExpiresAt { get; init; }
}

/// <summary>
/// Request to update scopes of a delegated admin.
/// </summary>
public record UpdateDelegatedAdminScopesRequest
{
    public IReadOnlyList<string> Scopes { get; init; } = Array.Empty<string>();
}
