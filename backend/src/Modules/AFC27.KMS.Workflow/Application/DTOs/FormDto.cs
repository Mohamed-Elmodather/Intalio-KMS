using AFC27.KMS.Workflow.Domain.Entities;

namespace AFC27.KMS.Workflow.Application.DTOs;

/// <summary>
/// Form DTO
/// </summary>
public record FormDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Version { get; init; } = string.Empty;
    public FormStatus Status { get; init; }
    public FormType Type { get; init; }
    public bool AllowSaveDraft { get; init; }
    public bool ShowProgressBar { get; init; }
    public List<FormSectionDto> Sections { get; init; } = new();
    public List<FormFieldDto> Fields { get; init; } = new();
    public long SubmissionCount { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Form summary
/// </summary>
public record FormSummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public FormStatus Status { get; init; }
    public FormType Type { get; init; }
    public int FieldCount { get; init; }
    public long SubmissionCount { get; init; }
}

/// <summary>
/// Form section DTO
/// </summary>
public record FormSectionDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Order { get; init; }
    public bool IsCollapsible { get; init; }
    public bool IsCollapsedByDefault { get; init; }
    public List<FormFieldDto> Fields { get; init; } = new();
}

/// <summary>
/// Form field DTO
/// </summary>
public record FormFieldDto
{
    public Guid Id { get; init; }
    public Guid? SectionId { get; init; }
    public string FieldName { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string? Placeholder { get; init; }
    public string? HelpText { get; init; }
    public FieldType Type { get; init; }
    public int Order { get; init; }
    public bool IsRequired { get; init; }
    public int? MinLength { get; init; }
    public int? MaxLength { get; init; }
    public string? Pattern { get; init; }
    public string? PatternMessage { get; init; }
    public decimal? MinValue { get; init; }
    public decimal? MaxValue { get; init; }
    public List<FieldOptionDto> Options { get; init; } = new();
    public string? DefaultValue { get; init; }
    public int ColumnSpan { get; init; }
    public bool IsHidden { get; init; }
    public bool IsReadOnly { get; init; }
    public string? LookupEndpoint { get; init; }
    public List<string> AllowedFileTypes { get; init; } = new();
    public long? MaxFileSize { get; init; }
    public int? MaxFiles { get; init; }
}

/// <summary>
/// Field option DTO
/// </summary>
public record FieldOptionDto
{
    public string Value { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public bool IsDefault { get; init; }
    public int Order { get; init; }
}

/// <summary>
/// Create form request
/// </summary>
public record CreateFormRequest
{
    public string NameEn { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string? DescriptionEn { get; init; }
    public string? DescriptionAr { get; init; }
    public FormType Type { get; init; } = FormType.ServiceRequest;
    public bool AllowSaveDraft { get; init; } = true;
    public bool ShowProgressBar { get; init; } = true;
}

/// <summary>
/// Create form section request
/// </summary>
public record CreateFormSectionRequest
{
    public string TitleEn { get; init; } = string.Empty;
    public string TitleAr { get; init; } = string.Empty;
    public string? DescriptionEn { get; init; }
    public string? DescriptionAr { get; init; }
    public bool IsCollapsible { get; init; }
    public bool IsCollapsedByDefault { get; init; }
}

/// <summary>
/// Create form field request
/// </summary>
public record CreateFormFieldRequest
{
    public Guid? SectionId { get; init; }
    public string FieldName { get; init; } = string.Empty;
    public string LabelEn { get; init; } = string.Empty;
    public string LabelAr { get; init; } = string.Empty;
    public string? PlaceholderEn { get; init; }
    public string? PlaceholderAr { get; init; }
    public string? HelpTextEn { get; init; }
    public string? HelpTextAr { get; init; }
    public FieldType Type { get; init; }
    public bool IsRequired { get; init; }
    public int? MinLength { get; init; }
    public int? MaxLength { get; init; }
    public string? Pattern { get; init; }
    public string? PatternMessage { get; init; }
    public decimal? MinValue { get; init; }
    public decimal? MaxValue { get; init; }
    public List<CreateFieldOptionRequest> Options { get; init; } = new();
    public string? DefaultValue { get; init; }
    public int ColumnSpan { get; init; } = 12;
    public bool IsHidden { get; init; }
    public bool IsReadOnly { get; init; }
    public string? LookupEndpoint { get; init; }
    public string? LookupDisplayField { get; init; }
    public string? LookupValueField { get; init; }
    public List<string> AllowedFileTypes { get; init; } = new();
    public long? MaxFileSize { get; init; }
    public int? MaxFiles { get; init; }
}

/// <summary>
/// Create field option request
/// </summary>
public record CreateFieldOptionRequest
{
    public string Value { get; init; } = string.Empty;
    public string LabelEn { get; init; } = string.Empty;
    public string LabelAr { get; init; } = string.Empty;
    public bool IsDefault { get; init; }
}

/// <summary>
/// Form submission DTO
/// </summary>
public record FormSubmissionDto
{
    public Guid Id { get; init; }
    public Guid FormId { get; init; }
    public string FormName { get; init; } = string.Empty;
    public string FormVersion { get; init; } = string.Empty;
    public Guid SubmitterId { get; init; }
    public string SubmitterName { get; init; } = string.Empty;
    public SubmissionStatus Status { get; init; }
    public DateTime? SubmittedAt { get; init; }
    public Dictionary<string, object?> Data { get; init; } = new();
    public List<FormAttachmentDto> Attachments { get; init; } = new();
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Form attachment DTO
/// </summary>
public record FormAttachmentDto
{
    public Guid Id { get; init; }
    public string FieldName { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public string MimeType { get; init; } = string.Empty;
    public string DownloadUrl { get; init; } = string.Empty;
}

/// <summary>
/// Submit form request
/// </summary>
public record SubmitFormRequest
{
    public Guid FormId { get; init; }
    public Dictionary<string, object?> Data { get; init; } = new();
    public bool SaveAsDraft { get; init; }
}

/// <summary>
/// Form filter
/// </summary>
public record FormFilterDto
{
    public string? Search { get; init; }
    public FormStatus? Status { get; init; }
    public FormType? Type { get; init; }
    public string SortBy { get; init; } = "name";
    public bool SortDescending { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

/// <summary>
/// Available field types
/// </summary>
public record FieldTypeInfoDto
{
    public FieldType Type { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Icon { get; init; } = string.Empty;
    public bool SupportsOptions { get; init; }
    public bool SupportsValidation { get; init; }
    public bool SupportsLookup { get; init; }
}
