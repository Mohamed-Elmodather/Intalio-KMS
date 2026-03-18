using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Content.Application.DTOs;
using AFC27.KMS.Content.Application.Services;
using AFC27.KMS.Content.Domain.Entities;
using AFC27.KMS.Contracts.Common;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.Content.Presentation.Controllers;

/// <summary>
/// Controller for content health scoring, stale content detection, and the
/// Knowledge Health Dashboard API.
/// </summary>
[ApiController]
[Route("api/content/health")]
[Authorize]
public class ContentHealthController : ControllerBase
{
    private readonly IContentHealthService _healthService;
    private readonly DbContext _dbContext;
    private readonly ILogger<ContentHealthController> _logger;

    public ContentHealthController(
        IContentHealthService healthService,
        DbContext dbContext,
        ILogger<ContentHealthController> logger)
    {
        _healthService = healthService;
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Get aggregated content health metrics.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<ContentHealthSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ContentHealthSummaryDto>>> GetHealthSummary(
        CancellationToken cancellationToken)
    {
        var summary = await _healthService.GetHealthSummaryAsync(cancellationToken);
        return Ok(ApiResponse<ContentHealthSummaryDto>.Ok(summary));
    }

    /// <summary>
    /// Get stale articles with health score below threshold (default 0.4).
    /// </summary>
    [HttpGet("stale")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<StaleContentDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<StaleContentDto>>>> GetStaleContent(
        [FromQuery] double threshold = 0.4,
        [FromQuery] int limit = 50,
        CancellationToken cancellationToken = default)
    {
        var staleContent = await _healthService.GetStaleContentAsync(threshold, limit, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<StaleContentDto>>.Ok(staleContent));
    }

    /// <summary>
    /// Get highest-performing content by health score.
    /// </summary>
    [HttpGet("top")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ContentHealthDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ContentHealthDto>>>> GetTopContent(
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        var topContent = await _healthService.GetTopContentAsync(limit, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<ContentHealthDto>>.Ok(topContent));
    }

    /// <summary>
    /// Get health score for a specific article.
    /// </summary>
    [HttpGet("article/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ContentHealthDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ContentHealthDto>>> GetArticleHealth(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var health = await _healthService.CalculateHealthScoreAsync(id, cancellationToken);
            return Ok(ApiResponse<ContentHealthDto>.Ok(health));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<ContentHealthDto>.Fail("Article not found"));
        }
    }

    /// <summary>
    /// Trigger a manual recalculation of all health scores.
    /// </summary>
    [HttpPost("recalculate")]
    [Authorize(Policy = "CanManageContent")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> RecalculateHealthScores(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Manual health score recalculation triggered");

        var count = await _healthService.RecalculateAllHealthScoresAsync(cancellationToken);
        return Ok(ApiResponse.Ok($"Health scores recalculated for {count} articles"));
    }

    // ========================================
    // Phase 2G: Knowledge Health Dashboard
    // ========================================

    /// <summary>
    /// Get comprehensive knowledge health dashboard data including
    /// verification breakdown, freshness heatmap, coverage gaps,
    /// top contributors, orphaned content, and health trend.
    /// </summary>
    [HttpGet("dashboard")]
    [Authorize(Policy = "CanManageContent")]
    [ProducesResponseType(typeof(ApiResponse<KnowledgeHealthDashboardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<KnowledgeHealthDashboardDto>>> GetDashboard(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Knowledge health dashboard requested");

        var healthSummary = await _healthService.GetHealthSummaryAsync(cancellationToken);
        var verificationBreakdown = await GetVerificationBreakdownAsync(cancellationToken);
        var freshnessHeatmap = await GetFreshnessHeatmapAsync(cancellationToken);
        var coverageGaps = await GetCoverageGapsAsync(cancellationToken);
        var topContributors = await GetTopContributorsAsync(cancellationToken);
        var orphanedContent = await GetOrphanedContentAsync(cancellationToken);
        var healthTrend = await GetHealthTrendAsync(cancellationToken);

        var dashboard = new KnowledgeHealthDashboardDto
        {
            HealthSummary = healthSummary,
            VerificationBreakdown = verificationBreakdown,
            FreshnessHeatmap = freshnessHeatmap,
            CoverageGaps = coverageGaps,
            TopContributors = topContributors,
            OrphanedContent = orphanedContent,
            HealthTrend = healthTrend
        };

        return Ok(ApiResponse<KnowledgeHealthDashboardDto>.Ok(dashboard));
    }

    // ========================================
    // Dashboard Data Helpers
    // ========================================

    private async Task<VerificationStatusBreakdown> GetVerificationBreakdownAsync(CancellationToken ct)
    {
        var articles = _dbContext.Set<Article>().AsNoTracking();

        var totalCount = await articles.CountAsync(ct);
        var verified = await articles.CountAsync(a => a.VerificationStatus == VerificationStatus.Verified, ct);
        var unverified = await articles.CountAsync(a => a.VerificationStatus == VerificationStatus.Unverified, ct);
        var dueSoon = await articles.CountAsync(a => a.VerificationStatus == VerificationStatus.DueSoon, ct);
        var overdue = await articles.CountAsync(a => a.VerificationStatus == VerificationStatus.Overdue, ct);

        return new VerificationStatusBreakdown
        {
            Verified = verified,
            Unverified = unverified,
            DueSoon = dueSoon,
            Overdue = overdue,
            VerifiedPercentage = totalCount > 0 ? Math.Round((double)verified / totalCount * 100, 1) : 0
        };
    }

    private async Task<IReadOnlyList<FreshnessHeatmapEntry>> GetFreshnessHeatmapAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var twelveMonthsAgo = now.AddMonths(-12);

        var articles = await _dbContext.Set<Article>()
            .AsNoTracking()
            .Where(a => a.CreatedAt >= twelveMonthsAgo || a.ModifiedAt >= twelveMonthsAgo)
            .Select(a => new { a.CreatedAt, a.ModifiedAt })
            .ToListAsync(ct);

        var entries = new List<FreshnessHeatmapEntry>();
        for (int i = 11; i >= 0; i--)
        {
            var monthDate = now.AddMonths(-i);
            var monthStart = new DateTime(monthDate.Year, monthDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var monthEnd = monthStart.AddMonths(1);

            var created = articles.Count(a => a.CreatedAt >= monthStart && a.CreatedAt < monthEnd);
            var updated = articles.Count(a => a.ModifiedAt.HasValue &&
                                              a.ModifiedAt >= monthStart && a.ModifiedAt < monthEnd);

            entries.Add(new FreshnessHeatmapEntry
            {
                Year = monthStart.Year,
                Month = monthStart.Month,
                MonthLabel = monthStart.ToString("MMM yyyy"),
                ArticlesCreated = created,
                ArticlesUpdated = updated
            });
        }

        return entries;
    }

    private async Task<IReadOnlyList<CoverageGap>> GetCoverageGapsAsync(CancellationToken ct)
    {
        var categories = await _dbContext.Set<Category>()
            .AsNoTracking()
            .Select(c => new
            {
                c.Id,
                NameEn = c.Name.English,
                NameAr = c.Name.Arabic,
                ArticleCount = _dbContext.Set<Article>().Count(a => a.CategoryId == c.Id)
            })
            .OrderBy(c => c.ArticleCount)
            .Take(20)
            .ToListAsync(ct);

        return categories.Select(c => new CoverageGap
        {
            CategoryId = c.Id,
            CategoryName = c.NameEn,
            CategoryNameArabic = c.NameAr,
            ArticleCount = c.ArticleCount,
            Severity = c.ArticleCount == 0 ? "Critical" :
                       c.ArticleCount <= 2 ? "Warning" : "Info"
        }).ToList();
    }

    private async Task<IReadOnlyList<TopContributor>> GetTopContributorsAsync(CancellationToken ct)
    {
        // Get top authors by article count
        var authors = await _dbContext.Set<Article>()
            .AsNoTracking()
            .GroupBy(a => new { a.AuthorId, a.AuthorName })
            .Select(g => new
            {
                g.Key.AuthorId,
                g.Key.AuthorName,
                ArticleCount = g.Count(),
                AverageHealthScore = g.Average(a => a.HealthScore ?? 0.0)
            })
            .OrderByDescending(a => a.ArticleCount)
            .Take(10)
            .ToListAsync(ct);

        // Get verification counts per user
        var verificationCounts = await _dbContext.Set<VerificationRecord>()
            .AsNoTracking()
            .GroupBy(v => v.VerifiedById)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var verificationLookup = verificationCounts.ToDictionary(v => v.UserId, v => v.Count);

        return authors.Select(a => new TopContributor
        {
            UserId = a.AuthorId,
            UserName = a.AuthorName,
            ArticleCount = a.ArticleCount,
            VerificationCount = verificationLookup.GetValueOrDefault(a.AuthorId, 0),
            AverageHealthScore = Math.Round(a.AverageHealthScore, 4)
        }).ToList();
    }

    private async Task<IReadOnlyList<OrphanedContent>> GetOrphanedContentAsync(CancellationToken ct)
    {
        var orphaned = await _dbContext.Set<Article>()
            .AsNoTracking()
            .Include(a => a.Tags)
            .Where(a => !a.CategoryId.HasValue || !a.OwnerId.HasValue || !a.Tags.Any())
            .OrderByDescending(a => a.CreatedAt)
            .Take(50)
            .ToListAsync(ct);

        return orphaned.Select(a =>
        {
            var reasons = new List<string>();
            if (!a.CategoryId.HasValue) reasons.Add("NoCategory");
            if (!a.OwnerId.HasValue) reasons.Add("NoOwner");
            if (!a.Tags.Any()) reasons.Add("NoTags");

            return new OrphanedContent
            {
                ArticleId = a.Id,
                Title = a.Title.English,
                TitleArabic = a.Title.Arabic,
                Slug = a.Slug,
                Status = a.Status.ToString(),
                CreatedAt = a.CreatedAt,
                OrphanReason = string.Join(", ", reasons)
            };
        }).ToList();
    }

    private async Task<IReadOnlyList<HealthTrendEntry>> GetHealthTrendAsync(CancellationToken ct)
    {
        // For health trend, we use the current health scores as a proxy.
        // In a production system, you'd store historical scores.
        // Here we simulate by grouping articles by their HealthScoreCalculatedAt week.
        var now = DateTime.UtcNow;
        var twelveWeeksAgo = now.AddDays(-84);

        var articles = await _dbContext.Set<Article>()
            .AsNoTracking()
            .Where(a => a.HealthScore.HasValue)
            .Select(a => new { a.HealthScore, a.HealthScoreCalculatedAt, a.CreatedAt })
            .ToListAsync(ct);

        var entries = new List<HealthTrendEntry>();
        for (int i = 11; i >= 0; i--)
        {
            var weekStart = now.AddDays(-7 * (i + 1));
            var weekEnd = now.AddDays(-7 * i);

            // Use articles that existed by that week
            var articlesInScope = articles
                .Where(a => a.CreatedAt <= weekEnd)
                .ToList();

            var avgScore = articlesInScope.Any()
                ? articlesInScope.Average(a => a.HealthScore ?? 0)
                : 0;

            entries.Add(new HealthTrendEntry
            {
                WeekStart = weekStart,
                WeekLabel = $"W{12 - i} ({weekStart:MMM dd})",
                AverageHealthScore = Math.Round(avgScore, 4),
                ArticleCount = articlesInScope.Count
            });
        }

        return entries;
    }
}
