using AFC27.KMS.Content.Application.DTOs;

namespace AFC27.KMS.Content.Application.Interfaces;

/// <summary>
/// Service interface for managing active editor tracking (Phase 3D).
/// </summary>
public interface IActiveEditorService
{
    /// <summary>
    /// Register a user as an active editor for a content item.
    /// </summary>
    Task<ActiveEditorDto> RegisterEditorAsync(
        string contentType,
        Guid contentId,
        Guid userId,
        string userName,
        string connectionId,
        string? userAvatarUrl = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregister a user as an active editor.
    /// </summary>
    Task UnregisterEditorAsync(
        Guid contentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregister all editors for a given connection ID (on disconnect).
    /// </summary>
    Task UnregisterByConnectionAsync(
        string connectionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all active editors for a content item.
    /// </summary>
    Task<ContentEditorsDto> GetEditorsAsync(
        Guid contentId,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update the focus/typing state of an editor.
    /// </summary>
    Task UpdateEditorFocusAsync(
        Guid contentId,
        Guid userId,
        Guid? focusedBlockId,
        bool isTyping,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Record a heartbeat for an editor (keeps them active).
    /// </summary>
    Task HeartbeatAsync(
        Guid contentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get presence indicators for multiple content items (for list views).
    /// </summary>
    Task<IReadOnlyList<ContentPresenceIndicator>> GetPresenceIndicatorsAsync(
        IReadOnlyList<Guid> contentIds,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Mark idle editors based on inactivity threshold.
    /// </summary>
    Task MarkIdleEditorsAsync(
        TimeSpan idleThreshold,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove disconnected editors based on timeout.
    /// </summary>
    Task CleanupDisconnectedEditorsAsync(
        TimeSpan disconnectThreshold,
        CancellationToken cancellationToken = default);
}
