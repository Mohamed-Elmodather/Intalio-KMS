using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.WebApi.Features.QualityAssurance.Models;
using AFC27.KMS.WebApi.Features.QualityAssurance.Services;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.WebApi.Features.QualityAssurance.Controllers;

/// <summary>
/// Controller for document quality assurance operations
/// </summary>
[ApiController]
[Route("api/quality")]
[Authorize]
public class QualityAssuranceController : ControllerBase
{
    private readonly IQualityAssuranceService _qaService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<QualityAssuranceController> _logger;

    public QualityAssuranceController(
        IQualityAssuranceService qaService,
        ICurrentUser currentUser,
        ILogger<QualityAssuranceController> logger)
    {
        _qaService = qaService;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <summary>
    /// Gets quality reviews with filtering
    /// </summary>
    [HttpGet("reviews")]
    public async Task<ActionResult<object>> GetReviews(
        [FromQuery] ReviewStatus? status,
        [FromQuery] ReviewType? type,
        [FromQuery] Guid? assignedTo,
        [FromQuery] Guid? documentId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (reviews, totalCount) = await _qaService.GetReviewsAsync(
            status, type, assignedTo, documentId, page, pageSize, cancellationToken);

        return Ok(new
        {
            data = reviews,
            pagination = new
            {
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        });
    }

    /// <summary>
    /// Gets a specific quality review
    /// </summary>
    [HttpGet("reviews/{id:guid}")]
    public async Task<ActionResult<QualityReview>> GetReview(
        Guid id,
        CancellationToken cancellationToken)
    {
        var review = await _qaService.GetReviewAsync(id, cancellationToken);
        if (review == null)
            return NotFound(new { message = $"Review {id} not found" });

        return Ok(review);
    }

    /// <summary>
    /// Creates a new quality review request
    /// </summary>
    [HttpPost("reviews")]
    public async Task<ActionResult<QualityReview>> CreateReview(
        [FromBody] CreateQualityReviewRequest request,
        CancellationToken cancellationToken)
    {
        var review = await _qaService.CreateReviewAsync(
            request,
            _currentUser.UserId ?? Guid.Empty,
            cancellationToken);

        return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
    }

    /// <summary>
    /// Assigns a review to a user
    /// </summary>
    [HttpPost("reviews/{id:guid}/assign")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<QualityReview>> AssignReview(
        Guid id,
        [FromBody] AssignReviewRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var review = await _qaService.AssignReviewAsync(
                id,
                request.AssignToUserId,
                cancellationToken);

            return Ok(review);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Review {id} not found" });
        }
    }

    /// <summary>
    /// Starts a review (changes status to in progress)
    /// </summary>
    [HttpPost("reviews/{id:guid}/start")]
    public async Task<ActionResult<QualityReview>> StartReview(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var review = await _qaService.StartReviewAsync(
                id,
                _currentUser.UserId ?? Guid.Empty,
                cancellationToken);

            return Ok(review);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Review {id} not found" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Submits a completed review
    /// </summary>
    [HttpPost("reviews/{id:guid}/submit")]
    public async Task<ActionResult<QualityReview>> SubmitReview(
        Guid id,
        [FromBody] SubmitReviewRequest request,
        CancellationToken cancellationToken)
    {
        request.ReviewId = id;

        try
        {
            var review = await _qaService.SubmitReviewAsync(
                request,
                _currentUser.UserId ?? Guid.Empty,
                cancellationToken);

            return Ok(review);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Review {id} not found" });
        }
    }

    /// <summary>
    /// Cancels a review
    /// </summary>
    [HttpPost("reviews/{id:guid}/cancel")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<IActionResult> CancelReview(
        Guid id,
        [FromBody] CancelReviewRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _qaService.CancelReviewAsync(
                id,
                request.Reason,
                _currentUser.UserId ?? Guid.Empty,
                cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Review {id} not found" });
        }
    }

    /// <summary>
    /// Adds a comment to a review
    /// </summary>
    [HttpPost("reviews/{id:guid}/comments")]
    public async Task<ActionResult<ReviewComment>> AddComment(
        Guid id,
        [FromBody] AddCommentRequest request,
        CancellationToken cancellationToken)
    {
        var comment = await _qaService.AddCommentAsync(
            id,
            request.Content,
            request.Type,
            request.Location,
            _currentUser.UserId ?? Guid.Empty,
            _currentUser.DisplayName ?? "Unknown",
            request.ParentCommentId,
            cancellationToken);

        return CreatedAtAction(nameof(GetReview), new { id }, comment);
    }

    /// <summary>
    /// Gets comments for a review
    /// </summary>
    [HttpGet("reviews/{id:guid}/comments")]
    public async Task<ActionResult<List<ReviewComment>>> GetComments(
        Guid id,
        CancellationToken cancellationToken)
    {
        var comments = await _qaService.GetCommentsAsync(id, cancellationToken);
        return Ok(comments);
    }

    /// <summary>
    /// Gets my assigned reviews
    /// </summary>
    [HttpGet("my-reviews")]
    public async Task<ActionResult<List<QualityReviewSummary>>> GetMyReviews(
        CancellationToken cancellationToken)
    {
        var reviews = await _qaService.GetMyReviewsAsync(
            _currentUser.UserId ?? Guid.Empty,
            cancellationToken);

        return Ok(reviews);
    }

    /// <summary>
    /// Gets overdue reviews
    /// </summary>
    [HttpGet("overdue")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<List<QualityReviewSummary>>> GetOverdueReviews(
        CancellationToken cancellationToken)
    {
        var reviews = await _qaService.GetOverdueReviewsAsync(cancellationToken);
        return Ok(reviews);
    }

    /// <summary>
    /// Gets quality dashboard summary
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult<QualityDashboardSummary>> GetDashboard(
        CancellationToken cancellationToken)
    {
        var summary = await _qaService.GetDashboardSummaryAsync(
            _currentUser.UserId,
            cancellationToken);

        return Ok(summary);
    }

    /// <summary>
    /// Gets quality history for a document
    /// </summary>
    [HttpGet("documents/{documentId:guid}/history")]
    public async Task<ActionResult<List<QualityReviewSummary>>> GetDocumentQualityHistory(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var history = await _qaService.GetDocumentQualityHistoryAsync(
            documentId,
            cancellationToken);

        return Ok(history);
    }

    /// <summary>
    /// Gets available checklists
    /// </summary>
    [HttpGet("checklists")]
    public async Task<ActionResult<List<QualityChecklist>>> GetChecklists(
        [FromQuery] string? documentType,
        CancellationToken cancellationToken)
    {
        var checklists = await _qaService.GetChecklistsAsync(
            documentType,
            activeOnly: true,
            cancellationToken);

        return Ok(checklists);
    }

    /// <summary>
    /// Creates a new checklist
    /// </summary>
    [HttpPost("checklists")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<QualityChecklist>> CreateChecklist(
        [FromBody] QualityChecklist checklist,
        CancellationToken cancellationToken)
    {
        var created = await _qaService.CreateChecklistAsync(
            checklist,
            _currentUser.UserId ?? Guid.Empty,
            cancellationToken);

        return CreatedAtAction(nameof(GetChecklists), new { }, created);
    }
}

public class AssignReviewRequest
{
    public Guid AssignToUserId { get; set; }
}

public class CancelReviewRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class AddCommentRequest
{
    public string Content { get; set; } = string.Empty;
    public CommentType Type { get; set; } = CommentType.General;
    public string? Location { get; set; }
    public Guid? ParentCommentId { get; set; }
}
