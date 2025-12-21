using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Workflow.Domain.Entities;

/// <summary>
/// Dynamic form definition
/// </summary>
public class Form : AuditableEntity
{
    public LocalizedString Name { get; private set; } = new();
    public LocalizedString? Description { get; private set; }
    public string Version { get; private set; } = "1.0";

    public FormStatus Status { get; private set; } = FormStatus.Draft;
    public FormType Type { get; private set; } = FormType.ServiceRequest;

    public List<FormSection> Sections { get; private set; } = new();
    public List<FormField> Fields { get; private set; } = new();

    // Settings
    public bool AllowSaveDraft { get; private set; } = true;
    public bool ShowProgressBar { get; private set; } = true;
    public bool RequireAllSections { get; private set; } = true;

    // Metadata
    public long SubmissionCount { get; private set; }

    private Form() { }

    public static Form Create(LocalizedString name, FormType type = FormType.ServiceRequest)
    {
        return new Form
        {
            Id = Guid.NewGuid(),
            Name = name,
            Type = type,
            Status = FormStatus.Draft
        };
    }

    public void Update(LocalizedString name, LocalizedString? description)
    {
        Name = name;
        Description = description;
    }

    public void AddSection(FormSection section)
    {
        section.SetOrder(Sections.Count + 1);
        Sections.Add(section);
    }

    public void AddField(FormField field)
    {
        field.SetOrder(Fields.Count + 1);
        Fields.Add(field);
    }

    public void RemoveSection(Guid sectionId)
    {
        var section = Sections.FirstOrDefault(s => s.Id == sectionId);
        if (section != null)
        {
            Sections.Remove(section);
            ReorderSections();
        }
    }

    public void RemoveField(Guid fieldId)
    {
        var field = Fields.FirstOrDefault(f => f.Id == fieldId);
        if (field != null)
        {
            Fields.Remove(field);
            ReorderFields();
        }
    }

    private void ReorderSections()
    {
        for (int i = 0; i < Sections.Count; i++)
        {
            Sections[i].SetOrder(i + 1);
        }
    }

    private void ReorderFields()
    {
        for (int i = 0; i < Fields.Count; i++)
        {
            Fields[i].SetOrder(i + 1);
        }
    }

    public void Publish()
    {
        Status = FormStatus.Published;
        Version = IncrementVersion();
    }

    public void Unpublish() => Status = FormStatus.Draft;
    public void Archive() => Status = FormStatus.Archived;

    public void IncrementSubmissionCount() => SubmissionCount++;

    private string IncrementVersion()
    {
        var parts = Version.Split('.');
        if (parts.Length == 2 && int.TryParse(parts[1], out int minor))
        {
            return $"{parts[0]}.{minor + 1}";
        }
        return "1.0";
    }
}

/// <summary>
/// Section within a form
/// </summary>
public class FormSection : Entity
{
    public Guid FormId { get; private set; }
    public LocalizedString Title { get; private set; } = new();
    public LocalizedString? Description { get; private set; }
    public int Order { get; private set; }
    public bool IsCollapsible { get; private set; }
    public bool IsCollapsedByDefault { get; private set; }
    public string? ConditionJson { get; private set; }

    public List<FormField> Fields { get; private set; } = new();

    private FormSection() { }

    public static FormSection Create(Guid formId, LocalizedString title)
    {
        return new FormSection
        {
            Id = Guid.NewGuid(),
            FormId = formId,
            Title = title
        };
    }

    public void Update(LocalizedString title, LocalizedString? description)
    {
        Title = title;
        Description = description;
    }

    public void SetOrder(int order) => Order = order;
    public void SetCollapsible(bool collapsible, bool collapsedByDefault = false)
    {
        IsCollapsible = collapsible;
        IsCollapsedByDefault = collapsedByDefault;
    }
    public void SetCondition(string? conditionJson) => ConditionJson = conditionJson;
}

/// <summary>
/// Field within a form
/// </summary>
public class FormField : Entity
{
    public Guid FormId { get; private set; }
    public Guid? SectionId { get; private set; }

    public string FieldName { get; private set; } = string.Empty;
    public LocalizedString Label { get; private set; } = new();
    public LocalizedString? Placeholder { get; private set; }
    public LocalizedString? HelpText { get; private set; }

    public FieldType Type { get; private set; }
    public int Order { get; private set; }

    // Validation
    public bool IsRequired { get; private set; }
    public int? MinLength { get; private set; }
    public int? MaxLength { get; private set; }
    public string? Pattern { get; private set; }
    public string? PatternMessage { get; private set; }
    public decimal? MinValue { get; private set; }
    public decimal? MaxValue { get; private set; }

    // Options (for select, radio, checkbox)
    public List<FieldOption> Options { get; private set; } = new();

    // Default value
    public string? DefaultValue { get; private set; }

    // Layout
    public int ColumnSpan { get; private set; } = 12; // 1-12 grid
    public bool IsHidden { get; private set; }
    public bool IsReadOnly { get; private set; }

    // Conditional visibility
    public string? ConditionJson { get; private set; }

    // Lookup configuration
    public string? LookupEndpoint { get; private set; }
    public string? LookupDisplayField { get; private set; }
    public string? LookupValueField { get; private set; }

    // File upload configuration
    public List<string> AllowedFileTypes { get; private set; } = new();
    public long? MaxFileSize { get; private set; }
    public int? MaxFiles { get; private set; }

    private FormField() { }

    public static FormField Create(
        Guid formId,
        string fieldName,
        LocalizedString label,
        FieldType type)
    {
        return new FormField
        {
            Id = Guid.NewGuid(),
            FormId = formId,
            FieldName = fieldName,
            Label = label,
            Type = type
        };
    }

