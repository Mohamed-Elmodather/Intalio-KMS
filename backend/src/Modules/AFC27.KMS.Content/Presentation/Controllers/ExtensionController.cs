using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AFC27.KMS.Content.Presentation.Controllers;

/// <summary>
/// Controller providing a lightweight API for the AFC27 KMS browser extension (Phase 9C).
/// Supports quick search, web-clip saving, and URL-based context suggestions.
/// </summary>
[ApiController]
[Route("api/content/extension")]
[Authorize]
public class ExtensionController : ControllerBase
{
    private readonly ILogger<ExtensionController> _logger;

    public ExtensionController(ILogger<ExtensionController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Lightweight search optimized for the browser extension popup.
    /// Returns a compact result set suitable for display in a small popup window.
    /// </summary>
    [HttpGet("quick-search")]
    [ProducesResponseType(typeof(ExtensionSearchResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ExtensionSearchResultDto>> QuickSearch(
        [FromQuery] string q,
        [FromQuery] int limit = 5,
        [FromQuery] string language = "en",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { error = "Query parameter 'q' is required" });

        _logger.LogInformation("Extension quick-search: {Query} (limit={Limit})", q, limit);

        // TODO: Integrate with the real Search module (AFC27.KMS.Search)
        // This should call SearchService.SearchAsync with a compact result projection.

        var result = new ExtensionSearchResultDto
        {
            Query = q,
            TotalResults = 3,
            Items = new List<ExtensionSearchItemDto>
            {
                new()
                {
                    ArticleId = Guid.NewGuid(),
                    Title = "Getting Started with AFC27 KMS",
                    Snippet = "Learn how to navigate and use the AFC27 Knowledge Management System...",
                    Url = "/articles/getting-started",
                    Category = "User Guide",
                    UpdatedAt = DateTime.UtcNow.AddDays(-5),
                    Relevance = 0.95
                },
                new()
                {
                    ArticleId = Guid.NewGuid(),
                    Title = "Knowledge Base Best Practices",
                    Snippet = "Tips for creating and maintaining high-quality knowledge articles...",
                    Url = "/articles/kb-best-practices",
                    Category = "Best Practices",
                    UpdatedAt = DateTime.UtcNow.AddDays(-12),
                    Relevance = 0.82
                },
                new()
                {
                    ArticleId = Guid.NewGuid(),
                    Title = "AFC27 Search Guide",
                    Snippet = "How to use advanced search features including filters, facets, and AI search...",
                    Url = "/articles/search-guide",
                    Category = "User Guide",
                    UpdatedAt = DateTime.UtcNow.AddDays(-20),
                    Relevance = 0.74
                }
            },
            ProcessingTimeMs = 42
        };

        return Ok(result);
    }

    /// <summary>
    /// Save a web clip (highlighted text, URL, and metadata) from the browser extension
    /// into the knowledge base as a new draft article or appended to an existing article.
    /// </summary>
    [HttpPost("save-clip")]
    [ProducesResponseType(typeof(SaveClipResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SaveClipResultDto>> SaveClip(
        [FromBody] SaveClipRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest(new { error = "Clip content is required" });

        _logger.LogInformation(
            "Extension save-clip from {Url}, content length: {Length}",
            request.SourceUrl, request.Content.Length);

        // TODO: Extract current user from claims
        var userId = Guid.Empty;

        // TODO: Create a draft article via the ArticleService
        // or append as a content block to an existing article if TargetArticleId is provided.

        var articleId = request.TargetArticleId ?? Guid.NewGuid();

        var result = new SaveClipResultDto
        {
            ArticleId = articleId,
            IsNewArticle = !request.TargetArticleId.HasValue,
            Title = request.Title ?? $"Web Clip - {DateTime.UtcNow:yyyy-MM-dd HH:mm}",
            Url = $"/articles/{articleId}",
            Message = request.TargetArticleId.HasValue
                ? "Clip appended to existing article"
                : "New draft article created from web clip",
            CreatedAt = DateTime.UtcNow
        };

        return Created(result.Url, result);
    }

    /// <summary>
    /// Get knowledge suggestions based on the current page URL the user is browsing.
    /// The extension calls this endpoint when a user navigates to a new page,
    /// allowing proactive knowledge surfacing.
    /// </summary>
    [HttpGet("context-suggestions")]
    [ProducesResponseType(typeof(ContextSuggestionsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ContextSuggestionsDto>> GetContextSuggestions(
        [FromQuery] string url,
        [FromQuery] string? pageTitle = null,
        [FromQuery] string language = "en",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
            return Ok(new ContextSuggestionsDto { Url = string.Empty });

        _logger.LogInformation("Extension context-suggestions for URL: {Url}", url);

        // TODO: Use URL patterns, page title keywords, and domain matching to find
        // relevant knowledge articles. Integrate with Search and AI modules.

        var suggestions = new ContextSuggestionsDto
        {
            Url = url,
            PageTitle = pageTitle,
            HasSuggestions = true,
            Articles = new List<ContextSuggestionArticleDto>
            {
                new()
                {
                    ArticleId = Guid.NewGuid(),
                    Title = "Related Internal Knowledge",
                    Snippet = "Your organization has internal documentation relevant to this page...",
                    Url = "/articles/related-knowledge",
                    Relevance = 0.78,
                    MatchReason = "URL domain match"
                }
            },
            RelatedTags = new List<string> { "documentation", "web-resources" }
        };

        return Ok(suggestions);
    }
}

#region Extension DTOs

/// <summary>
/// Extension quick-search result.
/// </summary>
public class ExtensionSearchResultDto
{
    public string Query { get; set; } = string.Empty;
    public int TotalResults { get; set; }
    public List<ExtensionSearchItemDto> Items { get; set; } = new();
    public int ProcessingTimeMs { get; set; }
}

public class ExtensionSearchItemDto
{
    public Guid ArticleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Snippet { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Category { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public double Relevance { get; set; }
}

/// <summary>
/// Request to save a web clip from the browser extension.
/// </summary>
public class SaveClipRequest
{
    /// <summary>
    /// The highlighted or selected text content from the web page.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Optional HTML content if the user clipped formatted text.
    /// </summary>
    public string? HtmlContent { get; set; }

    /// <summary>
    /// The URL of the source page.
    /// </summary>
    public string? SourceUrl { get; set; }

    /// <summary>
    /// The title of the source page.
    /// </summary>
    public string? SourceTitle { get; set; }

    /// <summary>
    /// Optional user-specified title for the clip.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Optional tags to apply to the new article.
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// If set, the clip is appended to this existing article instead of creating a new one.
    /// </summary>
    public Guid? TargetArticleId { get; set; }

    /// <summary>
    /// Optional note or comment about the clip.
    /// </summary>
    public string? Note { get; set; }
}

/// <summary>
/// Result of saving a web clip.
/// </summary>
public class SaveClipResultDto
{
    public Guid ArticleId { get; set; }
    public bool IsNewArticle { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Context-based knowledge suggestions for a URL.
/// </summary>
public class ContextSuggestionsDto
{
    public string Url { get; set; } = string.Empty;
    public string? PageTitle { get; set; }
    public bool HasSuggestions { get; set; }
    public List<ContextSuggestionArticleDto> Articles { get; set; } = new();
    public List<string> RelatedTags { get; set; } = new();
}

public class ContextSuggestionArticleDto
{
    public Guid ArticleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Snippet { get; set; }
    public string Url { get; set; } = string.Empty;
    public double Relevance { get; set; }
    public string? MatchReason { get; set; }
}

#endregion
