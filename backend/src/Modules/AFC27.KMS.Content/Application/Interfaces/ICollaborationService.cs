using AFC27.KMS.Content.Application.DTOs;

namespace AFC27.KMS.Content.Application.Interfaces;

/// <summary>
/// Service interface for managing collaboration sessions.
/// </summary>
public interface ICollaborationService
{
    /// <summary>
    /// Join or create a collaboration session for an article.
    /// </summary>
    Task<CollaborationSessionDto> JoinSessionAsync(
        Guid articleId,
        Guid userId,
        string userName,
        string connectionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Leave a collaboration session.
    /// </summary>
    Task LeaveSessionAsync(
        Guid articleId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the current CRDT state for a session.
    /// </summary>
    Task<byte[]?> GetSessionStateAsync(
        Guid articleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Apply a CRDT update to the session state.
    /// </summary>
    Task ApplyUpdateAsync(
        Guid articleId,
        byte[] update,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all sessions a user is currently in.
    /// </summary>
    Task<IReadOnlyList<Guid>> GetUserSessionsAsync(
        Guid userId,
        string connectionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get active session for an article if one exists.
    /// </summary>
    Task<CollaborationSessionDto?> GetActiveSessionAsync(
        Guid articleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// End a collaboration session and persist the final state.
    /// </summary>
    Task EndSessionAsync(
        Guid articleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Clean up inactive sessions.
    /// </summary>
    Task CleanupInactiveSessionsAsync(
        TimeSpan inactivityThreshold,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for presence awareness.
/// </summary>
public interface IPresenceService
{
    /// <summary>
    /// Update a user's cursor position.
    /// </summary>
    Task UpdateCursorAsync(
        Guid articleId,
        Guid userId,
        Guid? blockId,
        int? position,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all active participants in a session.
    /// </summary>
    Task<IReadOnlyList<ParticipantDto>> GetParticipantsAsync(
        Guid articleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a user is active in a session.
    /// </summary>
    Task<bool> IsUserActiveAsync(
        Guid articleId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user count for a session.
    /// </summary>
    Task<int> GetParticipantCountAsync(
        Guid articleId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for block-based content editing.
/// </summary>
public interface IBlockEditorService
{
    /// <summary>
    /// Get all blocks for an article.
    /// </summary>
    Task<IReadOnlyList<ContentBlockDto>> GetBlocksAsync(
        Guid articleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new block.
    /// </summary>
    Task<ContentBlockDto> CreateBlockAsync(
        Guid articleId,
        CreateBlockRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update a block's content.
    /// </summary>
    Task<bool> UpdateBlockAsync(
        Guid blockId,
        UpdateBlockRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a block.
    /// </summary>
    Task<bool> DeleteBlockAsync(
        Guid blockId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reorder blocks.
    /// </summary>
    Task<bool> ReorderBlocksAsync(
        Guid articleId,
        IReadOnlyList<Guid> blockIds,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Move a block to a new position.
    /// </summary>
    Task<bool> MoveBlockAsync(
        Guid blockId,
        int newPosition,
        Guid? newParentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Duplicate a block.
    /// </summary>
    Task<ContentBlockDto?> DuplicateBlockAsync(
        Guid blockId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Convert blocks to HTML for rendering/export.
    /// </summary>
    Task<string> RenderToHtmlAsync(
        Guid articleId,
        string language = "en",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Import content from HTML.
    /// </summary>
    Task<IReadOnlyList<ContentBlockDto>> ImportFromHtmlAsync(
        Guid articleId,
        string html,
        Guid userId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for CRDT operations.
/// </summary>
public interface ICRDTService
{
    /// <summary>
    /// Create a new CRDT document.
    /// </summary>
    byte[] CreateDocument();

    /// <summary>
    /// Apply an update to a document.
    /// </summary>
    byte[] ApplyUpdate(byte[] document, byte[] update);

    /// <summary>
    /// Merge two documents.
    /// </summary>
    byte[] MergeDocuments(byte[] doc1, byte[] doc2);

    /// <summary>
    /// Get the current state vector for a document.
    /// </summary>
    byte[] GetStateVector(byte[] document);

    /// <summary>
    /// Compute a diff update from a state vector.
    /// </summary>
    byte[] ComputeUpdate(byte[] document, byte[] stateVector);

    /// <summary>
    /// Extract text content from a CRDT document.
    /// </summary>
    string GetTextContent(byte[] document, string key);

    /// <summary>
    /// Get all content blocks from a CRDT document.
    /// </summary>
    IReadOnlyList<CRDTBlockData> GetBlocks(byte[] document);
}

/// <summary>
/// Block data extracted from CRDT document.
/// </summary>
public record CRDTBlockData
{
    public string Id { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public int Order { get; init; }
    public string? Metadata { get; init; }
    public string? ParentId { get; init; }
}
