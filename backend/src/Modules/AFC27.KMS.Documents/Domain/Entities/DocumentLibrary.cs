using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Documents.Domain.Entities;

/// <summary>
/// Document library for organizing documents.
/// </summary>
public class DocumentLibrary : AuditableEntity
{
    public LocalizedString Name { get; private set; } = null!;
    public LocalizedString? Description { get; private set; }
    public string? IconName { get; private set; }
    public string? Color { get; private set; }
    public LibraryType Type { get; private set; }
    public bool IsPublic { get; private set; }
    public bool RequiresApproval { get; private set; }
    public bool EnableVersioning { get; private set; } = true;
    public int? MaxVersions { get; private set; }
    public long? MaxFileSize { get; private set; }
    public string? AllowedExtensions { get; private set; }
    public Guid OwnerId { get; private set; }
    public string OwnerName { get; private set; } = string.Empty;

    // Navigation properties
    public virtual ICollection<Folder> Folders { get; private set; } = new List<Folder>();
    public virtual ICollection<Document> Documents { get; private set; } = new List<Document>();
    public virtual ICollection<LibraryPermission> Permissions { get; private set; } = new List<LibraryPermission>();

    private DocumentLibrary() { }

    public static DocumentLibrary Create(
        LocalizedString name,
        LibraryType type,
        Guid ownerId,
        string ownerName,
        bool isPublic = false)
    {
        return new DocumentLibrary
        {
            Name = name,
            Type = type,
            OwnerId = ownerId,
            OwnerName = ownerName,
            IsPublic = isPublic
        };
    }

    public void Update(LocalizedString name, LocalizedString? description)
    {
        Name = name;
        Description = description;
    }

    public void SetIcon(string? iconName, string? color)
    {
        IconName = iconName;
        Color = color;
    }

    public void SetVersioningOptions(bool enableVersioning, int? maxVersions)
    {
        EnableVersioning = enableVersioning;
        MaxVersions = maxVersions;
    }

    public void SetFileRestrictions(long? maxFileSize, string? allowedExtensions)
    {
        MaxFileSize = maxFileSize;
        AllowedExtensions = allowedExtensions;
    }

    public void SetApprovalRequired(bool requiresApproval)
    {
        RequiresApproval = requiresApproval;
    }

    public void SetPublic(bool isPublic)
    {
        IsPublic = isPublic;
    }
}

public enum LibraryType
{
    General,
    Project,
    Department,
    Personal,
    Archive
}

/// <summary>
/// Folder for organizing documents within a library.
/// </summary>
public class Folder : AuditableEntity
{
    public LocalizedString Name { get; private set; } = null!;
    public LocalizedString? Description { get; private set; }
    public Guid LibraryId { get; private set; }
    public Guid? ParentId { get; private set; }
    public string Path { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }
    public string? IconName { get; private set; }
    public string? Color { get; private set; }

    // Navigation properties
    public virtual DocumentLibrary Library { get; private set; } = null!;
    public virtual Folder? Parent { get; private set; }
    public virtual ICollection<Folder> Children { get; private set; } = new List<Folder>();
    public virtual ICollection<Document> Documents { get; private set; } = new List<Document>();

    private Folder() { }

    public static Folder Create(LocalizedString name, Guid libraryId, Guid? parentId = null, string? parentPath = null)
    {
        var path = string.IsNullOrEmpty(parentPath)
            ? name.English
            : $"{parentPath}/{name.English}";

        return new Folder
        {
            Name = name,
            LibraryId = libraryId,
            ParentId = parentId,
            Path = path
        };
    }

    public void Update(LocalizedString name, LocalizedString? description)
    {
        Name = name;
        Description = description;
    }

    public void MoveTo(Guid? parentId, string newPath)
    {
        ParentId = parentId;
        Path = newPath;
    }

    public void SetIcon(string? iconName, string? color)
    {
        IconName = iconName;
        Color = color;
    }

    public void SetSortOrder(int order)
    {
        SortOrder = order;
    }
}

/// <summary>
/// Library permission for access control.
/// </summary>
public class LibraryPermission : Entity
{
    public Guid LibraryId { get; private set; }
    public Guid? UserId { get; private set; }
    public Guid? RoleId { get; private set; }
    public Guid? GroupId { get; private set; }
    public LibraryAccessLevel AccessLevel { get; private set; }

    public virtual DocumentLibrary Library { get; private set; } = null!;
}

public enum LibraryAccessLevel
{
    Read,
    Write,
    Manage,
    FullControl
}