    public void Update(LocalizedString label, LocalizedString? placeholder, LocalizedString? helpText)
    {
        Label = label;
        Placeholder = placeholder;
        HelpText = helpText;
    }

    public void SetOrder(int order) => Order = order;
    public void SetSection(Guid? sectionId) => SectionId = sectionId;
    public void SetRequired(bool required) => IsRequired = required;
    public void SetColumnSpan(int span) => ColumnSpan = Math.Clamp(span, 1, 12);
    public void SetHidden(bool hidden) => IsHidden = hidden;
    public void SetReadOnly(bool readOnly) => IsReadOnly = readOnly;
    public void SetDefaultValue(string? value) => DefaultValue = value;
    public void SetCondition(string? conditionJson) => ConditionJson = conditionJson;

    public void SetValidation(
        int? minLength = null,
        int? maxLength = null,
        string? pattern = null,
        string? patternMessage = null,
        decimal? minValue = null,
        decimal? maxValue = null)
    {
        MinLength = minLength;
        MaxLength = maxLength;
        Pattern = pattern;
        PatternMessage = patternMessage;
        MinValue = minValue;
        MaxValue = maxValue;
    }

    public void SetOptions(List<FieldOption> options) => Options = options;

    public void SetLookup(string endpoint, string displayField, string valueField)
    {
        LookupEndpoint = endpoint;
        LookupDisplayField = displayField;
        LookupValueField = valueField;
    }

    public void SetFileUpload(List<string> allowedTypes, long? maxSize, int? maxFiles)
    {
        AllowedFileTypes = allowedTypes;
        MaxFileSize = maxSize;
        MaxFiles = maxFiles;
    }
}

/// <summary>
/// Option for select/radio/checkbox fields
/// </summary>
public class FieldOption
{
    public string Value { get; set; } = string.Empty;
    public LocalizedString Label { get; set; } = new();
    public bool IsDefault { get; set; }
    public int Order { get; set; }
    public bool IsDisabled { get; set; }
}

/// <summary>
/// Submitted form data
/// </summary>
public class FormSubmission : AuditableEntity
{
    public Guid FormId { get; private set; }
    public Form Form { get; private set; } = null!;
    public string FormVersion { get; private set; } = string.Empty;

    public Guid SubmitterId { get; private set; }
    public string SubmitterName { get; private set; } = string.Empty;

    public SubmissionStatus Status { get; private set; } = SubmissionStatus.Draft;
    public DateTime? SubmittedAt { get; private set; }

    // Form data as JSON
    public string DataJson { get; private set; } = "{}";

    // Parsed field values
    public List<SubmittedFieldValue> FieldValues { get; private set; } = new();

    // Attachments
    public List<FormAttachment> Attachments { get; private set; } = new();

    private FormSubmission() { }

    public static FormSubmission Create(
        Guid formId,
        string formVersion,
        Guid submitterId,
        string submitterName)
    {
        return new FormSubmission
        {
            Id = Guid.NewGuid(),
            FormId = formId,
            FormVersion = formVersion,
            SubmitterId = submitterId,
            SubmitterName = submitterName,
            Status = SubmissionStatus.Draft
        };
    }

    public void SetData(string dataJson)
    {
        DataJson = dataJson;
    }

    public void AddFieldValue(SubmittedFieldValue value)
    {
        FieldValues.Add(value);
    }

    public void SaveDraft()
    {
        Status = SubmissionStatus.Draft;
    }

    public void Submit()
    {
        Status = SubmissionStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
    }

    public void AddAttachment(FormAttachment attachment)
    {
        Attachments.Add(attachment);
    }
}

/// <summary>
/// Individual field value in a submission
/// </summary>
public class SubmittedFieldValue
{
    public Guid FieldId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string? Value { get; set; }
    public List<string> Values { get; set; } = new(); // For multi-select
}

/// <summary>
/// File attachment in a form submission
/// </summary>
public class FormAttachment : Entity
{
    public Guid FormSubmissionId { get; private set; }
    public Guid FieldId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public string MimeType { get; private set; } = string.Empty;
    public DateTime UploadedAt { get; private set; }

    private FormAttachment() { }

    public static FormAttachment Create(
        Guid submissionId,
        Guid fieldId,
        string fileName,
        string filePath,
        long fileSize,
        string mimeType)
    {
        return new FormAttachment
        {
            Id = Guid.NewGuid(),
            FormSubmissionId = submissionId,
            FieldId = fieldId,
            FileName = fileName,
            FilePath = filePath,
            FileSize = fileSize,
            MimeType = mimeType,
            UploadedAt = DateTime.UtcNow
        };
    }
}

public enum FormStatus
{
    Draft,
    Published,
    Archived
}

public enum FormType
{
    ServiceRequest,
    Approval,
    Survey,
    Feedback,
    Registration,
    Custom
}

public enum SubmissionStatus
{
    Draft,
    Submitted,
    Processing,
    Completed
}

public enum FieldType
{
    // Text inputs
    Text,
    TextArea,
    Email,
    Phone,
    Url,
    Password,

    // Numbers
    Number,
    Currency,
    Percentage,

    // Date/Time
    Date,
    Time,
    DateTime,
    DateRange,

    // Selection
    Select,
    MultiSelect,
    Radio,
    Checkbox,
    Toggle,

    // Lookup
    Lookup,
    UserLookup,
    DepartmentLookup,

    // Files
    FileUpload,
    ImageUpload,
    Signature,

    // Rich content
    RichText,
    Markdown,

    // Layout
    Heading,
    Paragraph,
    Divider,
    Spacer,

    // Special
    Rating,
    Slider,
    ColorPicker,
    Location,
    Hidden
}
