namespace AFC27.KMS.Collaboration.Application.DTOs;

/// <summary>
/// Discussion response DTO.
/// </summary>
public record DiscussionDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? TitleArabic { get; init; }
    public string Content { get; init; } = string.Empty;
    public string? ContentArabic { get; init; }
    public Guid CommunityId { get; init; }
    public string CommunityName { get; init; } = string.Empty;
    public Guid AuthorId { get; init; }
    public string AuthorName { get; init; } = string.Empty;
    public string? AuthorAvatarUrl { get; init; }
    public string? AuthorJobTitle { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public bool IsPinned { get; init; }
    public bool IsLocked { get; init; }
    public bool IsAnnouncement { get; init; }
    public int ViewCount { get; init; }
    public int ReplyCount { get; init; }
    public int LikeCount { get; init; }
    public bool IsLikedByCurrentUser { get; init; }
    public bool IsFollowedByCurrentUser { get; init; }
    public DateTime? LastActivityAt { get; init; }
    public string? LastReplyByName { get; init; }
    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// Discussion summary for lists.
/// </summary>
public record DiscussionSummaryDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? TitleArabic { get; init; }
    public string? ContentPreview { get; init; }
    public Guid CommunityId { get; init; }
    public string CommunityName { get; init; } = string.Empty;
    public Guid AuthorId { get; init; }
    public string AuthorName { get; init; } = string.Empty;
    public string? AuthorAvatarUrl { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public bool IsPinned { get; init; }
    public bool IsLocked { get; init; }
    public int ViewCount { get; init; }
    public int ReplyCount { get; init; }
    public int LikeCount { get; init; }
    public DateTime? LastActivityAt { get; init; }
    public string? LastReplyByName { get; init; }
    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Create discussion request.
/// </summary>
public record CreateDiscussionRequest
{
    public string Title { get; init; } = string.Empty;
    public string? TitleArabic { get; init; }
    public string Content { get; init; } = string.Empty;
    public string? ContentArabic { get; init; }
    public Guid CommunityId { get; init; }
    public string Type { get; init; } = "Discussion";
    public IReadOnlyList<string>? Tags { get; init; }
}

/// <summary>
/// Update discussion request.
/// </summary>
public record UpdateDiscussionRequest
{
    public string Title { get; init; } = string.Empty;
    public string? TitleArabic { get; init; }
    public string Content { get; init; } = string.Empty;
    public string? ContentArabic { get; init; }
    public IReadOnlyList<string>? Tags { get; init; }
}

/// <summary>
/// Discussion filter request.
/// </summary>
public record DiscussionFilterRequest
{
    public string? Search { get; init; }
    public Guid? CommunityId { get; init; }
    public Guid? AuthorId { get; init; }
    public string? Type { get; init; }
    public string? Status { get; init; }
    public string? Tag { get; init; }
    public bool? IsPinned { get; init; }
    public bool? IsUnanswered { get; init; }
    public string SortBy { get; init; } = "LastActivityAt";
    public bool SortDescending { get; init; } = true;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

/// <summary>
/// Poll option DTO (for poll-type discussions).
/// </summary>
public record PollOptionDto
{
    public Guid Id { get; init; }
    public string Text { get; init; } = string.Empty;
    public string? TextArabic { get; init; }
    public int VoteCount { get; init; }
    public decimal VotePercentage { get; init; }
    public bool IsVotedByCurrentUser { get; init; }
}

/// <summary>
/// Poll discussion DTO.
/// </summary>
public record PollDiscussionDto : DiscussionDto
{
    public IReadOnlyList<PollOptionDto> Options { get; init; } = Array.Empty<PollOptionDto>();
    public int TotalVotes { get; init; }
    public bool AllowMultipleVotes { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public bool HasVoted { get; init; }
}
