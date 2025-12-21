using AFC27.KMS.Admin.Domain.Entities;
using AFC27.KMS.Admin.Application.DTOs;

namespace AFC27.KMS.Admin.Application.Interfaces;

/// <summary>
/// Service interface for audit logging.
/// </summary>
public interface IAuditLogService
{
    /// <summary>
    /// Log an audit entry.
    /// </summary>
    Task LogAsync(AuditLogEntry entry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Log an action with automatic context capture.
    /// </summary>
    Task LogActionAsync(
        string action,
        string category,
        string entityType,
        Guid? entityId = null,
        object? oldValues = null,
        object? newValues = null,
        object? additionalData = null,
        AuditSeverity severity = AuditSeverity.Info,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Log a failed action.
    /// </summary>
    Task LogFailureAsync(
        string action,
        string category,
        string entityType,
        string errorMessage,
        Guid? entityId = null,
        AuditSeverity severity = AuditSeverity.Error,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Search audit logs with filtering.
    /// </summary>
    Task<PagedResult<AuditLogDto>> SearchAsync(
        AuditLogSearchRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get audit history for a specific entity.
    /// </summary>
    Task<IReadOnlyList<AuditLogDto>> GetEntityHistoryAsync(
        string entityType,
        Guid entityId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get audit history for a user.
    /// </summary>
    Task<IReadOnlyList<AuditLogDto>> GetUserActivityAsync(
        Guid userId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Export audit logs to CSV.
    /// </summary>
    Task<byte[]> ExportToCsvAsync(
        AuditLogSearchRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get audit statistics for dashboard.
    /// </summary>
    Task<AuditStatisticsDto> GetStatisticsAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for legal hold management.
/// </summary>
public interface ILegalHoldService
{
    /// <summary>
    /// Create a new legal hold.
    /// </summary>
    Task<LegalHoldDto> CreateAsync(
        CreateLegalHoldRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a legal hold by ID.
    /// </summary>
    Task<LegalHoldDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List all legal holds.
    /// </summary>
    Task<IReadOnlyList<LegalHoldDto>> ListAsync(
        bool includeReleased = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a document to a legal hold.
    /// </summary>
    Task<bool> AddDocumentAsync(
        Guid legalHoldId,
        Guid documentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove a document from a legal hold.
    /// </summary>
    Task<bool> RemoveDocumentAsync(
        Guid legalHoldId,
        Guid documentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a custodian to a legal hold.
    /// </summary>
    Task<bool> AddCustodianAsync(
        Guid legalHoldId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Release a legal hold.
    /// </summary>
    Task<bool> ReleaseAsync(
        Guid id,
        string? releaseNotes = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a document is under legal hold.
    /// </summary>
    Task<bool> IsDocumentUnderHoldAsync(
        Guid documentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all legal holds affecting a document.
    /// </summary>
    Task<IReadOnlyList<LegalHoldDto>> GetHoldsForDocumentAsync(
        Guid documentId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for quarantine management.
/// </summary>
public interface IQuarantineService
{
    /// <summary>
    /// Quarantine a document.
    /// </summary>
    Task<QuarantinedDocumentDto> QuarantineAsync(
        QuarantineDocumentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get quarantined document by ID.
    /// </summary>
    Task<QuarantinedDocumentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List quarantined documents.
    /// </summary>
    Task<PagedResult<QuarantinedDocumentDto>> ListAsync(
        QuarantineListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Approve a quarantined document (delete it).
    /// </summary>
    Task<bool> ApproveAsync(
        Guid id,
        string? notes = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reject quarantine (keep in quarantine).
    /// </summary>
    Task<bool> RejectAsync(
        Guid id,
        string? notes = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Release document from quarantine.
    /// </summary>
    Task<bool> ReleaseAsync(
        Guid id,
        string? notes = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a document is quarantined.
    /// </summary>
    Task<bool> IsQuarantinedAsync(
        Guid documentId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for user lifecycle management.
/// </summary>
public interface IUserLifecycleService
{
    /// <summary>
    /// Invite a new user.
    /// </summary>
    Task<UserDto> InviteUserAsync(
        InviteUserRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Suspend a user.
    /// </summary>
    Task<bool> SuspendUserAsync(
        Guid userId,
        string reason,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reactivate a suspended user.
    /// </summary>
    Task<bool> ReactivateUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a user (soft delete).
    /// </summary>
    Task<bool> DeleteUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update user roles.
    /// </summary>
    Task<bool> UpdateRolesAsync(
        Guid userId,
        IReadOnlyList<Guid> roleIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update user groups.
    /// </summary>
    Task<bool> UpdateGroupsAsync(
        Guid userId,
        IReadOnlyList<Guid> groupIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user activity summary.
    /// </summary>
    Task<UserActivitySummaryDto> GetActivitySummaryAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Bulk invite users.
    /// </summary>
    Task<BulkInviteResultDto> BulkInviteAsync(
        BulkInviteRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for impersonation.
/// </summary>
public interface IImpersonationService
{
    /// <summary>
    /// Start impersonating a user.
    /// </summary>
    Task<ImpersonationSessionDto> StartAsync(
        Guid targetUserId,
        string reason,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// End the current impersonation session.
    /// </summary>
    Task<bool> EndAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get active impersonation session.
    /// </summary>
    Task<ImpersonationSessionDto?> GetActiveSessionAsync(
        Guid adminUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get impersonation history.
    /// </summary>
    Task<IReadOnlyList<ImpersonationSessionDto>> GetHistoryAsync(
        Guid? adminUserId = null,
        Guid? targetUserId = null,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Log an action during impersonation.
    /// </summary>
    Task LogActionAsync(
        Guid sessionId,
        string action,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Generic paged result.
/// </summary>
public record PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}
