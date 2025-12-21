using AFC27.KMS.Infrastructure.Messaging;
using AFC27.KMS.Infrastructure.Storage;
using AFC27.KMS.AIWorker.Consumers;
using AFC27.KMS.AIWorker.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting AFC27 KMS AI Worker");

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
    builder.Services.AddSingleton<TextExtractionService>();
    builder.Services.AddSingleton<ChunkingService>();
    builder.Services.AddSingleton<MockEmbeddingService>();

    // Register Consumers
    builder.Services.AddScoped<IngestionConsumer>();
    builder.Services.AddScoped<EmbeddingConsumer>();

    var host = builder.Build();
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "AI Worker terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
