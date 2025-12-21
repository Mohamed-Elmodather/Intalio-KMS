namespace AFC27.KMS.Infrastructure.Messaging.Messages;

/// <summary>
/// Message for video transcoding request.
/// </summary>
public record TranscodingRequestMessage
{
    /// <summary>
    /// Media item ID.
    /// </summary>
    public Guid MediaItemId { get; init; }

    /// <summary>
    /// Source file storage path.
    /// </summary>
    public string SourcePath { get; init; } = string.Empty;

    /// <summary>
    /// Source content type.
    /// </summary>
    public string SourceContentType { get; init; } = string.Empty;

    /// <summary>
    /// Target format for transcoding.
    /// </summary>
    public TranscodingFormat TargetFormat { get; init; }

    /// <summary>
    /// Target quality preset.
    /// </summary>
    public QualityPreset Quality { get; init; } = QualityPreset.Standard;

    /// <summary>
    /// Target resolution (e.g., "1920x1080").
    /// </summary>
    public string? Resolution { get; init; }

    /// <summary>
    /// Target bitrate in kbps (null for auto).
    /// </summary>
    public int? Bitrate { get; init; }

    /// <summary>
    /// Whether to generate adaptive streaming variants (HLS/DASH).
    /// </summary>
    public bool GenerateAdaptiveStreaming { get; init; } = false;

    /// <summary>
    /// Priority level.
    /// </summary>
    public int Priority { get; init; } = 5;

    /// <summary>
    /// Request timestamp.
    /// </summary>
    public DateTime RequestedAt { get; init; }

    /// <summary>
    /// User who requested transcoding.
    /// </summary>
    public Guid RequestedByUserId { get; init; }
}

/// <summary>
/// Transcoding output format.
/// </summary>
public enum TranscodingFormat
{
    Mp4H264,
    Mp4H265,
    WebM,
    Hls,
    Dash
}

/// <summary>
/// Quality preset for transcoding.
/// </summary>
public enum QualityPreset
{
    Low,      // 480p, low bitrate
    Standard, // 720p, standard bitrate
    High,     // 1080p, high bitrate
    Ultra     // 4K, maximum quality
}

/// <summary>
/// Message for audio extraction from video.
/// </summary>
public record AudioExtractionMessage
{
    public Guid MediaItemId { get; init; }
    public string SourcePath { get; init; } = string.Empty;
    public string OutputFormat { get; init; } = "mp3";
    public int? Bitrate { get; init; } = 192;
    public Guid RequestedByUserId { get; init; }
}

/// <summary>
/// Message for video preview generation (animated GIF or short clip).
/// </summary>
public record VideoPreviewMessage
{
    public Guid MediaItemId { get; init; }
    public string SourcePath { get; init; } = string.Empty;
    public int StartTimeSeconds { get; init; } = 0;
    public int DurationSeconds { get; init; } = 5;
    public bool GenerateGif { get; init; } = true;
    public int Width { get; init; } = 320;
}

/// <summary>
/// Message for image processing operations.
/// </summary>
public record ImageProcessingMessage
{
    public Guid MediaItemId { get; init; }
    public string SourcePath { get; init; } = string.Empty;
    public ImageOperation Operation { get; init; }
    public Dictionary<string, object> Parameters { get; init; } = new();
    public Guid RequestedByUserId { get; init; }
}

/// <summary>
/// Image processing operations.
/// </summary>
public enum ImageOperation
{
    Resize,
    Crop,
    Rotate,
    Compress,
    ConvertFormat,
    Watermark,
    GenerateVariants
}
