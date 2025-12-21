using AFC27.KMS.Integration.Domain.Entities;

namespace AFC27.KMS.Integration.Application.DTOs;

/// <summary>
/// Integration connector DTO
/// </summary>
public class ConnectorDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public string Provider { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public string? Endpoint { get; set; }
    public string AuthType { get; set; } = string.Empty;

    public bool IsHealthy { get; set; }
    public DateTime? LastHealthCheck { get; set; }
    public DateTime? LastSuccessfulSync { get; set; }

    public List<string> SupportedOperations { get; set; } = new();
    public bool IsActive { get; set; }
    public int Priority { get; set; }
}

/// <summary>
/// Connector detail DTO
/// </summary>
public class ConnectorDetailDto : ConnectorDto
{
    public string? LastHealthMessage { get; set; }
    public int FailureCount { get; set; }
    public List<EntityMappingDto> EntityMappings { get; set; } = new();
    public int? RateLimitPerMinute { get; set; }
    public int? RateLimitPerHour { get; set; }
    public ConnectorStatsDto? Stats { get; set; }
}

/// <summary>
/// Entity mapping DTO
/// </summary>
public class EntityMappingDto
{
    public string SourceEntity { get; set; } = string.Empty;
    public string TargetEntity { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public List<FieldMappingDto> Fields { get; set; } = new();
}

/// <summary>
/// Field mapping DTO
/// </summary>
public class FieldMappingDto
{
    public string SourceField { get; set; } = string.Empty;
    public string TargetField { get; set; } = string.Empty;
    public string MappingType { get; set; } = string.Empty;
    public string? DefaultValue { get; set; }
    public bool IsRequired { get; set; }
}

/// <summary>
/// Connector stats DTO
/// </summary>
public class ConnectorStatsDto
{
    public int TotalSyncs { get; set; }
    public int SuccessfulSyncs { get; set; }
    public int FailedSyncs { get; set; }
    public int TotalRecordsSynced { get; set; }
    public int TotalErrors { get; set; }
    public DateTime? LastSyncTime { get; set; }
    public double AverageSyncDurationMs { get; set; }
}

/// <summary>
/// Create connector request
/// </summary>
public class CreateConnectorRequest
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public IntegrationProvider Provider { get; set; }
    public IntegrationCategory Category { get; set; }

    public string? Endpoint { get; set; }
    public string? ApiVersion { get; set; }
    public AuthenticationType AuthType { get; set; }

    public Dictionary<string, string>? ConnectionSettings { get; set; }
    public Dictionary<string, string>? Secrets { get; set; }

    public int? RateLimitPerMinute { get; set; }
    public int? RateLimitPerHour { get; set; }
}

/// <summary>
/// Update connector request
/// </summary>
public class UpdateConnectorRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Endpoint { get; set; }
    public string? ApiVersion { get; set; }
    public AuthenticationType? AuthType { get; set; }
    public Dictionary<string, string>? ConnectionSettings { get; set; }
    public Dictionary<string, string>? Secrets { get; set; }
    public int? RateLimitPerMinute { get; set; }
    public int? RateLimitPerHour { get; set; }
    public bool? IsActive { get; set; }
    public int? Priority { get; set; }
}

/// <summary>
/// Test connection request
/// </summary>
public class TestConnectionRequest
{
    public string? Endpoint { get; set; }
    public AuthenticationType? AuthType { get; set; }
    public Dictionary<string, string>? ConnectionSettings { get; set; }
    public Dictionary<string, string>? Secrets { get; set; }
}

/// <summary>
/// Test connection result DTO
/// </summary>
public class TestConnectionResultDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int? ResponseTimeMs { get; set; }
    public string? ServerVersion { get; set; }
    public Dictionary<string, object>? Details { get; set; }
}

/// <summary>
/// Sync job DTO
/// </summary>
public class SyncJobDto
{
    public Guid Id { get; set; }
    public Guid ConnectorId { get; set; }
    public string ConnectorName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;

    public string? CronExpression { get; set; }
    public bool IsScheduled { get; set; }
    public DateTime? NextRunTime { get; set; }
    public DateTime? LastRunTime { get; set; }

    public string Status { get; set; } = string.Empty;
    public int TotalRecords { get; set; }
    public int ProcessedRecords { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }

    public bool IsActive { get; set; }
}

/// <summary>
/// Sync job detail DTO
/// </summary>
public class SyncJobDetailDto : SyncJobDto
{
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? DurationMs { get; set; }
    public int SkippedCount { get; set; }
    public string? ErrorMessage { get; set; }
    public List<SyncErrorDto>? Errors { get; set; }
    public int RetryCount { get; set; }
    public Dictionary<string, object>? Parameters { get; set; }
    public int BatchSize { get; set; }
}

