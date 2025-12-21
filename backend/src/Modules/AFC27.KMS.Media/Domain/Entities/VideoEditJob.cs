using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Media.Domain.Entities;

/// <summary>
/// Video editing job for processing video modifications.
/// </summary>
public class VideoEditJob : AuditableEntity
{
    public Guid SourceMediaItemId { get; private set; }
    public MediaItem SourceMediaItem { get; private set; } = null!;
    public Guid? OutputMediaItemId { get; private set; }
    public MediaItem? OutputMediaItem { get; private set; }
    public Guid RequestedById { get; private set; }
    public string JobName { get; private set; } = string.Empty;
    public VideoEditType EditType { get; private set; }
    public string EditParameters { get; private set; } = string.Empty;
    public VideoEditStatus Status { get; private set; }
    public int Progress { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? OutputStoragePath { get; private set; }
    public string? OutputUrl { get; private set; }
    public long? OutputFileSizeBytes { get; private set; }
    public int? OutputDurationSeconds { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public TimeSpan? ProcessingDuration { get; private set; }

    private VideoEditJob() { }

    public static VideoEditJob CreateTrimJob(
        Guid sourceMediaItemId,
        Guid requestedById,
        string jobName,
        int startSeconds,
        int endSeconds)
    {
        var parameters = System.Text.Json.JsonSerializer.Serialize(new TrimParameters
        {
            StartSeconds = startSeconds,
            EndSeconds = endSeconds
        });

        return new VideoEditJob
        {
            Id = Guid.NewGuid(),
            SourceMediaItemId = sourceMediaItemId,
            RequestedById = requestedById,
            JobName = jobName,
            EditType = VideoEditType.Trim,
            EditParameters = parameters,
            Status = VideoEditStatus.Pending,
            Progress = 0
        };
    }

    public static VideoEditJob CreateMergeJob(
        Guid sourceMediaItemId,
        Guid requestedById,
        string jobName,
        IEnumerable<Guid> mediaItemIds)
    {
        var parameters = System.Text.Json.JsonSerializer.Serialize(new MergeParameters
        {
            MediaItemIds = mediaItemIds.ToList()
        });

        return new VideoEditJob
        {
            Id = Guid.NewGuid(),
            SourceMediaItemId = sourceMediaItemId,
            RequestedById = requestedById,
            JobName = jobName,
            EditType = VideoEditType.Merge,
            EditParameters = parameters,
            Status = VideoEditStatus.Pending,
            Progress = 0
        };
    }

    public static VideoEditJob CreateOverlayJob(
        Guid sourceMediaItemId,
        Guid requestedById,
        string jobName,
        IEnumerable<TextOverlay> textOverlays,
        IEnumerable<ImageOverlay>? imageOverlays = null)
    {
        var parameters = System.Text.Json.JsonSerializer.Serialize(new OverlayParameters
        {
            TextOverlays = textOverlays.ToList(),
            ImageOverlays = imageOverlays?.ToList() ?? new List<ImageOverlay>()
        });

        return new VideoEditJob
        {
            Id = Guid.NewGuid(),
            SourceMediaItemId = sourceMediaItemId,
            RequestedById = requestedById,
            JobName = jobName,
            EditType = VideoEditType.Overlay,
            EditParameters = parameters,
            Status = VideoEditStatus.Pending,
            Progress = 0
        };
    }

    public static VideoEditJob CreateWatermarkJob(
        Guid sourceMediaItemId,
        Guid requestedById,
        string jobName,
        string watermarkImagePath,
        WatermarkPosition position,
        int opacity = 50)
    {
        var parameters = System.Text.Json.JsonSerializer.Serialize(new WatermarkParameters
        {
            ImagePath = watermarkImagePath,
            Position = position,
            Opacity = opacity
        });

        return new VideoEditJob
        {
            Id = Guid.NewGuid(),
            SourceMediaItemId = sourceMediaItemId,
            RequestedById = requestedById,
            JobName = jobName,
            EditType = VideoEditType.Watermark,
            EditParameters = parameters,
            Status = VideoEditStatus.Pending,
            Progress = 0
        };
    }

    public static VideoEditJob CreateConvertJob(
        Guid sourceMediaItemId,
        Guid requestedById,
        string jobName,
        string outputFormat,
        VideoQuality quality)
    {
        var parameters = System.Text.Json.JsonSerializer.Serialize(new ConvertParameters
        {
            OutputFormat = outputFormat,
            Quality = quality
        });

        return new VideoEditJob
        {
            Id = Guid.NewGuid(),
            SourceMediaItemId = sourceMediaItemId,
            RequestedById = requestedById,
            JobName = jobName,
            EditType = VideoEditType.Convert,
            EditParameters = parameters,
            Status = VideoEditStatus.Pending,
            Progress = 0
        };
    }

    public void Queue()
    {
        Status = VideoEditStatus.Queued;
    }

    public void Start()
    {
        Status = VideoEditStatus.Processing;
        StartedAt = DateTime.UtcNow;
        Progress = 0;
    }

    public void UpdateProgress(int progress)
    {
        Progress = Math.Clamp(progress, 0, 100);
    }

    public void Complete(
        Guid outputMediaItemId,
        string outputStoragePath,
        string outputUrl,
        long outputFileSizeBytes,
        int outputDurationSeconds)
    {
        OutputMediaItemId = outputMediaItemId;
        OutputStoragePath = outputStoragePath;
        OutputUrl = outputUrl;
        OutputFileSizeBytes = outputFileSizeBytes;
        OutputDurationSeconds = outputDurationSeconds;
        Status = VideoEditStatus.Completed;
        Progress = 100;
        CompletedAt = DateTime.UtcNow;
        ProcessingDuration = CompletedAt - StartedAt;

        AddDomainEvent(new VideoEditCompletedEvent(Id, EditType, outputMediaItemId));
    }

    public void Fail(string errorMessage)
    {
        Status = VideoEditStatus.Failed;
        ErrorMessage = errorMessage;
        CompletedAt = DateTime.UtcNow;
        if (StartedAt.HasValue)
        {
            ProcessingDuration = CompletedAt - StartedAt;
        }

        AddDomainEvent(new VideoEditFailedEvent(Id, EditType, errorMessage));
    }

    public void Cancel()
    {
        Status = VideoEditStatus.Cancelled;
        CompletedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Type of video edit operation.
/// </summary>
public enum VideoEditType
{
    Trim,
    Merge,
    Overlay,
    Watermark,
    Convert,
    ExtractAudio,
    AddAudio,
    SpeedChange,
    Rotate,
    Crop,
    Filter
}

/// <summary>
/// Status of video edit job.
/// </summary>
public enum VideoEditStatus
{
    Pending,
    Queued,
    Processing,
    Completed,
    Failed,
    Cancelled
}

/// <summary>
/// Watermark position options.
/// </summary>
public enum WatermarkPosition
{
    TopLeft,
    TopCenter,
    TopRight,
    MiddleLeft,
    Center,
    MiddleRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}

/// <summary>
/// Parameters for trim operation.
/// </summary>
public class TrimParameters
{
    public int StartSeconds { get; set; }
    public int EndSeconds { get; set; }
}

/// <summary>
/// Parameters for merge operation.
/// </summary>
public class MergeParameters
{
    public List<Guid> MediaItemIds { get; set; } = new();
}

/// <summary>
/// Parameters for overlay operation.
/// </summary>
public class OverlayParameters
{
    public List<TextOverlay> TextOverlays { get; set; } = new();
    public List<ImageOverlay> ImageOverlays { get; set; } = new();
}

/// <summary>
/// Text overlay configuration.
/// </summary>
public class TextOverlay
{
    public string Text { get; set; } = string.Empty;
    public string FontFamily { get; set; } = "Arial";
    public int FontSize { get; set; } = 24;
    public string FontColor { get; set; } = "#FFFFFF";
    public string? BackgroundColor { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int StartSeconds { get; set; }
    public int EndSeconds { get; set; }
    public TextAnimation Animation { get; set; } = TextAnimation.None;
}

/// <summary>
/// Image overlay configuration.
/// </summary>
public class ImageOverlay
{
    public string ImagePath { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Opacity { get; set; } = 100;
    public int StartSeconds { get; set; }
    public int EndSeconds { get; set; }
}

/// <summary>
/// Text animation options.
/// </summary>
public enum TextAnimation
{
    None,
    FadeIn,
    FadeOut,
    FadeInOut,
    SlideIn,
    SlideOut,
    Typewriter
}

/// <summary>
/// Parameters for watermark operation.
/// </summary>
public class WatermarkParameters
{
    public string ImagePath { get; set; } = string.Empty;
    public WatermarkPosition Position { get; set; }
    public int Opacity { get; set; } = 50;
}

/// <summary>
/// Parameters for convert operation.
/// </summary>
public class ConvertParameters
{
    public string OutputFormat { get; set; } = "mp4";
    public VideoQuality Quality { get; set; }
}

/// <summary>
/// Domain event for completed video edit.
/// </summary>
public record VideoEditCompletedEvent(Guid JobId, VideoEditType EditType, Guid OutputMediaItemId) : DomainEvent;

/// <summary>
/// Domain event for failed video edit.
/// </summary>
public record VideoEditFailedEvent(Guid JobId, VideoEditType EditType, string ErrorMessage) : DomainEvent;
