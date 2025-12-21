namespace AFC27.KMS.Media.Application.DTOs;

/// <summary>
/// Video edit job details.
/// </summary>
public record VideoEditJobDto
{
    public Guid Id { get; init; }
    public Guid SourceMediaItemId { get; init; }
    public string SourceFileName { get; init; } = string.Empty;
    public string? SourceThumbnailUrl { get; init; }
    public Guid? OutputMediaItemId { get; init; }
    public string? OutputFileName { get; init; }
    public string? OutputThumbnailUrl { get; init; }
    public string? OutputUrl { get; init; }
    public Guid RequestedById { get; init; }
    public string RequestedByName { get; init; } = string.Empty;
    public string JobName { get; init; } = string.Empty;
    public string EditType { get; init; } = string.Empty;
    public object? EditParameters { get; init; }
    public string Status { get; init; } = string.Empty;
    public int Progress { get; init; }
    public string? ErrorMessage { get; init; }
    public long? OutputFileSizeBytes { get; init; }
    public string? OutputFileSizeFormatted { get; init; }
    public int? OutputDurationSeconds { get; init; }
    public string? OutputDurationFormatted { get; init; }
    public DateTime? StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public string? ProcessingDuration { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Summary view of video edit job.
/// </summary>
public record VideoEditJobSummaryDto
{
    public Guid Id { get; init; }
    public string JobName { get; init; } = string.Empty;
    public string EditType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int Progress { get; init; }
    public string? SourceThumbnailUrl { get; init; }
    public string? OutputThumbnailUrl { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Request to trim a video.
/// </summary>
public record TrimVideoRequest
{
    public Guid SourceMediaItemId { get; init; }
    public string JobName { get; init; } = string.Empty;
    public int StartSeconds { get; init; }
    public int EndSeconds { get; init; }
    public Guid? OutputGalleryId { get; init; }
}

/// <summary>
/// Request to merge videos.
/// </summary>
public record MergeVideosRequest
{
    public IEnumerable<Guid> MediaItemIds { get; init; } = Array.Empty<Guid>();
    public string JobName { get; init; } = string.Empty;
    public Guid? OutputGalleryId { get; init; }
}

/// <summary>
/// Request to add overlays to video.
/// </summary>
public record AddOverlaysRequest
{
    public Guid SourceMediaItemId { get; init; }
    public string JobName { get; init; } = string.Empty;
    public IEnumerable<TextOverlayDto>? TextOverlays { get; init; }
    public IEnumerable<ImageOverlayDto>? ImageOverlays { get; init; }
    public Guid? OutputGalleryId { get; init; }
}

/// <summary>
/// Text overlay configuration.
/// </summary>
public record TextOverlayDto
{
    public string Text { get; init; } = string.Empty;
    public string FontFamily { get; init; } = "Arial";
    public int FontSize { get; init; } = 24;
    public string FontColor { get; init; } = "#FFFFFF";
    public string? BackgroundColor { get; init; }
    public int X { get; init; }
    public int Y { get; init; }
    public int StartSeconds { get; init; }
    public int EndSeconds { get; init; }
    public string Animation { get; init; } = "None";
}

/// <summary>
/// Image overlay configuration.
/// </summary>
public record ImageOverlayDto
{
    public Guid ImageMediaItemId { get; init; }
    public int X { get; init; }
    public int Y { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public int Opacity { get; init; } = 100;
    public int StartSeconds { get; init; }
    public int EndSeconds { get; init; }
}

/// <summary>
/// Request to add watermark to video.
/// </summary>
public record AddWatermarkRequest
{
    public Guid SourceMediaItemId { get; init; }
    public string JobName { get; init; } = string.Empty;
    public Guid WatermarkImageId { get; init; }
    public string Position { get; init; } = "BottomRight";
    public int Opacity { get; init; } = 50;
    public Guid? OutputGalleryId { get; init; }
}

/// <summary>
/// Request to convert video format.
/// </summary>
public record ConvertVideoRequest
{
    public Guid SourceMediaItemId { get; init; }
    public string JobName { get; init; } = string.Empty;
    public string OutputFormat { get; init; } = "mp4";
    public string Quality { get; init; } = "HD";
    public Guid? OutputGalleryId { get; init; }
}

/// <summary>
/// Request to extract audio from video.
/// </summary>
public record ExtractAudioRequest
{
    public Guid SourceMediaItemId { get; init; }
    public string JobName { get; init; } = string.Empty;
    public string OutputFormat { get; init; } = "mp3";
    public Guid? OutputGalleryId { get; init; }
}

/// <summary>
/// Request to generate thumbnails.
/// </summary>
public record GenerateThumbnailsRequest
{
    public Guid MediaItemId { get; init; }
    public IEnumerable<ThumbnailConfigDto>? Sizes { get; init; }
    public bool ReplaceExisting { get; init; } = false;
}

/// <summary>
/// Thumbnail generation configuration.
/// </summary>
public record ThumbnailConfigDto
{
    public string Size { get; init; } = "Medium";
    public int? Width { get; init; }
    public int? Height { get; init; }
    public bool MaintainAspectRatio { get; init; } = true;
}

/// <summary>
/// Request to transcode video.
/// </summary>
public record TranscodeVideoRequest
{
    public Guid MediaItemId { get; init; }
    public IEnumerable<string> Qualities { get; init; } = Array.Empty<string>();
}

/// <summary>
/// Video edit job filter.
/// </summary>
public record VideoEditJobFilterRequest
{
    public string? EditType { get; init; }
    public string? Status { get; init; }
    public Guid? RequestedById { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string SortBy { get; init; } = "CreatedAt";
    public bool SortDescending { get; init; } = true;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

/// <summary>
/// Media statistics.
/// </summary>
public record MediaStatisticsDto
{
    public int TotalGalleries { get; init; }
    public int TotalMediaItems { get; init; }
    public int TotalImages { get; init; }
    public int TotalVideos { get; init; }
    public int TotalAudio { get; init; }
    public int TotalDocuments { get; init; }
    public long TotalStorageBytes { get; init; }
    public string TotalStorageFormatted { get; init; } = string.Empty;
    public int ProcessingJobs { get; init; }
    public int PendingTranscodings { get; init; }
    public IReadOnlyList<StorageByTypeDto> StorageByType { get; init; } = Array.Empty<StorageByTypeDto>();
    public IReadOnlyList<UploadTrendDto> UploadTrend { get; init; } = Array.Empty<UploadTrendDto>();
}

/// <summary>
/// Storage usage by media type.
/// </summary>
public record StorageByTypeDto
{
    public string Type { get; init; } = string.Empty;
    public int Count { get; init; }
    public long SizeBytes { get; init; }
    public string SizeFormatted { get; init; } = string.Empty;
    public double Percentage { get; init; }
}

/// <summary>
/// Upload trend data point.
/// </summary>
public record UploadTrendDto
{
    public string Date { get; init; } = string.Empty;
    public int Count { get; init; }
    public long SizeBytes { get; init; }
}
