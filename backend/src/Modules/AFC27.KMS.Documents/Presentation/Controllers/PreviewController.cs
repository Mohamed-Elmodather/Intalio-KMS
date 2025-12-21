using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Documents.Application.Interfaces;
using AFC27.KMS.Contracts.Common;
using AFC27.KMS.Infrastructure.Storage;
using System.Security.Claims;

namespace AFC27.KMS.Documents.Presentation.Controllers;

/// <summary>
/// Document preview controller.
/// Provides preview URLs, thumbnails, and streaming for various document types.
/// </summary>
[ApiController]
[Route("api/documents")]
[Authorize]
public class PreviewController : ControllerBase
{
    private readonly IPreviewService _previewService;
    private readonly IDocumentService _documentService;
    private readonly IStorageService _storageService;
    private readonly ILogger<PreviewController> _logger;

    public PreviewController(
        IPreviewService previewService,
        IDocumentService documentService,
        IStorageService storageService,
        ILogger<PreviewController> logger)
    {
        _previewService = previewService;
        _documentService = documentService;
        _storageService = storageService;
        _logger = logger;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;

        // Fallback for development - in production this should throw
        return Guid.Empty;
    }

    /// <summary>
    /// Get preview information for a document.
    /// </summary>
    [HttpGet("{id:guid}/preview")]
    [ProducesResponseType(typeof(ApiResponse<PreviewResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<PreviewResult>>> GetPreview(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _previewService.GetPreviewAsync(id, userId, cancellationToken);

        if (!result.IsAvailable)
        {
            return NotFound(ApiResponse.Fail(result.ErrorMessage ?? "Preview not available"));
        }

        return Ok(ApiResponse<PreviewResult>.Ok(result));
    }

    /// <summary>
    /// Get preview content (actual file content for embedding).
    /// </summary>
    [HttpGet("{id:guid}/preview/content")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> GetPreviewContent(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        // Get document to check access and get storage path
        var document = await _documentService.GetDocumentAsync(id, userId, cancellationToken);
        if (document == null)
        {
            return NotFound();
        }

        // Get the file stream from storage
        var stream = await _storageService.DownloadAsync(document.StoragePath, cancellationToken);
        if (stream == null)
        {
            _logger.LogError("Document {DocumentId} not found in storage at path {Path}", id, document.StoragePath);
            return NotFound();
        }

        // Log view
        await _documentService.LogViewAsync(id, userId, cancellationToken);

        // Return file with appropriate content type
        return File(stream, document.ContentType ?? "application/octet-stream");
    }

    /// <summary>
    /// Get PDF version of Office documents.
    /// </summary>
    [HttpGet("{id:guid}/preview/pdf")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetPdfPreview(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var preview = await _previewService.GetPreviewAsync(id, userId, cancellationToken);

        if (!preview.IsAvailable)
        {
            return NotFound();
        }

        if (preview.Type != PreviewType.Office && preview.Type != PreviewType.Pdf)
        {
            return BadRequest(ApiResponse.Fail("PDF preview only available for Office and PDF documents"));
        }

        // For Office documents, we would need to convert to PDF
        // For now, return the original if it's already a PDF, or 501 for Office docs
        if (preview.Type == PreviewType.Office)
        {
            // TODO: Implement Office to PDF conversion using LibreOffice or similar
            return StatusCode(501, ApiResponse.Fail("Office to PDF conversion not yet implemented"));
        }

        // It's a PDF, redirect to content endpoint
        return RedirectToAction(nameof(GetPreviewContent), new { id });
    }

    /// <summary>
    /// Get document thumbnail.
    /// </summary>
    [HttpGet("{id:guid}/thumbnail")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [AllowAnonymous] // Thumbnails might need to be public for some use cases
    public async Task<IActionResult> GetThumbnail(
        Guid id,
        [FromQuery] int w = 200,
        [FromQuery] int h = 200,
        CancellationToken cancellationToken = default)
    {
        var thumbnailUrl = await _previewService.GetThumbnailUrlAsync(id, w, h, cancellationToken);

        if (string.IsNullOrEmpty(thumbnailUrl))
        {
            // Return default thumbnail based on file type
            return NotFound();
        }

        // If it's an external URL, redirect
        if (thumbnailUrl.StartsWith("http"))
        {
            return Redirect(thumbnailUrl);
        }

        // Try to get from storage
        var thumbnailPath = GetThumbnailPath(id, w, h);
        var stream = await _storageService.DownloadAsync(thumbnailPath, cancellationToken);

        if (stream == null)
        {
            // TODO: Generate thumbnail on-the-fly if not cached
            return NotFound();
        }

        return File(stream, "image/png");
    }

    /// <summary>
    /// Get streaming information for video/audio.
    /// </summary>
    [HttpGet("{id:guid}/streaming")]
    [ProducesResponseType(typeof(ApiResponse<StreamingInfo>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<StreamingInfo>>> GetStreamingInfo(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var streamingInfo = await _previewService.GetStreamingInfoAsync(id, userId, cancellationToken);

        if (streamingInfo == null)
        {
            return NotFound(ApiResponse.Fail("Streaming not available for this document"));
        }

        return Ok(ApiResponse<StreamingInfo>.Ok(streamingInfo));
    }

    /// <summary>
    /// Stream video/audio content.
    /// </summary>
    [HttpGet("{id:guid}/stream")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> StreamContent(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var document = await _documentService.GetDocumentAsync(id, userId, cancellationToken);
        if (document == null)
        {
            return NotFound();
        }

        var previewType = _previewService.GetPreviewType(document.ContentType ?? string.Empty);
        if (previewType != PreviewType.Video && previewType != PreviewType.Audio)
        {
            return BadRequest(ApiResponse.Fail("Streaming only available for video and audio content"));
        }

        var stream = await _storageService.DownloadAsync(document.StoragePath, cancellationToken);
        if (stream == null)
        {
            return NotFound();
        }

        // Support range requests for seeking
        var contentType = document.ContentType ?? "application/octet-stream";

        // Log view
        await _documentService.LogViewAsync(id, userId, cancellationToken);

        return File(stream, contentType, enableRangeProcessing: true);
    }

    /// <summary>
    /// Get preview metadata.
    /// </summary>
    [HttpGet("{id:guid}/preview/metadata")]
    [ProducesResponseType(typeof(ApiResponse<PreviewMetadata>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<PreviewMetadata>>> GetPreviewMetadata(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var metadata = await _previewService.GetPreviewMetadataAsync(id, userId, cancellationToken);

        if (metadata == null)
        {
            return NotFound(ApiResponse.Fail("Document not found or access denied"));
        }

        return Ok(ApiResponse<PreviewMetadata>.Ok(metadata));
    }

    /// <summary>
    /// Check if preview is supported for a mime type.
    /// </summary>
    [HttpGet("preview/supported")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PreviewSupportInfo>), 200)]
    public ActionResult<ApiResponse<PreviewSupportInfo>> CheckPreviewSupport([FromQuery] string mimeType)
    {
        var isSupported = _previewService.IsPreviewSupported(mimeType);
        var previewType = _previewService.GetPreviewType(mimeType);

        var result = new PreviewSupportInfo
        {
            MimeType = mimeType,
            IsSupported = isSupported,
            PreviewType = previewType.ToString()
        };

        return Ok(ApiResponse<PreviewSupportInfo>.Ok(result));
    }

    private string GetThumbnailPath(Guid documentId, int width, int height)
    {
        return $"thumbnails/{documentId}/{width}x{height}.png";
    }
}

/// <summary>
/// Preview support information response.
/// </summary>
public record PreviewSupportInfo
{
    public string MimeType { get; init; } = string.Empty;
    public bool IsSupported { get; init; }
    public string PreviewType { get; init; } = string.Empty;
}
