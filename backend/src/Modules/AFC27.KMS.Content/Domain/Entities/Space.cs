using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Content.Domain.Entities;

/// <summary>
/// Types of spaces for organizing content.
/// </summary>
public enum SpaceType
{
    Personal,
    Team,
    Department,
    Project,
    Public
}

/// <summary>
/// Access levels for space permissions.
/// </summary>
public enum SpaceAccessLevel
{
    Read,
    Write,
    Manage,
    Admin
}

/// <summary>
/// Roles within a space membership.
/// </summary>
public enum SpaceMemberRole
{
    Member,
    Editor,
    Admin,
    Owner
}

/// <summary>
/// Space/Teamspace entity for organizing and grouping content.
/// Supports hierarchical structure with parent/child relationships.
/// </summary>
public class Space : AuditableEntity
{
    public LocalizedString Name { get; private set; } = default!;
    public LocalizedString? Description { get; private set; }
    public SpaceType SpaceType { get; private set; }
    public Guid OwnerId { get; private set; }
    public string OwnerName { get; private set; } = string.Empty;
    public string? IconName { get; private set; }
    public string? Color { get; private set; }
    public bool IsPublic { get; private set; }
    public bool IsArchived { get; private set; }
    public Guid? ParentSpaceId { get; private set; }

    // Navigation properties
    public virtual Space? ParentSpace { get; private set; }
    public virtual ICollection<Space> ChildSpaces { get; private set; } = new List<Space>();
    public virtual ICollection<SpaceMember> Members { get; private set; } = new List<SpaceMember>();
    public virtual ICollection<SpacePermission> Permissions { get; private set; } = new List<SpacePermission>();

    private Space() { }

    public static Space Create(
        LocalizedString name,
        SpaceType type,
        Guid ownerId,
        string ownerName,
        bool isPublic = false,
        LocalizedString? description = null)
    {
        var space = new Space
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            SpaceType = type,
            OwnerId = ownerId,
            OwnerName = ownerName,
            IsPublic = isPublic
        };

        // Auto-add owner as Owner member
        space.Members.Add(SpaceMember.Create(space.Id, ownerId, ownerName, SpaceMemberRole.Owner));
        return space;
    }

    public void Update(LocalizedString name, LocalizedString? description, string? iconName, string? color, bool isPublic)
    {
        Name = name;
        Description = description;
        IconName = iconName;
        Color = color;
        IsPublic = isPublic;
    }

    public void Archive() => IsArchived = true;
    public void Unarchive() => IsArchived = false;

    public SpaceMember AddMember(Guid userId, string userName, SpaceMemberRole role = SpaceMemberRole.Member)
    {
        var member = SpaceMember.Create(Id, userId, userName, role);
        Members.Add(member);
        return member;
    }

    public void RemoveMember(Guid userId)
    {
        var member = Members.FirstOrDefault(m => m.UserId == userId);
        if (member != null) Members.Remove(member);
    }

    public void SetParent(Guid? parentSpaceId) => ParentSpaceId = parentSpaceId;
}

/// <summary>
/// Represents a member of a space with their role.
/// </summary>
public class SpaceMember
{
    public Guid Id { get; private set; }
    public Guid SpaceId { get; private set; }
    public Guid UserId { get; private set; }
    public string UserName { get; private set; } = string.Empty;
    public SpaceMemberRole Role { get; private set; }
    public DateTime JoinedAt { get; private set; }

    public virtual Space? Space { get; private set; }

    private SpaceMember() { }

    public static SpaceMember Create(Guid spaceId, Guid userId, string userName, SpaceMemberRole role)
    {
        return new SpaceMember
        {
            Id = Guid.NewGuid(),
            SpaceId = spaceId,
            UserId = userId,
            UserName = userName,
            Role = role,
            JoinedAt = DateTime.UtcNow
        };
    }

    public void ChangeRole(SpaceMemberRole newRole) => Role = newRole;
}

/// <summary>
/// Represents a permission grant on a space for a user, group, or role.
/// </summary>
public class SpacePermission
{
    public Guid Id { get; private set; }
    public Guid SpaceId { get; private set; }
    public Guid? UserId { get; private set; }
    public Guid? GroupId { get; private set; }
    public Guid? RoleId { get; private set; }
    public SpaceAccessLevel AccessLevel { get; private set; }

    public virtual Space? Space { get; private set; }

    private SpacePermission() { }

    public static SpacePermission Create(
        Guid spaceId,
        SpaceAccessLevel level,
        Guid? userId = null,
        Guid? groupId = null,
        Guid? roleId = null)
    {
        return new SpacePermission
        {
            Id = Guid.NewGuid(),
            SpaceId = spaceId,
            UserId = userId,
            GroupId = groupId,
            RoleId = roleId,
            AccessLevel = level
        };
    }
}
