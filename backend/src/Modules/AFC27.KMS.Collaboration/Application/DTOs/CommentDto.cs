namespace AFC27.KMS.Collaboration.Application.DTOs;

/// <summary>
/// Comment response DTO.
/// </summary>
public record CommentDto
{
    public Guid Id { get; init; }
    public string Content { get; init; } = string.Empty;
    public string? ContentArabic { get; init; }
    public string TargetType { get; init; } = string.Empty;
    public Guid TargetId { get; init; }
    public Guid? ParentId { get; init; }
    public Guid AuthorId { get; init; }
    public string AuthorName { get; init; } = string.Empty;
    public string? AuthorAvatarUrl { get; init; }
    public string? AuthorJobTitle { get; init; }
    public bool IsEdited { get; init; }
    public DateTime? EditedAt { get; init; }
    public bool IsAcceptedAnswer { get; init; }
    public int LikeCount { get; init; }
    public int ReplyCount { get; init; }
    public bool IsLikedByCurrentUser { get; init; }
    public IReadOnlyList<MentionDto> Mentions { get; init; } = Array.Empty<MentionDto>();
    public IReadOnlyList<CommentDto> Replies { get; init; } = Array.Empty<CommentDto>();
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Mention DTO.
/// </summary>
public record MentionDto
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public int StartIndex { get; init; }
    public int EndIndex { get; init; }
}

/// <summary>
/// Create comment request.
/// </summary>
public record CreateCommentRequest
{
    public string Content { get; init; } = string.Empty;
    public string? ContentArabic { get; init; }
    public string TargetType { get; init; } = string.Empty;
    public Guid TargetId { get; init; }
    public Guid? ParentId { get; init; }
    public IReadOnlyList<MentionRequest>? Mentions { get; init; }
}

/// <summary>
/// Mention in create request.
/// </summary>
public record MentionRequest
{
    public Guid UserId { get; init; }
    public int StartIndex { get; init; }
    public int EndIndex { get; init; }
}

/// <summary>
/// Update comment request.
/// </summary>
public record UpdateCommentRequest
{
    public string Content { get; init; } = string.Empty;
    public string? ContentArabic { get; init; }
    public IReadOnlyList<MentionRequest>? Mentions { get; init; }
}

/// <summary>
/// Follow DTO.
/// </summary>
public record FollowDto
{
    public Guid Id { get; init; }
    public string TargetType { get; init; } = string.Empty;
    public Guid TargetId { get; init; }
    public string TargetName { get; init; } = string.Empty;
    public bool NotificationsEnabled { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Follow request.
/// </summary>
public record FollowRequest
{
    public string TargetType { get; init; } = string.Empty;
    public Guid TargetId { get; init; }
    public bool NotificationsEnabled { get; init; } = true;
}

/// <summary>
/// Lessons learned DTO.
/// </summary>
public record LessonLearnedDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? TitleArabic { get; init; }
    public string Description { get; init; } = string.Empty;
    public string? DescriptionArabic { get; init; }
    public string Context { get; init; } = string.Empty;
    public string? ContextArabic { get; init; }
    public string Challenge { get; init; } = string.Empty;
    public string? ChallengeArabic { get; init; }
    public string Solution { get; init; } = string.Empty;
    public string? SolutionArabic { get; init; }
    public string Outcome { get; init; } = string.Empty;
    public string? OutcomeArabic { get; init; }
    public string? Recommendations { get; init; }
    public string? RecommendationsArabic { get; init; }

    // Positive lessons
    public string? WhatWentWell { get; init; }
    public string? WhatWentWellArabic { get; init; }

    // Root cause analysis
    public string? RootCause { get; init; }
    public string? RootCauseArabic { get; init; }
    public string? RootCauseMethod { get; init; }

    // Classification
    public string Category { get; init; } = string.Empty;
    public string Impact { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? ProjectPhase { get; init; }
    public string? ImpactType { get; init; }

    // Authorship
    public Guid AuthorId { get; init; }
    public string AuthorName { get; init; } = string.Empty;
    public string? AuthorAvatarUrl { get; init; }
    public bool IsAnonymous { get; init; }

    // Process owner
    public Guid? ProcessOwnerId { get; init; }
    public string? ProcessOwnerName { get; init; }

    // Associations
    public Guid? CommunityId { get; init; }
    public string? CommunityName { get; init; }
    public Guid? ProjectId { get; init; }
    public string? ProjectName { get; init; }
    public DateTime? OccurredAt { get; init; }

    // Analytics
    public int ViewCount { get; init; }
    public int UsefulCount { get; init; }
    public bool IsMarkedUsefulByCurrentUser { get; init; }
    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();
    public DateTime CreatedAt { get; init; }

    // Rejection info
    public string? RejectionReason { get; init; }

    // Action tracking summary
    public IReadOnlyList<LessonActionDto> Actions { get; init; } = Array.Empty<LessonActionDto>();
    public int TotalActions { get; init; }
    public int CompletedActions { get; init; }
    public int OverdueActions { get; init; }
}

/// <summary>
/// Lessons learned summary for lists.
/// </summary>
public record LessonLearnedSummaryDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? TitleArabic { get; init; }
    public string? DescriptionPreview { get; init; }
    public string Category { get; init; } = string.Empty;
    public string Impact { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string AuthorName { get; init; } = string.Empty;
    public string? ProjectName { get; init; }
    public string? ProcessOwnerName { get; init; }
    public int ViewCount { get; init; }
    public int UsefulCount { get; init; }
    public int TotalActions { get; init; }
    public int CompletedActions { get; init; }
    public int OverdueActions { get; init; }
    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Create lesson learned request.
/// </summary>
public record CreateLessonLearnedRequest
{
    public string Title { get; init; } = string.Empty;
    public string? TitleArabic { get; init; }
    public string Description { get; init; } = string.Empty;
    public string? DescriptionArabic { get; init; }
    public string Context { get; init; } = string.Empty;
    public string? ContextArabic { get; init; }
    public string Challenge { get; init; } = string.Empty;
    public string? ChallengeArabic { get; init; }
    public string Solution { get; init; } = string.Empty;
    public string? SolutionArabic { get; init; }
    public string Outcome { get; init; } = string.Empty;
    public string? OutcomeArabic { get; init; }
    public string? Recommendations { get; init; }
    public string? RecommendationsArabic { get; init; }

    // New fields
    public string? WhatWentWell { get; init; }
    public string? WhatWentWellArabic { get; init; }
    public string? RootCause { get; init; }
    public string? RootCauseArabic { get; init; }
    public string? RootCauseMethod { get; init; }
    public string? ProjectPhase { get; init; }
    public string? ImpactType { get; init; }
    public bool IsAnonymous { get; init; }
    public Guid? ProcessOwnerId { get; init; }
    public string? ProcessOwnerName { get; init; }

    // Existing fields
    public string Category { get; init; } = "Other";
    public string Impact { get; init; } = "Medium";
    public Guid? CommunityId { get; init; }
    public Guid? ProjectId { get; init; }
    public DateTime? OccurredAt { get; init; }
    public IReadOnlyList<string>? Tags { get; init; }
}
