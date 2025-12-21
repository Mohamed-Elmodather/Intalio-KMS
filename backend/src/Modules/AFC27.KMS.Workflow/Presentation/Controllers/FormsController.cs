using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Workflow.Application.DTOs;
using AFC27.KMS.Workflow.Domain.Entities;

namespace AFC27.KMS.Workflow.Presentation.Controllers;

/// <summary>
/// Controller for form builder and submissions
/// </summary>
[ApiController]
[Route("api/workflow/forms")]
[Authorize]
public class FormsController : ControllerBase
{
    #region Form Definitions

    /// <summary>
    /// Get all forms
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(typeof(IEnumerable<FormSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FormSummaryDto>>> GetForms([FromQuery] FormFilterDto filter)
    {
        // TODO: Return forms
        var forms = new List<FormSummaryDto>();
        return Ok(forms);
    }

    /// <summary>
    /// Get form by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FormDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FormDto>> GetForm(Guid id)
    {
        // TODO: Return form
        return NotFound();
    }

    /// <summary>
    /// Create a new form
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(typeof(FormDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<FormDto>> CreateForm([FromBody] CreateFormRequest request)
    {
        // TODO: Create form
        var form = new FormDto
        {
            Id = Guid.NewGuid(),
            Name = request.NameEn,
            Description = request.DescriptionEn,
            Type = request.Type,
            Status = FormStatus.Draft,
            Version = "1.0",
            AllowSaveDraft = request.AllowSaveDraft,
            ShowProgressBar = request.ShowProgressBar,
            Sections = new List<FormSectionDto>(),
            Fields = new List<FormFieldDto>(),
            CreatedAt = DateTime.UtcNow
        };

        return CreatedAtAction(nameof(GetForm), new { id = form.Id }, form);
    }

    /// <summary>
    /// Update a form
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(typeof(FormDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FormDto>> UpdateForm(Guid id, [FromBody] CreateFormRequest request)
    {
        // TODO: Update form
        return NotFound();
    }

    /// <summary>
    /// Add a section to form
    /// </summary>
    [HttpPost("{id:guid}/sections")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(typeof(FormSectionDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<FormSectionDto>> AddSection(
        Guid id,
        [FromBody] CreateFormSectionRequest request)
    {
        // TODO: Add section
        var section = new FormSectionDto
        {
            Id = Guid.NewGuid(),
            Title = request.TitleEn,
            Description = request.DescriptionEn,
            IsCollapsible = request.IsCollapsible,
            Order = 1
        };

        return CreatedAtAction(nameof(GetForm), new { id }, section);
    }

    /// <summary>
    /// Update a section
    /// </summary>
    [HttpPut("{id:guid}/sections/{sectionId:guid}")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(typeof(FormSectionDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<FormSectionDto>> UpdateSection(
        Guid id,
        Guid sectionId,
        [FromBody] CreateFormSectionRequest request)
    {
        // TODO: Update section
        return NotFound();
    }

    /// <summary>
    /// Delete a section
    /// </summary>
    [HttpDelete("{id:guid}/sections/{sectionId:guid}")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteSection(Guid id, Guid sectionId)
    {
        // TODO: Delete section
        return NoContent();
    }

    /// <summary>
    /// Add a field to form
    /// </summary>
    [HttpPost("{id:guid}/fields")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(typeof(FormFieldDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<FormFieldDto>> AddField(
        Guid id,
        [FromBody] CreateFormFieldRequest request)
    {
        // TODO: Add field
        var field = new FormFieldDto
        {
            Id = Guid.NewGuid(),
            SectionId = request.SectionId,
            FieldName = request.FieldName,
            Label = request.LabelEn,
            Placeholder = request.PlaceholderEn,
            HelpText = request.HelpTextEn,
            Type = request.Type,
            IsRequired = request.IsRequired,
            ColumnSpan = request.ColumnSpan,
            Order = 1
        };

        return CreatedAtAction(nameof(GetForm), new { id }, field);
    }

    /// <summary>
    /// Update a field
    /// </summary>
    [HttpPut("{id:guid}/fields/{fieldId:guid}")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(typeof(FormFieldDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<FormFieldDto>> UpdateField(
        Guid id,
        Guid fieldId,
        [FromBody] CreateFormFieldRequest request)
    {
        // TODO: Update field
        return NotFound();
    }

    /// <summary>
    /// Delete a field
    /// </summary>
    [HttpDelete("{id:guid}/fields/{fieldId:guid}")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteField(Guid id, Guid fieldId)
    {
        // TODO: Delete field
        return NoContent();
    }

    /// <summary>
    /// Reorder fields
    /// </summary>
    [HttpPost("{id:guid}/fields/reorder")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ReorderFields(Guid id, [FromBody] List<Guid> fieldIds)
    {
        // TODO: Reorder fields
        return NoContent();
    }

    /// <summary>
    /// Publish form
    /// </summary>
    [HttpPost("{id:guid}/publish")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> PublishForm(Guid id)
    {
        // TODO: Publish form
        return NoContent();
    }

    /// <summary>
    /// Unpublish form
    /// </summary>
    [HttpPost("{id:guid}/unpublish")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UnpublishForm(Guid id)
    {
        // TODO: Unpublish form
        return NoContent();
    }

    /// <summary>
    /// Clone a form
    /// </summary>
    [HttpPost("{id:guid}/clone")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(typeof(FormDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<FormDto>> CloneForm(Guid id, [FromQuery] string? newName = null)
    {
        // TODO: Clone form
        var form = new FormDto
        {
            Id = Guid.NewGuid(),
            Name = newName ?? "Copy of Form",
            Status = FormStatus.Draft,
            Version = "1.0",
            CreatedAt = DateTime.UtcNow
        };

        return CreatedAtAction(nameof(GetForm), new { id = form.Id }, form);
    }

    /// <summary>
    /// Delete form
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteForm(Guid id)
    {
        // TODO: Delete form
        return NoContent();
    }

    #endregion

    #region Form Submissions

    /// <summary>
    /// Submit a form
    /// </summary>
    [HttpPost("{id:guid}/submit")]
    [ProducesResponseType(typeof(FormSubmissionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FormSubmissionDto>> SubmitForm(
        Guid id,
        [FromBody] SubmitFormRequest request)
    {
        // TODO: Submit form
        var submission = new FormSubmissionDto
        {
            Id = Guid.NewGuid(),
            FormId = id,
            Status = request.SaveAsDraft ? SubmissionStatus.Draft : SubmissionStatus.Submitted,
            SubmittedAt = request.SaveAsDraft ? null : DateTime.UtcNow,
            Data = request.Data,
            CreatedAt = DateTime.UtcNow
        };

        return CreatedAtAction(nameof(GetSubmission), new { id, submissionId = submission.Id }, submission);
    }

    /// <summary>
    /// Get form submission
    /// </summary>
    [HttpGet("{id:guid}/submissions/{submissionId:guid}")]
    [ProducesResponseType(typeof(FormSubmissionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FormSubmissionDto>> GetSubmission(Guid id, Guid submissionId)
    {
        // TODO: Return submission
        return NotFound();
    }

    /// <summary>
    /// Get all submissions for a form
    /// </summary>
    [HttpGet("{id:guid}/submissions")]
    [Authorize(Policy = "CanManageWorkflow")]
    [ProducesResponseType(typeof(IEnumerable<FormSubmissionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FormSubmissionDto>>> GetSubmissions(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // TODO: Return submissions
        var submissions = new List<FormSubmissionDto>();
        return Ok(submissions);
    }

    /// <summary>
    /// Update draft submission
    /// </summary>
    [HttpPut("{id:guid}/submissions/{submissionId:guid}")]
    [ProducesResponseType(typeof(FormSubmissionDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<FormSubmissionDto>> UpdateSubmission(
        Guid id,
        Guid submissionId,
        [FromBody] SubmitFormRequest request)
    {
        // TODO: Update submission
        return NotFound();
    }

    /// <summary>
    /// Upload file for form field
    /// </summary>
    [HttpPost("{id:guid}/submissions/{submissionId:guid}/files")]
    [ProducesResponseType(typeof(FormAttachmentDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<FormAttachmentDto>> UploadFile(
        Guid id,
        Guid submissionId,
        [FromQuery] Guid fieldId,
        IFormFile file)
    {
        // TODO: Upload file
        var attachment = new FormAttachmentDto
        {
            Id = Guid.NewGuid(),
            FileName = file.FileName,
            FileSize = file.Length,
            MimeType = file.ContentType
        };

        return Created($"/api/workflow/forms/{id}/submissions/{submissionId}/files/{attachment.Id}", attachment);
    }

    #endregion

    #region Field Types

    /// <summary>
    /// Get available field types
    /// </summary>
    [HttpGet("field-types")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<FieldTypeInfoDto>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<FieldTypeInfoDto>> GetFieldTypes()
    {
        var types = new List<FieldTypeInfoDto>
        {
            new() { Type = FieldType.Text, Name = "Text", Category = "Input", Icon = "pi-pencil", SupportsValidation = true },
            new() { Type = FieldType.TextArea, Name = "Text Area", Category = "Input", Icon = "pi-align-left", SupportsValidation = true },
            new() { Type = FieldType.Email, Name = "Email", Category = "Input", Icon = "pi-envelope", SupportsValidation = true },
            new() { Type = FieldType.Phone, Name = "Phone", Category = "Input", Icon = "pi-phone", SupportsValidation = true },
            new() { Type = FieldType.Number, Name = "Number", Category = "Input", Icon = "pi-hashtag", SupportsValidation = true },
            new() { Type = FieldType.Currency, Name = "Currency", Category = "Input", Icon = "pi-dollar", SupportsValidation = true },
            new() { Type = FieldType.Date, Name = "Date", Category = "Date/Time", Icon = "pi-calendar" },
            new() { Type = FieldType.Time, Name = "Time", Category = "Date/Time", Icon = "pi-clock" },
            new() { Type = FieldType.DateTime, Name = "Date & Time", Category = "Date/Time", Icon = "pi-calendar-plus" },
            new() { Type = FieldType.Select, Name = "Dropdown", Category = "Selection", Icon = "pi-list", SupportsOptions = true },
            new() { Type = FieldType.MultiSelect, Name = "Multi-Select", Category = "Selection", Icon = "pi-check-square", SupportsOptions = true },
            new() { Type = FieldType.Radio, Name = "Radio Buttons", Category = "Selection", Icon = "pi-circle", SupportsOptions = true },
            new() { Type = FieldType.Checkbox, Name = "Checkboxes", Category = "Selection", Icon = "pi-check-square", SupportsOptions = true },
            new() { Type = FieldType.Toggle, Name = "Toggle", Category = "Selection", Icon = "pi-power-off" },
            new() { Type = FieldType.Lookup, Name = "Lookup", Category = "Selection", Icon = "pi-search", SupportsLookup = true },
            new() { Type = FieldType.UserLookup, Name = "User Lookup", Category = "Selection", Icon = "pi-user", SupportsLookup = true },
            new() { Type = FieldType.FileUpload, Name = "File Upload", Category = "Files", Icon = "pi-upload" },
            new() { Type = FieldType.ImageUpload, Name = "Image Upload", Category = "Files", Icon = "pi-image" },
            new() { Type = FieldType.Signature, Name = "Signature", Category = "Files", Icon = "pi-pencil" },
            new() { Type = FieldType.RichText, Name = "Rich Text", Category = "Content", Icon = "pi-file-edit" },
            new() { Type = FieldType.Rating, Name = "Rating", Category = "Input", Icon = "pi-star" },
            new() { Type = FieldType.Slider, Name = "Slider", Category = "Input", Icon = "pi-sliders-h" },
            new() { Type = FieldType.Heading, Name = "Heading", Category = "Layout", Icon = "pi-minus" },
            new() { Type = FieldType.Paragraph, Name = "Paragraph", Category = "Layout", Icon = "pi-align-justify" },
            new() { Type = FieldType.Divider, Name = "Divider", Category = "Layout", Icon = "pi-minus" },
            new() { Type = FieldType.Hidden, Name = "Hidden", Category = "Special", Icon = "pi-eye-slash" }
        };

        return Ok(types);
    }

    /// <summary>
    /// Get form types
    /// </summary>
    [HttpGet("types")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<object>> GetFormTypes()
    {
        var types = Enum.GetValues<FormType>()
            .Select(t => new { Value = (int)t, Name = t.ToString() });
        return Ok(types);
    }

    #endregion
}
