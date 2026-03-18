using AFC27.KMS.Admin.Application.DTOs;

namespace AFC27.KMS.Admin.Application.Interfaces;

/// <summary>
/// Service interface for advanced analytics.
/// </summary>
public interface IAnalyticsService
{
    /// <summary>
    /// Get usage metrics for a given time range.
    /// </summary>
    Task<UsageMetricsDto> GetUsageMetricsAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get engagement trends for a given time range.
    /// </summary>
    Task<EngagementTrendsDto> GetEngagementTrendsAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get search analytics for a given time range.
    /// </summary>
    Task<SearchAnalyticsDto> GetSearchAnalyticsAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get AI usage statistics for a given time range.
    /// </summary>
    Task<AIUsageStatsDto> GetAIUsageStatsAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get content growth statistics for a given time range.
    /// </summary>
    Task<ContentGrowthDto> GetContentGrowthAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);
}
