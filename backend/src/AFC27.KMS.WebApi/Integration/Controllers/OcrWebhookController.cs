using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AFC27.KMS.WebApi.Integration.Configuration;
using AFC27.KMS.WebApi.Integration.Ocr;
using AFC27.KMS.WebApi.Integration.Ocr.Models;

namespace AFC27.KMS.WebApi.Integration.Controllers;

/// <summary>
/// Controller for handling OCR service webhooks
/// </summary>
[ApiController]
[Route("api/webhooks/ocr")]
public class OcrWebhookController : ControllerBase
{
    private readonly IOcrIntegrationService _ocrService;
    private readonly ILogger<OcrWebhookController> _logger;
    private readonly IntegrationSettings _settings;

    public OcrWebhookController(
        IOcrIntegrationService ocrService,
        ILogger<OcrWebhookController> logger,
        IOptions<IntegrationSettings> settings)
    {
        _ocrService = ocrService;
        _logger = logger;
        _settings = settings.Value;
    }

    /// <summary>
    /// Receives OCR job status updates from external OCR service
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> HandleOcrWebhook([FromBody] OcrWebhookPayload payload)
    {
        _logger.LogInformation(
            "Received OCR webhook for job {JobId}, status: {Status}",
            payload.JobId, payload.Status);

        // Validate webhook signature
        var webhookSecret = _settings.Ocr?.WebhookSecret;
        if (!string.IsNullOrEmpty(webhookSecret))
        {
            if (!_ocrService.ValidateWebhookSignature(payload, webhookSecret))
            {
                _logger.LogWarning("Invalid webhook signature for OCR job {JobId}", payload.JobId);
                return Unauthorized(new { error = "Invalid signature" });
            }
        }

        try
        {
            // Process the webhook based on status
            switch (payload.Status)
            {
                case OcrJobStatus.Completed:
                    await HandleOcrCompletedAsync(payload);
                    break;

                case OcrJobStatus.Failed:
                    await HandleOcrFailedAsync(payload);
                    break;

                case OcrJobStatus.Cancelled:
                    _logger.LogInformation("OCR job {JobId} was cancelled", payload.JobId);
                    break;

                default:
                    _logger.LogInformation(
                        "OCR job {JobId} status updated to {Status}",
                        payload.JobId, payload.Status);
                    break;
            }

            return Ok(new { received = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing OCR webhook for job {JobId}", payload.JobId);
            // Return 200 to prevent retries, but log the error
            return Ok(new { received = true, error = "Processing failed" });
        }
    }

    private async Task HandleOcrCompletedAsync(OcrWebhookPayload payload)
    {
        _logger.LogInformation(
            "OCR job {JobId} completed for document {DocumentId}",
            payload.JobId, payload.DocumentId);

        // Get the OCR result
        var result = await _ocrService.GetJobResultAsync(payload.JobId);
        if (result.IsSuccess && result.Data != null)
        {
            // TODO: Store OCR result in database
            // TODO: Update document with extracted text
            // TODO: Trigger indexing for search

            _logger.LogInformation(
                "OCR result retrieved for job {JobId}: {PageCount} pages, confidence: {Confidence}",
                payload.JobId,
                result.Data.Pages?.Count ?? 0,
                result.Data.Confidence);
        }
        else
        {
            _logger.LogWarning(
                "Failed to retrieve OCR result for job {JobId}: {Error}",
                payload.JobId, result.ErrorMessage);
        }
    }

    private Task HandleOcrFailedAsync(OcrWebhookPayload payload)
    {
        _logger.LogError(
            "OCR job {JobId} failed for document {DocumentId}: {Error}",
            payload.JobId, payload.DocumentId, payload.ErrorMessage);

        // TODO: Update document status to indicate OCR failure
        // TODO: Notify relevant users

        return Task.CompletedTask;
    }
}
