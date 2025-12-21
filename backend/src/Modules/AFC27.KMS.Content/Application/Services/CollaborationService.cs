using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Content.Application.DTOs;
using AFC27.KMS.Content.Application.Interfaces;
using AFC27.KMS.Content.Domain.Entities;

namespace AFC27.KMS.Content.Application.Services;

/// <summary>
/// Service for managing real-time collaboration sessions.
/// </summary>
public class CollaborationService : ICollaborationService
{
    private readonly DbContext _dbContext;
    private readonly ICRDTService _crdtService;
    private readonly ILogger<CollaborationService> _logger;

    public CollaborationService(
        DbContext dbContext,
        ICRDTService crdtService,
        ILogger<CollaborationService> logger)
    {
        _dbContext = dbContext;
        _crdtService = crdtService;
        _logger = logger;
    }

    public async Task<CollaborationSessionDto> JoinSessionAsync(
        Guid articleId,
        Guid userId,
        string userName,
        string connectionId,
        CancellationToken cancellationToken = default)
    {
        // Find or create an active session
        var session = await _dbContext.Set<CollaborationSession>()
            .Include(s => s.Participants)
            .FirstOrDefaultAsync(s => s.ArticleId == articleId && s.IsActive, cancellationToken);

        if (session == null)
        {
            // Create new session
            session = CollaborationSession.Create(articleId);

            // Initialize CRDT state with existing blocks
            var blocks = await _dbContext.Set<ContentBlock>()
                .Where(b => b.ArticleId == articleId && !b.IsDeleted)
                .OrderBy(b => b.Order)
                .ToListAsync(cancellationToken);

            // Create initial CRDT document
            var initialState = _crdtService.CreateDocument();
            session.UpdateState(initialState);

            _dbContext.Set<CollaborationSession>().Add(session);

            _logger.LogInformation(
                "Created new collaboration session {SessionId} for article {ArticleId}",
                session.SessionId, articleId);
        }

        // Find existing participant or create new one
        var participant = session.Participants.FirstOrDefault(p => p.UserId == userId);
        if (participant != null)
        {
            // Update existing participant's connection
            participant.UpdateConnection(connectionId);
        }
        else
        {
            // Add new participant
            participant = CollaborationParticipant.Create(session.Id, userId, userName, connectionId);
            _dbContext.Set<CollaborationParticipant>().Add(participant);
        }

        session.UpdateActivity();
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "User {UserId} ({UserName}) joined session {SessionId}",
            userId, userName, session.SessionId);

