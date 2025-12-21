using System;
using System.Collections.Generic;

namespace AFC27.KMS.WebApi.Features.QualityAssurance.Models;

/// <summary>
/// Document quality review entity
/// </summary>
public class QualityReview
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public string DocumentTitle { get; set; } = string.Empty;
    public int DocumentVersion { get; set; }
    public ReviewType Type { get; set; }
    public ReviewStatus Status { get; set; } = ReviewStatus.Pending;
    public Guid RequestedBy { get; set; }
    public DateTime RequestedAt { get; set; }
    public Guid? AssignedTo { get; set; }
    public DateTime? AssignedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public ReviewPriority Priority { get; set; } = ReviewPriority.Normal;
    public List<QualityCriterion> Criteria { get; set; } = new();
    public List<ReviewComment> Comments { get; set; } = new();
    public QualityScore? FinalScore { get; set; }
    public string? Recommendation { get; set; }
    public bool RequiresRevision { get; set; }
    public List<string> RevisionNotes { get; set; } = new();
}

public enum ReviewType
{
    Initial,
    Revision,
    Periodic,
    Compliance,
    PeerReview,
    ExpertReview,
    FinalApproval
}

public enum ReviewStatus
{
    Pending,
    Assigned,
    InProgress,
    AwaitingInfo,
    Completed,
    Rejected,
    Cancelled
}

public enum ReviewPriority
{
    Low,
    Normal,
    High,
    Urgent
}

/// <summary>
/// Quality criterion for evaluation
/// </summary>
public class QualityCriterion
{
    public Guid Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Weight { get; set; } = 1.0;
    public CriterionType Type { get; set; }
    public double? Score { get; set; }
    public string? Feedback { get; set; }
    public bool IsMandatory { get; set; }
    public List<string>? Guidelines { get; set; }
}

public enum CriterionType
{
    Boolean,
    Scale,
    Percentage,
    Text
}

/// <summary>
/// Quality score calculation
/// </summary>
public class QualityScore
{
    public double OverallScore { get; set; }
    public QualityGrade Grade { get; set; }
    public Dictionary<string, double> CategoryScores { get; set; } = new();
    public List<QualityIssue> Issues { get; set; } = new();
    public List<string> Strengths { get; set; } = new();
    public List<string> Improvements { get; set; } = new();
}

public enum QualityGrade
{
    Excellent,  // 90-100
    Good,       // 75-89
    Acceptable, // 60-74
    NeedsWork,  // 40-59
    Poor        // 0-39
}

/// <summary>
/// Quality issue found during review
/// </summary>
public class QualityIssue
{
    public Guid Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public IssueSeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Suggestion { get; set; }
    public bool IsResolved { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public enum IssueSeverity
{
    Info,
    Minor,
    Major,
    Critical
}

/// <summary>
/// Review comment
/// </summary>
public class ReviewComment
{
    public Guid Id { get; set; }
    public Guid ReviewId { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Location { get; set; }
    public CommentType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsResolved { get; set; }
    public Guid? ParentCommentId { get; set; }
}

public enum CommentType
{
    General,
    Suggestion,
    Issue,
    Question,
    Approval
}

/// <summary>
/// Quality checklist template
/// </summary>
public class QualityChecklist
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public List<QualityCriterion> Criteria { get; set; } = new();
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}

/// <summary>
/// Request to create a quality review
/// </summary>
public class CreateQualityReviewRequest
{
    public Guid DocumentId { get; set; }
    public ReviewType Type { get; set; }
    public Guid? AssignToUserId { get; set; }
    public DateTime? DueDate { get; set; }
    public ReviewPriority Priority { get; set; } = ReviewPriority.Normal;
    public Guid? ChecklistId { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Request to submit review result
/// </summary>
public class SubmitReviewRequest
{
    public Guid ReviewId { get; set; }
    public List<CriterionScore> CriterionScores { get; set; } = new();
    public string? Recommendation { get; set; }
    public bool Approved { get; set; }
    public bool RequiresRevision { get; set; }
    public List<string>? RevisionNotes { get; set; }
}

public class CriterionScore
{
    public Guid CriterionId { get; set; }
    public double? Score { get; set; }
    public string? Feedback { get; set; }
}

/// <summary>
/// Quality dashboard summary
/// </summary>
public class QualityDashboardSummary
{
    public int PendingReviews { get; set; }
    public int InProgressReviews { get; set; }
    public int CompletedThisMonth { get; set; }
    public double AverageScore { get; set; }
    public double AverageReviewTime { get; set; } // in hours
    public List<ReviewByType> ReviewsByType { get; set; } = new();
    public List<ScoreDistribution> ScoreDistribution { get; set; } = new();
    public List<CommonIssue> CommonIssues { get; set; } = new();
}

public class ReviewByType
{
    public ReviewType Type { get; set; }
    public int Count { get; set; }
    public double AverageScore { get; set; }
}

public class ScoreDistribution
{
    public QualityGrade Grade { get; set; }
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class CommonIssue
{
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Occurrences { get; set; }
}

/// <summary>
/// Quality review summary for lists
/// </summary>
public class QualityReviewSummary
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public string DocumentTitle { get; set; } = string.Empty;
    public ReviewType Type { get; set; }
    public ReviewStatus Status { get; set; }
    public ReviewPriority Priority { get; set; }
    public string? AssignedToName { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public double? FinalScore { get; set; }
    public QualityGrade? Grade { get; set; }
}
