using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Application.Interfaces;

namespace AFC27.KMS.Admin.Presentation.Controllers;

/// <summary>
/// Controller for advanced analytics endpoints.
/// </summary>
[ApiController]
[Route("api/admin/analytics")]
[Authorize(Policy = "CanManageUsers")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    /// <summary>
    /// Get platform usage metrics for a time range.
    /// </summary>
    [HttpGet("usage")]
    [ProducesResponseType(typeof(UsageMetricsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UsageMetricsDto>> GetUsageMetrics(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetUsageMetricsAsync(from, to, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get engagement trends across content types.
    /// </summary>
    [HttpGet("engagement")]
    [ProducesResponseType(typeof(EngagementTrendsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<EngagementTrendsDto>> GetEngagementTrends(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetEngagementTrendsAsync(from, to, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get search analytics.
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(SearchAnalyticsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SearchAnalyticsDto>> GetSearchAnalytics(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetSearchAnalyticsAsync(from, to, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get AI usage statistics.
    /// </summary>
    [HttpGet("ai-usage")]
    [ProducesResponseType(typeof(AIUsageStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AIUsageStatsDto>> GetAIUsageStats(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetAIUsageStatsAsync(from, to, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get content growth statistics.
    /// </summary>
    [HttpGet("content-growth")]
    [ProducesResponseType(typeof(ContentGrowthDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ContentGrowthDto>> GetContentGrowth(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetContentGrowthAsync(from, to, cancellationToken);
        return Ok(result);
    }
}
