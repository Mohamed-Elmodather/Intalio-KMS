using AFC27.KMS.Admin.Domain.Entities;

namespace AFC27.KMS.Admin.Application.DTOs;

/// <summary>
/// Governance review DTO.
/// </summary>
public record GovernanceReviewDto
{
    public Guid Id { get; init; }
    public string TitleEnglish { get; init; } = string.Empty;
    public string TitleArabic { get; init; } = string.Empty;
    public string ReviewType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime ReviewDate { get; init; }
    public DateTime? CompletedAt { get; init; }
    public Guid ReviewerId { get; init; }
    public string ReviewerName { get; init; } = string.Empty;
    public string? Scope { get; init; }
    public string? Findings { get; init; }
    public int TotalFindings { get; init; }
    public int CriticalFindings { get; init; }
    public int ResolvedFindings { get; init; }
    public string? RemediationPlan { get; init; }
    public DateTime? RemediationDeadline { get; init; }
    public double? GovernanceScore { get; init; }
    public double? PreviousScore { get; init; }
    public double RemediationProgress { get; init; }
    public bool IsOverdue { get; init; }
    public IReadOnlyList<GovernanceActionDto> Actions { get; init; } = Array.Empty<GovernanceActionDto>();
}

/// <summary>
/// Governance action DTO.
/// </summary>
public record GovernanceActionDto
{
    public Guid Id { get; init; }
    public Guid GovernanceReviewId { get; init; }
    public string TitleEnglish { get; init; } = string.Empty;
    public string TitleArabic { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public Guid AssigneeId { get; init; }
    public string AssigneeName { get; init; } = string.Empty;
    public DateTime DueDate { get; init; }
    public DateTime? CompletedAt { get; init; }
    public string? CompletionNotes { get; init; }
    public bool IsOverdue { get; init; }
}

/// <summary>
/// Create governance review request.
/// </summary>
public record CreateGovernanceReviewRequest
{
    public string TitleEnglish { get; init; } = string.Empty;
    public string? TitleArabic { get; init; }
    public GovernanceReviewType ReviewType { get; init; }
    public DateTime ReviewDate { get; init; }
    public string? Scope { get; init; }
    public string? ScopeArabic { get; init; }
}

/// <summary>
/// Record findings for a governance review.
/// </summary>
public record RecordFindingsRequest
{
    public string Findings { get; init; } = string.Empty;
    public string? FindingsArabic { get; init; }
    public int TotalFindings { get; init; }
    public int CriticalFindings { get; init; }
    public double? GovernanceScore { get; init; }
}

/// <summary>
/// Set remediation plan request.
/// </summary>
public record SetRemediationPlanRequest
{
    public string Plan { get; init; } = string.Empty;
    public string? PlanArabic { get; init; }
    public DateTime Deadline { get; init; }
}

/// <summary>
/// Create governance action request.
/// </summary>
public record CreateGovernanceActionRequest
{
    public string TitleEnglish { get; init; } = string.Empty;
    public string? TitleArabic { get; init; }
    public string Description { get; init; } = string.Empty;
    public string? DescriptionArabic { get; init; }
    public GovernanceActionPriority Priority { get; init; }
    public Guid AssigneeId { get; init; }
    public string AssigneeName { get; init; } = string.Empty;
    public DateTime DueDate { get; init; }
}

/// <summary>
/// Governance dashboard summary.
/// </summary>
public record GovernanceDashboardDto
{
    public int TotalReviews { get; init; }
    public int CompletedReviews { get; init; }
    public int OverdueReviews { get; init; }
    public int OpenActions { get; init; }
    public int OverdueActions { get; init; }
    public double? LatestGovernanceScore { get; init; }
    public double? ScoreTrend { get; init; } // Positive = improving
    public IReadOnlyList<GovernanceReviewDto> UpcomingReviews { get; init; } = Array.Empty<GovernanceReviewDto>();
    public IReadOnlyList<GovernanceActionDto> OverdueActionItems { get; init; } = Array.Empty<GovernanceActionDto>();
}
