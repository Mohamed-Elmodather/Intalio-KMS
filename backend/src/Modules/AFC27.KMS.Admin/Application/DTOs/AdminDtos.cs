using AFC27.KMS.Admin.Domain.Entities;

namespace AFC27.KMS.Admin.Application.DTOs;

/// <summary>
/// Audit log DTO.
/// </summary>
public record AuditLogDto
{
    public Guid Id { get; init; }
    public DateTime Timestamp { get; init; }
    public string Action { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public Guid? EntityId { get; init; }
    public Guid? UserId { get; init; }
    public string? UserName { get; init; }
    public string? UserEmail { get; init; }
    public string? IpAddress { get; init; }
    public string Severity { get; init; } = string.Empty;
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public string? OldValues { get; init; }
    public string? NewValues { get; init; }
    public string? AdditionalData { get; init; }
}

/// <summary>
/// Audit log search request.
/// </summary>
public record AuditLogSearchRequest
{
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
    public string? Action { get; init; }
    public string? Category { get; init; }
    public string? EntityType { get; init; }
    public Guid? EntityId { get; init; }
    public Guid? UserId { get; init; }
    public string? SearchText { get; init; }
    public AuditSeverity? MinSeverity { get; init; }
    public bool? IsSuccess { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
    public string? SortBy { get; init; } = "Timestamp";
    public bool SortDescending { get; init; } = true;
}

/// <summary>
/// Audit statistics DTO.
/// </summary>
public record AuditStatisticsDto
{
    public int TotalEvents { get; init; }
    public int SuccessfulEvents { get; init; }
    public int FailedEvents { get; init; }
    public Dictionary<string, int> EventsByCategory { get; init; } = new();
    public Dictionary<string, int> EventsByAction { get; init; } = new();
    public Dictionary<string, int> EventsBySeverity { get; init; } = new();
    public IReadOnlyList<DailyAuditCountDto> DailyBreakdown { get; init; } = Array.Empty<DailyAuditCountDto>();
    public IReadOnlyList<TopUserActivityDto> TopActiveUsers { get; init; } = Array.Empty<TopUserActivityDto>();
}

public record DailyAuditCountDto
{
    public DateTime Date { get; init; }
    public int Count { get; init; }
}

public record TopUserActivityDto
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public int ActionCount { get; init; }
}

/// <summary>
/// Legal hold DTO.
/// </summary>
public record LegalHoldDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string CaseReference { get; init; } = string.Empty;
    public Guid CreatedByUserId { get; init; }
    public string CreatedByName { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public int DocumentCount { get; init; }
    public int CustodianCount { get; init; }
    public IReadOnlyList<LegalHoldDocumentDto> Documents { get; init; } = Array.Empty<LegalHoldDocumentDto>();
    public IReadOnlyList<LegalHoldCustodianDto> Custodians { get; init; } = Array.Empty<LegalHoldCustodianDto>();
}

public record LegalHoldDocumentDto
{
    public Guid DocumentId { get; init; }
    public string DocumentName { get; init; } = string.Empty;
    public DateTime AddedAt { get; init; }
}

public record LegalHoldCustodianDto
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime AddedAt { get; init; }
    public bool IsNotified { get; init; }
    public DateTime? NotifiedAt { get; init; }
}

/// <summary>
/// Create legal hold request.
/// </summary>
public record CreateLegalHoldRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string CaseReference { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public IReadOnlyList<Guid>? InitialDocumentIds { get; init; }
    public IReadOnlyList<Guid>? InitialCustodianIds { get; init; }
}

