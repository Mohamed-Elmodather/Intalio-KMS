using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AFC27.KMS.Content.Application.Services;

/// <summary>
/// Background service that recalculates content health scores on a weekly schedule.
/// Runs every 7 days, iterating all articles and updating their HealthScore property.
/// </summary>
public class ContentHealthCalculationJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ContentHealthCalculationJob> _logger;
    private static readonly TimeSpan Interval = TimeSpan.FromDays(7);

    public ContentHealthCalculationJob(
        IServiceScopeFactory scopeFactory,
        ILogger<ContentHealthCalculationJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ContentHealthCalculationJob started. Interval: {Interval}", Interval);

        // Run once on startup after a short delay to let the app warm up
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RecalculateAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during content health score recalculation");
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

        _logger.LogInformation("ContentHealthCalculationJob stopped");
    }

    private async Task RecalculateAsync(CancellationToken ct)
    {
        _logger.LogInformation("Starting weekly content health score recalculation...");

        using var scope = _scopeFactory.CreateScope();
        var healthService = scope.ServiceProvider.GetRequiredService<IContentHealthService>();

        var count = await healthService.RecalculateAllHealthScoresAsync(ct);

        _logger.LogInformation(
            "Content health score recalculation complete. {Count} articles processed", count);
    }
}
