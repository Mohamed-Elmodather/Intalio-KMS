using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using AFC27.KMS.WebApi.Data;
using AFC27.KMS.WebApi.Data.Entities;

namespace AFC27.KMS.WebApi.Features.Security.Services;

/// <summary>
/// Implementation of field-level encryption service with database persistence
/// </summary>
public class EncryptionService : IEncryptionService
{
    private readonly KmsDbContext _dbContext;
    private readonly ILogger<EncryptionService> _logger;
    private readonly IConfiguration _configuration;

    // In-memory cache for decrypted keys (cleared on app restart)
    private static readonly Dictionary<string, byte[]> _keyCache = new();
    private static readonly object _lock = new();
    private static string? _activeKeyId;

    // Master key for encrypting/decrypting key material (from config)
    private readonly byte[] _masterKey;

    public EncryptionService(
        KmsDbContext dbContext,
        ILogger<EncryptionService> logger,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _logger = logger;
        _configuration = configuration;

        // Get master key from configuration or generate default for development
        var masterKeyString = _configuration["Encryption:MasterKey"];
        if (!string.IsNullOrEmpty(masterKeyString))
        {
            _masterKey = Convert.FromBase64String(masterKeyString);
        }
        else
        {
            // For development only - in production, this should come from secure config/vault
            _masterKey = SHA256.HashData(Encoding.UTF8.GetBytes("AFC27-KMS-DEV-MASTER-KEY-2027"));
            _logger.LogWarning("Using development master key. Configure Encryption:MasterKey for production.");
        }
    }

    public async Task<string> EncryptAsync(string plainText, string? keyId = null)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        var activeKeyId = keyId ?? await GetActiveKeyIdAsync();
        var key = await GetKeyMaterialAsync(activeKeyId);

        var iv = RandomNumberGenerator.GetBytes(12);
        var tag = new byte[16];

        using var aes = new AesGcm(key, 16);
        var cipherText = new byte[Encoding.UTF8.GetByteCount(plainText)];
        aes.Encrypt(iv, Encoding.UTF8.GetBytes(plainText), cipherText, tag);

