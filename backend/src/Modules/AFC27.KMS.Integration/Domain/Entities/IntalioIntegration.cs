using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Integration.Domain.Entities;

/// <summary>
/// Intalio Case (BPM/Workflow) integration
/// </summary>
public class IntalioCaseConfig : AuditableEntity
{
    public Guid ConnectorId { get; set; }

    // Process definitions synced from Intalio Case
    public List<IntalioProcessDefinition> ProcessDefinitions { get; set; } = new();

    // Form mappings
    public List<IntalioFormMapping> FormMappings { get; set; } = new();

    // Task routing configuration
    public bool SyncTasks { get; set; } = true;
    public bool CreateTasksInKMS { get; set; } = true;
    public bool SendTaskUpdatesToCase { get; set; } = true;

    // Notification settings
    public bool NotifyOnTaskAssignment { get; set; } = true;
    public bool NotifyOnTaskCompletion { get; set; } = true;
    public bool NotifyOnProcessCompletion { get; set; } = true;
}

/// <summary>
/// Intalio process definition
/// </summary>
public class IntalioProcessDefinition
{
    public string ProcessId { get; set; } = string.Empty;
    public string ProcessKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public int Version { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastSyncedAt { get; set; }

    // KMS mapping
    public Guid? MappedServiceId { get; set; }
    public bool AutoCreateService { get; set; }
}

/// <summary>
/// Intalio form mapping
/// </summary>
public class IntalioFormMapping
{
    public string IntalioFormKey { get; set; } = string.Empty;
    public Guid? KMSFormId { get; set; }
    public List<FieldMapping> FieldMappings { get; set; } = new();
    public bool AutoSync { get; set; }
}

/// <summary>
/// Intalio IAM integration
/// </summary>
public class IntalioIAMConfig : AuditableEntity
{
    public Guid ConnectorId { get; set; }

    // User sync settings
    public bool SyncUsers { get; set; } = true;
    public bool SyncGroups { get; set; } = true;
    public bool SyncRoles { get; set; } = true;
    public bool SyncDepartments { get; set; } = true;

    // Direction
    public SyncDirection UserSyncDirection { get; set; } = SyncDirection.Inbound;
    public SyncDirection GroupSyncDirection { get; set; } = SyncDirection.Inbound;

    // SSO settings
    public bool EnableSSO { get; set; } = true;
    public string? SSOEndpoint { get; set; }
    public string? LogoutEndpoint { get; set; }

    // Attribute mappings
    public List<FieldMapping> UserAttributeMappings { get; set; } = new();
    public List<FieldMapping> GroupAttributeMappings { get; set; } = new();

    // Filters
    public string? UserFilter { get; set; }
    public string? GroupFilter { get; set; }
    public List<string>? IncludeGroups { get; set; }
    public List<string>? ExcludeGroups { get; set; }
}

/// <summary>
/// Intalio DMS integration
/// </summary>
public class IntalioDMSConfig : AuditableEntity
{
    public Guid ConnectorId { get; set; }

    // Document sync settings
    public bool SyncDocuments { get; set; } = true;
    public bool SyncFolders { get; set; } = true;
    public bool SyncMetadata { get; set; } = true;
    public bool SyncVersions { get; set; } = true;
    public bool SyncPermissions { get; set; } = true;

    // Direction
    public SyncDirection DocumentSyncDirection { get; set; } = SyncDirection.Bidirectional;

    // Library mappings
    public List<LibraryMapping> LibraryMappings { get; set; } = new();

    // Content type mappings
    public List<ContentTypeMapping> ContentTypeMappings { get; set; } = new();

    // Metadata mappings
    public List<FieldMapping> MetadataMappings { get; set; } = new();

    // Storage settings
    public string? DefaultLibraryId { get; set; }
    public bool UseExternalStorage { get; set; }
    public string? StorageBasePath { get; set; }
}

/// <summary>
/// Library mapping
/// </summary>
public class LibraryMapping
{
    public Guid KMSLibraryId { get; set; }
    public string IntalioDMSLibraryId { get; set; } = string.Empty;
    public string? IntalioDMSLibraryName { get; set; }
    public bool Enabled { get; set; } = true;
    public SyncDirection Direction { get; set; }
}

/// <summary>
/// Content type mapping
/// </summary>
public class ContentTypeMapping
{
    public string KMSContentType { get; set; } = string.Empty;
    public string IntalioDMSContentType { get; set; } = string.Empty;
    public List<FieldMapping> FieldMappings { get; set; } = new();
}

/// <summary>
/// Intalio Correspondence integration
/// </summary>
public class IntalioCorrespondenceConfig : AuditableEntity
{
    public Guid ConnectorId { get; set; }

    // Sync settings
    public bool SyncIncomingCorrespondence { get; set; } = true;
    public bool SyncOutgoingCorrespondence { get; set; } = true;
    public bool SyncInternalMemos { get; set; } = true;
    public bool SyncAttachments { get; set; } = true;

    // Category mappings
    public List<CorrespondenceCategoryMapping> CategoryMappings { get; set; } = new();

