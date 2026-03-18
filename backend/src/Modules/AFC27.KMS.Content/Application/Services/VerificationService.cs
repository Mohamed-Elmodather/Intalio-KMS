using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Content.Application.DTOs;
using AFC27.KMS.Content.Application.Interfaces;
using AFC27.KMS.Content.Domain.Entities;

namespace AFC27.KMS.Content.Application.Services;

/// <summary>
/// Implementation of the Knowledge Verification Lifecycle service.
/// Handles article verification, ownership assignment, and dashboard analytics.
/// </summary>
public class VerificationService : IVerificationService
{
    private readonly DbContext _dbContext;
    private readonly ILogger<VerificationService> _logger;

    public VerificationService(DbContext dbContext, ILogger<VerificationService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<VerificationRecordDto> VerifyArticleAsync(
        Guid articleId, VerifyArticleRequest request, Guid verifiedById, string verifiedByName,
        CancellationToken ct = default)
    {
        var article = await _dbContext.Set<Article>()
            .FirstOrDefaultAsync(a => a.Id == articleId, ct)
            ?? throw new KeyNotFoundException($"Article {articleId} not found");

        var previousStatus = article.VerificationStatus;

        article.Verify(verifiedById, verifiedByName, request.ReviewIntervalDays);

        var record = VerificationRecord.Create(
            articleId,
            verifiedById,
            verifiedByName,
            previousStatus,
            VerificationStatus.Verified,
            article.NextVerificationDue,
            request.Notes);

        _dbContext.Set<VerificationRecord>().Add(record);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Article {ArticleId} verified by {VerifiedByName} ({VerifiedById}). " +
            "Previous status: {PreviousStatus}. Next due: {NextDue}",
            articleId, verifiedByName, verifiedById, previousStatus, article.NextVerificationDue);

        return MapToRecordDto(record, article.Title.English);
    }

    public async Task<bool> AssignOwnerAsync(
        Guid articleId, AssignOwnerRequest request, CancellationToken ct = default)
    {
        var article = await _dbContext.Set<Article>()
            .FirstOrDefaultAsync(a => a.Id == articleId, ct);

        if (article == null)
            return false;

        article.AssignOwner(request.OwnerId, request.OwnerName);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Article {ArticleId} owner assigned to {OwnerName} ({OwnerId})",
            articleId, request.OwnerName, request.OwnerId);

