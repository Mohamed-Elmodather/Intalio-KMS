using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AFC27.KMS.Infrastructure.Storage;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

namespace AFC27.KMS.MediaWorker.Services;

/// <summary>
/// Service for image processing operations using ImageSharp.
/// </summary>
public class ImageProcessingService
{
    private readonly IStorageService _storageService;
    private readonly LocalStorageOptions _options;
    private readonly ILogger<ImageProcessingService> _logger;

    public ImageProcessingService(
        IStorageService storageService,
        IOptions<LocalStorageOptions> options,
        ILogger<ImageProcessingService> logger)
    {
        _storageService = storageService;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Generates a thumbnail for an image file.
    /// </summary>
    public async Task<string> GenerateThumbnailAsync(
        string sourcePath,
        int width,
        int height,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Generating thumbnail for {Source}, size: {Width}x{Height}",
            sourcePath, width, height);

        await using var sourceStream = await _storageService.GetFileStreamAsync(sourcePath);
        using var image = await Image.LoadAsync(sourceStream, cancellationToken);

        // Calculate dimensions maintaining aspect ratio
        var (targetWidth, targetHeight) = CalculateAspectRatioDimensions(
            image.Width, image.Height, width, height);

        // Resize the image
        image.Mutate(x => x.Resize(targetWidth, targetHeight));

        // Generate thumbnail path
        var thumbnailPath = GenerateThumbnailPath(sourcePath, width, height);

        // Save thumbnail
        using var outputStream = new MemoryStream();
        await image.SaveAsync(outputStream, new JpegEncoder { Quality = 85 }, cancellationToken);
        outputStream.Position = 0;

        var storagePath = await _storageService.UploadFileAsync(
            outputStream,
            Path.GetFileName(thumbnailPath),
            "image/jpeg",
            "thumbnails");

        _logger.LogInformation("Thumbnail generated: {Path}", storagePath);
        return storagePath;
    }

    /// <summary>
    /// Generates a thumbnail for a PDF document (first page).
    /// </summary>
    public async Task<string> GeneratePdfThumbnailAsync(
        string sourcePath,
        int width,
        int height,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating PDF thumbnail for {Source}", sourcePath);

        // For PDF thumbnails, we would typically use a library like PDFium or call an external tool
        // For now, return a placeholder path indicating this needs implementation
        // In production, consider using PdfiumViewer, Docnet, or ImageMagick

        _logger.LogWarning("PDF thumbnail generation not fully implemented, returning placeholder");

        // Return a default document thumbnail path
        return "thumbnails/default-pdf.jpg";
    }

    /// <summary>
    /// Resizes an image to specified dimensions.
    /// </summary>
    public async Task<string> ResizeImageAsync(
        string sourcePath,
        int maxWidth,
        int maxHeight,
        CancellationToken cancellationToken = default)
    {
        await using var sourceStream = await _storageService.GetFileStreamAsync(sourcePath);
        using var image = await Image.LoadAsync(sourceStream, cancellationToken);

        if (image.Width <= maxWidth && image.Height <= maxHeight)
        {
            // No resize needed
            return sourcePath;
        }

        var (targetWidth, targetHeight) = CalculateAspectRatioDimensions(
            image.Width, image.Height, maxWidth, maxHeight);

        image.Mutate(x => x.Resize(targetWidth, targetHeight));

        using var outputStream = new MemoryStream();
        var extension = Path.GetExtension(sourcePath).ToLowerInvariant();

        if (extension == ".png")
        {
            await image.SaveAsync(outputStream, new PngEncoder(), cancellationToken);
        }
        else
        {
            await image.SaveAsync(outputStream, new JpegEncoder { Quality = 90 }, cancellationToken);
        }

        outputStream.Position = 0;

        var resizedPath = await _storageService.UploadFileAsync(
            outputStream,
            $"resized_{Path.GetFileName(sourcePath)}",
            extension == ".png" ? "image/png" : "image/jpeg",
            "processed");

        _logger.LogInformation("Image resized: {Path}", resizedPath);
        return resizedPath;
    }

    /// <summary>
    /// Generates multiple thumbnail sizes for responsive display.
    /// </summary>
    public async Task<Dictionary<string, string>> GenerateResponsiveThumbnailsAsync(
        string sourcePath,
        CancellationToken cancellationToken = default)
    {
        var sizes = new Dictionary<string, (int Width, int Height)>
        {
            ["small"] = (128, 128),
            ["medium"] = (256, 256),
            ["large"] = (512, 512)
        };

        var results = new Dictionary<string, string>();

        foreach (var (name, size) in sizes)
        {
            try
            {
                var path = await GenerateThumbnailAsync(
                    sourcePath, size.Width, size.Height, cancellationToken);
                results[name] = path;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate {Size} thumbnail for {Path}", name, sourcePath);
            }
        }

        return results;
    }

    private static (int Width, int Height) CalculateAspectRatioDimensions(
        int sourceWidth,
        int sourceHeight,
        int maxWidth,
        int maxHeight)
    {
        var ratioX = (double)maxWidth / sourceWidth;
        var ratioY = (double)maxHeight / sourceHeight;
        var ratio = Math.Min(ratioX, ratioY);

        return (
            Width: (int)(sourceWidth * ratio),
            Height: (int)(sourceHeight * ratio)
        );
    }

    private static string GenerateThumbnailPath(string sourcePath, int width, int height)
    {
        var fileName = Path.GetFileNameWithoutExtension(sourcePath);
        return $"thumbnails/{fileName}_{width}x{height}.jpg";
    }
}
