using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Content.Application.DTOs;
using AFC27.KMS.Content.Application.Interfaces;
using AFC27.KMS.Content.Domain.Entities;

namespace AFC27.KMS.Content.Application.Services;

/// <summary>
/// Service for managing user presence and cursor positions in collaboration sessions.
/// </summary>
public class PresenceService : IPresenceService
{
    private readonly DbContext _dbContext;
    private readonly ILogger<PresenceService> _logger;

    public PresenceService(
        DbContext dbContext,
        ILogger<PresenceService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task UpdateCursorAsync(
        Guid articleId,
        Guid userId,
        Guid? blockId,
        int? position,
        CancellationToken cancellationToken = default)
    {
        var participant = await GetActiveParticipantAsync(articleId, userId, cancellationToken);

        if (participant == null)
        {
            _logger.LogWarning(
                "Attempted to update cursor for user {UserId} not in session for article {ArticleId}",
                userId, articleId);
            return;
        }

        participant.UpdateCursor(blockId, position);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogDebug(
            "Updated cursor for user {UserId} in article {ArticleId}: Block={BlockId}, Position={Position}",
            userId, articleId, blockId, position);
    }

    public async Task<IReadOnlyList<ParticipantDto>> GetParticipantsAsync(
        Guid articleId,
        CancellationToken cancellationToken = default)
    {
        var session = await _dbContext.Set<CollaborationSession>()
            .AsNoTracking()
            .Include(s => s.Participants.Where(p => p.IsActive))
            .FirstOrDefaultAsync(s => s.ArticleId == articleId && s.IsActive, cancellationToken);

        if (session == null)
            return Array.Empty<ParticipantDto>();

        return session.Participants
            .Select(MapToDto)
            .ToList();
    }

    public async Task<bool> IsUserActiveAsync(
        Guid articleId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<CollaborationParticipant>()
            .AsNoTracking()
            .AnyAsync(p =>
                p.UserId == userId &&
                p.IsActive &&
                p.Session.ArticleId == articleId &&
                p.Session.IsActive,
                cancellationToken);
    }

    public async Task<int> GetParticipantCountAsync(
        Guid articleId,
        CancellationToken cancellationToken = default)
    {
        var session = await _dbContext.Set<CollaborationSession>()
            .AsNoTracking()
            .Include(s => s.Participants)
            .FirstOrDefaultAsync(s => s.ArticleId == articleId && s.IsActive, cancellationToken);

        return session?.Participants.Count(p => p.IsActive) ?? 0;
    }

    /// <summary>
    /// Get all cursor positions for a session.
    /// </summary>
    public async Task<IReadOnlyList<CursorStateDto>> GetCursorsAsync(
        Guid articleId,
        CancellationToken cancellationToken = default)
    {
        var participants = await _dbContext.Set<CollaborationParticipant>()
            .AsNoTracking()
            .Where(p =>
                p.Session.ArticleId == articleId &&
                p.Session.IsActive &&
                p.IsActive)
            .ToListAsync(cancellationToken);

        return participants
            .Where(p => p.CursorBlockId.HasValue || p.CursorPosition.HasValue)
            .Select(p => new CursorStateDto
            {
                UserId = p.UserId,
                UserName = p.UserName,
                Color = p.Color,
                BlockId = p.CursorBlockId,
                Position = p.CursorPosition
            })
            .ToList();
    }

    /// <summary>
    /// Update participant's last seen time (heartbeat).
    /// </summary>
    public async Task HeartbeatAsync(
        Guid articleId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var participant = await GetActiveParticipantAsync(articleId, userId, cancellationToken);

        if (participant != null)
        {
            // Just touch the participant to update LastSeenAt
            participant.UpdateCursor(participant.CursorBlockId, participant.CursorPosition);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Mark inactive participants based on last seen time.
    /// </summary>
    public async Task MarkInactiveParticipantsAsync(
        TimeSpan inactivityThreshold,
        CancellationToken cancellationToken = default)
    {
        var cutoff = DateTime.UtcNow - inactivityThreshold;

        var inactiveParticipants = await _dbContext.Set<CollaborationParticipant>()
            .Where(p => p.IsActive && p.LastSeenAt < cutoff)
            .ToListAsync(cancellationToken);

        foreach (var participant in inactiveParticipants)
        {
            participant.Disconnect();
            _logger.LogInformation(
                "Marked participant {UserId} as inactive due to inactivity",
                participant.UserId);
        }

        if (inactiveParticipants.Any())
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<CollaborationParticipant?> GetActiveParticipantAsync(
        Guid articleId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Set<CollaborationParticipant>()
            .FirstOrDefaultAsync(p =>
                p.UserId == userId &&
                p.IsActive &&
                p.Session.ArticleId == articleId &&
                p.Session.IsActive,
                cancellationToken);
    }

    private static ParticipantDto MapToDto(CollaborationParticipant participant)
    {
        return new ParticipantDto
        {
            UserId = participant.UserId,
            UserName = participant.UserName,
            AvatarUrl = participant.AvatarUrl,
            Color = participant.Color,
            IsActive = participant.IsActive,
            JoinedAt = participant.JoinedAt,
            LastSeenAt = participant.LastSeenAt,
            Cursor = participant.CursorBlockId.HasValue || participant.CursorPosition.HasValue
                ? new CursorPositionDto
                {
                    BlockId = participant.CursorBlockId,
                    Position = participant.CursorPosition
                }
                : null
        };
    }
}
