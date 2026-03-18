using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Content.Application.Interfaces;
using AFC27.KMS.Content.Application.DTOs;

namespace AFC27.KMS.Content.Infrastructure.Hubs;

/// <summary>
/// SignalR hub for real-time document collaboration.
/// Handles CRDT synchronization, presence, cursor updates, and active editor indicators (Phase 3D).
/// </summary>
[Authorize]
public class CollaborationHub : Hub<ICollaborationClient>
{
    private readonly ICollaborationService _collaborationService;
    private readonly IPresenceService _presenceService;
    private readonly IActiveEditorService _activeEditorService;
    private readonly ILogger<CollaborationHub> _logger;

    public CollaborationHub(
        ICollaborationService collaborationService,
        IPresenceService presenceService,
        IActiveEditorService activeEditorService,
        ILogger<CollaborationHub> logger)
    {
        _collaborationService = collaborationService;
        _presenceService = presenceService;
        _activeEditorService = activeEditorService;
        _logger = logger;
    }

    /// <summary>
    /// Join a collaboration session for an article.
    /// </summary>
    public async Task JoinSession(Guid articleId)
    {
        var userId = GetUserId();
        var userName = GetUserName();
        var connectionId = Context.ConnectionId;

        _logger.LogInformation(
            "User {UserId} ({UserName}) joining collaboration session for article {ArticleId}",
            userId, userName, articleId);

        // Join SignalR group for this article
        var groupName = GetGroupName(articleId);
        await Groups.AddToGroupAsync(connectionId, groupName);

        // Register participant in the session
        var session = await _collaborationService.JoinSessionAsync(
            articleId, userId, userName, connectionId);

        // Get current CRDT state
        var state = await _collaborationService.GetSessionStateAsync(articleId);

        // Send current state to the joining user
        await Clients.Caller.SessionJoined(new SessionJoinedMessage
        {
            ArticleId = articleId,
            SessionId = session.SessionId,
            State = state,
            Participants = session.Participants
        });

        // Notify other participants
        await Clients.OthersInGroup(groupName).ParticipantJoined(new ParticipantMessage
        {
            UserId = userId,
            UserName = userName,
            Color = session.Participants.FirstOrDefault(p => p.UserId == userId)?.Color ?? "#000"
        });
    }

    /// <summary>
    /// Leave a collaboration session.
    /// </summary>
    public async Task LeaveSession(Guid articleId)
    {
        var userId = GetUserId();
        var connectionId = Context.ConnectionId;
        var groupName = GetGroupName(articleId);

        _logger.LogInformation(
            "User {UserId} leaving collaboration session for article {ArticleId}",
            userId, articleId);

        // Remove from SignalR group
        await Groups.RemoveFromGroupAsync(connectionId, groupName);

        // Unregister participant
        await _collaborationService.LeaveSessionAsync(articleId, userId);

        // Notify other participants
        await Clients.OthersInGroup(groupName).ParticipantLeft(new ParticipantLeftMessage
        {
            UserId = userId
        });
    }

    /// <summary>
    /// Sync CRDT update to all participants.
    /// </summary>
    public async Task SyncUpdate(Guid articleId, byte[] update)
    {
        var userId = GetUserId();
        var groupName = GetGroupName(articleId);

        _logger.LogDebug(
            "User {UserId} syncing update for article {ArticleId}, size: {Size} bytes",
            userId, articleId, update.Length);

        // Apply update to server-side CRDT state
        await _collaborationService.ApplyUpdateAsync(articleId, update);

        // Broadcast update to other participants
        await Clients.OthersInGroup(groupName).SyncUpdate(new SyncUpdateMessage
        {
            ArticleId = articleId,
            Update = update,
            SenderId = userId
        });
    }

    /// <summary>
    /// Update cursor position for presence awareness.
    /// </summary>
    public async Task UpdateCursor(Guid articleId, Guid? blockId, int? position)
    {
        var userId = GetUserId();
        var groupName = GetGroupName(articleId);

        // Update cursor in presence service
        await _presenceService.UpdateCursorAsync(articleId, userId, blockId, position);

        // Broadcast cursor update to other participants
        await Clients.OthersInGroup(groupName).CursorUpdated(new CursorUpdateMessage
        {
            UserId = userId,
            BlockId = blockId,
            Position = position
        });
    }

    /// <summary>
    /// Update user selection for highlighting.
    /// </summary>
    public async Task UpdateSelection(Guid articleId, SelectionRange? selection)
    {
        var userId = GetUserId();
        var groupName = GetGroupName(articleId);

        // Broadcast selection update
        await Clients.OthersInGroup(groupName).SelectionUpdated(new SelectionUpdateMessage
        {
            UserId = userId,
            Selection = selection
        });
    }

