using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Collaboration.Application.DTOs;
using AFC27.KMS.Collaboration.Application.Interfaces;
using AFC27.KMS.Contracts.Common;

namespace AFC27.KMS.Collaboration.Presentation.Controllers;

/// <summary>
/// Lessons Learned management controller.
/// </summary>
[ApiController]
[Route("api/collaboration/lessons-learned")]
[Authorize]
public class LessonsLearnedController : ControllerBase
{
    private readonly ILogger<LessonsLearnedController> _logger;
    private readonly ILessonLearnedService _lessonService;
    private readonly ILessonActionService _actionService;

    public LessonsLearnedController(
        ILogger<LessonsLearnedController> logger,
        ILessonLearnedService lessonService,
        ILessonActionService actionService)
    {
        _logger = logger;
        _lessonService = lessonService;
        _actionService = actionService;
    }

    private Guid GetCurrentUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

    /// <summary>
    /// Get paginated list of lessons learned.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetLessonsLearned(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] string? impact,
        [FromQuery] string? status,
        [FromQuery] Guid? projectId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _lessonService.GetLessonsAsync(search, category, impact, status, projectId, page, pageSize, ct);
        return Ok(result);
    }

    /// <summary>
    /// Get lesson learned by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetLessonLearned(Guid id, CancellationToken ct = default)
    {
        var lesson = await _lessonService.GetLessonByIdAsync(id, GetCurrentUserId(), ct);
        if (lesson == null) return NotFound();
        return Ok(lesson);
    }

    /// <summary>
    /// Create a new lesson learned.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CreateLessonLearned(
        [FromBody] CreateLessonLearnedRequest request, CancellationToken ct = default)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
        var lesson = await _lessonService.CreateLessonAsync(request, GetCurrentUserId(), userName, ct);
        return CreatedAtAction(nameof(GetLessonLearned), new { id = lesson.Id }, lesson);
    }

    /// <summary>
    /// Update a lesson learned.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateLessonLearned(
        Guid id, [FromBody] CreateLessonLearnedRequest request, CancellationToken ct = default)
    {
        var result = await _lessonService.UpdateLessonAsync(id, request, GetCurrentUserId(), ct);
        if (!result) return NotFound();
        return Ok();
    }

    /// <summary>
    /// Delete a lesson learned.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteLessonLearned(Guid id, CancellationToken ct = default)
    {
        var result = await _lessonService.DeleteLessonAsync(id, GetCurrentUserId(), ct);
        if (!result) return NotFound();
        return Ok();
    }

    /// <summary>
    /// Submit lesson for review.
    /// </summary>
    [HttpPost("{id:guid}/submit")]
    public async Task<ActionResult> SubmitForReview(Guid id, CancellationToken ct = default)
    {
        var result = await _lessonService.SubmitForReviewAsync(id, GetCurrentUserId(), ct);
        if (!result) return NotFound();
        return Ok();
    }

    /// <summary>
    /// Approve a lesson.
    /// </summary>
    [HttpPost("{id:guid}/approve")]
    [Authorize(Policy = "CanApproveLessons")]
    public async Task<ActionResult> ApproveLessonLearned(Guid id, CancellationToken ct = default)
    {
        var result = await _lessonService.ApproveAsync(id, GetCurrentUserId(), ct);
        if (!result) return NotFound();
        return Ok();
    }

    /// <summary>
    /// Reject a lesson.
    /// </summary>
    [HttpPost("{id:guid}/reject")]
    [Authorize(Policy = "CanApproveLessons")]
    public async Task<ActionResult> RejectLessonLearned(
        Guid id, [FromBody] string reason, CancellationToken ct = default)
    {
        var result = await _lessonService.RejectAsync(id, reason, GetCurrentUserId(), ct);
        if (!result) return NotFound();
        return Ok();
    }

    /// <summary>
    /// Publish a lesson.
    /// </summary>
    [HttpPost("{id:guid}/publish")]
    public async Task<ActionResult> PublishLessonLearned(Guid id, CancellationToken ct = default)
    {
        var result = await _lessonService.PublishAsync(id, GetCurrentUserId(), ct);
        if (!result) return NotFound();
        return Ok();
    }

    /// <summary>
    /// Mark lesson as useful.
    /// </summary>
    [HttpPost("{id:guid}/useful")]
    public async Task<ActionResult> MarkAsUseful(Guid id, CancellationToken ct = default)
    {
        var result = await _lessonService.MarkAsUsefulAsync(id, GetCurrentUserId(), ct);
        if (!result) return NotFound();
        return Ok();
    }

    /// <summary>
    /// Unmark lesson as useful.
    /// </summary>
    [HttpDelete("{id:guid}/useful")]
    public async Task<ActionResult> UnmarkAsUseful(Guid id, CancellationToken ct = default)
    {
        var result = await _lessonService.UnmarkAsUsefulAsync(id, GetCurrentUserId(), ct);
        if (!result) return NotFound();
        return Ok();
    }

    /// <summary>
    /// Get related lessons.
    /// </summary>
    [HttpGet("{id:guid}/related")]
    public async Task<ActionResult> GetRelatedLessons(
        Guid id, [FromQuery] int limit = 5, CancellationToken ct = default)
    {
        var lessons = await _lessonService.GetRelatedLessonsAsync(id, limit, ct);
        return Ok(lessons);
    }

    /// <summary>
    /// Get categories for filtering.
    /// </summary>
    [HttpGet("categories")]
    [AllowAnonymous]
    public ActionResult<ApiResponse<IReadOnlyList<object>>> GetCategories()
    {
        var categories = new List<object>
        {
            new { Value = "Process", Label = "Process", LabelArabic = "العمليات", Count = 15 },
            new { Value = "Technical", Label = "Technical", LabelArabic = "التقنية", Count = 12 },
            new { Value = "Communication", Label = "Communication", LabelArabic = "التواصل", Count = 8 },
            new { Value = "TeamManagement", Label = "Team Management", LabelArabic = "إدارة الفريق", Count = 6 },
            new { Value = "RiskManagement", Label = "Risk Management", LabelArabic = "إدارة المخاطر", Count = 4 },
            new { Value = "StakeholderManagement", Label = "Stakeholder Management", LabelArabic = "إدارة أصحاب المصلحة", Count = 3 },
            new { Value = "BudgetManagement", Label = "Budget Management", LabelArabic = "إدارة الميزانية", Count = 2 },
            new { Value = "QualityAssurance", Label = "Quality Assurance", LabelArabic = "ضمان الجودة", Count = 4 },
            new { Value = "VendorManagement", Label = "Vendor Management", LabelArabic = "إدارة الموردين", Count = 5 },
            new { Value = "Other", Label = "Other", LabelArabic = "أخرى", Count = 3 }
        };

        return Ok(ApiResponse<IReadOnlyList<object>>.Ok(categories));
    }

    // ========================================
    // Action Item Endpoints
    // ========================================

    /// <summary>
    /// Get action items for a lesson.
    /// </summary>
    [HttpGet("{id:guid}/actions")]
    public async Task<ActionResult> GetActions(Guid id, CancellationToken ct = default)
    {
        var actions = await _actionService.GetActionsAsync(id, ct);
        return Ok(actions);
    }

    /// <summary>
    /// Create an action item for a lesson.
    /// </summary>
    [HttpPost("{id:guid}/actions")]
    public async Task<ActionResult> CreateAction(
        Guid id, [FromBody] CreateLessonActionRequest request, CancellationToken ct = default)
    {
        var action = await _actionService.CreateActionAsync(id, request, GetCurrentUserId(), ct);
        return CreatedAtAction(nameof(GetActions), new { id }, action);
    }

    /// <summary>
    /// Update an action item.
    /// </summary>
    [HttpPut("{id:guid}/actions/{actionId:guid}")]
    public async Task<ActionResult> UpdateAction(
        Guid id, Guid actionId, [FromBody] UpdateLessonActionRequest request, CancellationToken ct = default)
    {
        var result = await _actionService.UpdateActionAsync(id, actionId, request, GetCurrentUserId(), ct);
        if (!result) return NotFound();
        return Ok();
    }

    /// <summary>
    /// Start progress on an action.
    /// </summary>
    [HttpPost("{id:guid}/actions/{actionId:guid}/start")]
    public async Task<ActionResult> StartAction(Guid id, Guid actionId, CancellationToken ct = default)
    {
        var result = await _actionService.StartActionAsync(id, actionId, GetCurrentUserId(), ct);
        if (!result) return NotFound();
        return Ok();
    }

    /// <summary>
    /// Complete an action.
    /// </summary>
    [HttpPost("{id:guid}/actions/{actionId:guid}/complete")]
    public async Task<ActionResult> CompleteAction(
        Guid id, Guid actionId, [FromBody] CompleteActionRequest request, CancellationToken ct = default)
    {
        var result = await _actionService.CompleteActionAsync(id, actionId, request, GetCurrentUserId(), ct);
        if (!result) return NotFound();
        return Ok();
    }

    /// <summary>
    /// Verify an action was implemented effectively.
    /// </summary>
    [HttpPost("{id:guid}/actions/{actionId:guid}/verify")]
    [Authorize(Policy = "CanApproveLessons")]
    public async Task<ActionResult> VerifyAction(
        Guid id, Guid actionId, [FromBody] VerifyActionRequest request, CancellationToken ct = default)
    {
        var verifierName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
        var result = await _actionService.VerifyActionAsync(id, actionId, request, GetCurrentUserId(), verifierName, ct);
        if (!result) return NotFound();
        return Ok();
    }

    /// <summary>
    /// Cancel an action.
    /// </summary>
    [HttpPost("{id:guid}/actions/{actionId:guid}/cancel")]
    public async Task<ActionResult> CancelAction(Guid id, Guid actionId, CancellationToken ct = default)
    {
        var result = await _actionService.CancelActionAsync(id, actionId, GetCurrentUserId(), ct);
        if (!result) return NotFound();
        return Ok();
    }

    /// <summary>
    /// Delegate an action to another user.
    /// </summary>
    [HttpPost("{id:guid}/actions/{actionId:guid}/delegate")]
    public async Task<ActionResult> DelegateAction(
        Guid id, Guid actionId, [FromBody] DelegateActionRequest request, CancellationToken ct = default)
    {
        var result = await _actionService.DelegateActionAsync(id, actionId, request, GetCurrentUserId(), ct);
        if (!result) return NotFound();
        return Ok();
    }

    /// <summary>
    /// Link a document/procedure to an action.
    /// </summary>
    [HttpPost("{id:guid}/actions/{actionId:guid}/link-document")]
    public async Task<ActionResult> LinkDocument(
        Guid id, Guid actionId, [FromBody] LinkDocumentRequest request, CancellationToken ct = default)
    {
        var result = await _actionService.LinkDocumentAsync(id, actionId, request, GetCurrentUserId(), ct);
        if (!result) return NotFound();
        return Ok();
    }

    // ========================================
    // Process Owner & Root Cause Endpoints
    // ========================================

    /// <summary>
    /// Assign a process owner to a lesson.
    /// </summary>
    [HttpPost("{id:guid}/assign-process-owner")]
    public async Task<ActionResult> AssignProcessOwner(
        Guid id, [FromBody] AssignProcessOwnerRequest request, CancellationToken ct = default)
    {
        var result = await _lessonService.AssignProcessOwnerAsync(id, request, GetCurrentUserId(), ct);
        if (!result) return NotFound();
        return Ok();
    }

    /// <summary>
    /// Set root cause analysis for a lesson.
    /// </summary>
    [HttpPut("{id:guid}/root-cause")]
    public async Task<ActionResult> SetRootCause(
        Guid id, [FromBody] SetRootCauseRequest request, CancellationToken ct = default)
    {
        var result = await _lessonService.SetRootCauseAsync(id, request, GetCurrentUserId(), ct);
        if (!result) return NotFound();
        return Ok();
    }

    // ========================================
    // Extended Workflow Endpoints
    // ========================================

    /// <summary>
    /// Transition lesson to ActionsPending state.
    /// </summary>
    [HttpPost("{id:guid}/mark-actions-pending")]
    public async Task<ActionResult> MarkActionsPending(Guid id, CancellationToken ct = default)
    {
        var result = await _lessonService.MarkActionsPendingAsync(id, GetCurrentUserId(), ct);
        if (!result) return NotFound();
        return Ok();
    }

    /// <summary>
    /// Transition lesson to ActionsComplete state.
    /// </summary>
    [HttpPost("{id:guid}/mark-actions-complete")]
    public async Task<ActionResult> MarkActionsComplete(Guid id, CancellationToken ct = default)
    {
        var result = await _lessonService.MarkActionsCompleteAsync(id, GetCurrentUserId(), ct);
        if (!result) return NotFound();
        return Ok();
    }

    /// <summary>
    /// Verify lesson — all actions implemented and effective.
    /// </summary>
    [HttpPost("{id:guid}/verify-lesson")]
    [Authorize(Policy = "CanApproveLessons")]
    public async Task<ActionResult> VerifyLesson(Guid id, CancellationToken ct = default)
    {
        var result = await _lessonService.VerifyLessonAsync(id, GetCurrentUserId(), ct);
        if (!result) return NotFound();
        return Ok();
    }

    // ========================================
    // Analytics Endpoints
    // ========================================

    /// <summary>
    /// Get lessons learned analytics overview.
    /// </summary>
    [HttpGet("analytics/overview")]
    public async Task<ActionResult> GetAnalyticsOverview(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct = default)
    {
        var analytics = await _lessonService.GetAnalyticsOverviewAsync(from, to, ct);
        return Ok(analytics);
    }

    /// <summary>
    /// Get project phases for filtering.
    /// </summary>
    [HttpGet("project-phases")]
    [AllowAnonymous]
    public ActionResult<ApiResponse<IReadOnlyList<object>>> GetProjectPhases()
    {
        var phases = new List<object>
        {
            new { Value = "Initiation", Label = "Initiation", LabelArabic = "البدء" },
            new { Value = "Planning", Label = "Planning", LabelArabic = "التخطيط" },
            new { Value = "Execution", Label = "Execution", LabelArabic = "التنفيذ" },
            new { Value = "Monitoring", Label = "Monitoring & Control", LabelArabic = "المراقبة والتحكم" },
            new { Value = "Closure", Label = "Closure", LabelArabic = "الإغلاق" },
            new { Value = "Operations", Label = "Operations", LabelArabic = "العمليات" }
        };

        return Ok(ApiResponse<IReadOnlyList<object>>.Ok(phases));
    }

    /// <summary>
    /// Get impact types for filtering.
    /// </summary>
    [HttpGet("impact-types")]
    [AllowAnonymous]
    public ActionResult<ApiResponse<IReadOnlyList<object>>> GetImpactTypes()
    {
        var types = new List<object>
        {
            new { Value = "Cost", Label = "Cost Impact", LabelArabic = "تأثير التكلفة" },
            new { Value = "Schedule", Label = "Schedule Impact", LabelArabic = "تأثير الجدول الزمني" },
            new { Value = "Quality", Label = "Quality Impact", LabelArabic = "تأثير الجودة" },
            new { Value = "Safety", Label = "Safety Impact", LabelArabic = "تأثير السلامة" },
            new { Value = "Stakeholder", Label = "Stakeholder Impact", LabelArabic = "تأثير أصحاب المصلحة" },
            new { Value = "Reputation", Label = "Reputation Impact", LabelArabic = "تأثير السمعة" },
            new { Value = "Compliance", Label = "Compliance Impact", LabelArabic = "تأثير الامتثال" }
        };

        return Ok(ApiResponse<IReadOnlyList<object>>.Ok(types));
    }
}
