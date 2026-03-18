using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Content.Application.DTOs;
using AFC27.KMS.Content.Application.Interfaces;
using AFC27.KMS.Content.Application.Services;
using AFC27.KMS.Contracts.Common;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.Content.Presentation.Controllers;

/// <summary>
/// Article management controller.
/// </summary>
[ApiController]
[Route("api/content/articles")]
[Authorize]
public class ArticlesController : ControllerBase
{
    private readonly ILogger<ArticlesController> _logger;
    private readonly IVerificationService _verificationService;
    private readonly IArticleExportService _articleExportService;
    private readonly IImportService _importService;
    private readonly IBulkExportService _bulkExportService;
    private readonly ICurrentUser _currentUser;

    public ArticlesController(
        ILogger<ArticlesController> logger,
        IVerificationService verificationService,
        IArticleExportService articleExportService,
        IImportService importService,
        IBulkExportService bulkExportService,
        ICurrentUser currentUser)
    {
        _logger = logger;
        _verificationService = verificationService;
        _articleExportService = articleExportService;
        _importService = importService;
        _bulkExportService = bulkExportService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Get paginated list of articles.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public ActionResult<ApiResponse<PagedResult<ArticleSummaryDto>>> GetArticles([FromQuery] ArticleFilterRequest filter)
    {
        var articles = new List<ArticleSummaryDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "AFC Asian Cup 2027 Venues Announced",
                TitleArabic = "الإعلان عن ملاعب كأس آسيا 2027",
                Summary = "Saudi Arabia unveils world-class stadiums for the tournament",
                SummaryArabic = "المملكة العربية السعودية تكشف عن ملاعب عالمية المستوى للبطولة",
                Slug = "afc-asian-cup-2027-venues-announced",
                Type = "News",
                Status = "Published",
                ThumbnailUrl = "/images/news/venues-announcement.jpg",
                AuthorName = "Mohammed Al-Rashid",
                CategoryName = "Tournament News",
                IsFeatured = true,
                ViewCount = 15420,
                CommentCount = 45,
                PublishedAt = DateTime.UtcNow.AddDays(-2),
                Tags = new List<TagSummaryDto>
                {
                    new() { Id = Guid.NewGuid(), Name = "Venues", NameArabic = "الملاعب", Slug = "venues", Color = "#2E7D32" },
                    new() { Id = Guid.NewGuid(), Name = "Tournament", NameArabic = "البطولة", Slug = "tournament", Color = "#D4AF37" }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Volunteer Registration Opens",
                TitleArabic = "فتح باب التسجيل للمتطوعين",
                Summary = "Join the team and be part of history",
                SummaryArabic = "انضم إلى الفريق وكن جزءاً من التاريخ",
                Slug = "volunteer-registration-opens",
                Type = "Announcement",
                Status = "Published",
                ThumbnailUrl = "/images/news/volunteers.jpg",
                AuthorName = "Sara Ali",
                CategoryName = "Announcements",
                IsFeatured = true,
                ViewCount = 8500,
                CommentCount = 120,
                PublishedAt = DateTime.UtcNow.AddDays(-5),
                Tags = new List<TagSummaryDto>
                {
                    new() { Id = Guid.NewGuid(), Name = "Volunteers", NameArabic = "المتطوعين", Slug = "volunteers", Color = "#1976D2" }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Behind the Scenes: Stadium Construction",
                TitleArabic = "خلف الكواليس: بناء الملاعب",
                Summary = "An exclusive look at the making of world-class venues",
                Slug = "behind-the-scenes-stadium-construction",
                Type = "Blog",
                Status = "Published",
                AuthorName = "Ahmed Hassan",
                CategoryName = "Features",
                IsFeatured = false,
                ViewCount = 3200,
                CommentCount = 28,
                PublishedAt = DateTime.UtcNow.AddDays(-7)
            }
        };

        var result = PagedResult<ArticleSummaryDto>.Create(articles, 150, filter.Page, filter.PageSize);
        return Ok(ApiResponse<PagedResult<ArticleSummaryDto>>.Ok(result));
    }

    /// <summary>
    /// Get featured articles.
    /// </summary>
    [HttpGet("featured")]
    [AllowAnonymous]
    public ActionResult<ApiResponse<IReadOnlyList<ArticleSummaryDto>>> GetFeaturedArticles([FromQuery] int limit = 5)
    {
        var articles = new List<ArticleSummaryDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "AFC Asian Cup 2027 Venues Announced",
                TitleArabic = "الإعلان عن ملاعب كأس آسيا 2027",
                Slug = "afc-asian-cup-2027-venues-announced",
                Type = "News",
                Status = "Published",
                ThumbnailUrl = "/images/news/venues-announcement.jpg",
                AuthorName = "Mohammed Al-Rashid",
                IsFeatured = true,
                ViewCount = 15420,
                PublishedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        return Ok(ApiResponse<IReadOnlyList<ArticleSummaryDto>>.Ok(articles));
    }

    /// <summary>
    /// Get article by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public ActionResult<ApiResponse<ArticleDto>> GetArticle(Guid id)
    {
        var article = new ArticleDto
        {
            Id = id,
            Title = "AFC Asian Cup 2027 Venues Announced",
            TitleArabic = "الإعلان عن ملاعب كأس آسيا 2027",
            Summary = "Saudi Arabia unveils world-class stadiums for the tournament",
            SummaryArabic = "المملكة العربية السعودية تكشف عن ملاعب عالمية المستوى للبطولة",
            Content = "<h2>World-Class Venues</h2><p>The Local Organising Committee has announced the venues...</p>",
            ContentArabic = "<h2>ملاعب عالمية المستوى</h2><p>أعلنت اللجنة المنظمة المحلية عن الملاعب...</p>",
            Slug = "afc-asian-cup-2027-venues-announced",
            Type = "News",
            Status = "Published",
            FeaturedImageUrl = "/images/news/venues-announcement-full.jpg",
            ThumbnailUrl = "/images/news/venues-announcement.jpg",
            AuthorId = Guid.NewGuid(),
            AuthorName = "Mohammed Al-Rashid",
            AuthorAvatarUrl = "/avatars/mohammed.jpg",
            CategoryId = Guid.NewGuid(),
            CategoryName = "Tournament News",
            IsFeatured = true,
            AllowComments = true,
            ViewCount = 15420,
            CommentCount = 45,
            PublishedAt = DateTime.UtcNow.AddDays(-2),
            Version = 3,
            Tags = new List<TagSummaryDto>
            {
                new() { Id = Guid.NewGuid(), Name = "Venues", NameArabic = "الملاعب", Slug = "venues", Color = "#2E7D32" },
                new() { Id = Guid.NewGuid(), Name = "Tournament", NameArabic = "البطولة", Slug = "tournament", Color = "#D4AF37" }
            },
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-2)
        };

        return Ok(ApiResponse<ArticleDto>.Ok(article));
    }

