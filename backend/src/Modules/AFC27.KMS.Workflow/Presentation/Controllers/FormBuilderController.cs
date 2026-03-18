using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AFC27.KMS.SharedKernel.Domain;
using AFC27.KMS.Workflow.Domain.Entities;

namespace AFC27.KMS.Workflow.Presentation.Controllers;

/// <summary>
/// CRUD controller for the configurable Form Builder (Phase 10).
/// Manages FormDefinition entities - create, read, update, delete,
/// publish/unpublish, and section/field management.
/// </summary>
[ApiController]
[Route("api/workflow/form-definitions")]
[Authorize]
public class FormBuilderController : ControllerBase
{
    private readonly ILogger<FormBuilderController> _logger;

    // In-memory store (replace with repository in production)
    private static readonly List<FormDefinition> _formDefinitions = new();

    public FormBuilderController(ILogger<FormBuilderController> logger)
    {
        _logger = logger;
    }

    #region CRUD

    /// <summary>
    /// List all form definitions with optional filtering.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FormDefinitionSummaryDto>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<FormDefinitionSummaryDto>> ListFormDefinitions(
        [FromQuery] string? status = null,
        [FromQuery] string? type = null,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // TODO: Query from repository with filtering
        var summaries = _formDefinitions
            .Where(f => string.IsNullOrEmpty(status) || f.Status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase))
            .Where(f => string.IsNullOrEmpty(type) || f.Type.ToString().Equals(type, StringComparison.OrdinalIgnoreCase))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(f => new FormDefinitionSummaryDto
            {
                Id = f.Id,
                Name = f.Name.English,
                NameArabic = f.Name.Arabic,
                Description = f.Description?.English,
                Status = f.Status.ToString(),
                Type = f.Type.ToString(),
                Version = f.Version,
                SectionCount = f.Sections.Count,
                FieldCount = f.Sections.Sum(s => s.Fields.Count),
                SubmissionCount = f.SubmissionCount,
                CreatedAt = f.CreatedAt,
                Deadline = f.Deadline
            })
            .ToList();

        return Ok(summaries);
    }

    /// <summary>
    /// Get a form definition by ID with full details.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FormDefinitionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<FormDefinitionDetailDto> GetFormDefinition(Guid id)
    {
        var form = _formDefinitions.FirstOrDefault(f => f.Id == id);
        if (form == null) return NotFound();

        return Ok(MapToDetailDto(form));
    }

    /// <summary>
    /// Create a new form definition.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(typeof(FormDefinitionDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<FormDefinitionDetailDto> CreateFormDefinition(
        [FromBody] CreateFormDefinitionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { error = "Form name is required" });

        var name = LocalizedString.Create(request.Name, request.NameArabic);
        var description = !string.IsNullOrEmpty(request.Description)
            ? LocalizedString.Create(request.Description, request.DescriptionArabic)
            : null;

        var formType = Enum.TryParse<FormDefinitionType>(request.Type, true, out var t)
            ? t
            : FormDefinitionType.Standard;

        var form = FormDefinition.Create(name, formType);
        form.Update(name, description);

        if (request.AllowSaveDraft.HasValue || request.RequireAuthentication.HasValue)
        {
            form.SetSettings(
                request.AllowSaveDraft ?? true,
                request.AllowMultipleSubmissions ?? false,
                request.ShowProgressIndicator ?? true,
                request.RequireAuthentication ?? true);
        }

        if (request.Deadline.HasValue)
            form.SetDeadline(request.Deadline);

        _formDefinitions.Add(form);

        _logger.LogInformation("Created form definition {FormId}: {Name}", form.Id, request.Name);

        return Created(
            $"/api/workflow/form-definitions/{form.Id}",
            MapToDetailDto(form));
    }

    /// <summary>
    /// Update a form definition.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(typeof(FormDefinitionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<FormDefinitionDetailDto> UpdateFormDefinition(
        Guid id,
        [FromBody] UpdateFormDefinitionRequest request)
    {
        var form = _formDefinitions.FirstOrDefault(f => f.Id == id);
        if (form == null) return NotFound();

        if (!string.IsNullOrEmpty(request.Name))
        {
            var name = LocalizedString.Create(request.Name, request.NameArabic);
            var desc = !string.IsNullOrEmpty(request.Description)
                ? LocalizedString.Create(request.Description, request.DescriptionArabic)
                : null;
            form.Update(name, desc);
        }

        if (request.AllowSaveDraft.HasValue)
        {
            form.SetSettings(
                request.AllowSaveDraft ?? true,
                request.AllowMultipleSubmissions ?? false,
                request.ShowProgressIndicator ?? true,
                request.RequireAuthentication ?? true);
        }

        if (request.Deadline.HasValue)
            form.SetDeadline(request.Deadline);

        _logger.LogInformation("Updated form definition {FormId}", id);

        return Ok(MapToDetailDto(form));
    }

    /// <summary>
    /// Delete a form definition (soft delete).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteFormDefinition(Guid id)
    {
        var form = _formDefinitions.FirstOrDefault(f => f.Id == id);
        if (form == null) return NotFound();

        form.SoftDelete(Guid.Empty);
        _logger.LogInformation("Deleted form definition {FormId}", id);

        return NoContent();
    }

    #endregion

    #region Lifecycle

    /// <summary>
    /// Publish a form definition, making it available for submissions.
    /// </summary>
    [HttpPost("{id:guid}/publish")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult PublishFormDefinition(Guid id)
    {
        var form = _formDefinitions.FirstOrDefault(f => f.Id == id);
        if (form == null) return NotFound();

        form.Publish();
        _logger.LogInformation("Published form definition {FormId}, version: {Version}", id, form.Version);

        return NoContent();
    }

    /// <summary>
    /// Unpublish (revert to draft) a form definition.
    /// </summary>
    [HttpPost("{id:guid}/unpublish")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UnpublishFormDefinition(Guid id)
    {
        var form = _formDefinitions.FirstOrDefault(f => f.Id == id);
        if (form == null) return NotFound();

        form.Unpublish();
        return NoContent();
    }

    /// <summary>
    /// Archive a form definition.
    /// </summary>
    [HttpPost("{id:guid}/archive")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult ArchiveFormDefinition(Guid id)
    {
        var form = _formDefinitions.FirstOrDefault(f => f.Id == id);
        if (form == null) return NotFound();

        form.Archive();
        return NoContent();
    }

    #endregion

    #region Sections

    /// <summary>
    /// Add a section to a form definition.
    /// </summary>
    [HttpPost("{id:guid}/sections")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(typeof(FormDefinitionSectionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<FormDefinitionSectionDto> AddSection(
        Guid id,
        [FromBody] AddSectionRequest request)
    {
        var form = _formDefinitions.FirstOrDefault(f => f.Id == id);
        if (form == null) return NotFound();

        var title = LocalizedString.Create(request.Title, request.TitleArabic);
        var desc = !string.IsNullOrEmpty(request.Description)
            ? LocalizedString.Create(request.Description, request.DescriptionArabic)
            : null;

        var section = form.AddSection(title, desc);

        if (request.IsCollapsible.HasValue)
            section.SetCollapsible(request.IsCollapsible.Value, request.IsCollapsedByDefault ?? false);

        if (request.ColumnCount.HasValue)
            section.SetColumnCount(request.ColumnCount.Value);

        return Created(
            $"/api/workflow/form-definitions/{id}/sections/{section.Id}",
            MapSectionDto(section));
    }

    /// <summary>
    /// Remove a section from a form definition.
    /// </summary>
    [HttpDelete("{id:guid}/sections/{sectionId:guid}")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult RemoveSection(Guid id, Guid sectionId)
    {
        var form = _formDefinitions.FirstOrDefault(f => f.Id == id);
        if (form == null) return NotFound();

        form.RemoveSection(sectionId);
        return NoContent();
    }

    #endregion

    #region Fields

    /// <summary>
    /// Add a field to a section.
    /// </summary>
    [HttpPost("{id:guid}/sections/{sectionId:guid}/fields")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(typeof(FormDefinitionFieldDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<FormDefinitionFieldDto> AddField(
        Guid id,
        Guid sectionId,
        [FromBody] AddFieldRequest request)
    {
        var form = _formDefinitions.FirstOrDefault(f => f.Id == id);
        if (form == null) return NotFound();

        var section = form.Sections.FirstOrDefault(s => s.Id == sectionId);
        if (section == null) return NotFound();

        var fieldType = Enum.TryParse<FormDefinitionFieldType>(request.FieldType, true, out var ft)
            ? ft
            : FormDefinitionFieldType.Text;

        var label = LocalizedString.Create(request.Label, request.LabelArabic);
        var field = section.AddField(request.FieldKey, label, fieldType);

        if (request.IsRequired.HasValue)
            field.SetRequired(request.IsRequired.Value);

        if (!string.IsNullOrEmpty(request.Placeholder))
            field.Update(label, LocalizedString.Create(request.Placeholder), null);

        if (request.ColumnSpan.HasValue)
            field.SetColumnSpan(request.ColumnSpan.Value);

        if (!string.IsNullOrEmpty(request.DefaultValue))
            field.SetDefaultValue(request.DefaultValue);

        if (!string.IsNullOrEmpty(request.OptionsJson))
            field.SetOptions(request.OptionsJson);

        return Created(
            $"/api/workflow/form-definitions/{id}/sections/{sectionId}/fields/{field.Id}",
            MapFieldDto(field));
    }

    /// <summary>
    /// Remove a field from a section.
    /// </summary>
    [HttpDelete("{id:guid}/sections/{sectionId:guid}/fields/{fieldId:guid}")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult RemoveField(Guid id, Guid sectionId, Guid fieldId)
    {
        var form = _formDefinitions.FirstOrDefault(f => f.Id == id);
        if (form == null) return NotFound();

        var section = form.Sections.FirstOrDefault(s => s.Id == sectionId);
        if (section == null) return NotFound();

        section.RemoveField(fieldId);
        return NoContent();
    }

    #endregion

    #region Mapping Helpers

    private static FormDefinitionDetailDto MapToDetailDto(FormDefinition form)
    {
        return new FormDefinitionDetailDto
        {
            Id = form.Id,
            Name = form.Name.English,
            NameArabic = form.Name.Arabic,
            Description = form.Description?.English,
            DescriptionArabic = form.Description?.Arabic,
            Status = form.Status.ToString(),
            Type = form.Type.ToString(),
            Version = form.Version,
            AllowSaveDraft = form.AllowSaveDraft,
            AllowMultipleSubmissions = form.AllowMultipleSubmissions,
            ShowProgressIndicator = form.ShowProgressIndicator,
            RequireAuthentication = form.RequireAuthentication,
            Deadline = form.Deadline,
            MaxSubmissions = form.MaxSubmissions,
            SubmissionCount = form.SubmissionCount,
            LinkedWorkflowDefinitionId = form.LinkedWorkflowDefinitionId,
            CreatedAt = form.CreatedAt,
            Sections = form.Sections.Select(MapSectionDto).ToList()
        };
    }

    private static FormDefinitionSectionDto MapSectionDto(FormDefinitionSection section)
    {
        return new FormDefinitionSectionDto
        {
            Id = section.Id,
            Title = section.Title.English,
            TitleArabic = section.Title.Arabic,
            Description = section.Description?.English,
            Order = section.Order,
            IsCollapsible = section.IsCollapsible,
            IsCollapsedByDefault = section.IsCollapsedByDefault,
            ColumnCount = section.ColumnCount,
            Fields = section.Fields.Select(MapFieldDto).ToList()
        };
    }

    private static FormDefinitionFieldDto MapFieldDto(FormDefinitionField field)
    {
        return new FormDefinitionFieldDto
        {
            Id = field.Id,
            FieldKey = field.FieldKey,
            Label = field.Label.English,
            LabelArabic = field.Label.Arabic,
            Placeholder = field.Placeholder?.English,
            HelpText = field.HelpText?.English,
            FieldType = field.FieldType.ToString(),
            Order = field.Order,
            IsRequired = field.IsRequired,
            ColumnSpan = field.ColumnSpan,
            IsHidden = field.IsHidden,
            IsReadOnly = field.IsReadOnly,
            DefaultValue = field.DefaultValue,
            OptionsJson = field.OptionsJson
        };
    }

    #endregion
}

#region DTOs

public class FormDefinitionSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameArabic { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public int SectionCount { get; set; }
    public int FieldCount { get; set; }
    public long SubmissionCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? Deadline { get; set; }
}

public class FormDefinitionDetailDto : FormDefinitionSummaryDto
{
    public string? DescriptionArabic { get; set; }
    public bool AllowSaveDraft { get; set; }
    public bool AllowMultipleSubmissions { get; set; }
    public bool ShowProgressIndicator { get; set; }
    public bool RequireAuthentication { get; set; }
    public int? MaxSubmissions { get; set; }
    public Guid? LinkedWorkflowDefinitionId { get; set; }
    public List<FormDefinitionSectionDto> Sections { get; set; } = new();
}

public class FormDefinitionSectionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TitleArabic { get; set; }
    public string? Description { get; set; }
    public int Order { get; set; }
    public bool IsCollapsible { get; set; }
    public bool IsCollapsedByDefault { get; set; }
    public int ColumnCount { get; set; }
    public List<FormDefinitionFieldDto> Fields { get; set; } = new();
}

public class FormDefinitionFieldDto
{
    public Guid Id { get; set; }
    public string FieldKey { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? LabelArabic { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public string FieldType { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsRequired { get; set; }
    public int ColumnSpan { get; set; }
    public bool IsHidden { get; set; }
    public bool IsReadOnly { get; set; }
    public string? DefaultValue { get; set; }
    public string? OptionsJson { get; set; }
}

public class CreateFormDefinitionRequest
{
    public string Name { get; set; } = string.Empty;
    public string? NameArabic { get; set; }
    public string? Description { get; set; }
    public string? DescriptionArabic { get; set; }
    public string? Type { get; set; }
    public bool? AllowSaveDraft { get; set; }
    public bool? AllowMultipleSubmissions { get; set; }
    public bool? ShowProgressIndicator { get; set; }
    public bool? RequireAuthentication { get; set; }
    public DateTime? Deadline { get; set; }
}

public class UpdateFormDefinitionRequest : CreateFormDefinitionRequest { }

public class AddSectionRequest
{
    public string Title { get; set; } = string.Empty;
    public string? TitleArabic { get; set; }
    public string? Description { get; set; }
    public string? DescriptionArabic { get; set; }
    public bool? IsCollapsible { get; set; }
    public bool? IsCollapsedByDefault { get; set; }
    public int? ColumnCount { get; set; }
}

public class AddFieldRequest
{
    public string FieldKey { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? LabelArabic { get; set; }
    public string? Placeholder { get; set; }
    public string FieldType { get; set; } = "Text";
    public bool? IsRequired { get; set; }
    public int? ColumnSpan { get; set; }
    public string? DefaultValue { get; set; }
    public string? OptionsJson { get; set; }
}

#endregion
