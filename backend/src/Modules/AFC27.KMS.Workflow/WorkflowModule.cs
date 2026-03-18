using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AFC27.KMS.Workflow.Application.Services;

namespace AFC27.KMS.Workflow;

/// <summary>
/// Workflow module registration and configuration
/// </summary>
public static class WorkflowModule
{
    /// <summary>
    /// Add Workflow module services to DI container
    /// </summary>
    public static IServiceCollection AddWorkflowModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure Workflow options
        services.Configure<WorkflowOptions>(
            configuration.GetSection("Workflow"));

        // Configure SLA options
        services.Configure<SlaOptions>(
            configuration.GetSection("Workflow:Sla"));

        // Register Workflow Engine
        // The DbContext base class must resolve to the application's concrete DbContext.
        // This is done by also registering the concrete context as DbContext in the host.
        services.AddScoped<IWorkflowEngineService, WorkflowEngineService>();

        // TODO: Register services
        // services.AddScoped<IServiceCatalogService, ServiceCatalogService>();
        // services.AddScoped<IServiceRequestService, ServiceRequestService>();
        // services.AddScoped<ITaskService, TaskService>();
        // services.AddScoped<IFormService, FormService>();

        // TODO: Register repositories
        // services.AddScoped<IServiceCategoryRepository, ServiceCategoryRepository>();
        // services.AddScoped<IServiceRepository, ServiceRepository>();
        // services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
        // services.AddScoped<IWorkflowDefinitionRepository, WorkflowDefinitionRepository>();
        // services.AddScoped<ITaskRepository, TaskRepository>();
        // services.AddScoped<IFormRepository, FormRepository>();

        // Configure authorization policies
        services.AddAuthorizationBuilder()
            .AddPolicy("CanManageWorkflow", policy =>
                policy.RequireClaim("permission", "workflow:manage"))
            .AddPolicy("CanAssignRequests", policy =>
                policy.RequireClaim("permission", "workflow:assign"))
            .AddPolicy("CanViewAllTasks", policy =>
                policy.RequireClaim("permission", "workflow:view-all-tasks"))
            .AddPolicy("CanViewAllRequests", policy =>
                policy.RequireClaim("permission", "workflow:view-all-requests"));

        return services;
    }
}

/// <summary>
/// Workflow module configuration options
/// </summary>
public class WorkflowOptions
{
    /// <summary>
    /// Enable workflow module
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Auto-assign requests based on workflow configuration
    /// </summary>
    public bool AutoAssignEnabled { get; set; } = true;

    /// <summary>
    /// Send notifications for task assignments
    /// </summary>
    public bool SendTaskNotifications { get; set; } = true;

    /// <summary>
    /// Send reminders for overdue tasks
    /// </summary>
    public bool SendOverdueReminders { get; set; } = true;

    /// <summary>
    /// Hours before due date to send reminder
    /// </summary>
    public int ReminderHoursBeforeDue { get; set; } = 24;

    /// <summary>
    /// Maximum attachments per request
    /// </summary>
    public int MaxAttachmentsPerRequest { get; set; } = 10;

    /// <summary>
    /// Maximum file size for attachments (bytes)
    /// </summary>
    public long MaxAttachmentSize { get; set; } = 10 * 1024 * 1024; // 10 MB

    /// <summary>
    /// Allowed attachment file types
    /// </summary>
    public List<string> AllowedAttachmentTypes { get; set; } = new()
    {
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
        ".jpg", ".jpeg", ".png", ".gif", ".txt", ".zip"
    };

    /// <summary>
    /// Enable draft saving for requests
    /// </summary>
    public bool EnableDraftSaving { get; set; } = true;

    /// <summary>
    /// Days to keep draft requests before auto-deletion
    /// </summary>
    public int DraftRetentionDays { get; set; } = 30;

    /// <summary>
    /// Enable request cancellation by requester
    /// </summary>
    public bool AllowRequesterCancellation { get; set; } = true;

    /// <summary>
    /// Enable task delegation
    /// </summary>
    public bool AllowTaskDelegation { get; set; } = true;

    /// <summary>
    /// Request number prefix
    /// </summary>
    public string RequestNumberPrefix { get; set; } = "SR";

    /// <summary>
    /// Task number prefix
    /// </summary>
    public string TaskNumberPrefix { get; set; } = "T";
}

/// <summary>
/// SLA configuration options
/// </summary>
public class SlaOptions
{
    /// <summary>
    /// Enable SLA tracking
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Default response SLA (hours)
    /// </summary>
    public int DefaultResponseHours { get; set; } = 24;

    /// <summary>
    /// Default resolution SLA (hours)
    /// </summary>
    public int DefaultResolutionHours { get; set; } = 72;

    /// <summary>
    /// SLA by priority (hours)
    /// </summary>
    public Dictionary<string, SlaPriorityConfig> ByPriority { get; set; } = new()
    {
        ["Critical"] = new SlaPriorityConfig { ResponseHours = 1, ResolutionHours = 4 },
        ["Urgent"] = new SlaPriorityConfig { ResponseHours = 4, ResolutionHours = 24 },
        ["High"] = new SlaPriorityConfig { ResponseHours = 8, ResolutionHours = 48 },
        ["Normal"] = new SlaPriorityConfig { ResponseHours = 24, ResolutionHours = 72 },
        ["Low"] = new SlaPriorityConfig { ResponseHours = 48, ResolutionHours = 120 }
    };

    /// <summary>
    /// Calculate SLA based on business hours only
    /// </summary>
    public bool UseBusinessHours { get; set; } = true;

    /// <summary>
    /// Business hours start
    /// </summary>
    public TimeSpan BusinessHoursStart { get; set; } = new TimeSpan(8, 0, 0);

    /// <summary>
    /// Business hours end
    /// </summary>
    public TimeSpan BusinessHoursEnd { get; set; } = new TimeSpan(17, 0, 0);

    /// <summary>
    /// Business days (0 = Sunday, 6 = Saturday)
    /// </summary>
    public List<int> BusinessDays { get; set; } = new() { 0, 1, 2, 3, 4 }; // Sun-Thu for Saudi Arabia

    /// <summary>
    /// Holidays to exclude from SLA calculation
    /// </summary>
    public List<DateTime> Holidays { get; set; } = new();

    /// <summary>
    /// Send notification when SLA is at risk
    /// </summary>
    public bool SendSlaWarningNotification { get; set; } = true;

    /// <summary>
    /// Percentage of SLA time remaining to trigger warning
    /// </summary>
    public int SlaWarningThresholdPercent { get; set; } = 25;
}

/// <summary>
/// SLA configuration for a priority level
/// </summary>
public class SlaPriorityConfig
{
    public int ResponseHours { get; set; }
    public int ResolutionHours { get; set; }
}

/// <summary>
/// Form field validation rules
/// </summary>
public static class FormValidation
{
    public const int MaxFieldNameLength = 100;
    public const int MaxLabelLength = 500;
    public const int MaxHelpTextLength = 1000;
    public const int MaxOptionsPerField = 100;
    public const int MaxFieldsPerForm = 200;
    public const int MaxSectionsPerForm = 20;
}
