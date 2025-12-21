using System;
using System.Threading;
using System.Threading.Tasks;
using AFC27.KMS.WebApi.Features.Barcodes.Models;

namespace AFC27.KMS.WebApi.Features.Barcodes.Services;

/// <summary>
/// Interface for barcode generation and decoding service
/// </summary>
public interface IBarcodeService
{
    /// <summary>
    /// Generates a barcode image
    /// </summary>
    Task<GenerateBarcodeResponse> GenerateBarcodeAsync(
        GenerateBarcodeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Decodes a barcode from an image
    /// </summary>
    Task<DecodeBarcodeResult> DecodeBarcodeAsync(
        DecodeBarcodeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a QR code for document access
    /// </summary>
    Task<GenerateBarcodeResponse> GenerateDocumentQrCodeAsync(
        GenerateDocumentQrRequest request,
        string baseUrl,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates accreditation barcode with encoded data
    /// </summary>
    Task<GenerateBarcodeResponse> GenerateAccreditationBarcodeAsync(
        GenerateAccreditationBarcodeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates and decodes an accreditation barcode
    /// </summary>
    Task<(bool IsValid, AccreditationBarcodeData? Data, string Message)> ValidateAccreditationBarcodeAsync(
        string barcodeContent,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Tracks a barcode scan
    /// </summary>
    Task LogBarcodeScanAsync(
        string barcodeContent,
        BarcodeFormat format,
        Guid? userId,
        string? device,
        string? location,
        bool isValid,
        string? message,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets scan history for a barcode
    /// </summary>
    Task<(List<BarcodeScanLog> Logs, int TotalCount)> GetScanHistoryAsync(
        string barcodeContent,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or retrieves document barcode tracking
    /// </summary>
    Task<DocumentBarcode> GetOrCreateDocumentBarcodeAsync(
        Guid documentId,
        BarcodeFormat format,
        string purpose,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a unique barcode identifier
    /// </summary>
    string GenerateUniqueBarcodeId(string prefix = "AFC27");
}
