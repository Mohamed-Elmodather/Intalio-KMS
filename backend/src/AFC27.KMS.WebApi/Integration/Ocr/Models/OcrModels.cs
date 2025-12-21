using System;
using System.Collections.Generic;

namespace AFC27.KMS.WebApi.Integration.Ocr.Models;

/// <summary>
/// Request to submit a document for OCR processing
/// </summary>
public class OcrJobRequest
{
    public Guid DocumentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public byte[]? FileContent { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Language { get; set; } = "auto";
    public OcrOptions Options { get; set; } = new();
    public string CallbackUrl { get; set; } = string.Empty;
}

/// <summary>
/// OCR processing options
/// </summary>
public class OcrOptions
{
    public bool ExtractTables { get; set; } = true;
    public bool ExtractImages { get; set; } = false;
    public bool PreserveFormatting { get; set; } = true;
    public bool DetectLanguage { get; set; } = true;
    public int DPI { get; set; } = 300;
}

/// <summary>
/// Response when submitting an OCR job
/// </summary>
public class OcrJobResponse
{
    public string JobId { get; set; } = string.Empty;
    public OcrJobStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? EstimatedCompletionAt { get; set; }
    public string? Message { get; set; }
}

/// <summary>
/// OCR job status
/// </summary>
public enum OcrJobStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Cancelled
}

/// <summary>
/// Response when getting OCR job status
/// </summary>
public class OcrStatusResponse
{
    public string JobId { get; set; } = string.Empty;
    public OcrJobStatus Status { get; set; }
    public int ProgressPercentage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
}

/// <summary>
/// OCR processing result
/// </summary>
public class OcrResultResponse
{
    public string JobId { get; set; } = string.Empty;
    public string ExtractedText { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string DetectedLanguage { get; set; } = string.Empty;
    public int PageCount { get; set; }
    public int WordCount { get; set; }
    public int CharacterCount { get; set; }
    public List<OcrPage> Pages { get; set; } = new();
    public OcrMetadata Metadata { get; set; } = new();
    public DateTime ProcessedAt { get; set; }
    public TimeSpan ProcessingDuration { get; set; }
}

/// <summary>
/// OCR result for a single page
/// </summary>
public class OcrPage
{
    public int PageNumber { get; set; }
    public string Text { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public List<OcrTable>? Tables { get; set; }
    public List<OcrTextBlock>? TextBlocks { get; set; }
}

/// <summary>
/// Extracted table from OCR
/// </summary>
public class OcrTable
{
    public int RowCount { get; set; }
    public int ColumnCount { get; set; }
    public List<List<string>> Cells { get; set; } = new();
    public OcrBoundingBox BoundingBox { get; set; } = new();
}

/// <summary>
/// Text block with position information
/// </summary>
public class OcrTextBlock
{
    public string Text { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public OcrBoundingBox BoundingBox { get; set; } = new();
}

/// <summary>
/// Bounding box coordinates
/// </summary>
public class OcrBoundingBox
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

/// <summary>
/// Additional metadata extracted during OCR
/// </summary>
public class OcrMetadata
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public Dictionary<string, string> CustomFields { get; set; } = new();
}

/// <summary>
/// Webhook payload for OCR completion
/// </summary>
public class OcrWebhookPayload
{
    public string JobId { get; set; } = string.Empty;
    public string DocumentId { get; set; } = string.Empty;
    public OcrJobStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime Timestamp { get; set; }
    public string Signature { get; set; } = string.Empty;
}
