using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.WebApi.Features.Templates.Models;
using AFC27.KMS.WebApi.Features.Templates.Services;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.WebApi.Features.Templates.Controllers;

/// <summary>
/// Controller for managing document templates
/// </summary>
[ApiController]
[Route("api/templates")]
[Authorize]
public class DocumentTemplatesController : ControllerBase
{
    private readonly IDocumentTemplateService _templateService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<DocumentTemplatesController> _logger;

    public DocumentTemplatesController(
        IDocumentTemplateService templateService,
        ICurrentUser currentUser,
        ILogger<DocumentTemplatesController> logger)
    {
        _templateService = templateService;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <summary>
    /// Gets a paginated list of templates
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<object>> GetTemplates(
        [FromQuery] string? category,
        [FromQuery] TemplateType? type,
        [FromQuery] TemplateStatus? status,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (templates, totalCount) = await _templateService.GetTemplatesAsync(
            category, type, status, search, page, pageSize, cancellationToken);

        return Ok(new
        {
            data = templates,
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
    /// Gets a template by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DocumentTemplate>> GetTemplate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var template = await _templateService.GetTemplateAsync(id, cancellationToken);
        if (template == null)
            return NotFound(new { message = $"Template {id} not found" });

        return Ok(template);
    }

    /// <summary>
    /// Creates a new template
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<DocumentTemplate>> CreateTemplate(
        [FromBody] CreateTemplateRequest request,
        CancellationToken cancellationToken)
    {
        var template = await _templateService.CreateTemplateAsync(
            request,
            _currentUser.UserId ?? Guid.Empty,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetTemplate),
            new { id = template.Id },
            template);
    }

    /// <summary>
    /// Updates an existing template
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<DocumentTemplate>> UpdateTemplate(
        Guid id,
        [FromBody] CreateTemplateRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var template = await _templateService.UpdateTemplateAsync(
                id,
                request,
                _currentUser.UserId ?? Guid.Empty,
                cancellationToken);

            return Ok(template);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Template {id} not found" });
        }
    }

    /// <summary>
    /// Deletes a template
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<IActionResult> DeleteTemplate(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _templateService.DeleteTemplateAsync(
                id,
                _currentUser.UserId ?? Guid.Empty,
                cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Template {id} not found" });
        }
    }

    /// <summary>
    /// Validates a template
    /// </summary>
    [HttpPost("{id:guid}/validate")]
    public async Task<ActionResult<TemplateValidationResult>> ValidateTemplate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _templateService.ValidateTemplateAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Publishes a template
    /// </summary>
    [HttpPost("{id:guid}/publish")]
    [Authorize(Policy = "CanPublishContent")]
    public async Task<ActionResult<DocumentTemplate>> PublishTemplate(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var template = await _templateService.PublishTemplateAsync(
                id,
                _currentUser.UserId ?? Guid.Empty,
                cancellationToken);

            return Ok(template);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Template {id} not found" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Deprecates a template
    /// </summary>
    [HttpPost("{id:guid}/deprecate")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<DocumentTemplate>> DeprecateTemplate(
        Guid id,
        [FromBody] DeprecateRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var template = await _templateService.DeprecateTemplateAsync(
                id,
                _currentUser.UserId ?? Guid.Empty,
                request.Reason,
                cancellationToken);

            return Ok(template);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Template {id} not found" });
        }
    }

    /// <summary>
    /// Creates a new version of a template
    /// </summary>
    [HttpPost("{id:guid}/versions")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<DocumentTemplate>> CreateNewVersion(
        Guid id,
        [FromBody] CreateTemplateRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var template = await _templateService.CreateNewVersionAsync(
                id,
                request,
                _currentUser.UserId ?? Guid.Empty,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetTemplate),
                new { id = template.Id },
                template);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Template {id} not found" });
        }
    }

    /// <summary>
    /// Gets version history of a template
    /// </summary>
    [HttpGet("{id:guid}/versions")]
    public async Task<ActionResult<List<TemplateSummaryDto>>> GetTemplateVersions(
        Guid id,
        CancellationToken cancellationToken)
    {
        var versions = await _templateService.GetTemplateVersionsAsync(id, cancellationToken);
        return Ok(versions);
    }

    /// <summary>
    /// Generates a document from a template
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<GeneratedDocumentResponse>> GenerateDocument(
        [FromBody] GenerateDocumentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _templateService.GenerateDocumentAsync(
                request,
                _currentUser.UserId ?? Guid.Empty,
                cancellationToken);

            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Template {request.TemplateId} not found" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Gets template usage statistics
    /// </summary>
    [HttpGet("{id:guid}/stats")]
    public async Task<ActionResult<TemplateUsageStats>> GetTemplateStats(
        Guid id,
        CancellationToken cancellationToken)
    {
        var stats = await _templateService.GetTemplateUsageStatsAsync(id, cancellationToken);
        return Ok(stats);
    }

    /// <summary>
    /// Gets all template categories
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<List<string>>> GetCategories(
        CancellationToken cancellationToken)
    {
        var categories = await _templateService.GetCategoriesAsync(cancellationToken);
        return Ok(categories);
    }

    /// <summary>
    /// Uploads a template file
    /// </summary>
    [HttpPost("upload")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<DocumentTemplate>> UploadTemplate(
        [FromForm] string name,
        [FromForm] string category,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file provided" });

        using var stream = file.OpenReadStream();
        var template = await _templateService.UploadTemplateFileAsync(
            name,
            category,
            stream,
            file.FileName,
            _currentUser.UserId ?? Guid.Empty,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetTemplate),
            new { id = template.Id },
            template);
    }

    /// <summary>
    /// Validates template content without saving
    /// </summary>
    [HttpPost("validate-content")]
    public ActionResult<TemplateValidationResult> ValidateContent(
        [FromBody] ValidateContentRequest request)
    {
        var result = _templateService.ValidateTemplateContent(request.Content, request.Type);
        return Ok(result);
    }
}

public class DeprecateRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class ValidateContentRequest
{
    public string Content { get; set; } = string.Empty;
    public TemplateType Type { get; set; }
}
