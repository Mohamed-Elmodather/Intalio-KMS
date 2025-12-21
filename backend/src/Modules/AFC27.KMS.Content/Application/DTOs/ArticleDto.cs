namespace AFC27.KMS.Content.Application.DTOs;

/// <summary>
/// Article response DTO.
/// </summary>
public record ArticleDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? TitleArabic { get; init; }
    public string? Summary { get; init; }
    public string? SummaryArabic { get; init; }
    public string Content { get; init; } = string.Empty;
    public string? ContentArabic { get; init; }
    public string Slug { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? FeaturedImageUrl { get; init; }
    public string? ThumbnailUrl { get; init; }
    public Guid AuthorId { get; init; }
    public string AuthorName { get; init; } = string.Empty;
    public string? AuthorAvatarUrl { get; init; }
    public Guid? CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public bool IsFeatured { get; init; }
    public bool AllowComments { get; init; }
    public int ViewCount { get; init; }
    public int CommentCount { get; init; }
    public DateTime? PublishedAt { get; init; }
    public DateTime? ScheduledPublishAt { get; init; }
    public int Version { get; init; }
    public IReadOnlyList<TagSummaryDto> Tags { get; init; } = Array.Empty<TagSummaryDto>();
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// Article summary for lists.
/// </summary>
public record ArticleSummaryDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? TitleArabic { get; init; }
    public string? Summary { get; init; }
    public string? SummaryArabic { get; init; }
    public string Slug { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? ThumbnailUrl { get; init; }
    public string AuthorName { get; init; } = string.Empty;
    public string? CategoryName { get; init; }
    public bool IsFeatured { get; init; }
    public int ViewCount { get; init; }
    public int CommentCount { get; init; }
    public DateTime? PublishedAt { get; init; }
    public IReadOnlyList<TagSummaryDto> Tags { get; init; } = Array.Empty<TagSummaryDto>();
}

/// <summary>
/// Create article request.
/// </summary>
public record CreateArticleRequest
{
    public string Title { get; init; } = string.Empty;
    public string? TitleArabic { get; init; }
    public string? Summary { get; init; }
    public string? SummaryArabic { get; init; }
    public string Content { get; init; } = string.Empty;
    public string? ContentArabic { get; init; }
    public string Type { get; init; } = "Article";
    public Guid? CategoryId { get; init; }
    public IReadOnlyList<Guid>? TagIds { get; init; }
    public string? FeaturedImageUrl { get; init; }
    public bool AllowComments { get; init; } = true;
}

/// <summary>
/// Update article request.
/// </summary>
public record UpdateArticleRequest
{
    public string Title { get; init; } = string.Empty;
    public string? TitleArabic { get; init; }
    public string? Summary { get; init; }
    public string? SummaryArabic { get; init; }
    public string Content { get; init; } = string.Empty;
    public string? ContentArabic { get; init; }
    public Guid? CategoryId { get; init; }
    public IReadOnlyList<Guid>? TagIds { get; init; }
    public string? FeaturedImageUrl { get; init; }
    public bool AllowComments { get; init; } = true;
}

/// <summary>
/// Article filter request.
/// </summary>
public record ArticleFilterRequest
{
    public string? Search { get; init; }
    public string? Type { get; init; }
    public string? Status { get; init; }
    public Guid? CategoryId { get; init; }
    public Guid? AuthorId { get; init; }
    public Guid? TagId { get; init; }
    public bool? IsFeatured { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string SortBy { get; init; } = "PublishedAt";
    public bool SortDescending { get; init; } = true;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

/// <summary>
/// Publish article request.
/// </summary>
public record PublishArticleRequest
{
    public DateTime? ScheduledAt { get; init; }
}

/// <summary>
/// Article version DTO.
/// </summary>
public record ArticleVersionDto
{
    public Guid Id { get; init; }
    public int VersionNumber { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? TitleArabic { get; init; }
    public string Content { get; init; } = string.Empty;
    public string? ContentArabic { get; init; }
    public string ModifiedByName { get; init; } = string.Empty;
    public DateTime ModifiedAt { get; init; }
    public string? ChangeNotes { get; init; }
}
