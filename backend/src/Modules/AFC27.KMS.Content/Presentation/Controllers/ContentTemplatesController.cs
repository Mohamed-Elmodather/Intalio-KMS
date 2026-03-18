using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Content.Application.DTOs;
using AFC27.KMS.Content.Application.Interfaces;
using AFC27.KMS.Contracts.Common;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.Content.Presentation.Controllers;

/// <summary>
/// Content templates management controller (Phase 3C).
/// Provides CRUD operations for reusable content templates with review cycles (Phase 3E).
/// </summary>
[ApiController]
[Route("api/content/templates")]
[Authorize]
public class ContentTemplatesController : ControllerBase
{
    private readonly IContentTemplateService _templateService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<ContentTemplatesController> _logger;

    public ContentTemplatesController(
        IContentTemplateService templateService,
        ICurrentUser currentUser,
        ILogger<ContentTemplatesController> logger)
    {
        _templateService = templateService;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <summary>
    /// Get a paginated list of content templates.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ContentTemplateSummaryDto>>>> GetTemplates(
        [FromQuery] ContentTemplateFilterRequest filter)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;
        var result = await _templateService.GetTemplatesAsync(filter, userId);
        return Ok(ApiResponse<PagedResult<ContentTemplateSummaryDto>>.Ok(result));
    }

    /// <summary>
    /// Get a content template by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ContentTemplateDto>>> GetTemplate(Guid id)
    {
        var template = await _templateService.GetTemplateByIdAsync(id);

        if (template == null)
            return NotFound(ApiResponse<ContentTemplateDto>.Fail("Template not found"));

        return Ok(ApiResponse<ContentTemplateDto>.Ok(template));
    }

    /// <summary>
    /// Create a new content template.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "CanCreateContent")]
    public async Task<ActionResult<ApiResponse<ContentTemplateDto>>> CreateTemplate(
        [FromBody] CreateContentTemplateRequest request)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;
        var userName = _currentUser.DisplayName ?? "Unknown";

        _logger.LogInformation("Creating content template '{Name}' by {UserName}", request.Name, userName);

        var template = await _templateService.CreateTemplateAsync(request, userId, userName);

        return CreatedAtAction(
            nameof(GetTemplate),
            new { id = template.Id },
            ApiResponse<ContentTemplateDto>.Ok(template, "Template created successfully"));
    }

    /// <summary>
    /// Update an existing content template.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CanEditContent")]
    public async Task<ActionResult<ApiResponse>> UpdateTemplate(
        Guid id,
        [FromBody] UpdateContentTemplateRequest request)
    {
        _logger.LogInformation("Updating content template {TemplateId}", id);

        var result = await _templateService.UpdateTemplateAsync(id, request);

        if (!result)
            return NotFound(ApiResponse.Fail("Template not found"));

        return Ok(ApiResponse.Ok("Template updated successfully"));
    }

    /// <summary>
    /// Delete a content template.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "CanDeleteContent")]
    public async Task<ActionResult<ApiResponse>> DeleteTemplate(Guid id)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;

        _logger.LogInformation("Deleting content template {TemplateId}", id);

        var result = await _templateService.DeleteTemplateAsync(id, userId);

        if (!result)
            return NotFound(ApiResponse.Fail("Template not found"));

        return Ok(ApiResponse.Ok("Template deleted successfully"));
    }

    /// <summary>
    /// Get available template categories.
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<string>>>> GetCategories()
    {
        var categories = await _templateService.GetCategoriesAsync();
        return Ok(ApiResponse<IReadOnlyList<string>>.Ok(categories));
    }

    /// <summary>
    /// Increment the usage count when a template is used to create content.
    /// </summary>
    [HttpPost("{id:guid}/use")]
    public async Task<ActionResult<ApiResponse>> UseTemplate(Guid id)
    {
        var result = await _templateService.IncrementUsageAsync(id);

        if (!result)
            return NotFound(ApiResponse.Fail("Template not found"));

        return Ok(ApiResponse.Ok("Template usage recorded"));
    }

    // ========================================
    // Phase 3E: Template Review Cycles
    // ========================================

    /// <summary>
    /// Mark a template as reviewed.
    /// </summary>
    [HttpPost("{id:guid}/review")]
    [Authorize(Policy = "CanEditContent")]
    public async Task<ActionResult<ApiResponse>> ReviewTemplate(
        Guid id,
        [FromBody] ReviewTemplateRequest request)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;
        var userName = _currentUser.DisplayName ?? "Unknown";

        _logger.LogInformation("Reviewing template {TemplateId} by {UserName}", id, userName);

        var result = await _templateService.ReviewTemplateAsync(id, userId, userName, request);

        if (!result)
            return NotFound(ApiResponse.Fail("Template not found"));

        return Ok(ApiResponse.Ok("Template reviewed successfully"));
    }

    /// <summary>
    /// Set or update the review interval for a template.
    /// </summary>
    [HttpPut("{id:guid}/review-interval")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<ApiResponse>> SetReviewInterval(
        Guid id,
        [FromBody] SetReviewIntervalRequest request)
    {
        _logger.LogInformation(
            "Setting review interval for template {TemplateId} to {Days} days",
            id, request.ReviewIntervalDays);

        var result = await _templateService.SetReviewIntervalAsync(id, request.ReviewIntervalDays);

        if (!result)
            return NotFound(ApiResponse.Fail("Template not found"));

        return Ok(ApiResponse.Ok("Review interval updated successfully"));
    }

    /// <summary>
    /// Get templates whose review is overdue.
    /// </summary>
    [HttpGet("overdue")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ContentTemplateSummaryDto>>>> GetOverdueTemplates()
    {
        var templates = await _templateService.GetOverdueTemplatesAsync();
        return Ok(ApiResponse<IReadOnlyList<ContentTemplateSummaryDto>>.Ok(templates));
    }

    /// <summary>
    /// Get templates whose review is due soon.
    /// </summary>
    [HttpGet("due-soon")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ContentTemplateSummaryDto>>>> GetDueSoonTemplates(
        [FromQuery] int daysThreshold = 7)
    {
        var templates = await _templateService.GetDueSoonTemplatesAsync(daysThreshold);
        return Ok(ApiResponse<IReadOnlyList<ContentTemplateSummaryDto>>.Ok(templates));
    }
}
