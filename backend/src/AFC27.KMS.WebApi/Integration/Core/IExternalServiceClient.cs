using System;
using System.Threading;
using System.Threading.Tasks;

namespace AFC27.KMS.WebApi.Integration.Core;

/// <summary>
/// Base interface for all external service clients
/// </summary>
public interface IExternalServiceClient
{
    /// <summary>
    /// Gets the name of the external service
    /// </summary>
    string ServiceName { get; }

    /// <summary>
    /// Checks if the external service is healthy and available
    /// </summary>
    Task<ServiceHealthStatus> CheckHealthAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current circuit breaker state
    /// </summary>
    CircuitBreakerState GetCircuitBreakerState();
}

/// <summary>
/// Generic interface for external service clients with typed requests/responses
/// </summary>
public interface IExternalServiceClient<TRequest, TResponse> : IExternalServiceClient
{
    /// <summary>
    /// Executes a request to the external service
    /// </summary>
    Task<ServiceResponse<TResponse>> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents the health status of an external service
/// </summary>
public class ServiceHealthStatus
{
    public bool IsHealthy { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string? Message { get; set; }
    public TimeSpan ResponseTime { get; set; }
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Circuit breaker states
/// </summary>
public enum CircuitBreakerState
{
    Closed,      // Normal operation
    Open,        // Blocking requests
    HalfOpen     // Testing if service recovered
}

/// <summary>
/// Generic service response wrapper
/// </summary>
public class ServiceResponse<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public int HttpStatusCode { get; set; }
    public TimeSpan ResponseTime { get; set; }
    public string? RequestId { get; set; }

    public static ServiceResponse<T> Success(T data, int statusCode = 200, TimeSpan? responseTime = null)
    {
        return new ServiceResponse<T>
        {
            IsSuccess = true,
            Data = data,
            HttpStatusCode = statusCode,
            ResponseTime = responseTime ?? TimeSpan.Zero,
            RequestId = Guid.NewGuid().ToString()
        };
    }

    public static ServiceResponse<T> Failure(string errorMessage, string? errorCode = null, int statusCode = 500)
    {
        return new ServiceResponse<T>
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode,
            HttpStatusCode = statusCode,
            RequestId = Guid.NewGuid().ToString()
        };
    }
}
