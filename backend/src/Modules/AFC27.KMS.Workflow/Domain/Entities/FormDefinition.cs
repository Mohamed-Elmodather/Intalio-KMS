using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Workflow.Domain.Entities;

/// <summary>
/// A configurable form definition with fields, validation rules, and sections (Phase 10).
/// Extends the existing Form concept with richer builder capabilities including
/// conditional logic, multi-page layouts, and submission routing.
/// </summary>
public class FormDefinition : AuditableEntity
{
    public LocalizedString Name { get; private set; } = new();
    public LocalizedString? Description { get; private set; }
    public string Version { get; private set; } = "1.0";

    public FormDefinitionStatus Status { get; private set; } = FormDefinitionStatus.Draft;
    public FormDefinitionType Type { get; private set; } = FormDefinitionType.Standard;

    /// <summary>
    /// Ordered list of form sections (pages/groups).
    /// </summary>
    public List<FormDefinitionSection> Sections { get; private set; } = new();

    /// <summary>
    /// Global validation rules applied across fields (cross-field validation).
    /// Stored as JSON array of rule objects.
    /// </summary>
    public string? GlobalValidationRulesJson { get; private set; }

    /// <summary>
    /// Form-level settings.
    /// </summary>
    public bool AllowSaveDraft { get; private set; } = true;
    public bool AllowMultipleSubmissions { get; private set; } = false;
    public bool ShowProgressIndicator { get; private set; } = true;
    public bool RequireAuthentication { get; private set; } = true;

    /// <summary>
    /// Submission behaviour: where to route completed submissions.
    /// </summary>
    public string? SubmissionRoutingRuleJson { get; private set; }

    /// <summary>
    /// Optional workflow definition to trigger upon submission.
    /// </summary>
    public Guid? LinkedWorkflowDefinitionId { get; private set; }

    /// <summary>
    /// Custom CSS or theme identifier.
    /// </summary>
    public string? ThemeId { get; private set; }

    /// <summary>
    /// Confirmation/thank-you message shown after submission.
    /// </summary>
    public LocalizedString? ConfirmationMessage { get; private set; }

    /// <summary>
    /// Maximum submissions allowed (null = unlimited).
    /// </summary>
    public int? MaxSubmissions { get; private set; }

    /// <summary>
    /// Submission deadline (null = no deadline).
    /// </summary>
    public DateTime? Deadline { get; private set; }

    /// <summary>
    /// Running count of submissions received.
    /// </summary>
    public long SubmissionCount { get; private set; }

    private FormDefinition() { }

    public static FormDefinition Create(
        LocalizedString name,
        FormDefinitionType type = FormDefinitionType.Standard)
    {
        return new FormDefinition
        {
            Id = Guid.NewGuid(),
            Name = name,
            Type = type,
            Status = FormDefinitionStatus.Draft
        };
    }

    public void Update(LocalizedString name, LocalizedString? description)
    {
        Name = name;
        Description = description;
    }

    public void SetSettings(
        bool allowSaveDraft,
        bool allowMultipleSubmissions,
        bool showProgressIndicator,
        bool requireAuthentication)
    {
        AllowSaveDraft = allowSaveDraft;
        AllowMultipleSubmissions = allowMultipleSubmissions;
        ShowProgressIndicator = showProgressIndicator;
        RequireAuthentication = requireAuthentication;
    }

    public void SetDeadline(DateTime? deadline) => Deadline = deadline;
    public void SetMaxSubmissions(int? max) => MaxSubmissions = max;
    public void SetConfirmationMessage(LocalizedString? message) => ConfirmationMessage = message;
    public void SetTheme(string? themeId) => ThemeId = themeId;
    public void SetGlobalValidationRules(string? rulesJson) => GlobalValidationRulesJson = rulesJson;
    public void SetSubmissionRouting(string? routingJson) => SubmissionRoutingRuleJson = routingJson;
    public void LinkWorkflow(Guid? workflowDefinitionId) => LinkedWorkflowDefinitionId = workflowDefinitionId;

