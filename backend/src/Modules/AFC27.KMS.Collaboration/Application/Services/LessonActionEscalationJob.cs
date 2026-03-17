using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Collaboration.Domain.Entities;

namespace AFC27.KMS.Collaboration.Application.Services;

/// <summary>
/// Background service that periodically checks for overdue lesson actions,
/// increments reminder counts, and escalates to process owners when thresholds are reached.
/// Runs every hour.
/// </summary>
public class LessonActionEscalationJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LessonActionEscalationJob> _logger;
    private static readonly TimeSpan Interval = TimeSpan.FromHours(1);
    private const int EscalationThreshold = 3;

    public LessonActionEscalationJob(
        IServiceScopeFactory scopeFactory,
        ILogger<LessonActionEscalationJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("LessonActionEscalationJob started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOverdueActionsAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Graceful shutdown
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing overdue lesson actions");
            }

            try
            {
                await Task.Delay(Interval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation("LessonActionEscalationJob stopped");
    }

    private async Task ProcessOverdueActionsAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        // Query overdue actions: Open or InProgress with DueDate in the past
        var overdueActions = await dbContext.Set<LessonAction>()
            .Include(a => a.LessonLearned)
            .Where(a => (a.Status == LessonActionStatus.Open || a.Status == LessonActionStatus.InProgress)
                     && a.DueDate < DateTime.UtcNow)
            .ToListAsync(ct);

        if (overdueActions.Count == 0)
        {
            _logger.LogDebug("No overdue lesson actions found");
            return;
        }

        var remindedCount = 0;
        var escalatedCount = 0;

        foreach (var action in overdueActions)
        {
            action.IncrementReminder();
            remindedCount++;

            // Escalate to process owner if threshold reached and not already escalated
            if (action.ReminderCount >= EscalationThreshold && action.EscalatedAt == null)
            {
                var lesson = action.LessonLearned;

                if (lesson?.ProcessOwnerId != null && !string.IsNullOrEmpty(lesson.ProcessOwnerName))
                {
                    action.Escalate(lesson.ProcessOwnerId.Value, lesson.ProcessOwnerName);
                    escalatedCount++;

                    _logger.LogWarning(
                        "Action {ActionId} for lesson {LessonId} escalated to process owner {OwnerName} " +
                        "(overdue since {DueDate}, reminders: {ReminderCount})",
                        action.Id, action.LessonLearnedId, lesson.ProcessOwnerName,
                        action.DueDate, action.ReminderCount);
                }
                else
                {
                    _logger.LogWarning(
                        "Action {ActionId} for lesson {LessonId} reached escalation threshold " +
                        "but lesson has no process owner assigned",
                        action.Id, action.LessonLearnedId);
                }
            }
        }

        await dbContext.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Overdue action processing complete: {TotalOverdue} overdue, {Reminded} reminded, {Escalated} escalated",
            overdueActions.Count, remindedCount, escalatedCount);
    }
}
