using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Integration.Domain.Entities;

/// <summary>
/// Integration connector configuration
/// </summary>
public class IntegrationConnector : AuditableEntity
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public IntegrationProvider Provider { get; set; }
    public IntegrationCategory Category { get; set; }
    public ConnectorStatus Status { get; set; }

    // Connection settings
    public string? Endpoint { get; set; }
    public string? ApiVersion { get; set; }
    public AuthenticationType AuthType { get; set; }
    public Dictionary<string, string> ConnectionSettings { get; set; } = new();
    public Dictionary<string, string> EncryptedSecrets { get; set; } = new();

    // Health & monitoring
    public bool IsHealthy { get; set; }
    public DateTime? LastHealthCheck { get; set; }
    public string? LastHealthMessage { get; set; }
    public DateTime? LastSuccessfulSync { get; set; }
    public int FailureCount { get; set; }

    // Capabilities
    public List<string> SupportedOperations { get; set; } = new();
    public List<EntityMapping> EntityMappings { get; set; } = new();

    // Rate limiting
    public int? RateLimitPerMinute { get; set; }
    public int? RateLimitPerHour { get; set; }
    public int CurrentMinuteRequests { get; set; }
    public int CurrentHourRequests { get; set; }

    public bool IsActive { get; set; }
    public int Priority { get; set; }
}

/// <summary>
/// Integration providers
/// </summary>
public enum IntegrationProvider
{
    // Intalio Suite
    IntalioCase,
    IntalioIAM,
    IntalioDMS,
    IntalioCorrespondence,
    IntalioPortal,
    IntalioForms,
    IntalioAnalytics,

    // Microsoft 365
    SharePoint,
    Exchange,
    MicrosoftTeams,
    OneDrive,
    AzureAD,
    MicrosoftGraph,

    // ERP Systems
    SAP,
    Oracle,
    Dynamics365,

    // Other
    LDAP,
    SMTP,
    SMS,
    Webhook,
    Custom
}

/// <summary>
/// Integration category
/// </summary>
public enum IntegrationCategory
{
    Identity,
    Workflow,
    DocumentManagement,
    Correspondence,
    Communication,
    Calendar,
    Storage,
    ERP,
    Analytics,
    Custom
}

/// <summary>
/// Connector status
/// </summary>
public enum ConnectorStatus
{
    Active,
    Inactive,
    Error,
    Maintenance,
    Configuring
}

/// <summary>
/// Authentication type
/// </summary>
public enum AuthenticationType
{
    None,
    ApiKey,
    OAuth2ClientCredentials,
    OAuth2AuthorizationCode,
    BasicAuth,
    Certificate,
    SAML,
    JWT,
    Custom
}

/// <summary>
/// Entity mapping between systems
/// </summary>
public class EntityMapping
{
    public string SourceEntity { get; set; } = string.Empty;
    public string TargetEntity { get; set; } = string.Empty;
    public SyncDirection Direction { get; set; }
    public List<FieldMapping> Fields { get; set; } = new();
    public string? FilterExpression { get; set; }
    public string? TransformScript { get; set; }
}

/// <summary>
/// Sync direction
/// </summary>
public enum SyncDirection
{
    Inbound,
    Outbound,
    Bidirectional
}

/// <summary>
/// Field mapping
/// </summary>
public class FieldMapping
{
    public string SourceField { get; set; } = string.Empty;
    public string TargetField { get; set; } = string.Empty;
    public FieldMappingType MappingType { get; set; }
    public string? DefaultValue { get; set; }
    public string? TransformExpression { get; set; }
    public bool IsRequired { get; set; }
}

/// <summary>
/// Field mapping type
/// </summary>
public enum FieldMappingType
{
    Direct,
    Constant,
    Lookup,
    Transform,
    Concatenate,
    Split,
    DateFormat,
    Custom
}

