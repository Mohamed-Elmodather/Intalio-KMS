using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Documents.Application.DTOs;
using AFC27.KMS.Contracts.Common;

namespace AFC27.KMS.Documents.Presentation.Controllers;

/// <summary>
/// Document library management controller.
/// </summary>
[ApiController]
[Route("api/documents/libraries")]
[Authorize]
public class LibrariesController : ControllerBase
{
    private readonly ILogger<LibrariesController> _logger;

    public LibrariesController(ILogger<LibrariesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all accessible libraries.
    /// </summary>
    [HttpGet]
    public ActionResult<ApiResponse<IReadOnlyList<LibrarySummaryDto>>> GetLibraries()
    {
        var libraries = new List<LibrarySummaryDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Operations Documents",
                NameArabic = "وثائق العمليات",
                IconName = "pi pi-folder",
                Color = "#2E7D32",
                Type = "Department",
                IsPublic = false,
                DocumentCount = 156,
                TotalSize = 524288000 // 500 MB
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Media Assets",
                NameArabic = "الملفات الإعلامية",
                IconName = "pi pi-images",
                Color = "#D4AF37",
                Type = "General",
                IsPublic = true,
                DocumentCount = 2500,
                TotalSize = 10737418240 // 10 GB
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Brand Guidelines",
                NameArabic = "إرشادات العلامة التجارية",
                IconName = "pi pi-palette",
                Color = "#1976D2",
                Type = "General",
                IsPublic = true,
                DocumentCount = 45,
                TotalSize = 157286400 // 150 MB
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Project Archives",
                NameArabic = "أرشيف المشاريع",
                IconName = "pi pi-inbox",
                Color = "#5D4037",
                Type = "Archive",
                IsPublic = false,
                DocumentCount = 890,
                TotalSize = 5368709120 // 5 GB
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "My Documents",
                NameArabic = "مستنداتي",
                IconName = "pi pi-user",
                Color = "#7B1FA2",
                Type = "Personal",
                IsPublic = false,
                DocumentCount = 23,
                TotalSize = 52428800 // 50 MB
            }
        };

