using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AFC27.KMS.WebApi.Integration.Core;

/// <summary>
/// Monitors the health of all external service integrations
/// </summary>
public interface IIntegrationHealthMonitor
{
    /// <summary>
    /// Registers an external service client for health monitoring
    /// </summary>
    void RegisterService(IExternalServiceClient client);

    /// <summary>
    /// Checks health of all registered services
    /// </summary>
    Task<IntegrationHealthReport> CheckAllServicesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the last known health status of a specific service
    /// </summary>
    ServiceHealthStatus? GetServiceHealth(string serviceName);

    /// <summary>
    /// Gets the overall integration health status
    /// </summary>
    IntegrationHealthSummary GetHealthSummary();
}

public class IntegrationHealthMonitor : IIntegrationHealthMonitor
{
    private readonly ILogger<IntegrationHealthMonitor> _logger;
    private readonly ConcurrentDictionary<string, IExternalServiceClient> _clients = new();
    private readonly ConcurrentDictionary<string, ServiceHealthStatus> _healthStatuses = new();

    public IntegrationHealthMonitor(ILogger<IntegrationHealthMonitor> logger)
    {
        _logger = logger;
    }

    public void RegisterService(IExternalServiceClient client)
    {
        if (_clients.TryAdd(client.ServiceName, client))
        {
            _logger.LogInformation("Registered service {ServiceName} for health monitoring", client.ServiceName);
        }
    }

    public async Task<IntegrationHealthReport> CheckAllServicesAsync(CancellationToken cancellationToken = default)
    {
        var report = new IntegrationHealthReport
        {
            CheckedAt = DateTime.UtcNow,
            ServiceStatuses = new List<ServiceHealthStatus>()
        };

        var tasks = _clients.Values.Select(async client =>
        {
            try
            {
                var status = await client.CheckHealthAsync(cancellationToken);
                status.ServiceName = client.ServiceName;
                _healthStatuses[client.ServiceName] = status;
                return status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check health for {ServiceName}", client.ServiceName);
                var status = new ServiceHealthStatus
                {
                    ServiceName = client.ServiceName,
                    IsHealthy = false,
                    Message = ex.Message,
                    CheckedAt = DateTime.UtcNow
                };
                _healthStatuses[client.ServiceName] = status;
                return status;
            }
        });

        var results = await Task.WhenAll(tasks);
        report.ServiceStatuses = results.ToList();
        report.IsHealthy = results.All(s => s.IsHealthy);
        report.HealthyCount = results.Count(s => s.IsHealthy);
        report.UnhealthyCount = results.Count(s => !s.IsHealthy);

        return report;
    }

    public ServiceHealthStatus? GetServiceHealth(string serviceName)
    {
        return _healthStatuses.TryGetValue(serviceName, out var status) ? status : null;
    }

    public IntegrationHealthSummary GetHealthSummary()
    {
        var statuses = _healthStatuses.Values.ToList();
        var circuitBreakerStates = _clients.ToDictionary(
            c => c.Key,
            c => c.Value.GetCircuitBreakerState());

        return new IntegrationHealthSummary
        {
            TotalServices = _clients.Count,
            HealthyServices = statuses.Count(s => s.IsHealthy),
            UnhealthyServices = statuses.Count(s => !s.IsHealthy),
            OpenCircuitBreakers = circuitBreakerStates.Count(c => c.Value == CircuitBreakerState.Open),
            ServiceStates = circuitBreakerStates,
            LastChecked = statuses.Any() ? statuses.Max(s => s.CheckedAt) : null
        };
    }
}

/// <summary>
/// Report of all integration health checks
/// </summary>
public class IntegrationHealthReport
{
    public bool IsHealthy { get; set; }
    public DateTime CheckedAt { get; set; }
    public int HealthyCount { get; set; }
    public int UnhealthyCount { get; set; }
    public List<ServiceHealthStatus> ServiceStatuses { get; set; } = new();
}

/// <summary>
/// Summary of integration health
/// </summary>
public class IntegrationHealthSummary
{
    public int TotalServices { get; set; }
    public int HealthyServices { get; set; }
    public int UnhealthyServices { get; set; }
    public int OpenCircuitBreakers { get; set; }
    public Dictionary<string, CircuitBreakerState> ServiceStates { get; set; } = new();
    public DateTime? LastChecked { get; set; }
}
