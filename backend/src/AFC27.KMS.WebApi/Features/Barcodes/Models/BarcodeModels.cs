using System;
using System.Collections.Generic;

namespace AFC27.KMS.WebApi.Features.Barcodes.Models;

/// <summary>
/// Request to generate a barcode
/// </summary>
public class GenerateBarcodeRequest
{
    public string Content { get; set; } = string.Empty;
    public BarcodeFormat Format { get; set; } = BarcodeFormat.QRCode;
    public int Width { get; set; } = 300;
    public int Height { get; set; } = 300;
    public string? ForegroundColor { get; set; } = "#000000";
    public string? BackgroundColor { get; set; } = "#FFFFFF";
    public int Margin { get; set; } = 10;
    public BarcodeOutputFormat OutputFormat { get; set; } = BarcodeOutputFormat.PNG;
    public QrCodeOptions? QrOptions { get; set; }
}

public enum BarcodeFormat
{
    QRCode,
    Code128,
    Code39,
    EAN13,
    EAN8,
    UPC_A,
    UPC_E,
    DataMatrix,
    PDF417,
    Aztec
}

public enum BarcodeOutputFormat
{
    PNG,
    SVG,
    JPEG,
    Base64
}

/// <summary>
/// QR Code specific options
/// </summary>
public class QrCodeOptions
{
    public QrCodeErrorCorrection ErrorCorrection { get; set; } = QrCodeErrorCorrection.Medium;
    public bool IncludeLogo { get; set; }
    public byte[]? LogoImage { get; set; }
    public int LogoSizePercent { get; set; } = 20;
}

public enum QrCodeErrorCorrection
{
    Low,      // 7% recovery
    Medium,   // 15% recovery
    Quartile, // 25% recovery
    High      // 30% recovery
}

/// <summary>
/// Response after generating a barcode
/// </summary>
public class GenerateBarcodeResponse
{
    public byte[]? ImageData { get; set; }
    public string? Base64Data { get; set; }
    public string? SvgContent { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public BarcodeFormat Format { get; set; }
    public string EncodedContent { get; set; } = string.Empty;
}

/// <summary>
/// Request to decode a barcode from an image
/// </summary>
public class DecodeBarcodeRequest
{
    public byte[]? ImageData { get; set; }
    public string? ImageUrl { get; set; }
    public string? Base64Image { get; set; }
    public List<BarcodeFormat>? ExpectedFormats { get; set; }
    public bool TryMultiple { get; set; } = true;
}

/// <summary>
/// Result of barcode decoding
/// </summary>
public class DecodeBarcodeResult
{
    public bool Success { get; set; }
    public string? Content { get; set; }
    public BarcodeFormat? DetectedFormat { get; set; }
    public BarcodeLocation? Location { get; set; }
    public double Confidence { get; set; }
    public List<DecodedBarcode>? AllDetected { get; set; }
}

/// <summary>
/// Location of barcode in image
/// </summary>
public class BarcodeLocation
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public double Angle { get; set; }
}

/// <summary>
/// Individual decoded barcode
/// </summary>
public class DecodedBarcode
{
    public string Content { get; set; } = string.Empty;
    public BarcodeFormat Format { get; set; }
    public BarcodeLocation Location { get; set; } = new();
    public double Confidence { get; set; }
}

/// <summary>
/// Request to generate QR code with document link
/// </summary>
public class GenerateDocumentQrRequest
{
    public Guid DocumentId { get; set; }
    public string? AccessToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int Width { get; set; } = 200;
    public int Height { get; set; } = 200;
    public bool IncludeLogo { get; set; }
}

/// <summary>
/// Request to generate accreditation barcode
/// </summary>
public class GenerateAccreditationBarcodeRequest
{
    public string AccreditationNumber { get; set; } = string.Empty;
    public string HolderName { get; set; } = string.Empty;
    public string Organization { get; set; } = string.Empty;
    public string AccreditationType { get; set; } = string.Empty;
    public List<string> AccessZones { get; set; } = new();
    public DateTime ValidFrom { get; set; }
    public DateTime ValidUntil { get; set; }
    public BarcodeFormat Format { get; set; } = BarcodeFormat.QRCode;
    public int Size { get; set; } = 300;
}

/// <summary>
/// Accreditation data encoded in barcode
/// </summary>
public class AccreditationBarcodeData
{
    public string AccreditationNumber { get; set; } = string.Empty;
    public string HolderName { get; set; } = string.Empty;
    public string Organization { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public List<string> Zones { get; set; } = new();
    public long ValidFrom { get; set; }
    public long ValidUntil { get; set; }
    public string Checksum { get; set; } = string.Empty;
}

/// <summary>
/// Barcode tracking for documents
/// </summary>
public class DocumentBarcode
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public string BarcodeContent { get; set; } = string.Empty;
    public BarcodeFormat Format { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public int ScanCount { get; set; }
    public DateTime? LastScannedAt { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Barcode scan log entry
/// </summary>
public class BarcodeScanLog
{
    public Guid Id { get; set; }
    public string BarcodeContent { get; set; } = string.Empty;
    public BarcodeFormat Format { get; set; }
    public DateTime ScannedAt { get; set; }
    public Guid? ScannedBy { get; set; }
    public string? ScannerDevice { get; set; }
    public string? Location { get; set; }
    public bool IsValid { get; set; }
    public string? ValidationMessage { get; set; }
}
