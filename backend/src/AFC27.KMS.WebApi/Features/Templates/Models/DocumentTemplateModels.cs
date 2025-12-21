using System;
using System.Collections.Generic;

namespace AFC27.KMS.WebApi.Features.Templates.Models;

/// <summary>
/// Document template entity
/// </summary>
public class DocumentTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public TemplateType Type { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string TemplateContent { get; set; } = string.Empty;
    public byte[]? TemplateFile { get; set; }
    public string? TemplateFilePath { get; set; }
    public List<TemplateVariable> Variables { get; set; } = new();
    public TemplateMetadata Metadata { get; set; } = new();
    public TemplateStatus Status { get; set; } = TemplateStatus.Draft;
    public int Version { get; set; } = 1;
    public Guid? ParentTemplateId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}

public enum TemplateType
{
    WordDocument,
    PdfForm,
    ExcelSpreadsheet,
    PowerPointPresentation,
    HtmlEmail,
    PlainText,
    Markdown
}

public enum TemplateStatus
{
    Draft,
    Active,
    Deprecated,
    Archived
}

/// <summary>
/// Template variable definition
/// </summary>
public class TemplateVariable
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public VariableType Type { get; set; } = VariableType.Text;
    public bool IsRequired { get; set; } = true;
    public string? DefaultValue { get; set; }
    public string? Format { get; set; }
    public string? ValidationPattern { get; set; }
    public List<string>? AllowedValues { get; set; }
    public string? DataSource { get; set; }
}

public enum VariableType
{
    Text,
    Number,
    Date,
    DateTime,
    Boolean,
    Currency,
    Email,
    Phone,
    Url,
    Image,
    Table,
    List,
    RichText,
    Signature,
    Barcode,
    QrCode
}

/// <summary>
/// Template metadata
/// </summary>
public class TemplateMetadata
{
    public string Language { get; set; } = "en";
    public string? Author { get; set; }
    public string? Department { get; set; }
    public List<string> Tags { get; set; } = new();
    public string? ComplianceLevel { get; set; }
    public bool RequiresApproval { get; set; }
    public List<string>? ApprovalRoles { get; set; }
    public int? ExpiryDays { get; set; }
    public bool IsConfidential { get; set; }
}

/// <summary>
/// Request to create a document template
/// </summary>
public class CreateTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public TemplateType Type { get; set; }
    public string? ContentType { get; set; }
    public string? TemplateContent { get; set; }
    public List<TemplateVariable>? Variables { get; set; }
    public TemplateMetadata? Metadata { get; set; }
}

/// <summary>
/// Request to generate a document from template
/// </summary>
public class GenerateDocumentRequest
{
    public Guid TemplateId { get; set; }
    public Dictionary<string, object> VariableValues { get; set; } = new();
    public string OutputFormat { get; set; } = "pdf";
    public string? OutputFileName { get; set; }
    public GenerationOptions Options { get; set; } = new();
}

/// <summary>
/// Document generation options
/// </summary>
public class GenerationOptions
{
    public bool IncludeWatermark { get; set; }
    public string? WatermarkText { get; set; }
    public bool IncludePageNumbers { get; set; } = true;
    public bool IncludeHeader { get; set; } = true;
    public bool IncludeFooter { get; set; } = true;
    public string? HeaderText { get; set; }
    public string? FooterText { get; set; }
    public bool ProtectDocument { get; set; }
    public string? DocumentPassword { get; set; }
    public bool AddDigitalSignaturePlaceholder { get; set; }
    public List<SignaturePlacement>? SignaturePlacements { get; set; }
}

/// <summary>
/// Signature placement in generated document
/// </summary>
public class SignaturePlacement
{
    public int PageNumber { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; } = 200;
    public int Height { get; set; } = 50;
    public string SignerRole { get; set; } = string.Empty;
}

/// <summary>
/// Response after generating a document
/// </summary>
public class GeneratedDocumentResponse
{
    public Guid DocumentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string? DownloadUrl { get; set; }
    public DateTime GeneratedAt { get; set; }
    public Guid TemplateId { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public int TemplateVersion { get; set; }
}

/// <summary>
/// Template summary for listings
/// </summary>
public class TemplateSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public TemplateType Type { get; set; }
    public TemplateStatus Status { get; set; }
    public int Version { get; set; }
    public int VariableCount { get; set; }
    public int UsageCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Template validation result
/// </summary>
public class TemplateValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> DetectedVariables { get; set; } = new();
}

/// <summary>
/// Template usage statistics
/// </summary>
public class TemplateUsageStats
{
    public Guid TemplateId { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public int TotalGenerations { get; set; }
    public int Last30DaysGenerations { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public List<GenerationByMonth> GenerationsByMonth { get; set; } = new();
}

public class GenerationByMonth
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Count { get; set; }
}
