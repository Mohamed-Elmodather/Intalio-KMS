using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AFC27.KMS.Integration;

/// <summary>
/// Integration Module registration
/// </summary>
public static class IntegrationModule
{
    /// <summary>
    /// Add Integration Module services
    /// </summary>
    public static IServiceCollection AddIntegrationModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register options
        services.Configure<IntegrationOptions>(configuration.GetSection("Integration"));
        services.Configure<IntalioCaseOptions>(configuration.GetSection("Integration:IntalioCase"));
        services.Configure<IntalioIAMOptions>(configuration.GetSection("Integration:IntalioIAM"));
        services.Configure<IntalioDMSOptions>(configuration.GetSection("Integration:IntalioDMS"));
        services.Configure<IntalioCorrespondenceOptions>(configuration.GetSection("Integration:IntalioCorrespondence"));
        services.Configure<Microsoft365Options>(configuration.GetSection("Integration:Microsoft365"));
        services.Configure<ERPOptions>(configuration.GetSection("Integration:ERP"));

        // TODO: Register repositories
        // services.AddScoped<IConnectorRepository, ConnectorRepository>();
        // services.AddScoped<ISyncJobRepository, SyncJobRepository>();
        // services.AddScoped<IWebhookRepository, WebhookRepository>();
        // services.AddScoped<IEntityRefRepository, EntityRefRepository>();

        // TODO: Register services
        // services.AddScoped<IConnectorService, ConnectorService>();
        // services.AddScoped<ISyncService, SyncService>();
        // services.AddScoped<IWebhookService, WebhookService>();

        // TODO: Register Intalio services
        // services.AddScoped<IIntalioCaseService, IntalioCaseService>();
        // services.AddScoped<IIntalioIAMService, IntalioIAMService>();
        // services.AddScoped<IIntalioDMSService, IntalioDMSService>();
        // services.AddScoped<IIntalioCorrespondenceService, IntalioCorrespondenceService>();

        // TODO: Register M365 services
        // services.AddScoped<ISharePointService, SharePointService>();
        // services.AddScoped<IExchangeService, ExchangeService>();
        // services.AddScoped<ITeamsService, TeamsService>();
        // services.AddScoped<IAzureADService, AzureADService>();

        // TODO: Register ERP services
        // services.AddScoped<IERPService, ERPService>();
        // services.AddScoped<ISAPService, SAPService>();
        // services.AddScoped<IOracleService, OracleService>();

        // TODO: Register HTTP clients for external systems
        // services.AddHttpClient<IntalioCaseClient>();
        // services.AddHttpClient<IntalioIAMClient>();
        // services.AddHttpClient<IntalioDMSClient>();
        // services.AddHttpClient<SharePointClient>();

        // TODO: Register background services
        // services.AddHostedService<SyncJobScheduler>();
        // services.AddHostedService<WebhookDeliveryWorker>();
        // services.AddHostedService<HealthCheckWorker>();

        return services;
    }
}

#region Configuration Options

/// <summary>
/// Main integration options
/// </summary>
public class IntegrationOptions
{
    /// <summary>
    /// Enable integration module
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Default sync batch size
    /// </summary>
    public int DefaultBatchSize { get; set; } = 100;

    /// <summary>
    /// Default sync retry count
    /// </summary>
    public int DefaultRetryCount { get; set; } = 3;

    /// <summary>
    /// Webhook signature secret
    /// </summary>
    public string WebhookSecret { get; set; } = string.Empty;

    /// <summary>
    /// Webhook timeout in seconds
    /// </summary>
    public int WebhookTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Enable event logging
    /// </summary>
    public bool EnableEventLogging { get; set; } = true;

    /// <summary>
    /// Event log retention days
    /// </summary>
    public int EventLogRetentionDays { get; set; } = 90;
}

/// <summary>
/// Intalio Case (BPM/Workflow) options
/// </summary>
public class IntalioCaseOptions
{
    /// <summary>
    /// Enable Intalio Case integration
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Intalio Case API endpoint
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// API version
    /// </summary>
    public string ApiVersion { get; set; } = "v1";

    /// <summary>
    /// Client ID for OAuth2
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Client secret for OAuth2
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Token endpoint
    /// </summary>
    public string TokenEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Scope for OAuth2
    /// </summary>
    public string Scope { get; set; } = "case.read case.write";

    /// <summary>
    /// Sync tasks from Case to KMS
    /// </summary>
    public bool SyncTasks { get; set; } = true;

    /// <summary>
    /// Create corresponding tasks in KMS
    /// </summary>
    public bool CreateTasksInKMS { get; set; } = true;

    /// <summary>
    /// Send task updates back to Case
    /// </summary>
    public bool SendTaskUpdates { get; set; } = true;

    /// <summary>
    /// Sync process definitions
    /// </summary>
    public bool SyncProcessDefinitions { get; set; } = true;

