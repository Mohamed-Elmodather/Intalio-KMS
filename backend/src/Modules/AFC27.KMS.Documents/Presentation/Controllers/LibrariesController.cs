using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Documents.Application.DTOs;
using AFC27.KMS.Documents.Application.Interfaces;
using AFC27.KMS.Contracts.Common;
using System.Security.Claims;

namespace AFC27.KMS.Documents.Presentation.Controllers;

/// <summary>
/// Document library management controller.
/// </summary>
[ApiController]
[Route("api/documents/libraries")]
[Authorize]
public class LibrariesController : ControllerBase
{
    private readonly ILibraryService _libraryService;
    private readonly IFolderService _folderService;
    private readonly ILogger<LibrariesController> _logger;

    public LibrariesController(
        ILibraryService libraryService,
        IFolderService folderService,
        ILogger<LibrariesController> logger)
    {
        _libraryService = libraryService;
        _folderService = folderService;
        _logger = logger;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;

        return Guid.Empty;
    }

    private string GetCurrentUserName()
    {
        return User.FindFirst(ClaimTypes.Name)?.Value
            ?? User.FindFirst("name")?.Value
            ?? "Unknown User";
    }

    /// <summary>
    /// Get all accessible libraries.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<LibrarySummaryDto>>), 200)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<LibrarySummaryDto>>>> GetLibraries(
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var libraries = await _libraryService.GetLibrariesAsync(userId, cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<LibrarySummaryDto>>.Ok(libraries));
    }

    /// <summary>
    /// Get library by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<LibraryDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<LibraryDto>>> GetLibrary(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var library = await _libraryService.GetLibraryAsync(id, userId, cancellationToken);

        if (library == null)
        {
            return NotFound(ApiResponse.Fail("Library not found or access denied"));
        }

        return Ok(ApiResponse<LibraryDto>.Ok(library));
    }

    /// <summary>
    /// Get library contents (folders and documents).
    /// </summary>
    [HttpGet("{id:guid}/contents")]
    [ProducesResponseType(typeof(ApiResponse<LibraryContentsDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<LibraryContentsDto>>> GetLibraryContents(
        Guid id,
        [FromQuery] Guid? folderId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        // If folderId is provided, get folder contents instead
        if (folderId.HasValue)
        {
            var folderContents = await _folderService.GetFolderContentsAsync(folderId.Value, userId, cancellationToken);

            var libraryInfo = await _libraryService.GetLibraryAsync(id, userId, cancellationToken);
            if (libraryInfo == null)
            {
                return NotFound(ApiResponse.Fail("Library not found or access denied"));
            }

            var contents = new LibraryContentsDto
            {
                Library = new LibrarySummaryDto
                {
                    Id = libraryInfo.Id,
                    Name = libraryInfo.Name,
                    NameArabic = libraryInfo.NameArabic,
                    IconName = libraryInfo.IconName,
                    Color = libraryInfo.Color,
                    Type = libraryInfo.Type,
                    IsPublic = libraryInfo.IsPublic,
                    DocumentCount = libraryInfo.DocumentCount,
                    TotalSize = libraryInfo.TotalSize
                },
                CurrentFolder = folderContents.Folder,
                Folders = folderContents.Subfolders,
                Documents = folderContents.Documents,
                Breadcrumbs = folderContents.Breadcrumbs
            };

            return Ok(ApiResponse<LibraryContentsDto>.Ok(contents));
        }

        // Get library root contents
        var library = await _libraryService.GetLibraryAsync(id, userId, cancellationToken);
        if (library == null)
        {
            return NotFound(ApiResponse.Fail("Library not found or access denied"));
        }

        var folderTree = await _libraryService.GetFolderTreeAsync(id, userId, cancellationToken);

        // Get documents at library root (documents without a folder)
        // For now, return an empty list as we'd need a specific method for this
        var rootDocuments = new List<DocumentSummaryDto>();

        var libraryContents = new LibraryContentsDto
        {
            Library = new LibrarySummaryDto
            {
                Id = library.Id,
                Name = library.Name,
                NameArabic = library.NameArabic,
                IconName = library.IconName,
                Color = library.Color,
                Type = library.Type,
                IsPublic = library.IsPublic,
                DocumentCount = library.DocumentCount,
                TotalSize = library.TotalSize
            },
            Folders = folderTree.Select(f => new FolderSummaryDto
            {
                Id = f.Id,
                Name = f.Name,
                NameArabic = f.NameArabic,
                IconName = f.IconName,
                DocumentCount = f.DocumentCount,
                ChildFolderCount = f.Children.Count
            }).ToList(),
            Documents = rootDocuments,
            Breadcrumbs = new List<BreadcrumbDto>
            {
                new()
                {
                    Id = library.Id,
                    Name = library.Name,
                    NameArabic = library.NameArabic,
                    Type = "library"
                }
            }
        };

        return Ok(ApiResponse<LibraryContentsDto>.Ok(libraryContents));
    }

    /// <summary>
    /// Create a new library.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "CanManageLibraries")]
    [ProducesResponseType(typeof(ApiResponse<LibraryDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    public async Task<ActionResult<ApiResponse<LibraryDto>>> CreateLibrary(
        [FromBody] CreateLibraryRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var userName = GetCurrentUserName();

        _logger.LogInformation("User {UserId} creating library {LibraryName}", userId, request.Name);

        try
        {
            var library = await _libraryService.CreateLibraryAsync(request, userId, userName, cancellationToken);

            return CreatedAtAction(nameof(GetLibrary), new { id = library.Id }, ApiResponse<LibraryDto>.Ok(library));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Update a library.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CanManageLibraries")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse>> UpdateLibrary(
        Guid id,
        [FromBody] UpdateLibraryRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} updating library {LibraryId}", userId, id);

        var success = await _libraryService.UpdateLibraryAsync(id, request, userId, cancellationToken);

        if (!success)
        {
            return NotFound(ApiResponse.Fail("Library not found or access denied"));
        }

        return Ok(ApiResponse.Ok("Library updated successfully"));
    }

    /// <summary>
    /// Delete a library.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "CanManageLibraries")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse>> DeleteLibrary(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} deleting library {LibraryId}", userId, id);

        var success = await _libraryService.DeleteLibraryAsync(id, userId, cancellationToken);

        if (!success)
        {
            return NotFound(ApiResponse.Fail("Library not found or access denied"));
        }

        return Ok(ApiResponse.Ok("Library deleted successfully"));
    }

    /// <summary>
    /// Create a folder in a library.
    /// </summary>
    [HttpPost("{id:guid}/folders")]
    [Authorize(Policy = "CanUploadDocuments")]
    [ProducesResponseType(typeof(ApiResponse<FolderDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    public async Task<ActionResult<ApiResponse<FolderDto>>> CreateFolder(
        Guid id,
        [FromBody] CreateFolderRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} creating folder {FolderName} in library {LibraryId}",
            userId, request.Name, id);

        // Ensure the library ID in the request matches the route
        var folderRequest = request with { LibraryId = id };

        try
        {
            var folder = await _folderService.CreateFolderAsync(folderRequest, userId, cancellationToken);

            return CreatedAtAction(nameof(GetFolder), new { id, folderId = folder.Id }, ApiResponse<FolderDto>.Ok(folder));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Get a folder.
    /// </summary>
    [HttpGet("{id:guid}/folders/{folderId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<FolderDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<FolderDto>>> GetFolder(
        Guid id,
        Guid folderId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var folder = await _folderService.GetFolderAsync(folderId, userId, cancellationToken);

        if (folder == null)
        {
            return NotFound(ApiResponse.Fail("Folder not found or access denied"));
        }

        // Verify folder belongs to the specified library
        if (folder.LibraryId != id)
        {
            return NotFound(ApiResponse.Fail("Folder not found in this library"));
        }

        return Ok(ApiResponse<FolderDto>.Ok(folder));
    }

    /// <summary>
    /// Get folder contents.
    /// </summary>
    [HttpGet("{id:guid}/folders/{folderId:guid}/contents")]
    [ProducesResponseType(typeof(ApiResponse<FolderContentsDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<FolderContentsDto>>> GetFolderContents(
        Guid id,
        Guid folderId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var contents = await _folderService.GetFolderContentsAsync(folderId, userId, cancellationToken);

        if (contents.Folder == null || contents.Folder.Id == Guid.Empty)
        {
            return NotFound(ApiResponse.Fail("Folder not found or access denied"));
        }

        return Ok(ApiResponse<FolderContentsDto>.Ok(contents));
    }

    /// <summary>
    /// Update a folder.
    /// </summary>
    [HttpPut("{id:guid}/folders/{folderId:guid}")]
    [Authorize(Policy = "CanEditDocuments")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse>> UpdateFolder(
        Guid id,
        Guid folderId,
        [FromBody] UpdateFolderRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} updating folder {FolderId} in library {LibraryId}",
            userId, folderId, id);

        var success = await _folderService.UpdateFolderAsync(folderId, request, userId, cancellationToken);

        if (!success)
        {
            return NotFound(ApiResponse.Fail("Folder not found or access denied"));
        }

        return Ok(ApiResponse.Ok("Folder updated successfully"));
    }

    /// <summary>
    /// Delete a folder.
    /// </summary>
    [HttpDelete("{id:guid}/folders/{folderId:guid}")]
    [Authorize(Policy = "CanDeleteDocuments")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    public async Task<ActionResult<ApiResponse>> DeleteFolder(
        Guid id,
        Guid folderId,
        [FromQuery] bool deleteContents = false,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} deleting folder {FolderId} from library {LibraryId}, deleteContents={DeleteContents}",
            userId, folderId, id, deleteContents);

        var success = await _folderService.DeleteFolderAsync(folderId, deleteContents, userId, cancellationToken);

        if (!success)
        {
            return NotFound(ApiResponse.Fail("Folder not found, not empty, or access denied"));
        }

        return Ok(ApiResponse.Ok("Folder deleted successfully"));
    }

    /// <summary>
    /// Move a folder.
    /// </summary>
    [HttpPost("{id:guid}/folders/{folderId:guid}/move")]
    [Authorize(Policy = "CanEditDocuments")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse>> MoveFolder(
        Guid id,
        Guid folderId,
        [FromBody] MoveFolderRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} moving folder {FolderId} to parent {TargetParentId}",
            userId, folderId, request.TargetParentId);

        var success = await _folderService.MoveFolderAsync(folderId, request.TargetParentId, userId, cancellationToken);

        if (!success)
        {
            return NotFound(ApiResponse.Fail("Folder not found or cannot be moved"));
        }

        return Ok(ApiResponse.Ok("Folder moved successfully"));
    }

    /// <summary>
    /// Get library folder tree.
    /// </summary>
    [HttpGet("{id:guid}/tree")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<FolderTreeDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<FolderTreeDto>>>> GetFolderTree(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var tree = await _libraryService.GetFolderTreeAsync(id, userId, cancellationToken);

        if (tree == null || !tree.Any())
        {
            // Check if library exists
            var library = await _libraryService.GetLibraryAsync(id, userId, cancellationToken);
            if (library == null)
            {
                return NotFound(ApiResponse.Fail("Library not found or access denied"));
            }
        }

        return Ok(ApiResponse<IReadOnlyList<FolderTreeDto>>.Ok(tree));
    }

    /// <summary>
    /// Get library statistics.
    /// </summary>
    [HttpGet("{id:guid}/stats")]
    [ProducesResponseType(typeof(ApiResponse<LibraryStatsDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse<LibraryStatsDto>>> GetLibraryStats(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var stats = await _libraryService.GetLibraryStatsAsync(id, userId, cancellationToken);

        if (stats.LibraryId == Guid.Empty && stats.TotalDocuments == 0)
        {
            return NotFound(ApiResponse.Fail("Library not found or access denied"));
        }

        return Ok(ApiResponse<LibraryStatsDto>.Ok(stats));
    }

    /// <summary>
    /// Get library permissions.
    /// </summary>
    [HttpGet("{id:guid}/permissions")]
    [Authorize(Policy = "CanManageLibraries")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<LibraryPermissionDto>>), 200)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<LibraryPermissionDto>>>> GetLibraryPermissions(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var permissions = await _libraryService.GetLibraryPermissionsAsync(id, userId, cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<LibraryPermissionDto>>.Ok(permissions));
    }

    /// <summary>
    /// Set library permission.
    /// </summary>
    [HttpPost("{id:guid}/permissions")]
    [Authorize(Policy = "CanManageLibraries")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    public async Task<ActionResult<ApiResponse>> SetLibraryPermission(
        Guid id,
        [FromBody] SetPermissionRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} setting permission on library {LibraryId}", userId, id);

        var success = await _libraryService.SetLibraryPermissionAsync(id, request, userId, cancellationToken);

        if (!success)
        {
            return BadRequest(ApiResponse.Fail("Failed to set permission"));
        }

        return Ok(ApiResponse.Ok("Permission set successfully"));
    }

    /// <summary>
    /// Remove library permission.
    /// </summary>
    [HttpDelete("{id:guid}/permissions/{permissionId:guid}")]
    [Authorize(Policy = "CanManageLibraries")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    public async Task<ActionResult<ApiResponse>> RemoveLibraryPermission(
        Guid id,
        Guid permissionId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} removing permission {PermissionId} from library {LibraryId}",
            userId, permissionId, id);

        var success = await _libraryService.RemoveLibraryPermissionAsync(id, permissionId, userId, cancellationToken);

        if (!success)
        {
            return NotFound(ApiResponse.Fail("Permission not found"));
        }

        return Ok(ApiResponse.Ok("Permission removed successfully"));
    }
}

/// <summary>
/// Move folder request.
/// </summary>
public record MoveFolderRequest
{
    public Guid? TargetParentId { get; init; }
}
