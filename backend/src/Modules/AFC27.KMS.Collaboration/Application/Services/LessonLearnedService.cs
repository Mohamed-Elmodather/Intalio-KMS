using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Collaboration.Application.DTOs;
using AFC27.KMS.Collaboration.Application.Interfaces;
using AFC27.KMS.Collaboration.Domain.Entities;
using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Collaboration.Application.Services;

/// <summary>
/// Service implementation for Lessons Learned CRUD, workflow transitions, and analytics.
/// </summary>
public class LessonLearnedService : ILessonLearnedService
{
    private readonly DbContext _dbContext;
    private readonly ILogger<LessonLearnedService> _logger;

    public LessonLearnedService(DbContext dbContext, ILogger<LessonLearnedService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    // ========================================
    // Queries
    // ========================================

    public async Task<PagedResult<LessonLearnedSummaryDto>> GetLessonsAsync(
        string? search, string? category, string? impact, string? status,
        Guid? projectId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _dbContext.Set<LessonLearned>()
            .AsNoTracking()
            .Include(l => l.Tags)
            .Include(l => l.Actions)
            .AsQueryable();

        // Search filter on Title and Description
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(l =>
                l.Title.English.ToLower().Contains(searchLower) ||
                l.Title.Arabic.ToLower().Contains(searchLower) ||
                l.Description.ToLower().Contains(searchLower));
        }

        // Category filter
        if (!string.IsNullOrWhiteSpace(category) && Enum.TryParse<LessonCategory>(category, true, out var cat))
        {
            query = query.Where(l => l.Category == cat);
        }

        // Impact filter
        if (!string.IsNullOrWhiteSpace(impact) && Enum.TryParse<LessonImpact>(impact, true, out var imp))
        {
            query = query.Where(l => l.Impact == imp);
        }

        // Status filter
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<LessonStatus>(status, true, out var st))
        {
            query = query.Where(l => l.Status == st);
        }

        // Project filter
        if (projectId.HasValue)
        {
            query = query.Where(l => l.ProjectId == projectId.Value);
        }

        // Order by most recent first
        query = query.OrderByDescending(l => l.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new LessonLearnedSummaryDto
            {
                Id = l.Id,
                Title = l.Title.English,
                TitleArabic = l.Title.Arabic,
                DescriptionPreview = l.Description.Length > 200
                    ? l.Description.Substring(0, 200) + "..."
                    : l.Description,
                Category = l.Category.ToString(),
                Impact = l.Impact.ToString(),
                Status = l.Status.ToString(),
                AuthorName = l.AuthorName,
                ProjectName = l.ProjectName,
                ProcessOwnerName = l.ProcessOwnerName,
                ViewCount = l.ViewCount,
                UsefulCount = l.UsefulCount,
                TotalActions = l.Actions.Count,
                CompletedActions = l.Actions.Count(a =>
                    a.Status == LessonActionStatus.Completed || a.Status == LessonActionStatus.Verified),
                OverdueActions = l.Actions.Count(a =>
                    (a.Status == LessonActionStatus.Open || a.Status == LessonActionStatus.InProgress)
                    && a.DueDate < DateTime.UtcNow),
                Tags = l.Tags.Select(t => t.Tag).ToList(),
                CreatedAt = l.CreatedAt
            })
            .ToListAsync(ct);

        return new PagedResult<LessonLearnedSummaryDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<LessonLearnedDto?> GetLessonByIdAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var lesson = await _dbContext.Set<LessonLearned>()
            .Include(l => l.Tags)
            .Include(l => l.Actions)
            .Include(l => l.Community)
            .FirstOrDefaultAsync(l => l.Id == id, ct);

        if (lesson == null)
            return null;

        // Increment view count
        lesson.IncrementViewCount();
        await _dbContext.SaveChangesAsync(ct);

