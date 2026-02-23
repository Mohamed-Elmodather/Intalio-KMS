using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Content.Domain.Entities;

/// <summary>
/// A content block within an article. Supports various block types
/// and maintains ordering within the document.
/// </summary>
public class ContentBlock : AuditableEntity
{
    public Guid ArticleId { get; private set; }
    public BlockType Type { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public string? ContentArabic { get; private set; }
    public int Order { get; private set; }
    public string? Metadata { get; private set; } // JSON for type-specific data
    public Guid? ParentBlockId { get; private set; } // For nested blocks (e.g., list items)

    // Navigation properties
    public virtual Article Article { get; private set; } = null!;
    public virtual ContentBlock? ParentBlock { get; private set; }
    public virtual ICollection<ContentBlock> ChildBlocks { get; private set; } = new List<ContentBlock>();

    private ContentBlock() { }

    public static ContentBlock Create(
        Guid articleId,
        BlockType type,
        string content,
        int order,
        string? contentArabic = null,
        string? metadata = null,
        Guid? parentBlockId = null)
    {
        return new ContentBlock
        {
            ArticleId = articleId,
            Type = type,
            Content = content,
            ContentArabic = contentArabic,
            Order = order,
            Metadata = metadata,
            ParentBlockId = parentBlockId
        };
    }

    public void UpdateContent(string content, string? contentArabic = null)
    {
        Content = content;
        ContentArabic = contentArabic;
    }

    public void UpdateMetadata(string? metadata)
    {
        Metadata = metadata;
    }

    public void UpdateOrder(int newOrder)
    {
        Order = newOrder;
    }

    public void ChangeType(BlockType newType)
    {
        Type = newType;
    }

    public void SetParent(Guid? parentBlockId)
    {
        ParentBlockId = parentBlockId;
    }

    public void Delete()
    {
        SoftDelete(Guid.Empty);
    }
}

/// <summary>
/// Types of content blocks supported by the editor.
/// </summary>
public enum BlockType
{
    /// <summary>Standard paragraph text</summary>
    Paragraph = 0,

    /// <summary>Heading levels 1-6</summary>
    Heading = 1,

    /// <summary>Bulleted list</summary>
    BulletList = 2,

    /// <summary>Numbered list</summary>
    NumberedList = 3,

    /// <summary>List item (child of BulletList or NumberedList)</summary>
    ListItem = 4,

    /// <summary>Block quote</summary>
    Quote = 5,

    /// <summary>Code block with syntax highlighting</summary>
    Code = 6,

    /// <summary>Embedded image</summary>
    Image = 7,

    /// <summary>Embedded video</summary>
    Video = 8,

    /// <summary>File attachment</summary>
    File = 9,

    /// <summary>Embedded content (YouTube, Twitter, etc.)</summary>
    Embed = 10,

    /// <summary>Horizontal divider</summary>
    Divider = 11,

    /// <summary>Table</summary>
    Table = 12,

    /// <summary>Table row</summary>
    TableRow = 13,

    /// <summary>Table cell</summary>
    TableCell = 14,

    /// <summary>Callout/Alert box</summary>
    Callout = 15,

    /// <summary>Toggle/Accordion block</summary>
    Toggle = 16,

    /// <summary>Two-column layout</summary>
    TwoColumn = 17,

    /// <summary>Three-column layout</summary>
    ThreeColumn = 18,

    /// <summary>Column container (child of layout blocks)</summary>
    Column = 19
}

/// <summary>
/// Block-specific metadata for different block types.
/// Serialized as JSON in ContentBlock.Metadata.
/// </summary>
public static class BlockMetadata
{
    /// <summary>Heading block metadata</summary>
    public record HeadingMeta(int Level); // 1-6

    /// <summary>Code block metadata</summary>
    public record CodeMeta(string Language, bool ShowLineNumbers = true);

    /// <summary>Image block metadata</summary>
    public record ImageMeta(
        string Url,
        string? AltText,
        string? Caption,
        int? Width,
        int? Height,
        string Alignment = "center"); // left, center, right, full

    /// <summary>Video block metadata</summary>
    public record VideoMeta(
        string Url,
        string? ThumbnailUrl,
        string? Caption,
        bool AutoPlay = false,
        bool ShowControls = true);

    /// <summary>File block metadata</summary>
    public record FileMeta(
        string Url,
        string FileName,
        long FileSize,
        string? MimeType);

