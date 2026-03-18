using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Content.Application.DTOs;
using AFC27.KMS.Content.Application.Interfaces;
using AFC27.KMS.Content.Domain.Entities;

namespace AFC27.KMS.Content.Application.Services;

/// <summary>
/// Service for managing active editor tracking and presence indicators (Phase 3D).
/// </summary>
public class ActiveEditorService : IActiveEditorService
{
    private readonly DbContext _dbContext;
    private readonly ILogger<ActiveEditorService> _logger;

    public ActiveEditorService(
        DbContext dbContext,
        ILogger<ActiveEditorService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ActiveEditorDto> RegisterEditorAsync(
        string contentType,
        Guid contentId,
        Guid userId,
        string userName,
        string connectionId,
        string? userAvatarUrl = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default)
    {
        // Check if user is already registered for this content
        var existing = await _dbContext.Set<ActiveEditor>()
            .FirstOrDefaultAsync(e =>
                e.ContentId == contentId &&
                e.UserId == userId &&
                e.Status != EditorStatus.Disconnected,
                cancellationToken);

        if (existing != null)
        {
            existing.Reconnect(connectionId);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Editor {UserId} reconnected to {ContentType} {ContentId}",
                userId, contentType, contentId);

            return MapToDto(existing);
        }

        var editor = ActiveEditor.Create(
            contentType, contentId, userId, userName,
            connectionId, userAvatarUrl, userAgent);

        _dbContext.Set<ActiveEditor>().Add(editor);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Editor {UserId} ({UserName}) registered for {ContentType} {ContentId}",
            userId, userName, contentType, contentId);

        return MapToDto(editor);
    }

    public async Task UnregisterEditorAsync(
        Guid contentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var editor = await _dbContext.Set<ActiveEditor>()
            .FirstOrDefaultAsync(e =>
                e.ContentId == contentId &&
                e.UserId == userId &&
                e.Status != EditorStatus.Disconnected,
                cancellationToken);

        if (editor != null)
        {
            editor.MarkDisconnected();
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Editor {UserId} unregistered from content {ContentId}",
                userId, contentId);
        }
    }

    public async Task UnregisterByConnectionAsync(
        string connectionId,
        CancellationToken cancellationToken = default)
    {
        var editors = await _dbContext.Set<ActiveEditor>()
            .Where(e => e.ConnectionId == connectionId && e.Status != EditorStatus.Disconnected)
            .ToListAsync(cancellationToken);

        foreach (var editor in editors)
        {
            editor.MarkDisconnected();
        }

        if (editors.Any())
        {
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Unregistered {Count} editor(s) for disconnected connection {ConnectionId}",
                editors.Count, connectionId);
        }
    }

    public async Task<ContentEditorsDto> GetEditorsAsync(
        Guid contentId,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var editors = await _dbContext.Set<ActiveEditor>()
            .AsNoTracking()
            .Where(e =>
                e.ContentId == contentId &&
                e.Status != EditorStatus.Disconnected)
            .OrderBy(e => e.StartedAt)
            .ToListAsync(cancellationToken);

        return new ContentEditorsDto
        {
            ContentId = contentId,
            ContentType = contentType,
            EditorCount = editors.Count,
            Editors = editors.Select(MapToDto).ToList()
        };
    }

    public async Task UpdateEditorFocusAsync(
        Guid contentId,
        Guid userId,
        Guid? focusedBlockId,
        bool isTyping,
        CancellationToken cancellationToken = default)
    {
        var editor = await _dbContext.Set<ActiveEditor>()
            .FirstOrDefaultAsync(e =>
                e.ContentId == contentId &&
                e.UserId == userId &&
                e.Status != EditorStatus.Disconnected,
                cancellationToken);

        if (editor != null)
        {
            editor.UpdateFocus(focusedBlockId);
            editor.SetTyping(isTyping);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task HeartbeatAsync(
        Guid contentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var editor = await _dbContext.Set<ActiveEditor>()
            .FirstOrDefaultAsync(e =>
                e.ContentId == contentId &&
                e.UserId == userId &&
                e.Status != EditorStatus.Disconnected,
                cancellationToken);

        if (editor != null)
        {
            editor.UpdateActivity();
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IReadOnlyList<ContentPresenceIndicator>> GetPresenceIndicatorsAsync(
        IReadOnlyList<Guid> contentIds,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var editors = await _dbContext.Set<ActiveEditor>()
            .AsNoTracking()
            .Where(e =>
                contentIds.Contains(e.ContentId) &&
                e.ContentType == contentType &&
                e.Status != EditorStatus.Disconnected)
            .ToListAsync(cancellationToken);

        return contentIds
            .Select(contentId =>
            {
                var contentEditors = editors.Where(e => e.ContentId == contentId).ToList();
                return new ContentPresenceIndicator
                {
                    ContentId = contentId,
                    ContentType = contentType,
                    ActiveEditorCount = contentEditors.Count,
                    Editors = contentEditors.Select(e => new EditorBadgeDto
                    {
                        UserId = e.UserId,
                        UserName = e.UserName,
                        AvatarUrl = e.UserAvatarUrl,
                        Color = e.Color
                    }).ToList()
                };
            })
            .Where(p => p.ActiveEditorCount > 0)
            .ToList();
    }

    public async Task MarkIdleEditorsAsync(
        TimeSpan idleThreshold,
        CancellationToken cancellationToken = default)
    {
        var cutoff = DateTime.UtcNow - idleThreshold;

        var idleEditors = await _dbContext.Set<ActiveEditor>()
            .Where(e => e.Status == EditorStatus.Active && e.LastActivityAt < cutoff)
            .ToListAsync(cancellationToken);

        foreach (var editor in idleEditors)
        {
            editor.MarkIdle();
            _logger.LogDebug("Marked editor {UserId} as idle for content {ContentId}", editor.UserId, editor.ContentId);
        }

        if (idleEditors.Any())
            await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CleanupDisconnectedEditorsAsync(
        TimeSpan disconnectThreshold,
        CancellationToken cancellationToken = default)
    {
        var cutoff = DateTime.UtcNow - disconnectThreshold;

        var disconnectedEditors = await _dbContext.Set<ActiveEditor>()
            .Where(e =>
                e.Status == EditorStatus.Disconnected &&
                e.LastActivityAt < cutoff)
            .ToListAsync(cancellationToken);

        if (disconnectedEditors.Any())
        {
            _dbContext.Set<ActiveEditor>().RemoveRange(disconnectedEditors);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Cleaned up {Count} disconnected editor records",
                disconnectedEditors.Count);
        }
    }

    private static ActiveEditorDto MapToDto(ActiveEditor editor)
    {
        return new ActiveEditorDto
        {
            Id = editor.Id,
            ContentType = editor.ContentType,
            ContentId = editor.ContentId,
            UserId = editor.UserId,
            UserName = editor.UserName,
            UserAvatarUrl = editor.UserAvatarUrl,
            Color = editor.Color,
            Status = editor.Status.ToString(),
            FocusedBlockId = editor.FocusedBlockId,
            IsTyping = editor.IsTyping,
            StartedAt = editor.StartedAt,
            LastActivityAt = editor.LastActivityAt
        };
    }
}
