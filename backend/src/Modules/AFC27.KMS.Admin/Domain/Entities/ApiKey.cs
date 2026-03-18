using System.Security.Cryptography;
using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Admin.Domain.Entities;

/// <summary>
/// API key for external integrations. Keys are stored as SHA-256 hashes.
/// </summary>
public class ApiKey : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string KeyHash { get; private set; } = string.Empty;
    public string KeyPrefix { get; private set; } = string.Empty; // First 8 chars for identification
    public List<string> Scopes { get; private set; } = new();
    public DateTime? ExpiresAt { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime? LastUsedAt { get; private set; }
    public int TotalUsageCount { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public string CreatedByName { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    private ApiKey() { }

    /// <summary>
    /// Create a new API key. Returns the key entity and the plain-text key (shown once).
    /// </summary>
    public static (ApiKey Entity, string PlainTextKey) Create(
        string name,
        List<string> scopes,
        Guid createdByUserId,
        string createdByName,
        DateTime? expiresAt = null,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        var plainTextKey = GenerateKey();
        var keyHash = HashKey(plainTextKey);

        var entity = new ApiKey
        {
            Name = name,
            KeyHash = keyHash,
            KeyPrefix = plainTextKey.Substring(0, Math.Min(8, plainTextKey.Length)),
            Scopes = scopes ?? new List<string>(),
            ExpiresAt = expiresAt,
            IsActive = true,
            CreatedByUserId = createdByUserId,
            CreatedByName = createdByName,
            Description = description,
            TotalUsageCount = 0
        };

        return (entity, plainTextKey);
    }

    public void Update(string name, List<string>? scopes, DateTime? expiresAt, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        Name = name;
        Scopes = scopes ?? Scopes;
        ExpiresAt = expiresAt;
        Description = description;
    }

    public void Activate() => IsActive = true;
    public void Revoke() => IsActive = false;

    public void RecordUsage()
    {
        LastUsedAt = DateTime.UtcNow;
        TotalUsageCount++;
    }

    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
    public bool IsValid => IsActive && !IsExpired;

    public bool HasScope(string scope)
    {
        if (Scopes.Count == 0) return true; // No scopes = all access
        return Scopes.Any(s =>
            s == "*" ||
            s.Equals(scope, StringComparison.OrdinalIgnoreCase));
    }

    public static bool VerifyKey(string plainTextKey, string storedHash)
    {
        return HashKey(plainTextKey) == storedHash;
    }

    private static string GenerateKey()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return "afc27_" + Convert.ToBase64String(bytes).Replace("+", "").Replace("/", "").Replace("=", "").Substring(0, 40);
    }

    private static string HashKey(string key)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(key);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}
