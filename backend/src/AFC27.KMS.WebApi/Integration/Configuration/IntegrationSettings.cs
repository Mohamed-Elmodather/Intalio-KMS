using AFC27.KMS.WebApi.Integration.Core;

namespace AFC27.KMS.WebApi.Integration.Configuration;

/// <summary>
/// Root configuration for all external service integrations
/// </summary>
public class IntegrationSettings
{
    public const string SectionName = "ExternalServices";

    public OcrServiceSettings Ocr { get; set; } = new();
    public SignatureServiceSettings DigitalSignature { get; set; } = new();
    public MeetingServiceSettings MeetingManagement { get; set; } = new();
    public AiEngineSettings AiEngine { get; set; } = new();
}

/// <summary>
/// OCR Service configuration
/// </summary>
public class OcrServiceSettings : ExternalServiceSettings
{
    public string WebhookEndpoint { get; set; } = "/api/webhooks/ocr";
    public string WebhookSecret { get; set; } = string.Empty;
    public int MaxFileSizeMB { get; set; } = 50;
    public string[] SupportedFormats { get; set; } = { "pdf", "png", "jpg", "jpeg", "tiff", "bmp" };
}

/// <summary>
/// Digital Signature Service configuration
/// </summary>
public class SignatureServiceSettings : ExternalServiceSettings
{
    public string WebhookEndpoint { get; set; } = "/api/webhooks/signature";
    public string WebhookSecret { get; set; } = string.Empty;
    public string CallbackUrl { get; set; } = string.Empty;
    public string SigningPortalUrl { get; set; } = string.Empty;
    public int SignatureValidityDays { get; set; } = 365;
}

/// <summary>
/// Meeting Management Service configuration
/// </summary>
public class MeetingServiceSettings : ExternalServiceSettings
{
    public string WebhookEndpoint { get; set; } = "/api/webhooks/meetings";
    public string WebhookSecret { get; set; } = string.Empty;
    public int SyncIntervalMinutes { get; set; } = 15;
    public bool EnableCalendarSync { get; set; } = true;
    public bool EnableTeamsIntegration { get; set; } = true;
}

/// <summary>
/// AI Engine Service configuration
/// </summary>
public class AiEngineSettings : ExternalServiceSettings
{
    public string WebhookEndpoint { get; set; } = "/api/webhooks/ai";
    public string WebhookSecret { get; set; } = string.Empty;
    public int MaxTextLength { get; set; } = 100000;
    public int MaxAudioDurationMinutes { get; set; } = 120;
    public string DefaultLanguage { get; set; } = "en";
    public string[] SupportedLanguages { get; set; } = { "en", "ar" };
    public bool EnableSentimentAnalysis { get; set; } = true;
    public bool EnableEntityExtraction { get; set; } = true;
    public bool EnableAutoTagging { get; set; } = true;
}
