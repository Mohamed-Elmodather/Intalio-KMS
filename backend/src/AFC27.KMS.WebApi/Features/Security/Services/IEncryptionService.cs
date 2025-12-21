namespace AFC27.KMS.WebApi.Features.Security.Services;

/// <summary>
/// Interface for field-level encryption service
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypts a string value
    /// </summary>
    Task<string> EncryptAsync(string plainText, string? keyId = null);

    /// <summary>
    /// Decrypts an encrypted string
    /// </summary>
    Task<string> DecryptAsync(string cipherText, string? keyId = null);

    /// <summary>
    /// Encrypts binary data
    /// </summary>
    Task<byte[]> EncryptBytesAsync(byte[] data, string? keyId = null);

    /// <summary>
    /// Decrypts binary data
    /// </summary>
    Task<byte[]> DecryptBytesAsync(byte[] encryptedData, string? keyId = null);

    /// <summary>
    /// Generates a new encryption key
    /// </summary>
    Task<EncryptionKeyInfo> GenerateKeyAsync(string purpose, DateTime? expiresAt = null);

    /// <summary>
    /// Rotates the active encryption key
    /// </summary>
    Task<EncryptionKeyInfo> RotateKeyAsync(string? oldKeyId = null);

    /// <summary>
    /// Gets encryption key information
    /// </summary>
    Task<EncryptionKeyInfo?> GetKeyInfoAsync(string keyId);

    /// <summary>
    /// Lists all encryption keys
    /// </summary>
    Task<List<EncryptionKeyInfo>> ListKeysAsync(bool includeExpired = false);

    /// <summary>
    /// Hashes a value for secure storage
    /// </summary>
    string HashValue(string value, string? salt = null);

    /// <summary>
    /// Verifies a hashed value
    /// </summary>
    bool VerifyHash(string value, string hash, string? salt = null);
}

public class EncryptionKeyInfo
{
    public string KeyId { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public string Algorithm { get; set; } = "AES-256-GCM";
    public int KeyVersion { get; set; }
}
