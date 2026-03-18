using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Contracts.Common;

namespace AFC27.KMS.Content.Presentation.Controllers;

/// <summary>
/// Smart link preview controller (Phase 8B).
/// Returns rich preview data for internal entity links (articles, documents, discussions, etc.)
/// so the frontend can render hover popovers with title, snippet, thumbnail, and author info.
/// </summary>
[ApiController]
[Route("api/content/link-preview")]
[Authorize]
public class LinkPreviewController : ControllerBase
{
    private readonly ILogger<LinkPreviewController> _logger;

    public LinkPreviewController(ILogger<LinkPreviewController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get a link preview for an internal entity.
    /// Used by the frontend to render rich hover cards on internal links within content.
    /// </summary>
    /// <param name="entityType">The type of entity (Article, Document, Discussion, Event, LessonLearned, Community, User).</param>
    /// <param name="entityId">The ID of the entity.</param>
    [HttpGet]
    public ActionResult<ApiResponse<LinkPreviewDto>> GetLinkPreview(
        [FromQuery] string entityType,
        [FromQuery] Guid entityId)
    {
        if (string.IsNullOrWhiteSpace(entityType))
        {
            return BadRequest(ApiResponse<LinkPreviewDto>.Fail("entityType is required."));
        }

        _logger.LogInformation("Generating link preview for {EntityType} {EntityId}", entityType, entityId);

        // In a real implementation, this would look up the entity from the appropriate repository
        // based on entityType and return its metadata. For now, return mock data based on type.

        var preview = entityType.ToLowerInvariant() switch
        {
            "article" => new LinkPreviewDto
            {
                EntityType = "Article",
                EntityId = entityId,
                Title = "AFC Asian Cup 2027 Venues Announced",
                TitleArabic = "الإعلان عن ملاعب كأس آسيا 2027",
                Snippet = "Saudi Arabia unveils world-class stadiums for the tournament. The Local Organising Committee has announced the venues that will host matches...",
                SnippetArabic = "المملكة العربية السعودية تكشف عن ملاعب عالمية المستوى للبطولة...",
                ThumbnailUrl = "/images/news/venues-announcement.jpg",
                AuthorId = Guid.NewGuid(),
                AuthorName = "Mohammed Al-Rashid",
                AuthorAvatarUrl = "/avatars/mohammed.jpg",
                EntityStatus = "Published",
                EntitySubType = "News",
                Url = $"/articles/{entityId}",
                LastModifiedAt = DateTime.UtcNow.AddDays(-2),
                ViewCount = 15420,
                CommentCount = 45,
                Tags = new List<string> { "Venues", "Tournament" }
            },
            "document" => new LinkPreviewDto
            {
                EntityType = "Document",
                EntityId = entityId,
                Title = "Stadium Operations Handbook v3.2",
                Snippet = "Comprehensive guide covering all operational procedures for stadium management during the tournament...",
                ThumbnailUrl = "/images/documents/handbook-thumb.jpg",
                AuthorId = Guid.NewGuid(),
                AuthorName = "Sara Ali",
                AuthorAvatarUrl = "/avatars/sara.jpg",
                EntityStatus = "Active",
                EntitySubType = "PDF",
                Url = $"/documents/{entityId}",
                LastModifiedAt = DateTime.UtcNow.AddDays(-5),
                FileSize = "4.2 MB",
                MimeType = "application/pdf"
            },
            "discussion" => new LinkPreviewDto
            {
                EntityType = "Discussion",
                EntityId = entityId,
                Title = "Best practices for crowd management?",
                Snippet = "Looking for insights and experiences from teams who have managed large-scale event crowds...",
                AuthorId = Guid.NewGuid(),
                AuthorName = "Ahmed Hassan",
                EntityStatus = "Open",
                Url = $"/discussions/{entityId}",
                LastModifiedAt = DateTime.UtcNow.AddDays(-1),
                CommentCount = 12
            },
            "user" => new LinkPreviewDto
            {
                EntityType = "User",
                EntityId = entityId,
                Title = "Mohammed Al-Rashid",
                TitleArabic = "محمد الراشد",
                Snippet = "Operations Director | Technology Department",
                ThumbnailUrl = "/avatars/mohammed.jpg",
                AuthorId = entityId,
                AuthorName = "Mohammed Al-Rashid",
                AuthorAvatarUrl = "/avatars/mohammed.jpg",
                EntityStatus = "Active",
                Url = $"/users/{entityId}",
                Department = "Technology",
                JobTitle = "Operations Director"
            },
            "event" => new LinkPreviewDto
            {
                EntityType = "Event",
                EntityId = entityId,
                Title = "Venue Inspection - Riyadh Stadium",
                Snippet = "Scheduled inspection of the main tournament venue with safety and operations teams.",
                AuthorId = Guid.NewGuid(),
                AuthorName = "Sara Ali",
                EntityStatus = "Upcoming",
                Url = $"/events/{entityId}",
                LastModifiedAt = DateTime.UtcNow.AddDays(5),
                EventDate = DateTime.UtcNow.AddDays(14)
            },
            "lessonlearned" => new LinkPreviewDto
            {
                EntityType = "LessonLearned",
                EntityId = entityId,
                Title = "Early vendor onboarding reduces delays",
                Snippet = "Starting vendor onboarding 6 months early significantly reduced setup time for the test event...",
                AuthorId = Guid.NewGuid(),
                AuthorName = "Ahmed Hassan",
                EntityStatus = "Approved",
                EntitySubType = "High Impact",
                Url = $"/lessons/{entityId}",
                LastModifiedAt = DateTime.UtcNow.AddDays(-10)
            },
            "community" => new LinkPreviewDto
            {
                EntityType = "Community",
                EntityId = entityId,
                Title = "Stadium Operations",
                TitleArabic = "عمليات الملاعب",
                Snippet = "Community for stadium operations teams to share knowledge, best practices, and updates.",
                EntityStatus = "Active",
                Url = $"/communities/{entityId}",
                MemberCount = 245
            },
            _ => new LinkPreviewDto
            {
                EntityType = entityType,
                EntityId = entityId,
                Title = $"{entityType} {entityId}",
                Snippet = "Preview not available for this entity type.",
                Url = $"/{entityType.ToLowerInvariant()}/{entityId}"
            }
        };

        return Ok(ApiResponse<LinkPreviewDto>.Ok(preview));
    }

    /// <summary>
    /// Batch link preview for multiple entities at once.
    /// Used when rendering a page that contains many internal links.
    /// </summary>
    [HttpPost("batch")]
    public ActionResult<ApiResponse<IReadOnlyList<LinkPreviewDto>>> GetBatchLinkPreviews(
        [FromBody] BatchLinkPreviewRequest request)
    {
        if (request.Items == null || request.Items.Count == 0)
        {
            return BadRequest(ApiResponse<IReadOnlyList<LinkPreviewDto>>.Fail("At least one item is required."));
        }

        if (request.Items.Count > 50)
        {
            return BadRequest(ApiResponse<IReadOnlyList<LinkPreviewDto>>.Fail("Maximum 50 items per batch request."));
        }

        _logger.LogInformation("Generating batch link previews for {Count} items", request.Items.Count);

        var previews = request.Items.Select(item => new LinkPreviewDto
        {
            EntityType = item.EntityType,
            EntityId = item.EntityId,
            Title = $"Preview for {item.EntityType} {item.EntityId}",
            Snippet = "Preview content placeholder...",
            Url = $"/{item.EntityType.ToLowerInvariant()}/{item.EntityId}",
            LastModifiedAt = DateTime.UtcNow.AddDays(-1)
        }).ToList();

        return Ok(ApiResponse<IReadOnlyList<LinkPreviewDto>>.Ok(previews));
    }
}

// ========================================
// Phase 8B: Link Preview DTOs
// ========================================

/// <summary>
/// Rich link preview data for internal entity references.
/// </summary>
public record LinkPreviewDto
{
    public string EntityType { get; init; } = string.Empty;
    public Guid EntityId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? TitleArabic { get; init; }
    public string? Snippet { get; init; }
    public string? SnippetArabic { get; init; }
    public string? ThumbnailUrl { get; init; }
    public Guid? AuthorId { get; init; }
    public string? AuthorName { get; init; }
    public string? AuthorAvatarUrl { get; init; }
    public string? EntityStatus { get; init; }
    public string? EntitySubType { get; init; }
    public string Url { get; init; } = string.Empty;
    public DateTime? LastModifiedAt { get; init; }

    // Type-specific fields
    public int? ViewCount { get; init; }
    public int? CommentCount { get; init; }
    public int? MemberCount { get; init; }
    public IReadOnlyList<string>? Tags { get; init; }
    public string? FileSize { get; init; }
    public string? MimeType { get; init; }
    public string? Department { get; init; }
    public string? JobTitle { get; init; }
    public DateTime? EventDate { get; init; }
}

/// <summary>
/// Batch link preview request.
/// </summary>
public record BatchLinkPreviewRequest
{
    public IReadOnlyList<LinkPreviewItem> Items { get; init; } = Array.Empty<LinkPreviewItem>();
}

/// <summary>
/// Single item in a batch link preview request.
/// </summary>
public record LinkPreviewItem
{
    public string EntityType { get; init; } = string.Empty;
    public Guid EntityId { get; init; }
}
