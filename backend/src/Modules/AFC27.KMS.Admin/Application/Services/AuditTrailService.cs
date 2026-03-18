using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Application.Interfaces;
using AFC27.KMS.Admin.Domain.Entities;

namespace AFC27.KMS.Admin.Application.Services;

/// <summary>
/// Service interface for the enhanced audit trail.
/// </summary>
public interface IAuditTrailService
{
    Task<PagedResult<AuditTrailEntryDto>> QueryAsync(AuditTrailQueryRequest request, CancellationToken cancellationToken = default);
    Task<AuditTrailSummaryDto> GetSummaryAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<AuditExportResultDto> ExportAsync(AuditTrailQueryRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Enhanced audit trail service with rich filtering and export support.
/// </summary>
public class AuditTrailService : IAuditTrailService
{
    private readonly DbContext _dbContext;
    private readonly ILogger<AuditTrailService> _logger;

    public AuditTrailService(
        DbContext dbContext,
        ILogger<AuditTrailService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<PagedResult<AuditTrailEntryDto>> QueryAsync(
        AuditTrailQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(request);

        var totalCount = await query.CountAsync(cancellationToken);

        // Sorting
        query = request.SortBy?.ToLowerInvariant() switch
        {
            "action" => request.SortDescending ? query.OrderByDescending(l => l.Action) : query.OrderBy(l => l.Action),
            "category" => request.SortDescending ? query.OrderByDescending(l => l.Category) : query.OrderBy(l => l.Category),
            "severity" => request.SortDescending ? query.OrderByDescending(l => l.Severity) : query.OrderBy(l => l.Severity),
            "username" => request.SortDescending ? query.OrderByDescending(l => l.UserName) : query.OrderBy(l => l.UserName),
            _ => request.SortDescending ? query.OrderByDescending(l => l.Timestamp) : query.OrderBy(l => l.Timestamp)
        };

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(l => MapToDto(l))
            .ToListAsync(cancellationToken);

        return new PagedResult<AuditTrailEntryDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<AuditTrailSummaryDto> GetSummaryAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<AuditLogEntry>().AsNoTracking()
            .Where(l => l.Timestamp >= from && l.Timestamp <= to);

        var totalEntries = await query.CountAsync(cancellationToken);
        var successCount = await query.CountAsync(l => l.IsSuccess, cancellationToken);
        var criticalCount = await query.CountAsync(l => l.Severity >= AuditSeverity.Critical, cancellationToken);

        var byCategory = await query
            .GroupBy(l => l.Category)
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Key, g => g.Count, cancellationToken);

        var byAction = await query
            .GroupBy(l => l.Action)
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(20)
            .ToDictionaryAsync(g => g.Key, g => g.Count, cancellationToken);

        var bySeverity = await query
            .GroupBy(l => l.Severity)
            .Select(g => new { Key = g.Key.ToString(), Count = g.Count() })
            .ToDictionaryAsync(g => g.Key, g => g.Count, cancellationToken);

        var byEntityType = await query
            .Where(l => !string.IsNullOrEmpty(l.EntityType))
            .GroupBy(l => l.EntityType)
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Key, g => g.Count, cancellationToken);

        var timeline = await query
            .GroupBy(l => l.Timestamp.Date)
            .Select(g => new AuditTimelinePointDto
            {
                Timestamp = g.Key,
                Count = g.Count(),
                SuccessCount = g.Count(l => l.IsSuccess),
                FailureCount = g.Count(l => !l.IsSuccess)
            })
            .OrderBy(t => t.Timestamp)
            .ToListAsync(cancellationToken);

        var topUsers = await query
            .Where(l => l.UserId.HasValue)
            .GroupBy(l => new { l.UserId, l.UserName, l.UserEmail })
            .Select(g => new AuditUserSummaryDto
            {
                UserId = g.Key.UserId ?? Guid.Empty,
                UserName = g.Key.UserName ?? "Unknown",
                UserEmail = g.Key.UserEmail,
                ActionCount = g.Count(),
                FailureCount = g.Count(l => !l.IsSuccess),
                LastActivity = g.Max(l => l.Timestamp)
            })
            .OrderByDescending(u => u.ActionCount)
            .Take(10)
            .ToListAsync(cancellationToken);

        return new AuditTrailSummaryDto
        {
            TotalEntries = totalEntries,
            SuccessCount = successCount,
            FailureCount = totalEntries - successCount,
            CriticalCount = criticalCount,
            ByCategory = byCategory,
            ByAction = byAction,
            BySeverity = bySeverity,
            ByEntityType = byEntityType,
            Timeline = timeline,
            TopUsers = topUsers
        };
    }

    public async Task<AuditExportResultDto> ExportAsync(
        AuditTrailQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(request);

        var entries = await query
            .OrderByDescending(l => l.Timestamp)
            .Take(10000) // Limit export size
            .ToListAsync(cancellationToken);

        var format = request.ExportFormat?.ToLowerInvariant() ?? "csv";

        return format switch
        {
            "json" => ExportToJson(entries),
            _ => ExportToCsv(entries)
        };
    }

    private IQueryable<AuditLogEntry> BuildQuery(AuditTrailQueryRequest request)
    {
        var query = _dbContext.Set<AuditLogEntry>().AsNoTracking().AsQueryable();

        if (request.From.HasValue)
            query = query.Where(l => l.Timestamp >= request.From.Value);
        if (request.To.HasValue)
            query = query.Where(l => l.Timestamp <= request.To.Value);
        if (!string.IsNullOrEmpty(request.Action))
            query = query.Where(l => l.Action == request.Action);
        if (!string.IsNullOrEmpty(request.Category))
            query = query.Where(l => l.Category == request.Category);
        if (!string.IsNullOrEmpty(request.EntityType))
            query = query.Where(l => l.EntityType == request.EntityType);
        if (request.EntityId.HasValue)
            query = query.Where(l => l.EntityId == request.EntityId.Value);
        if (request.UserId.HasValue)
            query = query.Where(l => l.UserId == request.UserId.Value);
        if (!string.IsNullOrEmpty(request.UserEmail))
            query = query.Where(l => l.UserEmail != null && l.UserEmail.Contains(request.UserEmail));
        if (!string.IsNullOrEmpty(request.UserName))
            query = query.Where(l => l.UserName != null && l.UserName.Contains(request.UserName));
        if (request.MinSeverity.HasValue)
            query = query.Where(l => l.Severity >= request.MinSeverity.Value);
        if (request.MaxSeverity.HasValue)
            query = query.Where(l => l.Severity <= request.MaxSeverity.Value);
        if (request.IsSuccess.HasValue)
            query = query.Where(l => l.IsSuccess == request.IsSuccess.Value);
        if (!string.IsNullOrEmpty(request.IpAddress))
            query = query.Where(l => l.IpAddress == request.IpAddress);
        if (!string.IsNullOrEmpty(request.CorrelationId))
            query = query.Where(l => l.CorrelationId == request.CorrelationId);
        if (!string.IsNullOrEmpty(request.SessionId))
            query = query.Where(l => l.SessionId == request.SessionId);

        return query;
    }

    private static AuditTrailEntryDto MapToDto(AuditLogEntry entry)
    {
        return new AuditTrailEntryDto
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
            UserAgent = entry.UserAgent,
            Severity = entry.Severity,
            IsSuccess = entry.IsSuccess,
            ErrorMessage = entry.ErrorMessage,
            OldValues = entry.OldValues,
            NewValues = entry.NewValues,
            AdditionalData = entry.AdditionalData,
            CorrelationId = entry.CorrelationId,
            SessionId = entry.SessionId
        };
    }

