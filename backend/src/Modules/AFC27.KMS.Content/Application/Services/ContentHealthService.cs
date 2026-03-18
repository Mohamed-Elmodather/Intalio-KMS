using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Content.Application.DTOs;
using AFC27.KMS.Content.Domain.Entities;

namespace AFC27.KMS.Content.Application.Services;

/// <summary>
/// Service interface for content health scoring and monitoring.
/// </summary>
public interface IContentHealthService
{
    /// <summary>
    /// Calculate the health score for a single article.
    /// </summary>
    Task<ContentHealthDto> CalculateHealthScoreAsync(Guid articleId, CancellationToken ct = default);

    /// <summary>
    /// Calculate health scores for all articles (used by background job).
    /// </summary>
    Task<int> RecalculateAllHealthScoresAsync(CancellationToken ct = default);

    /// <summary>
    /// Get aggregated health summary metrics.
    /// </summary>
    Task<ContentHealthSummaryDto> GetHealthSummaryAsync(CancellationToken ct = default);

    /// <summary>
    /// Get stale articles (health score below threshold).
    /// </summary>
    Task<IReadOnlyList<StaleContentDto>> GetStaleContentAsync(double threshold = 0.4, int limit = 50, CancellationToken ct = default);

    /// <summary>
    /// Get top-performing content by health score.
    /// </summary>
    Task<IReadOnlyList<ContentHealthDto>> GetTopContentAsync(int limit = 20, CancellationToken ct = default);
}

/// <summary>
/// Implementation of content health scoring.
/// Health Score = weighted average of:
///   Freshness (0.3) + Verification (0.3) + Engagement (0.2) + Completeness (0.1) + Quality (0.1)
/// </summary>
public class ContentHealthService : IContentHealthService
{
    private readonly DbContext _dbContext;
    private readonly ILogger<ContentHealthService> _logger;

    // Score weights
    private const double FreshnessWeight = 0.3;
    private const double VerificationWeight = 0.3;
    private const double EngagementWeight = 0.2;
    private const double CompletenessWeight = 0.1;
    private const double QualityWeight = 0.1;

    // Freshness decay: content older than this is considered fully stale
    private const int MaxFreshnessDays = 365;

    // Engagement: articles with this many views or more get max engagement score
    private const int MaxViewsForFullEngagement = 5000;

    // Minimum content length for good quality
    private const int MinContentLengthForQuality = 500;

