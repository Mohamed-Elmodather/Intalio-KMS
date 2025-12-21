using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.WebApi.Features.Barcodes.Models;
using AFC27.KMS.WebApi.Features.Barcodes.Services;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.WebApi.Features.Barcodes.Controllers;

/// <summary>
/// Controller for barcode and QR code operations
/// </summary>
[ApiController]
[Route("api/barcodes")]
[Authorize]
public class BarcodesController : ControllerBase
{
    private readonly IBarcodeService _barcodeService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<BarcodesController> _logger;

    public BarcodesController(
        IBarcodeService barcodeService,
        ICurrentUser currentUser,
        ILogger<BarcodesController> logger)
    {
        _barcodeService = barcodeService;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <summary>
    /// Generates a barcode or QR code
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult> GenerateBarcode(
        [FromBody] GenerateBarcodeRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Content))
            return BadRequest(new { message = "Content is required" });

        var result = await _barcodeService.GenerateBarcodeAsync(request, cancellationToken);

        if (request.OutputFormat == BarcodeOutputFormat.Base64)
        {
            return Ok(new { base64 = result.Base64Data, contentType = result.ContentType });
        }

        if (request.OutputFormat == BarcodeOutputFormat.SVG)
        {
            return Content(result.SvgContent ?? string.Empty, "image/svg+xml");
        }

        return File(result.ImageData ?? Array.Empty<byte>(), result.ContentType);
    }

    /// <summary>
    /// Decodes a barcode from an uploaded image
    /// </summary>
    [HttpPost("decode")]
    public async Task<ActionResult<DecodeBarcodeResult>> DecodeBarcode(
        IFormFile image,
        [FromQuery] bool tryMultiple = true,
        CancellationToken cancellationToken = default)
    {
        if (image == null || image.Length == 0)
            return BadRequest(new { message = "Image is required" });

        using var memoryStream = new MemoryStream();
        await image.CopyToAsync(memoryStream, cancellationToken);

        var request = new DecodeBarcodeRequest
        {
            ImageData = memoryStream.ToArray(),
            TryMultiple = tryMultiple
        };

        var result = await _barcodeService.DecodeBarcodeAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Generates a QR code for document access
    /// </summary>
    [HttpPost("documents/{documentId:guid}/qr")]
    public async Task<ActionResult> GenerateDocumentQr(
        Guid documentId,
        [FromBody] GenerateDocumentQrOptions? options,
        CancellationToken cancellationToken)
    {
        var request = new GenerateDocumentQrRequest
        {
            DocumentId = documentId,
            Width = options?.Width ?? 200,
            Height = options?.Height ?? 200,
            ExpiresAt = options?.ExpiresAt,
            IncludeLogo = options?.IncludeLogo ?? false
        };

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var result = await _barcodeService.GenerateDocumentQrCodeAsync(
            request,
            baseUrl,
            cancellationToken);

        return File(result.ImageData ?? Array.Empty<byte>(), result.ContentType);
    }

    /// <summary>
    /// Generates an accreditation barcode
    /// </summary>
    [HttpPost("accreditation")]
    [Authorize(Policy = "CanManageUsers")]
    public async Task<ActionResult> GenerateAccreditationBarcode(
        [FromBody] GenerateAccreditationBarcodeRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.AccreditationNumber))
            return BadRequest(new { message = "Accreditation number is required" });

        var result = await _barcodeService.GenerateAccreditationBarcodeAsync(
            request,
            cancellationToken);

        return File(result.ImageData ?? Array.Empty<byte>(), result.ContentType);
    }

    /// <summary>
    /// Validates and scans an accreditation barcode
    /// </summary>
    [HttpPost("accreditation/scan")]
    public async Task<ActionResult<AccreditationScanResponse>> ScanAccreditationBarcode(
        [FromBody] ScanAccreditationRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.BarcodeContent))
            return BadRequest(new { message = "Barcode content is required" });

        var (isValid, data, message) = await _barcodeService.ValidateAccreditationBarcodeAsync(
            request.BarcodeContent,
            cancellationToken);

        // Log the scan
        await _barcodeService.LogBarcodeScanAsync(
            request.BarcodeContent,
            BarcodeFormat.QRCode,
            _currentUser.UserId,
            request.ScannerDevice,
            request.Location,
            isValid,
            message,
            cancellationToken);

        return Ok(new AccreditationScanResponse
        {
            IsValid = isValid,
            Message = message,
            AccreditationNumber = data?.AccreditationNumber,
            HolderName = data?.HolderName,
            Organization = data?.Organization,
            AccreditationType = data?.Type,
            AccessZones = data?.Zones ?? new List<string>(),
            ValidFrom = data != null ? DateTimeOffset.FromUnixTimeSeconds(data.ValidFrom).DateTime : null,
            ValidUntil = data != null ? DateTimeOffset.FromUnixTimeSeconds(data.ValidUntil).DateTime : null
        });
    }

    /// <summary>
    /// Gets scan history for a barcode
    /// </summary>
    [HttpGet("scans/{barcodeContent}")]
    [Authorize(Policy = "CanManageUsers")]
    public async Task<ActionResult<object>> GetScanHistory(
        string barcodeContent,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var (logs, totalCount) = await _barcodeService.GetScanHistoryAsync(
            barcodeContent,
            page,
            pageSize,
            cancellationToken);

        return Ok(new
        {
            data = logs,
            pagination = new
            {
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        });
    }

    /// <summary>
    /// Generates a unique barcode ID
    /// </summary>
    [HttpGet("generate-id")]
    public ActionResult<object> GenerateUniqueId([FromQuery] string? prefix)
    {
        var id = _barcodeService.GenerateUniqueBarcodeId(prefix ?? "AFC27");
        return Ok(new { barcodeId = id });
    }
}

public class GenerateDocumentQrOptions
{
    public int Width { get; set; } = 200;
    public int Height { get; set; } = 200;
    public DateTime? ExpiresAt { get; set; }
    public bool IncludeLogo { get; set; }
}

public class ScanAccreditationRequest
{
    public string BarcodeContent { get; set; } = string.Empty;
    public string? ScannerDevice { get; set; }
    public string? Location { get; set; }
}

public class AccreditationScanResponse
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? AccreditationNumber { get; set; }
    public string? HolderName { get; set; }
    public string? Organization { get; set; }
    public string? AccreditationType { get; set; }
    public List<string> AccessZones { get; set; } = new();
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
}
