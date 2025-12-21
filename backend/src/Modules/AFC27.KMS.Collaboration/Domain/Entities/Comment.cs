using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Collaboration.Domain.Entities;

/// <summary>
/// Comment on discussions, articles, documents, etc.
/// </summary>
public class Comment : AuditableEntity
{
    public string Content { get; private set; } = string.Empty;
    public string? ContentArabic { get; private set; }
    public CommentableType TargetType { get; private set; }
    public Guid TargetId { get; private set; }
    public Guid? ParentId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string AuthorName { get; private set; } = string.Empty;
    public string? AuthorAvatarUrl { get; private set; }
    public bool IsEdited { get; private set; }
    public DateTime? EditedAt { get; private set; }
    public bool IsAcceptedAnswer { get; private set; }
    public int LikeCount { get; private set; }
    public int ReplyCount { get; private set; }

    // Navigation properties
    public virtual Comment? Parent { get; private set; }
    public virtual ICollection<Comment> Replies { get; private set; } = new List<Comment>();
    public virtual ICollection<CommentReaction> Reactions { get; private set; } = new List<CommentReaction>();
    public virtual ICollection<Mention> Mentions { get; private set; } = new List<Mention>();

    private Comment() { }

    public static Comment Create(
        string content,
        CommentableType targetType,
        Guid targetId,
        Guid authorId,
        string authorName,
        Guid? parentId = null)
    {
        var comment = new Comment
        {
            Content = content,
            TargetType = targetType,
            TargetId = targetId,
            ParentId = parentId,
            AuthorId = authorId,
            AuthorName = authorName
        };

        comment.AddDomainEvent(new CommentCreatedEvent(comment.Id, targetType, targetId, authorId));
        return comment;
    }

    public void Update(string content, string? contentArabic)
    {
        Content = content;
        ContentArabic = contentArabic;
        IsEdited = true;
        EditedAt = DateTime.UtcNow;
    }

    public void MarkAsAcceptedAnswer()
    {
        IsAcceptedAnswer = true;
        AddDomainEvent(new CommentAcceptedAsAnswerEvent(Id, TargetId, AuthorId));
    }

    public void UnmarkAsAcceptedAnswer()
    {
        IsAcceptedAnswer = false;
    }

    public void IncrementLikeCount() => LikeCount++;
    public void DecrementLikeCount() => LikeCount = Math.Max(0, LikeCount - 1);
    public void IncrementReplyCount() => ReplyCount++;
    public void DecrementReplyCount() => ReplyCount = Math.Max(0, ReplyCount - 1);
}

public enum CommentableType
{
    Discussion,
    Article,
    Document,
    Event,
    Task,
    LessonLearned
}

/// <summary>
/// Reaction (like, etc.) on a comment.
/// </summary>
public class CommentReaction : Entity
{
    public Guid CommentId { get; private set; }
    public Guid UserId { get; private set; }
    public string ReactionType { get; private set; } = "like";
    public DateTime CreatedAt { get; private set; }

    public virtual Comment Comment { get; private set; } = null!;

    private CommentReaction() { }

    public static CommentReaction Create(Guid commentId, Guid userId, string reactionType = "like")
    {
        return new CommentReaction
        {
            CommentId = commentId,
            UserId = userId,
            ReactionType = reactionType,
            CreatedAt = DateTime.UtcNow
        };
    }
}

/// <summary>
/// @Mention in a comment.
/// </summary>
public class Mention : Entity
{
    public Guid CommentId { get; private set; }
    public Guid MentionedUserId { get; private set; }
    public string MentionedUserName { get; private set; } = string.Empty;
    public int StartIndex { get; private set; }
    public int EndIndex { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public virtual Comment Comment { get; private set; } = null!;

    private Mention() { }

    public static Mention Create(
        Guid commentId,
        Guid mentionedUserId,
        string mentionedUserName,
        int startIndex,
        int endIndex)
    {
        return new Mention
        {
            CommentId = commentId,
            MentionedUserId = mentionedUserId,
            MentionedUserName = mentionedUserName,
            StartIndex = startIndex,
            EndIndex = endIndex,
            CreatedAt = DateTime.UtcNow
        };
    }
}

// Domain Events
public record CommentCreatedEvent(Guid CommentId, CommentableType TargetType, Guid TargetId, Guid AuthorId) : DomainEvent;
public record CommentAcceptedAsAnswerEvent(Guid CommentId, Guid DiscussionId, Guid AuthorId) : DomainEvent;
public record UserMentionedEvent(Guid CommentId, Guid MentionedUserId, Guid MentionedByUserId) : DomainEvent;
