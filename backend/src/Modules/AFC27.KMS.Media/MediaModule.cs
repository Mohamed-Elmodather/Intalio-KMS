using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AFC27.KMS.Media;

/// <summary>
/// Media module registration.
/// </summary>
public static class MediaModule
{
    /// <summary>
    /// Add Media module services.
    /// </summary>
    public static IServiceCollection AddMediaModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Add module-specific services
        // services.AddScoped<IMediaGalleryService, MediaGalleryService>();
        // services.AddScoped<IMediaItemService, MediaItemService>();
        // services.AddScoped<IVideoEditorService, VideoEditorService>();
        // services.AddScoped<IThumbnailGenerator, ThumbnailGenerator>();
        // services.AddScoped<IVideoTranscoder, VideoTranscoder>();
        // services.AddScoped<IStorageService, LocalStorageService>();

        // Configure media processing options
        services.Configure<MediaOptions>(configuration.GetSection("Media"));

        // Add authorization policies
        services.AddAuthorizationBuilder()
            .AddPolicy("CanUploadMedia", policy =>
                policy.RequireClaim("permission", "media:upload"))
            .AddPolicy("CanDeleteMedia", policy =>
                policy.RequireClaim("permission", "media:delete"))
            .AddPolicy("CanManageGalleries", policy =>
                policy.RequireClaim("permission", "galleries:manage"))
            .AddPolicy("CanEditVideos", policy =>
                policy.RequireClaim("permission", "media:edit"))
            .AddPolicy("CanViewStatistics", policy =>
                policy.RequireClaim("permission", "media:statistics"));

        return services;
    }
}

/// <summary>
/// Media module configuration options.
/// </summary>
public class MediaOptions
{
    /// <summary>
    /// Base storage path for media files.
    /// </summary>
    public string StoragePath { get; set; } = "/data/media";

    /// <summary>
    /// Maximum file size in bytes (default 100MB).
    /// </summary>
    public long MaxFileSizeBytes { get; set; } = 104857600;

    /// <summary>
    /// Maximum bulk upload size in bytes (default 500MB).
    /// </summary>
    public long MaxBulkUploadSizeBytes { get; set; } = 524288000;

    /// <summary>
    /// Allowed image extensions.
    /// </summary>
    public string[] AllowedImageExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".bmp" };

    /// <summary>
    /// Allowed video extensions.
    /// </summary>
    public string[] AllowedVideoExtensions { get; set; } = { ".mp4", ".webm", ".mov", ".avi", ".mkv", ".wmv" };

    /// <summary>
    /// Allowed audio extensions.
    /// </summary>
    public string[] AllowedAudioExtensions { get; set; } = { ".mp3", ".wav", ".ogg", ".m4a", ".aac", ".flac" };

    /// <summary>
    /// Allowed document extensions.
    /// </summary>
    public string[] AllowedDocumentExtensions { get; set; } = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx" };

    /// <summary>
    /// Thumbnail sizes to generate.
    /// </summary>
    public ThumbnailConfig[] ThumbnailSizes { get; set; } =
    {
        new() { Name = "Tiny", Width = 64, Height = 64 },
        new() { Name = "Small", Width = 150, Height = 150 },
        new() { Name = "Medium", Width = 300, Height = 300 },
        new() { Name = "Large", Width = 600, Height = 600 },
        new() { Name = "XLarge", Width = 1200, Height = 1200 }
    };

    /// <summary>
    /// Video transcoding quality presets.
    /// </summary>
    public VideoQualityPreset[] VideoQualityPresets { get; set; } =
    {
        new() { Name = "Low", Width = 640, Height = 360, Bitrate = 800000 },
        new() { Name = "SD", Width = 854, Height = 480, Bitrate = 1500000 },
        new() { Name = "HD", Width = 1280, Height = 720, Bitrate = 3000000 },
        new() { Name = "FullHD", Width = 1920, Height = 1080, Bitrate = 6000000 },
        new() { Name = "QHD", Width = 2560, Height = 1440, Bitrate = 12000000 },
        new() { Name = "UHD", Width = 3840, Height = 2160, Bitrate = 25000000 }
    };

    /// <summary>
    /// FFmpeg path for video processing.
    /// </summary>
    public string FFmpegPath { get; set; } = "/usr/bin/ffmpeg";

    /// <summary>
    /// FFprobe path for video analysis.
    /// </summary>
    public string FFprobePath { get; set; } = "/usr/bin/ffprobe";

    /// <summary>
    /// Enable automatic transcoding for uploaded videos.
    /// </summary>
    public bool AutoTranscodeVideos { get; set; } = true;

    /// <summary>
    /// Default video qualities to generate during auto-transcoding.
    /// </summary>
    public string[] DefaultTranscodeQualities { get; set; } = { "SD", "HD", "FullHD" };

    /// <summary>
    /// CDN base URL for serving media.
    /// </summary>
    public string? CdnBaseUrl { get; set; }
}

/// <summary>
/// Thumbnail configuration.
/// </summary>
public class ThumbnailConfig
{
    public string Name { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
}

/// <summary>
/// Video quality preset configuration.
/// </summary>
public class VideoQualityPreset
{
    public string Name { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public int Bitrate { get; set; }
}
