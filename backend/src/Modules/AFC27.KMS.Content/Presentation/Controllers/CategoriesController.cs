using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Content.Application.DTOs;
using AFC27.KMS.Contracts.Common;

namespace AFC27.KMS.Content.Presentation.Controllers;

/// <summary>
/// Category management controller.
/// </summary>
[ApiController]
[Route("api/content/categories")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ILogger<CategoriesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all categories as a tree structure.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public ActionResult<ApiResponse<IReadOnlyList<CategoryDto>>> GetCategories()
    {
        var categories = new List<CategoryDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Tournament News",
                NameArabic = "أخبار البطولة",
                Description = "Official tournament news and updates",
                DescriptionArabic = "أخبار وتحديثات البطولة الرسمية",
                Slug = "tournament-news",
                IconName = "pi pi-megaphone",
                Color = "#2E7D32",
                SortOrder = 1,
                IsActive = true,
                ArticleCount = 45,
                Children = new List<CategorySummaryDto>
                {
                    new() { Id = Guid.NewGuid(), Name = "Match Updates", NameArabic = "تحديثات المباريات", Slug = "match-updates", ArticleCount = 20 },
                    new() { Id = Guid.NewGuid(), Name = "Team News", NameArabic = "أخبار الفرق", Slug = "team-news", ArticleCount = 25 }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Announcements",
                NameArabic = "الإعلانات",
                Description = "Official announcements and press releases",
                DescriptionArabic = "الإعلانات الرسمية والبيانات الصحفية",
                Slug = "announcements",
                IconName = "pi pi-bell",
                Color = "#D4AF37",
                SortOrder = 2,
                IsActive = true,
                ArticleCount = 30
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Features",
                NameArabic = "المقالات المميزة",
                Description = "In-depth features and interviews",
                DescriptionArabic = "مقالات ومقابلات متعمقة",
                Slug = "features",
                IconName = "pi pi-star",
                Color = "#1976D2",
                SortOrder = 3,
                IsActive = true,
                ArticleCount = 25
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Behind the Scenes",
                NameArabic = "خلف الكواليس",
                Description = "Exclusive behind-the-scenes content",
                DescriptionArabic = "محتوى حصري من خلف الكواليس",
                Slug = "behind-the-scenes",
                IconName = "pi pi-video",
                Color = "#E65100",
                SortOrder = 4,
                IsActive = true,
                ArticleCount = 15
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Volunteers",
                NameArabic = "المتطوعين",
                Description = "Volunteer program updates",
                DescriptionArabic = "تحديثات برنامج التطوع",
                Slug = "volunteers",
                IconName = "pi pi-users",
                Color = "#7B1FA2",
                SortOrder = 5,
                IsActive = true,
                ArticleCount = 12
            }
        };

        return Ok(ApiResponse<IReadOnlyList<CategoryDto>>.Ok(categories));
    }

    /// <summary>
    /// Get category by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public ActionResult<ApiResponse<CategoryDto>> GetCategory(Guid id)
    {
        var category = new CategoryDto
        {
            Id = id,
            Name = "Tournament News",
            NameArabic = "أخبار البطولة",
            Description = "Official tournament news and updates",
            DescriptionArabic = "أخبار وتحديثات البطولة الرسمية",
            Slug = "tournament-news",
            IconName = "pi pi-megaphone",
            Color = "#2E7D32",
            SortOrder = 1,
            IsActive = true,
            ArticleCount = 45
        };

        return Ok(ApiResponse<CategoryDto>.Ok(category));
    }

    /// <summary>
    /// Get category by slug.
    /// </summary>
    [HttpGet("by-slug/{slug}")]
    [AllowAnonymous]
    public ActionResult<ApiResponse<CategoryDto>> GetCategoryBySlug(string slug)
    {
        return GetCategory(Guid.NewGuid());
    }

    /// <summary>
    /// Create a new category.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        _logger.LogInformation("Creating category {CategoryName}", request.Name);

        await Task.Delay(100);

        var category = new CategoryDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            NameArabic = request.NameArabic,
            Description = request.Description,
            DescriptionArabic = request.DescriptionArabic,
            Slug = request.Name.ToLower().Replace(" ", "-"),
            IconName = request.IconName,
            Color = request.Color,
            ParentId = request.ParentId,
            IsActive = true,
            ArticleCount = 0
        };

        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, ApiResponse<CategoryDto>.Ok(category));
    }

    /// <summary>
    /// Update a category.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<ApiResponse>> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequest request)
    {
        _logger.LogInformation("Updating category {CategoryId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Category updated successfully"));
    }

    /// <summary>
    /// Delete a category.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<ApiResponse>> DeleteCategory(Guid id)
    {
        _logger.LogInformation("Deleting category {CategoryId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Category deleted successfully"));
    }

    /// <summary>
    /// Reorder categories.
    /// </summary>
    [HttpPut("reorder")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<ApiResponse>> ReorderCategories([FromBody] Dictionary<Guid, int> orderMap)
    {
        _logger.LogInformation("Reordering {Count} categories", orderMap.Count);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Categories reordered"));
    }

    /// <summary>
    /// Get articles in a category.
    /// </summary>
    [HttpGet("{id:guid}/articles")]
    [AllowAnonymous]
    public ActionResult<ApiResponse<PagedResult<ArticleSummaryDto>>> GetCategoryArticles(
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

        var result = PagedResult<ArticleSummaryDto>.Create(articles, 45, page, pageSize);
        return Ok(ApiResponse<PagedResult<ArticleSummaryDto>>.Ok(result));
    }
}
