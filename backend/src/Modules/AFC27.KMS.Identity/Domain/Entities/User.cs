using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Identity.Domain.Entities;

/// <summary>
/// Represents a user in the Knowledge Management System.
/// </summary>
public class User : AuditableEntity
{
    public string Email { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string DisplayNameArabic { get; private set; } = string.Empty;
    public string? AvatarUrl { get; private set; }
    public string? JobTitle { get; private set; }
    public string? JobTitleArabic { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? EmployeeId { get; private set; }
    public string PreferredLanguage { get; private set; } = "en";
    public bool IsActive { get; private set; } = true;
    public DateTime? LastLoginAt { get; private set; }

    // Navigation properties
    public Guid? DepartmentId { get; private set; }
    public Department? Department { get; private set; }
    public Guid? ManagerId { get; private set; }
    public User? Manager { get; private set; }
    public ICollection<User> DirectReports { get; private set; } = new List<User>();
    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public ICollection<GroupMember> GroupMemberships { get; private set; } = new List<GroupMember>();

    private User() { }

    public static User Create(
        string email,
        string displayName,
        string? displayNameArabic = null)
    {
        var user = new User
        {
            Email = email.ToLowerInvariant(),
            DisplayName = displayName,
            DisplayNameArabic = displayNameArabic ?? displayName
        };

        user.AddDomainEvent(new UserCreatedEvent(user.Id, user.Email));
        return user;
    }

    public void UpdateProfile(
        string displayName,
        string? displayNameArabic,
        string? jobTitle,
        string? jobTitleArabic,
        string? phoneNumber,
        string preferredLanguage)
    {
        DisplayName = displayName;
        DisplayNameArabic = displayNameArabic ?? displayName;
        JobTitle = jobTitle;
        JobTitleArabic = jobTitleArabic;
        PhoneNumber = phoneNumber;
        PreferredLanguage = preferredLanguage;

        AddDomainEvent(new UserProfileUpdatedEvent(Id));
    }

    public void SetDepartment(Guid? departmentId)
    {
        DepartmentId = departmentId;
    }

    public void SetManager(Guid? managerId)
    {
        ManagerId = managerId;
    }

    public void SetAvatar(string avatarUrl)
    {
        AvatarUrl = avatarUrl;
    }

    public void Activate()
    {
        IsActive = true;
        AddDomainEvent(new UserActivatedEvent(Id));
    }

    public void Deactivate()
    {
        IsActive = false;
        AddDomainEvent(new UserDeactivatedEvent(Id));
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public string GetDisplayName(string language)
    {
        return language?.ToLowerInvariant() == "ar" ? DisplayNameArabic : DisplayName;
    }
}

// Domain Events
public record UserCreatedEvent(Guid UserId, string Email) : DomainEvent;
public record UserProfileUpdatedEvent(Guid UserId) : DomainEvent;
public record UserActivatedEvent(Guid UserId) : DomainEvent;
public record UserDeactivatedEvent(Guid UserId) : DomainEvent;
