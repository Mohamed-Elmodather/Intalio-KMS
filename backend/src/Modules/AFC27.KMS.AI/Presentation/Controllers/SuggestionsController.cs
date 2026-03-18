using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.AI.Application.Services;
using AFC27.KMS.Contracts.Common;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.AI.Presentation.Controllers;

/// <summary>
/// Proactive recommendation surfacing controller (Phase 8C).
/// Provides personalized content suggestions and related content based on user activity.
/// </summary>
[ApiController]
[Route("api/ai/suggestions")]
[Authorize]
public class SuggestionsController : ControllerBase
{
    private readonly IRecommendationService _recommendationService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<SuggestionsController> _logger;

    public SuggestionsController(
        IRecommendationService recommendationService,
        ICurrentUser currentUser,
        ILogger<SuggestionsController> logger)
    {
        _recommendationService = recommendationService;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <summary>
    /// Get personalized suggestions for the current user.
    /// Returns content recommendations based on user activity, department, role, and interests.
    /// Used for the "Recommended for you" dashboard section.
    /// </summary>
    [HttpGet("for-me")]
    public async Task<ActionResult<ApiResponse<PersonalizedSuggestionsDto>>> GetSuggestionsForMe(
        [FromQuery] SuggestionFilterRequest? filter = null,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;

        _logger.LogInformation("Fetching personalized suggestions for user {UserId}", userId);

        var suggestions = await _recommendationService.GetSuggestionsForUserAsync(
            userId, filter, cancellationToken);

        return Ok(ApiResponse<PersonalizedSuggestionsDto>.Ok(suggestions));
    }

    /// <summary>
    /// Get related content for a specific entity.
    /// Used for the "Related content" sidebar on article detail pages.
    /// </summary>
    [HttpGet("related")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<RecommendationDto>>>> GetRelatedContent(
        [FromQuery] string entityType,
        [FromQuery] Guid entityId,
        [FromQuery] int maxResults = 5,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(entityType))
        {
            return BadRequest(ApiResponse<IReadOnlyList<RecommendationDto>>.Fail("entityType is required."));
        }

        _logger.LogInformation("Fetching related content for {EntityType} {EntityId}", entityType, entityId);

        var related = await _recommendationService.GetRelatedContentAsync(
            entityType, entityId, maxResults, cancellationToken);

        return Ok(ApiResponse<IReadOnlyList<RecommendationDto>>.Ok(related));
    }

    /// <summary>
    /// Record feedback on a suggestion (accepted or dismissed).
    /// This feedback is used to improve future recommendations.
    /// </summary>
    [HttpPost("{suggestionId:guid}/feedback")]
    public async Task<ActionResult<ApiResponse>> RecordFeedback(
        Guid suggestionId,
        [FromBody] SuggestionFeedbackRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Recording feedback for suggestion {SuggestionId}: {Action}",
            suggestionId, request.WasAccepted ? "accepted" : "dismissed");

        await _recommendationService.RecordSuggestionFeedbackAsync(
            suggestionId, request.WasAccepted, cancellationToken);

        return Ok(ApiResponse.Ok("Feedback recorded"));
    }

    /// <summary>
    /// Force a refresh of suggestions for the current user.
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse>> RefreshSuggestions(
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;

        _logger.LogInformation("Refreshing suggestions for user {UserId}", userId);

        await _recommendationService.RefreshSuggestionsAsync(userId, cancellationToken);

        return Ok(ApiResponse.Ok("Suggestion refresh triggered"));
    }
}

/// <summary>
/// Suggestion feedback request.
/// </summary>
public record SuggestionFeedbackRequest
{
    public bool WasAccepted { get; init; }
}
