using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Documents.Application.DTOs;
using AFC27.KMS.Contracts.Common;

namespace AFC27.KMS.Documents.Presentation.Controllers;

/// <summary>
/// Document management controller.
/// </summary>
[ApiController]
[Route("api/documents")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(ILogger<DocumentsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of documents.
    /// </summary>
    [HttpGet]
    public ActionResult<ApiResponse<PagedResult<DocumentSummaryDto>>> GetDocuments([FromQuery] DocumentFilterRequest filter)
    {
        var documents = new List<DocumentSummaryDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Tournament Operations Manual",
                NameArabic = "دليل عمليات البطولة",
                FileName = "tournament-operations-manual-v2.pdf",
                FileExtension = ".pdf",
                FileSize = 5242880, // 5 MB
                ThumbnailUrl = "/thumbnails/pdf-icon.png",
                Version = "2.1",
                Status = "Published",
                IsCheckedOut = false,
                CreatedByName = "Mohammed Al-Rashid",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Venue Specifications",
                NameArabic = "مواصفات الملاعب",
                FileName = "venue-specifications.xlsx",
                FileExtension = ".xlsx",
                FileSize = 2097152, // 2 MB
                ThumbnailUrl = "/thumbnails/excel-icon.png",
                Version = "1.3",
                Status = "CheckedOut",
                IsCheckedOut = true,
                CheckedOutByName = "Sara Ali",
                CreatedByName = "Ahmed Hassan",
                CreatedAt = DateTime.UtcNow.AddDays(-45),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Brand Guidelines",
                NameArabic = "إرشادات العلامة التجارية",
                FileName = "afc27-brand-guidelines.pdf",
                FileExtension = ".pdf",
                FileSize = 15728640, // 15 MB
                Version = "1.0",
                Status = "Published",
                IsCheckedOut = false,
                CreatedByName = "Fatima Al-Saud",
                CreatedAt = DateTime.UtcNow.AddDays(-60)
            }
        };

        var result = PagedResult<DocumentSummaryDto>.Create(documents, 250, filter.Page, filter.PageSize);
        return Ok(ApiResponse<PagedResult<DocumentSummaryDto>>.Ok(result));
    }

    /// <summary>
    /// Get recent documents for current user.
    /// </summary>
    [HttpGet("recent")]
    public ActionResult<ApiResponse<IReadOnlyList<DocumentSummaryDto>>> GetRecentDocuments([FromQuery] int limit = 10)
    {
        var documents = new List<DocumentSummaryDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Tournament Operations Manual",
                FileName = "tournament-operations-manual-v2.pdf",
                FileExtension = ".pdf",
                FileSize = 5242880,
                Version = "2.1",
                Status = "Published",
                UpdatedAt = DateTime.UtcNow.AddHours(-2)
            }
        };

        return Ok(ApiResponse<IReadOnlyList<DocumentSummaryDto>>.Ok(documents));
    }

    /// <summary>
    /// Get document by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public ActionResult<ApiResponse<DocumentDto>> GetDocument(Guid id)
    {
        var document = new DocumentDto
        {
            Id = id,
            Name = "Tournament Operations Manual",
            NameArabic = "دليل عمليات البطولة",
            Description = "Complete operational guidelines for the AFC Asian Cup 2027",
            DescriptionArabic = "الإرشادات التشغيلية الكاملة لكأس آسيا 2027",
            FileName = "tournament-operations-manual-v2.pdf",
            FileExtension = ".pdf",
            ContentType = "application/pdf",
            FileSize = 5242880,
            ThumbnailUrl = "/thumbnails/pdf-icon.png",
            LibraryId = Guid.NewGuid(),
            LibraryName = "Operations Documents",
            FolderId = Guid.NewGuid(),
            FolderPath = "Operations/Manuals",
            Version = "2.1",
            Status = "Published",
            RequiresApproval = true,
            DownloadCount = 156,
            ViewCount = 423,
            CreatedById = Guid.NewGuid(),
            CreatedByName = "Mohammed Al-Rashid",
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-5),
            Metadata = new List<DocumentMetadataDto>
            {
                new() { Key = "Department", Value = "Operations", DataType = "String" },
                new() { Key = "Confidentiality", Value = "Internal", DataType = "String" },
                new() { Key = "ExpiryDate", Value = "2027-12-31", DataType = "Date" }
            }
        };

        return Ok(ApiResponse<DocumentDto>.Ok(document));
    }

    /// <summary>
    /// Upload a new document.
    /// </summary>
    [HttpPost("upload")]
    [Authorize(Policy = "CanUploadDocuments")]
    public async Task<ActionResult<ApiResponse<DocumentDto>>> UploadDocument(
        [FromForm] IFormFile file,
        [FromForm] UploadDocumentRequest request)
    {
        _logger.LogInformation("Uploading document {FileName} to library {LibraryId}", file.FileName, request.LibraryId);

        await Task.Delay(100);

        var document = new DocumentDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name ?? Path.GetFileNameWithoutExtension(file.FileName),
            NameArabic = request.NameArabic,
            Description = request.Description,
            FileName = file.FileName,
            FileExtension = Path.GetExtension(file.FileName),
            ContentType = file.ContentType,
            FileSize = file.Length,
            LibraryId = request.LibraryId,
            FolderId = request.FolderId,
            Version = "1.0",
            Status = "Draft",
            CreatedById = Guid.NewGuid(),
            CreatedByName = "Current User",
            CreatedAt = DateTime.UtcNow
        };

        return CreatedAtAction(nameof(GetDocument), new { id = document.Id }, ApiResponse<DocumentDto>.Ok(document));
    }

    /// <summary>
    /// Update document metadata.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CanEditDocuments")]
    public async Task<ActionResult<ApiResponse>> UpdateDocument(Guid id, [FromBody] UpdateDocumentRequest request)
    {
        _logger.LogInformation("Updating document {DocumentId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Document updated successfully"));
    }

    /// <summary>
    /// Delete a document.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "CanDeleteDocuments")]
    public async Task<ActionResult<ApiResponse>> DeleteDocument(Guid id)
    {
        _logger.LogInformation("Deleting document {DocumentId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Document deleted successfully"));
    }

    /// <summary>
    /// Download a document.
    /// </summary>
    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> DownloadDocument(Guid id)
    {
        _logger.LogInformation("Downloading document {DocumentId}", id);

        await Task.Delay(100);

        // In real implementation, this would stream the file
        var bytes = new byte[] { 0x25, 0x50, 0x44, 0x46 }; // PDF magic bytes
        return File(bytes, "application/pdf", "document.pdf");
    }

    /// <summary>
    /// Get document preview URL.
    /// </summary>
    [HttpGet("{id:guid}/preview")]
    public ActionResult<ApiResponse<string>> GetPreviewUrl(Guid id)
    {
        var previewUrl = $"/api/documents/{id}/preview/content";
        return Ok(ApiResponse<string>.Ok(previewUrl));
    }

    /// <summary>
    /// Check out a document.
    /// </summary>
    [HttpPost("{id:guid}/checkout")]
    public async Task<ActionResult<ApiResponse>> CheckoutDocument(Guid id)
    {
        _logger.LogInformation("Checking out document {DocumentId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Document checked out"));
    }

    /// <summary>
    /// Check in a document with a new version.
    /// </summary>
    [HttpPost("{id:guid}/checkin")]
    public async Task<ActionResult<ApiResponse>> CheckinDocument(
        Guid id,
        [FromForm] IFormFile? file,
        [FromForm] CheckInDocumentRequest request)
    {
        _logger.LogInformation("Checking in document {DocumentId}, Major version: {IsMajor}", id, request.IsMajorVersion);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Document checked in"));
    }

    /// <summary>
    /// Discard checkout.
    /// </summary>
    [HttpPost("{id:guid}/discard-checkout")]
    public async Task<ActionResult<ApiResponse>> DiscardCheckout(Guid id)
    {
        _logger.LogInformation("Discarding checkout for document {DocumentId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Checkout discarded"));
    }

    /// <summary>
    /// Get document versions.
    /// </summary>
    [HttpGet("{id:guid}/versions")]
    public ActionResult<ApiResponse<IReadOnlyList<DocumentVersionDto>>> GetVersions(Guid id)
    {
        var versions = new List<DocumentVersionDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Version = "2.1",
                FileName = "tournament-operations-manual-v2.pdf",
                FileSize = 5242880,
                CreatedByName = "Mohammed Al-Rashid",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                ChangeNotes = "Updated security protocols"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Version = "2.0",
                FileName = "tournament-operations-manual-v2.pdf",
                FileSize = 5100000,
                CreatedByName = "Sara Ali",
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                ChangeNotes = "Major revision - added new chapters"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Version = "1.0",
                FileName = "tournament-operations-manual.pdf",
                FileSize = 4800000,
                CreatedByName = "Mohammed Al-Rashid",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                ChangeNotes = "Initial version"
            }
        };

        return Ok(ApiResponse<IReadOnlyList<DocumentVersionDto>>.Ok(versions));
    }

    /// <summary>
    /// Restore a specific version.
    /// </summary>
    [HttpPost("{id:guid}/versions/{versionId:guid}/restore")]
    [Authorize(Policy = "CanEditDocuments")]
    public async Task<ActionResult<ApiResponse>> RestoreVersion(Guid id, Guid versionId)
    {
        _logger.LogInformation("Restoring document {DocumentId} to version {VersionId}", id, versionId);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Version restored"));
    }

    /// <summary>
    /// Get document audit trail.
    /// </summary>
    [HttpGet("{id:guid}/audit")]
    public ActionResult<ApiResponse<IReadOnlyList<DocumentAuditDto>>> GetAuditTrail(Guid id)
    {
        var auditTrail = new List<DocumentAuditDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Action = "Downloaded",
                PerformedByName = "Ahmed Hassan",
                PerformedAt = DateTime.UtcNow.AddHours(-2)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Action = "Viewed",
                PerformedByName = "Sara Ali",
                PerformedAt = DateTime.UtcNow.AddHours(-5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Action = "Checked In",
                Details = "Version 2.1 - Updated security protocols",
                PerformedByName = "Mohammed Al-Rashid",
                PerformedAt = DateTime.UtcNow.AddDays(-5)
            }
        };

        return Ok(ApiResponse<IReadOnlyList<DocumentAuditDto>>.Ok(auditTrail));
    }

    /// <summary>
    /// Move document to another folder.
    /// </summary>
    [HttpPost("{id:guid}/move")]
    [Authorize(Policy = "CanEditDocuments")]
    public async Task<ActionResult<ApiResponse>> MoveDocument(Guid id, [FromBody] MoveDocumentRequest request)
    {
        _logger.LogInformation("Moving document {DocumentId} to folder {FolderId}", id, request.TargetFolderId);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Document moved successfully"));
    }

    /// <summary>
    /// Copy document.
    /// </summary>
    [HttpPost("{id:guid}/copy")]
    [Authorize(Policy = "CanUploadDocuments")]
    public async Task<ActionResult<ApiResponse<DocumentDto>>> CopyDocument(Guid id, [FromBody] MoveDocumentRequest request)
    {
        _logger.LogInformation("Copying document {DocumentId} to folder {FolderId}", id, request.TargetFolderId);

        await Task.Delay(100);

        var document = new DocumentDto
        {
            Id = Guid.NewGuid(),
            Name = "Tournament Operations Manual (Copy)",
            FileName = "tournament-operations-manual-v2-copy.pdf",
            FileExtension = ".pdf",
            FileSize = 5242880,
            Version = "1.0",
            Status = "Draft",
            CreatedAt = DateTime.UtcNow
        };

        return Ok(ApiResponse<DocumentDto>.Ok(document));
    }

    /// <summary>
    /// Publish a document.
    /// </summary>
    [HttpPost("{id:guid}/publish")]
    [Authorize(Policy = "CanPublishDocuments")]
    public async Task<ActionResult<ApiResponse>> PublishDocument(Guid id)
    {
        _logger.LogInformation("Publishing document {DocumentId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Document published"));
    }

    /// <summary>
    /// Archive a document.
    /// </summary>
    [HttpPost("{id:guid}/archive")]
    [Authorize(Policy = "CanEditDocuments")]
    public async Task<ActionResult<ApiResponse>> ArchiveDocument(Guid id)
    {
        _logger.LogInformation("Archiving document {DocumentId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Document archived"));
    }
}
