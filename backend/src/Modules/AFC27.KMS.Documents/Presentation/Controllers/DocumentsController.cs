using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Documents.Application.DTOs;
using AFC27.KMS.Documents.Application.Interfaces;
using AFC27.KMS.Documents.Domain.Entities;
using AFC27.KMS.Contracts.Common;
using AFC27.KMS.Infrastructure.Storage;
using System.Security.Claims;

namespace AFC27.KMS.Documents.Presentation.Controllers;

/// <summary>
/// Document management controller.
/// </summary>
[ApiController]
[Route("api/documents")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;
    private readonly IStorageService _storageService;
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(
        IDocumentService documentService,
        IStorageService storageService,
        ILogger<DocumentsController> logger)
    {
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

        // Fallback for development
        return Guid.Empty;
    }

    private string GetCurrentUserName()
    {
        return User.FindFirst(ClaimTypes.Name)?.Value
            ?? User.FindFirst("name")?.Value
            ?? "Unknown User";
    }

    /// <summary>
    /// Get paginated list of documents.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<DocumentSummaryDto>>), 200)]
    public async Task<ActionResult<ApiResponse<PagedResult<DocumentSummaryDto>>>> GetDocuments(
        [FromQuery] DocumentFilterRequest filter,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var result = await _documentService.SearchDocumentsAsync(
            userId,
            filter.LibraryId,
            filter.FolderId,
            filter.SearchTerm,
            filter.Page,
            filter.PageSize,
            cancellationToken);

        return Ok(ApiResponse<PagedResult<DocumentSummaryDto>>.Ok(result));
    }

    /// <summary>
    /// Get recent documents for current user.
    /// </summary>
    [HttpGet("recent")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DocumentSummaryDto>>), 200)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DocumentSummaryDto>>>> GetRecentDocuments(
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        var documents = await _documentService.GetRecentDocumentsAsync(userId, limit, cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<DocumentSummaryDto>>.Ok(documents));
    }

    /// <summary>
    /// Get document by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<DocumentDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<DocumentDto>>> GetDocument(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var document = await _documentService.GetDocumentAsync(id, userId, cancellationToken);

        if (document == null)
        {
            return NotFound(ApiResponse.Fail("Document not found or access denied"));
        }

        return Ok(ApiResponse<DocumentDto>.Ok(document));
    }

    /// <summary>
    /// Upload a new document.
    /// </summary>
    [HttpPost("upload")]
    [Authorize(Policy = "CanUploadDocuments")]
    [ProducesResponseType(typeof(ApiResponse<DocumentDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    public async Task<ActionResult<ApiResponse<DocumentDto>>> UploadDocument(
        [FromForm] IFormFile file,
        [FromForm] UploadDocumentRequest request,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse.Fail("No file provided"));
        }

        var userId = GetCurrentUserId();
        var userName = GetCurrentUserName();

        _logger.LogInformation("User {UserId} uploading document {FileName} to library {LibraryId}",
            userId, file.FileName, request.LibraryId);

        try
        {
            var document = await _documentService.UploadDocumentAsync(
                file.OpenReadStream(),
                file.FileName,
                file.ContentType,
                request.LibraryId,
                request.FolderId,
                request.Name ?? Path.GetFileNameWithoutExtension(file.FileName),
                request.NameArabic,
                request.Description,
                request.DescriptionArabic,
                request.Tags,
                userId,
                userName,
                cancellationToken);

            return CreatedAtAction(nameof(GetDocument), new { id = document.Id }, ApiResponse<DocumentDto>.Ok(document));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Update document metadata.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CanEditDocuments")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse>> UpdateDocument(
        Guid id,
        [FromBody] UpdateDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} updating document {DocumentId}", userId, id);

        var success = await _documentService.UpdateDocumentAsync(
            id,
            request.Name,
            request.NameArabic,
            request.Description,
            request.DescriptionArabic,
            request.Tags,
            userId,
            cancellationToken);

        if (!success)
        {
            return NotFound(ApiResponse.Fail("Document not found or access denied"));
        }

        return Ok(ApiResponse.Ok("Document updated successfully"));
    }

    /// <summary>
    /// Delete a document.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "CanDeleteDocuments")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse>> DeleteDocument(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} deleting document {DocumentId}", userId, id);

        var success = await _documentService.DeleteDocumentAsync(id, userId, cancellationToken);

        if (!success)
        {
            return NotFound(ApiResponse.Fail("Document not found or access denied"));
        }

        return Ok(ApiResponse.Ok("Document deleted successfully"));
    }

    /// <summary>
    /// Download a document.
    /// </summary>
    [HttpGet("{id:guid}/download")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DownloadDocument(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} downloading document {DocumentId}", userId, id);

        var document = await _documentService.GetDocumentAsync(id, userId, cancellationToken);
        if (document == null)
        {
            return NotFound();
        }

        var stream = await _storageService.DownloadAsync(document.StoragePath, cancellationToken);
        if (stream == null)
        {
            _logger.LogError("Document {DocumentId} not found in storage", id);
            return NotFound();
        }

        // Log download
        await _documentService.LogDownloadAsync(id, userId, cancellationToken);

        return File(stream, document.ContentType ?? "application/octet-stream", document.FileName);
    }

    /// <summary>
    /// Check out a document.
    /// </summary>
    [HttpPost("{id:guid}/checkout")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse>> CheckoutDocument(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var userName = GetCurrentUserName();

        _logger.LogInformation("User {UserId} checking out document {DocumentId}", userId, id);

        try
        {
            var success = await _documentService.CheckOutDocumentAsync(id, userId, userName, cancellationToken);

            if (!success)
            {
                return NotFound(ApiResponse.Fail("Document not found or access denied"));
            }

            return Ok(ApiResponse.Ok("Document checked out"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Check in a document with a new version.
    /// </summary>
    [HttpPost("{id:guid}/checkin")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse>> CheckinDocument(
        Guid id,
        [FromForm] IFormFile? file,
        [FromForm] CheckInDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var userName = GetCurrentUserName();

        _logger.LogInformation("User {UserId} checking in document {DocumentId}", userId, id);

        try
        {
            Stream? fileStream = file?.OpenReadStream();
            string? fileName = file?.FileName;
            string? contentType = file?.ContentType;

            var success = await _documentService.CheckInDocumentAsync(
                id,
                fileStream,
                fileName,
                contentType,
                request.IsMajorVersion,
                request.Comments,
                userId,
                userName,
                cancellationToken);

            if (!success)
            {
                return NotFound(ApiResponse.Fail("Document not found or access denied"));
            }

            return Ok(ApiResponse.Ok("Document checked in"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Discard checkout.
    /// </summary>
    [HttpPost("{id:guid}/discard-checkout")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse>> DiscardCheckout(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} discarding checkout for document {DocumentId}", userId, id);

        try
        {
            var success = await _documentService.DiscardCheckoutAsync(id, userId, cancellationToken);

            if (!success)
            {
                return NotFound(ApiResponse.Fail("Document not found or access denied"));
            }

            return Ok(ApiResponse.Ok("Checkout discarded"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Get document versions.
    /// </summary>
    [HttpGet("{id:guid}/versions")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DocumentVersionDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DocumentVersionDto>>>> GetVersions(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var versions = await _documentService.GetDocumentVersionsAsync(id, userId, cancellationToken);

        if (versions == null)
        {
            return NotFound(ApiResponse.Fail("Document not found or access denied"));
        }

        return Ok(ApiResponse<IReadOnlyList<DocumentVersionDto>>.Ok(versions));
    }

    /// <summary>
    /// Download a specific version.
    /// </summary>
    [HttpGet("{id:guid}/versions/{versionId:guid}/download")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DownloadVersion(
        Guid id,
        Guid versionId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var result = await _documentService.DownloadVersionAsync(id, versionId, userId, cancellationToken);

        if (result == null)
        {
            return NotFound();
        }

        return File(result.Value.Stream, result.Value.ContentType, result.Value.FileName);
    }

    /// <summary>
    /// Restore a specific version.
    /// </summary>
    [HttpPost("{id:guid}/versions/{versionId:guid}/restore")]
    [Authorize(Policy = "CanEditDocuments")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse>> RestoreVersion(
        Guid id,
        Guid versionId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var userName = GetCurrentUserName();

        _logger.LogInformation("User {UserId} restoring document {DocumentId} to version {VersionId}",
            userId, id, versionId);

        var success = await _documentService.RestoreVersionAsync(id, versionId, userId, userName, cancellationToken);

        if (!success)
        {
            return NotFound(ApiResponse.Fail("Document or version not found, or access denied"));
        }

        return Ok(ApiResponse.Ok("Version restored"));
    }

    /// <summary>
    /// Get document audit trail.
    /// </summary>
    [HttpGet("{id:guid}/audit")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DocumentAuditDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DocumentAuditDto>>>> GetAuditTrail(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var auditTrail = await _documentService.GetAuditTrailAsync(id, userId, cancellationToken);

        if (auditTrail == null)
        {
            return NotFound(ApiResponse.Fail("Document not found or access denied"));
        }

        return Ok(ApiResponse<IReadOnlyList<DocumentAuditDto>>.Ok(auditTrail));
    }

    /// <summary>
    /// Move document to another folder.
    /// </summary>
    [HttpPost("{id:guid}/move")]
    [Authorize(Policy = "CanEditDocuments")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse>> MoveDocument(
        Guid id,
        [FromBody] MoveDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} moving document {DocumentId} to folder {FolderId}",
            userId, id, request.TargetFolderId);

        var success = await _documentService.MoveDocumentAsync(
            id,
            request.TargetFolderId,
            userId,
            cancellationToken);

        if (!success)
        {
            return NotFound(ApiResponse.Fail("Document or folder not found, or access denied"));
        }

        return Ok(ApiResponse.Ok("Document moved successfully"));
    }

    /// <summary>
    /// Copy document.
    /// </summary>
    [HttpPost("{id:guid}/copy")]
    [Authorize(Policy = "CanUploadDocuments")]
    [ProducesResponseType(typeof(ApiResponse<DocumentDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<DocumentDto>>> CopyDocument(
        Guid id,
        [FromBody] MoveDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var userName = GetCurrentUserName();

        _logger.LogInformation("User {UserId} copying document {DocumentId} to folder {FolderId}",
            userId, id, request.TargetFolderId);

        var document = await _documentService.CopyDocumentAsync(
            id,
            request.TargetFolderId,
            userId,
            userName,
            cancellationToken);

        if (document == null)
        {
            return NotFound(ApiResponse.Fail("Document or folder not found, or access denied"));
        }

        return Ok(ApiResponse<DocumentDto>.Ok(document));
    }

    /// <summary>
    /// Publish a document.
    /// </summary>
    [HttpPost("{id:guid}/publish")]
    [Authorize(Policy = "CanPublishDocuments")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse>> PublishDocument(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} publishing document {DocumentId}", userId, id);

        var success = await _documentService.PublishDocumentAsync(id, userId, cancellationToken);

        if (!success)
        {
            return NotFound(ApiResponse.Fail("Document not found or access denied"));
        }

        return Ok(ApiResponse.Ok("Document published"));
    }

    /// <summary>
    /// Archive a document.
    /// </summary>
    [HttpPost("{id:guid}/archive")]
    [Authorize(Policy = "CanEditDocuments")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse>> ArchiveDocument(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} archiving document {DocumentId}", userId, id);

        var success = await _documentService.ArchiveDocumentAsync(id, userId, cancellationToken);

        if (!success)
        {
            return NotFound(ApiResponse.Fail("Document not found or access denied"));
        }

        return Ok(ApiResponse.Ok("Document archived"));
    }

    /// <summary>
    /// Bulk delete documents.
    /// </summary>
    [HttpPost("bulk/delete")]
    [Authorize(Policy = "CanDeleteDocuments")]
    [ProducesResponseType(typeof(ApiResponse<BulkOperationResult>), 200)]
    public async Task<ActionResult<ApiResponse<BulkOperationResult>>> BulkDeleteDocuments(
        [FromBody] BulkDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} bulk deleting {Count} documents", userId, request.DocumentIds.Count);

        var successCount = 0;
        var failedIds = new List<Guid>();

        foreach (var documentId in request.DocumentIds)
        {
            var success = await _documentService.DeleteDocumentAsync(documentId, userId, cancellationToken);
            if (success)
                successCount++;
            else
                failedIds.Add(documentId);
        }

        var result = new BulkOperationResult
        {
            SuccessCount = successCount,
            FailedCount = failedIds.Count,
            FailedIds = failedIds
        };

        return Ok(ApiResponse<BulkOperationResult>.Ok(result));
    }

    /// <summary>
    /// Bulk move documents.
    /// </summary>
    [HttpPost("bulk/move")]
    [Authorize(Policy = "CanEditDocuments")]
    [ProducesResponseType(typeof(ApiResponse<BulkOperationResult>), 200)]
    public async Task<ActionResult<ApiResponse<BulkOperationResult>>> BulkMoveDocuments(
        [FromBody] BulkMoveRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} bulk moving {Count} documents to folder {FolderId}",
            userId, request.DocumentIds.Count, request.TargetFolderId);

        var successCount = 0;
        var failedIds = new List<Guid>();

        foreach (var documentId in request.DocumentIds)
        {
            var success = await _documentService.MoveDocumentAsync(documentId, request.TargetFolderId, userId, cancellationToken);
            if (success)
                successCount++;
            else
                failedIds.Add(documentId);
        }

        var result = new BulkOperationResult
        {
            SuccessCount = successCount,
            FailedCount = failedIds.Count,
            FailedIds = failedIds
        };

        return Ok(ApiResponse<BulkOperationResult>.Ok(result));
    }
}

/// <summary>
/// Document filter request.
/// </summary>
public record DocumentFilterRequest
{
    public Guid? LibraryId { get; init; }
    public Guid? FolderId { get; init; }
    public string? SearchTerm { get; init; }
    public string? Status { get; init; }
    public string? FileType { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

/// <summary>
/// Bulk document request.
/// </summary>
public record BulkDocumentRequest
{
    public IReadOnlyList<Guid> DocumentIds { get; init; } = Array.Empty<Guid>();
}

/// <summary>
/// Bulk move request.
/// </summary>
public record BulkMoveRequest
{
    public IReadOnlyList<Guid> DocumentIds { get; init; } = Array.Empty<Guid>();
    public Guid? TargetFolderId { get; init; }
}

/// <summary>
/// Bulk operation result.
/// </summary>
public record BulkOperationResult
{
    public int SuccessCount { get; init; }
    public int FailedCount { get; init; }
    public IReadOnlyList<Guid> FailedIds { get; init; } = Array.Empty<Guid>();
}