        // Check if current user marked as useful
        var isMarkedUseful = await _dbContext.Set<LessonUsefulVote>()
            .AsNoTracking()
            .AnyAsync(v => v.LessonLearnedId == id && v.UserId == userId, ct);

        return MapToDto(lesson, isMarkedUseful);
    }

    // ========================================
    // CRUD
    // ========================================

    public async Task<LessonLearnedDto> CreateLessonAsync(
        CreateLessonLearnedRequest request, Guid userId, string userName, CancellationToken ct = default)
    {
        var title = new LocalizedString(request.Title, request.TitleArabic ?? string.Empty);

        Enum.TryParse<LessonCategory>(request.Category, true, out var category);
        Enum.TryParse<LessonImpact>(request.Impact, true, out var impact);

        var lesson = LessonLearned.Create(
            title,
            request.Description,
            request.Context,
            request.Challenge,
            request.Solution,
            request.Outcome,
            category,
            impact,
            userId,
            userName);

        // Set optional fields
        if (request.ProjectId.HasValue)
            lesson.SetProject(request.ProjectId, request.ProjectName ?? string.Empty);

        if (request.CommunityId.HasValue)
            lesson.SetCommunity(request.CommunityId);

        if (request.IsAnonymous)
            lesson.SetAnonymous(true);

        if (request.ProcessOwnerId.HasValue && !string.IsNullOrEmpty(request.ProcessOwnerName))
            lesson.AssignProcessOwner(request.ProcessOwnerId.Value, request.ProcessOwnerName);

        if (!string.IsNullOrEmpty(request.RootCause))
        {
            RootCauseMethod? rootMethod = null;
            if (!string.IsNullOrEmpty(request.RootCauseMethod) &&
                Enum.TryParse<RootCauseMethod>(request.RootCauseMethod, true, out var rm))
            {
                rootMethod = rm;
            }
            lesson.SetRootCause(request.RootCause, request.RootCauseArabic, rootMethod);
        }

        if (!string.IsNullOrEmpty(request.ProjectPhase) || !string.IsNullOrEmpty(request.ImpactType))
        {
            ProjectPhase? phase = null;
            ImpactType? impType = null;
            if (!string.IsNullOrEmpty(request.ProjectPhase) &&
                Enum.TryParse<ProjectPhase>(request.ProjectPhase, true, out var pp))
                phase = pp;
            if (!string.IsNullOrEmpty(request.ImpactType) &&
                Enum.TryParse<ImpactType>(request.ImpactType, true, out var it))
                impType = it;
            lesson.SetClassification(phase, impType);
        }

        _dbContext.Set<LessonLearned>().Add(lesson);

        // Add tags
        if (request.Tags is { Count: > 0 })
        {
            foreach (var tag in request.Tags)
            {
                _dbContext.Set<LessonTag>().Add(new LessonTag
                {
                    LessonId = lesson.Id,
                    Tag = tag
                });
            }
        }

        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Lesson {LessonId} created by user {UserId}", lesson.Id, userId);

        return MapToDto(lesson, false);
    }

    public async Task<bool> UpdateLessonAsync(
        Guid id, CreateLessonLearnedRequest request, Guid userId, CancellationToken ct = default)
    {
        var lesson = await _dbContext.Set<LessonLearned>()
            .Include(l => l.Tags)
            .FirstOrDefaultAsync(l => l.Id == id, ct);

        if (lesson == null)
            return false;

        var title = new LocalizedString(request.Title, request.TitleArabic ?? string.Empty);

        lesson.Update(
            title,
            request.Description,
            request.Context,
            request.Challenge,
            request.Solution,
            request.Outcome,
            request.Recommendations);

        // Update project association
        lesson.SetProject(request.ProjectId, request.ProjectName ?? string.Empty);
        lesson.SetCommunity(request.CommunityId);
        lesson.SetAnonymous(request.IsAnonymous);

        // Update classification
        ProjectPhase? phase = null;
        ImpactType? impType = null;
        if (!string.IsNullOrEmpty(request.ProjectPhase) &&
            Enum.TryParse<ProjectPhase>(request.ProjectPhase, true, out var pp))
            phase = pp;
        if (!string.IsNullOrEmpty(request.ImpactType) &&
            Enum.TryParse<ImpactType>(request.ImpactType, true, out var it))
            impType = it;
        lesson.SetClassification(phase, impType);

        // Replace tags
        var existingTags = await _dbContext.Set<LessonTag>()
            .Where(t => t.LessonId == id)
            .ToListAsync(ct);
        _dbContext.Set<LessonTag>().RemoveRange(existingTags);

        if (request.Tags is { Count: > 0 })
        {
            foreach (var tag in request.Tags)
            {
                _dbContext.Set<LessonTag>().Add(new LessonTag
                {
                    LessonId = lesson.Id,
                    Tag = tag
                });
            }
        }

        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Lesson {LessonId} updated by user {UserId}", id, userId);
        return true;
    }

    public async Task<bool> DeleteLessonAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var lesson = await _dbContext.Set<LessonLearned>()
            .FirstOrDefaultAsync(l => l.Id == id, ct);

        if (lesson == null)
            return false;

        _dbContext.Set<LessonLearned>().Remove(lesson);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Lesson {LessonId} deleted by user {UserId}", id, userId);
        return true;
    }

    // ========================================
    // Workflow Transitions
    // ========================================

    public async Task<bool> SubmitForReviewAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var lesson = await _dbContext.Set<LessonLearned>().FirstOrDefaultAsync(l => l.Id == id, ct);
        if (lesson == null) return false;

        lesson.Submit();
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Lesson {LessonId} submitted for review by {UserId}", id, userId);
        return true;
    }

    public async Task<bool> ApproveAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var lesson = await _dbContext.Set<LessonLearned>().FirstOrDefaultAsync(l => l.Id == id, ct);
        if (lesson == null) return false;

        lesson.Approve();
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Lesson {LessonId} approved by {UserId}", id, userId);
        return true;
    }

    public async Task<bool> RejectAsync(Guid id, string reason, Guid userId, CancellationToken ct = default)
    {
        var lesson = await _dbContext.Set<LessonLearned>().FirstOrDefaultAsync(l => l.Id == id, ct);
        if (lesson == null) return false;

        lesson.Reject(reason, userId);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Lesson {LessonId} rejected by {UserId}: {Reason}", id, userId, reason);
        return true;
    }

    public async Task<bool> PublishAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var lesson = await _dbContext.Set<LessonLearned>().FirstOrDefaultAsync(l => l.Id == id, ct);
        if (lesson == null) return false;

        lesson.Publish();
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Lesson {LessonId} published by {UserId}", id, userId);
        return true;
    }

    public async Task<bool> MarkActionsPendingAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var lesson = await _dbContext.Set<LessonLearned>().FirstOrDefaultAsync(l => l.Id == id, ct);
        if (lesson == null) return false;

        lesson.MarkActionsPending();
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Lesson {LessonId} marked actions pending by {UserId}", id, userId);
        return true;
    }

    public async Task<bool> MarkActionsCompleteAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var lesson = await _dbContext.Set<LessonLearned>().FirstOrDefaultAsync(l => l.Id == id, ct);
        if (lesson == null) return false;

        lesson.MarkActionsComplete();
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Lesson {LessonId} marked actions complete by {UserId}", id, userId);
        return true;
    }

    public async Task<bool> VerifyLessonAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var lesson = await _dbContext.Set<LessonLearned>().FirstOrDefaultAsync(l => l.Id == id, ct);
        if (lesson == null) return false;

        lesson.MarkVerified();
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Lesson {LessonId} verified by {UserId}", id, userId);
        return true;
    }

    public async Task<bool> ArchiveAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var lesson = await _dbContext.Set<LessonLearned>().FirstOrDefaultAsync(l => l.Id == id, ct);
        if (lesson == null) return false;

        lesson.Archive();
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Lesson {LessonId} archived by {UserId}", id, userId);
        return true;
    }

    // ========================================
    // Useful Voting
    // ========================================

    public async Task<bool> MarkAsUsefulAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var lesson = await _dbContext.Set<LessonLearned>().FirstOrDefaultAsync(l => l.Id == id, ct);
        if (lesson == null) return false;

        var alreadyVoted = await _dbContext.Set<LessonUsefulVote>()
            .AnyAsync(v => v.LessonLearnedId == id && v.UserId == userId, ct);

        if (alreadyVoted)
            return true; // Already marked

        var vote = LessonUsefulVote.Create(id, userId);
        _dbContext.Set<LessonUsefulVote>().Add(vote);
        lesson.IncrementUsefulCount();

        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Lesson {LessonId} marked as useful by {UserId}", id, userId);
        return true;
    }

    public async Task<bool> UnmarkAsUsefulAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var lesson = await _dbContext.Set<LessonLearned>().FirstOrDefaultAsync(l => l.Id == id, ct);
        if (lesson == null) return false;

        var vote = await _dbContext.Set<LessonUsefulVote>()
            .FirstOrDefaultAsync(v => v.LessonLearnedId == id && v.UserId == userId, ct);

        if (vote == null)
            return true; // Not voted

        _dbContext.Set<LessonUsefulVote>().Remove(vote);
        lesson.DecrementUsefulCount();

        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Lesson {LessonId} unmarked as useful by {UserId}", id, userId);
        return true;
    }

    // ========================================
    // Related Lessons
    // ========================================

    public async Task<List<LessonLearnedSummaryDto>> GetRelatedLessonsAsync(
        Guid id, int limit, CancellationToken ct = default)
    {
        var lesson = await _dbContext.Set<LessonLearned>()
            .AsNoTracking()
            .Include(l => l.Tags)
            .FirstOrDefaultAsync(l => l.Id == id, ct);

        if (lesson == null)
            return new List<LessonLearnedSummaryDto>();

        var lessonTags = lesson.Tags.Select(t => t.Tag).ToList();

        // Find lessons with same category or overlapping tags, excluding self
        var query = _dbContext.Set<LessonLearned>()
            .AsNoTracking()
            .Include(l => l.Tags)
            .Include(l => l.Actions)
            .Where(l => l.Id != id && l.Status == LessonStatus.Published);

        // Prefer same category or matching tags
        var relatedLessons = await query
            .Where(l => l.Category == lesson.Category
                || l.Tags.Any(t => lessonTags.Contains(t.Tag)))
            .OrderByDescending(l => l.UsefulCount)
            .ThenByDescending(l => l.ViewCount)
            .Take(limit)
            .Select(l => new LessonLearnedSummaryDto
            {
                Id = l.Id,
                Title = l.Title.English,
                TitleArabic = l.Title.Arabic,
                DescriptionPreview = l.Description.Length > 200
                    ? l.Description.Substring(0, 200) + "..."
                    : l.Description,
                Category = l.Category.ToString(),
                Impact = l.Impact.ToString(),
                Status = l.Status.ToString(),
                AuthorName = l.AuthorName,
                ProjectName = l.ProjectName,
                ProcessOwnerName = l.ProcessOwnerName,
                ViewCount = l.ViewCount,
                UsefulCount = l.UsefulCount,
                TotalActions = l.Actions.Count,
                CompletedActions = l.Actions.Count(a =>
                    a.Status == LessonActionStatus.Completed || a.Status == LessonActionStatus.Verified),
                OverdueActions = l.Actions.Count(a =>
                    (a.Status == LessonActionStatus.Open || a.Status == LessonActionStatus.InProgress)
                    && a.DueDate < DateTime.UtcNow),
                Tags = l.Tags.Select(t => t.Tag).ToList(),
                CreatedAt = l.CreatedAt
            })
            .ToListAsync(ct);

        return relatedLessons;
    }

    // ========================================
    // Process Owner & Root Cause
    // ========================================

    public async Task<bool> AssignProcessOwnerAsync(
        Guid id, AssignProcessOwnerRequest request, Guid userId, CancellationToken ct = default)
    {
        var lesson = await _dbContext.Set<LessonLearned>().FirstOrDefaultAsync(l => l.Id == id, ct);
        if (lesson == null) return false;

        lesson.AssignProcessOwner(request.ProcessOwnerId, request.ProcessOwnerName);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Process owner {OwnerId} assigned to lesson {LessonId} by {UserId}",
            request.ProcessOwnerId, id, userId);
        return true;
    }

    public async Task<bool> SetRootCauseAsync(
        Guid id, SetRootCauseRequest request, Guid userId, CancellationToken ct = default)
    {
        var lesson = await _dbContext.Set<LessonLearned>().FirstOrDefaultAsync(l => l.Id == id, ct);
        if (lesson == null) return false;

        RootCauseMethod? method = null;
        if (!string.IsNullOrEmpty(request.Method) &&
            Enum.TryParse<RootCauseMethod>(request.Method, true, out var rm))
        {
            method = rm;
        }

        lesson.SetRootCause(request.RootCause, request.RootCauseArabic, method);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Root cause set for lesson {LessonId} by {UserId}", id, userId);
        return true;
    }

    // ========================================
    // Analytics
    // ========================================

    public async Task<LessonsAnalyticsOverviewDto> GetAnalyticsOverviewAsync(
        DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var allLessons = _dbContext.Set<LessonLearned>().AsNoTracking();
        var allActions = _dbContext.Set<LessonAction>().AsNoTracking();

        // Period filter for created/published counts
        var periodLessons = allLessons.AsQueryable();
        if (from.HasValue)
            periodLessons = periodLessons.Where(l => l.CreatedAt >= from.Value);
        if (to.HasValue)
            periodLessons = periodLessons.Where(l => l.CreatedAt <= to.Value);

        var totalLessons = await allLessons.CountAsync(ct);
        var lessonsCreatedInPeriod = await periodLessons.CountAsync(ct);
        var lessonsPublishedInPeriod = await periodLessons
            .Where(l => l.Status == LessonStatus.Published
                     || l.Status == LessonStatus.ActionsPending
                     || l.Status == LessonStatus.ActionsComplete
                     || l.Status == LessonStatus.Verified
                     || l.Status == LessonStatus.Archived)
            .CountAsync(ct);

        // Status distribution
        var lessonsByStatus = await allLessons
            .GroupBy(l => l.Status)
            .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
            .ToListAsync(ct);

        // Category distribution
        var lessonsByCategory = await allLessons
            .GroupBy(l => l.Category)
            .Select(g => new { Category = g.Key.ToString(), Count = g.Count() })
            .ToListAsync(ct);

        // Impact distribution
        var lessonsByImpact = await allLessons
            .GroupBy(l => l.Impact)
            .Select(g => new { Impact = g.Key.ToString(), Count = g.Count() })
            .ToListAsync(ct);

        // Action stats
        var totalActions = await allActions.CountAsync(ct);
        var openActions = await allActions.Where(a => a.Status == LessonActionStatus.Open).CountAsync(ct);
        var completedActions = await allActions
            .Where(a => a.Status == LessonActionStatus.Completed || a.Status == LessonActionStatus.Verified)
            .CountAsync(ct);
        var verifiedActions = await allActions.Where(a => a.Status == LessonActionStatus.Verified).CountAsync(ct);
        var overdueActions = await allActions
            .Where(a => (a.Status == LessonActionStatus.Open || a.Status == LessonActionStatus.InProgress)
                     && a.DueDate < DateTime.UtcNow)
            .CountAsync(ct);

        var actionCompletionRate = totalActions > 0
            ? Math.Round((double)completedActions / totalActions * 100, 1)
            : 0;

        // Average time to complete (for completed actions with StartedAt)
        var completedWithStart = await allActions
            .Where(a => a.Status == LessonActionStatus.Completed && a.StartedAt != null && a.CompletedAt != null)
            .Select(a => EF.Functions.DateDiffDay(a.StartedAt!.Value, a.CompletedAt!.Value))
            .ToListAsync(ct);

        var avgTimeToComplete = completedWithStart.Count > 0
            ? Math.Round(completedWithStart.Average(), 1)
            : 0;

        // Engagement
        var totalViews = await allLessons.SumAsync(l => l.ViewCount, ct);
        var totalUsefulVotes = await allLessons.SumAsync(l => l.UsefulCount, ct);

        // Top contributors
        var topContributors = await allLessons
            .GroupBy(l => new { l.AuthorId, l.AuthorName })
            .Select(g => new LessonContributorDto
            {
                UserId = g.Key.AuthorId,
                UserName = g.Key.AuthorName,
                LessonsAuthored = g.Count(),
                UsefulVotesReceived = g.Sum(l => l.UsefulCount)
            })
            .OrderByDescending(c => c.LessonsAuthored)
            .Take(10)
            .ToListAsync(ct);

        // Overdue actions list
        var overdueActionsList = await allActions
            .Where(a => (a.Status == LessonActionStatus.Open || a.Status == LessonActionStatus.InProgress)
                     && a.DueDate < DateTime.UtcNow)
            .Include(a => a.LessonLearned)
            .OrderBy(a => a.DueDate)
            .Take(20)
            .Select(a => new OverdueActionSummaryDto
            {
                ActionId = a.Id,
                ActionDescription = a.Description,
                LessonId = a.LessonLearnedId,
                LessonTitle = a.LessonLearned.Title.English,
                AssigneeName = a.AssigneeName,
                DueDate = a.DueDate,
                DaysOverdue = EF.Functions.DateDiffDay(a.DueDate, DateTime.UtcNow),
                ReminderCount = a.ReminderCount
            })
            .ToListAsync(ct);

        // Lessons without process owner
        var lessonsWithoutOwner = await allLessons
            .Where(l => l.ProcessOwnerId == null
                     && l.Status != LessonStatus.Draft
                     && l.Status != LessonStatus.Archived)
            .CountAsync(ct);

        return new LessonsAnalyticsOverviewDto
        {
            TotalLessons = totalLessons,
            LessonsCreatedInPeriod = lessonsCreatedInPeriod,
            LessonsPublishedInPeriod = lessonsPublishedInPeriod,
            LessonsByStatus = lessonsByStatus.ToDictionary(x => x.Status, x => x.Count),
            LessonsByCategory = lessonsByCategory.ToDictionary(x => x.Category, x => x.Count),
            LessonsByImpact = lessonsByImpact.ToDictionary(x => x.Impact, x => x.Count),
            TotalActions = totalActions,
            OpenActions = openActions,
            CompletedActions = completedActions,
            VerifiedActions = verifiedActions,
            OverdueActions = overdueActions,
            ActionCompletionRate = actionCompletionRate,
            AverageTimeToCompleteActionDays = avgTimeToComplete,
            TotalViews = totalViews,
            TotalUsefulVotes = totalUsefulVotes,
            TopContributors = topContributors,
            OverdueActionsList = overdueActionsList,
            LessonsWithoutProcessOwner = lessonsWithoutOwner
        };
    }

    // ========================================
    // Private Helpers
    // ========================================

    private static LessonLearnedDto MapToDto(LessonLearned lesson, bool isMarkedUseful)
    {
        return new LessonLearnedDto
        {
            Id = lesson.Id,
            Title = lesson.Title.English,
            TitleArabic = lesson.Title.Arabic,
            Description = lesson.Description,
            DescriptionArabic = lesson.DescriptionArabic,
            Context = lesson.Context,
            ContextArabic = lesson.ContextArabic,
            Challenge = lesson.Challenge,
            ChallengeArabic = lesson.ChallengeArabic,
            Solution = lesson.Solution,
            SolutionArabic = lesson.SolutionArabic,
            Outcome = lesson.Outcome,
            OutcomeArabic = lesson.OutcomeArabic,
            Recommendations = lesson.Recommendations,
            RecommendationsArabic = lesson.RecommendationsArabic,
            WhatWentWell = lesson.WhatWentWell,
            WhatWentWellArabic = lesson.WhatWentWellArabic,
            RootCause = lesson.RootCause,
            RootCauseArabic = lesson.RootCauseArabic,
            RootCauseMethod = lesson.RootCauseMethod?.ToString(),
            Category = lesson.Category.ToString(),
            Impact = lesson.Impact.ToString(),
            Status = lesson.Status.ToString(),
            ProjectPhase = lesson.ProjectPhase?.ToString(),
            ImpactType = lesson.ImpactType?.ToString(),
            AuthorId = lesson.AuthorId,
            AuthorName = lesson.IsAnonymous ? "Anonymous" : lesson.AuthorName,
            IsAnonymous = lesson.IsAnonymous,
            ProcessOwnerId = lesson.ProcessOwnerId,
            ProcessOwnerName = lesson.ProcessOwnerName,
            CommunityId = lesson.CommunityId,
            CommunityName = lesson.Community?.Name?.English,
            ProjectId = lesson.ProjectId,
            ProjectName = lesson.ProjectName,
            OccurredAt = lesson.OccurredAt,
            ViewCount = lesson.ViewCount,
            UsefulCount = lesson.UsefulCount,
            IsMarkedUsefulByCurrentUser = isMarkedUseful,
            Tags = lesson.Tags.Select(t => t.Tag).ToList(),
            CreatedAt = lesson.CreatedAt,
            RejectionReason = lesson.RejectionReason,
            Actions = lesson.Actions.Select(MapActionToDto).ToList(),
            TotalActions = lesson.Actions.Count,
            CompletedActions = lesson.Actions.Count(a =>
                a.Status == LessonActionStatus.Completed || a.Status == LessonActionStatus.Verified),
            OverdueActions = lesson.Actions.Count(a =>
                (a.Status == LessonActionStatus.Open || a.Status == LessonActionStatus.InProgress)
                && a.DueDate < DateTime.UtcNow)
        };
    }

    private static LessonActionDto MapActionToDto(LessonAction action)
    {
        return new LessonActionDto
        {
            Id = action.Id,
            LessonLearnedId = action.LessonLearnedId,
            Description = action.Description,
            DescriptionArabic = action.DescriptionArabic,
            AssigneeId = action.AssigneeId,
            AssigneeName = action.AssigneeName,
            DelegatedToId = action.DelegatedToId,
            DelegatedToName = action.DelegatedToName,
            Status = action.Status.ToString(),
            Priority = action.Priority.ToString(),
            DueDate = action.DueDate,
            StartedAt = action.StartedAt,
            CompletedAt = action.CompletedAt,
            CompletionNotes = action.CompletionNotes,
            CompletionNotesArabic = action.CompletionNotesArabic,
            VerifiedAt = action.VerifiedAt,
            VerifiedByName = action.VerifiedByName,
            VerificationNotes = action.VerificationNotes,
            AffectedDocumentId = action.AffectedDocumentId,
            AffectedDocumentTitle = action.AffectedDocumentTitle,
            AffectedDocumentType = action.AffectedDocumentType,
            IsOverdue = action.IsOverdue,
            ReminderCount = action.ReminderCount,
            EscalatedAt = action.EscalatedAt,
            EscalatedToName = action.EscalatedToName,
            SortOrder = action.SortOrder,
            CreatedAt = action.CreatedAt
        };
    }
}
