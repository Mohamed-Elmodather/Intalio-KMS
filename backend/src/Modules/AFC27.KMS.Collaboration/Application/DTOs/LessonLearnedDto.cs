namespace AFC27.KMS.Collaboration.Application.DTOs;

/// <summary>
/// Lesson action item DTO.
/// </summary>
public record LessonActionDto
{
    public Guid Id { get; init; }
    public Guid LessonLearnedId { get; init; }
    public string Description { get; init; } = string.Empty;
    public string? DescriptionArabic { get; init; }
    public Guid AssigneeId { get; init; }
    public string AssigneeName { get; init; } = string.Empty;
    public string? AssigneeAvatarUrl { get; init; }
    public Guid? DelegatedToId { get; init; }
    public string? DelegatedToName { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public DateTime DueDate { get; init; }
    public DateTime? StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public string? CompletionNotes { get; init; }
    public string? CompletionNotesArabic { get; init; }
    public DateTime? VerifiedAt { get; init; }
    public string? VerifiedByName { get; init; }
    public string? VerificationNotes { get; init; }
    public Guid? AffectedDocumentId { get; init; }
    public string? AffectedDocumentTitle { get; init; }
    public string? AffectedDocumentType { get; init; }
    public bool IsOverdue { get; init; }
    public int ReminderCount { get; init; }
    public DateTime? EscalatedAt { get; init; }
    public string? EscalatedToName { get; init; }
    public int SortOrder { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Create lesson action request.
/// </summary>
public record CreateLessonActionRequest
{
    public string Description { get; init; } = string.Empty;
    public string? DescriptionArabic { get; init; }
    public Guid AssigneeId { get; init; }
    public DateTime DueDate { get; init; }
    public string Priority { get; init; } = "Normal";
    public Guid? AffectedDocumentId { get; init; }
    public string? AffectedDocumentTitle { get; init; }
    public string? AffectedDocumentType { get; init; }
}

/// <summary>
/// Update lesson action request.
/// </summary>
public record UpdateLessonActionRequest
{
    public string Description { get; init; } = string.Empty;
    public string? DescriptionArabic { get; init; }
    public DateTime DueDate { get; init; }
    public string Priority { get; init; } = "Normal";
}

/// <summary>
/// Complete action request.
/// </summary>
public record CompleteActionRequest
{
    public string? Notes { get; init; }
    public string? NotesArabic { get; init; }
}

/// <summary>
/// Verify action request.
/// </summary>
public record VerifyActionRequest
{
    public string? Notes { get; init; }
}

/// <summary>
/// Delegate action request.
/// </summary>
public record DelegateActionRequest
{
    public Guid DelegateToId { get; init; }
    public string DelegateToName { get; init; } = string.Empty;
}

/// <summary>
/// Link document to action request.
/// </summary>
public record LinkDocumentRequest
{
    public Guid DocumentId { get; init; }
    public string DocumentTitle { get; init; } = string.Empty;
    public string DocumentType { get; init; } = string.Empty;
}

/// <summary>
/// Assign process owner request.
/// </summary>
public record AssignProcessOwnerRequest
{
    public Guid ProcessOwnerId { get; init; }
    public string ProcessOwnerName { get; init; } = string.Empty;
}

/// <summary>
/// Set root cause request.
/// </summary>
public record SetRootCauseRequest
{
    public string RootCause { get; init; } = string.Empty;
    public string? RootCauseArabic { get; init; }
    public string? Method { get; init; }
}

/// <summary>
/// Lesson analytics overview DTO.
/// </summary>
public record LessonsAnalyticsOverviewDto
{
    // Volume
    public int TotalLessons { get; init; }
    public int LessonsCreatedInPeriod { get; init; }
    public int LessonsPublishedInPeriod { get; init; }

    // Status distribution
    public Dictionary<string, int> LessonsByStatus { get; init; } = new();

    // Category distribution
    public Dictionary<string, int> LessonsByCategory { get; init; } = new();

    // Impact distribution
    public Dictionary<string, int> LessonsByImpact { get; init; } = new();

    // Action tracking
    public int TotalActions { get; init; }
    public int OpenActions { get; init; }
    public int CompletedActions { get; init; }
    public int VerifiedActions { get; init; }
    public int OverdueActions { get; init; }
    public double ActionCompletionRate { get; init; }
    public double AverageTimeToCompleteActionDays { get; init; }

    // Engagement
    public int TotalViews { get; init; }
    public int TotalUsefulVotes { get; init; }

    // Contributors
    public IReadOnlyList<LessonContributorDto> TopContributors { get; init; } = Array.Empty<LessonContributorDto>();

    // Overdue items
    public IReadOnlyList<OverdueActionSummaryDto> OverdueActionsList { get; init; } = Array.Empty<OverdueActionSummaryDto>();

    // Lessons without process owners
    public int LessonsWithoutProcessOwner { get; init; }
}

/// <summary>
/// Contributor summary for analytics.
/// </summary>
public record LessonContributorDto
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public int LessonsAuthored { get; init; }
    public int ActionsCompleted { get; init; }
    public int UsefulVotesReceived { get; init; }
}

/// <summary>
/// Overdue action summary for dashboards.
/// </summary>
public record OverdueActionSummaryDto
{
    public Guid ActionId { get; init; }
    public string ActionDescription { get; init; } = string.Empty;
    public Guid LessonId { get; init; }
    public string LessonTitle { get; init; } = string.Empty;
    public string AssigneeName { get; init; } = string.Empty;
    public DateTime DueDate { get; init; }
    public int DaysOverdue { get; init; }
    public int ReminderCount { get; init; }
}
