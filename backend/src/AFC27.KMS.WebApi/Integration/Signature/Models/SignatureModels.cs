using System;
using System.Collections.Generic;

namespace AFC27.KMS.WebApi.Integration.Signature.Models;

/// <summary>
/// Request to create a signature request
/// </summary>
public class SignatureRequestModel
{
    public Guid DocumentId { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public string DocumentUrl { get; set; } = string.Empty;
    public byte[]? DocumentContent { get; set; }
    public List<SignerInfo> Signers { get; set; } = new();
    public SignatureType SignatureType { get; set; } = SignatureType.Electronic;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public string CallbackUrl { get; set; } = string.Empty;
    public SignatureOptions Options { get; set; } = new();
}

/// <summary>
/// Information about a signer
/// </summary>
public class SignerInfo
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public int SigningOrder { get; set; } = 1;
    public SignerRole Role { get; set; } = SignerRole.Signer;
    public List<SignatureField>? SignatureFields { get; set; }
}

/// <summary>
/// Signature field placement
/// </summary>
public class SignatureField
{
    public int PageNumber { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public FieldType FieldType { get; set; } = FieldType.Signature;
    public bool IsRequired { get; set; } = true;
}

public enum FieldType
{
    Signature,
    Initials,
    DateSigned,
    Text,
    Checkbox
}

public enum SignerRole
{
    Signer,
    Approver,
    CarbonCopy,
    Witness
}

public enum SignatureType
{
    Electronic,
    Digital,
    Advanced
}

/// <summary>
/// Signature options
/// </summary>
public class SignatureOptions
{
    public bool RequireAuthentication { get; set; } = false;
    public AuthenticationMethod AuthenticationMethod { get; set; } = AuthenticationMethod.Email;
    public bool AllowDecline { get; set; } = true;
    public bool AllowReassign { get; set; } = false;
    public bool SendReminders { get; set; } = true;
    public int ReminderIntervalDays { get; set; } = 3;
    public string Language { get; set; } = "en";
}

public enum AuthenticationMethod
{
    None,
    Email,
    SMS,
    KnowledgeBased,
    Certificate
}

/// <summary>
/// Response when creating a signature request
/// </summary>
public class SignatureRequestResponse
{
    public string RequestId { get; set; } = string.Empty;
    public SignatureRequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public List<SignerStatus> SignerStatuses { get; set; } = new();
}

public enum SignatureRequestStatus
{
    Created,
    Sent,
    InProgress,
    Completed,
    Declined,
    Expired,
    Cancelled
}

/// <summary>
/// Status of individual signer
/// </summary>
public class SignerStatus
{
    public string SignerId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public SignerSignatureStatus Status { get; set; }
    public DateTime? SignedAt { get; set; }
    public DateTime? ViewedAt { get; set; }
    public string? SigningUrl { get; set; }
}

public enum SignerSignatureStatus
{
    Pending,
    Sent,
    Viewed,
    Signed,
    Declined,
    Expired
}

/// <summary>
/// Response when getting signing URL
/// </summary>
public class SigningUrlResponse
{
    public string SigningUrl { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string SignerId { get; set; } = string.Empty;
}

/// <summary>
/// Signature verification request
/// </summary>
public class SignatureVerificationRequest
{
    public Guid DocumentId { get; set; }
    public string DocumentUrl { get; set; } = string.Empty;
    public byte[]? DocumentContent { get; set; }
}

/// <summary>
/// Signature verification result
/// </summary>
public class SignatureVerificationResult
{
    public bool IsValid { get; set; }
    public bool HasSignatures { get; set; }
    public int SignatureCount { get; set; }
    public List<SignatureInfo> Signatures { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}

/// <summary>
/// Information about a signature
/// </summary>
public class SignatureInfo
{
    public string SignerName { get; set; } = string.Empty;
    public string SignerEmail { get; set; } = string.Empty;
    public DateTime SignedAt { get; set; }
    public bool IsValid { get; set; }
    public string? CertificateSubject { get; set; }
    public string? CertificateIssuer { get; set; }
    public DateTime? CertificateValidFrom { get; set; }
    public DateTime? CertificateValidTo { get; set; }
    public SignatureType SignatureType { get; set; }
}

/// <summary>
/// Webhook payload for signature events
/// </summary>
public class SignatureWebhookPayload
{
    public string RequestId { get; set; } = string.Empty;
    public string DocumentId { get; set; } = string.Empty;
    public string Event { get; set; } = string.Empty;
    public SignatureRequestStatus Status { get; set; }
    public SignerInfo? Signer { get; set; }
    public DateTime Timestamp { get; set; }
    public string Signature { get; set; } = string.Empty;
}
