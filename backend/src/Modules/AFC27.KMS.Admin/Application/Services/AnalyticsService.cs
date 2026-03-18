using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Application.Interfaces;
using AFC27.KMS.Admin.Domain.Entities;

namespace AFC27.KMS.Admin.Application.Services;

/// <summary>
/// Service providing advanced analytics by aggregating audit logs and system data.
/// </summary>
public class AnalyticsService : IAnalyticsService
{
    private readonly DbContext _dbContext;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(
        DbContext dbContext,
        ILogger<AnalyticsService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<UsageMetricsDto> GetUsageMetricsAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculating usage metrics from {From} to {To}", from, to);

        var logs = _dbContext.Set<AuditLogEntry>().AsNoTracking()
            .Where(l => l.Timestamp >= from && l.Timestamp <= to);

        var totalLogins = await logs.CountAsync(l => l.Action == AuditActions.Login && l.IsSuccess, cancellationToken);
        var activeUserIds = await logs.Where(l => l.UserId.HasValue).Select(l => l.UserId!.Value).Distinct().CountAsync(cancellationToken);
        var totalApiCalls = await logs.CountAsync(cancellationToken);

        var dailyBreakdown = await logs
            .GroupBy(l => l.Timestamp.Date)
            .Select(g => new DailyUsageDto
            {
                Date = g.Key,
                ActiveUsers = g.Where(l => l.UserId.HasValue).Select(l => l.UserId).Distinct().Count(),
                Logins = g.Count(l => l.Action == AuditActions.Login && l.IsSuccess),
                PageViews = g.Count(l => l.Action == AuditActions.DocumentViewed || l.Category == AuditCategories.ContentManagement),
                ApiCalls = g.Count()
            })
            .OrderBy(d => d.Date)
            .ToListAsync(cancellationToken);

        return new UsageMetricsDto
        {
            PeriodStart = from,
            PeriodEnd = to,
            TotalActiveUsers = activeUserIds,
            TotalLogins = totalLogins,
            TotalPageViews = dailyBreakdown.Sum(d => d.PageViews),
            TotalApiCalls = totalApiCalls,
            AverageSessionDurationMinutes = 0, // Requires session tracking; placeholder
            DailyBreakdown = dailyBreakdown
        };
    }

    public async Task<EngagementTrendsDto> GetEngagementTrendsAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculating engagement trends from {From} to {To}", from, to);

        var logs = _dbContext.Set<AuditLogEntry>().AsNoTracking()
            .Where(l => l.Timestamp >= from && l.Timestamp <= to && l.IsSuccess);

        var contentEngagement = await logs
            .Where(l => !string.IsNullOrEmpty(l.EntityType))
            .GroupBy(l => l.EntityType)
            .Select(g => new ContentEngagementDto
            {
                ContentType = g.Key,
                Views = g.Count(l => l.Action.Contains("Viewed")),
                Edits = g.Count(l => l.Action.Contains("Updated") || l.Action.Contains("Edited")),
                Shares = g.Count(l => l.Action.Contains("Shared")),
                Comments = g.Count(l => l.Action.Contains("Comment")),
                Downloads = g.Count(l => l.Action.Contains("Downloaded"))
            })
            .OrderByDescending(c => c.Views)
            .Take(10)
            .ToListAsync(cancellationToken);

        var topContributors = await logs
            .Where(l => l.UserId.HasValue && (
                l.Action == AuditActions.ArticleCreated ||
                l.Action == AuditActions.DocumentCreated))
            .GroupBy(l => new { l.UserId, l.UserName })
            .Select(g => new TopContributorDto
            {
                UserId = g.Key.UserId ?? Guid.Empty,
                DisplayName = g.Key.UserName ?? "Unknown",
                Contributions = g.Count(),
                ArticlesCreated = g.Count(l => l.Action == AuditActions.ArticleCreated),
                DocumentsUploaded = g.Count(l => l.Action == AuditActions.DocumentCreated),
                CommentsPosted = 0
            })
            .OrderByDescending(c => c.Contributions)
            .Take(10)
            .ToListAsync(cancellationToken);

