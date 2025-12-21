using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Collaboration.Domain.Entities;

/// <summary>
/// Community workspace for collaboration.
/// </summary>
public class Community : AuditableEntity
{
    public LocalizedString Name { get; private set; } = null!;
    public LocalizedString? Description { get; private set; }
    public string Slug { get; private set; } = string.Empty;
    public string? CoverImageUrl { get; private set; }
    public string? IconUrl { get; private set; }
    public CommunityType Type { get; private set; }
    public CommunityVisibility Visibility { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool AllowMemberPosts { get; private set; } = true;
    public bool RequirePostApproval { get; private set; }
    public Guid OwnerId { get; private set; }
    public string OwnerName { get; private set; } = string.Empty;
    public int MemberCount { get; private set; }
    public int DiscussionCount { get; private set; }

    // Navigation properties
    public virtual ICollection<CommunityMember> Members { get; private set; } = new List<CommunityMember>();
    public virtual ICollection<Discussion> Discussions { get; private set; } = new List<Discussion>();

    private Community() { }

    public static Community Create(
        LocalizedString name,
        CommunityType type,
        CommunityVisibility visibility,
        Guid ownerId,
        string ownerName)
    {
        var community = new Community
        {
            Name = name,
            Type = type,
            Visibility = visibility,
            OwnerId = ownerId,
            OwnerName = ownerName,
            Slug = GenerateSlug(name.English),
            MemberCount = 1 // Owner is first member
        };

        community.AddDomainEvent(new CommunityCreatedEvent(community.Id, name.English, ownerId));
        return community;
    }

    public void Update(LocalizedString name, LocalizedString? description)
    {
        Name = name;
        Description = description;
        Slug = GenerateSlug(name.English);
    }

    public void SetCoverImage(string? coverImageUrl, string? iconUrl)
    {
        CoverImageUrl = coverImageUrl;
        IconUrl = iconUrl;
    }

    public void SetSettings(bool allowMemberPosts, bool requirePostApproval)
    {
        AllowMemberPosts = allowMemberPosts;
        RequirePostApproval = requirePostApproval;
    }

    public void SetVisibility(CommunityVisibility visibility)
    {
        Visibility = visibility;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public void IncrementMemberCount() => MemberCount++;
    public void DecrementMemberCount() => MemberCount = Math.Max(0, MemberCount - 1);
    public void IncrementDiscussionCount() => DiscussionCount++;
    public void DecrementDiscussionCount() => DiscussionCount = Math.Max(0, DiscussionCount - 1);

    private static string GenerateSlug(string name)
    {
        return name
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "");
    }
}

public enum CommunityType
{
    General,
    Project,
    Department,
    Interest,
    WorkingGroup,
    Committee
}

public enum CommunityVisibility
{
    Public,      // Anyone can view and join
    Private,     // Invite only, content visible to members
    Secret       // Invite only, hidden from search
}

/// <summary>
/// Community membership.
/// </summary>
public class CommunityMember : Entity
{
    public Guid CommunityId { get; private set; }
    public Guid UserId { get; private set; }
    public string UserName { get; private set; } = string.Empty;
    public string? UserAvatarUrl { get; private set; }
    public CommunityRole Role { get; private set; }
    public DateTime JoinedAt { get; private set; }
    public bool NotificationsEnabled { get; private set; } = true;

    public virtual Community Community { get; private set; } = null!;

    private CommunityMember() { }

    public static CommunityMember Create(
        Guid communityId,
        Guid userId,
        string userName,
        CommunityRole role = CommunityRole.Member)
    {
        return new CommunityMember
        {
            CommunityId = communityId,
            UserId = userId,
            UserName = userName,
            Role = role,
            JoinedAt = DateTime.UtcNow
        };
    }

    public void SetRole(CommunityRole role) => Role = role;
    public void SetNotifications(bool enabled) => NotificationsEnabled = enabled;
}

public enum CommunityRole
{
    Member,
    Moderator,
    Admin,
    Owner
}

// Domain Events
public record CommunityCreatedEvent(Guid CommunityId, string Name, Guid OwnerId) : DomainEvent;
public record MemberJoinedCommunityEvent(Guid CommunityId, Guid UserId) : DomainEvent;
public record MemberLeftCommunityEvent(Guid CommunityId, Guid UserId) : DomainEvent;
