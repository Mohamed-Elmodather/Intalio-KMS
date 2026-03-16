using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Collaboration.Application.DTOs;
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

    public LessonsLearnedController(ILogger<LessonsLearnedController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of lessons learned.
    /// </summary>
    [HttpGet]
    public ActionResult<ApiResponse<PagedResult<LessonLearnedSummaryDto>>> GetLessonsLearned(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] string? impact,
        [FromQuery] Guid? projectId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var lessons = new List<LessonLearnedSummaryDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Effective Crowd Management at Large Events",
                TitleArabic = "إدارة الحشود الفعالة في الفعاليات الكبيرة",
                DescriptionPreview = "Key insights from managing crowd flow during the opening ceremony rehearsal...",
                Category = "Process",
                Impact = "High",
                Status = "Published",
                AuthorName = "Mohammed Al-Rashid",
                ProjectName = "Opening Ceremony",
                ViewCount = 456,
                UsefulCount = 89,
                Tags = new[] { "Crowd Management", "Safety", "Events" },
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Vendor Communication Best Practices",
                TitleArabic = "أفضل ممارسات التواصل مع الموردين",
                DescriptionPreview = "Lessons from coordinating with multiple vendors during venue setup...",
                Category = "VendorManagement",
                Impact = "Medium",
                Status = "Published",
                AuthorName = "Sara Ali",
                ProjectName = "Venue Setup",
                ViewCount = 234,
                UsefulCount = 45,
                Tags = new[] { "Vendors", "Communication", "Coordination" },
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Technology Integration Challenges",
                TitleArabic = "تحديات تكامل التقنية",
                DescriptionPreview = "Technical challenges faced during system integration and how we resolved them...",
                Category = "Technical",
                Impact = "Critical",
                Status = "Published",
                AuthorName = "Ahmed Hassan",
                ProjectName = "IT Infrastructure",
                ViewCount = 567,
                UsefulCount = 123,
                Tags = new[] { "Technology", "Integration", "Systems" },
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            }
        };

        var result = PagedResult<LessonLearnedSummaryDto>.Create(lessons, 45, page, pageSize);
        return Ok(ApiResponse<PagedResult<LessonLearnedSummaryDto>>.Ok(result));
    }

    /// <summary>
    /// Get lesson learned by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public ActionResult<ApiResponse<LessonLearnedDto>> GetLessonLearned(Guid id)
    {
        var lesson = new LessonLearnedDto
        {
            Id = id,
            Title = "Effective Crowd Management at Large Events",
            TitleArabic = "إدارة الحشود الفعالة في الفعاليات الكبيرة",
            Description = "This lesson documents our experience managing large crowds during the opening ceremony rehearsal, including what worked well and what we would do differently.",
            DescriptionArabic = "هذا الدرس يوثق تجربتنا في إدارة الحشود الكبيرة خلال بروفة حفل الافتتاح",
            Context = "During the opening ceremony rehearsal, we had approximately 15,000 attendees. This was our first large-scale crowd management test.",
            ContextArabic = "خلال بروفة حفل الافتتاح، كان لدينا حوالي 15,000 حاضر. كان هذا أول اختبار لإدارة الحشود على نطاق واسع.",
            Challenge = "Managing crowd flow at entry points became congested, causing delays of up to 45 minutes for some attendees. Communication between security teams was also inconsistent.",
            ChallengeArabic = "أصبح تدفق الحشود عند نقاط الدخول مكتظاً، مما تسبب في تأخيرات تصل إلى 45 دقيقة لبعض الحضور.",
            Solution = "We implemented a zone-based entry system with dedicated lanes for different ticket types. Added real-time radio communication protocols and deployed crowd density monitoring.",
            SolutionArabic = "قمنا بتنفيذ نظام دخول قائم على المناطق مع ممرات مخصصة لأنواع التذاكر المختلفة.",
            Outcome = "Entry times reduced by 60%. Attendee satisfaction scores improved significantly. Zero safety incidents reported.",
            OutcomeArabic = "انخفضت أوقات الدخول بنسبة 60%. تحسنت درجات رضا الحضور بشكل ملحوظ.",
            Recommendations = "1. Always conduct crowd simulations before large events\n2. Establish clear communication protocols with all teams\n3. Deploy crowd monitoring technology at all major venues\n4. Train staff on crowd management best practices",
            RecommendationsArabic = "1. إجراء محاكاة للحشود دائماً قبل الفعاليات الكبيرة\n2. وضع بروتوكولات اتصال واضحة مع جميع الفرق",
            Category = "Process",
            Impact = "High",
            Status = "Published",
            AuthorId = Guid.NewGuid(),
            AuthorName = "Mohammed Al-Rashid",
            AuthorAvatarUrl = "/avatars/mohammed.jpg",
            ProjectId = Guid.NewGuid(),
            ProjectName = "Opening Ceremony",
            OccurredAt = DateTime.UtcNow.AddDays(-30),
            ViewCount = 456,
            UsefulCount = 89,
            IsMarkedUsefulByCurrentUser = true,
            Tags = new[] { "Crowd Management", "Safety", "Events", "Operations" },
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        };

        return Ok(ApiResponse<LessonLearnedDto>.Ok(lesson));
    }

    /// <summary>
    /// Create a new lesson learned.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<LessonLearnedDto>>> CreateLessonLearned([FromBody] CreateLessonLearnedRequest request)
    {
        _logger.LogInformation("Creating lesson learned: {Title}", request.Title);

        await Task.Delay(100);

        var lesson = new LessonLearnedDto
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            TitleArabic = request.TitleArabic,
            Description = request.Description,
            Context = request.Context,
            Challenge = request.Challenge,
            Solution = request.Solution,
            Outcome = request.Outcome,
            Recommendations = request.Recommendations,
            Category = request.Category,
            Impact = request.Impact,
            Status = "Draft",
            AuthorId = Guid.NewGuid(),
            AuthorName = "Current User",
            ProjectId = request.ProjectId,
            OccurredAt = request.OccurredAt,
            Tags = request.Tags?.ToArray() ?? Array.Empty<string>(),
            CreatedAt = DateTime.UtcNow
        };

        return CreatedAtAction(nameof(GetLessonLearned), new { id = lesson.Id }, ApiResponse<LessonLearnedDto>.Ok(lesson));
    }

    /// <summary>
    /// Update a lesson learned.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> UpdateLessonLearned(Guid id, [FromBody] CreateLessonLearnedRequest request)
    {
        _logger.LogInformation("Updating lesson learned {LessonId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Lesson learned updated successfully"));
    }

    /// <summary>
    /// Delete a lesson learned.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> DeleteLessonLearned(Guid id)
    {
        _logger.LogInformation("Deleting lesson learned {LessonId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Lesson learned deleted successfully"));
    }

    /// <summary>
    /// Submit lesson for review.
    /// </summary>
    [HttpPost("{id:guid}/submit")]
    public async Task<ActionResult<ApiResponse>> SubmitForReview(Guid id)
    {
        _logger.LogInformation("Submitting lesson {LessonId} for review", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Lesson submitted for review"));
    }

    /// <summary>
    /// Approve a lesson.
    /// </summary>
    [HttpPost("{id:guid}/approve")]
    [Authorize(Policy = "CanApproveLessons")]
    public async Task<ActionResult<ApiResponse>> ApproveLessonLearned(Guid id)
    {
        _logger.LogInformation("Approving lesson {LessonId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Lesson approved"));
    }

    /// <summary>
    /// Reject a lesson.
    /// </summary>
    [HttpPost("{id:guid}/reject")]
    [Authorize(Policy = "CanApproveLessons")]
    public async Task<ActionResult<ApiResponse>> RejectLessonLearned(Guid id, [FromBody] string reason)
    {
        _logger.LogInformation("Rejecting lesson {LessonId}: {Reason}", id, reason);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Lesson rejected"));
    }

    /// <summary>
    /// Publish a lesson.
    /// </summary>
    [HttpPost("{id:guid}/publish")]
    public async Task<ActionResult<ApiResponse>> PublishLessonLearned(Guid id)
    {
        _logger.LogInformation("Publishing lesson {LessonId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Lesson published"));
    }

    /// <summary>
    /// Mark lesson as useful.
    /// </summary>
    [HttpPost("{id:guid}/useful")]
    public async Task<ActionResult<ApiResponse>> MarkAsUseful(Guid id)
    {
        _logger.LogInformation("Marking lesson {LessonId} as useful", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Marked as useful"));
    }

    /// <summary>
    /// Unmark lesson as useful.
    /// </summary>
    [HttpDelete("{id:guid}/useful")]
    public async Task<ActionResult<ApiResponse>> UnmarkAsUseful(Guid id)
    {
        _logger.LogInformation("Unmarking lesson {LessonId} as useful", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Unmarked as useful"));
    }

    /// <summary>
    /// Get related lessons.
    /// </summary>
    [HttpGet("{id:guid}/related")]
    public ActionResult<ApiResponse<IReadOnlyList<LessonLearnedSummaryDto>>> GetRelatedLessons(Guid id, [FromQuery] int limit = 5)
    {
        var lessons = new List<LessonLearnedSummaryDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Emergency Evacuation Procedures",
                Category = "Process",
                Impact = "Critical",
                Status = "Published",
                AuthorName = "Sara Ali",
                ViewCount = 345,
                UsefulCount = 78,
                Tags = new[] { "Safety", "Emergency", "Procedures" },
                CreatedAt = DateTime.UtcNow.AddDays(-25)
            }
        };

        return Ok(ApiResponse<IReadOnlyList<LessonLearnedSummaryDto>>.Ok(lessons));
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
    public ActionResult<ApiResponse<IReadOnlyList<LessonActionDto>>> GetActions(Guid id)
    {
        var actions = new List<LessonActionDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                LessonLearnedId = id,
                Description = "Update crowd management standard operating procedures to include zone-based entry system",
                DescriptionArabic = "تحديث إجراءات التشغيل القياسية لإدارة الحشود لتشمل نظام الدخول القائم على المناطق",
                AssigneeId = Guid.NewGuid(),
                AssigneeName = "Khalid Al-Mansoori",
                Status = "InProgress",
                Priority = "High",
                DueDate = DateTime.UtcNow.AddDays(14),
                StartedAt = DateTime.UtcNow.AddDays(-3),
                AffectedDocumentTitle = "Crowd Management SOP v2.1",
                AffectedDocumentType = "Document",
                IsOverdue = false,
                ReminderCount = 0,
                SortOrder = 0,
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                LessonLearnedId = id,
                Description = "Procure and deploy crowd density monitoring sensors at all major venue entry points",
                DescriptionArabic = "شراء ونشر أجهزة استشعار كثافة الحشود عند جميع نقاط دخول الأماكن الرئيسية",
                AssigneeId = Guid.NewGuid(),
                AssigneeName = "Sara Ali",
                Status = "Open",
                Priority = "Urgent",
                DueDate = DateTime.UtcNow.AddDays(-2),
                IsOverdue = true,
                ReminderCount = 2,
                SortOrder = 1,
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                LessonLearnedId = id,
                Description = "Conduct training sessions on radio communication protocols for all security teams",
                DescriptionArabic = "إجراء دورات تدريبية على بروتوكولات الاتصال اللاسلكي لجميع فرق الأمن",
                AssigneeId = Guid.NewGuid(),
                AssigneeName = "Ahmed Hassan",
                Status = "Completed",
                Priority = "Normal",
                DueDate = DateTime.UtcNow.AddDays(-10),
                StartedAt = DateTime.UtcNow.AddDays(-20),
                CompletedAt = DateTime.UtcNow.AddDays(-12),
                CompletionNotes = "Completed 4 training sessions covering all 120 security team members",
                IsOverdue = false,
                ReminderCount = 0,
                SortOrder = 2,
                CreatedAt = DateTime.UtcNow.AddDays(-25)
            }
        };

        return Ok(ApiResponse<IReadOnlyList<LessonActionDto>>.Ok(actions));
    }

    /// <summary>
    /// Create an action item for a lesson.
    /// </summary>
    [HttpPost("{id:guid}/actions")]
    public async Task<ActionResult<ApiResponse<LessonActionDto>>> CreateAction(
        Guid id, [FromBody] CreateLessonActionRequest request)
    {
        _logger.LogInformation("Creating action for lesson {LessonId}: {Description}", id, request.Description);

        await Task.Delay(100);

        var action = new LessonActionDto
        {
            Id = Guid.NewGuid(),
            LessonLearnedId = id,
            Description = request.Description,
            DescriptionArabic = request.DescriptionArabic,
            AssigneeId = request.AssigneeId,
            AssigneeName = "Assigned User",
            Status = "Open",
            Priority = request.Priority,
            DueDate = request.DueDate,
            AffectedDocumentId = request.AffectedDocumentId,
            AffectedDocumentTitle = request.AffectedDocumentTitle,
            AffectedDocumentType = request.AffectedDocumentType,
            IsOverdue = false,
            SortOrder = 0,
            CreatedAt = DateTime.UtcNow
        };

        return CreatedAtAction(nameof(GetActions), new { id }, ApiResponse<LessonActionDto>.Ok(action));
    }

    /// <summary>
    /// Update an action item.
    /// </summary>
    [HttpPut("{id:guid}/actions/{actionId:guid}")]
    public async Task<ActionResult<ApiResponse>> UpdateAction(
        Guid id, Guid actionId, [FromBody] UpdateLessonActionRequest request)
    {
        _logger.LogInformation("Updating action {ActionId} for lesson {LessonId}", actionId, id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Action updated successfully"));
    }

    /// <summary>
    /// Start progress on an action.
    /// </summary>
    [HttpPost("{id:guid}/actions/{actionId:guid}/start")]
    public async Task<ActionResult<ApiResponse>> StartAction(Guid id, Guid actionId)
    {
        _logger.LogInformation("Starting action {ActionId} for lesson {LessonId}", actionId, id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Action started"));
    }

    /// <summary>
    /// Complete an action.
    /// </summary>
    [HttpPost("{id:guid}/actions/{actionId:guid}/complete")]
    public async Task<ActionResult<ApiResponse>> CompleteAction(
        Guid id, Guid actionId, [FromBody] CompleteActionRequest request)
    {
        _logger.LogInformation("Completing action {ActionId} for lesson {LessonId}", actionId, id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Action completed"));
    }

    /// <summary>
    /// Verify an action was implemented effectively.
    /// </summary>
    [HttpPost("{id:guid}/actions/{actionId:guid}/verify")]
    [Authorize(Policy = "CanApproveLessons")]
    public async Task<ActionResult<ApiResponse>> VerifyAction(
        Guid id, Guid actionId, [FromBody] VerifyActionRequest request)
    {
        _logger.LogInformation("Verifying action {ActionId} for lesson {LessonId}", actionId, id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Action verified"));
    }

    /// <summary>
    /// Cancel an action.
    /// </summary>
    [HttpPost("{id:guid}/actions/{actionId:guid}/cancel")]
    public async Task<ActionResult<ApiResponse>> CancelAction(Guid id, Guid actionId)
    {
        _logger.LogInformation("Cancelling action {ActionId} for lesson {LessonId}", actionId, id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Action cancelled"));
    }

    /// <summary>
    /// Delegate an action to another user.
    /// </summary>
    [HttpPost("{id:guid}/actions/{actionId:guid}/delegate")]
    public async Task<ActionResult<ApiResponse>> DelegateAction(
        Guid id, Guid actionId, [FromBody] DelegateActionRequest request)
    {
        _logger.LogInformation("Delegating action {ActionId} to {DelegateTo}", actionId, request.DelegateToName);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Action delegated"));
    }

    /// <summary>
    /// Link a document/procedure to an action.
    /// </summary>
    [HttpPost("{id:guid}/actions/{actionId:guid}/link-document")]
    public async Task<ActionResult<ApiResponse>> LinkDocument(
        Guid id, Guid actionId, [FromBody] LinkDocumentRequest request)
    {
        _logger.LogInformation("Linking document {DocumentId} to action {ActionId}", request.DocumentId, actionId);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Document linked to action"));
    }

    // ========================================
    // Process Owner & Root Cause Endpoints
    // ========================================

    /// <summary>
    /// Assign a process owner to a lesson.
    /// </summary>
    [HttpPost("{id:guid}/assign-process-owner")]
    public async Task<ActionResult<ApiResponse>> AssignProcessOwner(
        Guid id, [FromBody] AssignProcessOwnerRequest request)
    {
        _logger.LogInformation("Assigning process owner {OwnerId} to lesson {LessonId}", request.ProcessOwnerId, id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Process owner assigned"));
    }

    /// <summary>
    /// Set root cause analysis for a lesson.
    /// </summary>
    [HttpPut("{id:guid}/root-cause")]
    public async Task<ActionResult<ApiResponse>> SetRootCause(
        Guid id, [FromBody] SetRootCauseRequest request)
    {
        _logger.LogInformation("Setting root cause for lesson {LessonId}: {Method}", id, request.Method);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Root cause set"));
    }

    // ========================================
    // Extended Workflow Endpoints
    // ========================================

    /// <summary>
    /// Transition lesson to ActionsPending state.
    /// </summary>
    [HttpPost("{id:guid}/mark-actions-pending")]
    public async Task<ActionResult<ApiResponse>> MarkActionsPending(Guid id)
    {
        _logger.LogInformation("Marking lesson {LessonId} as actions pending", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Lesson marked as actions pending"));
    }

    /// <summary>
    /// Transition lesson to ActionsComplete state.
    /// </summary>
    [HttpPost("{id:guid}/mark-actions-complete")]
    public async Task<ActionResult<ApiResponse>> MarkActionsComplete(Guid id)
    {
        _logger.LogInformation("Marking lesson {LessonId} as actions complete", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Lesson marked as actions complete"));
    }

    /// <summary>
    /// Verify lesson — all actions implemented and effective.
    /// </summary>
    [HttpPost("{id:guid}/verify-lesson")]
    [Authorize(Policy = "CanApproveLessons")]
    public async Task<ActionResult<ApiResponse>> VerifyLesson(Guid id)
    {
        _logger.LogInformation("Verifying lesson {LessonId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Lesson verified"));
    }

    // ========================================
    // Analytics Endpoints
    // ========================================

    /// <summary>
    /// Get lessons learned analytics overview.
    /// </summary>
    [HttpGet("analytics/overview")]
    public ActionResult<ApiResponse<LessonsAnalyticsOverviewDto>> GetAnalyticsOverview(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var analytics = new LessonsAnalyticsOverviewDto
        {
            TotalLessons = 87,
            LessonsCreatedInPeriod = 12,
            LessonsPublishedInPeriod = 8,
            LessonsByStatus = new Dictionary<string, int>
            {
                ["Draft"] = 5,
                ["PendingReview"] = 3,
                ["Approved"] = 2,
                ["Published"] = 45,
                ["ActionsPending"] = 15,
                ["ActionsComplete"] = 8,
                ["Verified"] = 6,
                ["Archived"] = 3
            },
            LessonsByCategory = new Dictionary<string, int>
            {
                ["Process"] = 22,
                ["Technical"] = 18,
                ["Communication"] = 12,
                ["TeamManagement"] = 10,
                ["RiskManagement"] = 8,
                ["VendorManagement"] = 7,
                ["QualityAssurance"] = 5,
                ["Other"] = 5
            },
            LessonsByImpact = new Dictionary<string, int>
            {
                ["Critical"] = 8,
                ["High"] = 25,
                ["Medium"] = 38,
                ["Low"] = 16
            },
            TotalActions = 156,
            OpenActions = 34,
            CompletedActions = 89,
            VerifiedActions = 22,
            OverdueActions = 11,
            ActionCompletionRate = 71.2,
            AverageTimeToCompleteActionDays = 18.5,
            TotalViews = 12450,
            TotalUsefulVotes = 2340,
            TopContributors = new List<LessonContributorDto>
            {
                new() { UserId = Guid.NewGuid(), UserName = "Mohammed Al-Rashid", LessonsAuthored = 12, ActionsCompleted = 8, UsefulVotesReceived = 234 },
                new() { UserId = Guid.NewGuid(), UserName = "Sara Ali", LessonsAuthored = 9, ActionsCompleted = 15, UsefulVotesReceived = 189 },
                new() { UserId = Guid.NewGuid(), UserName = "Ahmed Hassan", LessonsAuthored = 7, ActionsCompleted = 12, UsefulVotesReceived = 156 }
            },
            OverdueActionsList = new List<OverdueActionSummaryDto>
            {
                new() { ActionId = Guid.NewGuid(), ActionDescription = "Deploy crowd density sensors", LessonId = Guid.NewGuid(), LessonTitle = "Crowd Management", AssigneeName = "Sara Ali", DueDate = DateTime.UtcNow.AddDays(-5), DaysOverdue = 5, ReminderCount = 2 }
            },
            LessonsWithoutProcessOwner = 14
        };

        return Ok(ApiResponse<LessonsAnalyticsOverviewDto>.Ok(analytics));
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
