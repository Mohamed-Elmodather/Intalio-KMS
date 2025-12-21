using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using FluentValidation;
using AFC27.KMS.WebApi.Data;
using AFC27.KMS.WebApi.Services;
using AFC27.KMS.Infrastructure.Caching;
using AFC27.KMS.Infrastructure.Storage;
using AFC27.KMS.Infrastructure.Messaging;
using AFC27.KMS.Infrastructure.Persistence.Interceptors;
using AFC27.KMS.SharedKernel.Interfaces;
using AFC27.KMS.Documents.Application.Interfaces;
using AFC27.KMS.Documents.Application.Services;
using AFC27.KMS.Content.Application.Interfaces;
using AFC27.KMS.Content.Application.Services;
using AFC27.KMS.Admin.Application.Interfaces;
using AFC27.KMS.Admin.Application.Services;
using AFC27.KMS.AI.Application.Interfaces;
using AFC27.KMS.AI.Application.Services;
using AFC27.KMS.AI.Infrastructure.Clients;
using AFC27.KMS.WebApi.Integration;
using AFC27.KMS.WebApi.Features.Templates.Services;
using AFC27.KMS.WebApi.Features.Barcodes.Services;
using AFC27.KMS.WebApi.Features.QualityAssurance.Services;
using AFC27.KMS.WebApi.Features.KpiManagement.Services;
using AFC27.KMS.WebApi.Features.Polling.Services;
using AFC27.KMS.WebApi.Features.Learning.Services;
using AFC27.KMS.WebApi.Features.Meetings.Services;
using AFC27.KMS.WebApi.Features.AIAnalysis.Services;
using AFC27.KMS.WebApi.Features.Security.Services;
using AFC27.KMS.WebApi.Features.ServiceCatalog.Services;

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

        // HttpContext accessor for CurrentUser
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

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

        // Register abstract DbContext for services that depend on it
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<KmsDbContext>());

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

        // EF Core Interceptors
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<SoftDeleteInterceptor>();
        services.AddScoped<DomainEventDispatcherInterceptor>();

        // Storage Service
        var storageSection = configuration.GetSection("Storage");
        var storageProvider = storageSection.GetValue<string>("Provider") ?? "Local";

        if (storageProvider.Equals("Local", StringComparison.OrdinalIgnoreCase))
        {
            services.Configure<LocalStorageOptions>(storageSection.GetSection("Local"));
            services.AddSingleton<IStorageService, LocalStorageService>();
            services.AddSingleton<MultipartUploadHandler>();
        }

        // MassTransit (Message Queue)
        services.AddMassTransitWithRabbitMq(configuration);

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
                policy.RequireAuthenticatedUser()); // Allow any authenticated user to upload

            options.AddPolicy("CanEditDocuments", policy =>
                policy.RequireAuthenticatedUser()); // Will be enforced at service level via ACL

            options.AddPolicy("CanDeleteDocuments", policy =>
                policy.RequireAuthenticatedUser()); // Will be enforced at service level via ACL

            options.AddPolicy("CanPublishDocuments", policy =>
                policy.RequireAuthenticatedUser()); // Will be enforced at service level via ACL

            options.AddPolicy("CanManageLibraries", policy =>
                policy.RequireAuthenticatedUser()); // Will be enforced at service level via ACL

            // Admin permissions
            options.AddPolicy("CanManageUsers", policy =>
                policy.RequireRole("Administrator", "UserManager"));

            // Content management permissions
            options.AddPolicy("CanManageContent", policy =>
                policy.RequireRole("Administrator", "ContentManager", "Editor"));
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
        // Documents Module
        services.AddDocumentsModule(configuration);

        // Content Module (Block Editor + Collaboration)
        services.AddContentModule(configuration);

        // Admin Module (Audit, Legal Hold, User Lifecycle)
        services.AddAdminModule(configuration);

        // AI Module (RAG, Chat, Embeddings)
        services.AddAIModule(configuration);

        // Integration Services (OCR, Signature, Meeting, AI Engine)
        services.AddIntegrationServices(configuration);

        // Document Templates
        services.AddScoped<IDocumentTemplateService, DocumentTemplateService>();

        // Barcode/QR Code Services
        services.AddScoped<IBarcodeService, BarcodeService>();

        // Quality Assurance Services
        services.AddScoped<IQualityAssuranceService, QualityAssuranceService>();

        // KPI Management Services
        services.AddScoped<IKpiService, KpiService>();

        // Polling/Voting Services
        services.AddScoped<IPollingService, PollingService>();

        // Learning Management Services
        services.AddScoped<ILearningService, LearningService>();

        // Meeting Management Services (Phase 2)
        services.AddScoped<IMeetingLinkService, MeetingLinkService>();

        // AI Analysis Services (Phase 3)
        services.AddScoped<IAIAnalysisService, AIAnalysisService>();

        // Security/Encryption Services (Phase 4)
        services.AddScoped<IEncryptionService, EncryptionService>();

        // Service Catalog Services (Phase 5.3)
        services.AddScoped<IServiceCatalogService, ServiceCatalogService>();

        return services;
    }

    /// <summary>
    /// Adds Documents module services.
    /// </summary>
    public static IServiceCollection AddDocumentsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Core document services
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<ILibraryService, LibraryService>();
        services.AddScoped<IFolderService, FolderService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IPreviewService, PreviewService>();

        // Preview settings
        services.Configure<PreviewSettings>(configuration.GetSection("Preview"));

        return services;
    }

    /// <summary>
    /// Adds Content module services (Block Editor + Live Collaboration).
    /// </summary>
    public static IServiceCollection AddContentModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Block Editor Services
        services.AddScoped<IBlockEditorService, BlockEditorService>();

        // Collaboration Services
        services.AddScoped<ICollaborationService, CollaborationService>();
        services.AddScoped<IPresenceService, PresenceService>();

        // CRDT Service (singleton for stateless operations)
        services.AddSingleton<ICRDTService, CRDTService>();

        // SignalR for real-time collaboration
        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
            options.MaximumReceiveMessageSize = 1024 * 1024; // 1MB for CRDT updates
        });

        return services;
    }

    /// <summary>
    /// Adds Admin module services (Audit Logging, Legal Hold, Quarantine, User Lifecycle).
    /// </summary>
    public static IServiceCollection AddAdminModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Audit logging
        services.AddScoped<IAuditLogService, AuditLogService>();

        // Legal hold management
        services.AddScoped<ILegalHoldService, LegalHoldService>();

        // Document quarantine
        services.AddScoped<IQuarantineService, QuarantineService>();

        // User lifecycle management
        services.AddScoped<IUserLifecycleService, UserLifecycleService>();

        // Impersonation service
        services.AddScoped<IImpersonationService, ImpersonationService>();

        return services;
    }

    /// <summary>
    /// Adds AI module services (RAG, Chat, Embeddings).
    /// </summary>
    public static IServiceCollection AddAIModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var aiSettings = configuration.GetSection("AI");
        var useMock = aiSettings.GetValue<bool>("UseMock", true);

        // AI Client - Mock or Intalio
        if (useMock)
        {
            services.AddScoped<IIntalioAIClient, MockIntalioAIClient>();
        }
        else
        {
            // TODO: Register actual Intalio AI client when available
            services.AddScoped<IIntalioAIClient, MockIntalioAIClient>();
        }

        // Embedding service (using concrete type for status methods)
        services.AddScoped<EmbeddingService>();
        services.AddScoped<IEmbeddingService>(sp => sp.GetRequiredService<EmbeddingService>());

        // RAG service
        services.AddScoped<IRAGService, RAGService>();

        // Chat service (using concrete type for additional methods)
        services.AddScoped<ChatService>();
        services.AddScoped<IChatService>(sp => sp.GetRequiredService<ChatService>());

        return services;
    }
}
