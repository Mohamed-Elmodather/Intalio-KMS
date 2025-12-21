using AFC27.KMS.Infrastructure.Messaging;
using AFC27.KMS.Infrastructure.Storage;
using AFC27.KMS.MediaWorker.Consumers;
using AFC27.KMS.MediaWorker.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting AFC27 KMS Media Worker");

    var builder = Host.CreateApplicationBuilder(args);

    // Configure Serilog
    builder.Services.AddSerilog();

    // Configure Storage
    builder.Services.Configure<LocalStorageOptions>(
        builder.Configuration.GetSection(LocalStorageOptions.SectionName));
    builder.Services.AddSingleton<IStorageService, LocalStorageService>();

    // Configure MassTransit
    builder.Services.AddMassTransitWithRabbitMq(builder.Configuration);

    // Register Services
    builder.Services.AddSingleton<ImageProcessingService>();
    builder.Services.AddSingleton<FFmpegService>();

    // Register Consumers
    builder.Services.AddScoped<ThumbnailGenerationConsumer>();
    builder.Services.AddScoped<VideoTranscodingConsumer>();

    var host = builder.Build();
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Media Worker terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
