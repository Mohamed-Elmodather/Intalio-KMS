using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Documents.Domain.Entities;

/// <summary>
/// Folder-level permission for granular access control.
/// Supports inheritance from parent folders and propagation to children.
/// </summary>
public class FolderPermission : AuditableEntity
{
    public Guid FolderId { get; private set; }
    public Guid? UserId { get; private set; }
    public Guid? GroupId { get; private set; }
    public PermissionLevel Level { get; private set; }
    public bool InheritFromParent { get; private set; } = true;
    public bool PropagateToChildren { get; private set; } = true;
    public Guid? GrantedById { get; private set; }
    public string? GrantedByName { get; private set; }

    // Navigation properties
    public virtual Folder Folder { get; private set; } = null!;

    private FolderPermission() { }

    /// <summary>
    /// Creates a permission for a specific user.
    /// </summary>
    public static FolderPermission CreateForUser(
        Guid folderId,
        Guid userId,
        PermissionLevel level,
        Guid grantedById,
        string grantedByName,
        bool inheritFromParent = true,
        bool propagateToChildren = true)
    {
        return new FolderPermission
        {
            FolderId = folderId,
            UserId = userId,
            Level = level,
            InheritFromParent = inheritFromParent,
            PropagateToChildren = propagateToChildren,
            GrantedById = grantedById,
            GrantedByName = grantedByName
        };
    }

    /// <summary>
    /// Creates a permission for a group.
    /// </summary>
    public static FolderPermission CreateForGroup(
        Guid folderId,
        Guid groupId,
        PermissionLevel level,
        Guid grantedById,
        string grantedByName,
        bool inheritFromParent = true,
        bool propagateToChildren = true)
    {
        return new FolderPermission
        {
            FolderId = folderId,
            GroupId = groupId,
            Level = level,
            InheritFromParent = inheritFromParent,
            PropagateToChildren = propagateToChildren,
            GrantedById = grantedById,
            GrantedByName = grantedByName
        };
    }

    public void UpdateLevel(PermissionLevel newLevel)
    {
        Level = newLevel;
    }

    public void SetInheritance(bool inheritFromParent, bool propagateToChildren)
    {
        InheritFromParent = inheritFromParent;
        PropagateToChildren = propagateToChildren;
    }

    /// <summary>
    /// Checks if this permission applies to a specific user (directly or via group).
    /// </summary>
    public bool AppliesToUser(Guid userId, IEnumerable<Guid>? userGroupIds = null)
    {
        if (UserId == userId)
            return true;

        if (GroupId.HasValue && userGroupIds?.Contains(GroupId.Value) == true)
            return true;

        return false;
    }

    /// <summary>
    /// Checks if this permission grants a specific access level.
    /// </summary>
    public bool HasLevel(PermissionLevel requiredLevel)
    {
        return (Level & requiredLevel) == requiredLevel;
    }
}

/// <summary>
/// Document-level permission for file-specific access control.
/// Overrides folder permissions when set explicitly.
/// </summary>
public class DocumentPermission : AuditableEntity
{
    public Guid DocumentId { get; private set; }
    public Guid? UserId { get; private set; }
    public Guid? GroupId { get; private set; }
    public PermissionLevel Level { get; private set; }
    public Guid? GrantedById { get; private set; }
    public string? GrantedByName { get; private set; }

    // Navigation properties
    public virtual Document Document { get; private set; } = null!;

    private DocumentPermission() { }

    /// <summary>
    /// Creates a permission for a specific user on a document.
    /// </summary>
    public static DocumentPermission CreateForUser(
        Guid documentId,
        Guid userId,
        PermissionLevel level,
        Guid grantedById,
        string grantedByName)
    {
        return new DocumentPermission
        {
            DocumentId = documentId,
            UserId = userId,
            Level = level,
            GrantedById = grantedById,
            GrantedByName = grantedByName
        };
    }

    /// <summary>
    /// Creates a permission for a group on a document.
    /// </summary>
    public static DocumentPermission CreateForGroup(
        Guid documentId,
        Guid groupId,
        PermissionLevel level,
        Guid grantedById,
        string grantedByName)
    {
        return new DocumentPermission
        {
            DocumentId = documentId,
            GroupId = groupId,
            Level = level,
            GrantedById = grantedById,
            GrantedByName = grantedByName
        };
    }

    public void UpdateLevel(PermissionLevel newLevel)
    {
        Level = newLevel;
    }

    /// <summary>
    /// Checks if this permission applies to a specific user.
    /// </summary>
    public bool AppliesToUser(Guid userId, IEnumerable<Guid>? userGroupIds = null)
    {
        if (UserId == userId)
            return true;

        if (GroupId.HasValue && userGroupIds?.Contains(GroupId.Value) == true)
            return true;

        return false;
    }

    /// <summary>
    /// Checks if this permission grants a specific access level.
    /// </summary>
    public bool HasLevel(PermissionLevel requiredLevel)
    {
        return (Level & requiredLevel) == requiredLevel;
    }
}

/// <summary>
/// Permission levels for ACL (bitwise flags for combination).
/// </summary>
[Flags]
public enum PermissionLevel
{
    /// <summary>No access</summary>
    None = 0,

    /// <summary>Can view/read the item</summary>
    Read = 1,

    /// <summary>Can edit/modify the item</summary>
    Write = 2,

    /// <summary>Can delete the item</summary>
    Delete = 4,

    /// <summary>Can share the item with others</summary>
    Share = 8,

    /// <summary>Can manage permissions on the item</summary>
    Manage = 16,

    /// <summary>Full control (all permissions)</summary>
    FullControl = Read | Write | Delete | Share | Manage
}

/// <summary>
/// Extension methods for permission level operations.
/// </summary>
public static class PermissionLevelExtensions
{
    /// <summary>
    /// Checks if a permission level includes another level.
    /// </summary>
    public static bool Includes(this PermissionLevel level, PermissionLevel requiredLevel)
    {
        return (level & requiredLevel) == requiredLevel;
    }

    /// <summary>
    /// Combines two permission levels.
    /// </summary>
    public static PermissionLevel Combine(this PermissionLevel level, PermissionLevel other)
    {
        return level | other;
    }

    /// <summary>
    /// Converts library access level to permission level.
    /// </summary>
    public static PermissionLevel ToPermissionLevel(this LibraryAccessLevel accessLevel)
    {
        return accessLevel switch
        {
            LibraryAccessLevel.Read => PermissionLevel.Read,
            LibraryAccessLevel.Write => PermissionLevel.Read | PermissionLevel.Write,
            LibraryAccessLevel.Manage => PermissionLevel.Read | PermissionLevel.Write | PermissionLevel.Delete | PermissionLevel.Share,
            LibraryAccessLevel.FullControl => PermissionLevel.FullControl,
            _ => PermissionLevel.None
        };
    }
}