/// <summary>
/// Sync job
/// </summary>
public class SyncJob : AuditableEntity
{
    public Guid ConnectorId { get; set; }
    public IntegrationConnector Connector { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public SyncJobType Type { get; set; }
    public SyncDirection Direction { get; set; }

    // Scheduling
    public string? CronExpression { get; set; }
    public bool IsScheduled { get; set; }
    public DateTime? NextRunTime { get; set; }
    public DateTime? LastRunTime { get; set; }

    // Execution
    public SyncJobStatus Status { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? DurationMs { get; set; }

    // Results
    public int TotalRecords { get; set; }
    public int ProcessedRecords { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public int SkippedCount { get; set; }

    // Error handling
    public string? ErrorMessage { get; set; }
    public List<SyncError>? Errors { get; set; }
    public int RetryCount { get; set; }

    // Configuration
    public Dictionary<string, object> Parameters { get; set; } = new();
    public string? EntityFilter { get; set; }
    public int BatchSize { get; set; } = 100;
    public bool ContinueOnError { get; set; } = true;

    public bool IsActive { get; set; }
}

/// <summary>
/// Sync job type
/// </summary>
public enum SyncJobType
{
    FullSync,
    IncrementalSync,
    DeltaSync,
    RealTimeSync,
    OnDemand
}

/// <summary>
/// Sync job status
/// </summary>
public enum SyncJobStatus
{
    Pending,
    Running,
    Completed,
    CompletedWithErrors,
    Failed,
    Cancelled,
    Paused
}

/// <summary>
/// Sync error
/// </summary>
public class SyncError
{
    public string RecordId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public bool IsRetryable { get; set; }
}

/// <summary>
/// Webhook configuration
/// </summary>
public class WebhookConfig : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ConnectorId { get; set; }

    public string TargetUrl { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = "POST";
    public Dictionary<string, string> Headers { get; set; } = new();
    public string? SecretKey { get; set; }

    public List<string> TriggerEvents { get; set; } = new();
    public string? FilterExpression { get; set; }
    public string? PayloadTemplate { get; set; }

    public bool IsActive { get; set; }
    public int RetryCount { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 30;

    public DateTime? LastTriggeredAt { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
}

/// <summary>
/// Webhook delivery log
/// </summary>
public class WebhookDelivery : AuditableEntity
{
    public Guid WebhookId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public Guid? SourceEntityId { get; set; }
    public string? SourceEntityType { get; set; }

    public string RequestUrl { get; set; } = string.Empty;
    public string? RequestBody { get; set; }
    public Dictionary<string, string>? RequestHeaders { get; set; }

    public WebhookDeliveryStatus Status { get; set; }
    public int? ResponseStatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public int? DurationMs { get; set; }

    public string? ErrorMessage { get; set; }
    public int AttemptNumber { get; set; }
    public DateTime? NextRetryAt { get; set; }
}

/// <summary>
/// Webhook delivery status
/// </summary>
public enum WebhookDeliveryStatus
{
    Pending,
    Delivered,
    Failed,
    Retrying
}

/// <summary>
/// External entity reference
/// </summary>
public class ExternalEntityRef : AuditableEntity
{
    public Guid LocalEntityId { get; set; }
    public string LocalEntityType { get; set; } = string.Empty;

    public Guid ConnectorId { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public string ExternalType { get; set; } = string.Empty;
    public string? ExternalUrl { get; set; }

    public DateTime? LastSyncedAt { get; set; }
    public string? SyncHash { get; set; }
    public SyncStatus SyncStatus { get; set; }
    public string? SyncMessage { get; set; }

    public Dictionary<string, object>? ExternalMetadata { get; set; }
}

/// <summary>
/// Sync status
/// </summary>
public enum SyncStatus
{
    Synced,
    Pending,
    Conflict,
    Error,
    LocalOnly,
    ExternalOnly
}

/// <summary>
/// Integration event log
/// </summary>
public class IntegrationEventLog : AuditableEntity
{
    public Guid ConnectorId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public EventDirection Direction { get; set; }

    public Guid? SourceEntityId { get; set; }
    public string? SourceEntityType { get; set; }
    public string? ExternalId { get; set; }

    public IntegrationEventStatus Status { get; set; }
    public string? RequestData { get; set; }
    public string? ResponseData { get; set; }
    public int? DurationMs { get; set; }

    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public string? StackTrace { get; set; }

    public Guid? UserId { get; set; }
    public string? CorrelationId { get; set; }
}

/// <summary>
/// Event direction
/// </summary>
public enum EventDirection
{
    Inbound,
    Outbound
}

/// <summary>
/// Integration event status
/// </summary>
public enum IntegrationEventStatus
{
    Success,
    Failed,
    PartialSuccess,
    Skipped
}
