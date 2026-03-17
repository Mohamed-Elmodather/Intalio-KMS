using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Collaboration.Application.DTOs;
using AFC27.KMS.Collaboration.Application.Interfaces;
using AFC27.KMS.Collaboration.Domain.Entities;

namespace AFC27.KMS.Collaboration.Application.Services;

/// <summary>
/// Service implementation for Lesson Action CRUD and lifecycle management.
/// </summary>
public class LessonActionService : ILessonActionService
{
    private readonly DbContext _dbContext;
    private readonly ILogger<LessonActionService> _logger;

    public LessonActionService(DbContext dbContext, ILogger<LessonActionService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<List<LessonActionDto>> GetActionsAsync(Guid lessonId, CancellationToken ct = default)
    {
        return await _dbContext.Set<LessonAction>()
            .AsNoTracking()
            .Where(a => a.LessonLearnedId == lessonId)
            .OrderBy(a => a.SortOrder)
            .ThenBy(a => a.CreatedAt)
            .Select(a => new LessonActionDto
            {
                Id = a.Id,
                LessonLearnedId = a.LessonLearnedId,
                Description = a.Description,
                DescriptionArabic = a.DescriptionArabic,
                AssigneeId = a.AssigneeId,
                AssigneeName = a.AssigneeName,
                DelegatedToId = a.DelegatedToId,
                DelegatedToName = a.DelegatedToName,
                Status = a.Status.ToString(),
                Priority = a.Priority.ToString(),
                DueDate = a.DueDate,
                StartedAt = a.StartedAt,
                CompletedAt = a.CompletedAt,
                CompletionNotes = a.CompletionNotes,
                CompletionNotesArabic = a.CompletionNotesArabic,
                VerifiedAt = a.VerifiedAt,
                VerifiedByName = a.VerifiedByName,
                VerificationNotes = a.VerificationNotes,
                AffectedDocumentId = a.AffectedDocumentId,
                AffectedDocumentTitle = a.AffectedDocumentTitle,
                AffectedDocumentType = a.AffectedDocumentType,
                IsOverdue = (a.Status == LessonActionStatus.Open || a.Status == LessonActionStatus.InProgress)
                            && a.DueDate < DateTime.UtcNow,
                ReminderCount = a.ReminderCount,
                EscalatedAt = a.EscalatedAt,
                EscalatedToName = a.EscalatedToName,
                SortOrder = a.SortOrder,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync(ct);
    }

    public async Task<LessonActionDto> CreateActionAsync(
        Guid lessonId, CreateLessonActionRequest request, Guid userId, CancellationToken ct = default)
    {
        // Verify the lesson exists
        var lesson = await _dbContext.Set<LessonLearned>()
            .FirstOrDefaultAsync(l => l.Id == lessonId, ct);

        if (lesson == null)
            throw new InvalidOperationException($"Lesson {lessonId} not found.");

        Enum.TryParse<LessonActionPriority>(request.Priority, true, out var priority);

        // Determine sort order (append to end)
        var maxSortOrder = await _dbContext.Set<LessonAction>()
            .Where(a => a.LessonLearnedId == lessonId)
            .Select(a => (int?)a.SortOrder)
            .MaxAsync(ct) ?? -1;

        var action = LessonAction.Create(
            lessonId,
            request.Description,
            request.DescriptionArabic,
            request.AssigneeId,
            "Assigned User", // In production, resolve from user service
            request.DueDate,
            priority,
            maxSortOrder + 1,
            request.AffectedDocumentId,
            request.AffectedDocumentTitle,
            request.AffectedDocumentType);

        _dbContext.Set<LessonAction>().Add(action);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Action {ActionId} created for lesson {LessonId} by {UserId}",
            action.Id, lessonId, userId);

        return new LessonActionDto
        {
            Id = action.Id,
            LessonLearnedId = action.LessonLearnedId,
            Description = action.Description,
            DescriptionArabic = action.DescriptionArabic,
            AssigneeId = action.AssigneeId,
            AssigneeName = action.AssigneeName,
            Status = action.Status.ToString(),
            Priority = action.Priority.ToString(),
            DueDate = action.DueDate,
            AffectedDocumentId = action.AffectedDocumentId,
            AffectedDocumentTitle = action.AffectedDocumentTitle,
            AffectedDocumentType = action.AffectedDocumentType,
            IsOverdue = false,
            ReminderCount = 0,
            SortOrder = action.SortOrder,
            CreatedAt = action.CreatedAt
        };
    }

    public async Task<bool> UpdateActionAsync(
        Guid lessonId, Guid actionId, UpdateLessonActionRequest request, Guid userId, CancellationToken ct = default)
    {
        var action = await _dbContext.Set<LessonAction>()
            .FirstOrDefaultAsync(a => a.Id == actionId && a.LessonLearnedId == lessonId, ct);

        if (action == null)
            return false;

        Enum.TryParse<LessonActionPriority>(request.Priority, true, out var priority);

        action.Update(request.Description, request.DescriptionArabic, request.DueDate, priority);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Action {ActionId} updated for lesson {LessonId} by {UserId}",
            actionId, lessonId, userId);
        return true;
    }

    public async Task<bool> StartActionAsync(
        Guid lessonId, Guid actionId, Guid userId, CancellationToken ct = default)
    {
        var action = await _dbContext.Set<LessonAction>()
            .FirstOrDefaultAsync(a => a.Id == actionId && a.LessonLearnedId == lessonId, ct);

        if (action == null)
            return false;

        action.StartProgress();
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Action {ActionId} started for lesson {LessonId} by {UserId}",
            actionId, lessonId, userId);
        return true;
    }

