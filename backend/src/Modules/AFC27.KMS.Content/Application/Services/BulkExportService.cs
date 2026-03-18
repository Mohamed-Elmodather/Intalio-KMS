using System.IO.Compression;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Content.Application.DTOs;
using AFC27.KMS.Content.Application.Interfaces;
using AFC27.KMS.Content.Domain.Entities;

namespace AFC27.KMS.Content.Application.Services;

/// <summary>
/// Service interface for bulk-exporting multiple articles as a ZIP archive.
/// </summary>
public interface IBulkExportService
{
    /// <summary>
    /// Export multiple articles as a ZIP archive containing individual files.
    /// </summary>
    Task<BulkExportResult> ExportAsync(
        BulkExportRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Exports multiple knowledge articles into a single ZIP archive (Phase 9D).
/// Each article is exported as an individual file (PDF, DOCX, or Markdown)
/// within the ZIP, optionally organized by category folders.
/// </summary>
public class BulkExportService : IBulkExportService
{
    private readonly DbContext _dbContext;
    private readonly IArticleExportService _articleExportService;
    private readonly IBlockEditorService _blockEditorService;
    private readonly ILogger<BulkExportService> _logger;

    public BulkExportService(
        DbContext dbContext,
        IArticleExportService articleExportService,
        IBlockEditorService blockEditorService,
        ILogger<BulkExportService> logger)
    {
        _dbContext = dbContext;
        _articleExportService = articleExportService;
        _blockEditorService = blockEditorService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<BulkExportResult> ExportAsync(
        BulkExportRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Bulk export requested: {Count} articles, format: {Format}, organize by category: {ByCategory}",
            request.ArticleIds.Count, request.Format, request.OrganizeByCategory);

        if (request.ArticleIds.Count == 0)
        {
            return new BulkExportResult
            {
                Success = false,
                ErrorMessage = "No article IDs provided"
            };
        }

        // Load article metadata
        var articles = await _dbContext.Set<Article>()
            .AsNoTracking()
            .Include(a => a.Category)
            .Where(a => request.ArticleIds.Contains(a.Id))
            .ToListAsync(cancellationToken);

        if (articles.Count == 0)
        {
            return new BulkExportResult
            {
                Success = false,
                ErrorMessage = "No articles found matching the provided IDs"
            };
        }

        _logger.LogInformation("Found {Count} articles for bulk export", articles.Count);

        var errors = new List<BulkExportError>();

        using var zipStream = new MemoryStream();
        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var article in articles)
            {
                try
                {
                    var fileBytes = await ExportSingleArticleAsync(
                        article, request.Format, request.Language, cancellationToken);

                    var extension = GetFileExtension(request.Format);
                    var slug = SanitizeFileName(article.Slug ?? article.Id.ToString());

                    var entryPath = request.OrganizeByCategory && article.Category != null
                        ? $"{SanitizeFileName(article.Category.Name.English)}/{slug}.{extension}"
                        : $"{slug}.{extension}";

                    var entry = archive.CreateEntry(entryPath, CompressionLevel.Optimal);
                    using var entryStream = entry.Open();
                    await entryStream.WriteAsync(fileBytes, cancellationToken);

                    _logger.LogDebug("Added {Path} to ZIP ({Size} bytes)", entryPath, fileBytes.Length);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to export article {ArticleId}", article.Id);
                    errors.Add(new BulkExportError
                    {
                        ArticleId = article.Id,
                        Title = article.Title.English,
                        ErrorMessage = ex.Message
                    });
                }
            }

            // Add a manifest file
            var manifest = BuildManifest(articles, request, errors);
            var manifestEntry = archive.CreateEntry("_manifest.json", CompressionLevel.Optimal);
            using var manifestStream = manifestEntry.Open();
            var manifestBytes = Encoding.UTF8.GetBytes(manifest);
            await manifestStream.WriteAsync(manifestBytes, cancellationToken);
        }

        zipStream.Position = 0;
        var zipBytes = zipStream.ToArray();

        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
        var zipFileName = $"kms-export-{timestamp}.zip";

        _logger.LogInformation(
            "Bulk export complete: {SuccessCount}/{Total} articles, ZIP size: {Size} bytes",
            articles.Count - errors.Count, articles.Count, zipBytes.Length);

        return new BulkExportResult
        {
            Success = true,
            FileName = zipFileName,
            ContentType = "application/zip",
            FileContent = zipBytes,
            FileSizeBytes = zipBytes.Length,
            ArticleCount = articles.Count,
            SuccessCount = articles.Count - errors.Count,
            ErrorCount = errors.Count,
            Errors = errors,
            GeneratedAt = DateTime.UtcNow
        };
    }

    private async Task<byte[]> ExportSingleArticleAsync(
        Article article,
        string format,
        string language,
        CancellationToken cancellationToken)
    {
        return format.ToLowerInvariant() switch
        {
            "pdf" => await _articleExportService.GeneratePdfAsync(
                article.Id, language, true, false, false, null, null, cancellationToken),
            "docx" => await _articleExportService.GenerateDocxAsync(
                article.Id, language, true, false, false, null, null, cancellationToken),
            "markdown" or "md" => await ExportAsMarkdownAsync(article, language, cancellationToken),
            _ => await _articleExportService.GeneratePdfAsync(
                article.Id, language, true, false, false, null, null, cancellationToken)
        };
    }

