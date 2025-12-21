using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.WebApi.Data.Entities;

/// <summary>
/// Encryption key entity for key management
/// </summary>
public class EncryptionKeyEntity : AuditableEntity
{
    public string KeyId { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty; // FieldEncryption, DocumentEncryption, etc.
    public string Algorithm { get; set; } = "AES-256-GCM";
    public string EncryptedKeyMaterial { get; set; } = string.Empty; // Key encrypted with master key
    public int KeyVersion { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public bool IsRotated { get; set; }
    public string? RotatedFromKeyId { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? RotatedAt { get; set; }
    public Guid? RotatedById { get; set; }
    public string? RotatedByName { get; set; }
    public string? MetadataJson { get; set; }
}

/// <summary>
/// Encryption audit log for compliance
/// </summary>
public class EncryptionAuditLogEntity : AuditableEntity
{
    public string KeyId { get; set; } = string.Empty;
    public EncryptionOperationType Operation { get; set; }
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string? FieldName { get; set; }
    public Guid PerformedById { get; set; }
    public string PerformedByName { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Encrypted field reference for tracking encrypted data
/// </summary>
public class EncryptedFieldReferenceEntity : AuditableEntity
{
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string KeyId { get; set; } = string.Empty;
    public int KeyVersion { get; set; }
    public DateTime EncryptedAt { get; set; } = DateTime.UtcNow;
    public bool NeedsReEncryption { get; set; }
}

// Enums
public enum EncryptionOperationType
{
    Encrypt,
    Decrypt,
    KeyGeneration,
    KeyRotation,
    KeyRevocation
}