    /// <summary>Embed block metadata</summary>
    public record EmbedMeta(
        string Url,
        string Provider, // youtube, vimeo, twitter, etc.
        string? EmbedHtml,
        int? Width,
        int? Height);

    /// <summary>Table block metadata</summary>
    public record TableMeta(
        int Rows,
        int Columns,
        bool HasHeader = true);

    /// <summary>Callout block metadata</summary>
    public record CalloutMeta(
        string Type, // info, warning, error, success, tip
        string? Icon);
}

/// <summary>
/// Collaboration session for real-time editing.
/// </summary>
public class CollaborationSession : Entity
{
    public Guid ArticleId { get; private set; }
    public string SessionId { get; private set; } = string.Empty;
    public byte[] CrdtState { get; private set; } = Array.Empty<byte>(); // Y.js document state
    public DateTime CreatedAt { get; private set; }
    public DateTime LastActivityAt { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation
    public virtual Article Article { get; private set; } = null!;
    public virtual ICollection<CollaborationParticipant> Participants { get; private set; } = new List<CollaborationParticipant>();

    private CollaborationSession() { }

    public static CollaborationSession Create(Guid articleId)
    {
        return new CollaborationSession
        {
            ArticleId = articleId,
            SessionId = Guid.NewGuid().ToString("N"),
            CreatedAt = DateTime.UtcNow,
            LastActivityAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public void UpdateState(byte[] newState)
    {
        CrdtState = newState;
        LastActivityAt = DateTime.UtcNow;
    }

    public void UpdateActivity()
    {
        LastActivityAt = DateTime.UtcNow;
    }

    public void End()
    {
        IsActive = false;
    }
}

/// <summary>
/// A participant in a collaboration session.
/// </summary>
public class CollaborationParticipant : Entity
{
    public Guid SessionId { get; private set; }
    public Guid UserId { get; private set; }
    public string UserName { get; private set; } = string.Empty;
    public string? AvatarUrl { get; private set; }
    public string Color { get; private set; } = string.Empty; // Cursor/selection color
    public string ConnectionId { get; private set; } = string.Empty; // SignalR connection ID
    public DateTime JoinedAt { get; private set; }
    public DateTime LastSeenAt { get; private set; }
    public bool IsActive { get; private set; }
    public int? CursorPosition { get; private set; }
    public Guid? CursorBlockId { get; private set; }

    // Navigation
    public virtual CollaborationSession Session { get; private set; } = null!;

    private CollaborationParticipant() { }

    public static CollaborationParticipant Create(
        Guid sessionId,
        Guid userId,
        string userName,
        string connectionId,
        string? avatarUrl = null)
    {
        return new CollaborationParticipant
        {
            SessionId = sessionId,
            UserId = userId,
            UserName = userName,
            ConnectionId = connectionId,
            AvatarUrl = avatarUrl,
            Color = GenerateColor(userId),
            JoinedAt = DateTime.UtcNow,
            LastSeenAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public void UpdateCursor(Guid? blockId, int? position)
    {
        CursorBlockId = blockId;
        CursorPosition = position;
        LastSeenAt = DateTime.UtcNow;
    }

    public void UpdateConnection(string connectionId)
    {
        ConnectionId = connectionId;
        LastSeenAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void Disconnect()
    {
        IsActive = false;
    }

    private static string GenerateColor(Guid userId)
    {
        // Generate a consistent color based on user ID
        var hash = userId.GetHashCode();
        var hue = Math.Abs(hash % 360);
        return $"hsl({hue}, 70%, 50%)";
    }
}

/// <summary>
/// Domain events for collaboration.
/// </summary>
public record BlockCreatedEvent(Guid ArticleId, Guid BlockId, BlockType Type) : DomainEvent;
public record BlockUpdatedEvent(Guid ArticleId, Guid BlockId) : DomainEvent;
public record BlockDeletedEvent(Guid ArticleId, Guid BlockId) : DomainEvent;
public record BlocksReorderedEvent(Guid ArticleId, IReadOnlyList<Guid> BlockIds) : DomainEvent;
public record CollaborationStartedEvent(Guid ArticleId, string SessionId) : DomainEvent;
public record CollaborationEndedEvent(Guid ArticleId, string SessionId) : DomainEvent;
public record ParticipantJoinedEvent(Guid ArticleId, Guid UserId, string UserName) : DomainEvent;
public record ParticipantLeftEvent(Guid ArticleId, Guid UserId) : DomainEvent;