        // Format: keyId:iv:tag:ciphertext (all base64)
        var result = $"{activeKeyId}:{Convert.ToBase64String(iv)}:{Convert.ToBase64String(tag)}:{Convert.ToBase64String(cipherText)}";
        return result;
    }

    public async Task<string> DecryptAsync(string cipherText, string? keyId = null)
    {
        if (string.IsNullOrEmpty(cipherText))
            return string.Empty;

        try
        {
            var parts = cipherText.Split(':');
            if (parts.Length != 4)
                throw new ArgumentException("Invalid cipher text format");

            var encKeyId = parts[0];
            var iv = Convert.FromBase64String(parts[1]);
            var tag = Convert.FromBase64String(parts[2]);
            var encrypted = Convert.FromBase64String(parts[3]);

            var key = await GetKeyMaterialAsync(keyId ?? encKeyId);

            using var aes = new AesGcm(key, 16);
            var plainText = new byte[encrypted.Length];
            aes.Decrypt(iv, encrypted, tag, plainText);

            return Encoding.UTF8.GetString(plainText);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to decrypt data");
            throw new CryptographicException("Decryption failed", ex);
        }
    }

    public async Task<byte[]> EncryptBytesAsync(byte[] data, string? keyId = null)
    {
        if (data == null || data.Length == 0)
            return Array.Empty<byte>();

        var activeKeyId = keyId ?? await GetActiveKeyIdAsync();
        var key = await GetKeyMaterialAsync(activeKeyId);

        var iv = RandomNumberGenerator.GetBytes(12);
        var tag = new byte[16];

        using var aes = new AesGcm(key, 16);
        var cipherText = new byte[data.Length];
        aes.Encrypt(iv, data, cipherText, tag);

        // Combine iv + tag + ciphertext
        var result = new byte[iv.Length + tag.Length + cipherText.Length];
        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(tag, 0, result, iv.Length, tag.Length);
        Buffer.BlockCopy(cipherText, 0, result, iv.Length + tag.Length, cipherText.Length);

        return result;
    }

    public async Task<byte[]> DecryptBytesAsync(byte[] encryptedData, string? keyId = null)
    {
        if (encryptedData == null || encryptedData.Length < 28)
            return Array.Empty<byte>();

        var activeKeyId = keyId ?? await GetActiveKeyIdAsync();
        var key = await GetKeyMaterialAsync(activeKeyId);

        var iv = new byte[12];
        var tag = new byte[16];
        var cipherText = new byte[encryptedData.Length - 28];

        Buffer.BlockCopy(encryptedData, 0, iv, 0, 12);
        Buffer.BlockCopy(encryptedData, 12, tag, 0, 16);
        Buffer.BlockCopy(encryptedData, 28, cipherText, 0, cipherText.Length);

        using var aes = new AesGcm(key, 16);
        var plainText = new byte[cipherText.Length];
        aes.Decrypt(iv, cipherText, tag, plainText);

        return plainText;
    }

    public async Task<EncryptionKeyInfo> GenerateKeyAsync(string purpose, DateTime? expiresAt = null)
    {
        var keyId = $"key-{Guid.NewGuid():N}".Substring(0, 16);
        var keyMaterial = RandomNumberGenerator.GetBytes(32);

        // Encrypt key material with master key
        var encryptedKeyMaterial = EncryptWithMasterKey(keyMaterial);

        var maxVersion = await _dbContext.EncryptionKeys
            .Where(k => k.Purpose == purpose)
            .MaxAsync(k => (int?)k.KeyVersion) ?? 0;

        var entity = new EncryptionKeyEntity
        {
            KeyId = keyId,
            Purpose = purpose,
            Algorithm = "AES-256-GCM",
            EncryptedKeyMaterial = encryptedKeyMaterial,
            KeyVersion = maxVersion + 1,
            IsActive = true,
            ExpiresAt = expiresAt
        };

        _dbContext.EncryptionKeys.Add(entity);
        await _dbContext.SaveChangesAsync();

        // Cache the decrypted key
        lock (_lock)
        {
            _keyCache[keyId] = keyMaterial;
        }

        _logger.LogInformation("Generated new encryption key {KeyId} for {Purpose}", keyId, purpose);

        return new EncryptionKeyInfo
        {
            KeyId = keyId,
            Purpose = purpose,
            CreatedAt = entity.CreatedAt,
            ExpiresAt = expiresAt,
            IsActive = true,
            Algorithm = "AES-256-GCM",
            KeyVersion = entity.KeyVersion
        };
    }

    public async Task<EncryptionKeyInfo> RotateKeyAsync(string? oldKeyId = null)
    {
        var newKey = await GenerateKeyAsync("primary", DateTime.UtcNow.AddYears(1));

        // Deactivate old key
        var keyToDeactivate = oldKeyId ?? _activeKeyId;
        if (!string.IsNullOrEmpty(keyToDeactivate))
        {
            var oldKeyEntity = await _dbContext.EncryptionKeys
                .FirstOrDefaultAsync(k => k.KeyId == keyToDeactivate);

            if (oldKeyEntity != null)
            {
                oldKeyEntity.IsActive = false;
                oldKeyEntity.IsRotated = true;
                oldKeyEntity.RotatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
        }

        lock (_lock)
        {
            _activeKeyId = newKey.KeyId;
        }

        _logger.LogInformation("Rotated encryption key. New active key: {KeyId}", newKey.KeyId);
        return newKey;
    }

    public async Task<EncryptionKeyInfo?> GetKeyInfoAsync(string keyId)
    {
        var entity = await _dbContext.EncryptionKeys
            .FirstOrDefaultAsync(k => k.KeyId == keyId);

        if (entity == null) return null;

        return new EncryptionKeyInfo
        {
            KeyId = entity.KeyId,
            Purpose = entity.Purpose,
            CreatedAt = entity.CreatedAt,
            ExpiresAt = entity.ExpiresAt,
            IsActive = entity.IsActive,
            Algorithm = entity.Algorithm,
            KeyVersion = entity.KeyVersion
        };
    }

    public async Task<List<EncryptionKeyInfo>> ListKeysAsync(bool includeExpired = false)
    {
        var query = _dbContext.EncryptionKeys.AsQueryable();

        if (!includeExpired)
            query = query.Where(k => k.ExpiresAt == null || k.ExpiresAt > DateTime.UtcNow);

        var entities = await query
            .OrderByDescending(k => k.CreatedAt)
            .ToListAsync();

        return entities.Select(e => new EncryptionKeyInfo
        {
            KeyId = e.KeyId,
            Purpose = e.Purpose,
            CreatedAt = e.CreatedAt,
            ExpiresAt = e.ExpiresAt,
            IsActive = e.IsActive,
            Algorithm = e.Algorithm,
            KeyVersion = e.KeyVersion
        }).ToList();
    }

    public string HashValue(string value, string? salt = null)
    {
        salt ??= Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
        var combined = $"{salt}:{value}";

        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(combined));
        return $"{salt}:{Convert.ToBase64String(hash)}";
    }

    public bool VerifyHash(string value, string hash, string? salt = null)
    {
        var parts = hash.Split(':');
        if (parts.Length != 2) return false;

        var storedSalt = parts[0];
        var expectedHash = HashValue(value, storedSalt);
        return hash == expectedHash;
    }

    private async Task<string> GetActiveKeyIdAsync()
    {
        if (!string.IsNullOrEmpty(_activeKeyId))
            return _activeKeyId;

        var activeKey = await _dbContext.EncryptionKeys
            .Where(k => k.IsActive && k.Purpose == "primary")
            .OrderByDescending(k => k.KeyVersion)
            .FirstOrDefaultAsync();

        if (activeKey != null)
        {
            lock (_lock)
            {
                _activeKeyId = activeKey.KeyId;
            }
            return activeKey.KeyId;
        }

        // No active key - generate one
        var newKey = await GenerateKeyAsync("primary", DateTime.UtcNow.AddYears(1));
        lock (_lock)
        {
            _activeKeyId = newKey.KeyId;
        }
        return newKey.KeyId;
    }

    private async Task<byte[]> GetKeyMaterialAsync(string keyId)
    {
        // Check cache first
        lock (_lock)
        {
            if (_keyCache.TryGetValue(keyId, out var cachedKey))
                return cachedKey;
        }

        // Load from database
        var entity = await _dbContext.EncryptionKeys
            .FirstOrDefaultAsync(k => k.KeyId == keyId);

        if (entity == null)
            throw new KeyNotFoundException($"Encryption key {keyId} not found");

        // Decrypt key material
        var keyMaterial = DecryptWithMasterKey(entity.EncryptedKeyMaterial);

        // Cache it
        lock (_lock)
        {
            _keyCache[keyId] = keyMaterial;
        }

        return keyMaterial;
    }

    private string EncryptWithMasterKey(byte[] data)
    {
        var iv = RandomNumberGenerator.GetBytes(12);
        var tag = new byte[16];

        using var aes = new AesGcm(_masterKey, 16);
        var cipherText = new byte[data.Length];
        aes.Encrypt(iv, data, cipherText, tag);

        var result = new byte[iv.Length + tag.Length + cipherText.Length];
        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(tag, 0, result, iv.Length, tag.Length);
        Buffer.BlockCopy(cipherText, 0, result, iv.Length + tag.Length, cipherText.Length);

        return Convert.ToBase64String(result);
    }

    private byte[] DecryptWithMasterKey(string encryptedData)
    {
        var data = Convert.FromBase64String(encryptedData);

        var iv = new byte[12];
        var tag = new byte[16];
        var cipherText = new byte[data.Length - 28];

        Buffer.BlockCopy(data, 0, iv, 0, 12);
        Buffer.BlockCopy(data, 12, tag, 0, 16);
        Buffer.BlockCopy(data, 28, cipherText, 0, cipherText.Length);

        using var aes = new AesGcm(_masterKey, 16);
        var plainText = new byte[cipherText.Length];
        aes.Decrypt(iv, cipherText, tag, plainText);

        return plainText;
    }
}
