using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AFC27.KMS.WebApi.Integration.Configuration;
using AFC27.KMS.WebApi.Integration.Core;
using AFC27.KMS.WebApi.Integration.Ocr;
using AFC27.KMS.WebApi.Integration.Signature;
using AFC27.KMS.WebApi.Integration.Meeting;
using AFC27.KMS.WebApi.Integration.AiEngine;

namespace AFC27.KMS.WebApi.Integration;

/// <summary>
/// Extension methods for registering integration services
/// </summary>
public static class IntegrationServiceExtensions
{
    /// <summary>
    /// Adds all external integration services to the DI container
    /// </summary>
    public static IServiceCollection AddIntegrationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind configuration
        services.Configure<IntegrationSettings>(
            configuration.GetSection("Integration"));

        var integrationSettings = configuration
            .GetSection("Integration")
            .Get<IntegrationSettings>() ?? new IntegrationSettings();

        // Register OCR Integration Service
        if (integrationSettings.Ocr?.IsEnabled == true)
        {
            services.AddHttpClient<IOcrIntegrationService, OcrIntegrationService>(client =>
            {
                ConfigureHttpClient(client, integrationSettings.Ocr);
            });
        }
        else
        {
            // Register as null service or mock for disabled integrations
            services.AddSingleton<IOcrIntegrationService>(sp =>
                new DisabledIntegrationService<IOcrIntegrationService>("OCR"));
        }

        // Register Digital Signature Integration Service
        if (integrationSettings.DigitalSignature?.IsEnabled == true)
        {
            services.AddHttpClient<ISignatureIntegrationService, SignatureIntegrationService>(client =>
            {
                ConfigureHttpClient(client, integrationSettings.DigitalSignature);
            });
        }
        else
        {
            services.AddSingleton<ISignatureIntegrationService>(sp =>
                new DisabledIntegrationService<ISignatureIntegrationService>("DigitalSignature"));
        }

        // Register Meeting Integration Service
        if (integrationSettings.MeetingManagement?.IsEnabled == true)
        {
            services.AddHttpClient<IMeetingIntegrationService, MeetingIntegrationService>(client =>
            {
                ConfigureHttpClient(client, integrationSettings.MeetingManagement);
            });
        }
        else
        {
            services.AddSingleton<IMeetingIntegrationService>(sp =>
                new DisabledIntegrationService<IMeetingIntegrationService>("Meeting"));
        }

        // Register AI Engine Integration Service
        if (integrationSettings.AiEngine?.IsEnabled == true)
        {
            services.AddHttpClient<IAiEngineIntegrationService, AiEngineIntegrationService>(client =>
            {
                ConfigureHttpClient(client, integrationSettings.AiEngine);
            });
        }
        else
        {
            services.AddSingleton<IAiEngineIntegrationService>(sp =>
                new DisabledIntegrationService<IAiEngineIntegrationService>("AiEngine"));
        }

        // Register Health Monitor
        services.AddSingleton<IIntegrationHealthMonitor, IntegrationHealthMonitor>();

        // Register health monitor with all services on startup
        services.AddHostedService<IntegrationHealthRegistrationService>();

        return services;
    }

    private static void ConfigureHttpClient(HttpClient client, ExternalServiceSettings settings)
    {
        if (!string.IsNullOrEmpty(settings.BaseUrl))
        {
            client.BaseAddress = new Uri(settings.BaseUrl);
        }

        if (settings.TimeoutSeconds > 0)
        {
            client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
        }

        if (!string.IsNullOrEmpty(settings.ApiKey))
        {
            client.DefaultRequestHeaders.Add("X-API-Key", settings.ApiKey);
        }

        client.DefaultRequestHeaders.Add("Accept", "application/json");
    }
}