    private async Task<byte[]> ExportAsMarkdownAsync(
        Article article,
        string language,
        CancellationToken cancellationToken)
    {
        var blocks = await _dbContext.Set<ContentBlock>()
            .AsNoTracking()
            .Where(b => b.ArticleId == article.Id && !b.IsDeleted)
            .OrderBy(b => b.Order)
            .ToListAsync(cancellationToken);

        var isArabic = language.Equals("ar", StringComparison.OrdinalIgnoreCase);
        var title = isArabic ? (article.Title.Arabic ?? article.Title.English) : article.Title.English;

        var sb = new StringBuilder();
        sb.AppendLine($"# {title}");
        sb.AppendLine();
        sb.AppendLine($"**Author:** {article.AuthorName}");
        sb.AppendLine($"**Status:** {article.Status}");
        if (article.PublishedAt.HasValue)
            sb.AppendLine($"**Published:** {article.PublishedAt:yyyy-MM-dd}");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();

        foreach (var block in blocks)
        {
            var content = isArabic ? (block.ContentArabic ?? block.Content) : block.Content;

            switch (block.Type)
            {
                case BlockType.Heading:
                    // Parse level from metadata, default to 2
                    var level = 2;
                    if (!string.IsNullOrEmpty(block.Metadata))
                    {
                        var levelMatch = System.Text.RegularExpressions.Regex.Match(block.Metadata, @"""level""\s*:\s*(\d)");
                        if (levelMatch.Success) level = int.Parse(levelMatch.Groups[1].Value);
                    }
                    sb.AppendLine($"{new string('#', level)} {content}");
                    sb.AppendLine();
                    break;

                case BlockType.BulletList:
                    sb.AppendLine($"- {content}");
                    break;

                case BlockType.NumberedList:
                    sb.AppendLine($"1. {content}");
                    break;

                case BlockType.Quote:
                    sb.AppendLine($"> {content}");
                    sb.AppendLine();
                    break;

                case BlockType.Code:
                    var lang = "";
                    if (!string.IsNullOrEmpty(block.Metadata))
                    {
                        var langMatch = System.Text.RegularExpressions.Regex.Match(block.Metadata, @"""language""\s*:\s*""([^""]+)""");
                        if (langMatch.Success) lang = langMatch.Groups[1].Value;
                    }
                    sb.AppendLine($"```{lang}");
                    sb.AppendLine(content);
                    sb.AppendLine("```");
                    sb.AppendLine();
                    break;

                case BlockType.Divider:
                    sb.AppendLine("---");
                    sb.AppendLine();
                    break;

                case BlockType.Image:
                    var url = "";
                    var alt = content;
                    if (!string.IsNullOrEmpty(block.Metadata))
                    {
                        var urlMatch = System.Text.RegularExpressions.Regex.Match(block.Metadata, @"""url""\s*:\s*""([^""]+)""");
                        if (urlMatch.Success) url = urlMatch.Groups[1].Value;
                    }
                    sb.AppendLine($"![{alt}]({url})");
                    sb.AppendLine();
                    break;

                default:
                    sb.AppendLine(content);
                    sb.AppendLine();
                    break;
            }
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static string GetFileExtension(string format) => format.ToLowerInvariant() switch
    {
        "pdf" => "pdf",
        "docx" => "docx",
        "markdown" or "md" => "md",
        _ => "pdf"
    };

    private static string SanitizeFileName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var sanitized = new StringBuilder(name.Length);
        foreach (var c in name)
        {
            sanitized.Append(invalid.Contains(c) ? '_' : c);
        }
        return sanitized.ToString();
    }

    private static string BuildManifest(
        List<Article> articles,
        BulkExportRequest request,
        List<BulkExportError> errors)
    {
        var articlesJson = string.Join(",\n    ",
            articles.Select(a =>
                $"{{\"id\":\"{a.Id}\",\"title\":\"{EscapeJson(a.Title.English)}\",\"slug\":\"{a.Slug}\"}}"));

        var errorsJson = string.Join(",\n    ",
            errors.Select(e =>
                $"{{\"articleId\":\"{e.ArticleId}\",\"title\":\"{EscapeJson(e.Title)}\",\"error\":\"{EscapeJson(e.ErrorMessage)}\"}}"));

        return $$"""
        {
          "exportedAt": "{{DateTime.UtcNow:O}}",
          "format": "{{request.Format}}",
          "language": "{{request.Language}}",
          "totalArticles": {{articles.Count}},
          "successCount": {{articles.Count - errors.Count}},
          "errorCount": {{errors.Count}},
          "articles": [
            {{articlesJson}}
          ],
          "errors": [
            {{errorsJson}}
          ]
        }
        """;
    }

    private static string EscapeJson(string text) =>
        text.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");
}

#region Bulk Export DTOs

/// <summary>
/// Request for bulk-exporting multiple articles.
/// </summary>
public class BulkExportRequest
{
    /// <summary>
    /// List of article IDs to export.
    /// </summary>
    public List<Guid> ArticleIds { get; set; } = new();

    /// <summary>
    /// Export format: "pdf", "docx", "markdown".
    /// </summary>
    public string Format { get; set; } = "pdf";

    /// <summary>
    /// Language: "en" or "ar".
    /// </summary>
    public string Language { get; set; } = "en";

    /// <summary>
    /// Whether to organize exported files into category-named folders.
    /// </summary>
    public bool OrganizeByCategory { get; set; } = true;

    /// <summary>
    /// Whether to include article metadata in each file.
    /// </summary>
    public bool IncludeMetadata { get; set; } = true;
}

/// <summary>
/// Result of a bulk export operation.
/// </summary>
public class BulkExportResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public byte[]? FileContent { get; set; }
    public long FileSizeBytes { get; set; }
    public int ArticleCount { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public List<BulkExportError> Errors { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}

/// <summary>
/// Error encountered during the export of a single article.
/// </summary>
public class BulkExportError
{
    public Guid ArticleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

#endregion
