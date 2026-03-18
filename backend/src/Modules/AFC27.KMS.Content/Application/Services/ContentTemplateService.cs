using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Content.Application.DTOs;
using AFC27.KMS.Content.Application.Interfaces;
using AFC27.KMS.Content.Domain.Entities;
using AFC27.KMS.Contracts.Common;

namespace AFC27.KMS.Content.Application.Services;

/// <summary>
/// Service for managing content templates, including CRUD and review cycles.
/// </summary>
public class ContentTemplateService : IContentTemplateService
{
    private readonly DbContext _dbContext;
    private readonly ILogger<ContentTemplateService> _logger;

    public ContentTemplateService(
        DbContext dbContext,
        ILogger<ContentTemplateService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<PagedResult<ContentTemplateSummaryDto>> GetTemplatesAsync(
        ContentTemplateFilterRequest filter,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<ContentTemplate>()
            .AsNoTracking()
            .Where(t => !t.IsDeleted);

        // Show public templates + user's own private templates
        query = query.Where(t => t.IsPublic || t.CreatorId == currentUserId);

        if (!string.IsNullOrEmpty(filter.Search))
        {
            var search = filter.Search.ToLower();
            query = query.Where(t =>
                t.Name.ToLower().Contains(search) ||
                (t.Description != null && t.Description.ToLower().Contains(search)));
        }

        if (!string.IsNullOrEmpty(filter.Category))
            query = query.Where(t => t.Category == filter.Category);

        if (filter.IsPublic.HasValue)
            query = query.Where(t => t.IsPublic == filter.IsPublic.Value);

        if (filter.CreatorId.HasValue)
            query = query.Where(t => t.CreatorId == filter.CreatorId.Value);

        if (!string.IsNullOrEmpty(filter.ReviewStatus) &&
            Enum.TryParse<TemplateReviewStatus>(filter.ReviewStatus, true, out var reviewStatus))
        {
            query = query.Where(t => t.ReviewStatus == reviewStatus);
        }

        // Sorting
        query = filter.SortBy?.ToLowerInvariant() switch
        {
            "name" => filter.SortDescending ? query.OrderByDescending(t => t.Name) : query.OrderBy(t => t.Name),
            "category" => filter.SortDescending ? query.OrderByDescending(t => t.Category) : query.OrderBy(t => t.Category),
            "usagecount" => filter.SortDescending ? query.OrderByDescending(t => t.UsageCount) : query.OrderBy(t => t.UsageCount),
            "createdat" => filter.SortDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt),
            _ => query.OrderBy(t => t.Name)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(t => MapToSummary(t))
            .ToListAsync(cancellationToken);

        return PagedResult<ContentTemplateSummaryDto>.Create(items, totalCount, filter.Page, filter.PageSize);
    }

    public async Task<ContentTemplateDto?> GetTemplateByIdAsync(
        Guid templateId,
        CancellationToken cancellationToken = default)
    {
        var template = await _dbContext.Set<ContentTemplate>()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == templateId && !t.IsDeleted, cancellationToken);

        return template == null ? null : MapToDto(template);
    }

    public async Task<ContentTemplateDto> CreateTemplateAsync(
        CreateContentTemplateRequest request,
        Guid creatorId,
        string creatorName,
        CancellationToken cancellationToken = default)
    {
        var template = ContentTemplate.Create(
            name: request.Name,
            structure: request.Structure,
            category: request.Category,
            isPublic: request.IsPublic,
            creatorId: creatorId,
            creatorName: creatorName,
            nameArabic: request.NameArabic,
            description: request.Description,
            descriptionArabic: request.DescriptionArabic,
            thumbnailUrl: request.ThumbnailUrl,
            tags: request.Tags,
            reviewIntervalDays: request.ReviewIntervalDays);

        _dbContext.Set<ContentTemplate>().Add(template);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created content template {TemplateId}: {Name}", template.Id, request.Name);

        return MapToDto(template);
    }

    public async Task<bool> UpdateTemplateAsync(
        Guid templateId,
        UpdateContentTemplateRequest request,
        CancellationToken cancellationToken = default)
    {
        var template = await _dbContext.Set<ContentTemplate>()
            .FirstOrDefaultAsync(t => t.Id == templateId && !t.IsDeleted, cancellationToken);

        if (template == null) return false;

        template.Update(
            name: request.Name,
            nameArabic: request.NameArabic,
            description: request.Description,
            descriptionArabic: request.DescriptionArabic,
            structure: request.Structure,
            category: request.Category,
            isPublic: request.IsPublic,
            thumbnailUrl: request.ThumbnailUrl,
            tags: request.Tags);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated content template {TemplateId}", templateId);
        return true;
    }

