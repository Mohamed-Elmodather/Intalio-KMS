namespace AFC27.KMS.Documents.Application.DTOs;

/// <summary>
/// Document response DTO.
/// </summary>
public record DocumentDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string FileExtension { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public string? ThumbnailUrl { get; init; }
    public string? StoragePath { get; init; }
    public Guid LibraryId { get; init; }
    public string LibraryName { get; init; } = string.Empty;
    public Guid? FolderId { get; init; }
    public string? FolderPath { get; init; }
    public string Version { get; init; } = "1.0";
    public string Status { get; init; } = string.Empty;
    public Guid? CheckedOutById { get; init; }
    public string? CheckedOutByName { get; init; }
    public DateTime? CheckedOutAt { get; init; }
    public bool RequiresApproval { get; init; }
    public int DownloadCount { get; init; }
    public int ViewCount { get; init; }
    public Guid CreatedById { get; init; }
    public string CreatedByName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public IReadOnlyList<DocumentMetadataDto> Metadata { get; init; } = Array.Empty<DocumentMetadataDto>();
}

/// <summary>
/// Document summary for lists.
/// </summary>
public record DocumentSummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string FileExtension { get; init; } = string.Empty;
    public string Extension { get; init; } = string.Empty;
    public string? MimeType { get; init; }
    public long FileSize { get; init; }
    public string? ThumbnailUrl { get; init; }
    public string Version { get; init; } = "1.0";
    public string Status { get; init; } = string.Empty;
    public bool IsCheckedOut { get; init; }
    public string? CheckedOutByName { get; init; }
    public string CreatedByName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
}

/// <summary>
/// Document metadata DTO.
/// </summary>
public record DocumentMetadataDto
{
    public string Key { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string? ValueArabic { get; init; }
    public string DataType { get; init; } = "String";
}

/// <summary>
/// Document version DTO.
/// </summary>
public record DocumentVersionDto
{
    public Guid Id { get; init; }
    public string Version { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public string CreatedByName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string? ChangeNotes { get; init; }
}

/// <summary>
/// Document audit entry DTO.
/// </summary>
public record DocumentAuditDto
{
    public Guid Id { get; init; }
    public string Action { get; init; } = string.Empty;
    public string? Details { get; init; }
    public string PerformedByName { get; init; } = string.Empty;
    public DateTime PerformedAt { get; init; }
}

/// <summary>
/// Upload document request.
/// </summary>
public record UploadDocumentRequest
{
    public Guid LibraryId { get; init; }
    public Guid? FolderId { get; init; }
    public string? Name { get; init; }
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
    public IReadOnlyList<string>? Tags { get; init; }
}

/// <summary>
/// Update document request.
/// </summary>
public record UpdateDocumentRequest
{
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
    public IReadOnlyList<string>? Tags { get; init; }
}

/// <summary>
/// Check in document request.
/// </summary>
public record CheckInDocumentRequest
{
    public bool IsMajorVersion { get; init; }
    public string? ChangeNotes { get; init; }
    public string? Comments { get; init; }
}

/// <summary>
/// Move document request.
/// </summary>
public record MoveDocumentRequest
{
    public Guid? TargetFolderId { get; init; }
    public Guid? TargetLibraryId { get; init; }
}

/// <summary>
/// Document filter request.
/// </summary>
public record DocumentFilterRequest
{
    public string? Search { get; init; }
    public Guid? LibraryId { get; init; }
    public Guid? FolderId { get; init; }
    public string? FileExtension { get; init; }
    public string? Status { get; init; }
    public Guid? CreatedById { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string SortBy { get; init; } = "UpdatedAt";
    public bool SortDescending { get; init; } = true;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
