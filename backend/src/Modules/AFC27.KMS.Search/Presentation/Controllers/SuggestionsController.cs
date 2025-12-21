using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Search.Application.DTOs;
using AFC27.KMS.Search.Domain.Entities;

namespace AFC27.KMS.Search.Presentation.Controllers;

/// <summary>
/// Controller for search suggestions and autocomplete
/// </summary>
[ApiController]
[Route("api/search/suggest")]
[Authorize]
public class SuggestionsController : ControllerBase
{
    /// <summary>
    /// Get autocomplete suggestions for a query
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(SuggestResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<SuggestResponse>> GetSuggestions([FromQuery] SuggestRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Query) || request.Query.Length < 2)
        {
            return Ok(new SuggestResponse { Query = request.Query });
        }

        // TODO: Implement Elasticsearch suggestions
        var response = new SuggestResponse
        {
            Query = request.Query,
            Suggestions = new List<SuggestionItem>()
        };

        return Ok(response);
    }

    /// <summary>
    /// Get suggestions based on popular searches
    /// </summary>
    [HttpGet("popular")]
    [ProducesResponseType(typeof(IEnumerable<SuggestionItem>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SuggestionItem>>> GetPopularSuggestions(
        [FromQuery] string language = "en",
        [FromQuery] int limit = 10)
    {
        // TODO: Return popular search terms as suggestions
        var suggestions = new List<SuggestionItem>();
        return Ok(suggestions);
    }

    /// <summary>
    /// Get entity suggestions (people, categories, tags)
    /// </summary>
    [HttpGet("entities")]
    [ProducesResponseType(typeof(IEnumerable<SuggestionItem>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SuggestionItem>>> GetEntitySuggestions(
        [FromQuery] string query,
        [FromQuery] string language = "en",
        [FromQuery] int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {
            return Ok(new List<SuggestionItem>());
        }

        // TODO: Return entity-based suggestions (people, tags, categories)
        var suggestions = new List<SuggestionItem>();
        return Ok(suggestions);
    }

    /// <summary>
    /// Get tag suggestions
    /// </summary>
    [HttpGet("tags")]
    [ProducesResponseType(typeof(IEnumerable<SuggestionItem>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SuggestionItem>>> GetTagSuggestions(
        [FromQuery] string query,
        [FromQuery] int limit = 10)
    {
        // TODO: Return tag suggestions
        var suggestions = new List<SuggestionItem>();
        return Ok(suggestions);
    }

    /// <summary>
    /// Get category suggestions
    /// </summary>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(IEnumerable<SuggestionItem>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SuggestionItem>>> GetCategorySuggestions(
        [FromQuery] string query,
        [FromQuery] int limit = 10)
    {
        // TODO: Return category suggestions
        var suggestions = new List<SuggestionItem>();
        return Ok(suggestions);
    }

    /// <summary>
    /// Get person suggestions (for @mentions)
    /// </summary>
    [HttpGet("people")]
    [ProducesResponseType(typeof(IEnumerable<SuggestionItem>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SuggestionItem>>> GetPeopleSuggestions(
        [FromQuery] string query,
        [FromQuery] int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {
            return Ok(new List<SuggestionItem>());
        }

        // TODO: Return people suggestions
        var suggestions = new List<SuggestionItem>();
        return Ok(suggestions);
    }

    /// <summary>
    /// Get "Did you mean" spelling corrections
    /// </summary>
    [HttpGet("spelling")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<string>>> GetSpellingCorrections(
        [FromQuery] string query,
        [FromQuery] string language = "en",
        [FromQuery] int limit = 5)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Ok(new List<string>());
        }

        // TODO: Return spelling corrections using Elasticsearch
        var corrections = new List<string>();
        return Ok(corrections);
    }

    /// <summary>
    /// Add a curated suggestion (admin)
    /// </summary>
    [HttpPost("curated")]
    [Authorize(Policy = "CanManageSearch")]
    [ProducesResponseType(typeof(SuggestionItem), StatusCodes.Status201Created)]
    public async Task<ActionResult<SuggestionItem>> AddCuratedSuggestion([FromBody] CreateSuggestionRequest request)
    {
        // TODO: Add curated suggestion
        var suggestion = new SuggestionItem
        {
            Text = request.Text,
            Type = SuggestionType.Curated,
            Weight = request.Weight
        };

        return CreatedAtAction(nameof(GetSuggestions), suggestion);
    }

    /// <summary>
    /// Remove a curated suggestion (admin)
    /// </summary>
    [HttpDelete("curated/{id:guid}")]
    [Authorize(Policy = "CanManageSearch")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveCuratedSuggestion(Guid id)
    {
        // TODO: Remove curated suggestion
        return NoContent();
    }

    /// <summary>
    /// Get all curated suggestions (admin)
    /// </summary>
    [HttpGet("curated")]
    [Authorize(Policy = "CanManageSearch")]
    [ProducesResponseType(typeof(IEnumerable<SuggestionItem>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SuggestionItem>>> GetCuratedSuggestions(
        [FromQuery] string language = "en")
    {
        // TODO: Return all curated suggestions
        var suggestions = new List<SuggestionItem>();
        return Ok(suggestions);
    }
}

/// <summary>
/// Request to create a curated suggestion
/// </summary>
public record CreateSuggestionRequest
{
    public string Text { get; init; } = string.Empty;
    public string Language { get; init; } = "en";
    public long Weight { get; init; } = 100;
    public SearchableContentType? RelatedContentType { get; init; }
    public Guid? RelatedEntityId { get; init; }
}