    public async Task<bool> DeleteTemplateAsync(
        Guid templateId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var template = await _dbContext.Set<ContentTemplate>()
            .FirstOrDefaultAsync(t => t.Id == templateId && !t.IsDeleted, cancellationToken);

        if (template == null) return false;

        template.SoftDelete(userId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted content template {TemplateId}", templateId);
        return true;
    }

    public async Task<bool> IncrementUsageAsync(
        Guid templateId,
        CancellationToken cancellationToken = default)
    {
        var template = await _dbContext.Set<ContentTemplate>()
            .FirstOrDefaultAsync(t => t.Id == templateId && !t.IsDeleted, cancellationToken);

        if (template == null) return false;

        template.IncrementUsageCount();
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<string>> GetCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ContentTemplate>()
            .AsNoTracking()
            .Where(t => !t.IsDeleted)
            .Select(t => t.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(cancellationToken);
    }

    // Phase 3E: Review cycle operations

    public async Task<bool> ReviewTemplateAsync(
        Guid templateId,
        Guid reviewerId,
        string reviewerName,
        ReviewTemplateRequest request,
        CancellationToken cancellationToken = default)
    {
        var template = await _dbContext.Set<ContentTemplate>()
            .FirstOrDefaultAsync(t => t.Id == templateId && !t.IsDeleted, cancellationToken);

        if (template == null) return false;

        template.MarkReviewed(reviewerId, reviewerName, request.NewReviewIntervalDays);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Template {TemplateId} reviewed by {ReviewerName}. Next review due: {NextReviewDue}",
            templateId, reviewerName, template.NextReviewDue);

        return true;
    }

    public async Task<bool> SetReviewIntervalAsync(
        Guid templateId,
        int intervalDays,
        CancellationToken cancellationToken = default)
    {
        var template = await _dbContext.Set<ContentTemplate>()
            .FirstOrDefaultAsync(t => t.Id == templateId && !t.IsDeleted, cancellationToken);

        if (template == null) return false;

        template.SetReviewInterval(intervalDays);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Template {TemplateId} review interval set to {IntervalDays} days",
            templateId, intervalDays);

        return true;
    }

    public async Task<IReadOnlyList<ContentTemplateSummaryDto>> GetOverdueTemplatesAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await _dbContext.Set<ContentTemplate>()
            .AsNoTracking()
            .Where(t => !t.IsDeleted &&
                        t.ReviewIntervalDays > 0 &&
                        t.NextReviewDue.HasValue &&
                        t.NextReviewDue.Value < now)
            .OrderBy(t => t.NextReviewDue)
            .Select(t => MapToSummary(t))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ContentTemplateSummaryDto>> GetDueSoonTemplatesAsync(
        int daysThreshold = 7,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var threshold = now.AddDays(daysThreshold);

        return await _dbContext.Set<ContentTemplate>()
            .AsNoTracking()
            .Where(t => !t.IsDeleted &&
                        t.ReviewIntervalDays > 0 &&
                        t.NextReviewDue.HasValue &&
                        t.NextReviewDue.Value >= now &&
                        t.NextReviewDue.Value <= threshold)
            .OrderBy(t => t.NextReviewDue)
            .Select(t => MapToSummary(t))
            .ToListAsync(cancellationToken);
    }

    private static ContentTemplateDto MapToDto(ContentTemplate template)
    {
        return new ContentTemplateDto
        {
            Id = template.Id,
            Name = template.Name,
            NameArabic = template.NameArabic,
            Description = template.Description,
            DescriptionArabic = template.DescriptionArabic,
            Structure = template.Structure,
            Category = template.Category,
            IsPublic = template.IsPublic,
            IsSystem = template.IsSystem,
            CreatorId = template.CreatorId,
            CreatorName = template.CreatorName,
            ThumbnailUrl = template.ThumbnailUrl,
            UsageCount = template.UsageCount,
            Tags = template.Tags,
            ReviewIntervalDays = template.ReviewIntervalDays,
            LastReviewedAt = template.LastReviewedAt,
            NextReviewDue = template.NextReviewDue,
            LastReviewedByName = template.LastReviewedByName,
            ReviewStatus = template.ReviewStatus.ToString(),
            CreatedAt = template.CreatedAt,
            ModifiedAt = template.ModifiedAt
        };
    }

    private static ContentTemplateSummaryDto MapToSummary(ContentTemplate template)
    {
        return new ContentTemplateSummaryDto
        {
            Id = template.Id,
            Name = template.Name,
            NameArabic = template.NameArabic,
            Description = template.Description,
            Category = template.Category,
            IsPublic = template.IsPublic,
            IsSystem = template.IsSystem,
            CreatorName = template.CreatorName,
            ThumbnailUrl = template.ThumbnailUrl,
            UsageCount = template.UsageCount,
            ReviewStatus = template.ReviewStatus.ToString(),
            NextReviewDue = template.NextReviewDue,
            CreatedAt = template.CreatedAt
        };
    }
}
