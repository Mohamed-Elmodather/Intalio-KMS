namespace AFC27.KMS.Admin.Application.DTOs;

/// <summary>
/// Compliance dashboard overview.
/// </summary>
public record ComplianceDashboardDto
{
    public ComplianceScoreDto OverallScore { get; init; } = new();
    public PolicyComplianceDto PolicyCompliance { get; init; } = new();
    public ContentReviewStatusDto ContentReviewStatus { get; init; } = new();
    public AccessAuditSummaryDto AccessAuditSummary { get; init; } = new();
    public DataRetentionStatusDto DataRetentionStatus { get; init; } = new();
    public IReadOnlyList<ComplianceAlertDto> RecentAlerts { get; init; } = Array.Empty<ComplianceAlertDto>();
}

/// <summary>
/// Overall compliance score.
/// </summary>
public record ComplianceScoreDto
{
    public double Score { get; init; } // 0-100
    public string Grade { get; init; } = string.Empty; // A, B, C, D, F
    public DateTime CalculatedAt { get; init; }
    public IReadOnlyList<ComplianceAreaScoreDto> AreaScores { get; init; } = Array.Empty<ComplianceAreaScoreDto>();
}

public record ComplianceAreaScoreDto
{
    public string Area { get; init; } = string.Empty;
    public double Score { get; init; }
    public int TotalChecks { get; init; }
    public int PassedChecks { get; init; }
    public int FailedChecks { get; init; }
}

/// <summary>
/// Policy compliance status.
/// </summary>
public record PolicyComplianceDto
{
    public int TotalPolicies { get; init; }
    public int ActivePolicies { get; init; }
    public int ExpiredPolicies { get; init; }
    public int PoliciesRequiringAcknowledgment { get; init; }
    public double AcknowledgmentRate { get; init; } // Percentage
    public IReadOnlyList<PolicyComplianceItemDto> Policies { get; init; } = Array.Empty<PolicyComplianceItemDto>();
}

public record PolicyComplianceItemDto
{
    public Guid PolicyId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int TotalUsers { get; init; }
    public int AcknowledgedUsers { get; init; }
    public double ComplianceRate { get; init; }
    public DateTime? ExpiryDate { get; init; }
}

/// <summary>
/// Content review status.
/// </summary>
public record ContentReviewStatusDto
{
    public int TotalContentItems { get; init; }
    public int PendingReview { get; init; }
    public int ApprovedContent { get; init; }
    public int RejectedContent { get; init; }
    public int QuarantinedContent { get; init; }
    public int ContentUnderLegalHold { get; init; }
    public double ReviewCompletionRate { get; init; }
    public IReadOnlyList<ContentReviewItemDto> PendingItems { get; init; } = Array.Empty<ContentReviewItemDto>();
}

public record ContentReviewItemDto
{
    public Guid ContentId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime SubmittedAt { get; init; }
    public string? SubmittedBy { get; init; }
    public int DaysPending { get; init; }
}

/// <summary>
/// Access audit summary.
/// </summary>
public record AccessAuditSummaryDto
{
    public int TotalAccessEvents { get; init; }
    public int UnauthorizedAttempts { get; init; }
    public int SuspiciousActivities { get; init; }
    public int PrivilegedAccessEvents { get; init; }
    public IReadOnlyList<AccessViolationDto> RecentViolations { get; init; } = Array.Empty<AccessViolationDto>();
    public IReadOnlyList<PrivilegedAccessDto> RecentPrivilegedAccess { get; init; } = Array.Empty<PrivilegedAccessDto>();
}

public record AccessViolationDto
{
    public DateTime Timestamp { get; init; }
    public Guid? UserId { get; init; }
    public string? UserName { get; init; }
    public string Action { get; init; } = string.Empty;
    public string Resource { get; init; } = string.Empty;
    public string? IpAddress { get; init; }
    public string Reason { get; init; } = string.Empty;
}

public record PrivilegedAccessDto
{
    public DateTime Timestamp { get; init; }
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public string? IpAddress { get; init; }
}

/// <summary>
/// Data retention status.
/// </summary>
public record DataRetentionStatusDto
{
    public int TotalRetentionPolicies { get; init; }
    public int ActiveRetentionPolicies { get; init; }
    public int ItemsPendingDeletion { get; init; }
    public int ItemsDeletedThisPeriod { get; init; }
    public long StorageRecoveredBytes { get; init; }
    public IReadOnlyList<RetentionPolicyStatusDto> Policies { get; init; } = Array.Empty<RetentionPolicyStatusDto>();
}

public record RetentionPolicyStatusDto
{
    public string PolicyName { get; init; } = string.Empty;
    public int RetentionDays { get; init; }
    public int AffectedItems { get; init; }
    public DateTime NextRunAt { get; init; }
    public DateTime? LastRunAt { get; init; }
}

/// <summary>
/// Compliance alert.
/// </summary>
public record ComplianceAlertDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Severity { get; init; } = string.Empty; // "critical", "warning", "info"
    public string Category { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public bool IsResolved { get; init; }
    public DateTime? ResolvedAt { get; init; }
}