    public FormDefinitionSection AddSection(LocalizedString title, LocalizedString? description = null)
    {
        var section = FormDefinitionSection.Create(Id, title, description, Sections.Count + 1);
        Sections.Add(section);
        return section;
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

    public void ReorderSections()
    {
        for (int i = 0; i < Sections.Count; i++)
            Sections[i].SetOrder(i + 1);
    }

    public void Publish()
    {
        Status = FormDefinitionStatus.Published;
        Version = IncrementVersion();
    }

    public void Unpublish() => Status = FormDefinitionStatus.Draft;
    public void Archive() => Status = FormDefinitionStatus.Archived;
    public void IncrementSubmissionCount() => SubmissionCount++;

    private string IncrementVersion()
    {
        var parts = Version.Split('.');
        if (parts.Length == 2 && int.TryParse(parts[1], out int minor))
            return $"{parts[0]}.{minor + 1}";
        return "1.0";
    }
}

/// <summary>
/// A section (page/group) within a FormDefinition.
/// </summary>
public class FormDefinitionSection : Entity
{
    public Guid FormDefinitionId { get; private set; }
    public LocalizedString Title { get; private set; } = new();
    public LocalizedString? Description { get; private set; }
    public int Order { get; private set; }

    /// <summary>
    /// Whether this section can be collapsed in the UI.
    /// </summary>
    public bool IsCollapsible { get; private set; }
    public bool IsCollapsedByDefault { get; private set; }

    /// <summary>
    /// Conditional visibility rule (JSON). Section is shown only when the condition evaluates to true.
    /// Example: {"field":"priority","operator":"equals","value":"High"}
    /// </summary>
    public string? VisibilityConditionJson { get; private set; }

    /// <summary>
    /// Layout columns for the section (1, 2, or 3).
    /// </summary>
    public int ColumnCount { get; private set; } = 1;

    /// <summary>
    /// Fields within this section.
    /// </summary>
    public List<FormDefinitionField> Fields { get; private set; } = new();

    private FormDefinitionSection() { }

