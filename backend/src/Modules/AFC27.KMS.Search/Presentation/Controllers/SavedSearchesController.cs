using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Search.Application.DTOs;

namespace AFC27.KMS.Search.Presentation.Controllers;

/// <summary>
/// Controller for managing saved searches
/// </summary>
[ApiController]
[Route("api/search/saved")]
[Authorize]
public class SavedSearchesController : ControllerBase
{
    /// <summary>
    /// Get all saved searches for current user
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SavedSearchDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SavedSearchDto>>> GetSavedSearches()
    {
        // TODO: Return user's saved searches
        var searches = new List<SavedSearchDto>();
        return Ok(searches);
    }

    /// <summary>
    /// Get a specific saved search
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SavedSearchDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SavedSearchDto>> GetSavedSearch(Guid id)
    {
        // TODO: Return saved search
        return NotFound();
    }

    /// <summary>
    /// Create a new saved search
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SavedSearchDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SavedSearchDto>> CreateSavedSearch([FromBody] SaveSearchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Query))
        {
            return BadRequest("Name and query are required");
        }

        // TODO: Create saved search
        var savedSearch = new SavedSearchDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Query = request.Query,
            ContentTypes = request.ContentTypes,
            FiltersJson = request.FiltersJson,
            DateFrom = request.DateFrom,
            DateTo = request.DateTo,
            NotifyOnNewResults = request.NotifyOnNewResults,
            NotificationFrequency = request.NotificationFrequency,
            ExecutionCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        return CreatedAtAction(nameof(GetSavedSearch), new { id = savedSearch.Id }, savedSearch);
    }

    /// <summary>
    /// Update a saved search
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SavedSearchDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SavedSearchDto>> UpdateSavedSearch(
        Guid id,
        [FromBody] UpdateSavedSearchRequest request)
    {
        // TODO: Update saved search
        return NotFound();
    }

    /// <summary>
    /// Delete a saved search
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSavedSearch(Guid id)
    {
        // TODO: Delete saved search
        return NoContent();
    }

    /// <summary>
    /// Execute a saved search
    /// </summary>
    [HttpPost("{id:guid}/execute")]
    [ProducesResponseType(typeof(SearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SearchResponse>> ExecuteSavedSearch(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // TODO: Execute saved search and return results
        return NotFound();
    }

    /// <summary>
    /// Toggle notifications for a saved search
    /// </summary>
    [HttpPost("{id:guid}/notifications")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleNotifications(
        Guid id,
        [FromBody] ToggleNotificationsRequest request)
    {
        // TODO: Toggle notifications
        return NoContent();
    }

    /// <summary>
    /// Get new results count for saved searches
    /// </summary>
    [HttpGet("new-results")]
    [ProducesResponseType(typeof(IEnumerable<SavedSearchNewResultsDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SavedSearchNewResultsDto>>> GetNewResultsCounts()
    {
        // TODO: Return new results counts for all saved searches
        var results = new List<SavedSearchNewResultsDto>();
        return Ok(results);
    }
}

/// <summary>
/// Request to toggle notifications
/// </summary>
public record ToggleNotificationsRequest
{
    public bool Enabled { get; init; }
    public Domain.Entities.NotificationFrequency Frequency { get; init; }
}

/// <summary>
/// New results count for a saved search
/// </summary>
public record SavedSearchNewResultsDto
{
    public Guid SavedSearchId { get; init; }
    public string Name { get; init; } = string.Empty;
    public int NewResultsCount { get; init; }
    public DateTime? LastCheckedAt { get; init; }
}
