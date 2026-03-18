using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Content.Domain.Entities;

/// <summary>
/// Tracks which users are actively editing which content items.
/// Provides real-time collaboration indicators across the system (Phase 3D).
/// </summary>
public class ActiveEditor : Entity
{
    /// <summary>
    /// The type of content being edited (Article, Template, Document, etc.).
    /// </summary>
    public string ContentType { get; private set; } = string.Empty;

    /// <summary>
    /// The ID of the content item being edited.
    /// </summary>
    public Guid ContentId { get; private set; }

    /// <summary>
    /// The user currently editing.
    /// </summary>
    public Guid UserId { get; private set; }
    public string UserName { get; private set; } = string.Empty;
    public string? UserAvatarUrl { get; private set; }

    /// <summary>
    /// Assigned color for cursor/presence display.
    /// </summary>
    public string Color { get; private set; } = string.Empty;

    /// <summary>
    /// SignalR connection ID for this editing session.
    /// </summary>
    public string ConnectionId { get; private set; } = string.Empty;

    /// <summary>
    /// When the user started editing.
    /// </summary>
    public DateTime StartedAt { get; private set; }

    /// <summary>
    /// Last heartbeat/activity timestamp.
    /// </summary>
    public DateTime LastActivityAt { get; private set; }

    /// <summary>
    /// Current editing status.
    /// </summary>
    public EditorStatus Status { get; private set; } = EditorStatus.Active;

    /// <summary>
    /// Which block/section the editor is focused on (if applicable).
    /// </summary>
    public Guid? FocusedBlockId { get; private set; }

    /// <summary>
    /// Whether the user is actively typing.
    /// </summary>
    public bool IsTyping { get; private set; }

    /// <summary>
    /// The device/browser the user is editing from.
    /// </summary>
    public string? UserAgent { get; private set; }

    private ActiveEditor() { }

    public static ActiveEditor Create(
        string contentType,
        Guid contentId,
        Guid userId,
        string userName,
        string connectionId,
        string? userAvatarUrl = null,
        string? userAgent = null)
    {
        return new ActiveEditor
        {
            ContentType = contentType,
            ContentId = contentId,
            UserId = userId,
            UserName = userName,
            ConnectionId = connectionId,
            UserAvatarUrl = userAvatarUrl,
            Color = GenerateColor(userId),
            StartedAt = DateTime.UtcNow,
            LastActivityAt = DateTime.UtcNow,
            Status = EditorStatus.Active,
            UserAgent = userAgent
        };
    }

    public void UpdateActivity()
    {
        LastActivityAt = DateTime.UtcNow;
        if (Status == EditorStatus.Idle)
            Status = EditorStatus.Active;
    }

    public void UpdateFocus(Guid? blockId)
    {
        FocusedBlockId = blockId;
        LastActivityAt = DateTime.UtcNow;
    }

    public void SetTyping(bool isTyping)
    {
        IsTyping = isTyping;
        LastActivityAt = DateTime.UtcNow;
    }

    public void MarkIdle()
    {
        Status = EditorStatus.Idle;
        IsTyping = false;
    }

    public void MarkDisconnected()
    {
        Status = EditorStatus.Disconnected;
        IsTyping = false;
    }

    public void Reconnect(string connectionId)
    {
        ConnectionId = connectionId;
        Status = EditorStatus.Active;
        LastActivityAt = DateTime.UtcNow;
    }

    private static string GenerateColor(Guid userId)
    {
        var hash = userId.GetHashCode();
        var hue = Math.Abs(hash % 360);
        return $"hsl({hue}, 70%, 50%)";
    }
}

/// <summary>
/// Status of an active editor.
/// </summary>
public enum EditorStatus
{
    /// <summary>Actively editing content.</summary>
    Active,

    /// <summary>Connected but idle for a period.</summary>
    Idle,

    /// <summary>Disconnected (may reconnect).</summary>
    Disconnected
}

// Domain Events
public record EditorJoinedEvent(Guid ContentId, string ContentType, Guid UserId, string UserName) : DomainEvent;
public record EditorLeftEvent(Guid ContentId, string ContentType, Guid UserId) : DomainEvent;
public record EditorIdleEvent(Guid ContentId, Guid UserId) : DomainEvent;
