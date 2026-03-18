using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AFC27.KMS.Content.Application.Interfaces;
using AFC27.KMS.Content.Application.Services;

namespace AFC27.KMS.Content;

/// <summary>
/// Content module registration.
/// </summary>
public static class ContentModule
{
    /// <summary>
    /// Add Content module services.
    /// </summary>
    public static IServiceCollection AddContentModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Add module-specific services
        services.AddScoped<ISpaceService, SpaceService>();
        services.AddScoped<ISpacePermissionService, SpacePermissionService>();
        services.AddScoped<IVerificationService, VerificationService>();
        services.AddScoped<IContentHealthService, ContentHealthService>();
        // services.AddScoped<IArticleService, ArticleService>();
        // services.AddScoped<ICategoryService, CategoryService>();
        // services.AddScoped<ITagService, TagService>();
        // services.AddScoped<ITemplateService, TemplateService>();

        // Phase 3B: Article Export Service
        services.AddScoped<IArticleExportService, ArticleExportService>();

        // Phase 3C: Content Template Service (also covers Phase 3E review cycles)
        services.AddScoped<IContentTemplateService, ContentTemplateService>();

        // Phase 3D: Active Editor Tracking Service
        services.AddScoped<IActiveEditorService, ActiveEditorService>();

        // Phase 9D: Import/Export services
        services.AddScoped<IImportService, ImportService>();
        services.AddScoped<IBulkExportService, BulkExportService>();

        // Background jobs
        services.AddHostedService<KnowledgeVerificationReminderJob>();
        services.AddHostedService<ContentHealthCalculationJob>();

        // Add authorization policies
        services.AddAuthorizationBuilder()
            .AddPolicy("CanCreateContent", policy =>
                policy.RequireClaim("permission", "content:create"))
            .AddPolicy("CanEditContent", policy =>
                policy.RequireClaim("permission", "content:edit"))
            .AddPolicy("CanDeleteContent", policy =>
                policy.RequireClaim("permission", "content:delete"))
            .AddPolicy("CanPublishContent", policy =>
                policy.RequireClaim("permission", "content:publish"))
            .AddPolicy("CanManageContent", policy =>
                policy.RequireClaim("permission", "content:manage"))
            .AddPolicy("CanManageSpacePermissions", policy =>
                policy.RequireClaim("permission", "space:manage-permissions"))
            .AddPolicy("CanAdminSpaces", policy =>
                policy.RequireClaim("permission", "space:admin"));

        return services;
    }
}