    /// <summary>
    /// Send awareness update (typing indicator, etc.).
    /// </summary>
    public async Task UpdateAwareness(Guid articleId, AwarenessState awareness)
    {
        var userId = GetUserId();
        var groupName = GetGroupName(articleId);

        await Clients.OthersInGroup(groupName).AwarenessUpdated(new AwarenessUpdateMessage
        {
            UserId = userId,
            Awareness = awareness
        });
    }

    /// <summary>
    /// Request full state sync (for recovery after disconnect).
    /// </summary>
    public async Task RequestStateSync(Guid articleId)
    {
        var state = await _collaborationService.GetSessionStateAsync(articleId);
        var participants = await _presenceService.GetParticipantsAsync(articleId);

        await Clients.Caller.StateSynced(new StateSyncMessage
        {
            ArticleId = articleId,
            State = state,
            Participants = participants
        });
    }

    // ========================================
    // Phase 3D: Real-time Collaboration Indicators
    // ========================================

    /// <summary>
    /// Register as an active editor for any content item.
    /// </summary>
    public async Task RegisterAsEditor(string contentType, Guid contentId)
    {
        var userId = GetUserId();
        var userName = GetUserName();
        var connectionId = Context.ConnectionId;

        _logger.LogInformation(
            "User {UserId} ({UserName}) registering as editor for {ContentType} {ContentId}",
            userId, userName, contentType, contentId);

        var groupName = GetPresenceGroupName(contentType, contentId);
        await Groups.AddToGroupAsync(connectionId, groupName);

        var editor = await _activeEditorService.RegisterEditorAsync(
            contentType, contentId, userId, userName, connectionId);

        // Notify others in the group
        await Clients.OthersInGroup(groupName).EditorJoined(new EditorJoinedMessage
        {
            ContentType = contentType,
            ContentId = contentId,
            Editor = editor
        });

        // Send current editors to the joining user
        var editors = await _activeEditorService.GetEditorsAsync(contentId, contentType);
        await Clients.Caller.ActiveEditorsUpdated(new ActiveEditorsMessage
        {
            ContentType = contentType,
            ContentId = contentId,
            Editors = editors.Editors
        });
    }

    /// <summary>
    /// Unregister as an active editor.
    /// </summary>
    public async Task UnregisterAsEditor(string contentType, Guid contentId)
    {
        var userId = GetUserId();
        var connectionId = Context.ConnectionId;
        var groupName = GetPresenceGroupName(contentType, contentId);

        await Groups.RemoveFromGroupAsync(connectionId, groupName);
        await _activeEditorService.UnregisterEditorAsync(contentId, userId);

        await Clients.OthersInGroup(groupName).EditorLeft(new EditorLeftMessage
        {
            ContentType = contentType,
            ContentId = contentId,
            UserId = userId
        });

        _logger.LogInformation(
            "User {UserId} unregistered as editor for {ContentType} {ContentId}",
            userId, contentType, contentId);
    }

    /// <summary>
    /// Update editor focus (which block the user is editing).
    /// </summary>
    public async Task UpdateEditorFocus(string contentType, Guid contentId, Guid? focusedBlockId, bool isTyping)
    {
        var userId = GetUserId();
        var groupName = GetPresenceGroupName(contentType, contentId);

        await _activeEditorService.UpdateEditorFocusAsync(contentId, userId, focusedBlockId, isTyping);

        await Clients.OthersInGroup(groupName).EditorFocusUpdated(new EditorFocusMessage
        {
            ContentType = contentType,
            ContentId = contentId,
            UserId = userId,
            FocusedBlockId = focusedBlockId,
            IsTyping = isTyping
        });
    }

    /// <summary>
    /// Editor heartbeat to maintain active status.
    /// </summary>
    public async Task EditorHeartbeat(string contentType, Guid contentId)
    {
        var userId = GetUserId();
        await _activeEditorService.HeartbeatAsync(contentId, userId);
    }

    /// <summary>
    /// Get active editors for a content item.
    /// </summary>
    public async Task GetActiveEditors(string contentType, Guid contentId)
    {
        var editors = await _activeEditorService.GetEditorsAsync(contentId, contentType);

        await Clients.Caller.ActiveEditorsUpdated(new ActiveEditorsMessage
        {
            ContentType = contentType,
            ContentId = contentId,
            Editors = editors.Editors
        });
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        _logger.LogInformation("User {UserId} connected to collaboration hub", userId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        var connectionId = Context.ConnectionId;

        _logger.LogInformation(
            "User {UserId} disconnected from collaboration hub. Exception: {Exception}",
            userId, exception?.Message);

        // Find and leave all sessions this connection was part of
        var sessions = await _collaborationService.GetUserSessionsAsync(userId, connectionId);
        foreach (var articleId in sessions)
        {
            var groupName = GetGroupName(articleId);

            await _collaborationService.LeaveSessionAsync(articleId, userId);

            await Clients.OthersInGroup(groupName).ParticipantLeft(new ParticipantLeftMessage
            {
                UserId = userId
            });
        }

        // Phase 3D: Clean up active editor registrations for this connection
        await _activeEditorService.UnregisterByConnectionAsync(connectionId);

        await base.OnDisconnectedAsync(exception);
    }

    private Guid GetUserId()
    {
        var userIdClaim = Context.User?.FindFirst("sub")?.Value
            ?? Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;

        return Guid.Empty;
    }

    private string GetUserName()
    {
        return Context.User?.FindFirst("name")?.Value
            ?? Context.User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
            ?? "Anonymous";
    }

    private static string GetGroupName(Guid articleId) => $"article_{articleId}";
    private static string GetPresenceGroupName(string contentType, Guid contentId) => $"presence_{contentType}_{contentId}";
}

/// <summary>
/// Client interface for SignalR hub callbacks.
/// </summary>
public interface ICollaborationClient
{
    /// <summary>Called when successfully joined a session</summary>
    Task SessionJoined(SessionJoinedMessage message);