    private static AuditExportResultDto ExportToCsv(List<AuditLogEntry> entries)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Timestamp,Action,Category,EntityType,EntityId,UserId,UserName,UserEmail,IpAddress,Severity,IsSuccess,ErrorMessage");

        foreach (var entry in entries)
        {
            sb.AppendLine(
                $"\"{entry.Timestamp:O}\",\"{Escape(entry.Action)}\",\"{Escape(entry.Category)}\",\"{Escape(entry.EntityType)}\"," +
                $"\"{entry.EntityId}\",\"{entry.UserId}\",\"{Escape(entry.UserName)}\",\"{Escape(entry.UserEmail)}\"," +
                $"\"{Escape(entry.IpAddress)}\",\"{entry.Severity}\",\"{entry.IsSuccess}\",\"{Escape(entry.ErrorMessage)}\"");
        }

        var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());

        return new AuditExportResultDto
        {
            Format = "csv",
            Data = bytes,
            FileName = $"audit-trail-{DateTime.UtcNow:yyyyMMddHHmmss}.csv",
            ContentType = "text/csv",
            RecordCount = entries.Count
        };
    }

    private static AuditExportResultDto ExportToJson(List<AuditLogEntry> entries)
    {
        var dtos = entries.Select(MapToDto).ToList();
        var json = System.Text.Json.JsonSerializer.Serialize(dtos, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);

        return new AuditExportResultDto
        {
            Format = "json",
            Data = bytes,
            FileName = $"audit-trail-{DateTime.UtcNow:yyyyMMddHHmmss}.json",
            ContentType = "application/json",
            RecordCount = entries.Count
        };
    }

    private static string Escape(string? value)
    {
        return value?.Replace("\"", "\"\"") ?? string.Empty;
    }
}
