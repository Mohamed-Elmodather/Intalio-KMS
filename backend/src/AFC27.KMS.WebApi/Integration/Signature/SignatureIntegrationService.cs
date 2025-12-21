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
using AFC27.KMS.WebApi.Integration.Signature.Models;

namespace AFC27.KMS.WebApi.Integration.Signature;

/// <summary>
/// Implementation of digital signature integration service
/// </summary>
public class SignatureIntegrationService : ExternalServiceClientBase, ISignatureIntegrationService
{
    private readonly SignatureServiceSettings _settings;

    public override string ServiceName => "DigitalSignature";

    public SignatureIntegrationService(
        HttpClient httpClient,
        ILogger<SignatureIntegrationService> logger,
        IOptions<IntegrationSettings> settings)
        : base(httpClient, logger, settings.Value.DigitalSignature)
    {
        _settings = settings.Value.DigitalSignature;
    }

    public async Task<ServiceResponse<SignatureRequestResponse>> CreateSignatureRequestAsync(
        SignatureRequestModel request,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation(
            "Creating signature request for document {DocumentId} with {SignerCount} signers",
            request.DocumentId, request.Signers.Count);

        // Set callback URL if not provided
        if (string.IsNullOrEmpty(request.CallbackUrl))
        {
            request.CallbackUrl = _settings.CallbackUrl + _settings.WebhookEndpoint;
        }

        return await PostAsync<SignatureRequestModel, SignatureRequestResponse>(
            "signatures/requests",
            request,
            cancellationToken);
    }

    public async Task<ServiceResponse<SignatureRequestResponse>> GetSignatureRequestStatusAsync(
        string requestId,
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<SignatureRequestResponse>(
            $"signatures/requests/{requestId}",
            cancellationToken);
    }

    public async Task<ServiceResponse<SigningUrlResponse>> GetSigningUrlAsync(
        string requestId,
        string signerId,
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<SigningUrlResponse>(
            $"signatures/requests/{requestId}/signers/{signerId}/url",
            cancellationToken);
    }

    public async Task<ServiceResponse<bool>> CancelSignatureRequestAsync(
        string requestId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        var response = await PostAsync<object, object>(
            $"signatures/requests/{requestId}/cancel",
            new { Reason = reason },
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

    public async Task<ServiceResponse<SignatureVerificationResult>> VerifySignaturesAsync(
        SignatureVerificationRequest request,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation(
            "Verifying signatures for document {DocumentId}",
            request.DocumentId);

        return await PostAsync<SignatureVerificationRequest, SignatureVerificationResult>(
            "signatures/verify",
            request,
            cancellationToken);
    }

    public async Task<ServiceResponse<bool>> SendReminderAsync(
        string requestId,
        string? signerId = null,
        CancellationToken cancellationToken = default)
    {
        var endpoint = string.IsNullOrEmpty(signerId)
            ? $"signatures/requests/{requestId}/remind"
            : $"signatures/requests/{requestId}/signers/{signerId}/remind";

        var response = await PostAsync<object, object>(
            endpoint,
            new { },
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

    public async Task<ServiceResponse<byte[]>> DownloadSignedDocumentAsync(
        string requestId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await HttpClient.GetAsync(
                $"signatures/requests/{requestId}/document",
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                return ServiceResponse<byte[]>.Success(content);
            }

            return ServiceResponse<byte[]>.Failure(
                $"Failed to download document: {response.StatusCode}",
                response.StatusCode.ToString(),
                (int)response.StatusCode);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to download signed document for request {RequestId}", requestId);
            return ServiceResponse<byte[]>.Failure(ex.Message, "DOWNLOAD_FAILED");
        }
    }

    public bool ValidateWebhookSignature(SignatureWebhookPayload payload, string secret)
    {
        if (string.IsNullOrEmpty(payload.Signature) || string.IsNullOrEmpty(secret))
            return false;

        var dataToSign = $"{payload.RequestId}:{payload.DocumentId}:{payload.Event}:{payload.Timestamp:O}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
        var computedSignature = Convert.ToBase64String(hash);

        return payload.Signature == computedSignature;
    }
}