    public static FormDefinitionSection Create(
        Guid formDefinitionId,
        LocalizedString title,
        LocalizedString? description,
        int order)
    {
        return new FormDefinitionSection
        {
            Id = Guid.NewGuid(),
            FormDefinitionId = formDefinitionId,
            Title = title,
            Description = description,
            Order = order
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
    public void SetVisibilityCondition(string? conditionJson) => VisibilityConditionJson = conditionJson;
    public void SetColumnCount(int columns) => ColumnCount = Math.Clamp(columns, 1, 3);

    public FormDefinitionField AddField(
        string fieldKey,
        LocalizedString label,
        FormDefinitionFieldType fieldType)
    {
        var field = FormDefinitionField.Create(FormDefinitionId, Id, fieldKey, label, fieldType, Fields.Count + 1);
        Fields.Add(field);
        return field;
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

    public void ReorderFields()
    {
        for (int i = 0; i < Fields.Count; i++)
            Fields[i].SetOrder(i + 1);
    }
}

/// <summary>
/// A field definition within a FormDefinitionSection.
/// </summary>
public class FormDefinitionField : Entity
{
    public Guid FormDefinitionId { get; private set; }
    public Guid SectionId { get; private set; }

    /// <summary>
    /// Unique key for this field within the form (used in submission data).
    /// </summary>
    public string FieldKey { get; private set; } = string.Empty;

    public LocalizedString Label { get; private set; } = new();
    public LocalizedString? Placeholder { get; private set; }
    public LocalizedString? HelpText { get; private set; }

    public FormDefinitionFieldType FieldType { get; private set; }
    public int Order { get; private set; }

    // Validation rules
    public bool IsRequired { get; private set; }
    public int? MinLength { get; private set; }
    public int? MaxLength { get; private set; }
    public string? ValidationRegex { get; private set; }
    public string? ValidationMessage { get; private set; }
    public decimal? MinValue { get; private set; }
    public decimal? MaxValue { get; private set; }

    /// <summary>
    /// Custom validation rules as JSON array.
    /// Example: [{"type":"email"},{"type":"custom","expression":"value.length > 5","message":"Too short"}]
    /// </summary>
    public string? CustomValidationRulesJson { get; private set; }

    // Options (for select, radio, checkbox)
    public string? OptionsJson { get; private set; }

    // Default value
    public string? DefaultValue { get; private set; }

    // Layout
    public int ColumnSpan { get; private set; } = 12;
    public bool IsHidden { get; private set; }
    public bool IsReadOnly { get; private set; }
    public bool IsDisabled { get; private set; }

    // Conditional visibility
    public string? VisibilityConditionJson { get; private set; }

    // Data source for dynamic options (e.g., API endpoint)
    public string? DataSourceUrl { get; private set; }
    public string? DataSourceDisplayField { get; private set; }
    public string? DataSourceValueField { get; private set; }

    // File constraints
    public string? AllowedFileTypesJson { get; private set; }
    public long? MaxFileSizeBytes { get; private set; }
    public int? MaxFileCount { get; private set; }

    private FormDefinitionField() { }

    public static FormDefinitionField Create(
        Guid formDefinitionId,
        Guid sectionId,
        string fieldKey,
        LocalizedString label,
        FormDefinitionFieldType fieldType,
        int order)
    {
        return new FormDefinitionField
        {
            Id = Guid.NewGuid(),
            FormDefinitionId = formDefinitionId,
            SectionId = sectionId,
            FieldKey = fieldKey,
            Label = label,
            FieldType = fieldType,
            Order = order
        };
    }

    public void Update(LocalizedString label, LocalizedString? placeholder, LocalizedString? helpText)
    {
        Label = label;
        Placeholder = placeholder;
        HelpText = helpText;
    }

    public void SetOrder(int order) => Order = order;
    public void SetRequired(bool required) => IsRequired = required;
    public void SetColumnSpan(int span) => ColumnSpan = Math.Clamp(span, 1, 12);
    public void SetHidden(bool hidden) => IsHidden = hidden;
    public void SetReadOnly(bool readOnly) => IsReadOnly = readOnly;
    public void SetDisabled(bool disabled) => IsDisabled = disabled;
    public void SetDefaultValue(string? value) => DefaultValue = value;
    public void SetVisibilityCondition(string? conditionJson) => VisibilityConditionJson = conditionJson;
    public void SetOptions(string? optionsJson) => OptionsJson = optionsJson;
    public void SetCustomValidationRules(string? rulesJson) => CustomValidationRulesJson = rulesJson;

    public void SetValidation(
        int? minLength = null,
        int? maxLength = null,
        string? regex = null,
        string? validationMessage = null,
        decimal? minValue = null,
        decimal? maxValue = null)
    {
        MinLength = minLength;
        MaxLength = maxLength;
        ValidationRegex = regex;
        ValidationMessage = validationMessage;
        MinValue = minValue;
        MaxValue = maxValue;
    }

    public void SetDataSource(string url, string displayField, string valueField)
    {
        DataSourceUrl = url;
        DataSourceDisplayField = displayField;
        DataSourceValueField = valueField;
    }

    public void SetFileConstraints(string? allowedTypesJson, long? maxSize, int? maxCount)
    {
        AllowedFileTypesJson = allowedTypesJson;
        MaxFileSizeBytes = maxSize;
        MaxFileCount = maxCount;
    }
}

public enum FormDefinitionStatus
{
    Draft = 0,
    Published = 1,
    Archived = 2,
    Deprecated = 3
}

public enum FormDefinitionType
{
    Standard = 0,
    Wizard = 1,
    Survey = 2,
    Feedback = 3,
    Registration = 4,
    ServiceRequest = 5,
    Approval = 6,
    Custom = 99
}

public enum FormDefinitionFieldType
{
    // Text
    Text = 0,
    TextArea = 1,
    Email = 2,
    Phone = 3,
    Url = 4,
    Password = 5,
    RichText = 6,

    // Numbers
    Number = 10,
    Currency = 11,
    Percentage = 12,
    Slider = 13,

    // Date/Time
    Date = 20,
    Time = 21,
    DateTime = 22,
    DateRange = 23,

    // Selection
    Select = 30,
    MultiSelect = 31,
    Radio = 32,
    Checkbox = 33,
    Toggle = 34,
    Rating = 35,

    // Lookup
    Lookup = 40,
    UserLookup = 41,
    DepartmentLookup = 42,

    // Files
    FileUpload = 50,
    ImageUpload = 51,
    Signature = 52,

    // Layout
    Heading = 60,
    Paragraph = 61,
    Divider = 62,
    Spacer = 63,

    // Special
    ColorPicker = 70,
    Location = 71,
    Hidden = 72,
    Calculated = 73,
    QRCode = 74
}