/// <summary>
/// Placeholder service for disabled integrations
/// </summary>
internal class DisabledIntegrationService<T> :
    IOcrIntegrationService,
    ISignatureIntegrationService,
    IMeetingIntegrationService,
    IAiEngineIntegrationService
{
    private readonly string _serviceName;

    public string ServiceName => _serviceName;

    public DisabledIntegrationService(string serviceName)
    {
        _serviceName = serviceName;
    }

    public Task<ServiceHealthStatus> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ServiceHealthStatus
        {
            ServiceName = _serviceName,
            IsHealthy = false,
            Message = "Service is disabled",
            CheckedAt = DateTime.UtcNow
        });
    }

    public CircuitBreakerState GetCircuitBreakerState() => CircuitBreakerState.Open;

    // OCR methods
    public Task<ServiceResponse<Ocr.Models.OcrJobResponse>> SubmitDocumentAsync(
        Ocr.Models.OcrJobRequest request, CancellationToken cancellationToken = default)
        => DisabledResponse<Ocr.Models.OcrJobResponse>();

    public Task<ServiceResponse<Ocr.Models.OcrStatusResponse>> GetJobStatusAsync(
        string jobId, CancellationToken cancellationToken = default)
        => DisabledResponse<Ocr.Models.OcrStatusResponse>();

    public Task<ServiceResponse<Ocr.Models.OcrResultResponse>> GetJobResultAsync(
        string jobId, CancellationToken cancellationToken = default)
        => DisabledResponse<Ocr.Models.OcrResultResponse>();

    public Task<ServiceResponse<bool>> CancelJobAsync(
        string jobId, CancellationToken cancellationToken = default)
        => DisabledResponse<bool>();

    public bool ValidateWebhookSignature(Ocr.Models.OcrWebhookPayload payload, string secret) => false;

    public Task<ServiceResponse<Ocr.Models.OcrResultResponse>> ProcessDocumentAsync(
        Ocr.Models.OcrJobRequest request, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        => DisabledResponse<Ocr.Models.OcrResultResponse>();

    // Signature methods
    public Task<ServiceResponse<Signature.Models.SignatureRequestResponse>> CreateSignatureRequestAsync(
        Signature.Models.SignatureRequestModel request, CancellationToken cancellationToken = default)
        => DisabledResponse<Signature.Models.SignatureRequestResponse>();

    public Task<ServiceResponse<Signature.Models.SignatureRequestResponse>> GetSignatureRequestStatusAsync(
        string requestId, CancellationToken cancellationToken = default)
        => DisabledResponse<Signature.Models.SignatureRequestResponse>();

    public Task<ServiceResponse<Signature.Models.SigningUrlResponse>> GetSigningUrlAsync(
        string requestId, string signerId, CancellationToken cancellationToken = default)
        => DisabledResponse<Signature.Models.SigningUrlResponse>();

    public Task<ServiceResponse<bool>> CancelSignatureRequestAsync(
        string requestId, string reason, CancellationToken cancellationToken = default)
        => DisabledResponse<bool>();

    public Task<ServiceResponse<Signature.Models.SignatureVerificationResult>> VerifySignaturesAsync(
        Signature.Models.SignatureVerificationRequest request, CancellationToken cancellationToken = default)
        => DisabledResponse<Signature.Models.SignatureVerificationResult>();

    public Task<ServiceResponse<bool>> SendReminderAsync(
        string requestId, string? signerId = null, CancellationToken cancellationToken = default)
        => DisabledResponse<bool>();

    public Task<ServiceResponse<byte[]>> DownloadSignedDocumentAsync(
        string requestId, CancellationToken cancellationToken = default)
        => DisabledResponse<byte[]>();

    public bool ValidateWebhookSignature(Signature.Models.SignatureWebhookPayload payload, string secret) => false;

    // Meeting methods
    public Task<ServiceResponse<Meeting.Models.MeetingResponse>> CreateMeetingAsync(
        Meeting.Models.CreateMeetingRequest request, CancellationToken cancellationToken = default)
        => DisabledResponse<Meeting.Models.MeetingResponse>();

    public Task<ServiceResponse<Meeting.Models.MeetingResponse>> UpdateMeetingAsync(
        string meetingId, Meeting.Models.CreateMeetingRequest request, CancellationToken cancellationToken = default)
        => DisabledResponse<Meeting.Models.MeetingResponse>();

    public Task<ServiceResponse<Meeting.Models.MeetingResponse>> GetMeetingAsync(
        string meetingId, CancellationToken cancellationToken = default)
        => DisabledResponse<Meeting.Models.MeetingResponse>();

    public Task<ServiceResponse<bool>> CancelMeetingAsync(
        string meetingId, string reason, bool notifyParticipants = true, CancellationToken cancellationToken = default)
        => DisabledResponse<bool>();

    public Task<ServiceResponse<Meeting.Models.MeetingRecording>> GetMeetingRecordingAsync(
        string meetingId, CancellationToken cancellationToken = default)
        => DisabledResponse<Meeting.Models.MeetingRecording>();

    public Task<ServiceResponse<Meeting.Models.MeetingMinutes>> GetMeetingMinutesAsync(
        string meetingId, CancellationToken cancellationToken = default)
        => DisabledResponse<Meeting.Models.MeetingMinutes>();

    public Task<ServiceResponse<Meeting.Models.MeetingMinutes>> UpdateMeetingMinutesAsync(
        string meetingId, Meeting.Models.MeetingMinutes minutes, CancellationToken cancellationToken = default)
        => DisabledResponse<Meeting.Models.MeetingMinutes>();

    public Task<ServiceResponse<Meeting.Models.ActionItem>> AddActionItemAsync(
        string meetingId, Meeting.Models.ActionItem actionItem, CancellationToken cancellationToken = default)
        => DisabledResponse<Meeting.Models.ActionItem>();

    public Task<ServiceResponse<Meeting.Models.ActionItem>> UpdateActionItemAsync(
        string meetingId, string actionId, Meeting.Models.ActionItemStatus status, CancellationToken cancellationToken = default)
        => DisabledResponse<Meeting.Models.ActionItem>();

    public Task<ServiceResponse<bool>> SendInvitationsAsync(
        string meetingId, CancellationToken cancellationToken = default)
        => DisabledResponse<bool>();

    public bool ValidateWebhookSignature(Meeting.Models.MeetingWebhookPayload payload, string secret) => false;

    // AI Engine methods
    public Task<ServiceResponse<AiEngine.Models.DocumentClassificationResult>> ClassifyDocumentAsync(
        AiEngine.Models.DocumentClassificationRequest request, CancellationToken cancellationToken = default)
        => DisabledResponse<AiEngine.Models.DocumentClassificationResult>();

    public Task<ServiceResponse<AiEngine.Models.SummarizationResult>> SummarizeContentAsync(
        AiEngine.Models.SummarizationRequest request, CancellationToken cancellationToken = default)
        => DisabledResponse<AiEngine.Models.SummarizationResult>();

    public Task<ServiceResponse<AiEngine.Models.SemanticSearchResult>> SemanticSearchAsync(
        AiEngine.Models.SemanticSearchRequest request, CancellationToken cancellationToken = default)
        => DisabledResponse<AiEngine.Models.SemanticSearchResult>();

    public Task<ServiceResponse<AiEngine.Models.RecommendationResult>> GetRecommendationsAsync(
        AiEngine.Models.RecommendationRequest request, CancellationToken cancellationToken = default)
        => DisabledResponse<AiEngine.Models.RecommendationResult>();

    public Task<ServiceResponse<AiEngine.Models.EntityExtractionResult>> ExtractEntitiesAsync(
        AiEngine.Models.EntityExtractionRequest request, CancellationToken cancellationToken = default)
        => DisabledResponse<AiEngine.Models.EntityExtractionResult>();

    public Task<ServiceResponse<AiEngine.Models.QualityAnalysisResult>> AnalyzeQualityAsync(
        AiEngine.Models.QualityAnalysisRequest request, CancellationToken cancellationToken = default)
        => DisabledResponse<AiEngine.Models.QualityAnalysisResult>();

    public Task<ServiceResponse<bool>> IndexDocumentAsync(
        Guid documentId, string content, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default)
        => DisabledResponse<bool>();

    public Task<ServiceResponse<bool>> RemoveFromIndexAsync(
        Guid documentId, CancellationToken cancellationToken = default)
        => DisabledResponse<bool>();

    public bool ValidateWebhookSignature(AiEngine.Models.AiEngineWebhookPayload payload, string secret) => false;

    private static Task<ServiceResponse<TResponse>> DisabledResponse<TResponse>()
    {
        return Task.FromResult(ServiceResponse<TResponse>.Failure(
            "Service is disabled",
            "SERVICE_DISABLED"));
    }
}

/// <summary>
/// Background service to register integration services with health monitor
/// </summary>
internal class IntegrationHealthRegistrationService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IIntegrationHealthMonitor _healthMonitor;

    public IntegrationHealthRegistrationService(
        IServiceProvider serviceProvider,
        IIntegrationHealthMonitor healthMonitor)
    {
        _serviceProvider = serviceProvider;
        _healthMonitor = healthMonitor;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        // Register each service with the health monitor
        var ocrService = scope.ServiceProvider.GetService<IOcrIntegrationService>();
        if (ocrService != null)
            _healthMonitor.RegisterService(ocrService);

        var signatureService = scope.ServiceProvider.GetService<ISignatureIntegrationService>();
        if (signatureService != null)
            _healthMonitor.RegisterService(signatureService);

        var meetingService = scope.ServiceProvider.GetService<IMeetingIntegrationService>();
        if (meetingService != null)
            _healthMonitor.RegisterService(meetingService);

        var aiService = scope.ServiceProvider.GetService<IAiEngineIntegrationService>();
        if (aiService != null)
            _healthMonitor.RegisterService(aiService);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