    /// <summary>
    /// Get article by slug.
    /// </summary>
    [HttpGet("by-slug/{slug}")]
    [AllowAnonymous]
    public ActionResult<ApiResponse<ArticleDto>> GetArticleBySlug(string slug)
    {
        return GetArticle(Guid.NewGuid());
    }

    /// <summary>
    /// Create a new article.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "CanCreateContent")]
    public async Task<ActionResult<ApiResponse<ArticleDto>>> CreateArticle([FromBody] CreateArticleRequest request)
    {
        _logger.LogInformation("Creating article {Title}", request.Title);

        await Task.Delay(100);

        var article = new ArticleDto
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            TitleArabic = request.TitleArabic,
            Summary = request.Summary,
            Content = request.Content,
            ContentArabic = request.ContentArabic,
            Slug = request.Title.ToLower().Replace(" ", "-"),
            Type = request.Type,
            Status = "Draft",
            FeaturedImageUrl = request.FeaturedImageUrl,
            AuthorId = Guid.NewGuid(),
            AuthorName = "Current User",
            CategoryId = request.CategoryId,
            AllowComments = request.AllowComments,
            Version = 1,
            CreatedAt = DateTime.UtcNow
        };

        return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, ApiResponse<ArticleDto>.Ok(article));
    }

    /// <summary>
    /// Update an article.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CanEditContent")]
    public async Task<ActionResult<ApiResponse>> UpdateArticle(Guid id, [FromBody] UpdateArticleRequest request)
    {
        _logger.LogInformation("Updating article {ArticleId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Article updated successfully"));
    }

    /// <summary>
    /// Delete an article.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "CanDeleteContent")]
    public async Task<ActionResult<ApiResponse>> DeleteArticle(Guid id)
    {
        _logger.LogInformation("Deleting article {ArticleId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Article deleted successfully"));
    }

    /// <summary>
    /// Submit article for review.
    /// </summary>
    [HttpPost("{id:guid}/submit")]
    public async Task<ActionResult<ApiResponse>> SubmitForReview(Guid id)
    {
        _logger.LogInformation("Submitting article {ArticleId} for review", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Article submitted for review"));
    }

    /// <summary>
    /// Approve an article.
    /// </summary>
    [HttpPost("{id:guid}/approve")]
    [Authorize(Policy = "CanPublishContent")]
    public async Task<ActionResult<ApiResponse>> ApproveArticle(Guid id)
    {
        _logger.LogInformation("Approving article {ArticleId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Article approved"));
    }

    /// <summary>
    /// Reject an article.
    /// </summary>
    [HttpPost("{id:guid}/reject")]
    [Authorize(Policy = "CanPublishContent")]
    public async Task<ActionResult<ApiResponse>> RejectArticle(Guid id, [FromBody] string reason)
    {
        _logger.LogInformation("Rejecting article {ArticleId}: {Reason}", id, reason);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Article rejected"));
    }

    /// <summary>
    /// Publish an article.
    /// </summary>
    [HttpPost("{id:guid}/publish")]
    [Authorize(Policy = "CanPublishContent")]
    public async Task<ActionResult<ApiResponse>> PublishArticle(Guid id, [FromBody] PublishArticleRequest? request)
    {
        if (request?.ScheduledAt != null)
        {
            _logger.LogInformation("Scheduling article {ArticleId} for {ScheduledAt}", id, request.ScheduledAt);
        }
        else
        {
            _logger.LogInformation("Publishing article {ArticleId}", id);
        }

        await Task.Delay(100);

        return Ok(ApiResponse.Ok(request?.ScheduledAt != null ? "Article scheduled for publishing" : "Article published"));
    }

    /// <summary>
    /// Unpublish an article.
    /// </summary>
    [HttpPost("{id:guid}/unpublish")]
    [Authorize(Policy = "CanPublishContent")]
    public async Task<ActionResult<ApiResponse>> UnpublishArticle(Guid id)
    {
        _logger.LogInformation("Unpublishing article {ArticleId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Article unpublished"));
    }

    /// <summary>
    /// Set article as featured.
    /// </summary>
    [HttpPost("{id:guid}/feature")]
    [Authorize(Policy = "CanPublishContent")]
    public async Task<ActionResult<ApiResponse>> SetFeatured(Guid id, [FromBody] bool isFeatured)
    {
        _logger.LogInformation("Setting article {ArticleId} featured: {IsFeatured}", id, isFeatured);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok(isFeatured ? "Article featured" : "Article unfeatured"));
    }

    /// <summary>
    /// Get article versions.
    /// </summary>
    [HttpGet("{id:guid}/versions")]
    public ActionResult<ApiResponse<IReadOnlyList<ArticleVersionDto>>> GetArticleVersions(Guid id)
    {
        var versions = new List<ArticleVersionDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                VersionNumber = 3,
                Title = "AFC Asian Cup 2027 Venues Announced",
                Content = "Updated content...",
                ModifiedByName = "Mohammed Al-Rashid",
                ModifiedAt = DateTime.UtcNow.AddDays(-2),
                ChangeNotes = "Updated venue details"
            },
            new()
            {
                Id = Guid.NewGuid(),
                VersionNumber = 2,
                Title = "AFC Asian Cup 2027 Venues Announced",
                Content = "Previous content...",
                ModifiedByName = "Sara Ali",
                ModifiedAt = DateTime.UtcNow.AddDays(-5),
                ChangeNotes = "Added images"
            },
            new()
            {
                Id = Guid.NewGuid(),
                VersionNumber = 1,
                Title = "AFC Asian Cup 2027 Venues",
                Content = "Initial content...",
                ModifiedByName = "Mohammed Al-Rashid",
                ModifiedAt = DateTime.UtcNow.AddDays(-10),
                ChangeNotes = "Initial draft"
            }
        };

        return Ok(ApiResponse<IReadOnlyList<ArticleVersionDto>>.Ok(versions));
    }

    /// <summary>
    /// Restore a specific version by version ID.
    /// </summary>
    [HttpPost("{id:guid}/versions/{versionId:guid}/restore")]
    [Authorize(Policy = "CanEditContent")]
    public async Task<ActionResult<ApiResponse>> RestoreVersion(Guid id, Guid versionId)
    {
        _logger.LogInformation("Restoring article {ArticleId} to version {VersionId}", id, versionId);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Version restored"));
    }

    // ========================================
    // Phase 8D: Version Rollback by Number
    // ========================================

    /// <summary>
    /// Rollback an article to a specific version number.
    /// Creates a new version with the content from the specified historical version,
    /// preserving the full version history (non-destructive rollback).
    /// </summary>
    /// <param name="id">The article ID.</param>
    /// <param name="versionNumber">The version number to rollback to.</param>
    [HttpPost("{id:guid}/rollback/{versionNumber:int}")]
    [Authorize(Policy = "CanEditContent")]
    public async Task<ActionResult<ApiResponse<ArticleVersionRollbackResultDto>>> RollbackToVersion(
        Guid id, int versionNumber)
    {
        if (versionNumber < 1)
        {
            return BadRequest(ApiResponse<ArticleVersionRollbackResultDto>.Fail(
                "Version number must be 1 or greater."));
        }

        _logger.LogInformation(
            "Rolling back article {ArticleId} to version {VersionNumber}", id, versionNumber);

        await Task.Delay(100);

        // In a real implementation, this would:
        // 1. Fetch the article and validate it exists
        // 2. Fetch the version history and find the target version number
        // 3. If version number doesn't exist, return 404
        // 4. Create a NEW version (non-destructive) using the content from the target version
        // 5. Update the current article content to match the rolled-back version
        // 6. Publish a domain event for audit trail

        var currentVersion = 4; // Simulated: the new version number after rollback

        var result = new ArticleVersionRollbackResultDto
        {
            ArticleId = id,
            RolledBackFromVersion = 3, // Was at version 3
            RolledBackToVersion = versionNumber,
            NewVersionNumber = currentVersion,
            RolledBackByName = _currentUser.DisplayName ?? "Current User",
            RolledBackAt = DateTime.UtcNow,
            ChangeNotes = $"Rolled back to version {versionNumber}"
        };

        return Ok(ApiResponse<ArticleVersionRollbackResultDto>.Ok(result, "Article rolled back successfully"));
    }

    /// <summary>
    /// Get related articles.
    /// </summary>
    [HttpGet("{id:guid}/related")]
    [AllowAnonymous]
    public ActionResult<ApiResponse<IReadOnlyList<ArticleSummaryDto>>> GetRelatedArticles(Guid id, [FromQuery] int limit = 5)
    {
        var articles = new List<ArticleSummaryDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Stadium Infrastructure Update",
                TitleArabic = "تحديث البنية التحتية للملاعب",
                Slug = "stadium-infrastructure-update",
                Type = "News",
                Status = "Published",
                ViewCount = 3400,
                PublishedAt = DateTime.UtcNow.AddDays(-10)
            }
        };

        return Ok(ApiResponse<IReadOnlyList<ArticleSummaryDto>>.Ok(articles));
    }

    // ========================================
    // Phase 3B: Article Export to PDF/DOCX
    // ========================================

    /// <summary>
    /// Export an article to PDF or DOCX format.
    /// </summary>
    [HttpPost("{id:guid}/export")]
    public async Task<IActionResult> ExportArticle(
        Guid id,
        [FromQuery] string format = "pdf",
        [FromBody] ExportArticleRequest? request = null)
    {
        var exportFormat = format.ToLowerInvariant() switch
        {
            "pdf" => ExportFormat.Pdf,
            "docx" => ExportFormat.Docx,
            _ => ExportFormat.Pdf
        };

        var exportRequest = request ?? new ExportArticleRequest { Format = exportFormat };

        // Override format from query parameter if not set in body
        if (request == null || request.Format != exportFormat)
        {
            exportRequest = exportRequest with { Format = exportFormat };
        }

        _logger.LogInformation("Exporting article {ArticleId} to {Format}", id, exportFormat);

        try
        {
            var result = await _articleExportService.ExportArticleAsync(id, exportRequest);

            return File(
                result.FileContent,
                result.ContentType,
                result.FileName);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse.Fail("Article not found"));
        }
    }

    /// <summary>
    /// Get export metadata without downloading the file.
    /// </summary>
    [HttpGet("{id:guid}/export/info")]
    public ActionResult<ApiResponse<object>> GetExportInfo(Guid id, [FromQuery] string format = "pdf")
    {
        var exportFormat = format.ToLowerInvariant() switch
        {
            "pdf" => ExportFormat.Pdf,
            "docx" => ExportFormat.Docx,
            _ => ExportFormat.Pdf
        };

        var info = new
        {
            ArticleId = id,
            Format = format.ToLowerInvariant(),
            ContentType = _articleExportService.GetContentType(exportFormat),
            FileExtension = _articleExportService.GetFileExtension(exportFormat),
            SupportedFormats = new[] { "pdf", "docx" }
        };

        return Ok(ApiResponse<object>.Ok(info));
    }

    // ========================================
    // Knowledge Verification Lifecycle
    // ========================================

    /// <summary>
    /// Verify that an article's knowledge content is still accurate.
    /// </summary>
    [HttpPost("{id:guid}/verify")]
    [Authorize(Policy = "CanEditContent")]
    public async Task<ActionResult<ApiResponse<VerificationRecordDto>>> VerifyArticle(
        Guid id, [FromBody] VerifyArticleRequest request)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;
        var userName = _currentUser.DisplayName ?? "Unknown";

        _logger.LogInformation("Verifying article {ArticleId} by {UserName}", id, userName);

        try
        {
            var record = await _verificationService.VerifyArticleAsync(id, request, userId, userName);
            return Ok(ApiResponse<VerificationRecordDto>.Ok(record, "Article verified successfully"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<VerificationRecordDto>.Fail("Article not found"));
        }
    }

    /// <summary>
    /// Assign a knowledge owner to an article.
    /// </summary>
    [HttpPost("{id:guid}/assign-owner")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<ApiResponse>> AssignOwner(
        Guid id, [FromBody] AssignOwnerRequest request)
    {
        _logger.LogInformation("Assigning owner {OwnerName} to article {ArticleId}", request.OwnerName, id);

        var result = await _verificationService.AssignOwnerAsync(id, request);
        if (!result)
            return NotFound(ApiResponse.Fail("Article not found"));

        return Ok(ApiResponse.Ok("Owner assigned successfully"));
    }

    /// <summary>
    /// Get the knowledge verification dashboard with metrics and lists.
    /// </summary>
    [HttpGet("verification-dashboard")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<ApiResponse<VerificationDashboardDto>>> GetVerificationDashboard()
    {
        var dashboard = await _verificationService.GetDashboardAsync();
        return Ok(ApiResponse<VerificationDashboardDto>.Ok(dashboard));
    }

    /// <summary>
    /// Get all articles whose verification is overdue.
    /// </summary>
    [HttpGet("overdue")]
    [Authorize(Policy = "CanEditContent")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ArticleVerificationSummaryDto>>>> GetOverdueArticles()
    {
        var articles = await _verificationService.GetOverdueArticlesAsync();
        return Ok(ApiResponse<IReadOnlyList<ArticleVerificationSummaryDto>>.Ok(articles));
    }

    /// <summary>
    /// Get all articles whose verification is due soon (within 7 days).
    /// </summary>
    [HttpGet("due-soon")]
    [Authorize(Policy = "CanEditContent")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ArticleVerificationSummaryDto>>>> GetDueSoonArticles()
    {
        var articles = await _verificationService.GetDueSoonArticlesAsync();
        return Ok(ApiResponse<IReadOnlyList<ArticleVerificationSummaryDto>>.Ok(articles));
    }

    /// <summary>
    /// Get the verification history for a specific article.
    /// </summary>
    [HttpGet("{id:guid}/verification-history")]
    [Authorize(Policy = "CanEditContent")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<VerificationRecordDto>>>> GetVerificationHistory(Guid id)
    {
        var history = await _verificationService.GetVerificationHistoryAsync(id);
        return Ok(ApiResponse<IReadOnlyList<VerificationRecordDto>>.Ok(history));
    }

    // ========================================
    // Phase 9D: Import / Export
    // ========================================

    /// <summary>
    /// Import a DOCX or Markdown file as a new article or append to an existing article.
    /// The file is parsed into content blocks matching the block editor format.
    /// </summary>
    [HttpPost("import")]
    [Authorize(Policy = "CanCreateContent")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<ImportResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ImportResult>>> ImportArticle(
        IFormFile file,
        [FromQuery] bool createArticle = true,
        [FromQuery] Guid? targetArticleId = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] string language = "en",
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<ImportResult>.Fail("A file is required for import"));

        var allowedExtensions = new[] { ".md", ".markdown", ".docx" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return BadRequest(ApiResponse<ImportResult>.Fail(
                $"Unsupported file type '{extension}'. Supported: {string.Join(", ", allowedExtensions)}"));

        _logger.LogInformation(
            "Importing file {FileName} ({Size} bytes), createArticle: {Create}",
            file.FileName, file.Length, createArticle);

        using var stream = file.OpenReadStream();
        var options = new ImportOptions
        {
            CreateArticle = createArticle,
            TargetArticleId = targetArticleId,
            CategoryId = categoryId,
            Language = language
        };

        var result = await _importService.ImportAsync(stream, file.FileName, file.ContentType, options, cancellationToken);

        if (!result.Success)
            return BadRequest(ApiResponse<ImportResult>.Fail(result.ErrorMessage ?? "Import failed"));

        return Ok(ApiResponse<ImportResult>.Ok(result, $"Imported {result.BlockCount} blocks from {result.FileName}"));
    }

    /// <summary>
    /// Get supported import formats.
    /// </summary>
    [HttpGet("import/formats")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ImportFormatInfo>>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<IReadOnlyList<ImportFormatInfo>>> GetImportFormats()
    {
        var formats = _importService.GetSupportedFormats();
        return Ok(ApiResponse<IReadOnlyList<ImportFormatInfo>>.Ok(formats));
    }

    /// <summary>
    /// Bulk-export multiple articles as a ZIP archive.
    /// Each article is exported in the specified format (PDF, DOCX, or Markdown)
    /// and bundled into a single downloadable ZIP file.
    /// </summary>
    [HttpPost("bulk-export")]
    [Authorize(Policy = "CanEditContent")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkExport(
        [FromBody] BulkExportRequest request,
        CancellationToken cancellationToken)
    {
        if (request.ArticleIds.Count == 0)
            return BadRequest(ApiResponse.Fail("At least one article ID is required"));

        if (request.ArticleIds.Count > 100)
            return BadRequest(ApiResponse.Fail("Maximum 100 articles per bulk export"));

        _logger.LogInformation(
            "Bulk export: {Count} articles, format: {Format}",
            request.ArticleIds.Count, request.Format);

        var result = await _bulkExportService.ExportAsync(request, cancellationToken);

        if (!result.Success || result.FileContent == null)
            return BadRequest(ApiResponse.Fail(result.ErrorMessage ?? "Bulk export failed"));

        return File(result.FileContent, result.ContentType, result.FileName);
    }
}
