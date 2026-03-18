namespace AFC27.KMS.Admin.Application.DTOs;

/// <summary>
/// Webhook subscription DTO.
/// </summary>
public record WebhookSubscriptionDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public IReadOnlyList<string> EventFilters { get; init; } = Array.Empty<string>();
    public bool IsActive { get; init; }
    public int MaxRetries { get; init; }
    public int RetryDelaySeconds { get; init; }
    public int TimeoutSeconds { get; init; }
    public string? Description { get; init; }
    public Guid CreatedByUserId { get; init; }
    public string CreatedByName { get; init; } = string.Empty;
    public DateTime? LastTriggeredAt { get; init; }
    public int TotalDeliveries { get; init; }
    public int FailedDeliveries { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Create webhook subscription request.
/// </summary>
public record CreateWebhookRequest
{
    public string Name { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string Secret { get; init; } = string.Empty;
    public IReadOnlyList<string>? EventFilters { get; init; }
    public string? Description { get; init; }
    public int MaxRetries { get; init; } = 3;
    public int RetryDelaySeconds { get; init; } = 30;
    public int TimeoutSeconds { get; init; } = 30;
}

/// <summary>
/// Update webhook subscription request.
/// </summary>
public record UpdateWebhookRequest
{
    public string Name { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public IReadOnlyList<string>? EventFilters { get; init; }
    public string? Description { get; init; }
    public int MaxRetries { get; init; } = 3;
    public int RetryDelaySeconds { get; init; } = 30;
    public int TimeoutSeconds { get; init; } = 30;
}

/// <summary>
/// Webhook delivery log DTO.
/// </summary>
public record WebhookDeliveryLogDto
{
    public Guid Id { get; init; }
    public Guid WebhookSubscriptionId { get; init; }
    public string EventType { get; init; } = string.Empty;
    public int HttpStatusCode { get; init; }
    public int AttemptNumber { get; init; }
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public int DurationMs { get; init; }
    public DateTime DeliveredAt { get; init; }
}

/// <summary>
/// Request to test a webhook.
/// </summary>
public record TestWebhookRequest
{
    public string? EventType { get; init; } = "test.ping";
}