    public async Task<bool> CompleteActionAsync(
        Guid lessonId, Guid actionId, CompleteActionRequest request, Guid userId, CancellationToken ct = default)
    {
        var action = await _dbContext.Set<LessonAction>()
            .FirstOrDefaultAsync(a => a.Id == actionId && a.LessonLearnedId == lessonId, ct);

        if (action == null)
            return false;

        action.Complete(userId, request.Notes, request.NotesArabic);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Action {ActionId} completed for lesson {LessonId} by {UserId}",
            actionId, lessonId, userId);
        return true;
    }

    public async Task<bool> VerifyActionAsync(
        Guid lessonId, Guid actionId, VerifyActionRequest request,
        Guid userId, string verifierName, CancellationToken ct = default)
    {
        var action = await _dbContext.Set<LessonAction>()
            .FirstOrDefaultAsync(a => a.Id == actionId && a.LessonLearnedId == lessonId, ct);

        if (action == null)
            return false;

        action.Verify(userId, verifierName, request.Notes);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Action {ActionId} verified for lesson {LessonId} by {UserId}",
            actionId, lessonId, userId);
        return true;
    }

    public async Task<bool> CancelActionAsync(
        Guid lessonId, Guid actionId, Guid userId, CancellationToken ct = default)
    {
        var action = await _dbContext.Set<LessonAction>()
            .FirstOrDefaultAsync(a => a.Id == actionId && a.LessonLearnedId == lessonId, ct);

        if (action == null)
            return false;

        action.Cancel();
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Action {ActionId} cancelled for lesson {LessonId} by {UserId}",
            actionId, lessonId, userId);
        return true;
    }

    public async Task<bool> DelegateActionAsync(
        Guid lessonId, Guid actionId, DelegateActionRequest request, Guid userId, CancellationToken ct = default)
    {
        var action = await _dbContext.Set<LessonAction>()
            .FirstOrDefaultAsync(a => a.Id == actionId && a.LessonLearnedId == lessonId, ct);

        if (action == null)
            return false;

        action.Delegate(request.DelegateToId, request.DelegateToName);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Action {ActionId} delegated to {DelegateName} for lesson {LessonId} by {UserId}",
            actionId, request.DelegateToName, lessonId, userId);
        return true;
    }

    public async Task<bool> LinkDocumentAsync(
        Guid lessonId, Guid actionId, LinkDocumentRequest request, Guid userId, CancellationToken ct = default)
    {
        var action = await _dbContext.Set<LessonAction>()
            .FirstOrDefaultAsync(a => a.Id == actionId && a.LessonLearnedId == lessonId, ct);

        if (action == null)
            return false;

        action.LinkDocument(request.DocumentId, request.DocumentTitle, request.DocumentType);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Document {DocumentId} linked to action {ActionId} for lesson {LessonId} by {UserId}",
            request.DocumentId, actionId, lessonId, userId);
        return true;
    }
}
