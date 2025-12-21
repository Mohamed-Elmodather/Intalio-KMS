namespace AFC27.KMS.Media.Application.DTOs;

/// <summary>
/// Full media item details.
/// </summary>
public record MediaItemDto
{
    public Guid Id { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string OriginalFileName { get; init; } = string.Empty;
    public string? Title { get; init; }
    public string? TitleArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string? AltText { get; init; }
    public string? AltTextArabic { get; init; }
    public string Type { get; init; } = string.Empty;
    public string MimeType { get; init; } = string.Empty;
    public string Extension { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }
    public string FileSizeFormatted { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string? ThumbnailUrl { get; init; }
    public string? PreviewUrl { get; init; }
    public Guid GalleryId { get; init; }
    public string GalleryName { get; init; } = string.Empty;
    public Guid UploadedById { get; init; }
    public string UploadedByName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int Width { get; init; }
    public int Height { get; init; }
    public string? Dimensions { get; init; }
    public int? DurationSeconds { get; init; }
    public string? DurationFormatted { get; init; }
    public MediaMetadataDto? Metadata { get; init; }
    public int ViewCount { get; init; }
    public int DownloadCount { get; init; }
    public int SortOrder { get; init; }
    public bool IsFeatured { get; init; }
    public IReadOnlyList<ThumbnailDto> Thumbnails { get; init; } = Array.Empty<ThumbnailDto>();
    public IReadOnlyList<TranscodingDto> Transcodings { get; init; } = Array.Empty<TranscodingDto>();
    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
}

/// <summary>
/// Summary view of a media item.
/// </summary>
public record MediaItemSummaryDto
{
    public Guid Id { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string? Title { get; init; }
    public string Type { get; init; } = string.Empty;
    public string MimeType { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }
    public string FileSizeFormatted { get; init; } = string.Empty;
    public string? ThumbnailUrl { get; init; }
    public string Status { get; init; } = string.Empty;
    public int Width { get; init; }
    public int Height { get; init; }
    public int? DurationSeconds { get; init; }
    public string? DurationFormatted { get; init; }
    public bool IsFeatured { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Thumbnail details.
/// </summary>
public record ThumbnailDto
{
    public Guid Id { get; init; }
    public string Size { get; init; } = string.Empty;
    public int Width { get; init; }
    public int Height { get; init; }
    public string Url { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }
}

/// <summary>
/// Transcoding variant details.
/// </summary>
public record TranscodingDto
{
    public Guid Id { get; init; }
    public string Quality { get; init; } = string.Empty;
    public string Resolution { get; init; } = string.Empty;
    public int Width { get; init; }
    public int Height { get; init; }
    public int Bitrate { get; init; }
    public string Codec { get; init; } = string.Empty;
    public string Format { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }
    public string FileSizeFormatted { get; init; } = string.Empty;
    public int DurationSeconds { get; init; }
    public string Status { get; init; } = string.Empty;
}

/// <summary>
/// Media metadata (EXIF, etc.).
/// </summary>
public record MediaMetadataDto
{
    public string? CameraMake { get; init; }
    public string? CameraModel { get; init; }
    public string? LensModel { get; init; }
    public string? FocalLength { get; init; }
    public string? Aperture { get; init; }
    public string? ShutterSpeed { get; init; }
    public string? ISO { get; init; }
    public string? DateTaken { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public string? Location { get; init; }
    public string? Copyright { get; init; }
    public string? Artist { get; init; }
    public int? Orientation { get; init; }
    public string? ColorSpace { get; init; }
    public int? BitDepth { get; init; }
    public string? VideoCodec { get; init; }
    public string? AudioCodec { get; init; }
    public int? AudioBitrate { get; init; }
    public int? AudioChannels { get; init; }
    public int? AudioSampleRate { get; init; }
    public int? FrameRate { get; init; }
}

/// <summary>
/// Request to upload media.
/// </summary>
public record UploadMediaRequest
{
    public Guid GalleryId { get; init; }
    public string? Title { get; init; }
    public string? TitleArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string? AltText { get; init; }
    public string? AltTextArabic { get; init; }
    public IEnumerable<string>? Tags { get; init; }
}

/// <summary>
/// Request to update media item.
/// </summary>
public record UpdateMediaItemRequest
{
    public string? Title { get; init; }
    public string? TitleArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string? AltText { get; init; }
    public string? AltTextArabic { get; init; }
    public IEnumerable<string>? Tags { get; init; }
}

/// <summary>
/// Request to move media items.
/// </summary>
public record MoveMediaRequest
{
    public IEnumerable<Guid> MediaItemIds { get; init; } = Array.Empty<Guid>();
    public Guid TargetGalleryId { get; init; }
}

/// <summary>
/// Request to reorder media items.
/// </summary>
public record ReorderMediaRequest
{
    public IEnumerable<MediaOrderItem> Items { get; init; } = Array.Empty<MediaOrderItem>();
}

/// <summary>
/// Media order item.
/// </summary>
public record MediaOrderItem
{
    public Guid MediaItemId { get; init; }
    public int SortOrder { get; init; }
}

/// <summary>
/// Filter request for media items.
/// </summary>
public record MediaFilterRequest
{
    public string? Search { get; init; }
    public Guid? GalleryId { get; init; }
    public string? Type { get; init; }
    public string? Status { get; init; }
    public Guid? UploadedById { get; init; }
    public IEnumerable<string>? Tags { get; init; }
    public bool? FeaturedOnly { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public long? MinSizeBytes { get; init; }
    public long? MaxSizeBytes { get; init; }
    public string SortBy { get; init; } = "CreatedAt";
    public bool SortDescending { get; init; } = true;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

/// <summary>
/// Upload response with progress info.
/// </summary>
public record UploadResponseDto
{
    public Guid MediaItemId { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? ThumbnailUrl { get; init; }
    public string Message { get; init; } = string.Empty;
}

/// <summary>
/// Bulk upload response.
/// </summary>
public record BulkUploadResponseDto
{
    public int TotalFiles { get; init; }
    public int SuccessCount { get; init; }
    public int FailedCount { get; init; }
    public IReadOnlyList<UploadResponseDto> Results { get; init; } = Array.Empty<UploadResponseDto>();
}
