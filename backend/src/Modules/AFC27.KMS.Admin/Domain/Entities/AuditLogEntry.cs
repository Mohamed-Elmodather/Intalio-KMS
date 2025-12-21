using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Admin.Domain.Entities;

/// <summary>
/// Represents an audit log entry for tracking user actions and system events.
/// </summary>
public class AuditLogEntry : Entity
{
    public DateTime Timestamp { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public string EntityType { get; private set; } = string.Empty;
    public Guid? EntityId { get; private set; }
    public Guid? UserId { get; private set; }
    public string? UserName { get; private set; }
    public string? UserEmail { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? OldValues { get; private set; } // JSON
    public string? NewValues { get; private set; } // JSON
    public string? AdditionalData { get; private set; } // JSON
    public AuditSeverity Severity { get; private set; }
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? CorrelationId { get; private set; }
    public string? SessionId { get; private set; }

    private AuditLogEntry() { }

    public static AuditLogEntry Create(
        string action,
        string category,
        string entityType,
        Guid? entityId = null,
        Guid? userId = null,
        string? userName = null,
        string? userEmail = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? oldValues = null,
        string? newValues = null,
        string? additionalData = null,
        AuditSeverity severity = AuditSeverity.Info,
        bool isSuccess = true,
        string? errorMessage = null,
        string? correlationId = null,
        string? sessionId = null)
    {
        return new AuditLogEntry
        {
            Timestamp = DateTime.UtcNow,
            Action = action,
            Category = category,
            EntityType = entityType,
            EntityId = entityId,
            UserId = userId,
            UserName = userName,
            UserEmail = userEmail,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            OldValues = oldValues,
            NewValues = newValues,
            AdditionalData = additionalData,
            Severity = severity,
            IsSuccess = isSuccess,
            ErrorMessage = errorMessage,
            CorrelationId = correlationId,
            SessionId = sessionId
        };
    }
}

/// <summary>
/// Audit log severity levels.
/// </summary>
public enum AuditSeverity
{
    Debug = 0,
    Info = 1,
    Warning = 2,
    Error = 3,
    Critical = 4
}

/// <summary>
/// Standard audit action categories.
/// </summary>
public static class AuditCategories
{
    public const string Authentication = "Authentication";
    public const string Authorization = "Authorization";
    public const string UserManagement = "UserManagement";
    public const string DocumentManagement = "DocumentManagement";
    public const string ContentManagement = "ContentManagement";
    public const string SystemConfiguration = "SystemConfiguration";
    public const string DataExport = "DataExport";
    public const string SecurityEvent = "SecurityEvent";
    public const string Compliance = "Compliance";
}

/// <summary>
/// Standard audit actions.
/// </summary>
public static class AuditActions
{
    // Authentication
    public const string Login = "Login";
    public const string Logout = "Logout";
    public const string LoginFailed = "LoginFailed";
    public const string PasswordChanged = "PasswordChanged";
    public const string TokenRefreshed = "TokenRefreshed";

    // User Management
    public const string UserCreated = "UserCreated";
    public const string UserUpdated = "UserUpdated";
    public const string UserDeleted = "UserDeleted";
    public const string UserSuspended = "UserSuspended";
    public const string UserReactivated = "UserReactivated";
    public const string UserInvited = "UserInvited";
    public const string RoleAssigned = "RoleAssigned";
    public const string RoleRevoked = "RoleRevoked";
    public const string ImpersonationStarted = "ImpersonationStarted";
    public const string ImpersonationEnded = "ImpersonationEnded";

    // Documents
    public const string DocumentCreated = "DocumentCreated";
    public const string DocumentUpdated = "DocumentUpdated";
    public const string DocumentDeleted = "DocumentDeleted";
    public const string DocumentViewed = "DocumentViewed";
    public const string DocumentDownloaded = "DocumentDownloaded";
    public const string DocumentCheckedOut = "DocumentCheckedOut";
    public const string DocumentCheckedIn = "DocumentCheckedIn";
    public const string DocumentShared = "DocumentShared";
    public const string DocumentMoved = "DocumentMoved";
    public const string PermissionGranted = "PermissionGranted";
    public const string PermissionRevoked = "PermissionRevoked";

    // Content
    public const string ArticleCreated = "ArticleCreated";
    public const string ArticleUpdated = "ArticleUpdated";
    public const string ArticleDeleted = "ArticleDeleted";
    public const string ArticlePublished = "ArticlePublished";
    public const string ArticleUnpublished = "ArticleUnpublished";

    // Compliance
    public const string LegalHoldApplied = "LegalHoldApplied";
    public const string LegalHoldReleased = "LegalHoldReleased";
    public const string DocumentQuarantined = "DocumentQuarantined";
    public const string DocumentReleased = "DocumentReleased";
    public const string DataExported = "DataExported";
    public const string AuditLogAccessed = "AuditLogAccessed";
}

/// <summary>
/// Legal hold entity for compliance requirements.
/// </summary>
public class LegalHold : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string CaseReference { get; private set; } = string.Empty;
    public Guid CreatedByUserId { get; private set; }
    public string CreatedByName { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public LegalHoldStatus Status { get; private set; }
    public string? Notes { get; private set; }

    // Navigation
    public virtual ICollection<LegalHoldDocument> Documents { get; private set; } = new List<LegalHoldDocument>();
    public virtual ICollection<LegalHoldCustodian> Custodians { get; private set; } = new List<LegalHoldCustodian>();

    private LegalHold() { }

    public static LegalHold Create(
        string name,
        string description,
        string caseReference,
        Guid createdByUserId,
        string createdByName,
        string? notes = null)
    {
        return new LegalHold
        {
            Name = name,
            Description = description,
            CaseReference = caseReference,
            CreatedByUserId = createdByUserId,
            CreatedByName = createdByName,
            StartDate = DateTime.UtcNow,
            Status = LegalHoldStatus.Active,
            Notes = notes
        };
    }

    public void Release(string? releaseNotes = null)
    {
        Status = LegalHoldStatus.Released;
        EndDate = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(releaseNotes))
        {
            Notes = string.IsNullOrEmpty(Notes)
                ? releaseNotes
                : $"{Notes}\n\nRelease Notes: {releaseNotes}";
        }
    }

    public void AddDocument(Guid documentId, string documentName)
    {
        if (!Documents.Any(d => d.DocumentId == documentId))
        {
            Documents.Add(LegalHoldDocument.Create(Id, documentId, documentName));
        }
    }

    public void RemoveDocument(Guid documentId)
    {
        var doc = Documents.FirstOrDefault(d => d.DocumentId == documentId);
        if (doc != null)
        {
            Documents.Remove(doc);
        }
    }

    public void AddCustodian(Guid userId, string userName, string email)
    {
        if (!Custodians.Any(c => c.UserId == userId))
        {
            Custodians.Add(LegalHoldCustodian.Create(Id, userId, userName, email));
        }
    }
}

public enum LegalHoldStatus
{
    Active = 0,
    Released = 1,
    Expired = 2
}

/// <summary>
/// Document under legal hold.
/// </summary>
public class LegalHoldDocument : Entity
{
    public Guid LegalHoldId { get; private set; }
    public Guid DocumentId { get; private set; }
    public string DocumentName { get; private set; } = string.Empty;
    public DateTime AddedAt { get; private set; }

