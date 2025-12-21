using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.WebApi.Integration.Core;

namespace AFC27.KMS.WebApi.Integration.Controllers;

/// <summary>
/// Controller for monitoring integration service health
/// </summary>
[ApiController]
[Route("api/integration/health")]
[Authorize(Policy = "IntegrationAdmin")]
public class IntegrationHealthController : ControllerBase
{
    private readonly IIntegrationHealthMonitor _healthMonitor;
    private readonly ILogger<IntegrationHealthController> _logger;

    public IntegrationHealthController(
        IIntegrationHealthMonitor healthMonitor,
        ILogger<IntegrationHealthController> logger)
    {
        _healthMonitor = healthMonitor;
        _logger = logger;
    }

    /// <summary>
    /// Gets health status of all integration services
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IntegrationHealthReport>> GetHealthReport(
        CancellationToken cancellationToken)
    {
        var report = await _healthMonitor.CheckAllServicesAsync(cancellationToken);
        return Ok(report);
    }

    /// <summary>
    /// Gets health summary for dashboard display
    /// </summary>
    [HttpGet("summary")]
    public ActionResult<IntegrationHealthSummary> GetHealthSummary()
    {
        var summary = _healthMonitor.GetHealthSummary();
        return Ok(summary);
    }

    /// <summary>
    /// Gets health status of a specific service
    /// </summary>
    [HttpGet("{serviceName}")]
    public ActionResult<ServiceHealthStatus> GetServiceHealth(string serviceName)
    {
        var status = _healthMonitor.GetServiceHealth(serviceName);
        if (status == null)
        {
            return NotFound(new { message = $"Service '{serviceName}' not found" });
        }
        return Ok(status);
    }
}
