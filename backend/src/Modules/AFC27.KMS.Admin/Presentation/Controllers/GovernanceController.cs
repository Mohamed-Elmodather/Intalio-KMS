using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Application.Services;
using AFC27.KMS.Admin.Domain.Entities;

namespace AFC27.KMS.Admin.Presentation.Controllers;

/// <summary>
/// Controller for knowledge governance tracking and reviews.
/// </summary>
[ApiController]
[Route("api/admin/governance")]
[Authorize(Policy = "CanManageUsers")]
public class GovernanceController : ControllerBase
{
    private readonly IGovernanceService _governanceService;

    public GovernanceController(IGovernanceService governanceService)
    {
        _governanceService = governanceService;
    }

    /// <summary>
    /// Get the governance dashboard with summary metrics.
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(GovernanceDashboardDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<GovernanceDashboardDto>> GetDashboard(
        CancellationToken cancellationToken)
    {
        var dashboard = await _governanceService.GetDashboardAsync(cancellationToken);
        return Ok(dashboard);
    }

    /// <summary>
    /// Create a new governance review.
    /// </summary>
    [HttpPost("reviews")]
    [ProducesResponseType(typeof(GovernanceReviewDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateReview(
        [FromBody] CreateGovernanceReviewRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TitleEnglish))
            return BadRequest(new { error = "Title is required" });

        var review = await _governanceService.CreateReviewAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
    }

    /// <summary>
    /// Get a governance review by ID.
    /// </summary>
    [HttpGet("reviews/{id:guid}")]
    [ProducesResponseType(typeof(GovernanceReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReview(
        Guid id,
        CancellationToken cancellationToken)
    {
        var review = await _governanceService.GetReviewByIdAsync(id, cancellationToken);
        if (review == null)
            return NotFound();

        return Ok(review);
    }

    /// <summary>
    /// List governance reviews, optionally filtered by status.
    /// </summary>
    [HttpGet("reviews")]
    [ProducesResponseType(typeof(IReadOnlyList<GovernanceReviewDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListReviews(
        [FromQuery] GovernanceReviewStatus? status,
        CancellationToken cancellationToken)
    {
        var reviews = await _governanceService.ListReviewsAsync(status, cancellationToken);
        return Ok(reviews);
    }

    /// <summary>
    /// Start a scheduled governance review.
    /// </summary>
    [HttpPost("reviews/{id:guid}/start")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartReview(
        Guid id,
        CancellationToken cancellationToken)
    {
        var success = await _governanceService.StartReviewAsync(id, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Record findings for a governance review.
    /// </summary>
    [HttpPost("reviews/{id:guid}/findings")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RecordFindings(
        Guid id,
        [FromBody] RecordFindingsRequest request,
        CancellationToken cancellationToken)
    {
        var success = await _governanceService.RecordFindingsAsync(id, request, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Set remediation plan for a governance review.
    /// </summary>
    [HttpPost("reviews/{id:guid}/remediation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetRemediationPlan(
        Guid id,
        [FromBody] SetRemediationPlanRequest request,
        CancellationToken cancellationToken)
    {
        var success = await _governanceService.SetRemediationPlanAsync(id, request, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Complete a governance review.
    /// </summary>
    [HttpPost("reviews/{id:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteReview(
        Guid id,
        CancellationToken cancellationToken)
    {
        var success = await _governanceService.CompleteReviewAsync(id, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Add an action item to a governance review.
    /// </summary>
    [HttpPost("reviews/{reviewId:guid}/actions")]
    [ProducesResponseType(typeof(GovernanceActionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddAction(
        Guid reviewId,
        [FromBody] CreateGovernanceActionRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TitleEnglish))
            return BadRequest(new { error = "Title is required" });

        try
        {
            var action = await _governanceService.AddActionAsync(reviewId, request, cancellationToken);
            return Created($"/api/admin/governance/reviews/{reviewId}/actions/{action.Id}", action);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Complete a governance action item.
    /// </summary>
    [HttpPost("actions/{actionId:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteAction(
        Guid actionId,
        [FromBody] ReviewRequest? request,
        CancellationToken cancellationToken)
    {
        var success = await _governanceService.CompleteActionAsync(actionId, request?.Notes, cancellationToken);
        return success ? NoContent() : NotFound();
    }
}
