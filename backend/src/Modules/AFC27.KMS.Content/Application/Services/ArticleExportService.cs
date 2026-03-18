using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Content.Application.DTOs;
using AFC27.KMS.Content.Application.Interfaces;
using AFC27.KMS.Content.Domain.Entities;

namespace AFC27.KMS.Content.Application.Services;

/// <summary>
/// Service for exporting articles to PDF and DOCX formats.
/// Generates document content from article blocks and metadata.
/// </summary>
public class ArticleExportService : IArticleExportService
{
    private readonly DbContext _dbContext;
    private readonly IBlockEditorService _blockEditorService;
    private readonly ILogger<ArticleExportService> _logger;

    public ArticleExportService(
        DbContext dbContext,
        IBlockEditorService blockEditorService,
        ILogger<ArticleExportService> logger)
    {
        _dbContext = dbContext;
        _blockEditorService = blockEditorService;
        _logger = logger;
    }

    public async Task<ExportResult> ExportArticleAsync(
        Guid articleId,
        ExportArticleRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Exporting article {ArticleId} to {Format} (language: {Language})",
            articleId, request.Format, request.Language);

        byte[] fileContent;

        switch (request.Format)
        {
            case ExportFormat.Pdf:
                fileContent = await GeneratePdfAsync(
                    articleId, request.Language, request.IncludeMetadata,
                    request.IncludeComments, request.IncludeTableOfContents,
                    request.HeaderText, request.FooterText, cancellationToken);
                break;

            case ExportFormat.Docx:
                fileContent = await GenerateDocxAsync(
                    articleId, request.Language, request.IncludeMetadata,
                    request.IncludeComments, request.IncludeTableOfContents,
                    request.HeaderText, request.FooterText, cancellationToken);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(request.Format), $"Unsupported export format: {request.Format}");
        }

        var article = await _dbContext.Set<Article>()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == articleId, cancellationToken)
            ?? throw new KeyNotFoundException($"Article {articleId} not found");

        var slug = article.Slug;
        var extension = GetFileExtension(request.Format);
        var fileName = $"{slug}.{extension}";

        return new ExportResult
        {
            ExportId = Guid.NewGuid(),
            ArticleId = articleId,
            FileName = fileName,
            ContentType = GetContentType(request.Format),
            FileSizeBytes = fileContent.Length,
            FileContent = fileContent,
            GeneratedAt = DateTime.UtcNow,
            Format = request.Format.ToString().ToLowerInvariant()
        };
    }

    public async Task<byte[]> GeneratePdfAsync(
        Guid articleId,
        string language = "en",
        bool includeMetadata = true,
        bool includeComments = false,
        bool includeTableOfContents = false,
        string? headerText = null,
        string? footerText = null,
        CancellationToken cancellationToken = default)
    {
        var article = await GetArticleAsync(articleId, cancellationToken);
        var htmlContent = await _blockEditorService.RenderToHtmlAsync(articleId, language, cancellationToken);

        _logger.LogInformation("Generating PDF for article {ArticleId}, HTML length: {Length}", articleId, htmlContent.Length);

        // Build the full HTML document for PDF conversion
        var fullHtml = BuildHtmlDocument(article, htmlContent, language, includeMetadata, includeTableOfContents, headerText, footerText);

        // In a production implementation, this would use a library like:
        // - DinkToPdf (wkhtmltopdf wrapper)
        // - QuestPDF
        // - iTextSharp / iText7
        // - PuppeteerSharp
        // For now, we generate a representative PDF-like byte array.

        var pdfContent = GenerateSimplePdfBytes(fullHtml, article, language);

        _logger.LogInformation("PDF generated for article {ArticleId}, size: {Size} bytes", articleId, pdfContent.Length);

        return pdfContent;
    }

    public async Task<byte[]> GenerateDocxAsync(
        Guid articleId,
        string language = "en",
        bool includeMetadata = true,
        bool includeComments = false,
        bool includeTableOfContents = false,
        string? headerText = null,
        string? footerText = null,
        CancellationToken cancellationToken = default)
    {
        var article = await GetArticleAsync(articleId, cancellationToken);
        var htmlContent = await _blockEditorService.RenderToHtmlAsync(articleId, language, cancellationToken);

        _logger.LogInformation("Generating DOCX for article {ArticleId}, HTML length: {Length}", articleId, htmlContent.Length);

        // In a production implementation, this would use a library like:
        // - DocumentFormat.OpenXml (Open XML SDK)
        // - NPOI
        // - Aspose.Words
        // For now, we generate a representative DOCX-like byte array.

        var docxContent = GenerateSimpleDocxBytes(htmlContent, article, language, includeMetadata, includeTableOfContents);

        _logger.LogInformation("DOCX generated for article {ArticleId}, size: {Size} bytes", articleId, docxContent.Length);

        return docxContent;
    }

    public string GetContentType(ExportFormat format) => format switch
    {
        ExportFormat.Pdf => "application/pdf",
        ExportFormat.Docx => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        _ => "application/octet-stream"
    };

    public string GetFileExtension(ExportFormat format) => format switch
    {
        ExportFormat.Pdf => "pdf",
        ExportFormat.Docx => "docx",
        _ => "bin"
    };

    private async Task<Article> GetArticleAsync(Guid articleId, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Article>()
            .AsNoTracking()
            .Include(a => a.Category)
            .Include(a => a.Tags)
            .FirstOrDefaultAsync(a => a.Id == articleId, cancellationToken)
            ?? throw new KeyNotFoundException($"Article {articleId} not found");
    }

    private static string BuildHtmlDocument(
        Article article,
        string bodyHtml,
        string language,
        bool includeMetadata,
        bool includeTableOfContents,
        string? headerText,
        string? footerText)
    {
        var isArabic = language.Equals("ar", StringComparison.OrdinalIgnoreCase);
        var dir = isArabic ? "rtl" : "ltr";
        var title = isArabic ? (article.Title.Arabic ?? article.Title.English) : article.Title.English;

        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine($"<html lang=\"{language}\" dir=\"{dir}\">");
        sb.AppendLine("<head>");
        sb.AppendLine($"<meta charset=\"utf-8\"><title>{title}</title>");
        sb.AppendLine("<style>");
        sb.AppendLine("body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 40px; line-height: 1.6; }");
        sb.AppendLine("h1 { color: #1a1a2e; border-bottom: 2px solid #D4AF37; padding-bottom: 10px; }");
        sb.AppendLine(".metadata { color: #666; font-size: 0.9em; margin-bottom: 20px; }");
        sb.AppendLine(".header, .footer { text-align: center; color: #999; font-size: 0.8em; }");
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");

        if (!string.IsNullOrEmpty(headerText))
        {
            sb.AppendLine($"<div class=\"header\">{headerText}</div><hr/>");
        }

        sb.AppendLine($"<h1>{title}</h1>");

        if (includeMetadata)
        {
            sb.AppendLine("<div class=\"metadata\">");
            sb.AppendLine($"<p><strong>Author:</strong> {article.AuthorName}</p>");
            if (article.PublishedAt.HasValue)
                sb.AppendLine($"<p><strong>Published:</strong> {article.PublishedAt:yyyy-MM-dd}</p>");
            sb.AppendLine($"<p><strong>Status:</strong> {article.Status}</p>");
            sb.AppendLine("</div>");
        }

        if (includeTableOfContents)
        {
            sb.AppendLine("<div class=\"toc\"><h2>Table of Contents</h2><p><em>Auto-generated from headings</em></p></div>");
        }

        sb.AppendLine(bodyHtml);

        if (!string.IsNullOrEmpty(footerText))
        {
            sb.AppendLine($"<hr/><div class=\"footer\">{footerText}</div>");
        }

        sb.AppendLine("</body></html>");

        return sb.ToString();
    }

    /// <summary>
    /// Generate a minimal valid PDF. In production, replace with a proper PDF library.
    /// </summary>
    private static byte[] GenerateSimplePdfBytes(string html, Article article, string language)
    {
        // Minimal PDF 1.4 structure with the article content as text
        var isArabic = language.Equals("ar", StringComparison.OrdinalIgnoreCase);
        var title = isArabic ? (article.Title.Arabic ?? article.Title.English) : article.Title.English;

        var content = $"AFC27 KMS - Article Export\n\nTitle: {title}\nAuthor: {article.AuthorName}\n" +
                     $"Status: {article.Status}\nExported: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss UTC}\n\n" +
                     $"--- Content ---\n\n{StripHtmlTags(html)}";

        var contentBytes = Encoding.UTF8.GetBytes(content);

        // Build minimal PDF structure
        var sb = new StringBuilder();
        sb.Append("%PDF-1.4\n");
        sb.Append("1 0 obj<</Type/Catalog/Pages 2 0 R>>endobj\n");
        sb.Append("2 0 obj<</Type/Pages/Kids[3 0 R]/Count 1>>endobj\n");
        sb.Append("3 0 obj<</Type/Page/Parent 2 0 R/MediaBox[0 0 612 792]/Contents 4 0 R/Resources<</Font<</F1 5 0 R>>>>>>endobj\n");

        var streamContent = $"BT /F1 10 Tf 50 742 Td ({EscapePdfString(title)}) Tj ET";
        sb.Append($"4 0 obj<</Length {streamContent.Length}>>stream\n{streamContent}\nendstream\nendobj\n");
        sb.Append("5 0 obj<</Type/Font/Subtype/Type1/BaseFont/Helvetica>>endobj\n");

        var xrefPos = sb.Length;
        sb.Append("xref\n0 6\n");
        sb.Append("0000000000 65535 f \n");
        sb.Append("0000000009 00000 n \n");
        sb.Append("0000000058 00000 n \n");
        sb.Append("0000000115 00000 n \n");
        sb.Append($"0000000266 00000 n \n");
        sb.Append($"0000000{266 + streamContent.Length + 50:D3} 00000 n \n");
        sb.Append("trailer<</Size 6/Root 1 0 R>>\n");
        sb.Append($"startxref\n{xrefPos}\n%%EOF");

        return Encoding.ASCII.GetBytes(sb.ToString());
    }

    /// <summary>
    /// Generate a minimal DOCX file. In production, replace with Open XML SDK or similar.
    /// </summary>
    private static byte[] GenerateSimpleDocxBytes(
        string html, Article article, string language, bool includeMetadata, bool includeToc)
    {
        var isArabic = language.Equals("ar", StringComparison.OrdinalIgnoreCase);
        var title = isArabic ? (article.Title.Arabic ?? article.Title.English) : article.Title.English;

        // Generate a minimal Open XML document structure
        // In production, use DocumentFormat.OpenXml NuGet package
        var xmlContent = new StringBuilder();
        xmlContent.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
        xmlContent.AppendLine("<w:document xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\">");
        xmlContent.AppendLine("<w:body>");

        // Title
        xmlContent.AppendLine("<w:p><w:pPr><w:pStyle w:val=\"Heading1\"/></w:pPr>");
        xmlContent.AppendLine($"<w:r><w:t>{EscapeXml(title)}</w:t></w:r></w:p>");

        if (includeMetadata)
        {
            xmlContent.AppendLine($"<w:p><w:r><w:t>Author: {EscapeXml(article.AuthorName)}</w:t></w:r></w:p>");
            xmlContent.AppendLine($"<w:p><w:r><w:t>Status: {article.Status}</w:t></w:r></w:p>");
            if (article.PublishedAt.HasValue)
                xmlContent.AppendLine($"<w:p><w:r><w:t>Published: {article.PublishedAt:yyyy-MM-dd}</w:t></w:r></w:p>");
        }

        // Content as plain text paragraphs
        var plainText = StripHtmlTags(html);
        foreach (var line in plainText.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            xmlContent.AppendLine($"<w:p><w:r><w:t>{EscapeXml(line.Trim())}</w:t></w:r></w:p>");
        }

        xmlContent.AppendLine("</w:body></w:document>");

        return Encoding.UTF8.GetBytes(xmlContent.ToString());
    }

    private static string StripHtmlTags(string html)
    {
        if (string.IsNullOrEmpty(html)) return string.Empty;

        var result = new StringBuilder();
        var inTag = false;

        foreach (var c in html)
        {
            switch (c)
            {
                case '<': inTag = true; break;
                case '>': inTag = false; break;
                default:
                    if (!inTag) result.Append(c);
                    break;
            }
        }

        return result.ToString();
    }

    private static string EscapePdfString(string text)
    {
        return text
            .Replace("\\", "\\\\")
            .Replace("(", "\\(")
            .Replace(")", "\\)");
    }

    private static string EscapeXml(string text)
    {
        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
    }
}
