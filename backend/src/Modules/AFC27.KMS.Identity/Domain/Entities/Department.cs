using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Identity.Domain.Entities;

/// <summary>
/// Represents an organizational department.
/// </summary>
public class Department : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string NameArabic { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? DescriptionArabic { get; private set; }
    public int SortOrder { get; private set; }

    // Hierarchical structure
    public Guid? ParentId { get; private set; }
    public Department? Parent { get; private set; }
    public ICollection<Department> Children { get; private set; } = new List<Department>();

    // Manager
    public Guid? ManagerId { get; private set; }
    public User? Manager { get; private set; }

    // Members
    public ICollection<User> Members { get; private set; } = new List<User>();

    private Department() { }

    public static Department Create(
        string name,
        string? nameArabic = null,
        string? description = null,
        Guid? parentId = null)
    {
        return new Department
        {
            Name = name,
            NameArabic = nameArabic ?? name,
            Description = description,
            ParentId = parentId
        };
    }

    public void Update(
        string name,
        string? nameArabic,
        string? description,
        string? descriptionArabic)
    {
        Name = name;
        NameArabic = nameArabic ?? name;
        Description = description;
        DescriptionArabic = descriptionArabic ?? description;
    }

    public void SetManager(Guid? managerId)
    {
        ManagerId = managerId;
    }

    public void SetParent(Guid? parentId)
    {
        if (parentId == Id)
            throw new InvalidOperationException("A department cannot be its own parent");

        ParentId = parentId;
    }

    public void SetSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
    }

    public string GetName(string language)
    {
        return language?.ToLowerInvariant() == "ar" ? NameArabic : Name;
    }
}

/// <summary>
/// Represents a user group (e.g., project team, committee).
/// </summary>
public class Group : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string NameArabic { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? DescriptionArabic { get; private set; }
    public GroupType Type { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Owner
    public Guid OwnerId { get; private set; }
    public User Owner { get; private set; } = null!;

    // Members
    public ICollection<GroupMember> Members { get; private set; } = new List<GroupMember>();

    private Group() { }

    public static Group Create(
        string name,
        string? nameArabic,
        GroupType type,
        Guid ownerId,
        string? description = null)
    {
        return new Group
        {
            Name = name,
            NameArabic = nameArabic ?? name,
            Type = type,
            OwnerId = ownerId,
            Description = description
        };
    }

    public void AddMember(Guid userId, GroupMemberRole role = GroupMemberRole.Member)
    {
        if (Members.Any(m => m.UserId == userId))
            return;

        Members.Add(new GroupMember
        {
            GroupId = Id,
            UserId = userId,
            Role = role,
            JoinedAt = DateTime.UtcNow
        });
    }

    public void RemoveMember(Guid userId)
    {
        var member = Members.FirstOrDefault(m => m.UserId == userId);
        if (member != null)
        {
            Members.Remove(member);
        }
    }
}

/// <summary>
/// Junction table for Group-User relationship.
/// </summary>
public class GroupMember
{
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public GroupMemberRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
}

public enum GroupType
{
    Team,
    Committee,
    Project,
    Community,
    Other
}

public enum GroupMemberRole
{
    Member,
    Moderator,
    Admin
}
