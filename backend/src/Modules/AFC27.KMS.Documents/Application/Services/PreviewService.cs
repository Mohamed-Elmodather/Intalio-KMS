using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AFC27.KMS.Documents.Application.Interfaces;
using AFC27.KMS.Documents.Domain.Entities;
using PermissionLevel = AFC27.KMS.Documents.Domain.Entities.PermissionLevel;

namespace AFC27.KMS.Documents.Application.Services;

/// <summary>
/// Service for generating and managing document previews.
/// Supports images, PDFs, Office documents, videos, and text files.
/// </summary>
public class PreviewService : IPreviewService
{
    private readonly DbContext _dbContext;
    private readonly IPermissionService _permissionService;
    private readonly ILogger<PreviewService> _logger;
    private readonly PreviewSettings _settings;

    // Mime type to preview type mapping
    private static readonly Dictionary<string, PreviewType> MimeTypeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        // Images
        { "image/jpeg", PreviewType.Image },
        { "image/png", PreviewType.Image },
        { "image/gif", PreviewType.Image },
        { "image/webp", PreviewType.Image },
        { "image/svg+xml", PreviewType.Image },
        { "image/bmp", PreviewType.Image },
        { "image/tiff", PreviewType.Image },

        // PDF
        { "application/pdf", PreviewType.Pdf },

        // Videos
        { "video/mp4", PreviewType.Video },
        { "video/webm", PreviewType.Video },
        { "video/ogg", PreviewType.Video },
        { "video/quicktime", PreviewType.Video },
        { "video/x-msvideo", PreviewType.Video },
        { "video/x-ms-wmv", PreviewType.Video },
        { "video/x-matroska", PreviewType.Video },

        // Audio
        { "audio/mpeg", PreviewType.Audio },
        { "audio/mp3", PreviewType.Audio },
        { "audio/wav", PreviewType.Audio },
        { "audio/ogg", PreviewType.Audio },
        { "audio/webm", PreviewType.Audio },
        { "audio/aac", PreviewType.Audio },
        { "audio/flac", PreviewType.Audio },

        // Office Documents
        { "application/msword", PreviewType.Office },
        { "application/vnd.openxmlformats-officedocument.wordprocessingml.document", PreviewType.Office },
        { "application/vnd.ms-excel", PreviewType.Office },
        { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", PreviewType.Office },
        { "application/vnd.ms-powerpoint", PreviewType.Office },
        { "application/vnd.openxmlformats-officedocument.presentationml.presentation", PreviewType.Office },
        { "application/vnd.oasis.opendocument.text", PreviewType.Office },
        { "application/vnd.oasis.opendocument.spreadsheet", PreviewType.Office },
        { "application/vnd.oasis.opendocument.presentation", PreviewType.Office },

        // Text/Code
        { "text/plain", PreviewType.Text },
        { "text/html", PreviewType.Text },
        { "text/css", PreviewType.Text },
        { "text/javascript", PreviewType.Text },
        { "application/javascript", PreviewType.Text },
        { "application/json", PreviewType.Text },
        { "application/xml", PreviewType.Text },
        { "text/xml", PreviewType.Text },
        { "text/csv", PreviewType.Text },
        { "application/x-yaml", PreviewType.Text },
        { "text/yaml", PreviewType.Text },

        // Markdown
        { "text/markdown", PreviewType.Markdown },
        { "text/x-markdown", PreviewType.Markdown }
    };

    // Extension to preview type mapping (fallback)
    private static readonly Dictionary<string, PreviewType> ExtensionMap = new(StringComparer.OrdinalIgnoreCase)
    {
        // Images
        { ".jpg", PreviewType.Image },
        { ".jpeg", PreviewType.Image },
        { ".png", PreviewType.Image },
        { ".gif", PreviewType.Image },
        { ".webp", PreviewType.Image },
        { ".svg", PreviewType.Image },
        { ".bmp", PreviewType.Image },
        { ".tiff", PreviewType.Image },
        { ".tif", PreviewType.Image },

        // PDF
        { ".pdf", PreviewType.Pdf },

        // Videos
        { ".mp4", PreviewType.Video },
        { ".webm", PreviewType.Video },
        { ".ogv", PreviewType.Video },
        { ".mov", PreviewType.Video },
        { ".avi", PreviewType.Video },
        { ".wmv", PreviewType.Video },
        { ".mkv", PreviewType.Video },
        { ".m4v", PreviewType.Video },

        // Audio
        { ".mp3", PreviewType.Audio },
        { ".wav", PreviewType.Audio },
        { ".ogg", PreviewType.Audio },
        { ".oga", PreviewType.Audio },
        { ".aac", PreviewType.Audio },
        { ".flac", PreviewType.Audio },
        { ".m4a", PreviewType.Audio },
        { ".wma", PreviewType.Audio },

        // Office
        { ".doc", PreviewType.Office },
        { ".docx", PreviewType.Office },
        { ".xls", PreviewType.Office },
        { ".xlsx", PreviewType.Office },
        { ".ppt", PreviewType.Office },
        { ".pptx", PreviewType.Office },
        { ".odt", PreviewType.Office },
        { ".ods", PreviewType.Office },
        { ".odp", PreviewType.Office },

        // Text/Code
        { ".txt", PreviewType.Text },
        { ".html", PreviewType.Text },
        { ".htm", PreviewType.Text },
        { ".css", PreviewType.Text },
        { ".js", PreviewType.Text },
        { ".ts", PreviewType.Text },
        { ".json", PreviewType.Text },
        { ".xml", PreviewType.Text },
        { ".csv", PreviewType.Text },
        { ".yaml", PreviewType.Text },
        { ".yml", PreviewType.Text },
        { ".cs", PreviewType.Text },
        { ".java", PreviewType.Text },
        { ".py", PreviewType.Text },
        { ".rb", PreviewType.Text },
        { ".go", PreviewType.Text },
        { ".rs", PreviewType.Text },
        { ".swift", PreviewType.Text },
        { ".kt", PreviewType.Text },
        { ".php", PreviewType.Text },
        { ".sql", PreviewType.Text },
        { ".sh", PreviewType.Text },
        { ".bash", PreviewType.Text },
        { ".ps1", PreviewType.Text },
        { ".bat", PreviewType.Text },
        { ".cmd", PreviewType.Text },

        // Markdown
        { ".md", PreviewType.Markdown },
        { ".markdown", PreviewType.Markdown }
    };

