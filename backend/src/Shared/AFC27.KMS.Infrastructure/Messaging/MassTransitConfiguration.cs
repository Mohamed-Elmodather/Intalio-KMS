using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AFC27.KMS.Infrastructure.Messaging;

/// <summary>
/// MassTransit configuration for message queue setup.
/// Supports RabbitMQ for production and In-Memory for development/testing.
/// </summary>
public static class MassTransitConfiguration
{
    /// <summary>
    /// Adds MassTransit with RabbitMQ to the service collection.
    /// </summary>
    public static IServiceCollection AddMassTransitWithRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration.GetSection(MassTransitOptions.SectionName).Get<MassTransitOptions>()
            ?? new MassTransitOptions();

        services.Configure<MassTransitOptions>(configuration.GetSection(MassTransitOptions.SectionName));

        services.AddMassTransit(x =>
        {
            // Add consumers from the Infrastructure assembly
            x.AddConsumers(typeof(MassTransitConfiguration).Assembly);

            if (options.UseInMemory)
            {
                // In-Memory transport for development/testing
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            }
            else
            {
                // RabbitMQ transport for production
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(options.Host, options.VirtualHost, h =>
                    {
                        h.Username(options.Username);
                        h.Password(options.Password);
                    });

                    // Configure retry policy
                    cfg.UseMessageRetry(r =>
                    {
                        r.Intervals(
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(15),
                            TimeSpan.FromSeconds(30),
                            TimeSpan.FromMinutes(1),
                            TimeSpan.FromMinutes(5));
                    });

                    // Configure circuit breaker
                    cfg.UseCircuitBreaker(cb =>
                    {
                        cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                        cb.TripThreshold = 15;
                        cb.ActiveThreshold = 10;
                        cb.ResetInterval = TimeSpan.FromMinutes(5);
                    });

                    // Configure specific queues
                    ConfigureQueues(cfg, context, options);
                });
            }
        });

        return services;
    }

    private static void ConfigureQueues(
        IRabbitMqBusFactoryConfigurator cfg,
        IBusRegistrationContext context,
        MassTransitOptions options)
    {
        // Document Processing Queue
        cfg.ReceiveEndpoint(options.Queues.DocumentProcessing, e =>
        {
            e.PrefetchCount = 16;
            e.UseMessageRetry(r => r.Intervals(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15)));
            e.ConfigureConsumers(context);
        });

        // AI Ingestion Queue
        cfg.ReceiveEndpoint(options.Queues.AIIngestion, e =>
        {
            e.PrefetchCount = 8;
            e.UseMessageRetry(r => r.Intervals(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30)));
            e.ConfigureConsumers(context);
        });

        // Media Transcoding Queue
        cfg.ReceiveEndpoint(options.Queues.MediaTranscoding, e =>
        {
            e.PrefetchCount = 4;
            e.UseMessageRetry(r => r.Intervals(TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(1)));
            e.ConfigureConsumers(context);
        });

        // Notification Queue
        cfg.ReceiveEndpoint(options.Queues.Notifications, e =>
        {
            e.PrefetchCount = 32;
            e.UseMessageRetry(r => r.Intervals(TimeSpan.FromSeconds(5)));
            e.ConfigureConsumers(context);
        });
    }
}

/// <summary>
/// MassTransit configuration options.
/// </summary>
public class MassTransitOptions
{
    public const string SectionName = "MassTransit";

    /// <summary>
    /// Use in-memory transport (for development).
    /// </summary>
    public bool UseInMemory { get; set; } = true;

    /// <summary>
    /// RabbitMQ host address.
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// RabbitMQ virtual host.
    /// </summary>
    public string VirtualHost { get; set; } = "/";

    /// <summary>
    /// RabbitMQ username.
    /// </summary>
    public string Username { get; set; } = "guest";

    /// <summary>
    /// RabbitMQ password.
    /// </summary>
    public string Password { get; set; } = "guest";

    /// <summary>
    /// Queue names configuration.
    /// </summary>
    public QueueNames Queues { get; set; } = new();
}

/// <summary>
/// Queue name configuration.
/// </summary>
public class QueueNames
{
    public string DocumentProcessing { get; set; } = "document-processing";
    public string AIIngestion { get; set; } = "ai-ingestion";
    public string MediaTranscoding { get; set; } = "media-transcoding";
    public string Notifications { get; set; } = "notifications";
}
