using AFC27.KMS.Documents.Domain.Entities;

namespace AFC27.KMS.Documents.Application.Interfaces;

/// <summary>
/// Service interface for document preview generation.
/// Supports various document types including Office, PDF, images, and videos.
/// </summary>
public interface IPreviewService
{
    /// <summary>
    /// Checks if a document type is supported for preview.
    /// </summary>
    bool IsPreviewSupported(string mimeType);

    /// <summary>
    /// Gets the preview type for a document based on its mime type.
    /// </summary>
    PreviewType GetPreviewType(string mimeType);

    /// <summary>
    /// Generates a preview URL for a document.
    /// May return a direct URL, a streaming URL, or a conversion URL.
    /// </summary>
    Task<PreviewResult> GetPreviewAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a thumbnail URL for a document.
    /// </summary>
    Task<string?> GetThumbnailUrlAsync(
        Guid documentId,
        int width = 200,
        int height = 200,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a streaming URL for video/audio content.
    /// </summary>
    Task<StreamingInfo?> GetStreamingInfoAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets preview metadata for a document.
    /// </summary>
    Task<PreviewMetadata?> GetPreviewMetadataAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Preview type classification.
/// </summary>
public enum PreviewType
{
    /// <summary>Preview not available</summary>
    None,
    /// <summary>Image preview (direct display)</summary>
    Image,
    /// <summary>PDF preview (embedded viewer)</summary>
    Pdf,
    /// <summary>Video preview (streaming player)</summary>
    Video,
    /// <summary>Audio preview (audio player)</summary>
    Audio,
    /// <summary>Office document (requires conversion)</summary>
    Office,
    /// <summary>Text/Code preview (syntax highlighting)</summary>
    Text,
    /// <summary>Markdown preview (rendered HTML)</summary>
    Markdown
}

/// <summary>
/// Preview generation result.
/// </summary>
public record PreviewResult
{
    /// <summary>Indicates if preview is available</summary>
    public bool IsAvailable { get; init; }

    /// <summary>Type of preview</summary>
    public PreviewType Type { get; init; }

    /// <summary>URL to access the preview</summary>
    public string? PreviewUrl { get; init; }

    /// <summary>URL to access the thumbnail</summary>
    public string? ThumbnailUrl { get; init; }

    /// <summary>For Office documents, URL to PDF version</summary>
    public string? PdfUrl { get; init; }

    /// <summary>Content type of the preview</summary>
    public string? ContentType { get; init; }

    /// <summary>Page count for multi-page documents</summary>
    public int? PageCount { get; init; }

    /// <summary>Duration in seconds for video/audio</summary>
    public double? Duration { get; init; }

    /// <summary>Width in pixels for images/videos</summary>
    public int? Width { get; init; }

    /// <summary>Height in pixels for images/videos</summary>
    public int? Height { get; init; }

    /// <summary>Error message if preview generation failed</summary>
    public string? ErrorMessage { get; init; }

    public static PreviewResult NotAvailable(string? reason = null) => new()
    {
        IsAvailable = false,
        Type = PreviewType.None,
        ErrorMessage = reason ?? "Preview not available for this document type"
    };

    public static PreviewResult Available(PreviewType type, string previewUrl) => new()
    {
        IsAvailable = true,
        Type = type,
        PreviewUrl = previewUrl
    };
}

/// <summary>
/// Streaming information for video/audio content.
/// </summary>
public record StreamingInfo
{
    /// <summary>Streaming URL (HLS/DASH)</summary>
    public string StreamUrl { get; init; } = string.Empty;

    /// <summary>Streaming protocol (HLS, DASH, Progressive)</summary>
    public string Protocol { get; init; } = "Progressive";

    /// <summary>Content type</summary>
    public string ContentType { get; init; } = string.Empty;

    /// <summary>Duration in seconds</summary>
    public double Duration { get; init; }

    /// <summary>Available quality levels</summary>
    public IReadOnlyList<QualityLevel> QualityLevels { get; init; } = Array.Empty<QualityLevel>();

    /// <summary>Poster/thumbnail image URL</summary>
    public string? PosterUrl { get; init; }

    /// <summary>Subtitles/captions tracks</summary>
    public IReadOnlyList<SubtitleTrack> Subtitles { get; init; } = Array.Empty<SubtitleTrack>();
}

/// <summary>
/// Video quality level information.
/// </summary>
public record QualityLevel
{
    public string Label { get; init; } = string.Empty;
    public int Width { get; init; }
    public int Height { get; init; }
    public int Bitrate { get; init; }
    public string Url { get; init; } = string.Empty;
}

/// <summary>
/// Subtitle track information.
/// </summary>
public record SubtitleTrack
{
    public string Label { get; init; } = string.Empty;
    public string Language { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public bool IsDefault { get; init; }
}

/// <summary>
/// Preview metadata.
/// </summary>
public record PreviewMetadata
{
    public Guid DocumentId { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string MimeType { get; init; } = string.Empty;
    public PreviewType PreviewType { get; init; }
    public long FileSize { get; init; }
    public int? PageCount { get; init; }
    public double? Duration { get; init; }
    public int? Width { get; init; }
    public int? Height { get; init; }
    public DateTime ModifiedAt { get; init; }
    public bool IsPreviewReady { get; init; }
    public string? PreviewGenerationStatus { get; init; }
}