        return Ok(ApiResponse<IReadOnlyList<LibrarySummaryDto>>.Ok(libraries));
    }

    /// <summary>
    /// Get library by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public ActionResult<ApiResponse<LibraryDto>> GetLibrary(Guid id)
    {
        var library = new LibraryDto
        {
            Id = id,
            Name = "Operations Documents",
            NameArabic = "وثائق العمليات",
            Description = "Central repository for all operational documentation",
            DescriptionArabic = "المستودع المركزي لجميع الوثائق التشغيلية",
            IconName = "pi pi-folder",
            Color = "#2E7D32",
            Type = "Department",
            IsPublic = false,
            RequiresApproval = true,
            EnableVersioning = true,
            MaxVersions = 10,
            MaxFileSize = 104857600, // 100 MB
            AllowedExtensions = ".pdf,.docx,.xlsx,.pptx,.jpg,.png",
            OwnerId = Guid.NewGuid(),
            OwnerName = "Mohammed Al-Rashid",
            DocumentCount = 156,
            FolderCount = 24,
            TotalSize = 524288000,
            CreatedAt = DateTime.UtcNow.AddYears(-1)
        };

        return Ok(ApiResponse<LibraryDto>.Ok(library));
    }

    /// <summary>
    /// Get library contents (folders and documents).
    /// </summary>
    [HttpGet("{id:guid}/contents")]
    public ActionResult<ApiResponse<LibraryContentsDto>> GetLibraryContents(
        Guid id,
        [FromQuery] Guid? folderId)
    {
        var contents = new LibraryContentsDto
        {
            Library = new LibrarySummaryDto
            {
                Id = id,
                Name = "Operations Documents",
                NameArabic = "وثائق العمليات",
                Type = "Department",
                DocumentCount = 156,
                TotalSize = 524288000
            },
            Folders = new List<FolderSummaryDto>
            {
                new() { Id = Guid.NewGuid(), Name = "Manuals", NameArabic = "الأدلة", IconName = "pi pi-book", DocumentCount = 25, ChildFolderCount = 3 },
                new() { Id = Guid.NewGuid(), Name = "Policies", NameArabic = "السياسات", IconName = "pi pi-file", DocumentCount = 18, ChildFolderCount = 0 },
                new() { Id = Guid.NewGuid(), Name = "Templates", NameArabic = "القوالب", IconName = "pi pi-copy", DocumentCount = 12, ChildFolderCount = 2 },
                new() { Id = Guid.NewGuid(), Name = "Reports", NameArabic = "التقارير", IconName = "pi pi-chart-bar", DocumentCount = 45, ChildFolderCount = 5 }
            },
            Documents = new List<DocumentSummaryDto>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "README",
                    FileName = "README.txt",
                    FileExtension = ".txt",
                    FileSize = 2048,
                    Version = "1.0",
                    Status = "Published",
                    CreatedByName = "System",
                    CreatedAt = DateTime.UtcNow.AddYears(-1)
                }
            },
            Breadcrumbs = new List<BreadcrumbDto>
            {
                new() { Id = id, Name = "Operations Documents", NameArabic = "وثائق العمليات", Type = "library" }
            }
        };

        return Ok(ApiResponse<LibraryContentsDto>.Ok(contents));
    }

    /// <summary>
    /// Create a new library.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "CanManageLibraries")]
    public async Task<ActionResult<ApiResponse<LibraryDto>>> CreateLibrary([FromBody] CreateLibraryRequest request)
    {
        _logger.LogInformation("Creating library {LibraryName}", request.Name);

        await Task.Delay(100);

        var library = new LibraryDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            NameArabic = request.NameArabic,
            Description = request.Description,
            Type = request.Type,
            IsPublic = request.IsPublic,
            RequiresApproval = request.RequiresApproval,
            EnableVersioning = request.EnableVersioning,
            MaxVersions = request.MaxVersions,
            MaxFileSize = request.MaxFileSize,
            AllowedExtensions = request.AllowedExtensions,
            OwnerId = Guid.NewGuid(),
            OwnerName = "Current User",
            DocumentCount = 0,
            FolderCount = 0,
            TotalSize = 0,
            CreatedAt = DateTime.UtcNow
        };

        return CreatedAtAction(nameof(GetLibrary), new { id = library.Id }, ApiResponse<LibraryDto>.Ok(library));
    }

    /// <summary>
    /// Update a library.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CanManageLibraries")]
    public async Task<ActionResult<ApiResponse>> UpdateLibrary(Guid id, [FromBody] UpdateLibraryRequest request)
    {
        _logger.LogInformation("Updating library {LibraryId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Library updated successfully"));
    }

    /// <summary>
    /// Delete a library.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "CanManageLibraries")]
    public async Task<ActionResult<ApiResponse>> DeleteLibrary(Guid id)
    {
        _logger.LogInformation("Deleting library {LibraryId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Library deleted successfully"));
    }

    /// <summary>
    /// Create a folder in a library.
    /// </summary>
    [HttpPost("{id:guid}/folders")]
    [Authorize(Policy = "CanUploadDocuments")]
    public async Task<ActionResult<ApiResponse<FolderDto>>> CreateFolder(Guid id, [FromBody] CreateFolderRequest request)
    {
        _logger.LogInformation("Creating folder {FolderName} in library {LibraryId}", request.Name, id);

        await Task.Delay(100);

        var folder = new FolderDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            NameArabic = request.NameArabic,
            Description = request.Description,
            LibraryId = id,
            ParentId = request.ParentId,
            Path = request.Name,
            DocumentCount = 0,
            ChildFolderCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        return CreatedAtAction(nameof(GetFolder), new { id, folderId = folder.Id }, ApiResponse<FolderDto>.Ok(folder));
    }

    /// <summary>
    /// Get a folder.
    /// </summary>
    [HttpGet("{id:guid}/folders/{folderId:guid}")]
    public ActionResult<ApiResponse<FolderDto>> GetFolder(Guid id, Guid folderId)
    {
        var folder = new FolderDto
        {
            Id = folderId,
            Name = "Manuals",
            NameArabic = "الأدلة",
            Description = "Operational manuals and guides",
            DescriptionArabic = "الأدلة والإرشادات التشغيلية",
            LibraryId = id,
            Path = "Manuals",
            IconName = "pi pi-book",
            DocumentCount = 25,
            ChildFolderCount = 3,
            Children = new List<FolderSummaryDto>
            {
                new() { Id = Guid.NewGuid(), Name = "Technical", NameArabic = "التقنية", DocumentCount = 10 },
                new() { Id = Guid.NewGuid(), Name = "Operations", NameArabic = "العمليات", DocumentCount = 8 },
                new() { Id = Guid.NewGuid(), Name = "HR", NameArabic = "الموارد البشرية", DocumentCount = 7 }
            },
            CreatedAt = DateTime.UtcNow.AddMonths(-6)
        };

        return Ok(ApiResponse<FolderDto>.Ok(folder));
    }

    /// <summary>
    /// Update a folder.
    /// </summary>
    [HttpPut("{id:guid}/folders/{folderId:guid}")]
    [Authorize(Policy = "CanEditDocuments")]
    public async Task<ActionResult<ApiResponse>> UpdateFolder(Guid id, Guid folderId, [FromBody] UpdateFolderRequest request)
    {
        _logger.LogInformation("Updating folder {FolderId} in library {LibraryId}", folderId, id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Folder updated successfully"));
    }

    /// <summary>
    /// Delete a folder.
    /// </summary>
    [HttpDelete("{id:guid}/folders/{folderId:guid}")]
    [Authorize(Policy = "CanDeleteDocuments")]
    public async Task<ActionResult<ApiResponse>> DeleteFolder(Guid id, Guid folderId)
    {
        _logger.LogInformation("Deleting folder {FolderId} from library {LibraryId}", folderId, id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Folder deleted successfully"));
    }

    /// <summary>
    /// Get library statistics.
    /// </summary>
    [HttpGet("{id:guid}/stats")]
    public ActionResult<ApiResponse<LibraryStatsDto>> GetLibraryStats(Guid id)
    {
        var stats = new LibraryStatsDto
        {
            TotalDocuments = 156,
            TotalFolders = 24,
            TotalSize = 524288000,
            DocumentsByType = new Dictionary<string, int>
            {
                { "PDF", 78 },
                { "Word", 35 },
                { "Excel", 28 },
                { "PowerPoint", 15 }
            },
            RecentActivity = new List<DocumentAuditDto>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Action = "Uploaded",
                    Details = "new-report.pdf",
                    PerformedByName = "Ahmed Hassan",
                    PerformedAt = DateTime.UtcNow.AddHours(-1)
                }
            }
        };

        return Ok(ApiResponse<LibraryStatsDto>.Ok(stats));
    }
}

/// <summary>
/// Library statistics DTO.
/// </summary>
public record LibraryStatsDto
{
    public int TotalDocuments { get; init; }
    public int TotalFolders { get; init; }
    public long TotalSize { get; init; }
    public Dictionary<string, int> DocumentsByType { get; init; } = new();
    public IReadOnlyList<DocumentAuditDto> RecentActivity { get; init; } = Array.Empty<DocumentAuditDto>();
}
