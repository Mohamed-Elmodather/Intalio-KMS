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
            new { Value = "VendorManagement", Label = "Vendor Management", LabelArabic = "إدارة الموردين", Count = 5 },
            new { Value = "Other", Label = "Other", LabelArabic = "أخرى", Count = 3 }
        };

        return Ok(ApiResponse<IReadOnlyList<object>>.Ok(categories));
    }
}