/// <summary>
/// Quarantined document DTO.
/// </summary>
public record QuarantinedDocumentDto
{
    public Guid Id { get; init; }
    public Guid DocumentId { get; init; }
    public string DocumentName { get; init; } = string.Empty;
    public string OriginalPath { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
    public string ReasonDetails { get; init; } = string.Empty;
    public Guid QuarantinedByUserId { get; init; }
    public string QuarantinedByName { get; init; } = string.Empty;
    public DateTime QuarantinedAt { get; init; }
    public string Status { get; init; } = string.Empty;
    public Guid? ReviewedByUserId { get; init; }
    public string? ReviewedByName { get; init; }
    public DateTime? ReviewedAt { get; init; }
    public string? ReviewNotes { get; init; }
}

/// <summary>
/// Quarantine document request.
/// </summary>
public record QuarantineDocumentRequest
{
    public Guid DocumentId { get; init; }
    public QuarantineReason Reason { get; init; }
    public string ReasonDetails { get; init; } = string.Empty;
}

/// <summary>
/// Quarantine list request.
/// </summary>
public record QuarantineListRequest
{
    public QuarantineStatus? Status { get; init; }
    public QuarantineReason? Reason { get; init; }
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

/// <summary>
/// User DTO.
/// </summary>
public record UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? DisplayNameArabic { get; init; }
    public string? AvatarUrl { get; init; }
    public string? JobTitle { get; init; }
    public string? Department { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> Groups { get; init; } = Array.Empty<string>();
}

/// <summary>
/// Invite user request.
/// </summary>
public record InviteUserRequest
{
    public string Email { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? DisplayNameArabic { get; init; }
    public string? JobTitle { get; init; }
    public Guid? DepartmentId { get; init; }
    public IReadOnlyList<Guid>? RoleIds { get; init; }
    public IReadOnlyList<Guid>? GroupIds { get; init; }
    public bool SendInviteEmail { get; init; } = true;
}

/// <summary>
/// Bulk invite request.
/// </summary>
public record BulkInviteRequest
{
    public IReadOnlyList<InviteUserRequest> Users { get; init; } = Array.Empty<InviteUserRequest>();
    public bool SendInviteEmails { get; init; } = true;
}

/// <summary>
/// Bulk invite result.
/// </summary>
public record BulkInviteResultDto
{
    public int TotalRequested { get; init; }
    public int SuccessCount { get; init; }
    public int FailureCount { get; init; }
    public IReadOnlyList<UserDto> CreatedUsers { get; init; } = Array.Empty<UserDto>();
    public IReadOnlyList<BulkInviteErrorDto> Errors { get; init; } = Array.Empty<BulkInviteErrorDto>();
}

public record BulkInviteErrorDto
{
    public string Email { get; init; } = string.Empty;
    public string Error { get; init; } = string.Empty;
}

/// <summary>
/// User activity summary.
/// </summary>
public record UserActivitySummaryDto
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public DateTime? LastLoginAt { get; init; }
    public int DocumentsCreated { get; init; }
    public int DocumentsViewed { get; init; }
    public int DocumentsEdited { get; init; }
    public int ArticlesCreated { get; init; }
    public int TotalActions { get; init; }
    public IReadOnlyList<RecentActivityDto> RecentActivity { get; init; } = Array.Empty<RecentActivityDto>();
}

public record RecentActivityDto
{
    public DateTime Timestamp { get; init; }
    public string Action { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public string? EntityName { get; init; }
}

/// <summary>
/// Impersonation session DTO.
/// </summary>
public record ImpersonationSessionDto
{
    public Guid Id { get; init; }
    public Guid AdminUserId { get; init; }
    public string AdminUserName { get; init; } = string.Empty;
    public Guid ImpersonatedUserId { get; init; }
    public string ImpersonatedUserName { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
    public DateTime StartedAt { get; init; }
    public DateTime? EndedAt { get; init; }
    public bool IsActive { get; init; }
    public string? IpAddress { get; init; }
    public IReadOnlyList<string>? Actions { get; init; }
}

/// <summary>
/// Dashboard statistics DTO.
/// </summary>
public record DashboardStatsDto
{
    public int TotalUsers { get; init; }
    public int ActiveUsers { get; init; }
    public int SuspendedUsers { get; init; }
    public int TotalDocuments { get; init; }
    public int TotalArticles { get; init; }
    public long TotalStorageUsed { get; init; }
    public int ActiveLegalHolds { get; init; }
    public int PendingQuarantined { get; init; }
    public int TodayAuditEvents { get; init; }
    public IReadOnlyList<ActivityTrendDto> UserActivityTrend { get; init; } = Array.Empty<ActivityTrendDto>();
    public IReadOnlyList<StorageTrendDto> StorageTrend { get; init; } = Array.Empty<StorageTrendDto>();
}

public record ActivityTrendDto
{
    public DateTime Date { get; init; }
    public int ActiveUsers { get; init; }
    public int Actions { get; init; }
}

public record StorageTrendDto
{
    public DateTime Date { get; init; }
    public long UsedBytes { get; init; }
}
