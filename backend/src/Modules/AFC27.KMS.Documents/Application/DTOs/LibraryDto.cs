namespace AFC27.KMS.Documents.Application.DTOs;

/// <summary>
/// Document library response DTO.
/// </summary>
public record LibraryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string? IconName { get; init; }
    public string? Color { get; init; }
    public string Type { get; init; } = string.Empty;
    public bool IsPublic { get; init; }
    public bool RequiresApproval { get; init; }
    public bool EnableVersioning { get; init; }
    public int? MaxVersions { get; init; }
    public long? MaxFileSize { get; init; }
    public string? AllowedExtensions { get; init; }
    public Guid OwnerId { get; init; }
    public string OwnerName { get; init; } = string.Empty;
    public int DocumentCount { get; init; }
    public int FolderCount { get; init; }
    public long TotalSize { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Library summary for lists.
/// </summary>
public record LibrarySummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? IconName { get; init; }
    public string? Color { get; init; }
    public string Type { get; init; } = string.Empty;
    public bool IsPublic { get; init; }
    public int DocumentCount { get; init; }
    public long TotalSize { get; init; }
}

/// <summary>
/// Create library request.
/// </summary>
public record CreateLibraryRequest
{
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string Type { get; init; } = "General";
    public bool IsPublic { get; init; }
    public bool RequiresApproval { get; init; }
    public bool EnableVersioning { get; init; } = true;
    public int? MaxVersions { get; init; }
    public long? MaxFileSize { get; init; }
    public string? AllowedExtensions { get; init; }
}

/// <summary>
/// Update library request.
/// </summary>
public record UpdateLibraryRequest
{
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public bool IsPublic { get; init; }
    public bool RequiresApproval { get; init; }
    public bool EnableVersioning { get; init; }
    public int? MaxVersions { get; init; }
    public long? MaxFileSize { get; init; }
    public string? AllowedExtensions { get; init; }
}

/// <summary>
/// Folder response DTO.
/// </summary>
public record FolderDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public Guid LibraryId { get; init; }
    public Guid? ParentId { get; init; }
    public string Path { get; init; } = string.Empty;
    public string? IconName { get; init; }
    public string? Color { get; init; }
    public int DocumentCount { get; init; }
    public int ChildFolderCount { get; init; }
    public IReadOnlyList<FolderSummaryDto> Children { get; init; } = Array.Empty<FolderSummaryDto>();
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Folder summary for lists.
/// </summary>
public record FolderSummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? IconName { get; init; }
    public string? Color { get; init; }
    public int DocumentCount { get; init; }
    public int ChildFolderCount { get; init; }
}

/// <summary>
/// Create folder request.
/// </summary>
public record CreateFolderRequest
{
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public Guid LibraryId { get; init; }
    public Guid? ParentId { get; init; }
}

/// <summary>
/// Update folder request.
/// </summary>
public record UpdateFolderRequest
{
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
}

/// <summary>
/// Library contents response (folders and documents).
/// </summary>
public record LibraryContentsDto
{
    public LibrarySummaryDto Library { get; init; } = null!;
    public FolderDto? CurrentFolder { get; init; }
    public IReadOnlyList<FolderSummaryDto> Folders { get; init; } = Array.Empty<FolderSummaryDto>();
    public IReadOnlyList<DocumentSummaryDto> Documents { get; init; } = Array.Empty<DocumentSummaryDto>();
    public IReadOnlyList<BreadcrumbDto> Breadcrumbs { get; init; } = Array.Empty<BreadcrumbDto>();
}

/// <summary>
/// Breadcrumb for navigation.
/// </summary>
public record BreadcrumbDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string Type { get; init; } = string.Empty; // "library" or "folder"
}

/// <summary>
/// Folder tree structure for navigation.
/// </summary>
public record FolderTreeDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public Guid? ParentId { get; init; }
    public string? IconName { get; init; }
    public int DocumentCount { get; init; }
    public IReadOnlyList<FolderTreeDto> Children { get; init; } = Array.Empty<FolderTreeDto>();
}

/// <summary>
/// Library statistics.
/// </summary>
public record LibraryStatsDto
{
    public Guid LibraryId { get; init; }
    public int TotalDocuments { get; init; }
    public int TotalFolders { get; init; }
    public long TotalSizeBytes { get; init; }
    public int PublishedDocuments { get; init; }
    public int DraftDocuments { get; init; }
    public int CheckedOutDocuments { get; init; }
    public int UniqueContributors { get; init; }
    public DateTime? LastActivityAt { get; init; }
    public Dictionary<string, int> DocumentsByExtension { get; init; } = new();
}

/// <summary>
/// Folder contents response.
/// </summary>
public record FolderContentsDto
{
    public FolderDto Folder { get; init; } = null!;
    public IReadOnlyList<FolderSummaryDto> Subfolders { get; init; } = Array.Empty<FolderSummaryDto>();
    public IReadOnlyList<DocumentSummaryDto> Documents { get; init; } = Array.Empty<DocumentSummaryDto>();
    public IReadOnlyList<BreadcrumbDto> Breadcrumbs { get; init; } = Array.Empty<BreadcrumbDto>();
}

/// <summary>
/// Library permission DTO.
/// </summary>
public record LibraryPermissionDto
{
    public Guid Id { get; init; }
    public Guid LibraryId { get; init; }
    public Guid? UserId { get; init; }
    public string? UserName { get; init; }
    public Guid? GroupId { get; init; }
    public string? GroupName { get; init; }
    public Guid? RoleId { get; init; }
    public string? RoleName { get; init; }
    public string AccessLevel { get; init; } = string.Empty;
    public DateTime GrantedAt { get; init; }
    public string? GrantedByName { get; init; }
}

/// <summary>
/// Folder permission DTO.
/// </summary>
public record FolderPermissionDto
{
    public Guid Id { get; init; }
    public Guid FolderId { get; init; }
    public Guid? UserId { get; init; }
    public string? UserName { get; init; }
    public Guid? GroupId { get; init; }
    public string? GroupName { get; init; }
    public string PermissionLevel { get; init; } = string.Empty;
    public bool InheritFromParent { get; init; }
    public bool PropagateToChildren { get; init; }
    public DateTime GrantedAt { get; init; }
}

/// <summary>
/// Set permission request.
/// </summary>
public record SetPermissionRequest
{
    public Guid? UserId { get; init; }
    public Guid? GroupId { get; init; }
    public Guid? RoleId { get; init; }
    public string AccessLevel { get; init; } = "Read";
    public bool InheritFromParent { get; init; } = true;
    public bool PropagateToChildren { get; init; } = true;
}