    // Workflow integration
    public bool TriggerWorkflowOnReceipt { get; set; } = true;
    public string? DefaultWorkflowProcessKey { get; set; }

    // Notification settings
    public bool NotifyOnNewCorrespondence { get; set; } = true;
    public bool NotifyOnStatusChange { get; set; } = true;

    // Archive settings
    public bool ArchiveToKMS { get; set; } = true;
    public Guid? ArchiveLibraryId { get; set; }
}

/// <summary>
/// Correspondence category mapping
/// </summary>
public class CorrespondenceCategoryMapping
{
    public string IntalioCategory { get; set; } = string.Empty;
    public string KMSCategory { get; set; } = string.Empty;
    public Guid? DefaultAssigneeId { get; set; }
    public int? SLADays { get; set; }
}

/// <summary>
/// Intalio task reference
/// </summary>
public class IntalioTaskRef : AuditableEntity
{
    public Guid KMSTaskId { get; set; }
    public string IntalioTaskId { get; set; } = string.Empty;
    public string IntalioProcessInstanceId { get; set; } = string.Empty;
    public string? IntalioProcessKey { get; set; }

    public string TaskName { get; set; } = string.Empty;
    public string? TaskNameAr { get; set; }
    public string? FormKey { get; set; }

    public IntalioTaskStatus IntalioStatus { get; set; }
    public DateTime? ClaimedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Outcome { get; set; }

    public Dictionary<string, object>? ProcessVariables { get; set; }
    public DateTime LastSyncedAt { get; set; }
}

/// <summary>
/// Intalio task status
/// </summary>
public enum IntalioTaskStatus
{
    Created,
    Assigned,
    Claimed,
    Completed,
    Cancelled,
    Delegated,
    Escalated
}

/// <summary>
/// Microsoft 365 integration
/// </summary>
public class Microsoft365Config : AuditableEntity
{
    public Guid ConnectorId { get; set; }

    // Tenant settings
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;

    // SharePoint settings
    public bool EnableSharePoint { get; set; }
    public string? SharePointSiteUrl { get; set; }
    public List<SharePointLibraryMapping> SharePointLibraries { get; set; } = new();

    // Exchange/Calendar settings
    public bool EnableExchange { get; set; }
    public bool SyncCalendars { get; set; }
    public bool SyncContacts { get; set; }
    public string? SharedMailbox { get; set; }

    // Teams settings
    public bool EnableTeams { get; set; }
    public bool CreateTeamsChannels { get; set; }
    public bool PostToTeams { get; set; }

    // OneDrive settings
    public bool EnableOneDrive { get; set; }
    public bool SyncUserFiles { get; set; }

    // Azure AD settings
    public bool SyncAzureADUsers { get; set; }
    public bool SyncAzureADGroups { get; set; }
}

/// <summary>
/// SharePoint library mapping
/// </summary>
public class SharePointLibraryMapping
{
    public Guid KMSLibraryId { get; set; }
    public string SharePointSiteId { get; set; } = string.Empty;
    public string SharePointDriveId { get; set; } = string.Empty;
    public string? SharePointFolderId { get; set; }
    public string LibraryName { get; set; } = string.Empty;
    public SyncDirection Direction { get; set; }
    public bool SyncSubfolders { get; set; } = true;
    public bool Enabled { get; set; } = true;
}

/// <summary>
/// ERP integration configuration
/// </summary>
public class ERPConfig : AuditableEntity
{
    public Guid ConnectorId { get; set; }
    public ERPSystem ERPSystem { get; set; }

    // Connection
    public string? SystemId { get; set; }
    public string? Client { get; set; }
    public string? Language { get; set; }

    // Data sync
    public bool SyncEmployees { get; set; }
    public bool SyncOrganization { get; set; }
    public bool SyncCostCenters { get; set; }
    public bool SyncProjects { get; set; }
    public bool SyncVendors { get; set; }

    // API settings
    public List<ERPAPIMapping> APIMappings { get; set; } = new();

    // Workflow integration
    public bool TriggerERPWorkflows { get; set; }
    public List<ERPWorkflowMapping> WorkflowMappings { get; set; } = new();
}

/// <summary>
/// ERP system type
/// </summary>
public enum ERPSystem
{
    SAP_S4HANA,
    SAP_ECC,
    Oracle_EBS,
    Oracle_Cloud,
    Microsoft_Dynamics365
}

/// <summary>
/// ERP API mapping
/// </summary>
public class ERPAPIMapping
{
    public string EntityType { get; set; } = string.Empty;
    public string APIEndpoint { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = "GET";
    public string? RequestTemplate { get; set; }
    public string? ResponseMapping { get; set; }
}

/// <summary>
/// ERP workflow mapping
/// </summary>
public class ERPWorkflowMapping
{
    public string KMSServiceKey { get; set; } = string.Empty;
    public string ERPWorkflowType { get; set; } = string.Empty;
    public Dictionary<string, string> ParameterMappings { get; set; } = new();
}