    /// <summary>
    /// Sync forms
    /// </summary>
    public bool SyncForms { get; set; } = true;

    /// <summary>
    /// Task sync interval in minutes
    /// </summary>
    public int TaskSyncIntervalMinutes { get; set; } = 15;

    /// <summary>
    /// Process sync interval in hours
    /// </summary>
    public int ProcessSyncIntervalHours { get; set; } = 6;

    /// <summary>
    /// Webhook URL for receiving Case events
    /// </summary>
    public string? WebhookUrl { get; set; }

    /// <summary>
    /// Webhook secret for signature validation
    /// </summary>
    public string? WebhookSecret { get; set; }
}

/// <summary>
/// Intalio IAM options
/// </summary>
public class IntalioIAMOptions
{
    /// <summary>
    /// Enable Intalio IAM integration
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Intalio IAM API endpoint
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Client ID for OAuth2
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Client secret for OAuth2
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Token endpoint
    /// </summary>
    public string TokenEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Sync users to KMS
    /// </summary>
    public bool SyncUsers { get; set; } = true;

    /// <summary>
    /// Sync groups to KMS
    /// </summary>
    public bool SyncGroups { get; set; } = true;

    /// <summary>
    /// Sync roles to KMS
    /// </summary>
    public bool SyncRoles { get; set; } = true;

    /// <summary>
    /// Enable SSO with Intalio IAM
    /// </summary>
    public bool EnableSSO { get; set; } = true;

    /// <summary>
    /// SSO endpoint
    /// </summary>
    public string? SSOEndpoint { get; set; }

    /// <summary>
    /// Logout endpoint
    /// </summary>
    public string? LogoutEndpoint { get; set; }

    /// <summary>
    /// User sync interval in hours
    /// </summary>
    public int UserSyncIntervalHours { get; set; } = 2;

    /// <summary>
    /// User filter query
    /// </summary>
    public string? UserFilter { get; set; }

    /// <summary>
    /// Groups to include
    /// </summary>
    public List<string>? IncludeGroups { get; set; }

    /// <summary>
    /// Groups to exclude
    /// </summary>
    public List<string>? ExcludeGroups { get; set; }
}

/// <summary>
/// Intalio DMS options
/// </summary>
public class IntalioDMSOptions
{
    /// <summary>
    /// Enable Intalio DMS integration
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Intalio DMS API endpoint
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Client ID for OAuth2
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Client secret for OAuth2
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Token endpoint
    /// </summary>
    public string TokenEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Sync documents bidirectionally
    /// </summary>
    public bool SyncDocuments { get; set; } = true;

    /// <summary>
    /// Sync document metadata
    /// </summary>
    public bool SyncMetadata { get; set; } = true;

    /// <summary>
    /// Sync document versions
    /// </summary>
    public bool SyncVersions { get; set; } = true;

    /// <summary>
    /// Sync document permissions
    /// </summary>
    public bool SyncPermissions { get; set; } = true;

    /// <summary>
    /// Default library ID in Intalio DMS
    /// </summary>
    public string? DefaultLibraryId { get; set; }

    /// <summary>
    /// Use Intalio DMS as external storage
    /// </summary>
    public bool UseAsExternalStorage { get; set; }

    /// <summary>
    /// Document sync interval in minutes
    /// </summary>
    public int SyncIntervalMinutes { get; set; } = 30;

    /// <summary>
    /// Maximum file size for sync in MB
    /// </summary>
    public int MaxFileSizeMB { get; set; } = 100;
}

/// <summary>
/// Intalio Correspondence options
/// </summary>
public class IntalioCorrespondenceOptions
{
    /// <summary>
    /// Enable Intalio Correspondence integration
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Intalio Correspondence API endpoint
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Client ID for OAuth2
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Client secret for OAuth2
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Token endpoint
    /// </summary>
    public string TokenEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Sync incoming correspondence
    /// </summary>
    public bool SyncIncoming { get; set; } = true;

    /// <summary>
    /// Sync outgoing correspondence
    /// </summary>
    public bool SyncOutgoing { get; set; } = true;

    /// <summary>
    /// Sync internal memos
    /// </summary>
    public bool SyncInternal { get; set; } = true;

    /// <summary>
    /// Sync attachments with correspondence
    /// </summary>
    public bool SyncAttachments { get; set; } = true;

    /// <summary>
    /// Trigger workflow on new correspondence
    /// </summary>
    public bool TriggerWorkflow { get; set; } = true;

    /// <summary>
    /// Default workflow process key
    /// </summary>
    public string? DefaultWorkflowProcessKey { get; set; }

    /// <summary>
    /// Sync interval in minutes
    /// </summary>
    public int SyncIntervalMinutes { get; set; } = 15;

    /// <summary>
    /// Archive correspondence to KMS
    /// </summary>
    public bool ArchiveToKMS { get; set; } = true;

    /// <summary>
    /// Archive library ID in KMS
    /// </summary>
    public Guid? ArchiveLibraryId { get; set; }
}

