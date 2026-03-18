namespace AFC27.KMS.Admin.Application.DTOs;

/// <summary>
/// Guest access DTO.
/// </summary>
public record GuestAccessDto
{
    public Guid Id { get; init; }
    public string Token { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public Guid EntityId { get; init; }
    public Guid GrantedById { get; init; }
    public string GrantedByName { get; init; } = string.Empty;
    public string GuestEmail { get; init; } = string.Empty;
    public string? GuestName { get; init; }
    public DateTime ExpiresAt { get; init; }
    public string AccessLevel { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public bool IsValid { get; init; }
    public string? Message { get; init; }
    public int AccessCount { get; init; }
    public DateTime? LastAccessedAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? RevokedAt { get; init; }
}

/// <summary>
/// Request to create a guest access invitation.
/// </summary>
public record CreateGuestAccessRequest
{
    public string EntityType { get; init; } = string.Empty;
    public Guid EntityId { get; init; }
    public string GuestEmail { get; init; } = string.Empty;
    public string? GuestName { get; init; }
    public string AccessLevel { get; init; } = "View";
    public int ExpirationDays { get; init; } = 7;
    public string? Message { get; init; }
}

/// <summary>
/// Request to extend guest access expiration.
/// </summary>
public record ExtendGuestAccessRequest
{
    public int AdditionalDays { get; init; } = 7;
}

/// <summary>
/// Filter for listing guest access entries.
/// </summary>
public record GuestAccessFilterRequest
{
    public string? EntityType { get; init; }
    public Guid? EntityId { get; init; }
    public Guid? GrantedById { get; init; }
    public string? GuestEmail { get; init; }
    public bool? IsActive { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
