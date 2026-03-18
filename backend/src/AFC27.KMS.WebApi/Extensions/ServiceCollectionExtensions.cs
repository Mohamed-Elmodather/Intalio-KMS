using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using FluentValidation;
using AFC27.KMS.WebApi.Data;
using AFC27.KMS.Infrastructure.Caching;
using AFC27.KMS.SharedKernel.Interfaces;
using AFC27.KMS.Collaboration;
using AFC27.KMS.Workflow;

namespace AFC27.KMS.WebApi.Extensions;

/// <summary>
/// Helper to detect database provider from connection string
/// </summary>
public static class ConnectionStringHelper
{
    public static bool IsSqlite(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return false;

        return connectionString.Contains("Data Source=", StringComparison.OrdinalIgnoreCase)
               && connectionString.EndsWith(".db", StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Extension methods for service registration.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds core application services.
    /// </summary>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<Program>();
        });

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<Program>();

        return services;
    }

    /// <summary>
    /// Adds infrastructure services (database, cache, etc.).
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var useInMemoryCache = configuration.GetValue<bool>("UseInMemoryCache");
        var redisConnection = configuration.GetConnectionString("Redis");

        // Database - SQLite for development, SQL Server for production
        if (ConnectionStringHelper.IsSqlite(connectionString))
        {
            services.AddDbContext<KmsDbContext>(options =>
            {
                options.UseSqlite(connectionString);
            });
        }
        else
        {
            services.AddDbContext<KmsDbContext>(options =>
            {
                options.UseSqlServer(
                    connectionString,
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
            });
        }

        // Cache - In-memory for development, Redis for production
        if (useInMemoryCache || string.IsNullOrEmpty(redisConnection))
        {
            services.AddMemoryCache();
            services.AddScoped<ICacheService, MemoryCacheService>();
        }
        else
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "AFC27KMS:";
            });
            services.AddScoped<ICacheService, RedisCacheService>();
        }

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<KmsDbContext>());

        // Allow modules to resolve the base DbContext type
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<KmsDbContext>());

        return services;
    }

    /// <summary>
    /// Adds authentication and authorization services.
    /// </summary>
    public static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(options =>
            {
                configuration.Bind("AzureAd", options);
                options.TokenValidationParameters.NameClaimType = "name";
            },
            options => configuration.Bind("AzureAd", options));

        services.AddAuthorization(options =>
        {
            // Content permissions
            options.AddPolicy("CanViewContent", policy =>
                policy.RequireAuthenticatedUser());

            options.AddPolicy("CanCreateContent", policy =>
                policy.RequireClaim("permissions", "content:create"));

            options.AddPolicy("CanPublishContent", policy =>
                policy.RequireClaim("permissions", "content:publish"));

            // Document permissions
            options.AddPolicy("CanViewDocuments", policy =>
                policy.RequireAuthenticatedUser());

            options.AddPolicy("CanUploadDocuments", policy =>
                policy.RequireClaim("permissions", "documents:upload"));

            // Admin permissions
            options.AddPolicy("CanManageUsers", policy =>
                policy.RequireRole("Administrator", "UserManager"));
        });

        return services;
    }

    /// <summary>
    /// Registers all feature modules.
    /// </summary>
    public static IServiceCollection AddModules(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Each module will register its own services
        // This will be implemented as we build each module

        // services.AddIdentityModule(configuration);
        // services.AddContentModule(configuration);
        // services.AddDocumentsModule(configuration);
        // etc.

        services.AddCollaborationModule(configuration);
        services.AddWorkflowModule(configuration);

        return services;
    }
}
