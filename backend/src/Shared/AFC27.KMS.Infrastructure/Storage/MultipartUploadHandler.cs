using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AFC27.KMS.Infrastructure.Storage;

/// <summary>
/// Handler for processing multipart file uploads from HTTP requests.
/// Supports chunked uploads for large files up to 5GB.
/// </summary>
public class MultipartUploadHandler
{
    private readonly IStorageService _storageService;
    private readonly LocalStorageOptions _options;
    private readonly ILogger<MultipartUploadHandler> _logger;

    public MultipartUploadHandler(
        IStorageService storageService,
        IOptions<LocalStorageOptions> options,
        ILogger<MultipartUploadHandler> logger)
    {
        _storageService = storageService;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Initiates a new multipart upload session.
    /// </summary>
    public async Task<MultipartUploadInitResponse> InitiateUploadAsync(
        string fileName,
        string contentType,
        long totalSize)
    {
        // Validate file size
        var maxSizeBytes = (long)_options.MaxFileSizeMB * 1024 * 1024;
        if (totalSize > maxSizeBytes)
        {
            throw new InvalidOperationException(
                $"File size {totalSize} exceeds maximum allowed size of {_options.MaxFileSizeMB}MB");
        }

        var uploadId = await _storageService.InitiateMultipartUploadAsync(fileName, contentType);
        var chunkSizeBytes = _options.ChunkSizeMB * 1024 * 1024;
        var totalParts = (int)Math.Ceiling((double)totalSize / chunkSizeBytes);

        _logger.LogInformation(
            "Initiated upload {UploadId} for {FileName}, total size: {Size}, parts: {Parts}",
            uploadId, fileName, totalSize, totalParts);

        return new MultipartUploadInitResponse
        {
            UploadId = uploadId,
            ChunkSize = chunkSizeBytes,
            TotalParts = totalParts
        };
    }

    /// <summary>
    /// Handles upload of a single chunk/part.
    /// </summary>
    public async Task<MultipartUploadPartResponse> UploadPartAsync(
        string uploadId,
        int partNumber,
        Stream content)
    {
        var etag = await _storageService.UploadPartAsync(uploadId, partNumber, content);

        _logger.LogDebug("Uploaded part {PartNumber} for {UploadId}", partNumber, uploadId);

        return new MultipartUploadPartResponse
        {
            UploadId = uploadId,
            PartNumber = partNumber,
            ETag = etag
        };
    }

    /// <summary>
    /// Completes the multipart upload and assembles the file.
    /// </summary>
    public async Task<MultipartUploadCompleteResponse> CompleteUploadAsync(
        string uploadId,
        List<PartETag> parts)
    {
        var orderedETags = parts.OrderBy(p => p.PartNumber).Select(p => p.ETag).ToList();
        var storagePath = await _storageService.CompleteMultipartUploadAsync(uploadId, orderedETags);

        // Get final file info
        var fileInfo = await _storageService.GetFileInfoAsync(storagePath);
        var contentHash = await _storageService.GetContentHashAsync(storagePath);

        _logger.LogInformation(
            "Completed upload {UploadId}, path: {Path}, size: {Size}",
            uploadId, storagePath, fileInfo.Size);

        return new MultipartUploadCompleteResponse
        {
            StoragePath = storagePath,
            FileName = fileInfo.FileName,
            ContentType = fileInfo.ContentType,
            Size = fileInfo.Size,
            ContentHash = contentHash
        };
    }

    /// <summary>
    /// Aborts an in-progress upload.
    /// </summary>
    public async Task AbortUploadAsync(string uploadId)
    {
        await _storageService.AbortMultipartUploadAsync(uploadId);
        _logger.LogInformation("Aborted upload {UploadId}", uploadId);
    }

    /// <summary>
    /// Handles a simple single-request file upload (for smaller files).
    /// </summary>
    public async Task<SimpleUploadResponse> UploadSimpleAsync(
        IFormFile file,
        string? folder = null)
    {
        // Validate file size for simple upload (max 500MB for non-chunked)
        var maxSimpleUploadBytes = 500 * 1024 * 1024L;
        if (file.Length > maxSimpleUploadBytes)
        {
            throw new InvalidOperationException(
                $"File size {file.Length} exceeds maximum for simple upload. Use multipart upload for files over 500MB.");
        }

        using var stream = file.OpenReadStream();
        var storagePath = await _storageService.UploadFileAsync(
            stream,
            file.FileName,
            file.ContentType,
            folder);

        var contentHash = await _storageService.GetContentHashAsync(storagePath);

        _logger.LogInformation(
            "Simple upload completed: {FileName}, path: {Path}, size: {Size}",
            file.FileName, storagePath, file.Length);

        return new SimpleUploadResponse
        {
            StoragePath = storagePath,
            FileName = file.FileName,
            ContentType = file.ContentType,
            Size = file.Length,
            ContentHash = contentHash
        };
    }
}

#region Response DTOs

public class MultipartUploadInitResponse
{
    public string UploadId { get; set; } = string.Empty;
    public int ChunkSize { get; set; }
    public int TotalParts { get; set; }
}

public class MultipartUploadPartResponse
{
    public string UploadId { get; set; } = string.Empty;
    public int PartNumber { get; set; }
    public string ETag { get; set; } = string.Empty;
}

public class MultipartUploadCompleteResponse
{
    public string StoragePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string ContentHash { get; set; } = string.Empty;
}

public class SimpleUploadResponse
{
    public string StoragePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string ContentHash { get; set; } = string.Empty;
}

public class PartETag
{
    public int PartNumber { get; set; }
    public string ETag { get; set; } = string.Empty;
}

#endregion
