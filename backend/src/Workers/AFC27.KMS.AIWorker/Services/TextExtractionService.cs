using Microsoft.Extensions.Logging;
using AFC27.KMS.Infrastructure.Storage;

namespace AFC27.KMS.AIWorker.Services;

/// <summary>
/// Service for extracting text content from various document formats.
/// </summary>
public class TextExtractionService
{
    private readonly IStorageService _storageService;
    private readonly ILogger<TextExtractionService> _logger;

    public TextExtractionService(
        IStorageService storageService,
        ILogger<TextExtractionService> logger)
    {
        _storageService = storageService;
        _logger = logger;
    }

    /// <summary>
    /// Extracts text from a document based on its content type.
    /// </summary>
    public async Task<TextExtractionResult> ExtractTextAsync(
        string storagePath,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Extracting text from {Path} with content type {ContentType}",
            storagePath, contentType);

        try
        {
            var text = contentType.ToLowerInvariant() switch
            {
                "text/plain" => await ExtractFromTextFileAsync(storagePath, cancellationToken),
                "application/pdf" => await ExtractFromPdfAsync(storagePath, cancellationToken),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document" =>
                    await ExtractFromDocxAsync(storagePath, cancellationToken),
                "application/msword" =>
                    throw new NotSupportedException("Legacy .doc format not supported. Convert to .docx"),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" =>
                    await ExtractFromXlsxAsync(storagePath, cancellationToken),
                "application/vnd.openxmlformats-officedocument.presentationml.presentation" =>
                    await ExtractFromPptxAsync(storagePath, cancellationToken),
                "text/html" => await ExtractFromHtmlAsync(storagePath, cancellationToken),
                "text/markdown" or "text/x-markdown" =>
                    await ExtractFromTextFileAsync(storagePath, cancellationToken),
                _ => throw new NotSupportedException($"Content type not supported: {contentType}")
            };

            _logger.LogInformation(
                "Extracted {CharCount} characters from {Path}",
                text.Length, storagePath);

            return new TextExtractionResult
            {
                Success = true,
                Text = text,
                CharacterCount = text.Length,
                WordCount = CountWords(text)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract text from {Path}", storagePath);
            return new TextExtractionResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    private async Task<string> ExtractFromTextFileAsync(
        string storagePath,
        CancellationToken cancellationToken)
    {
        await using var stream = await _storageService.GetFileStreamAsync(storagePath);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync(cancellationToken);
    }

    private async Task<string> ExtractFromPdfAsync(
        string storagePath,
        CancellationToken cancellationToken)
    {
        await using var stream = await _storageService.GetFileStreamAsync(storagePath);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        var text = new System.Text.StringBuilder();

        // Using iText7 for PDF text extraction
        using var pdfReader = new iText.Kernel.Pdf.PdfReader(memoryStream);
        using var pdfDocument = new iText.Kernel.Pdf.PdfDocument(pdfReader);

        for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
        {
            var page = pdfDocument.GetPage(i);
            var pageText = iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(page);
            text.AppendLine(pageText);
        }

        return text.ToString();
    }

    private async Task<string> ExtractFromDocxAsync(
        string storagePath,
        CancellationToken cancellationToken)
    {
        await using var stream = await _storageService.GetFileStreamAsync(storagePath);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        var text = new System.Text.StringBuilder();

        using var document = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(memoryStream, false);
        var body = document.MainDocumentPart?.Document?.Body;

        if (body != null)
        {
            foreach (var paragraph in body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
            {
                text.AppendLine(paragraph.InnerText);
            }
        }

        return text.ToString();
    }

    private async Task<string> ExtractFromXlsxAsync(
        string storagePath,
        CancellationToken cancellationToken)
    {
        await using var stream = await _storageService.GetFileStreamAsync(storagePath);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        var text = new System.Text.StringBuilder();

        using var document = DocumentFormat.OpenXml.Packaging.SpreadsheetDocument.Open(memoryStream, false);
        var workbookPart = document.WorkbookPart;
        var sharedStringTable = workbookPart?.SharedStringTablePart?.SharedStringTable;

        if (workbookPart?.WorksheetParts != null)
        {
            foreach (var worksheetPart in workbookPart.WorksheetParts)
            {
                var sheetData = worksheetPart.Worksheet
                    .GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.SheetData>();

                if (sheetData == null) continue;

                foreach (var row in sheetData.Elements<DocumentFormat.OpenXml.Spreadsheet.Row>())
                {
                    var rowText = new List<string>();

                    foreach (var cell in row.Elements<DocumentFormat.OpenXml.Spreadsheet.Cell>())
                    {
                        var cellValue = GetCellValue(cell, sharedStringTable);
                        if (!string.IsNullOrWhiteSpace(cellValue))
                        {
                            rowText.Add(cellValue);
                        }
                    }

                    if (rowText.Any())
                    {
                        text.AppendLine(string.Join("\t", rowText));
                    }
                }
            }
        }

        return text.ToString();
    }

    private async Task<string> ExtractFromPptxAsync(
        string storagePath,
        CancellationToken cancellationToken)
    {
        await using var stream = await _storageService.GetFileStreamAsync(storagePath);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        var text = new System.Text.StringBuilder();

        using var document = DocumentFormat.OpenXml.Packaging.PresentationDocument.Open(memoryStream, false);
        var presentationPart = document.PresentationPart;

        if (presentationPart?.SlideParts != null)
        {
            int slideNumber = 0;
            foreach (var slidePart in presentationPart.SlideParts)
            {
                slideNumber++;
                text.AppendLine($"--- Slide {slideNumber} ---");

                var shapes = slidePart.Slide?.Descendants<DocumentFormat.OpenXml.Drawing.Text>();
                if (shapes != null)
                {
                    foreach (var shape in shapes)
                    {
                        if (!string.IsNullOrWhiteSpace(shape.Text))
                        {
                            text.AppendLine(shape.Text);
                        }
                    }
                }
            }
        }

        return text.ToString();
    }

    private async Task<string> ExtractFromHtmlAsync(
        string storagePath,
        CancellationToken cancellationToken)
    {
        var html = await ExtractFromTextFileAsync(storagePath, cancellationToken);

        // Simple HTML tag removal - in production use HtmlAgilityPack or similar
        var text = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", " ");
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ");
        return System.Net.WebUtility.HtmlDecode(text.Trim());
    }

    private static string GetCellValue(
        DocumentFormat.OpenXml.Spreadsheet.Cell cell,
        DocumentFormat.OpenXml.Spreadsheet.SharedStringTable? sharedStringTable)
    {
        if (cell.CellValue == null)
            return string.Empty;

        var value = cell.CellValue.Text;

        if (cell.DataType?.Value == DocumentFormat.OpenXml.Spreadsheet.CellValues.SharedString
            && sharedStringTable != null
            && int.TryParse(value, out var index))
        {
            return sharedStringTable.ElementAt(index).InnerText;
        }

        return value;
    }

    private static int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        return text.Split(new[] { ' ', '\t', '\n', '\r' },
            StringSplitOptions.RemoveEmptyEntries).Length;
    }
}

/// <summary>
/// Result of text extraction operation.
/// </summary>
public class TextExtractionResult
{
    public bool Success { get; set; }
    public string Text { get; set; } = string.Empty;
    public int CharacterCount { get; set; }
    public int WordCount { get; set; }
    public string? Error { get; set; }
}