    public ContentHealthService(DbContext dbContext, ILogger<ContentHealthService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ContentHealthDto> CalculateHealthScoreAsync(
        Guid articleId, CancellationToken ct = default)
    {
        var article = await _dbContext.Set<Article>()
            .AsNoTracking()
            .Include(a => a.Category)
            .Include(a => a.Tags)
            .FirstOrDefaultAsync(a => a.Id == articleId, ct)
            ?? throw new KeyNotFoundException($"Article {articleId} not found");

        var breakdown = CalculateBreakdown(article);
        var healthScore = CalculateWeightedScore(breakdown);

        return MapToHealthDto(article, healthScore, breakdown);
    }

    public async Task<int> RecalculateAllHealthScoresAsync(CancellationToken ct = default)
    {
        var articles = await _dbContext.Set<Article>()
            .Include(a => a.Category)
            .Include(a => a.Tags)
            .ToListAsync(ct);

        var count = 0;
        foreach (var article in articles)
        {
            var breakdown = CalculateBreakdown(article);
            var healthScore = CalculateWeightedScore(breakdown);

            article.SetHealthScore(healthScore);
            count++;
        }

        if (count > 0)
        {
            await _dbContext.SaveChangesAsync(ct);
        }

        _logger.LogInformation("Recalculated health scores for {Count} articles", count);
        return count;
    }

    public async Task<ContentHealthSummaryDto> GetHealthSummaryAsync(CancellationToken ct = default)
    {
        var articles = await _dbContext.Set<Article>()
            .AsNoTracking()
            .Select(a => new { a.HealthScore })
            .ToListAsync(ct);

        var totalCount = articles.Count;
        var scoredArticles = articles.Where(a => a.HealthScore.HasValue).ToList();
        var averageScore = scoredArticles.Any() ? scoredArticles.Average(a => a.HealthScore!.Value) : 0;

        var healthyCount = scoredArticles.Count(a => a.HealthScore >= 0.7);
        var needsAttentionCount = scoredArticles.Count(a => a.HealthScore >= 0.4 && a.HealthScore < 0.7);
        var staleCount = scoredArticles.Count(a => a.HealthScore < 0.4);
        var unscoredCount = articles.Count(a => !a.HealthScore.HasValue);

        // Build distribution buckets
        var distribution = new List<HealthBucket>();
        for (double start = 0.0; start < 1.0; start += 0.1)
        {
            var rangeStart = Math.Round(start, 1);
            var rangeEnd = Math.Round(start + 0.1, 1);
            var count = scoredArticles.Count(a =>
                a.HealthScore >= rangeStart && a.HealthScore < rangeEnd);

            distribution.Add(new HealthBucket
            {
                RangeStart = rangeStart,
                RangeEnd = rangeEnd,
                Count = count
            });
        }

        return new ContentHealthSummaryDto
        {
            TotalArticles = totalCount,
            AverageHealthScore = Math.Round(averageScore, 4),
            HealthyCount = healthyCount,
            NeedsAttentionCount = needsAttentionCount,
            StaleCount = staleCount,
            UnscoreCount = unscoredCount,
            Distribution = distribution
        };
    }

    public async Task<IReadOnlyList<StaleContentDto>> GetStaleContentAsync(
        double threshold = 0.4, int limit = 50, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        var staleArticles = await _dbContext.Set<Article>()
            .AsNoTracking()
            .Include(a => a.Category)
            .Where(a => a.HealthScore.HasValue && a.HealthScore < threshold)
            .OrderBy(a => a.HealthScore)
            .Take(limit)
            .ToListAsync(ct);

        return staleArticles.Select(a =>
        {
            var lastUpdate = a.ModifiedAt ?? a.CreatedAt;
            var daysSinceUpdate = (int)(now - lastUpdate).TotalDays;

            return new StaleContentDto
            {
                ArticleId = a.Id,
                Title = a.Title.English,
                TitleArabic = a.Title.Arabic,
                Slug = a.Slug,
                HealthScore = a.HealthScore,
                Status = a.Status.ToString(),
                VerificationStatus = a.VerificationStatus.ToString(),
                OwnerName = a.OwnerName,
                OwnerId = a.OwnerId,
                CategoryName = a.Category?.Name.English,
                LastVerifiedAt = a.LastVerifiedAt,
                PublishedAt = a.PublishedAt,
                UpdatedAt = a.ModifiedAt,
                ViewCount = a.ViewCount,
                DaysSinceUpdate = daysSinceUpdate,
                StaleReason = DetermineStaleReason(a)
            };
        }).ToList();
    }

    public async Task<IReadOnlyList<ContentHealthDto>> GetTopContentAsync(
        int limit = 20, CancellationToken ct = default)
    {
        var topArticles = await _dbContext.Set<Article>()
            .AsNoTracking()
            .Include(a => a.Category)
            .Include(a => a.Tags)
            .Where(a => a.HealthScore.HasValue)
            .OrderByDescending(a => a.HealthScore)
            .Take(limit)
            .ToListAsync(ct);

        return topArticles.Select(a =>
        {
            var breakdown = CalculateBreakdown(a);
            return MapToHealthDto(a, a.HealthScore!.Value, breakdown);
        }).ToList();
    }

    // ========================================
    // Score Calculation
    // ========================================

    private HealthScoreBreakdown CalculateBreakdown(Article article)
    {
        return new HealthScoreBreakdown
        {
            Freshness = CalculateFreshness(article),
            Verification = CalculateVerification(article),
            Engagement = CalculateEngagement(article),
            Completeness = CalculateCompleteness(article),
            Quality = CalculateQuality(article)
        };
    }

    private static double CalculateWeightedScore(HealthScoreBreakdown breakdown)
    {
        var score = (breakdown.Freshness * FreshnessWeight)
                  + (breakdown.Verification * VerificationWeight)
                  + (breakdown.Engagement * EngagementWeight)
                  + (breakdown.Completeness * CompletenessWeight)
                  + (breakdown.Quality * QualityWeight);

        return Math.Round(Math.Clamp(score, 0.0, 1.0), 4);
    }

    /// <summary>
    /// Freshness: how recently the content was updated or published.
    /// Decays linearly over MaxFreshnessDays.
    /// </summary>
    private static double CalculateFreshness(Article article)
    {
        var now = DateTime.UtcNow;
        var lastActivity = article.ModifiedAt ?? article.PublishedAt ?? article.CreatedAt;
        var daysSinceActivity = (now - lastActivity).TotalDays;

        if (daysSinceActivity <= 0) return 1.0;
        if (daysSinceActivity >= MaxFreshnessDays) return 0.0;

        return 1.0 - (daysSinceActivity / MaxFreshnessDays);
    }

    /// <summary>
    /// Verification: based on the current verification status.
    /// Verified = 1.0, DueSoon = 0.6, Unverified = 0.3, Overdue = 0.0
    /// </summary>
    private static double CalculateVerification(Article article)
    {
        return article.VerificationStatus switch
        {
            VerificationStatus.Verified => 1.0,
            VerificationStatus.DueSoon => 0.6,
            VerificationStatus.Unverified => 0.3,
            VerificationStatus.Overdue => 0.0,
            _ => 0.3
        };
    }

    /// <summary>
    /// Engagement: based on view count, normalized against a max threshold.
    /// </summary>
    private static double CalculateEngagement(Article article)
    {
        if (article.ViewCount <= 0) return 0.0;
        if (article.ViewCount >= MaxViewsForFullEngagement) return 1.0;

        return (double)article.ViewCount / MaxViewsForFullEngagement;
    }

    /// <summary>
    /// Completeness: checks presence of bilingual content, summary, category, tags, owner.
    /// </summary>
    private static double CalculateCompleteness(Article article)
    {
        var checks = 0;
        var passed = 0;

        // Has English content
        checks++;
        if (!string.IsNullOrWhiteSpace(article.Content.English)) passed++;

        // Has Arabic content
        checks++;
        if (!string.IsNullOrWhiteSpace(article.Content.Arabic)) passed++;

        // Has summary
        checks++;
        if (article.Summary != null && !string.IsNullOrWhiteSpace(article.Summary.English)) passed++;

        // Has category
        checks++;
        if (article.CategoryId.HasValue) passed++;

        // Has tags
        checks++;
        if (article.Tags.Count > 0) passed++;

        // Has owner assigned
        checks++;
        if (article.OwnerId.HasValue) passed++;

        return checks > 0 ? (double)passed / checks : 0.0;
    }

    /// <summary>
    /// Quality: based on content length and structural indicators.
    /// </summary>
    private static double CalculateQuality(Article article)
    {
        var score = 0.0;

        // Content length
        var contentLength = article.Content.English?.Length ?? 0;
        if (contentLength >= MinContentLengthForQuality)
            score += 0.5;
        else if (contentLength > 0)
            score += 0.5 * ((double)contentLength / MinContentLengthForQuality);

        // Has featured image
        if (!string.IsNullOrWhiteSpace(article.FeaturedImageUrl))
            score += 0.2;

        // Is published (not draft)
        if (article.Status == ArticleStatus.Published)
            score += 0.3;

        return Math.Min(1.0, score);
    }

    // ========================================
    // Helpers
    // ========================================

    private static string DetermineStaleReason(Article article)
    {
        if (article.VerificationStatus == VerificationStatus.Overdue)
            return "Verification overdue";

        var lastUpdate = article.ModifiedAt ?? article.CreatedAt;
        var daysSinceUpdate = (DateTime.UtcNow - lastUpdate).TotalDays;

        if (daysSinceUpdate > 180)
            return $"Not updated in {(int)daysSinceUpdate} days";

        if (article.VerificationStatus == VerificationStatus.Unverified)
            return "Never verified";

        if (article.ViewCount == 0)
            return "No engagement (zero views)";

        if (article.HealthScore.HasValue && article.HealthScore < 0.2)
            return "Multiple health indicators are poor";

        return "Low overall health score";
    }

    private static ContentHealthDto MapToHealthDto(
        Article article, double healthScore, HealthScoreBreakdown breakdown)
    {
        return new ContentHealthDto
        {
            ArticleId = article.Id,
            Title = article.Title.English,
            TitleArabic = article.Title.Arabic,
            Slug = article.Slug,
            HealthScore = healthScore,
            HealthScoreCalculatedAt = article.HealthScoreCalculatedAt ?? DateTime.UtcNow,
            Status = article.Status.ToString(),
            VerificationStatus = article.VerificationStatus.ToString(),
            OwnerName = article.OwnerName,
            CategoryName = article.Category?.Name.English,
            PublishedAt = article.PublishedAt,
            LastVerifiedAt = article.LastVerifiedAt,
            UpdatedAt = article.ModifiedAt,
            ViewCount = article.ViewCount,
            Breakdown = breakdown
        };
    }
}