    // Navigation
    public virtual LegalHold LegalHold { get; private set; } = null!;

    private LegalHoldDocument() { }

    public static LegalHoldDocument Create(Guid legalHoldId, Guid documentId, string documentName)
    {
        return new LegalHoldDocument
        {
            LegalHoldId = legalHoldId,
            DocumentId = documentId,
            DocumentName = documentName,
            AddedAt = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Custodian (user) associated with a legal hold.
/// </summary>
public class LegalHoldCustodian : Entity
{
    public Guid LegalHoldId { get; private set; }
    public Guid UserId { get; private set; }
    public string UserName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public DateTime AddedAt { get; private set; }
    public bool IsNotified { get; private set; }
    public DateTime? NotifiedAt { get; private set; }

    // Navigation
    public virtual LegalHold LegalHold { get; private set; } = null!;

    private LegalHoldCustodian() { }

    public static LegalHoldCustodian Create(Guid legalHoldId, Guid userId, string userName, string email)
    {
        return new LegalHoldCustodian
        {
            LegalHoldId = legalHoldId,
            UserId = userId,
            UserName = userName,
            Email = email,
            AddedAt = DateTime.UtcNow
        };
    }

    public void MarkNotified()
    {
        IsNotified = true;
        NotifiedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Quarantined document for security/compliance.
/// </summary>
public class QuarantinedDocument : AuditableEntity
{
    public Guid DocumentId { get; private set; }
    public string DocumentName { get; private set; } = string.Empty;
    public string OriginalPath { get; private set; } = string.Empty;
    public QuarantineReason Reason { get; private set; }
    public string ReasonDetails { get; private set; } = string.Empty;
    public Guid QuarantinedByUserId { get; private set; }
    public string QuarantinedByName { get; private set; } = string.Empty;
    public DateTime QuarantinedAt { get; private set; }
    public QuarantineStatus Status { get; private set; }
    public Guid? ReviewedByUserId { get; private set; }
    public string? ReviewedByName { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public string? ReviewNotes { get; private set; }

    private QuarantinedDocument() { }

    public static QuarantinedDocument Create(
        Guid documentId,
        string documentName,
        string originalPath,
        QuarantineReason reason,
        string reasonDetails,
        Guid quarantinedByUserId,
        string quarantinedByName)
    {
        return new QuarantinedDocument
        {
            DocumentId = documentId,
            DocumentName = documentName,
            OriginalPath = originalPath,
            Reason = reason,
            ReasonDetails = reasonDetails,
            QuarantinedByUserId = quarantinedByUserId,
            QuarantinedByName = quarantinedByName,
            QuarantinedAt = DateTime.UtcNow,
            Status = QuarantineStatus.Pending
        };
    }

    public void Approve(Guid reviewedByUserId, string reviewedByName, string? notes = null)
    {
        Status = QuarantineStatus.Approved;
        ReviewedByUserId = reviewedByUserId;
        ReviewedByName = reviewedByName;
        ReviewedAt = DateTime.UtcNow;
        ReviewNotes = notes;
    }

    public void Reject(Guid reviewedByUserId, string reviewedByName, string? notes = null)
    {
        Status = QuarantineStatus.Rejected;
        ReviewedByUserId = reviewedByUserId;
        ReviewedByName = reviewedByName;
        ReviewedAt = DateTime.UtcNow;
        ReviewNotes = notes;
    }

    public void Release(Guid reviewedByUserId, string reviewedByName, string? notes = null)
    {
        Status = QuarantineStatus.Released;
        ReviewedByUserId = reviewedByUserId;
        ReviewedByName = reviewedByName;
        ReviewedAt = DateTime.UtcNow;
        ReviewNotes = notes;
    }
}

public enum QuarantineReason
{
    MalwareDetected = 0,
    PolicyViolation = 1,
    SensitiveContent = 2,
    CopyrightIssue = 3,
    ManualReview = 4,
    SuspiciousActivity = 5,
    Other = 99
}

public enum QuarantineStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Released = 3
}

/// <summary>
/// Impersonation session for admin troubleshooting.
/// </summary>
public class ImpersonationSession : Entity
{
    public Guid AdminUserId { get; private set; }
    public string AdminUserName { get; private set; } = string.Empty;
    public Guid ImpersonatedUserId { get; private set; }
    public string ImpersonatedUserName { get; private set; } = string.Empty;
    public string Reason { get; private set; } = string.Empty;
    public DateTime StartedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }
    public bool IsActive { get; private set; }
    public string? IpAddress { get; private set; }
    public string? Actions { get; private set; } // JSON array of actions taken

    private ImpersonationSession() { }

    public static ImpersonationSession Create(
        Guid adminUserId,
        string adminUserName,
        Guid impersonatedUserId,
        string impersonatedUserName,
        string reason,
        string? ipAddress = null)
    {
        return new ImpersonationSession
        {
            AdminUserId = adminUserId,
            AdminUserName = adminUserName,
            ImpersonatedUserId = impersonatedUserId,
            ImpersonatedUserName = impersonatedUserName,
            Reason = reason,
            StartedAt = DateTime.UtcNow,
            IsActive = true,
            IpAddress = ipAddress
        };
    }

    public void End()
    {
        IsActive = false;
        EndedAt = DateTime.UtcNow;
    }

    public void LogAction(string action)
    {
        var actions = string.IsNullOrEmpty(Actions)
            ? new List<string>()
            : System.Text.Json.JsonSerializer.Deserialize<List<string>>(Actions) ?? new List<string>();

        actions.Add($"{DateTime.UtcNow:O}: {action}");
        Actions = System.Text.Json.JsonSerializer.Serialize(actions);
    }
}
