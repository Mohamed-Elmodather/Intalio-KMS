using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AFC27.KMS.WebApi.Features.Barcodes.Models;

namespace AFC27.KMS.WebApi.Features.Barcodes.Services;

/// <summary>
/// Implementation of barcode generation and decoding service
/// </summary>
public class BarcodeService : IBarcodeService
{
    private readonly ILogger<BarcodeService> _logger;
    private static readonly List<DocumentBarcode> _documentBarcodes = new();
    private static readonly List<BarcodeScanLog> _scanLogs = new();
    private static int _barcodeCounter = 1000;

    public BarcodeService(ILogger<BarcodeService> logger)
    {
        _logger = logger;
    }

    public Task<GenerateBarcodeResponse> GenerateBarcodeAsync(
        GenerateBarcodeRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Generating {Format} barcode with content length {Length}",
            request.Format, request.Content.Length);

        // In production, use a library like ZXing.Net, QRCoder, or SkiaSharp
        // This is a placeholder implementation
        var response = new GenerateBarcodeResponse
        {
            Width = request.Width,
            Height = request.Height,
            Format = request.Format,
            EncodedContent = request.Content,
            ContentType = GetContentType(request.OutputFormat)
        };

        // Generate SVG placeholder for QR codes
        if (request.OutputFormat == BarcodeOutputFormat.SVG)
        {
            response.SvgContent = GeneratePlaceholderSvg(request);
        }
        else if (request.OutputFormat == BarcodeOutputFormat.Base64)
        {
            // Placeholder - in production generate actual image
            response.Base64Data = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"BARCODE:{request.Format}:{request.Content}"));
        }
        else
        {
            // Placeholder byte array
            response.ImageData = Encoding.UTF8.GetBytes(
                $"BARCODE_IMAGE:{request.Format}:{request.Content}");
        }

        return Task.FromResult(response);
    }

    public Task<DecodeBarcodeResult> DecodeBarcodeAsync(
        DecodeBarcodeRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Decoding barcode from image");

        // In production, use ZXing.Net or similar library to decode
        // This is a placeholder implementation
        var result = new DecodeBarcodeResult
        {
            Success = false,
            Content = null,
            DetectedFormat = null
        };

        // Simulate decoding if image data contains our placeholder
        if (request.ImageData != null)
        {
            var content = Encoding.UTF8.GetString(request.ImageData);
            if (content.StartsWith("BARCODE_IMAGE:"))
            {
                var parts = content.Split(':');
                if (parts.Length >= 3)
                {
                    result.Success = true;
                    result.DetectedFormat = Enum.TryParse<BarcodeFormat>(parts[1], out var format)
                        ? format
                        : BarcodeFormat.QRCode;
                    result.Content = string.Join(":", parts.Skip(2));
                    result.Confidence = 0.95;
                }
            }
        }

        return Task.FromResult(result);
    }

    public async Task<GenerateBarcodeResponse> GenerateDocumentQrCodeAsync(
        GenerateDocumentQrRequest request,
        string baseUrl,
        CancellationToken cancellationToken = default)
    {
        // Build the document access URL
        var accessUrl = $"{baseUrl.TrimEnd('/')}/documents/{request.DocumentId}";

        if (!string.IsNullOrEmpty(request.AccessToken))
        {
            accessUrl += $"?token={request.AccessToken}";
        }

        if (request.ExpiresAt.HasValue)
        {
            var expiry = request.ExpiresAt.Value.ToUniversalTime().ToString("O");
            accessUrl += request.AccessToken != null ? "&" : "?";
            accessUrl += $"expires={Uri.EscapeDataString(expiry)}";
        }

        var barcodeRequest = new GenerateBarcodeRequest
        {
            Content = accessUrl,
            Format = BarcodeFormat.QRCode,
            Width = request.Width,
            Height = request.Height,
            OutputFormat = BarcodeOutputFormat.PNG,
            QrOptions = new QrCodeOptions
            {
                ErrorCorrection = QrCodeErrorCorrection.Medium,
                IncludeLogo = request.IncludeLogo
            }
        };

        return await GenerateBarcodeAsync(barcodeRequest, cancellationToken);
    }

    public async Task<GenerateBarcodeResponse> GenerateAccreditationBarcodeAsync(
        GenerateAccreditationBarcodeRequest request,
        CancellationToken cancellationToken = default)
    {
        // Create accreditation data structure
        var data = new AccreditationBarcodeData
        {
            AccreditationNumber = request.AccreditationNumber,
            HolderName = request.HolderName,
            Organization = request.Organization,
            Type = request.AccreditationType,
            Zones = request.AccessZones,
            ValidFrom = new DateTimeOffset(request.ValidFrom).ToUnixTimeSeconds(),
            ValidUntil = new DateTimeOffset(request.ValidUntil).ToUnixTimeSeconds()
        };

        // Generate checksum for validation
        data.Checksum = GenerateChecksum(data);

        // Serialize to JSON
        var jsonContent = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var barcodeRequest = new GenerateBarcodeRequest
        {
            Content = jsonContent,
            Format = request.Format,
            Width = request.Size,
            Height = request.Size,
            OutputFormat = BarcodeOutputFormat.PNG,
            QrOptions = new QrCodeOptions
            {
                ErrorCorrection = QrCodeErrorCorrection.High // Higher error correction for accreditation
            }
        };

        _logger.LogInformation(
            "Generated accreditation barcode for {AccreditationNumber}",
            request.AccreditationNumber);

        return await GenerateBarcodeAsync(barcodeRequest, cancellationToken);
    }

    public Task<(bool IsValid, AccreditationBarcodeData? Data, string Message)> ValidateAccreditationBarcodeAsync(
        string barcodeContent,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var data = JsonSerializer.Deserialize<AccreditationBarcodeData>(
                barcodeContent,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            if (data == null)
            {
                return Task.FromResult((false, (AccreditationBarcodeData?)null, "Invalid barcode data format"));
            }

            // Validate checksum
            var expectedChecksum = GenerateChecksum(data);
            if (data.Checksum != expectedChecksum)
            {
                _logger.LogWarning(
                    "Invalid checksum for accreditation {AccreditationNumber}",
                    data.AccreditationNumber);
                return Task.FromResult((false, data, "Invalid checksum - barcode may be tampered"));
            }

            // Validate expiry
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (now < data.ValidFrom)
            {
                return Task.FromResult((false, data, "Accreditation not yet valid"));
            }

            if (now > data.ValidUntil)
            {
                return Task.FromResult((false, data, "Accreditation has expired"));
            }

            _logger.LogInformation(
                "Valid accreditation barcode scanned: {AccreditationNumber}",
                data.AccreditationNumber);

            return Task.FromResult((true, data, "Valid accreditation"));
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to parse accreditation barcode");
            return Task.FromResult((false, (AccreditationBarcodeData?)null, "Invalid barcode format"));
        }
    }

    public Task LogBarcodeScanAsync(
        string barcodeContent,
        BarcodeFormat format,
        Guid? userId,
        string? device,
        string? location,
        bool isValid,
        string? message,
        CancellationToken cancellationToken = default)
    {
        var log = new BarcodeScanLog
        {
            Id = Guid.NewGuid(),
            BarcodeContent = barcodeContent,
            Format = format,
            ScannedAt = DateTime.UtcNow,
            ScannedBy = userId,
            ScannerDevice = device,
            Location = location,
            IsValid = isValid,
            ValidationMessage = message
        };

        _scanLogs.Add(log);

        // Update document barcode scan count
        var docBarcode = _documentBarcodes.FirstOrDefault(b => b.BarcodeContent == barcodeContent);
        if (docBarcode != null)
        {
            docBarcode.ScanCount++;
            docBarcode.LastScannedAt = DateTime.UtcNow;
        }

        _logger.LogInformation(
            "Logged barcode scan: {Format}, Valid: {IsValid}",
            format, isValid);

        return Task.CompletedTask;
    }

    public Task<(List<BarcodeScanLog> Logs, int TotalCount)> GetScanHistoryAsync(
        string barcodeContent,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _scanLogs.Where(l => l.BarcodeContent == barcodeContent);
        var totalCount = query.Count();

        var logs = query
            .OrderByDescending(l => l.ScannedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult((logs, totalCount));
    }

    public Task<DocumentBarcode> GetOrCreateDocumentBarcodeAsync(
        Guid documentId,
        BarcodeFormat format,
        string purpose,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var existing = _documentBarcodes.FirstOrDefault(b =>
            b.DocumentId == documentId &&
            b.Format == format &&
            b.Purpose == purpose &&
            b.IsActive);

        if (existing != null)
        {
            return Task.FromResult(existing);
        }

        var barcode = new DocumentBarcode
        {
            Id = Guid.NewGuid(),
            DocumentId = documentId,
            BarcodeContent = GenerateUniqueBarcodeId("DOC"),
            Format = format,
            Purpose = purpose,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            ScanCount = 0,
            IsActive = true
        };

        _documentBarcodes.Add(barcode);

        _logger.LogInformation(
            "Created document barcode for {DocumentId}: {BarcodeContent}",
            documentId, barcode.BarcodeContent);

        return Task.FromResult(barcode);
    }

    public string GenerateUniqueBarcodeId(string prefix = "AFC27")
    {
        var counter = Interlocked.Increment(ref _barcodeCounter);
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = new Random().Next(1000, 9999);

        return $"{prefix}-{timestamp}-{counter:D6}-{random}";
    }

    private static string GenerateChecksum(AccreditationBarcodeData data)
    {
        var toHash = $"{data.AccreditationNumber}|{data.HolderName}|{data.ValidFrom}|{data.ValidUntil}";
        var bytes = Encoding.UTF8.GetBytes(toHash);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash)[..16];
    }

    private static string GetContentType(BarcodeOutputFormat format) => format switch
    {
        BarcodeOutputFormat.PNG => "image/png",
        BarcodeOutputFormat.SVG => "image/svg+xml",
        BarcodeOutputFormat.JPEG => "image/jpeg",
        BarcodeOutputFormat.Base64 => "text/plain",
        _ => "application/octet-stream"
    };

    private static string GeneratePlaceholderSvg(GenerateBarcodeRequest request)
    {
        return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{request.Width}"" height=""{request.Height}"" viewBox=""0 0 {request.Width} {request.Height}"">
            <rect width=""100%"" height=""100%"" fill=""{request.BackgroundColor}""/>
            <text x=""50%"" y=""50%"" dominant-baseline=""middle"" text-anchor=""middle"" font-family=""monospace"" font-size=""12"" fill=""{request.ForegroundColor}"">
                {request.Format}: {(request.Content.Length > 20 ? request.Content[..20] + "..." : request.Content)}
            </text>
            <rect x=""{request.Margin}"" y=""{request.Margin}"" width=""{request.Width - 2 * request.Margin}"" height=""{request.Height - 2 * request.Margin}"" fill=""none"" stroke=""{request.ForegroundColor}"" stroke-width=""2""/>
        </svg>";
    }
}
