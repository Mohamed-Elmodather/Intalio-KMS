using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using AFC27.KMS.Admin.Domain.Entities;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Application.Interfaces;
using AFC27.KMS.SharedKernel.Interfaces;

using AFC27.KMS.Identity.Domain.Entities;

namespace AFC27.KMS.Admin.Application.Services;

/// <summary>
/// Service for admin impersonation of users.
/// </summary>
public class ImpersonationService : IImpersonationService
{
    private readonly DbContext _dbContext;
    private readonly ICurrentUser _currentUser;
    private readonly IAuditLogService _auditLogService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ImpersonationService> _logger;

    public ImpersonationService(
        DbContext dbContext,
        ICurrentUser currentUser,
        IAuditLogService auditLogService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<ImpersonationService> logger)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _auditLogService = auditLogService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<ImpersonationSessionDto> StartAsync(
        Guid targetUserId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        // Check if admin already has an active session
        var existingSession = await _dbContext.Set<ImpersonationSession>()
            .FirstOrDefaultAsync(s => s.AdminUserId == _currentUser.UserId && s.IsActive,
                cancellationToken);

        if (existingSession != null)
            throw new InvalidOperationException("You already have an active impersonation session");

        // Get target user
        var targetUser = await _dbContext.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == targetUserId, cancellationToken);

        if (targetUser == null)
            throw new InvalidOperationException($"User {targetUserId} not found");

        // Cannot impersonate yourself
        if (targetUserId == _currentUser.UserId)
            throw new InvalidOperationException("You cannot impersonate yourself");

        var httpContext = _httpContextAccessor.HttpContext;
        var ipAddress = GetClientIpAddress(httpContext);

        var session = ImpersonationSession.Create(
            _currentUser.UserId ?? Guid.Empty,
            _currentUser.DisplayName ?? "Admin",
            targetUserId,
            targetUser.DisplayName,
            reason,
            ipAddress);

        _dbContext.Set<ImpersonationSession>().Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogActionAsync(
            AuditActions.ImpersonationStarted,
            AuditCategories.SecurityEvent,
            "User",
            targetUserId,
            additionalData: new
            {
                AdminUserId = _currentUser.UserId,
                TargetUserId = targetUserId,
                Reason = reason
            },
            severity: AuditSeverity.Warning,
            cancellationToken: cancellationToken);

        _logger.LogWarning(
            "Admin {AdminUserId} started impersonating user {TargetUserId}. Reason: {Reason}",
            _currentUser.UserId, targetUserId, reason);

        return MapToDto(session);
    }

    public async Task<bool> EndAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        var session = await _dbContext.Set<ImpersonationSession>()
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.IsActive, cancellationToken);

        if (session == null)
            return false;

        // Only the admin who started the session can end it
        if (session.AdminUserId != _currentUser.UserId)
            throw new InvalidOperationException("You can only end your own impersonation sessions");

        session.End();
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogActionAsync(
            AuditActions.ImpersonationEnded,
            AuditCategories.SecurityEvent,
            "User",
            session.ImpersonatedUserId,
            additionalData: new
            {
                SessionId = sessionId,
                Duration = (session.EndedAt - session.StartedAt)?.TotalMinutes
            },
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "Admin {AdminUserId} ended impersonation of user {TargetUserId}",
            session.AdminUserId, session.ImpersonatedUserId);

        return true;
    }

    public async Task<ImpersonationSessionDto?> GetActiveSessionAsync(
        Guid adminUserId,
        CancellationToken cancellationToken = default)
    {
        var session = await _dbContext.Set<ImpersonationSession>()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.AdminUserId == adminUserId && s.IsActive,
                cancellationToken);

        return session == null ? null : MapToDto(session);
    }

    public async Task<IReadOnlyList<ImpersonationSessionDto>> GetHistoryAsync(
        Guid? adminUserId = null,
        Guid? targetUserId = null,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<ImpersonationSession>().AsNoTracking();

        if (adminUserId.HasValue)
            query = query.Where(s => s.AdminUserId == adminUserId.Value);

        if (targetUserId.HasValue)
            query = query.Where(s => s.ImpersonatedUserId == targetUserId.Value);

        if (from.HasValue)
            query = query.Where(s => s.StartedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(s => s.StartedAt <= to.Value);

        return await query
            .OrderByDescending(s => s.StartedAt)
            .Take(100)
            .Select(s => MapToDto(s))
            .ToListAsync(cancellationToken);
    }

    public async Task LogActionAsync(
        Guid sessionId,
        string action,
        CancellationToken cancellationToken = default)
    {
        var session = await _dbContext.Set<ImpersonationSession>()
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.IsActive, cancellationToken);

        if (session == null)
            return;

        session.LogAction(action);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string? GetClientIpAddress(HttpContext? httpContext)
    {
        if (httpContext == null) return null;

        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',').First().Trim();
        }

        return httpContext.Connection.RemoteIpAddress?.ToString();
    }

    private static ImpersonationSessionDto MapToDto(ImpersonationSession session)
    {
        List<string>? actions = null;
        if (!string.IsNullOrEmpty(session.Actions))
        {
            try
            {
                actions = JsonSerializer.Deserialize<List<string>>(session.Actions);
            }
            catch { }
        }

        return new ImpersonationSessionDto
        {
            Id = session.Id,
            AdminUserId = session.AdminUserId,
            AdminUserName = session.AdminUserName,
            ImpersonatedUserId = session.ImpersonatedUserId,
            ImpersonatedUserName = session.ImpersonatedUserName,
            Reason = session.Reason,
            StartedAt = session.StartedAt,
            EndedAt = session.EndedAt,
            IsActive = session.IsActive,
            IpAddress = session.IpAddress,
            Actions = actions
        };
    }
}
