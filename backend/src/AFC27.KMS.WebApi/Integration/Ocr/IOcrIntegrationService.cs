using System;
using System.Threading;
using System.Threading.Tasks;
using AFC27.KMS.WebApi.Integration.Core;
using AFC27.KMS.WebApi.Integration.Ocr.Models;

namespace AFC27.KMS.WebApi.Integration.Ocr;

/// <summary>
/// Interface for OCR integration service
/// </summary>
public interface IOcrIntegrationService : IExternalServiceClient
{
    /// <summary>
    /// Submits a document for OCR processing
    /// </summary>
    Task<ServiceResponse<OcrJobResponse>> SubmitDocumentAsync(
        OcrJobRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the status of an OCR job
    /// </summary>
    Task<ServiceResponse<OcrStatusResponse>> GetJobStatusAsync(
        string jobId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the result of a completed OCR job
    /// </summary>
    Task<ServiceResponse<OcrResultResponse>> GetJobResultAsync(
        string jobId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels an OCR job
    /// </summary>
    Task<ServiceResponse<bool>> CancelJobAsync(
        string jobId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates the webhook payload signature
    /// </summary>
    bool ValidateWebhookSignature(OcrWebhookPayload payload, string secret);

    /// <summary>
    /// Processes a document synchronously (waits for completion)
    /// </summary>
    Task<ServiceResponse<OcrResultResponse>> ProcessDocumentAsync(
        OcrJobRequest request,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);
}
