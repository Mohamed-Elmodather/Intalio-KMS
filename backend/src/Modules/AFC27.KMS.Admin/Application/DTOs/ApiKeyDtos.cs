namespace AFC27.KMS.Admin.Application.DTOs;

/// <summary>
/// API key response DTO. Never includes the full key or hash.
/// </summary>
public record ApiKeyDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string KeyPrefix { get; init; } = string.Empty;
    public IReadOnlyList<string> Scopes { get; init; } = Array.Empty<string>();
    public DateTime? ExpiresAt { get; init; }
    public bool IsActive { get; init; }
    public DateTime? LastUsedAt { get; init; }
    public int TotalUsageCount { get; init; }
    public Guid CreatedByUserId { get; init; }
    public string CreatedByName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsExpired { get; init; }
}

/// <summary>
/// Response after creating an API key. Includes the plain-text key (shown only once).
/// </summary>
public record ApiKeyCreatedDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Key { get; init; } = string.Empty; // Shown only on creation
    public string KeyPrefix { get; init; } = string.Empty;
    public IReadOnlyList<string> Scopes { get; init; } = Array.Empty<string>();
    public DateTime? ExpiresAt { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Create API key request.
/// </summary>
public record CreateApiKeyRequest
{
    public string Name { get; init; } = string.Empty;
    public IReadOnlyList<string>? Scopes { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public string? Description { get; init; }
}

/// <summary>
/// Update API key request.
/// </summary>
public record UpdateApiKeyRequest
{
    public string Name { get; init; } = string.Empty;
    public IReadOnlyList<string>? Scopes { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public string? Description { get; init; }
}
