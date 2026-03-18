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

    // Inline annotation anchor (Phase 3A)
    public Guid? AnchorBlockId { get; private set; }
    public int? AnchorStartOffset { get; private set; }
    public int? AnchorEndOffset { get; private set; }
    public string? AnchorQuotedText { get; private set; }
    public bool IsInlineComment => AnchorBlockId.HasValue;

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

    /// <summary>
    /// Create an inline comment anchored to a specific text range within a content block.
    /// </summary>
    public static Comment CreateInline(
        string content,
        CommentableType targetType,
        Guid targetId,
        Guid authorId,
        string authorName,
        Guid anchorBlockId,
        int anchorStartOffset,
        int anchorEndOffset,
        string? anchorQuotedText = null,
        Guid? parentId = null)
    {
        var comment = new Comment
        {
            Content = content,
            TargetType = targetType,
            TargetId = targetId,
            ParentId = parentId,
            AuthorId = authorId,
            AuthorName = authorName,
            AnchorBlockId = anchorBlockId,
            AnchorStartOffset = anchorStartOffset,
            AnchorEndOffset = anchorEndOffset,
            AnchorQuotedText = anchorQuotedText
        };

        comment.AddDomainEvent(new InlineCommentCreatedEvent(
            comment.Id, targetType, targetId, authorId, anchorBlockId));
        return comment;
    }

    public void UpdateAnchor(Guid blockId, int startOffset, int endOffset, string? quotedText)
    {
        AnchorBlockId = blockId;
        AnchorStartOffset = startOffset;
        AnchorEndOffset = endOffset;
        AnchorQuotedText = quotedText;
    }

    public void ClearAnchor()
    {
        AnchorBlockId = null;
        AnchorStartOffset = null;
        AnchorEndOffset = null;
        AnchorQuotedText = null;
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
/// @Mention in a comment or in article body content (Phase 8A).
/// CommentId is nullable to support in-content mentions (not tied to a comment).
/// </summary>
public class Mention : Entity
{
    public Guid? CommentId { get; private set; }
    public Guid MentionedUserId { get; private set; }
    public string MentionedUserName { get; private set; } = string.Empty;
    public int StartIndex { get; private set; }
    public int EndIndex { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Phase 8A: In-content mention support
    /// <summary>
    /// The block ID within an article body where this mention appears. Null for comment mentions.
    /// </summary>
    public Guid? BlockId { get; private set; }

    /// <summary>
    /// Context describing where the mention occurs (e.g. "article-body", "comment", "inline-comment").
    /// </summary>
    public string? MentionContext { get; private set; }

    /// <summary>
    /// The entity type being mentioned in (Article, Document, etc.). Used for in-content mentions.
    /// </summary>
    public string? TargetEntityType { get; private set; }

    /// <summary>
    /// The entity ID being mentioned in. Used for in-content mentions.
    /// </summary>
    public Guid? TargetEntityId { get; private set; }

    /// <summary>
    /// The user who created the mention. Used for in-content mentions where there is no comment author.
    /// </summary>
    public Guid? MentionedByUserId { get; private set; }

    public virtual Comment? Comment { get; private set; }

    private Mention() { }

    /// <summary>
    /// Create a mention within a comment.
    /// </summary>
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
            MentionContext = "comment",
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Create an in-content mention (e.g. @mention in article body block editor). Phase 8A.
    /// </summary>
    public static Mention CreateInContent(
        Guid mentionedUserId,
        string mentionedUserName,
        Guid mentionedByUserId,
        string targetEntityType,
        Guid targetEntityId,
        Guid blockId,
        int startIndex,
        int endIndex)
    {
        var mention = new Mention
        {
            CommentId = null,
            MentionedUserId = mentionedUserId,
            MentionedUserName = mentionedUserName,
            MentionedByUserId = mentionedByUserId,
            TargetEntityType = targetEntityType,
            TargetEntityId = targetEntityId,
            BlockId = blockId,
            StartIndex = startIndex,
            EndIndex = endIndex,
            MentionContext = "article-body",
            CreatedAt = DateTime.UtcNow
        };

        mention.AddDomainEvent(new InContentMentionCreatedEvent(
            mention.Id, mentionedUserId, mentionedByUserId, targetEntityType, targetEntityId, blockId));
        return mention;
    }
}

// Domain Events
public record CommentCreatedEvent(Guid CommentId, CommentableType TargetType, Guid TargetId, Guid AuthorId) : DomainEvent;
public record CommentAcceptedAsAnswerEvent(Guid CommentId, Guid DiscussionId, Guid AuthorId) : DomainEvent;
public record UserMentionedEvent(Guid CommentId, Guid MentionedUserId, Guid MentionedByUserId) : DomainEvent;
public record InlineCommentCreatedEvent(Guid CommentId, CommentableType TargetType, Guid TargetId, Guid AuthorId, Guid AnchorBlockId) : DomainEvent;

// Phase 8A: In-content mention event
public record InContentMentionCreatedEvent(
    Guid MentionId, Guid MentionedUserId, Guid MentionedByUserId,
    string TargetEntityType, Guid TargetEntityId, Guid BlockId) : DomainEvent;
