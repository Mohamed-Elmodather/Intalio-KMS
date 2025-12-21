using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;

namespace AFC27.KMS.WebApi.Integration.Core;

/// <summary>
/// Base class for external service clients with built-in resilience patterns
/// </summary>
public abstract class ExternalServiceClientBase : IExternalServiceClient
{
    protected readonly HttpClient HttpClient;
    protected readonly ILogger Logger;
    protected readonly JsonSerializerOptions JsonOptions;

    private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreaker;
    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;
    private readonly IAsyncPolicy<HttpResponseMessage> _combinedPolicy;
    private CircuitBreakerState _currentState = CircuitBreakerState.Closed;

    public abstract string ServiceName { get; }

    protected ExternalServiceClientBase(
        HttpClient httpClient,
        ILogger logger,
        ExternalServiceSettings settings)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));

        JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        // Configure retry policy with exponential backoff
        _retryPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => (int)r.StatusCode >= 500 || r.StatusCode == System.Net.HttpStatusCode.RequestTimeout)
            .WaitAndRetryAsync(
                settings.RetryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    Logger.LogWarning(
                        "Retry {RetryAttempt} for {ServiceName} after {Delay}ms. Reason: {Reason}",
                        retryAttempt,
                        ServiceName,
                        timespan.TotalMilliseconds,
                        outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
                });

        // Configure circuit breaker
        _circuitBreaker = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => (int)r.StatusCode >= 500)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: settings.CircuitBreakerThreshold,
                durationOfBreak: TimeSpan.FromSeconds(settings.CircuitBreakerDurationSeconds),
                onBreak: (outcome, duration) =>
                {
                    _currentState = CircuitBreakerState.Open;
                    Logger.LogError(
                        "Circuit breaker opened for {ServiceName}. Duration: {Duration}s. Reason: {Reason}",
                        ServiceName,
                        duration.TotalSeconds,
                        outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
                },
                onReset: () =>
                {
                    _currentState = CircuitBreakerState.Closed;
                    Logger.LogInformation("Circuit breaker reset for {ServiceName}", ServiceName);
                },
                onHalfOpen: () =>
                {
                    _currentState = CircuitBreakerState.HalfOpen;
                    Logger.LogInformation("Circuit breaker half-open for {ServiceName}", ServiceName);
                });

        // Combine policies: retry inside circuit breaker
        _combinedPolicy = Policy.WrapAsync(_circuitBreaker, _retryPolicy);
    }

    public CircuitBreakerState GetCircuitBreakerState() => _currentState;

    public virtual async Task<ServiceHealthStatus> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var response = await HttpClient.GetAsync("health", cancellationToken);
            stopwatch.Stop();

            return new ServiceHealthStatus
            {
                IsHealthy = response.IsSuccessStatusCode,
                ServiceName = ServiceName,
                Message = response.IsSuccessStatusCode ? "Service is healthy" : $"Status code: {response.StatusCode}",
                ResponseTime = stopwatch.Elapsed,
                CheckedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new ServiceHealthStatus
            {
                IsHealthy = false,
                ServiceName = ServiceName,
                Message = ex.Message,
                ResponseTime = stopwatch.Elapsed,
                CheckedAt = DateTime.UtcNow
            };
        }
    }

    protected async Task<ServiceResponse<TResponse>> SendRequestAsync<TRequest, TResponse>(
        HttpMethod method,
        string endpoint,
        TRequest? request = default,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();

        try
        {
            Logger.LogInformation(
                "Sending {Method} request to {ServiceName}/{Endpoint}. RequestId: {RequestId}",
                method.Method, ServiceName, endpoint, requestId);

            var httpRequest = new HttpRequestMessage(method, endpoint);

            if (request != null && (method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Patch))
            {
                var json = JsonSerializer.Serialize(request, JsonOptions);
                httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var response = await _combinedPolicy.ExecuteAsync(
                async ct => await HttpClient.SendAsync(httpRequest, ct),
                cancellationToken);

            stopwatch.Stop();

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, cancellationToken);

                Logger.LogInformation(
                    "Request to {ServiceName}/{Endpoint} completed successfully in {Duration}ms. RequestId: {RequestId}",
                    ServiceName, endpoint, stopwatch.ElapsedMilliseconds, requestId);

                return ServiceResponse<TResponse>.Success(data!, (int)response.StatusCode, stopwatch.Elapsed);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);

                Logger.LogWarning(
                    "Request to {ServiceName}/{Endpoint} failed with status {StatusCode}. RequestId: {RequestId}. Error: {Error}",
                    ServiceName, endpoint, response.StatusCode, requestId, errorContent);

                return ServiceResponse<TResponse>.Failure(
                    errorContent,
                    response.StatusCode.ToString(),
                    (int)response.StatusCode);
            }
        }
        catch (BrokenCircuitException ex)
        {
            stopwatch.Stop();
            Logger.LogError(ex,
                "Circuit breaker is open for {ServiceName}. RequestId: {RequestId}",
                ServiceName, requestId);

            return ServiceResponse<TResponse>.Failure(
                $"Service {ServiceName} is temporarily unavailable",
                "CIRCUIT_BREAKER_OPEN",
                503);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Logger.LogError(ex,
                "Request to {ServiceName}/{Endpoint} failed. RequestId: {RequestId}",
                ServiceName, endpoint, requestId);

            return ServiceResponse<TResponse>.Failure(ex.Message, "REQUEST_FAILED", 500);
        }
    }

    protected async Task<ServiceResponse<TResponse>> GetAsync<TResponse>(
        string endpoint,
        CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<object, TResponse>(HttpMethod.Get, endpoint, null, cancellationToken);
    }

    protected async Task<ServiceResponse<TResponse>> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<TRequest, TResponse>(HttpMethod.Post, endpoint, request, cancellationToken);
    }

    protected async Task<ServiceResponse<TResponse>> PutAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<TRequest, TResponse>(HttpMethod.Put, endpoint, request, cancellationToken);
    }

    protected async Task<ServiceResponse<TResponse>> DeleteAsync<TResponse>(
        string endpoint,
        CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<object, TResponse>(HttpMethod.Delete, endpoint, null, cancellationToken);
    }
}

/// <summary>
/// Base settings for external services
/// </summary>
public class ExternalServiceSettings
{
    public bool IsEnabled { get; set; } = false;
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
    public int RetryCount { get; set; } = 3;
    public int CircuitBreakerThreshold { get; set; } = 5;
    public int CircuitBreakerDurationSeconds { get; set; } = 30;
}
