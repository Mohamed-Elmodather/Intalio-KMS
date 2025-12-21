using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Media.Application.DTOs;
using AFC27.KMS.Contracts.Common;

namespace AFC27.KMS.Media.Presentation.Controllers;

/// <summary>
/// Media galleries management controller.
/// </summary>
[ApiController]
[Route("api/media/galleries")]
[Authorize]
public class GalleriesController : ControllerBase
{
    private readonly ILogger<GalleriesController> _logger;

    public GalleriesController(ILogger<GalleriesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of galleries.
    /// </summary>
    [HttpGet]
    public ActionResult<ApiResponse<PagedResult<MediaGallerySummaryDto>>> GetGalleries(
        [FromQuery] string? search,
        [FromQuery] string? type,
        [FromQuery] string? visibility,
        [FromQuery] Guid? communityId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var galleries = new List<MediaGallerySummaryDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Opening Ceremony Photos",
                NameArabic = "صور حفل الافتتاح",
                Type = "Photos",
                Visibility = "Organization",
                CoverImageUrl = "/media/galleries/opening-ceremony/cover.jpg",
                ItemCount = 156,
                TotalSizeBytes = 524288000,
                TotalSizeFormatted = "500 MB",
                OwnerName = "Mohammed Al-Rashid",
                PreviewThumbnails = new[] { "/media/thumb1.jpg", "/media/thumb2.jpg", "/media/thumb3.jpg", "/media/thumb4.jpg" },
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Stadium Construction Progress",
                NameArabic = "تقدم بناء الملعب",
                Type = "Videos",
                Visibility = "Team",
                CoverImageUrl = "/media/galleries/construction/cover.jpg",
                ItemCount = 42,
                TotalSizeBytes = 2147483648,
                TotalSizeFormatted = "2 GB",
                OwnerName = "Sara Ali",
                PreviewThumbnails = new[] { "/media/thumb5.jpg", "/media/thumb6.jpg", "/media/thumb7.jpg" },
                CreatedAt = DateTime.UtcNow.AddDays(-60)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Team Photos 2024",
                NameArabic = "صور الفريق 2024",
                Type = "TeamMedia",
                Visibility = "Organization",
                CoverImageUrl = "/media/galleries/team/cover.jpg",
                ItemCount = 89,
                TotalSizeBytes = 314572800,
                TotalSizeFormatted = "300 MB",
                OwnerName = "Ahmed Hassan",
                PreviewThumbnails = new[] { "/media/thumb8.jpg", "/media/thumb9.jpg" },
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Press Conference Media",
                NameArabic = "وسائط المؤتمر الصحفي",
                Type = "EventMedia",
                Visibility = "Public",
                CoverImageUrl = "/media/galleries/press/cover.jpg",
                ItemCount = 234,
                TotalSizeBytes = 1073741824,
                TotalSizeFormatted = "1 GB",
                OwnerName = "Fatima Khan",
                PreviewThumbnails = new[] { "/media/thumb10.jpg", "/media/thumb11.jpg", "/media/thumb12.jpg", "/media/thumb13.jpg" },
                CreatedAt = DateTime.UtcNow.AddDays(-7)
            }
        };

