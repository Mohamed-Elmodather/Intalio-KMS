using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Application.Interfaces;
using AFC27.KMS.Admin.Domain.Entities;
using AFC27.KMS.SharedKernel.Domain;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.Admin.Application.Services;

/// <summary>
/// Service interface for knowledge governance.
/// </summary>
public interface IGovernanceService
{
    Task<GovernanceDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<GovernanceReviewDto> CreateReviewAsync(CreateGovernanceReviewRequest request, CancellationToken cancellationToken = default);
    Task<GovernanceReviewDto?> GetReviewByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GovernanceReviewDto>> ListReviewsAsync(GovernanceReviewStatus? status = null, CancellationToken cancellationToken = default);
    Task<bool> StartReviewAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> RecordFindingsAsync(Guid id, RecordFindingsRequest request, CancellationToken cancellationToken = default);
    Task<bool> SetRemediationPlanAsync(Guid id, SetRemediationPlanRequest request, CancellationToken cancellationToken = default);
    Task<bool> CompleteReviewAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GovernanceActionDto> AddActionAsync(Guid reviewId, CreateGovernanceActionRequest request, CancellationToken cancellationToken = default);
    Task<bool> CompleteActionAsync(Guid actionId, string? notes, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service for knowledge governance tracking and review management.
/// </summary>
public class GovernanceService : IGovernanceService
{
    private readonly DbContext _dbContext;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<GovernanceService> _logger;

    public GovernanceService(
        DbContext dbContext,
        ICurrentUser currentUser,
        ILogger<GovernanceService> logger)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<GovernanceDashboardDto> GetDashboardAsync(
        CancellationToken cancellationToken = default)
    {
        var reviews = await _dbContext.Set<GovernanceReview>().AsNoTracking()
            .Include(r => r.Actions)
            .ToListAsync(cancellationToken);

        var allActions = reviews.SelectMany(r => r.Actions).ToList();

        var latestCompleted = reviews
            .Where(r => r.Status == GovernanceReviewStatus.Completed && r.GovernanceScore.HasValue)
            .OrderByDescending(r => r.CompletedAt)
            .FirstOrDefault();

        var previousCompleted = reviews
            .Where(r => r.Status == GovernanceReviewStatus.Completed && r.GovernanceScore.HasValue && r.Id != latestCompleted?.Id)
            .OrderByDescending(r => r.CompletedAt)
            .FirstOrDefault();

        double? scoreTrend = null;
        if (latestCompleted?.GovernanceScore != null && previousCompleted?.GovernanceScore != null)
        {
            scoreTrend = latestCompleted.GovernanceScore.Value - previousCompleted.GovernanceScore.Value;
        }

        var upcomingReviews = reviews
            .Where(r => r.Status == GovernanceReviewStatus.Scheduled && r.ReviewDate >= DateTime.UtcNow)
            .OrderBy(r => r.ReviewDate)
            .Take(5)
            .Select(MapReviewToDto)
            .ToList();

        var overdueActionItems = allActions
            .Where(a => a.IsOverdue)
            .OrderBy(a => a.DueDate)
            .Take(10)
            .Select(MapActionToDto)
            .ToList();

        return new GovernanceDashboardDto
        {
            TotalReviews = reviews.Count,
            CompletedReviews = reviews.Count(r => r.Status == GovernanceReviewStatus.Completed),
            OverdueReviews = reviews.Count(r => r.IsOverdue),
            OpenActions = allActions.Count(a => a.Status == GovernanceActionStatus.Open || a.Status == GovernanceActionStatus.InProgress),
            OverdueActions = allActions.Count(a => a.IsOverdue),
            LatestGovernanceScore = latestCompleted?.GovernanceScore,
            ScoreTrend = scoreTrend,
            UpcomingReviews = upcomingReviews,
            OverdueActionItems = overdueActionItems
        };
    }

    public async Task<GovernanceReviewDto> CreateReviewAsync(
        CreateGovernanceReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        var review = GovernanceReview.Create(
            LocalizedString.Create(request.TitleEnglish, request.TitleArabic),
            request.ReviewType,
            _currentUser.UserId ?? Guid.Empty,
            _currentUser.DisplayName ?? "System",
            request.ReviewDate,
            request.Scope,
            request.ScopeArabic);

        _dbContext.Set<GovernanceReview>().Add(review);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created governance review {ReviewId}", review.Id);

        return MapReviewToDto(review);
    }

    public async Task<GovernanceReviewDto?> GetReviewByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var review = await _dbContext.Set<GovernanceReview>().AsNoTracking()
            .Include(r => r.Actions)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        return review == null ? null : MapReviewToDto(review);
    }

    public async Task<IReadOnlyList<GovernanceReviewDto>> ListReviewsAsync(
        GovernanceReviewStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<GovernanceReview>().AsNoTracking()
            .Include(r => r.Actions)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        var reviews = await query
            .OrderByDescending(r => r.ReviewDate)
            .ToListAsync(cancellationToken);

        return reviews.Select(MapReviewToDto).ToList();
    }

    public async Task<bool> StartReviewAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var review = await _dbContext.Set<GovernanceReview>()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (review == null) return false;

        review.StartReview();
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> RecordFindingsAsync(
        Guid id,
        RecordFindingsRequest request,
        CancellationToken cancellationToken = default)
    {
        var review = await _dbContext.Set<GovernanceReview>()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (review == null) return false;

        review.RecordFindings(
            request.Findings,
            request.FindingsArabic,
            request.TotalFindings,
            request.CriticalFindings,
            request.GovernanceScore);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> SetRemediationPlanAsync(
        Guid id,
        SetRemediationPlanRequest request,
        CancellationToken cancellationToken = default)
    {
        var review = await _dbContext.Set<GovernanceReview>()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (review == null) return false;

        review.SetRemediationPlan(request.Plan, request.PlanArabic, request.Deadline);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> CompleteReviewAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var review = await _dbContext.Set<GovernanceReview>()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (review == null) return false;

        // Get previous review's score for trend tracking
        var previousReview = await _dbContext.Set<GovernanceReview>().AsNoTracking()
            .Where(r => r.Status == GovernanceReviewStatus.Completed && r.GovernanceScore.HasValue && r.Id != id)
            .OrderByDescending(r => r.CompletedAt)
            .FirstOrDefaultAsync(cancellationToken);

        review.Complete(previousReview?.GovernanceScore);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Completed governance review {ReviewId} with score {Score}", id, review.GovernanceScore);

        return true;
    }

    public async Task<GovernanceActionDto> AddActionAsync(
        Guid reviewId,
        CreateGovernanceActionRequest request,
        CancellationToken cancellationToken = default)
    {
        var review = await _dbContext.Set<GovernanceReview>()
            .Include(r => r.Actions)
            .FirstOrDefaultAsync(r => r.Id == reviewId, cancellationToken);

        if (review == null)
            throw new InvalidOperationException($"Governance review {reviewId} not found");

        var action = GovernanceAction.Create(
            reviewId,
            LocalizedString.Create(request.TitleEnglish, request.TitleArabic),
            request.Description,
            request.DescriptionArabic,
            request.Priority,
            request.AssigneeId,
            request.AssigneeName,
            request.DueDate);

        review.AddAction(action);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Added governance action {ActionId} to review {ReviewId}", action.Id, reviewId);

        return MapActionToDto(action);
    }

    public async Task<bool> CompleteActionAsync(
        Guid actionId,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        var action = await _dbContext.Set<GovernanceAction>()
            .FirstOrDefaultAsync(a => a.Id == actionId, cancellationToken);

        if (action == null) return false;

        action.Complete(notes);

        // Update review's resolved findings count
        var review = await _dbContext.Set<GovernanceReview>()
            .FirstOrDefaultAsync(r => r.Id == action.GovernanceReviewId, cancellationToken);

        review?.ResolveFindings(1);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static GovernanceReviewDto MapReviewToDto(GovernanceReview review)
    {
        return new GovernanceReviewDto
        {
            Id = review.Id,
            TitleEnglish = review.Title?.English ?? string.Empty,
            TitleArabic = review.Title?.Arabic ?? string.Empty,
            ReviewType = review.ReviewType.ToString(),
            Status = review.Status.ToString(),
            ReviewDate = review.ReviewDate,
            CompletedAt = review.CompletedAt,
            ReviewerId = review.ReviewerId,
            ReviewerName = review.ReviewerName,
            Scope = review.Scope,
            Findings = review.Findings,
            TotalFindings = review.TotalFindings,
            CriticalFindings = review.CriticalFindings,
            ResolvedFindings = review.ResolvedFindings,
            RemediationPlan = review.RemediationPlan,
            RemediationDeadline = review.RemediationDeadline,
            GovernanceScore = review.GovernanceScore,
            PreviousScore = review.PreviousScore,
            RemediationProgress = review.RemediationProgress,
            IsOverdue = review.IsOverdue,
            Actions = review.Actions?.Select(MapActionToDto).ToList() ?? new List<GovernanceActionDto>()
        };
    }

    private static GovernanceActionDto MapActionToDto(GovernanceAction action)
    {
        return new GovernanceActionDto
        {
            Id = action.Id,
            GovernanceReviewId = action.GovernanceReviewId,
            TitleEnglish = action.Title?.English ?? string.Empty,
            TitleArabic = action.Title?.Arabic ?? string.Empty,
            Description = action.Description,
            Priority = action.Priority.ToString(),
            Status = action.Status.ToString(),
            AssigneeId = action.AssigneeId,
            AssigneeName = action.AssigneeName,
            DueDate = action.DueDate,
            CompletedAt = action.CompletedAt,
            CompletionNotes = action.CompletionNotes,
            IsOverdue = action.IsOverdue
        };
    }
}