    /// <summary>Called when another participant joins</summary>
    Task ParticipantJoined(ParticipantMessage message);

    /// <summary>Called when a participant leaves</summary>
    Task ParticipantLeft(ParticipantLeftMessage message);

    /// <summary>Called when receiving a CRDT update</summary>
    Task SyncUpdate(SyncUpdateMessage message);

    /// <summary>Called when another user's cursor moves</summary>
    Task CursorUpdated(CursorUpdateMessage message);

    /// <summary>Called when another user's selection changes</summary>
    Task SelectionUpdated(SelectionUpdateMessage message);

    /// <summary>Called when awareness state changes</summary>
    Task AwarenessUpdated(AwarenessUpdateMessage message);

    /// <summary>Called in response to state sync request</summary>
    Task StateSynced(StateSyncMessage message);

    // Phase 3D: Active editor indicator callbacks

    /// <summary>Called when an editor joins a content item</summary>
    Task EditorJoined(EditorJoinedMessage message);

    /// <summary>Called when an editor leaves a content item</summary>
    Task EditorLeft(EditorLeftMessage message);

    /// <summary>Called when the list of active editors is updated</summary>
    Task ActiveEditorsUpdated(ActiveEditorsMessage message);

    /// <summary>Called when an editor's focus/typing state changes</summary>
    Task EditorFocusUpdated(EditorFocusMessage message);
}

// Message DTOs

public record SessionJoinedMessage
{
    public Guid ArticleId { get; init; }
    public string SessionId { get; init; } = string.Empty;
    public byte[]? State { get; init; }
    public IReadOnlyList<ParticipantDto> Participants { get; init; } = Array.Empty<ParticipantDto>();
}

public record ParticipantMessage
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
}

public record ParticipantLeftMessage
{
    public Guid UserId { get; init; }
}

public record SyncUpdateMessage
{
    public Guid ArticleId { get; init; }
    public byte[] Update { get; init; } = Array.Empty<byte>();
    public Guid SenderId { get; init; }
}

public record CursorUpdateMessage
{
    public Guid UserId { get; init; }
    public Guid? BlockId { get; init; }
    public int? Position { get; init; }
}

public record SelectionUpdateMessage
{
    public Guid UserId { get; init; }
    public SelectionRange? Selection { get; init; }
}

public record SelectionRange
{
    public Guid StartBlockId { get; init; }
    public int StartOffset { get; init; }
    public Guid EndBlockId { get; init; }
    public int EndOffset { get; init; }
}

public record AwarenessUpdateMessage
{
    public Guid UserId { get; init; }
    public AwarenessState Awareness { get; init; } = new();
}

public record AwarenessState
{
    public bool IsTyping { get; init; }
    public string? FocusedBlockId { get; init; }
    public string? Status { get; init; } // "editing", "viewing", "idle"
}

public record StateSyncMessage
{
    public Guid ArticleId { get; init; }
    public byte[]? State { get; init; }
    public IReadOnlyList<ParticipantDto> Participants { get; init; } = Array.Empty<ParticipantDto>();
}

// Phase 3D: Active Editor Indicator Messages

public record EditorJoinedMessage
{
    public string ContentType { get; init; } = string.Empty;
    public Guid ContentId { get; init; }
    public ActiveEditorDto Editor { get; init; } = null!;
}

public record EditorLeftMessage
{
    public string ContentType { get; init; } = string.Empty;
    public Guid ContentId { get; init; }
    public Guid UserId { get; init; }
}

public record ActiveEditorsMessage
{
    public string ContentType { get; init; } = string.Empty;
    public Guid ContentId { get; init; }
    public IReadOnlyList<ActiveEditorDto> Editors { get; init; } = Array.Empty<ActiveEditorDto>();
}

public record EditorFocusMessage
{
    public string ContentType { get; init; } = string.Empty;
    public Guid ContentId { get; init; }
    public Guid UserId { get; init; }
    public Guid? FocusedBlockId { get; init; }
    public bool IsTyping { get; init; }
}
