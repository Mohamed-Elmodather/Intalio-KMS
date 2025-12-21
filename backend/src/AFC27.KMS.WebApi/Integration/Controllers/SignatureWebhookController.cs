using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AFC27.KMS.WebApi.Integration.Configuration;
using AFC27.KMS.WebApi.Integration.Signature;
using AFC27.KMS.WebApi.Integration.Signature.Models;

namespace AFC27.KMS.WebApi.Integration.Controllers;

/// <summary>
/// Controller for handling digital signature service webhooks
/// </summary>
[ApiController]
[Route("api/webhooks/signature")]
public class SignatureWebhookController : ControllerBase
{
    private readonly ISignatureIntegrationService _signatureService;
    private readonly ILogger<SignatureWebhookController> _logger;
    private readonly IntegrationSettings _settings;

    public SignatureWebhookController(
        ISignatureIntegrationService signatureService,
        ILogger<SignatureWebhookController> logger,
        IOptions<IntegrationSettings> settings)
    {
        _signatureService = signatureService;
        _logger = logger;
        _settings = settings.Value;
    }

    /// <summary>
    /// Receives signature request status updates from external signature service
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> HandleSignatureWebhook([FromBody] SignatureWebhookPayload payload)
    {
        _logger.LogInformation(
            "Received signature webhook for request {RequestId}, event: {Event}",
            payload.RequestId, payload.Event);

        // Validate webhook signature
        var webhookSecret = _settings.DigitalSignature?.WebhookSecret;
        if (!string.IsNullOrEmpty(webhookSecret))
        {
            if (!_signatureService.ValidateWebhookSignature(payload, webhookSecret))
            {
                _logger.LogWarning(
                    "Invalid webhook signature for signature request {RequestId}",
                    payload.RequestId);
                return Unauthorized(new { error = "Invalid signature" });
            }
        }

        try
        {
            // Process the webhook based on event type
            switch (payload.Event.ToLowerInvariant())
            {
                case "request.completed":
                    await HandleSignatureCompletedAsync(payload);
                    break;

                case "request.declined":
                    await HandleSignatureDeclinedAsync(payload);
                    break;

                case "signer.signed":
                    await HandleSignerSignedAsync(payload);
                    break;

                case "signer.viewed":
                    await HandleSignerViewedAsync(payload);
                    break;

                case "request.expired":
                    await HandleSignatureExpiredAsync(payload);
                    break;

                default:
                    _logger.LogInformation(
                        "Signature request {RequestId} event: {Event}",
                        payload.RequestId, payload.Event);
                    break;
            }

            return Ok(new { received = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing signature webhook for request {RequestId}",
                payload.RequestId);
            return Ok(new { received = true, error = "Processing failed" });
        }
    }

    private async Task HandleSignatureCompletedAsync(SignatureWebhookPayload payload)
    {
        _logger.LogInformation(
            "Signature request {RequestId} completed for document {DocumentId}",
            payload.RequestId, payload.DocumentId);

        // Download the signed document
        var signedDocResult = await _signatureService.DownloadSignedDocumentAsync(payload.RequestId);
        if (signedDocResult.IsSuccess && signedDocResult.Data != null)
        {
            // TODO: Store signed document
            // TODO: Update document status
            // TODO: Notify document owner

            _logger.LogInformation(
                "Signed document downloaded for request {RequestId}, size: {Size} bytes",
                payload.RequestId, signedDocResult.Data.Length);
        }
        else
        {
            _logger.LogWarning(
                "Failed to download signed document for request {RequestId}: {Error}",
                payload.RequestId, signedDocResult.ErrorMessage);
        }
    }

    private Task HandleSignatureDeclinedAsync(SignatureWebhookPayload payload)
    {
        _logger.LogWarning(
            "Signature request {RequestId} was declined by {SignerEmail}",
            payload.RequestId, payload.Signer?.Email);

        // TODO: Update document workflow status
        // TODO: Notify document owner

        return Task.CompletedTask;
    }

    private Task HandleSignerSignedAsync(SignatureWebhookPayload payload)
    {
        _logger.LogInformation(
            "Signer {SignerEmail} signed document for request {RequestId}",
            payload.Signer?.Email, payload.RequestId);

        // TODO: Update signer status in database
        // TODO: Check if all signers have completed
        // TODO: Notify next signer if sequential signing

        return Task.CompletedTask;
    }

    private Task HandleSignerViewedAsync(SignatureWebhookPayload payload)
    {
        _logger.LogInformation(
            "Signer {SignerEmail} viewed document for request {RequestId}",
            payload.Signer?.Email, payload.RequestId);

        // TODO: Update signer view timestamp

        return Task.CompletedTask;
    }

    private Task HandleSignatureExpiredAsync(SignatureWebhookPayload payload)
    {
        _logger.LogWarning(
            "Signature request {RequestId} expired for document {DocumentId}",
            payload.RequestId, payload.DocumentId);

        // TODO: Update document status
        // TODO: Notify document owner

        return Task.CompletedTask;
    }
}
