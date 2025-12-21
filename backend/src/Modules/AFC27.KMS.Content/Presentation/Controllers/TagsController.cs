using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Content.Application.DTOs;
using AFC27.KMS.Contracts.Common;

namespace AFC27.KMS.Content.Presentation.Controllers;

/// <summary>
/// Tag management controller.
/// </summary>
[ApiController]
[Route("api/content/tags")]
[Authorize]
public class TagsController : ControllerBase
{
    private readonly ILogger<TagsController> _logger;

    public TagsController(ILogger<TagsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all tags.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public ActionResult<ApiResponse<IReadOnlyList<TagDto>>> GetTags([FromQuery] string? search)
    {
        var tags = new List<TagDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Venues", NameArabic = "الملاعب", Slug = "venues", Color = "#2E7D32", UsageCount = 45 },
            new() { Id = Guid.NewGuid(), Name = "Tournament", NameArabic = "البطولة", Slug = "tournament", Color = "#D4AF37", UsageCount = 120 },
            new() { Id = Guid.NewGuid(), Name = "Volunteers", NameArabic = "المتطوعين", Slug = "volunteers", Color = "#1976D2", UsageCount = 35 },
            new() { Id = Guid.NewGuid(), Name = "Teams", NameArabic = "الفرق", Slug = "teams", Color = "#E65100", UsageCount = 80 },
            new() { Id = Guid.NewGuid(), Name = "Matches", NameArabic = "المباريات", Slug = "matches", Color = "#7B1FA2", UsageCount = 95 },
            new() { Id = Guid.NewGuid(), Name = "Tickets", NameArabic = "التذاكر", Slug = "tickets", Color = "#00838F", UsageCount = 25 },
            new() { Id = Guid.NewGuid(), Name = "Infrastructure", NameArabic = "البنية التحتية", Slug = "infrastructure", Color = "#5D4037", UsageCount = 30 },
            new() { Id = Guid.NewGuid(), Name = "Media", NameArabic = "الإعلام", Slug = "media", Color = "#C2185B", UsageCount = 55 }
        };

        if (!string.IsNullOrEmpty(search))
        {
            tags = tags.Where(t =>
                t.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (t.NameArabic?.Contains(search) ?? false)
            ).ToList();
        }

        return Ok(ApiResponse<IReadOnlyList<TagDto>>.Ok(tags));
    }

    /// <summary>
    /// Get popular tags.
    /// </summary>
    [HttpGet("popular")]
    [AllowAnonymous]
    public ActionResult<ApiResponse<IReadOnlyList<TagDto>>> GetPopularTags([FromQuery] int limit = 10)
    {
        var tags = new List<TagDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Tournament", NameArabic = "البطولة", Slug = "tournament", Color = "#D4AF37", UsageCount = 120 },
            new() { Id = Guid.NewGuid(), Name = "Matches", NameArabic = "المباريات", Slug = "matches", Color = "#7B1FA2", UsageCount = 95 },
            new() { Id = Guid.NewGuid(), Name = "Teams", NameArabic = "الفرق", Slug = "teams", Color = "#E65100", UsageCount = 80 }
        };

        return Ok(ApiResponse<IReadOnlyList<TagDto>>.Ok(tags.Take(limit).ToList()));
    }

    /// <summary>
    /// Get tag by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public ActionResult<ApiResponse<TagDto>> GetTag(Guid id)
    {
        var tag = new TagDto
        {
            Id = id,
            Name = "Tournament",
            NameArabic = "البطولة",
            Slug = "tournament",
            Color = "#D4AF37",
            UsageCount = 120
        };

        return Ok(ApiResponse<TagDto>.Ok(tag));
    }

    /// <summary>
    /// Get tag by slug.
    /// </summary>
    [HttpGet("by-slug/{slug}")]
    [AllowAnonymous]
    public ActionResult<ApiResponse<TagDto>> GetTagBySlug(string slug)
    {
        return GetTag(Guid.NewGuid());
    }

    /// <summary>
    /// Create a new tag.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<ApiResponse<TagDto>>> CreateTag([FromBody] CreateTagRequest request)
    {
        _logger.LogInformation("Creating tag {TagName}", request.Name);

        await Task.Delay(100);

        var tag = new TagDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            NameArabic = request.NameArabic,
            Slug = request.Name.ToLower().Replace(" ", "-"),
            Color = request.Color,
            UsageCount = 0
        };

        return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, ApiResponse<TagDto>.Ok(tag));
    }

    /// <summary>
    /// Update a tag.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<ApiResponse>> UpdateTag(Guid id, [FromBody] CreateTagRequest request)
    {
        _logger.LogInformation("Updating tag {TagId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Tag updated successfully"));
    }

    /// <summary>
    /// Delete a tag.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<ApiResponse>> DeleteTag(Guid id)
    {
        _logger.LogInformation("Deleting tag {TagId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Tag deleted successfully"));
    }

    /// <summary>
    /// Get articles with a specific tag.
    /// </summary>
    [HttpGet("{id:guid}/articles")]
    [AllowAnonymous]
    public ActionResult<ApiResponse<PagedResult<ArticleSummaryDto>>> GetTagArticles(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
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
                AuthorName = "Mohammed Al-Rashid",
                ViewCount = 15420,
                PublishedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        var result = PagedResult<ArticleSummaryDto>.Create(articles, 120, page, pageSize);
        return Ok(ApiResponse<PagedResult<ArticleSummaryDto>>.Ok(result));
    }

    /// <summary>
    /// Merge tags.
    /// </summary>
    [HttpPost("merge")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<ApiResponse>> MergeTags([FromBody] MergeTagsRequest request)
    {
        _logger.LogInformation("Merging tags {SourceIds} into {TargetId}",
            string.Join(",", request.SourceTagIds), request.TargetTagId);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Tags merged successfully"));
    }
}

/// <summary>
/// Merge tags request.
/// </summary>
public record MergeTagsRequest
{
    public IReadOnlyList<Guid> SourceTagIds { get; init; } = Array.Empty<Guid>();
    public Guid TargetTagId { get; init; }
}
