namespace AFC27.KMS.Media.Application.DTOs;

/// <summary>
/// Full media gallery details.
/// </summary>
public record MediaGalleryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Visibility { get; init; } = string.Empty;
    public string? CoverImageUrl { get; init; }
    public Guid? ParentId { get; init; }
    public int ItemCount { get; init; }
    public long TotalSizeBytes { get; init; }
    public string TotalSizeFormatted { get; init; } = string.Empty;
    public Guid OwnerId { get; init; }
    public string OwnerName { get; init; } = string.Empty;
    public Guid? CommunityId { get; init; }
    public string? CommunityName { get; init; }
    public Guid? ProjectId { get; init; }
    public string? ProjectName { get; init; }
    public bool IsArchived { get; init; }
    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();
    public IReadOnlyList<MediaItemSummaryDto> RecentItems { get; init; } = Array.Empty<MediaItemSummaryDto>();
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
}

/// <summary>
/// Summary view of a media gallery.
/// </summary>
public record MediaGallerySummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Visibility { get; init; } = string.Empty;
    public string? CoverImageUrl { get; init; }
    public int ItemCount { get; init; }
    public long TotalSizeBytes { get; init; }
    public string TotalSizeFormatted { get; init; } = string.Empty;
    public string OwnerName { get; init; } = string.Empty;
    public IReadOnlyList<string> PreviewThumbnails { get; init; } = Array.Empty<string>();
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Request to create a new gallery.
/// </summary>
public record CreateGalleryRequest
{
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string Type { get; init; } = "General";
    public string Visibility { get; init; } = "Private";
    public Guid? ParentId { get; init; }
    public Guid? CommunityId { get; init; }
    public Guid? ProjectId { get; init; }
    public IEnumerable<string>? Tags { get; init; }
}

/// <summary>
/// Request to update a gallery.
/// </summary>
public record UpdateGalleryRequest
{
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string Visibility { get; init; } = string.Empty;
    public IEnumerable<string>? Tags { get; init; }
}

/// <summary>
/// Filter request for galleries.
/// </summary>
public record GalleryFilterRequest
{
    public string? Search { get; init; }
    public string? Type { get; init; }
    public string? Visibility { get; init; }
    public Guid? OwnerId { get; init; }
    public Guid? CommunityId { get; init; }
    public Guid? ProjectId { get; init; }
    public bool? IncludeArchived { get; init; }
    public string SortBy { get; init; } = "CreatedAt";
    public bool SortDescending { get; init; } = true;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
