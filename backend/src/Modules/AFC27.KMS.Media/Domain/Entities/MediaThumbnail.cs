using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Media.Domain.Entities;

/// <summary>
/// Thumbnail variant of a media item.
/// </summary>
public class MediaThumbnail : Entity
{
    public Guid MediaItemId { get; private set; }
    public MediaItem MediaItem { get; private set; } = null!;
    public ThumbnailSize Size { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public string StoragePath { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;
    public long FileSizeBytes { get; private set; }
    public DateTime GeneratedAt { get; private set; }

    private MediaThumbnail() { }

    public static MediaThumbnail Create(
        Guid mediaItemId,
        ThumbnailSize size,
        int width,
        int height,
        string storagePath,
        string url,
        long fileSizeBytes)
    {
        return new MediaThumbnail
        {
            Id = Guid.NewGuid(),
            MediaItemId = mediaItemId,
            Size = size,
            Width = width,
            Height = height,
            StoragePath = storagePath,
            Url = url,
            FileSizeBytes = fileSizeBytes,
            GeneratedAt = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Standard thumbnail sizes.
/// </summary>
public enum ThumbnailSize
{
    /// <summary>
    /// 64x64 pixels
    /// </summary>
    Tiny,

    /// <summary>
    /// 150x150 pixels
    /// </summary>
    Small,

    /// <summary>
    /// 300x300 pixels
    /// </summary>
    Medium,

    /// <summary>
    /// 600x600 pixels
    /// </summary>
    Large,

    /// <summary>
    /// 1200x1200 pixels
    /// </summary>
    XLarge,

    /// <summary>
    /// Custom size
    /// </summary>
    Custom
}

/// <summary>
/// Video transcoding variant.
/// </summary>
public class MediaTranscoding : Entity
{
    public Guid MediaItemId { get; private set; }
    public MediaItem MediaItem { get; private set; } = null!;
    public VideoQuality Quality { get; private set; }
    public string Resolution { get; private set; } = string.Empty;
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int Bitrate { get; private set; }
    public string Codec { get; private set; } = string.Empty;
    public string Format { get; private set; } = string.Empty;
    public string StoragePath { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;
    public long FileSizeBytes { get; private set; }
    public int DurationSeconds { get; private set; }
    public TranscodingStatus Status { get; private set; }
    public int Progress { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private MediaTranscoding() { }

    public static MediaTranscoding Create(
        Guid mediaItemId,
        VideoQuality quality,
        string resolution,
        int width,
        int height,
        int bitrate,
        string codec,
        string format)
    {
        return new MediaTranscoding
        {
            Id = Guid.NewGuid(),
            MediaItemId = mediaItemId,
            Quality = quality,
            Resolution = resolution,
            Width = width,
            Height = height,
            Bitrate = bitrate,
            Codec = codec,
            Format = format,
            Status = TranscodingStatus.Pending,
            Progress = 0
        };
    }

    public void Start()
    {
        Status = TranscodingStatus.Processing;
        StartedAt = DateTime.UtcNow;
        Progress = 0;
    }

    public void UpdateProgress(int progress)
    {
        Progress = Math.Clamp(progress, 0, 100);
    }

    public void Complete(string storagePath, string url, long fileSizeBytes, int durationSeconds)
    {
        StoragePath = storagePath;
        Url = url;
        FileSizeBytes = fileSizeBytes;
        DurationSeconds = durationSeconds;
        Status = TranscodingStatus.Completed;
        Progress = 100;
        CompletedAt = DateTime.UtcNow;
    }

    public void Fail(string errorMessage)
    {
        Status = TranscodingStatus.Failed;
        ErrorMessage = errorMessage;
        CompletedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Video quality presets.
/// </summary>
public enum VideoQuality
{
    /// <summary>
    /// 360p - Low quality
    /// </summary>
    Low,

    /// <summary>
    /// 480p - Standard definition
    /// </summary>
    SD,

    /// <summary>
    /// 720p - High definition
    /// </summary>
    HD,

    /// <summary>
    /// 1080p - Full HD
    /// </summary>
    FullHD,

    /// <summary>
    /// 1440p - 2K
    /// </summary>
    QHD,

    /// <summary>
    /// 2160p - 4K Ultra HD
    /// </summary>
    UHD,

    /// <summary>
    /// Original quality
    /// </summary>
    Original
}

/// <summary>
/// Transcoding job status.
/// </summary>
public enum TranscodingStatus
{
    Pending,
    Queued,
    Processing,
    Completed,
    Failed,
    Cancelled
}
