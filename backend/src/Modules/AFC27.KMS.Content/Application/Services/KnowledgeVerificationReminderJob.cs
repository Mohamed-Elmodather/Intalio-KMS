using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Content.Domain.Entities;

namespace AFC27.KMS.Content.Application.Services;

/// <summary>
/// Background service that runs daily to check article verification deadlines.
/// Updates articles whose verification is due within 7 days to DueSoon status,
/// and articles past their verification date to Overdue status.
/// </summary>
public class KnowledgeVerificationReminderJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<KnowledgeVerificationReminderJob> _logger;
    private static readonly TimeSpan Interval = TimeSpan.FromHours(24);
    private const int DueSoonWindowDays = 7;

    public KnowledgeVerificationReminderJob(
        IServiceScopeFactory scopeFactory,
        ILogger<KnowledgeVerificationReminderJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("KnowledgeVerificationReminderJob started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessVerificationStatusesAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing knowledge verification reminders");
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

        _logger.LogInformation("KnowledgeVerificationReminderJob stopped");
    }

    private async Task ProcessVerificationStatusesAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

        var now = DateTime.UtcNow;
        var dueSoonThreshold = now.AddDays(DueSoonWindowDays);

        // Mark articles as Overdue: NextVerificationDue is in the past and status is not already Overdue
        var overdueArticles = await dbContext.Set<Article>()
            .Where(a => a.NextVerificationDue != null
                     && a.NextVerificationDue <= now
                     && a.VerificationStatus != VerificationStatus.Overdue
                     && a.VerificationStatus != VerificationStatus.Unverified)
            .ToListAsync(ct);

        foreach (var article in overdueArticles)
        {
            article.MarkVerificationOverdue();
        }

        // Mark articles as DueSoon: NextVerificationDue is within 7 days and currently Verified
        var dueSoonArticles = await dbContext.Set<Article>()
            .Where(a => a.NextVerificationDue != null
                     && a.NextVerificationDue > now
                     && a.NextVerificationDue <= dueSoonThreshold
                     && a.VerificationStatus == VerificationStatus.Verified)
            .ToListAsync(ct);

        foreach (var article in dueSoonArticles)
        {
            article.MarkVerificationDue();
        }

        if (overdueArticles.Count > 0 || dueSoonArticles.Count > 0)
        {
            await dbContext.SaveChangesAsync(ct);
        }

        _logger.LogInformation(
            "Verification reminder processing complete: {OverdueCount} marked overdue, {DueSoonCount} marked due soon",
            overdueArticles.Count, dueSoonArticles.Count);
    }
}
