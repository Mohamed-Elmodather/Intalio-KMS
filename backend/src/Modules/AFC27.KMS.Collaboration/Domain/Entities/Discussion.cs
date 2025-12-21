using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Collaboration.Domain.Entities;

/// <summary>
/// Discussion thread in a community.
/// </summary>
public class Discussion : AuditableEntity
{
    public LocalizedString Title { get; private set; } = null!;
    public string Content { get; private set; } = string.Empty;
    public string? ContentArabic { get; private set; }
    public Guid CommunityId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string AuthorName { get; private set; } = string.Empty;
    public string? AuthorAvatarUrl { get; private set; }
    public DiscussionType Type { get; private set; }
    public DiscussionStatus Status { get; private set; }
    public bool IsPinned { get; private set; }
    public bool IsLocked { get; private set; }
    public bool IsAnnouncement { get; private set; }
    public int ViewCount { get; private set; }
    public int ReplyCount { get; private set; }
    public int LikeCount { get; private set; }
    public DateTime? LastActivityAt { get; private set; }
    public Guid? LastReplyById { get; private set; }
    public string? LastReplyByName { get; private set; }

    // Navigation properties
    public virtual Community Community { get; private set; } = null!;
    public virtual ICollection<Comment> Comments { get; private set; } = new List<Comment>();
    public virtual ICollection<DiscussionTag> Tags { get; private set; } = new List<DiscussionTag>();
    public virtual ICollection<DiscussionReaction> Reactions { get; private set; } = new List<DiscussionReaction>();

    private Discussion() { }

    public static Discussion Create(
        LocalizedString title,
        string content,
        Guid communityId,
        Guid authorId,
        string authorName,
        DiscussionType type = DiscussionType.Discussion)
    {
        var discussion = new Discussion
        {
            Title = title,
            Content = content,
            CommunityId = communityId,
            AuthorId = authorId,
            AuthorName = authorName,
            Type = type,
            Status = DiscussionStatus.Active,
            LastActivityAt = DateTime.UtcNow
        };

        discussion.AddDomainEvent(new DiscussionCreatedEvent(discussion.Id, title.English, communityId, authorId));
        return discussion;
    }

    public void Update(LocalizedString title, string content, string? contentArabic)
    {
        Title = title;
        Content = content;
        ContentArabic = contentArabic;
    }

    public void SetPinned(bool isPinned) => IsPinned = isPinned;
    public void SetLocked(bool isLocked) => IsLocked = isLocked;
    public void SetAnnouncement(bool isAnnouncement) => IsAnnouncement = isAnnouncement;

    public void MarkAsAnswered()
    {
        if (Type == DiscussionType.Question)
        {
            Status = DiscussionStatus.Answered;
        }
    }

    public void Close()
    {
        Status = DiscussionStatus.Closed;
        IsLocked = true;
    }

    public void Reopen()
    {
        Status = DiscussionStatus.Active;
        IsLocked = false;
    }

    public void Archive()
    {
        Status = DiscussionStatus.Archived;
    }

    public void IncrementViewCount() => ViewCount++;
    public void IncrementReplyCount() => ReplyCount++;
    public void DecrementReplyCount() => ReplyCount = Math.Max(0, ReplyCount - 1);
    public void IncrementLikeCount() => LikeCount++;
    public void DecrementLikeCount() => LikeCount = Math.Max(0, LikeCount - 1);

    public void UpdateLastActivity(Guid userId, string userName)
    {
        LastActivityAt = DateTime.UtcNow;
        LastReplyById = userId;
        LastReplyByName = userName;
    }
}

public enum DiscussionType
{
    Discussion,
    Question,
    Announcement,
    Poll,
    Idea,
    Issue
}

public enum DiscussionStatus
{
    Active,
    Answered,    // For questions
    Closed,
    Archived
}

/// <summary>
/// Tag for discussions.
/// </summary>
public class DiscussionTag
{
    public Guid DiscussionId { get; set; }
    public string Tag { get; set; } = string.Empty;
    public string? TagArabic { get; set; }

    public virtual Discussion Discussion { get; set; } = null!;
}

/// <summary>
/// Reaction (like, etc.) on a discussion.
/// </summary>
public class DiscussionReaction : Entity
{
    public Guid DiscussionId { get; private set; }
    public Guid UserId { get; private set; }
    public string ReactionType { get; private set; } = "like";
    public DateTime CreatedAt { get; private set; }

    public virtual Discussion Discussion { get; private set; } = null!;

    private DiscussionReaction() { }

    public static DiscussionReaction Create(Guid discussionId, Guid userId, string reactionType = "like")
    {
        return new DiscussionReaction
        {
            DiscussionId = discussionId,
            UserId = userId,
            ReactionType = reactionType,
            CreatedAt = DateTime.UtcNow
        };
    }
}

// Domain Events
public record DiscussionCreatedEvent(Guid DiscussionId, string Title, Guid CommunityId, Guid AuthorId) : DomainEvent;
public record DiscussionRepliedEvent(Guid DiscussionId, Guid CommentId, Guid AuthorId) : DomainEvent;
