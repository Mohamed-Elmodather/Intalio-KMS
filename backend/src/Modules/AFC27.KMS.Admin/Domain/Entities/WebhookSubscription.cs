using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Admin.Domain.Entities;

/// <summary>
/// External webhook subscription for receiving event notifications.
/// Supports event filtering, secret-based signature verification, and retry logic.
/// </summary>
public class WebhookSubscription : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;
    public string Secret { get; private set; } = string.Empty;
    public List<string> EventFilters { get; private set; } = new();
    public bool IsActive { get; private set; } = true;
    public int MaxRetries { get; private set; } = 3;
    public int RetryDelaySeconds { get; private set; } = 30;
    public int TimeoutSeconds { get; private set; } = 30;
    public string? Description { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public string CreatedByName { get; private set; } = string.Empty;
    public DateTime? LastTriggeredAt { get; private set; }
    public int TotalDeliveries { get; private set; }
    public int FailedDeliveries { get; private set; }

    // Navigation
    public virtual ICollection<WebhookDeliveryLog> DeliveryLogs { get; private set; } = new List<WebhookDeliveryLog>();

    private WebhookSubscription() { }

    public static WebhookSubscription Create(
        string name,
        string url,
        string secret,
        List<string> eventFilters,
        Guid createdByUserId,
        string createdByName,
        string? description = null,
        int maxRetries = 3,
        int retryDelaySeconds = 30,
        int timeoutSeconds = 30)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL is required", nameof(url));
        if (string.IsNullOrWhiteSpace(secret))
            throw new ArgumentException("Secret is required", nameof(secret));

        return new WebhookSubscription
        {
            Name = name,
            Url = url,
            Secret = secret,
            EventFilters = eventFilters ?? new List<string>(),
            IsActive = true,
            MaxRetries = Math.Max(0, maxRetries),
            RetryDelaySeconds = Math.Max(1, retryDelaySeconds),
            TimeoutSeconds = Math.Max(5, timeoutSeconds),
            Description = description,
            CreatedByUserId = createdByUserId,
            CreatedByName = createdByName,
            TotalDeliveries = 0,
            FailedDeliveries = 0
        };
    }

    public void Update(
        string name,
        string url,
        List<string>? eventFilters,
        string? description,
        int maxRetries,
        int retryDelaySeconds,
        int timeoutSeconds)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL is required", nameof(url));

        Name = name;
        Url = url;
        EventFilters = eventFilters ?? EventFilters;
        Description = description;
        MaxRetries = Math.Max(0, maxRetries);
        RetryDelaySeconds = Math.Max(1, retryDelaySeconds);
        TimeoutSeconds = Math.Max(5, timeoutSeconds);
    }

    public void RotateSecret(string newSecret)
    {
        if (string.IsNullOrWhiteSpace(newSecret))
            throw new ArgumentException("Secret is required", nameof(newSecret));
        Secret = newSecret;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public void RecordDelivery(bool success)
    {
        TotalDeliveries++;
        if (!success) FailedDeliveries++;
        LastTriggeredAt = DateTime.UtcNow;
    }

    public bool MatchesEvent(string eventType)
    {
        if (!IsActive) return false;
        if (EventFilters.Count == 0) return true; // No filters = all events
        return EventFilters.Any(f =>
            f == "*" ||
            f.Equals(eventType, StringComparison.OrdinalIgnoreCase) ||
            (f.EndsWith(".*") && eventType.StartsWith(f.Replace(".*", "."), StringComparison.OrdinalIgnoreCase)));
    }
}

/// <summary>
/// Log entry for a webhook delivery attempt.
/// </summary>
public class WebhookDeliveryLog : Entity
{
    public Guid WebhookSubscriptionId { get; private set; }
    public string EventType { get; private set; } = string.Empty;
    public string PayloadJson { get; private set; } = string.Empty;
    public int HttpStatusCode { get; private set; }
    public string? ResponseBody { get; private set; }
    public int AttemptNumber { get; private set; }
    public bool Success { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int DurationMs { get; private set; }
    public DateTime DeliveredAt { get; private set; }

    // Navigation
    public virtual WebhookSubscription Subscription { get; private set; } = null!;

    private WebhookDeliveryLog() { }

    public static WebhookDeliveryLog Create(
        Guid subscriptionId,
        string eventType,
        string payloadJson,
        int httpStatusCode,
        string? responseBody,
        int attemptNumber,
        bool success,
        string? errorMessage,
        int durationMs)
    {
        return new WebhookDeliveryLog
        {
            WebhookSubscriptionId = subscriptionId,
            EventType = eventType,
            PayloadJson = payloadJson,
            HttpStatusCode = httpStatusCode,
            ResponseBody = responseBody?.Length > 2000 ? responseBody.Substring(0, 2000) : responseBody,
            AttemptNumber = attemptNumber,
            Success = success,
            ErrorMessage = errorMessage,
            DurationMs = durationMs,
            DeliveredAt = DateTime.UtcNow
        };
    }
}