/// <summary>
/// Microsoft 365 options
/// </summary>
public class Microsoft365Options
{
    /// <summary>
    /// Enable Microsoft 365 integration
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Azure AD tenant ID
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Azure AD client ID
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Azure AD client secret
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Microsoft Graph API endpoint
    /// </summary>
    public string GraphEndpoint { get; set; } = "https://graph.microsoft.com/v1.0";

    #region SharePoint

    /// <summary>
    /// Enable SharePoint integration
    /// </summary>
    public bool SharePointEnabled { get; set; } = true;

    /// <summary>
    /// SharePoint site URL
    /// </summary>
    public string? SharePointSiteUrl { get; set; }

    /// <summary>
    /// Sync documents from SharePoint
    /// </summary>
    public bool SyncSharePointDocuments { get; set; } = true;

    /// <summary>
    /// SharePoint sync interval in minutes
    /// </summary>
    public int SharePointSyncIntervalMinutes { get; set; } = 30;

    #endregion

    #region Exchange

    /// <summary>
    /// Enable Exchange integration
    /// </summary>
    public bool ExchangeEnabled { get; set; } = true;

    /// <summary>
    /// Sync calendars from Exchange
    /// </summary>
    public bool SyncCalendars { get; set; } = true;

    /// <summary>
    /// Sync contacts from Exchange
    /// </summary>
    public bool SyncContacts { get; set; }

    /// <summary>
    /// Shared mailbox to monitor
    /// </summary>
    public string? SharedMailbox { get; set; }

    /// <summary>
    /// Calendar sync interval in hours
    /// </summary>
    public int CalendarSyncIntervalHours { get; set; } = 2;

    #endregion

    #region Teams

    /// <summary>
    /// Enable Teams integration
    /// </summary>
    public bool TeamsEnabled { get; set; }

    /// <summary>
    /// Create Teams channels for communities
    /// </summary>
    public bool CreateTeamsChannels { get; set; }

    /// <summary>
    /// Post notifications to Teams
    /// </summary>
    public bool PostToTeams { get; set; }

    #endregion

    #region Azure AD

    /// <summary>
    /// Sync users from Azure AD
    /// </summary>
    public bool SyncAzureADUsers { get; set; } = true;

    /// <summary>
    /// Sync groups from Azure AD
    /// </summary>
    public bool SyncAzureADGroups { get; set; } = true;

    /// <summary>
    /// Azure AD sync interval in hours
    /// </summary>
    public int AzureADSyncIntervalHours { get; set; } = 4;

    #endregion
}

/// <summary>
/// ERP integration options
/// </summary>
public class ERPOptions
{
    /// <summary>
    /// Enable ERP integration
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// ERP system type (SAP_S4HANA, SAP_ECC, Oracle_EBS, Oracle_Cloud, Dynamics365)
    /// </summary>
    public string ERPSystem { get; set; } = "SAP_S4HANA";

    /// <summary>
    /// ERP API endpoint
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Authentication type (Basic, OAuth2, Certificate)
    /// </summary>
    public string AuthType { get; set; } = "OAuth2";

    /// <summary>
    /// Client ID for OAuth2
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Client secret for OAuth2
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Username for Basic auth
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Password for Basic auth
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// SAP system ID
    /// </summary>
    public string? SystemId { get; set; }

    /// <summary>
    /// SAP client number
    /// </summary>
    public string? Client { get; set; }

    /// <summary>
    /// Language
    /// </summary>
    public string Language { get; set; } = "EN";

    #region Data Sync

    /// <summary>
    /// Sync employees from ERP
    /// </summary>
    public bool SyncEmployees { get; set; } = true;

    /// <summary>
    /// Sync organization structure
    /// </summary>
    public bool SyncOrganization { get; set; } = true;

    /// <summary>
    /// Sync cost centers
    /// </summary>
    public bool SyncCostCenters { get; set; } = true;

    /// <summary>
    /// Sync projects
    /// </summary>
    public bool SyncProjects { get; set; }

    /// <summary>
    /// Sync vendors
    /// </summary>
    public bool SyncVendors { get; set; }

    /// <summary>
    /// Employee sync interval in hours
    /// </summary>
    public int EmployeeSyncIntervalHours { get; set; } = 2;

    /// <summary>
    /// Organization sync interval in days
    /// </summary>
    public int OrgSyncIntervalDays { get; set; } = 1;

    #endregion

    #region Workflow Integration

    /// <summary>
    /// Enable ERP workflow integration
    /// </summary>
    public bool EnableWorkflows { get; set; } = true;

    /// <summary>
    /// Workflow types to sync
    /// </summary>
    public List<string> WorkflowTypes { get; set; } = new()
    {
        "PurchaseRequisition",
        "PurchaseOrder",
        "LeaveRequest",
        "TravelRequest"
    };

    #endregion
}

#endregion
