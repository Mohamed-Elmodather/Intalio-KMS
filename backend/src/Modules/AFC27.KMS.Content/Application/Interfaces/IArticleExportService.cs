using AFC27.KMS.Content.Application.DTOs;

namespace AFC27.KMS.Content.Application.Interfaces;

/// <summary>
/// Service interface for exporting articles to PDF/DOCX formats.
/// </summary>
public interface IArticleExportService
{
    /// <summary>
    /// Export an article to the specified format.
    /// </summary>
    /// <param name="articleId">The article to export.</param>
    /// <param name="request">Export configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The export result containing the file content.</returns>
    Task<ExportResult> ExportArticleAsync(
        Guid articleId,
        ExportArticleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate a PDF from article content.
    /// </summary>
    Task<byte[]> GeneratePdfAsync(
        Guid articleId,
        string language = "en",
        bool includeMetadata = true,
        bool includeComments = false,
        bool includeTableOfContents = false,
        string? headerText = null,
        string? footerText = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate a DOCX from article content.
    /// </summary>
    Task<byte[]> GenerateDocxAsync(
        Guid articleId,
        string language = "en",
        bool includeMetadata = true,
        bool includeComments = false,
        bool includeTableOfContents = false,
        string? headerText = null,
        string? footerText = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the MIME type for a given export format.
    /// </summary>
    string GetContentType(ExportFormat format);

    /// <summary>
    /// Get the file extension for a given export format.
    /// </summary>
    string GetFileExtension(ExportFormat format);
}