        return true;
    }

    public async Task<VerificationDashboardDto> GetDashboardAsync(CancellationToken ct = default)
    {
        var articles = _dbContext.Set<Article>().AsNoTracking();

        var totalCount = await articles.CountAsync(ct);
        var verifiedCount = await articles.CountAsync(a => a.VerificationStatus == VerificationStatus.Verified, ct);
        var unverifiedCount = await articles.CountAsync(a => a.VerificationStatus == VerificationStatus.Unverified, ct);
        var dueSoonCount = await articles.CountAsync(a => a.VerificationStatus == VerificationStatus.DueSoon, ct);
        var overdueCount = await articles.CountAsync(a => a.VerificationStatus == VerificationStatus.Overdue, ct);
        var withOwnerCount = await articles.CountAsync(a => a.OwnerId != null, ct);

        var overdueArticles = await GetOverdueArticlesAsync(ct);
        var dueSoonArticles = await GetDueSoonArticlesAsync(ct);

        var recentVerifications = await _dbContext.Set<VerificationRecord>()
            .AsNoTracking()
            .Include(r => r.Article)
            .OrderByDescending(r => r.VerifiedAt)
            .Take(10)
            .Select(r => new VerificationRecordDto
            {
                Id = r.Id,
                ArticleId = r.ArticleId,
                ArticleTitle = r.Article != null ? r.Article.Title.English : string.Empty,
                VerifiedById = r.VerifiedById,
                VerifiedByName = r.VerifiedByName,
                VerifiedAt = r.VerifiedAt,
                Notes = r.Notes,
                PreviousStatus = r.PreviousStatus.ToString(),
                NewStatus = r.NewStatus.ToString(),
                ExpiresAt = r.ExpiresAt
            })
            .ToListAsync(ct);

        return new VerificationDashboardDto
        {
            Metrics = new VerificationMetricsDto
            {
                TotalArticles = totalCount,
                VerifiedCount = verifiedCount,
                UnverifiedCount = unverifiedCount,
                DueSoonCount = dueSoonCount,
                OverdueCount = overdueCount,
                VerifiedPercentage = totalCount > 0 ? Math.Round((double)verifiedCount / totalCount * 100, 1) : 0,
                ArticlesWithOwner = withOwnerCount,
                ArticlesWithoutOwner = totalCount - withOwnerCount
            },
            OverdueArticles = overdueArticles,
            DueSoonArticles = dueSoonArticles,
            RecentVerifications = recentVerifications
        };
    }

    public async Task<IReadOnlyList<ArticleVerificationSummaryDto>> GetOverdueArticlesAsync(
        CancellationToken ct = default)
    {
        return await _dbContext.Set<Article>()
            .AsNoTracking()
            .Include(a => a.Category)
            .Where(a => a.VerificationStatus == VerificationStatus.Overdue)
            .OrderBy(a => a.NextVerificationDue)
            .Select(a => MapToVerificationSummary(a))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ArticleVerificationSummaryDto>> GetDueSoonArticlesAsync(
        CancellationToken ct = default)
    {
        return await _dbContext.Set<Article>()
            .AsNoTracking()
            .Include(a => a.Category)
            .Where(a => a.VerificationStatus == VerificationStatus.DueSoon)
            .OrderBy(a => a.NextVerificationDue)
            .Select(a => MapToVerificationSummary(a))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<VerificationRecordDto>> GetVerificationHistoryAsync(
        Guid articleId, CancellationToken ct = default)
    {
        return await _dbContext.Set<VerificationRecord>()
            .AsNoTracking()
            .Include(r => r.Article)
            .Where(r => r.ArticleId == articleId)
            .OrderByDescending(r => r.VerifiedAt)
            .Select(r => new VerificationRecordDto
            {
                Id = r.Id,
                ArticleId = r.ArticleId,
                ArticleTitle = r.Article != null ? r.Article.Title.English : string.Empty,
                VerifiedById = r.VerifiedById,
                VerifiedByName = r.VerifiedByName,
                VerifiedAt = r.VerifiedAt,
                Notes = r.Notes,
                PreviousStatus = r.PreviousStatus.ToString(),
                NewStatus = r.NewStatus.ToString(),
                ExpiresAt = r.ExpiresAt
            })
            .ToListAsync(ct);
    }

    // ========================================
    // Private Helpers
    // ========================================

    private static ArticleVerificationSummaryDto MapToVerificationSummary(Article a)
    {
        return new ArticleVerificationSummaryDto
        {
            Id = a.Id,
            Title = a.Title.English,
            TitleArabic = a.Title.Arabic,
            Slug = a.Slug,
            Status = a.Status.ToString(),
            VerificationStatus = a.VerificationStatus.ToString(),
            OwnerId = a.OwnerId,
            OwnerName = a.OwnerName,
            LastVerifiedAt = a.LastVerifiedAt,
            NextVerificationDue = a.NextVerificationDue,
            ReviewIntervalDays = a.ReviewIntervalDays,
            CategoryName = a.Category != null ? a.Category.Name.English : null
        };
    }

    private static VerificationRecordDto MapToRecordDto(VerificationRecord r, string articleTitle)
    {
        return new VerificationRecordDto
        {
            Id = r.Id,
            ArticleId = r.ArticleId,
            ArticleTitle = articleTitle,
            VerifiedById = r.VerifiedById,
            VerifiedByName = r.VerifiedByName,
            VerifiedAt = r.VerifiedAt,
            Notes = r.Notes,
            PreviousStatus = r.PreviousStatus.ToString(),
            NewStatus = r.NewStatus.ToString(),
            ExpiresAt = r.ExpiresAt
        };
    }
}