        return new EngagementTrendsDto
        {
            PeriodStart = from,
            PeriodEnd = to,
            ContentEngagement = contentEngagement,
            MostViewedContent = Array.Empty<TopContentDto>(), // Requires content join; placeholder
            MostSharedContent = Array.Empty<TopContentDto>(),
            TopContributors = topContributors
        };
    }

    public async Task<SearchAnalyticsDto> GetSearchAnalyticsAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculating search analytics from {From} to {To}", from, to);

        // Search events are tracked via audit logs with a "Search" category
        var searchLogs = _dbContext.Set<AuditLogEntry>().AsNoTracking()
            .Where(l => l.Timestamp >= from && l.Timestamp <= to &&
                        l.Category == "Search");

        var totalSearches = await searchLogs.CountAsync(cancellationToken);
        var uniqueSearchers = await searchLogs
            .Where(l => l.UserId.HasValue)
            .Select(l => l.UserId)
            .Distinct()
            .CountAsync(cancellationToken);

        var dailyBreakdown = await searchLogs
            .GroupBy(l => l.Timestamp.Date)
            .Select(g => new DailySearchCountDto
            {
                Date = g.Key,
                Count = g.Count(),
                UniqueUsers = g.Where(l => l.UserId.HasValue).Select(l => l.UserId).Distinct().Count()
            })
            .OrderBy(d => d.Date)
            .ToListAsync(cancellationToken);

        return new SearchAnalyticsDto
        {
            PeriodStart = from,
            PeriodEnd = to,
            TotalSearches = totalSearches,
            UniqueSearchers = uniqueSearchers,
            AverageResultsReturned = 0, // Requires result count tracking; placeholder
            SearchSuccessRate = totalSearches > 0 ? (double)await searchLogs.CountAsync(l => l.IsSuccess, cancellationToken) / totalSearches : 0,
            TopSearchTerms = Array.Empty<TopSearchTermDto>(), // Requires search term storage; placeholder
            TopFailedSearches = Array.Empty<FailedSearchDto>(),
            DailyBreakdown = dailyBreakdown
        };
    }

    public async Task<AIUsageStatsDto> GetAIUsageStatsAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculating AI usage stats from {From} to {To}", from, to);

        var aiLogs = _dbContext.Set<AuditLogEntry>().AsNoTracking()
            .Where(l => l.Timestamp >= from && l.Timestamp <= to &&
                        l.Category == "AI");

        var totalRequests = await aiLogs.CountAsync(cancellationToken);
        var uniqueUsers = await aiLogs
            .Where(l => l.UserId.HasValue)
            .Select(l => l.UserId)
            .Distinct()
            .CountAsync(cancellationToken);
        var successCount = await aiLogs.CountAsync(l => l.IsSuccess, cancellationToken);

        var dailyBreakdown = await aiLogs
            .GroupBy(l => l.Timestamp.Date)
            .Select(g => new DailyAIUsageDto
            {
                Date = g.Key,
                Requests = g.Count(),
                TokensConsumed = 0, // Requires token tracking in additional data
                UniqueUsers = g.Where(l => l.UserId.HasValue).Select(l => l.UserId).Distinct().Count()
            })
            .OrderBy(d => d.Date)
            .ToListAsync(cancellationToken);

        return new AIUsageStatsDto
        {
            PeriodStart = from,
            PeriodEnd = to,
            TotalAIRequests = totalRequests,
            UniqueAIUsers = uniqueUsers,
            TotalTokensConsumed = 0, // Requires aggregation from AI usage logs
            AverageResponseTimeMs = 0,
            SuccessRate = totalRequests > 0 ? (double)successCount / totalRequests * 100 : 0,
            OperationBreakdown = Array.Empty<AIOperationStatsDto>(),
            DailyBreakdown = dailyBreakdown
        };
    }

    public async Task<ContentGrowthDto> GetContentGrowthAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculating content growth from {From} to {To}", from, to);

        var logs = _dbContext.Set<AuditLogEntry>().AsNoTracking()
            .Where(l => l.Timestamp >= from && l.Timestamp <= to && l.IsSuccess);

        var newDocuments = await logs.CountAsync(l => l.Action == AuditActions.DocumentCreated, cancellationToken);
        var newArticles = await logs.CountAsync(l => l.Action == AuditActions.ArticleCreated, cancellationToken);

        var growthTimeline = await logs
            .Where(l => l.Action == AuditActions.DocumentCreated || l.Action == AuditActions.ArticleCreated)
            .GroupBy(l => l.Timestamp.Date)
            .Select(g => new ContentGrowthPointDto
            {
                Date = g.Key,
                CumulativeDocuments = g.Count(l => l.Action == AuditActions.DocumentCreated),
                CumulativeArticles = g.Count(l => l.Action == AuditActions.ArticleCreated),
                CumulativeStorageBytes = 0
            })
            .OrderBy(g => g.Date)
            .ToListAsync(cancellationToken);

        return new ContentGrowthDto
        {
            PeriodStart = from,
            PeriodEnd = to,
            TotalDocuments = 0, // Requires cross-module query
            TotalArticles = 0,
            NewDocuments = newDocuments,
            NewArticles = newArticles,
            TotalStorageBytes = 0,
            StorageGrowthBytes = 0,
            GrowthTimeline = growthTimeline,
            TypeBreakdown = Array.Empty<ContentTypeBreakdownDto>()
        };
    }
}
