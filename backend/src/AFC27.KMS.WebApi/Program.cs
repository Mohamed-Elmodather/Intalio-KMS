using Serilog;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AFC27.KMS.WebApi.Extensions;
using AFC27.KMS.WebApi.Data;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting AFC27 Knowledge Management System API");

    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .WriteTo.Console());

    // Add services
    builder.Services.AddApplicationServices(builder.Configuration);
    builder.Services.AddInfrastructureServices(builder.Configuration);

    // Conditionally add authentication (can be disabled for development)
    var disableAuth = builder.Configuration.GetValue<bool>("DisableAuth");
    if (!disableAuth)
    {
        builder.Services.AddAuthenticationServices(builder.Configuration);
    }
    else
    {
        // Development mode: Add authorization with allow-all policies
        builder.Services.AddAuthorization(options =>
        {
            // Define all policies used in controllers with allow-all access
            var policyNames = new[]
            {
                "CanManageUsers", "CanViewUsers",
                "CanManageContent", "CanCreateContent", "CanEditContent", "CanDeleteContent", "CanPublishContent",
                "CanManageLibraries", "CanUploadDocuments", "CanEditDocuments", "CanDeleteDocuments", "CanPublishDocuments",
                "CanManageSearch",
                "CanManageCalendar",
                "CanManageWorkflow", "CanViewAllTasks", "CanAssignRequests",
                "CanApproveLessons",
                "AIAdmin", "IntegrationAdmin", "NotificationAdmin"
            };

            foreach (var policyName in policyNames)
            {
                options.AddPolicy(policyName, policy => policy.RequireAssertion(_ => true));
            }

            // Set default policy to allow all
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAssertion(_ => true)
                .Build();

            // Set fallback policy to allow all (for [Authorize] without policy)
            options.FallbackPolicy = null;
        });
    }

    builder.Services.AddModules(builder.Configuration);

    // Add API services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "AFC27 KMS API",
            Version = "v1",
            Description = "Knowledge Management System API for AFC Asian Cup 2027"
        });
    });

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins(
                    builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                    ?? new[] { "http://localhost:3000" })
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    // Add Health Checks (conditionally based on environment)
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var redisConnection = builder.Configuration.GetConnectionString("Redis");
    var useInMemoryCache = builder.Configuration.GetValue<bool>("UseInMemoryCache");
    var healthChecks = builder.Services.AddHealthChecks();

    // Only add SQL Server health check if not using SQLite
    if (!ConnectionStringHelper.IsSqlite(connectionString) && !string.IsNullOrEmpty(connectionString))
    {
        healthChecks.AddSqlServer(connectionString);
    }

    // Only add Redis health check if not using in-memory cache
    if (!useInMemoryCache && !string.IsNullOrEmpty(redisConnection))
    {
        healthChecks.AddRedis(redisConnection);
    }

    var app = builder.Build();

    // Ensure database is created and seeded
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<KmsDbContext>();

        if (ConnectionStringHelper.IsSqlite(connectionString))
        {
            // Create SQLite database if it doesn't exist
            dbContext.Database.EnsureCreated();
            Log.Information("SQLite database ensured at: {ConnectionString}", connectionString);
        }
        else
        {
            // For SQL Server, apply migrations
            dbContext.Database.Migrate();
            Log.Information("Database migrations applied");
        }

        // Seed data if configured
        var seedDatabase = builder.Configuration.GetValue<bool>("SeedDatabase");
        if (seedDatabase)
        {
            Log.Information("Seeding database with AFC 2027 demo data...");
            await DatabaseSeeder.SeedAsync(dbContext);
            Log.Information("Database seeding completed");
        }
    }

    // Configure pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.UseCors("AllowFrontend");

    if (!disableAuth)
    {
        app.UseAuthentication();
    }
    // Always use authorization (policies are configured in both modes)
    app.UseAuthorization();

    app.MapControllers();
    app.MapHealthChecks("/health/live");
    app.MapHealthChecks("/health/ready");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
