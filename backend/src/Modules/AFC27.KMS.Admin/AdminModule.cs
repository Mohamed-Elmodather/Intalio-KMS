using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AFC27.KMS.Admin.Application.Interfaces;
using AFC27.KMS.Admin.Application.Services;

namespace AFC27.KMS.Admin;

/// <summary>
/// Admin module registration.
/// </summary>
public static class AdminModule
{
    /// <summary>
    /// Add Admin module services.
    /// </summary>
    public static IServiceCollection AddAdminModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Existing services
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<ILegalHoldService, LegalHoldService>();
        services.AddScoped<IQuarantineService, QuarantineService>();
        services.AddScoped<IUserLifecycleService, UserLifecycleService>();
        services.AddScoped<IImpersonationService, ImpersonationService>();

        // Phase 4 services
        services.AddScoped<IDelegatedAdminService, DelegatedAdminService>();
        services.AddScoped<IGuestAccessService, GuestAccessService>();

        // Phase 6 services
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        services.AddScoped<IWebhookService, WebhookService>();
        services.AddScoped<IApiKeyService, ApiKeyService>();

        // Phase 7 services
        services.AddScoped<IAuditTrailService, AuditTrailService>();
        services.AddScoped<IComplianceService, ComplianceService>();
        services.AddScoped<IGovernanceService, GovernanceService>();

        // Phase 8E: Automation rules
        services.AddScoped<IAutomationRuleService, AutomationRuleService>();

        // Add authorization policies
        services.AddAuthorizationBuilder()
            .AddPolicy("CanManageUsers", policy =>
                policy.RequireClaim("permission", "admin:manage-users"))
            .AddPolicy("CanManageDelegation", policy =>
                policy.RequireClaim("permission", "admin:manage-delegation"))
            .AddPolicy("CanManageGuestAccess", policy =>
                policy.RequireClaim("permission", "admin:manage-guest-access"))
            .AddPolicy("CanManageAutomationRules", policy =>
                policy.RequireClaim("permission", "admin:manage-automation-rules"));

        return services;
    }
}
