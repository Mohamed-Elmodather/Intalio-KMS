using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using AFC27.KMS.Admin.Domain.Entities;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Application.Interfaces;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.Admin.Application.Services;

/// <summary>
/// Service for audit logging operations.
/// </summary>
public class AuditLogService : IAuditLogService
{
    private readonly DbContext _dbContext;
    private readonly ICurrentUser _currentUser;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuditLogService> _logger;

    public AuditLogService(
        DbContext dbContext,
        ICurrentUser currentUser,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuditLogService> logger)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task LogAsync(AuditLogEntry entry, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<AuditLogEntry>().Add(entry);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Audit: {Action} on {EntityType} by {UserName}",
            entry.Action, entry.EntityType, entry.UserName);
    }

    public async Task LogActionAsync(
        string action,
        string category,
        string entityType,
        Guid? entityId = null,
        object? oldValues = null,
        object? newValues = null,
        object? additionalData = null,
        AuditSeverity severity = AuditSeverity.Info,
        CancellationToken cancellationToken = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        var entry = AuditLogEntry.Create(
            action: action,
            category: category,
            entityType: entityType,
            entityId: entityId,
            userId: _currentUser.UserId,
            userName: _currentUser.DisplayName,
            userEmail: _currentUser.Email,
            ipAddress: GetClientIpAddress(httpContext),
            userAgent: httpContext?.Request.Headers["User-Agent"].FirstOrDefault(),
            oldValues: oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
            newValues: newValues != null ? JsonSerializer.Serialize(newValues) : null,
            additionalData: additionalData != null ? JsonSerializer.Serialize(additionalData) : null,
            severity: severity,
            isSuccess: true,
            correlationId: httpContext?.TraceIdentifier,
            sessionId: httpContext?.Session?.Id);

        await LogAsync(entry, cancellationToken);
    }

    public async Task LogFailureAsync(
        string action,
        string category,
        string entityType,
        string errorMessage,
        Guid? entityId = null,
        AuditSeverity severity = AuditSeverity.Error,
        CancellationToken cancellationToken = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        var entry = AuditLogEntry.Create(
            action: action,
            category: category,
            entityType: entityType,
            entityId: entityId,
            userId: _currentUser.UserId,
            userName: _currentUser.DisplayName,
            userEmail: _currentUser.Email,
            ipAddress: GetClientIpAddress(httpContext),
            userAgent: httpContext?.Request.Headers["User-Agent"].FirstOrDefault(),
            severity: severity,
            isSuccess: false,
            errorMessage: errorMessage,
            correlationId: httpContext?.TraceIdentifier);

        await LogAsync(entry, cancellationToken);
    }

    public async Task<PagedResult<AuditLogDto>> SearchAsync(
        AuditLogSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<AuditLogEntry>().AsNoTracking();

        // Apply filters
        if (request.From.HasValue)
            query = query.Where(e => e.Timestamp >= request.From.Value);

        if (request.To.HasValue)
            query = query.Where(e => e.Timestamp <= request.To.Value);

        if (!string.IsNullOrEmpty(request.Action))
            query = query.Where(e => e.Action == request.Action);

        if (!string.IsNullOrEmpty(request.Category))
            query = query.Where(e => e.Category == request.Category);

        if (!string.IsNullOrEmpty(request.EntityType))
            query = query.Where(e => e.EntityType == request.EntityType);

        if (request.EntityId.HasValue)
            query = query.Where(e => e.EntityId == request.EntityId.Value);

        if (request.UserId.HasValue)
            query = query.Where(e => e.UserId == request.UserId.Value);

        if (request.MinSeverity.HasValue)
            query = query.Where(e => e.Severity >= request.MinSeverity.Value);

        if (request.IsSuccess.HasValue)
            query = query.Where(e => e.IsSuccess == request.IsSuccess.Value);

        if (!string.IsNullOrEmpty(request.SearchText))
        {
            query = query.Where(e =>
                (e.UserName != null && e.UserName.Contains(request.SearchText)) ||
                (e.Action != null && e.Action.Contains(request.SearchText)) ||
                (e.EntityType != null && e.EntityType.Contains(request.SearchText)));
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = request.SortBy switch
        {
            "Action" => request.SortDescending ? query.OrderByDescending(e => e.Action) : query.OrderBy(e => e.Action),
            "Category" => request.SortDescending ? query.OrderByDescending(e => e.Category) : query.OrderBy(e => e.Category),
            "UserName" => request.SortDescending ? query.OrderByDescending(e => e.UserName) : query.OrderBy(e => e.UserName),
            _ => request.SortDescending ? query.OrderByDescending(e => e.Timestamp) : query.OrderBy(e => e.Timestamp)
        };

        // Apply pagination
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => MapToDto(e))
            .ToListAsync(cancellationToken);

        return new PagedResult<AuditLogDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<IReadOnlyList<AuditLogDto>> GetEntityHistoryAsync(
        string entityType,
        Guid entityId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<AuditLogEntry>()
            .AsNoTracking()
            .Where(e => e.EntityType == entityType && e.EntityId == entityId)
            .OrderByDescending(e => e.Timestamp)
            .Select(e => MapToDto(e))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AuditLogDto>> GetUserActivityAsync(
        Guid userId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<AuditLogEntry>()
            .AsNoTracking()
            .Where(e => e.UserId == userId);

        if (from.HasValue)
            query = query.Where(e => e.Timestamp >= from.Value);

        if (to.HasValue)
            query = query.Where(e => e.Timestamp <= to.Value);

        return await query
            .OrderByDescending(e => e.Timestamp)
            .Take(100)
            .Select(e => MapToDto(e))
            .ToListAsync(cancellationToken);
    }

    public async Task<byte[]> ExportToCsvAsync(
        AuditLogSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        // Get all matching records (remove pagination)
        request = request with { Page = 1, PageSize = int.MaxValue };
        var result = await SearchAsync(request, cancellationToken);

        var csv = new StringBuilder();
        csv.AppendLine("Timestamp,Action,Category,EntityType,EntityId,UserId,UserName,UserEmail,IpAddress,Severity,IsSuccess,ErrorMessage");

        foreach (var item in result.Items)
        {
            csv.AppendLine($"\"{item.Timestamp:O}\",\"{item.Action}\",\"{item.Category}\",\"{item.EntityType}\",\"{item.EntityId}\",\"{item.UserId}\",\"{EscapeCsv(item.UserName)}\",\"{item.UserEmail}\",\"{item.IpAddress}\",\"{item.Severity}\",\"{item.IsSuccess}\",\"{EscapeCsv(item.ErrorMessage)}\"");
        }

        // Log the export
        await LogActionAsync(
            AuditActions.DataExported,
            AuditCategories.Compliance,
            "AuditLog",
            additionalData: new { ExportedCount = result.Items.Count },
            cancellationToken: cancellationToken);

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    public async Task<AuditStatisticsDto> GetStatisticsAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<AuditLogEntry>()
            .AsNoTracking()
            .Where(e => e.Timestamp >= from && e.Timestamp <= to);

        var totalEvents = await query.CountAsync(cancellationToken);
        var successfulEvents = await query.CountAsync(e => e.IsSuccess, cancellationToken);
        var failedEvents = totalEvents - successfulEvents;

        var eventsByCategory = await query
            .GroupBy(e => e.Category)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Category, x => x.Count, cancellationToken);

        var eventsByAction = await query
            .GroupBy(e => e.Action)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .Select(g => new { Action = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Action, x => x.Count, cancellationToken);

        var eventsBySeverity = await query
            .GroupBy(e => e.Severity)
            .Select(g => new { Severity = g.Key.ToString(), Count = g.Count() })
            .ToDictionaryAsync(x => x.Severity, x => x.Count, cancellationToken);

        var dailyBreakdown = await query
            .GroupBy(e => e.Timestamp.Date)
            .Select(g => new DailyAuditCountDto { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);

        var topActiveUsers = await query
            .Where(e => e.UserId.HasValue)
            .GroupBy(e => new { e.UserId, e.UserName })
            .OrderByDescending(g => g.Count())
            .Take(10)
            .Select(g => new TopUserActivityDto
            {
                UserId = g.Key.UserId!.Value,
                UserName = g.Key.UserName ?? "Unknown",
                ActionCount = g.Count()
            })
            .ToListAsync(cancellationToken);

        return new AuditStatisticsDto
        {
            TotalEvents = totalEvents,
            SuccessfulEvents = successfulEvents,
            FailedEvents = failedEvents,
            EventsByCategory = eventsByCategory,
            EventsByAction = eventsByAction,
            EventsBySeverity = eventsBySeverity,
            DailyBreakdown = dailyBreakdown,
            TopActiveUsers = topActiveUsers
        };
    }

    private static string? GetClientIpAddress(HttpContext? httpContext)
    {
        if (httpContext == null) return null;

        // Check for forwarded IP first (for proxies/load balancers)
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',').First().Trim();
        }

        return httpContext.Connection.RemoteIpAddress?.ToString();
    }

    private static string EscapeCsv(string? value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value.Replace("\"", "\"\"");
    }

    private static AuditLogDto MapToDto(AuditLogEntry entry)
    {
        return new AuditLogDto
        {
            Id = entry.Id,
            Timestamp = entry.Timestamp,
            Action = entry.Action,
            Category = entry.Category,
            EntityType = entry.EntityType,
            EntityId = entry.EntityId,
            UserId = entry.UserId,
            UserName = entry.UserName,
            UserEmail = entry.UserEmail,
            IpAddress = entry.IpAddress,
            Severity = entry.Severity.ToString(),
            IsSuccess = entry.IsSuccess,
            ErrorMessage = entry.ErrorMessage,
            OldValues = entry.OldValues,
            NewValues = entry.NewValues,
            AdditionalData = entry.AdditionalData
        };
    }
}
