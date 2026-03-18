namespace AFC27.KMS.Content.Application.DTOs;

/// <summary>
/// Supported export formats for articles.
/// </summary>
public enum ExportFormat
{
    Pdf,
    Docx
}

/// <summary>
/// Request to export an article to a file format.
/// </summary>
public record ExportArticleRequest
{
    /// <summary>
    /// Target export format. Defaults to PDF.
    /// </summary>
    public ExportFormat Format { get; init; } = ExportFormat.Pdf;

    /// <summary>
    /// Language to export. "en" for English, "ar" for Arabic. Defaults to "en".
    /// </summary>
    public string Language { get; init; } = "en";

    /// <summary>
    /// Whether to include article metadata (author, date, tags) in the export.
    /// </summary>
    public bool IncludeMetadata { get; init; } = true;

    /// <summary>
    /// Whether to include inline comments in the export.
    /// </summary>
    public bool IncludeComments { get; init; } = false;

    /// <summary>
    /// Whether to include a table of contents.
    /// </summary>
    public bool IncludeTableOfContents { get; init; } = false;

    /// <summary>
    /// Optional custom header text.
    /// </summary>
    public string? HeaderText { get; init; }

    /// <summary>
    /// Optional custom footer text.
    /// </summary>
    public string? FooterText { get; init; }
}

/// <summary>
/// Result of an article export operation.
/// </summary>
public record ExportResult
{
    /// <summary>
    /// Unique identifier for this export.
    /// </summary>
    public Guid ExportId { get; init; }

    /// <summary>
    /// The article that was exported.
    /// </summary>
    public Guid ArticleId { get; init; }

    /// <summary>
    /// The generated file name.
    /// </summary>
    public string FileName { get; init; } = string.Empty;

    /// <summary>
    /// MIME type of the exported file.
    /// </summary>
    public string ContentType { get; init; } = string.Empty;

    /// <summary>
    /// Size of the exported file in bytes.
    /// </summary>
    public long FileSizeBytes { get; init; }

    /// <summary>
    /// The exported file content as bytes.
    /// </summary>
    public byte[] FileContent { get; init; } = Array.Empty<byte>();

    /// <summary>
    /// When the export was generated.
    /// </summary>
    public DateTime GeneratedAt { get; init; }

    /// <summary>
    /// The format that was exported.
    /// </summary>
    public string Format { get; init; } = string.Empty;
}