        return MapToDto(session);
    }

    public async Task LeaveSessionAsync(
        Guid articleId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var session = await _dbContext.Set<CollaborationSession>()
            .Include(s => s.Participants)
            .FirstOrDefaultAsync(s => s.ArticleId == articleId && s.IsActive, cancellationToken);

        if (session == null)
            return;

        var participant = session.Participants.FirstOrDefault(p => p.UserId == userId);
        if (participant != null)
        {
            participant.Disconnect();
        }

        // Check if session is now empty
        var activeCount = session.Participants.Count(p => p.IsActive);
        if (activeCount == 0)
        {
            // End session after a grace period (handled by cleanup job)
            _logger.LogInformation(
                "Session {SessionId} has no active participants",
                session.SessionId);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "User {UserId} left session {SessionId}",
            userId, session.SessionId);
    }

    public async Task<byte[]?> GetSessionStateAsync(
        Guid articleId,
        CancellationToken cancellationToken = default)
    {
        var session = await _dbContext.Set<CollaborationSession>()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.ArticleId == articleId && s.IsActive, cancellationToken);

        return session?.CrdtState;
    }

    public async Task ApplyUpdateAsync(
        Guid articleId,
        byte[] update,
        CancellationToken cancellationToken = default)
    {
        var session = await _dbContext.Set<CollaborationSession>()
            .FirstOrDefaultAsync(s => s.ArticleId == articleId && s.IsActive, cancellationToken);

        if (session == null)
        {
            _logger.LogWarning(
                "Attempted to apply update to non-existent session for article {ArticleId}",
                articleId);
            return;
        }

        // Apply CRDT update
        var currentState = session.CrdtState ?? _crdtService.CreateDocument();
        var newState = _crdtService.ApplyUpdate(currentState, update);
        session.UpdateState(newState);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Guid>> GetUserSessionsAsync(
        Guid userId,
        string connectionId,
        CancellationToken cancellationToken = default)
    {
        var articleIds = await _dbContext.Set<CollaborationParticipant>()
            .AsNoTracking()
            .Where(p => p.UserId == userId && p.ConnectionId == connectionId && p.IsActive)
            .Join(
                _dbContext.Set<CollaborationSession>().Where(s => s.IsActive),
                p => p.SessionId,
                s => s.Id,
                (p, s) => s.ArticleId)
            .ToListAsync(cancellationToken);

        return articleIds;
    }

    public async Task<CollaborationSessionDto?> GetActiveSessionAsync(
        Guid articleId,
        CancellationToken cancellationToken = default)
    {
        var session = await _dbContext.Set<CollaborationSession>()
            .AsNoTracking()
            .Include(s => s.Participants.Where(p => p.IsActive))
            .FirstOrDefaultAsync(s => s.ArticleId == articleId && s.IsActive, cancellationToken);

        return session == null ? null : MapToDto(session);
    }

    public async Task EndSessionAsync(
        Guid articleId,
        CancellationToken cancellationToken = default)
    {
        var session = await _dbContext.Set<CollaborationSession>()
            .Include(s => s.Participants)
            .FirstOrDefaultAsync(s => s.ArticleId == articleId && s.IsActive, cancellationToken);

        if (session == null)
            return;

        // Persist CRDT state to content blocks
        await PersistSessionStateAsync(session, cancellationToken);

        session.End();
        foreach (var participant in session.Participants)
        {
            participant.Disconnect();
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Ended collaboration session {SessionId} for article {ArticleId}",
            session.SessionId, articleId);
    }

    public async Task CleanupInactiveSessionsAsync(
        TimeSpan inactivityThreshold,
        CancellationToken cancellationToken = default)
    {
        var cutoff = DateTime.UtcNow - inactivityThreshold;

        var inactiveSessions = await _dbContext.Set<CollaborationSession>()
            .Include(s => s.Participants)
            .Where(s => s.IsActive && s.LastActivityAt < cutoff)
            .ToListAsync(cancellationToken);

        foreach (var session in inactiveSessions)
        {
            // Check if all participants are inactive
            var allInactive = session.Participants.All(p => !p.IsActive || p.LastSeenAt < cutoff);

            if (allInactive)
            {
                await PersistSessionStateAsync(session, cancellationToken);
                session.End();

                _logger.LogInformation(
                    "Cleaned up inactive session {SessionId} for article {ArticleId}",
                    session.SessionId, session.ArticleId);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task PersistSessionStateAsync(
        CollaborationSession session,
        CancellationToken cancellationToken)
    {
        if (session.CrdtState == null || session.CrdtState.Length == 0)
            return;

        // Extract blocks from CRDT state
        var crdtBlocks = _crdtService.GetBlocks(session.CrdtState);

        // Get existing blocks
        var existingBlocks = await _dbContext.Set<ContentBlock>()
            .Where(b => b.ArticleId == session.ArticleId)
            .ToListAsync(cancellationToken);

        // Sync blocks from CRDT state to database
        var crdtBlockIds = crdtBlocks.Select(b => b.Id).ToHashSet();

        foreach (var crdtBlock in crdtBlocks)
        {
            var existingBlock = existingBlocks.FirstOrDefault(b => b.Id.ToString() == crdtBlock.Id);

            if (existingBlock != null)
            {
                // Update existing block
                existingBlock.UpdateContent(crdtBlock.Content);
                existingBlock.UpdateOrder(crdtBlock.Order);
                if (crdtBlock.Metadata != null)
                    existingBlock.UpdateMetadata(crdtBlock.Metadata);
            }
            else if (Guid.TryParse(crdtBlock.Id, out var blockId))
            {
                // Create new block
                var blockType = Enum.TryParse<BlockType>(crdtBlock.Type, out var type)
                    ? type
                    : BlockType.Paragraph;

                Guid? parentId = null;
                if (!string.IsNullOrEmpty(crdtBlock.ParentId) && Guid.TryParse(crdtBlock.ParentId, out var pid))
                    parentId = pid;

                var newBlock = ContentBlock.Create(
                    session.ArticleId,
                    blockType,
                    crdtBlock.Content,
                    crdtBlock.Order,
                    metadata: crdtBlock.Metadata,
                    parentBlockId: parentId);

                _dbContext.Set<ContentBlock>().Add(newBlock);
            }
        }

        // Soft-delete blocks that are no longer in CRDT state
        foreach (var existingBlock in existingBlocks)
        {
            if (!crdtBlockIds.Contains(existingBlock.Id.ToString()) && !existingBlock.IsDeleted)
            {
                existingBlock.Delete();
            }
        }
    }

    private static CollaborationSessionDto MapToDto(CollaborationSession session)
    {
        return new CollaborationSessionDto
        {
            Id = session.Id,
            ArticleId = session.ArticleId,
            SessionId = session.SessionId,
            IsActive = session.IsActive,
            CreatedAt = session.CreatedAt,
            LastActivityAt = session.LastActivityAt,
            Participants = session.Participants
                .Where(p => p.IsActive)
                .Select(p => new ParticipantDto
                {
                    UserId = p.UserId,
                    UserName = p.UserName,
                    AvatarUrl = p.AvatarUrl,
                    Color = p.Color,
                    IsActive = p.IsActive,
                    JoinedAt = p.JoinedAt,
                    LastSeenAt = p.LastSeenAt,
                    Cursor = p.CursorBlockId.HasValue || p.CursorPosition.HasValue
                        ? new CursorPositionDto
                        {
                            BlockId = p.CursorBlockId,
                            Position = p.CursorPosition
                        }
                        : null
                })
                .ToList()
        };
    }
}
