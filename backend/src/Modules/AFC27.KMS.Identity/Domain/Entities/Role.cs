using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Identity.Domain.Entities;

/// <summary>
/// Represents a role with associated permissions.
/// </summary>
public class Role : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string NameArabic { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? DescriptionArabic { get; private set; }
    public bool IsSystemRole { get; private set; }

    // Navigation properties
    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();
    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

    private Role() { }

    public static Role Create(
        string name,
        string? nameArabic = null,
        string? description = null,
        string? descriptionArabic = null,
        bool isSystemRole = false)
    {
        return new Role
        {
            Name = name,
            NameArabic = nameArabic ?? name,
            Description = description,
            DescriptionArabic = descriptionArabic ?? description,
            IsSystemRole = isSystemRole
        };
    }

    public void Update(
        string name,
        string? nameArabic,
        string? description,
        string? descriptionArabic)
    {
        if (IsSystemRole)
            throw new InvalidOperationException("System roles cannot be modified");

        Name = name;
        NameArabic = nameArabic ?? name;
        Description = description;
        DescriptionArabic = descriptionArabic ?? description;
    }

    public void AddPermission(Permission permission)
    {
        if (RolePermissions.Any(rp => rp.PermissionId == permission.Id))
            return;

        RolePermissions.Add(new RolePermission { RoleId = Id, PermissionId = permission.Id });
    }

    public void RemovePermission(Guid permissionId)
    {
        var rolePermission = RolePermissions.FirstOrDefault(rp => rp.PermissionId == permissionId);
        if (rolePermission != null)
        {
            RolePermissions.Remove(rolePermission);
        }
    }
}

/// <summary>
/// Represents a permission that can be assigned to roles.
/// </summary>
public class Permission : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Resource { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    private Permission() { }

    public static Permission Create(string resource, string action, string? description = null)
    {
        return new Permission
        {
            Name = $"{resource}:{action}",
            Resource = resource,
            Action = action,
            Description = description
        };
    }
}

/// <summary>
/// Junction table for Role-Permission relationship.
/// </summary>
public class RolePermission
{
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}

/// <summary>
/// Junction table for User-Role relationship.
/// </summary>
public class UserRole
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public Guid? AssignedBy { get; set; }
}
