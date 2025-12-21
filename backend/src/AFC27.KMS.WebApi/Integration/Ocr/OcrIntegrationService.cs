using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AFC27.KMS.WebApi.Integration.Configuration;
using AFC27.KMS.WebApi.Integration.Core;
using AFC27.KMS.WebApi.Integration.Ocr.Models;

namespace AFC27.KMS.WebApi.Integration.Ocr;

/// <summary>
/// Implementation of OCR integration service
/// </summary>
public class OcrIntegrationService : ExternalServiceClientBase, IOcrIntegrationService
{
    private readonly OcrServiceSettings _settings;

    public override string ServiceName => "OCR";

    public OcrIntegrationService(
        HttpClient httpClient,
        ILogger<OcrIntegrationService> logger,
        IOptions<IntegrationSettings> settings)
        : base(httpClient, logger, settings.Value.Ocr)
    {
        _settings = settings.Value.Ocr;
    }

    public async Task<ServiceResponse<OcrJobResponse>> SubmitDocumentAsync(
        OcrJobRequest request,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation(
            "Submitting document {DocumentId} for OCR processing",
            request.DocumentId);

        return await PostAsync<OcrJobRequest, OcrJobResponse>(
            "ocr/documents",
            request,
            cancellationToken);
    }

    public async Task<ServiceResponse<OcrStatusResponse>> GetJobStatusAsync(
        string jobId,
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<OcrStatusResponse>(
            $"ocr/documents/{jobId}",
            cancellationToken);
    }

    public async Task<ServiceResponse<OcrResultResponse>> GetJobResultAsync(
        string jobId,
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<OcrResultResponse>(
            $"ocr/documents/{jobId}/result",
            cancellationToken);
    }

    public async Task<ServiceResponse<bool>> CancelJobAsync(
        string jobId,
        CancellationToken cancellationToken = default)
    {
        var response = await DeleteAsync<object>(
            $"ocr/documents/{jobId}",
            cancellationToken);

        return new ServiceResponse<bool>
        {
            IsSuccess = response.IsSuccess,
            Data = response.IsSuccess,
            ErrorMessage = response.ErrorMessage,
            ErrorCode = response.ErrorCode,
            HttpStatusCode = response.HttpStatusCode
        };
    }

    public bool ValidateWebhookSignature(OcrWebhookPayload payload, string secret)
    {
        if (string.IsNullOrEmpty(payload.Signature) || string.IsNullOrEmpty(secret))
            return false;

        var dataToSign = $"{payload.JobId}:{payload.DocumentId}:{payload.Status}:{payload.Timestamp:O}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
        var computedSignature = Convert.ToBase64String(hash);

        return payload.Signature == computedSignature;
    }

    public async Task<ServiceResponse<OcrResultResponse>> ProcessDocumentAsync(
        OcrJobRequest request,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        timeout ??= TimeSpan.FromMinutes(5);
        var startTime = DateTime.UtcNow;

        // Submit the document
        var submitResponse = await SubmitDocumentAsync(request, cancellationToken);
        if (!submitResponse.IsSuccess)
        {
            return ServiceResponse<OcrResultResponse>.Failure(
                submitResponse.ErrorMessage ?? "Failed to submit document",
                submitResponse.ErrorCode);
        }

        var jobId = submitResponse.Data!.JobId;

        // Poll for completion
        while (DateTime.UtcNow - startTime < timeout)
        {
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

            var statusResponse = await GetJobStatusAsync(jobId, cancellationToken);
            if (!statusResponse.IsSuccess)
            {
                Logger.LogWarning("Failed to get OCR job status for {JobId}", jobId);
                continue;
            }

            switch (statusResponse.Data!.Status)
            {
                case OcrJobStatus.Completed:
                    return await GetJobResultAsync(jobId, cancellationToken);

                case OcrJobStatus.Failed:
                    return ServiceResponse<OcrResultResponse>.Failure(
                        statusResponse.Data.ErrorMessage ?? "OCR processing failed",
                        statusResponse.Data.ErrorCode);

                case OcrJobStatus.Cancelled:
                    return ServiceResponse<OcrResultResponse>.Failure(
                        "OCR job was cancelled",
                        "JOB_CANCELLED");
            }
        }

        // Timeout reached
        await CancelJobAsync(jobId, cancellationToken);
        return ServiceResponse<OcrResultResponse>.Failure(
            "OCR processing timed out",
            "TIMEOUT");
    }
}
