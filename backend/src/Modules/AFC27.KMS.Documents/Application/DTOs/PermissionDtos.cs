namespace AFC27.KMS.Documents.Application.DTOs;

/// <summary>
/// Folder contents response DTO.
/// </summary>
public record FolderContentsDto
{
    public FolderDto Folder { get; init; } = null!;
    public IReadOnlyList<FolderSummaryDto> Subfolders { get; init; } = Array.Empty<FolderSummaryDto>();
    public IReadOnlyList<DocumentSummaryDto> Documents { get; init; } = Array.Empty<DocumentSummaryDto>();
    public IReadOnlyList<BreadcrumbDto> Breadcrumbs { get; init; } = Array.Empty<BreadcrumbDto>();
}

/// <summary>
/// Folder permission DTO.
/// </summary>
public record FolderPermissionDto
{
    public Guid Id { get; init; }
    public Guid FolderId { get; init; }
    public Guid? UserId { get; init; }
    public Guid? GroupId { get; init; }
    public string? UserName { get; init; }
    public string? GroupName { get; init; }
    public string AccessLevel { get; init; } = string.Empty;
    public string PermissionLevel { get; init; } = string.Empty;
    public bool InheritFromParent { get; init; }
    public bool PropagateToChildren { get; init; }
    public DateTime GrantedAt { get; init; }
    public string? GrantedBy { get; init; }
}

/// <summary>
/// Library permission DTO.
/// </summary>
public record LibraryPermissionDto
{
    public Guid Id { get; init; }
    public Guid LibraryId { get; init; }
    public Guid? UserId { get; init; }
    public Guid? GroupId { get; init; }
    public Guid? RoleId { get; init; }
    public string? UserName { get; init; }
    public string? GroupName { get; init; }
    public string? RoleName { get; init; }
    public string AccessLevel { get; init; } = string.Empty;
    public DateTime GrantedAt { get; init; }
    public string? GrantedBy { get; init; }
}

/// <summary>
/// Request to set permission on a library or folder.
/// </summary>
public record SetPermissionRequest
{
    public Guid? UserId { get; init; }
    public Guid? GroupId { get; init; }
    public Guid? RoleId { get; init; }
    public string AccessLevel { get; init; } = "Read";
    public bool InheritFromParent { get; init; } = true;
    public bool PropagateToChildren { get; init; } = false;
}

/// <summary>
/// Folder tree DTO for hierarchical display.
/// </summary>
public record FolderTreeDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? IconName { get; init; }
    public string? Color { get; init; }
    public Guid? ParentId { get; init; }
    public int DocumentCount { get; init; }
    public IReadOnlyList<FolderTreeDto> Children { get; init; } = Array.Empty<FolderTreeDto>();
}

/// <summary>
/// Library statistics DTO.
/// </summary>
public record LibraryStatsDto
{
    public Guid LibraryId { get; init; }
    public string LibraryName { get; init; } = string.Empty;
    public int TotalDocuments { get; init; }
    public int TotalFolders { get; init; }
    public long TotalSizeBytes { get; init; }
    public int TotalVersions { get; init; }
    public int PendingApprovals { get; init; }
    public int ActiveUsers { get; init; }
    public int PublishedDocuments { get; init; }
    public int DraftDocuments { get; init; }
    public int CheckedOutDocuments { get; init; }
    public int UniqueContributors { get; init; }
    public DateTime? LastActivityAt { get; init; }
    public Dictionary<string, int> DocumentsByType { get; init; } = new();
    public Dictionary<string, int> DocumentsByExtension { get; init; } = new();
    public Dictionary<string, long> SizeByType { get; init; } = new();
}