        var result = PagedResult<MediaGallerySummaryDto>.Create(galleries, 24, page, pageSize);
        return Ok(ApiResponse<PagedResult<MediaGallerySummaryDto>>.Ok(result));
    }

    /// <summary>
    /// Get gallery by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public ActionResult<ApiResponse<MediaGalleryDto>> GetGallery(Guid id)
    {
        var gallery = new MediaGalleryDto
        {
            Id = id,
            Name = "Opening Ceremony Photos",
            NameArabic = "صور حفل الافتتاح",
            Description = "Official photos from the AFC Asian Cup 2027 opening ceremony rehearsal and preparation events.",
            DescriptionArabic = "الصور الرسمية من بروفة حفل افتتاح كأس آسيا 2027 وفعاليات التحضير",
            Type = "Photos",
            Visibility = "Organization",
            CoverImageUrl = "/media/galleries/opening-ceremony/cover.jpg",
            ItemCount = 156,
            TotalSizeBytes = 524288000,
            TotalSizeFormatted = "500 MB",
            OwnerId = Guid.NewGuid(),
            OwnerName = "Mohammed Al-Rashid",
            Tags = new[] { "Opening Ceremony", "Official", "2027", "AFC" },
            RecentItems = new List<MediaItemSummaryDto>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    FileName = "ceremony_stage_01.jpg",
                    Title = "Main Stage Setup",
                    Type = "Image",
                    MimeType = "image/jpeg",
                    FileSizeBytes = 4194304,
                    FileSizeFormatted = "4 MB",
                    ThumbnailUrl = "/media/thumbnails/stage_01_thumb.jpg",
                    Status = "Ready",
                    Width = 4000,
                    Height = 3000,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    FileName = "rehearsal_crowd.jpg",
                    Title = "Rehearsal Crowd View",
                    Type = "Image",
                    MimeType = "image/jpeg",
                    FileSizeBytes = 3145728,
                    FileSizeFormatted = "3 MB",
                    ThumbnailUrl = "/media/thumbnails/crowd_thumb.jpg",
                    Status = "Ready",
                    Width = 3840,
                    Height = 2160,
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                }
            },
            CreatedAt = DateTime.UtcNow.AddDays(-30)
        };

        return Ok(ApiResponse<MediaGalleryDto>.Ok(gallery));
    }

    /// <summary>
    /// Create a new gallery.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<MediaGalleryDto>>> CreateGallery([FromBody] CreateGalleryRequest request)
    {
        _logger.LogInformation("Creating gallery: {Name}", request.Name);

        await Task.Delay(100);

        var gallery = new MediaGalleryDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            NameArabic = request.NameArabic,
            Description = request.Description,
            DescriptionArabic = request.DescriptionArabic,
            Type = request.Type,
            Visibility = request.Visibility,
            ItemCount = 0,
            TotalSizeBytes = 0,
            TotalSizeFormatted = "0 B",
            OwnerId = Guid.NewGuid(),
            OwnerName = "Current User",
            Tags = request.Tags?.ToArray() ?? Array.Empty<string>(),
            CreatedAt = DateTime.UtcNow
        };

        return CreatedAtAction(nameof(GetGallery), new { id = gallery.Id }, ApiResponse<MediaGalleryDto>.Ok(gallery));
    }

    /// <summary>
    /// Update a gallery.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> UpdateGallery(Guid id, [FromBody] UpdateGalleryRequest request)
    {
        _logger.LogInformation("Updating gallery {GalleryId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Gallery updated successfully"));
    }

    /// <summary>
    /// Delete a gallery.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> DeleteGallery(Guid id)
    {
        _logger.LogInformation("Deleting gallery {GalleryId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Gallery deleted successfully"));
    }

    /// <summary>
    /// Set gallery cover image.
    /// </summary>
    [HttpPost("{id:guid}/cover")]
    public async Task<ActionResult<ApiResponse>> SetCoverImage(Guid id, [FromBody] Guid mediaItemId)
    {
        _logger.LogInformation("Setting cover image for gallery {GalleryId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Cover image set successfully"));
    }

    /// <summary>
    /// Archive a gallery.
    /// </summary>
    [HttpPost("{id:guid}/archive")]
    public async Task<ActionResult<ApiResponse>> ArchiveGallery(Guid id)
    {
        _logger.LogInformation("Archiving gallery {GalleryId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Gallery archived"));
    }

    /// <summary>
    /// Restore an archived gallery.
    /// </summary>
    [HttpPost("{id:guid}/restore")]
    public async Task<ActionResult<ApiResponse>> RestoreGallery(Guid id)
    {
        _logger.LogInformation("Restoring gallery {GalleryId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Gallery restored"));
    }

    /// <summary>
    /// Get gallery media items.
    /// </summary>
    [HttpGet("{id:guid}/items")]
    public ActionResult<ApiResponse<PagedResult<MediaItemSummaryDto>>> GetGalleryItems(
        Guid id,
        [FromQuery] string? type,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var items = new List<MediaItemSummaryDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                FileName = "ceremony_stage_01.jpg",
                Title = "Main Stage Setup",
                Type = "Image",
                MimeType = "image/jpeg",
                FileSizeBytes = 4194304,
                FileSizeFormatted = "4 MB",
                ThumbnailUrl = "/media/thumbnails/stage_01_thumb.jpg",
                Status = "Ready",
                Width = 4000,
                Height = 3000,
                IsFeatured = true,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new()
            {
                Id = Guid.NewGuid(),
                FileName = "opening_video.mp4",
                Title = "Opening Sequence",
                Type = "Video",
                MimeType = "video/mp4",
                FileSizeBytes = 104857600,
                FileSizeFormatted = "100 MB",
                ThumbnailUrl = "/media/thumbnails/video_thumb.jpg",
                Status = "Ready",
                Width = 1920,
                Height = 1080,
                DurationSeconds = 180,
                DurationFormatted = "3:00",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            }
        };

        var result = PagedResult<MediaItemSummaryDto>.Create(items, 156, page, pageSize);
        return Ok(ApiResponse<PagedResult<MediaItemSummaryDto>>.Ok(result));
    }

    /// <summary>
    /// Get gallery types.
    /// </summary>
    [HttpGet("types")]
    [AllowAnonymous]
    public ActionResult<ApiResponse<IReadOnlyList<object>>> GetGalleryTypes()
    {
        var types = new List<object>
        {
            new { Value = "General", Label = "General", LabelArabic = "عام" },
            new { Value = "Photos", Label = "Photos", LabelArabic = "صور" },
            new { Value = "Videos", Label = "Videos", LabelArabic = "فيديو" },
            new { Value = "Documents", Label = "Documents", LabelArabic = "مستندات" },
            new { Value = "EventMedia", Label = "Event Media", LabelArabic = "وسائط الفعاليات" },
            new { Value = "ProjectMedia", Label = "Project Media", LabelArabic = "وسائط المشاريع" },
            new { Value = "TeamMedia", Label = "Team Media", LabelArabic = "وسائط الفريق" }
        };

        return Ok(ApiResponse<IReadOnlyList<object>>.Ok(types));
    }
}
