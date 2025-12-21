using System;
using System.Threading;
using System.Threading.Tasks;
using AFC27.KMS.WebApi.Integration.Core;
using AFC27.KMS.WebApi.Integration.Signature.Models;

namespace AFC27.KMS.WebApi.Integration.Signature;

/// <summary>
/// Interface for digital signature integration service
/// </summary>
public interface ISignatureIntegrationService : IExternalServiceClient
{
    /// <summary>
    /// Creates a new signature request
    /// </summary>
    Task<ServiceResponse<SignatureRequestResponse>> CreateSignatureRequestAsync(
        SignatureRequestModel request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the status of a signature request
    /// </summary>
    Task<ServiceResponse<SignatureRequestResponse>> GetSignatureRequestStatusAsync(
        string requestId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the signing URL for a specific signer
    /// </summary>
    Task<ServiceResponse<SigningUrlResponse>> GetSigningUrlAsync(
        string requestId,
        string signerId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels a signature request
    /// </summary>
    Task<ServiceResponse<bool>> CancelSignatureRequestAsync(
        string requestId,
        string reason,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies signatures on a document
    /// </summary>
    Task<ServiceResponse<SignatureVerificationResult>> VerifySignaturesAsync(
        SignatureVerificationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a reminder to pending signers
    /// </summary>
    Task<ServiceResponse<bool>> SendReminderAsync(
        string requestId,
        string? signerId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads the signed document
    /// </summary>
    Task<ServiceResponse<byte[]>> DownloadSignedDocumentAsync(
        string requestId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates webhook signature
    /// </summary>
    bool ValidateWebhookSignature(SignatureWebhookPayload payload, string secret);
}