/// <summary>
/// Sync error DTO
/// </summary>
public class SyncErrorDto
{
    public string RecordId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public bool IsRetryable { get; set; }
}

/// <summary>
/// Create sync job request
/// </summary>
public class CreateSyncJobRequest
{
    public Guid ConnectorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public SyncJobType Type { get; set; }
    public SyncDirection Direction { get; set; }

    public string? CronExpression { get; set; }
    public bool IsScheduled { get; set; }

    public string? EntityFilter { get; set; }
    public int BatchSize { get; set; } = 100;
    public bool ContinueOnError { get; set; } = true;
    public Dictionary<string, object>? Parameters { get; set; }
}

/// <summary>
/// Run sync job request
/// </summary>
public class RunSyncJobRequest
{
    public bool FullSync { get; set; }
    public DateTime? SyncSince { get; set; }
    public string? EntityFilter { get; set; }
    public Dictionary<string, object>? Parameters { get; set; }
}

/// <summary>
/// Webhook config DTO
/// </summary>
public class WebhookConfigDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ConnectorId { get; set; }
    public string? ConnectorName { get; set; }

    public string TargetUrl { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public List<string> TriggerEvents { get; set; } = new();

    public bool IsActive { get; set; }
    public DateTime? LastTriggeredAt { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
}

/// <summary>
/// Create webhook request
/// </summary>
public class CreateWebhookRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ConnectorId { get; set; }

    public string TargetUrl { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = "POST";
    public Dictionary<string, string>? Headers { get; set; }
    public string? SecretKey { get; set; }

    public List<string> TriggerEvents { get; set; } = new();
    public string? FilterExpression { get; set; }
    public string? PayloadTemplate { get; set; }

    public int RetryCount { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 30;
}

/// <summary>
/// Webhook delivery DTO
/// </summary>
public class WebhookDeliveryDto
{
    public Guid Id { get; set; }
    public Guid WebhookId { get; set; }
    public string EventType { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
    public int? ResponseStatusCode { get; set; }
    public int? DurationMs { get; set; }
    public string? ErrorMessage { get; set; }

    public int AttemptNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// External entity reference DTO
/// </summary>
public class ExternalEntityRefDto
{
    public Guid Id { get; set; }
    public Guid LocalEntityId { get; set; }
    public string LocalEntityType { get; set; } = string.Empty;

    public Guid ConnectorId { get; set; }
    public string ConnectorName { get; set; } = string.Empty;
    public string ExternalId { get; set; } = string.Empty;
    public string ExternalType { get; set; } = string.Empty;
    public string? ExternalUrl { get; set; }

    public DateTime? LastSyncedAt { get; set; }
    public string SyncStatus { get; set; } = string.Empty;
    public string? SyncMessage { get; set; }
}

/// <summary>
/// Integration event log DTO
/// </summary>
public class IntegrationEventLogDto
{
    public Guid Id { get; set; }
    public Guid ConnectorId { get; set; }
    public string ConnectorName { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;

    public Guid? SourceEntityId { get; set; }
    public string? SourceEntityType { get; set; }
    public string? ExternalId { get; set; }

    public string Status { get; set; } = string.Empty;
    public int? DurationMs { get; set; }
    public string? ErrorMessage { get; set; }

    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Integration dashboard DTO
/// </summary>
public class IntegrationDashboardDto
{
    public int TotalConnectors { get; set; }
    public int ActiveConnectors { get; set; }
    public int HealthyConnectors { get; set; }
    public int UnhealthyConnectors { get; set; }

    public int TotalSyncJobs { get; set; }
    public int RunningSyncJobs { get; set; }
    public int FailedSyncJobs { get; set; }

    public int TodayEvents { get; set; }
    public int TodayErrors { get; set; }

    public List<ConnectorStatusDto> ConnectorStatuses { get; set; } = new();
    public List<RecentSyncJobDto> RecentSyncJobs { get; set; } = new();
}

/// <summary>
/// Connector status DTO
/// </summary>
public class ConnectorStatusDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public bool IsHealthy { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? LastSync { get; set; }
}

/// <summary>
/// Recent sync job DTO
/// </summary>
public class RecentSyncJobDto
{
    public Guid Id { get; set; }
    public string ConnectorName { get; set; } = string.Empty;
    public string JobName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int RecordCount { get; set; }
    public int ErrorCount { get; set; }
    public DateTime CompletedAt { get; set; }
}
