using AFC27.KMS.Infrastructure.Messaging;
using AFC27.KMS.NotificationWorker.Consumers;
using AFC27.KMS.NotificationWorker.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting AFC27 KMS Notification Worker");

    var builder = Host.CreateApplicationBuilder(args);

    // Configure Serilog
    builder.Services.AddSerilog();

    // Configure MassTransit
    builder.Services.AddMassTransitWithRabbitMq(builder.Configuration);

    // Register Services
    builder.Services.Configure<EmailOptions>(
        builder.Configuration.GetSection(EmailOptions.SectionName));
    builder.Services.AddSingleton<EmailService>();
    builder.Services.AddSingleton<PushNotificationService>();
    builder.Services.AddSingleton<InAppNotificationService>();

    // Register Consumers
    builder.Services.AddScoped<NotificationConsumer>();
    builder.Services.AddScoped<EmailConsumer>();
    builder.Services.AddScoped<BulkNotificationConsumer>();

    var host = builder.Build();
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Notification Worker terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