    public PreviewService(
        DbContext dbContext,
        IPermissionService permissionService,
        IOptions<PreviewSettings> settings,
        ILogger<PreviewService> logger)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
        _settings = settings.Value;
        _logger = logger;
    }

    public bool IsPreviewSupported(string mimeType)
    {
        return GetPreviewType(mimeType) != PreviewType.None;
    }

    public PreviewType GetPreviewType(string mimeType)
    {
        if (string.IsNullOrEmpty(mimeType))
            return PreviewType.None;

        if (MimeTypeMap.TryGetValue(mimeType, out var previewType))
            return previewType;

        return PreviewType.None;
    }

    public PreviewType GetPreviewTypeByExtension(string? extension)
    {
        if (string.IsNullOrEmpty(extension))
            return PreviewType.None;

        var ext = extension.StartsWith('.') ? extension : $".{extension}";

        if (ExtensionMap.TryGetValue(ext, out var previewType))
            return previewType;

        return PreviewType.None;
    }

    public async Task<PreviewResult> GetPreviewAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Check access
        var hasAccess = await _permissionService.HasDocumentAccessAsync(
            documentId, userId, PermissionLevel.Read, cancellationToken);

        if (!hasAccess)
        {
            _logger.LogWarning("User {UserId} denied preview access to document {DocumentId}", userId, documentId);
            return PreviewResult.NotAvailable("Access denied");
        }

        var document = await _dbContext.Set<Document>()
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted, cancellationToken);

        if (document == null)
        {
            return PreviewResult.NotAvailable("Document not found");
        }

        var previewType = GetPreviewType(document.ContentType);
        if (previewType == PreviewType.None)
        {
            previewType = GetPreviewTypeByExtension(document.FileExtension);
        }

        if (previewType == PreviewType.None)
        {
            return PreviewResult.NotAvailable($"Preview not supported for {document.ContentType}");
        }

        // Generate preview URL based on type
        var baseUrl = _settings.BaseUrl.TrimEnd('/');
        var previewUrl = $"{baseUrl}/api/documents/{documentId}/preview/content";
        var thumbnailUrl = document.ThumbnailPath ?? $"{baseUrl}/api/documents/{documentId}/thumbnail";

        var result = new PreviewResult
        {
            IsAvailable = true,
            Type = previewType,
            PreviewUrl = previewUrl,
            ThumbnailUrl = thumbnailUrl,
            ContentType = document.ContentType
        };

        // Add type-specific information
        // TODO: Width, Height, PageCount, Duration properties need to be added to Document entity
        // For now, return basic preview info without these metadata properties
        switch (previewType)
        {
            case PreviewType.Pdf:
            case PreviewType.Office:
                result = result with
                {
                    PageCount = null, // TODO: Add PageCount to Document entity
                    PdfUrl = previewType == PreviewType.Office
                        ? $"{baseUrl}/api/documents/{documentId}/preview/pdf"
                        : previewUrl
                };
                break;

            case PreviewType.Video:
            case PreviewType.Audio:
                result = result with
                {
                    Duration = null, // TODO: Add Duration to Document entity
                    Width = null,    // TODO: Add Width to Document entity
                    Height = null    // TODO: Add Height to Document entity
                };
                break;

            case PreviewType.Image:
                result = result with
                {
                    Width = null,    // TODO: Add Width to Document entity
                    Height = null    // TODO: Add Height to Document entity
                };
                break;
        }

        return result;
    }

    public async Task<string?> GetThumbnailUrlAsync(
        Guid documentId,
        int width = 200,
        int height = 200,
        CancellationToken cancellationToken = default)
    {
        var document = await _dbContext.Set<Document>()
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted, cancellationToken);

        if (document == null)
            return null;

        // If document has a pre-generated thumbnail, use it
        if (!string.IsNullOrEmpty(document.ThumbnailPath))
        {
            return document.ThumbnailPath;
        }

        // Generate dynamic thumbnail URL
        var baseUrl = _settings.BaseUrl.TrimEnd('/');
        return $"{baseUrl}/api/documents/{documentId}/thumbnail?w={width}&h={height}";
    }

    public async Task<StreamingInfo?> GetStreamingInfoAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Check access
        var hasAccess = await _permissionService.HasDocumentAccessAsync(
            documentId, userId, PermissionLevel.Read, cancellationToken);

        if (!hasAccess)
        {
            _logger.LogWarning("User {UserId} denied streaming access to document {DocumentId}", userId, documentId);
            return null;
        }

        var document = await _dbContext.Set<Document>()
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted, cancellationToken);

        if (document == null)
            return null;

        var previewType = GetPreviewType(document.ContentType);
        if (previewType != PreviewType.Video && previewType != PreviewType.Audio)
        {
            return null;
        }

        var baseUrl = _settings.BaseUrl.TrimEnd('/');

        return new StreamingInfo
        {
            StreamUrl = $"{baseUrl}/api/documents/{documentId}/stream",
            Protocol = "Progressive", // For now, use progressive download. HLS/DASH would require transcoding
            ContentType = document.ContentType,
            Duration = 0, // TODO: Add Duration to Document entity
            PosterUrl = document.ThumbnailPath ?? $"{baseUrl}/api/documents/{documentId}/thumbnail",
            QualityLevels = new List<QualityLevel>
            {
                new QualityLevel
                {
                    Label = "Original",
                    Width = 0, // TODO: Add Width to Document entity
                    Height = 0, // TODO: Add Height to Document entity
                    Bitrate = 0, // Original bitrate
                    Url = $"{baseUrl}/api/documents/{documentId}/stream"
                }
            },
            Subtitles = Array.Empty<SubtitleTrack>() // Could be extended to support subtitles
        };
    }

    public async Task<PreviewMetadata?> GetPreviewMetadataAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Check access
        var hasAccess = await _permissionService.HasDocumentAccessAsync(
            documentId, userId, PermissionLevel.Read, cancellationToken);

        if (!hasAccess)
        {
            return null;
        }

        var document = await _dbContext.Set<Document>()
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted, cancellationToken);

        if (document == null)
            return null;

        var previewType = GetPreviewType(document.ContentType);
        if (previewType == PreviewType.None)
        {
            previewType = GetPreviewTypeByExtension(document.FileExtension);
        }

        return new PreviewMetadata
        {
            DocumentId = document.Id,
            FileName = document.Name,
            MimeType = document.ContentType,
            PreviewType = previewType,
            FileSize = document.FileSize,
            PageCount = null, // TODO: Add PageCount to Document entity
            Duration = null, // TODO: Add Duration to Document entity
            Width = null, // TODO: Add Width to Document entity
            Height = null, // TODO: Add Height to Document entity
            ModifiedAt = document.ModifiedAt ?? document.CreatedAt,
            IsPreviewReady = previewType != PreviewType.None,
            PreviewGenerationStatus = previewType != PreviewType.None ? "Ready" : "Not Supported"
        };
    }

    private async Task<int?> GetPageCountAsync(Document document, CancellationToken cancellationToken)
    {
        // TODO: Add PageCount property to Document entity
        // For now, return null. A full implementation would parse the PDF/Office document
        // to extract page count. This could be done during upload processing.
        return null;
    }

    private async Task<double?> GetMediaDurationAsync(Document document, CancellationToken cancellationToken)
    {
        // TODO: Add Duration property to Document entity
        // For now, return null. A full implementation would use FFprobe
        // to extract duration. This could be done during upload processing.
        return null;
    }
}

/// <summary>
/// Preview service configuration settings.
/// </summary>
public class PreviewSettings
{
    /// <summary>
    /// Base URL for generating preview links.
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:5001";

    /// <summary>
    /// Maximum file size for preview generation (in bytes).
    /// </summary>
    public long MaxPreviewSize { get; set; } = 100 * 1024 * 1024; // 100MB

    /// <summary>
    /// Default thumbnail width.
    /// </summary>
    public int DefaultThumbnailWidth { get; set; } = 200;

    /// <summary>
    /// Default thumbnail height.
    /// </summary>
    public int DefaultThumbnailHeight { get; set; } = 200;

    /// <summary>
    /// Enable Office document to PDF conversion.
    /// </summary>
    public bool EnableOfficeToPdfConversion { get; set; } = true;

    /// <summary>
    /// Path to LibreOffice for document conversion.
    /// </summary>
    public string? LibreOfficePath { get; set; }
}

/// <summary>
/// Extension methods for Document entity to support preview features.
/// </summary>
public static class DocumentPreviewExtensions
{
    public static bool HasPreviewMetadata(this Document document)
    {
        // Check if document has preview metadata in its Metadata collection
        return document.Metadata?.Any() == true;
    }
}
