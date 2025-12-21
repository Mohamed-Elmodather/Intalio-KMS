using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Content.Application.Interfaces;
using AFC27.KMS.Content.Application.DTOs;

namespace AFC27.KMS.Content.Infrastructure.Hubs;

/// <summary>
/// SignalR hub for real-time document collaboration.
/// Handles CRDT synchronization, presence, and cursor updates.
/// </summary>
[Authorize]
public class CollaborationHub : Hub<ICollaborationClient>
{
    private readonly ICollaborationService _collaborationService;
    private readonly IPresenceService _presenceService;
    private readonly ILogger<CollaborationHub> _logger;

    public CollaborationHub(
        ICollaborationService collaborationService,
        IPresenceService presenceService,
        ILogger<CollaborationHub> logger)
    {
        _collaborationService = collaborationService;
        _presenceService = presenceService;
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
