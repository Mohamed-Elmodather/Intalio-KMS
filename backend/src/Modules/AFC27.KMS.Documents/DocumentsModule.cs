using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AFC27.KMS.Documents;

/// <summary>
/// Documents module registration.
/// </summary>
public static class DocumentsModule
{
    /// <summary>
    /// Add Documents module services.
    /// </summary>
    public static IServiceCollection AddDocumentsModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Add module-specific services
        // services.AddScoped<IDocumentService, DocumentService>();
        // services.AddScoped<ILibraryService, LibraryService>();
        // services.AddScoped<IFolderService, FolderService>();
        // services.AddScoped<IStorageService, StorageService>();

        // Add authorization policies
        services.AddAuthorizationBuilder()
            .AddPolicy("CanUploadDocuments", policy =>
                policy.RequireClaim("permission", "documents:upload"))
            .AddPolicy("CanEditDocuments", policy =>
                policy.RequireClaim("permission", "documents:edit"))
            .AddPolicy("CanDeleteDocuments", policy =>
                policy.RequireClaim("permission", "documents:delete"))
            .AddPolicy("CanPublishDocuments", policy =>
                policy.RequireClaim("permission", "documents:publish"))
            .AddPolicy("CanManageLibraries", policy =>
                policy.RequireClaim("permission", "documents:manage"));

        return services;
    }
}
