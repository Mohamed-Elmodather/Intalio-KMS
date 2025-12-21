using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AFC27.KMS.Infrastructure.Storage;

/// <summary>
/// Local file system storage implementation.
/// Supports multipart uploads up to 5GB with chunked transfer.
/// </summary>
public class LocalStorageService : IStorageService
{
    private readonly LocalStorageOptions _options;
    private readonly ILogger<LocalStorageService> _logger;
    private readonly ConcurrentDictionary<string, MultipartUploadSession> _uploadSessions = new();

    public LocalStorageService(
        IOptions<LocalStorageOptions> options,
        ILogger<LocalStorageService> logger)
    {
        _options = options.Value;
        _logger = logger;

        // Ensure base directories exist
        EnsureDirectoryExists(_options.BasePath);
        EnsureDirectoryExists(_options.TempPath);
    }

    public async Task<string> InitiateMultipartUploadAsync(string fileName, string contentType)
    {
        var uploadId = Guid.NewGuid().ToString("N");
        var tempFolder = Path.Combine(_options.TempPath, uploadId);
        EnsureDirectoryExists(tempFolder);

        var session = new MultipartUploadSession
        {
            UploadId = uploadId,
            FileName = fileName,
            ContentType = contentType,
            TempFolder = tempFolder,
            CreatedAt = DateTime.UtcNow
        };

        _uploadSessions[uploadId] = session;

        // Persist session info
        var sessionFile = Path.Combine(tempFolder, "session.json");
        await File.WriteAllTextAsync(sessionFile, JsonSerializer.Serialize(session));

        _logger.LogInformation("Initiated multipart upload {UploadId} for {FileName}", uploadId, fileName);
        return uploadId;
    }

    public async Task<string> UploadPartAsync(string uploadId, int partNumber, Stream content)
    {
        if (!_uploadSessions.TryGetValue(uploadId, out var session))
        {
            // Try to restore from disk
            session = await RestoreSessionAsync(uploadId);
            if (session is null)
                throw new InvalidOperationException($"Upload session {uploadId} not found");
        }

        var partPath = Path.Combine(session.TempFolder, $"part_{partNumber:D5}");

        using var fileStream = new FileStream(partPath, FileMode.Create, FileAccess.Write);
        await content.CopyToAsync(fileStream);
        var size = fileStream.Length;

        // Calculate ETag (MD5 hash of part)
        fileStream.Position = 0;
        using var md5 = MD5.Create();
        var hash = await md5.ComputeHashAsync(new FileStream(partPath, FileMode.Open, FileAccess.Read));
        var etag = Convert.ToBase64String(hash);

        var part = new UploadedPart
        {
            PartNumber = partNumber,
            ETag = etag,
            TempPath = partPath,
            Size = size
        };

        session.Parts.Add(part);

        // Update session file
        var sessionFile = Path.Combine(session.TempFolder, "session.json");
        await File.WriteAllTextAsync(sessionFile, JsonSerializer.Serialize(session));

        _logger.LogDebug("Uploaded part {PartNumber} for {UploadId}, size: {Size}", partNumber, uploadId, size);
        return etag;
    }

    public async Task<string> CompleteMultipartUploadAsync(string uploadId, List<string> partETags)
    {
        if (!_uploadSessions.TryGetValue(uploadId, out var session))
        {
            session = await RestoreSessionAsync(uploadId);
            if (session is null)
                throw new InvalidOperationException($"Upload session {uploadId} not found");
        }

        // Generate final path
        var finalPath = GenerateStoragePath(session.FileName);
        var fullPath = Path.Combine(_options.BasePath, finalPath);
        EnsureDirectoryExists(Path.GetDirectoryName(fullPath)!);

        // Assemble parts in order
        using (var outputStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
        {
            var orderedParts = session.Parts.OrderBy(p => p.PartNumber).ToList();

            foreach (var part in orderedParts)
            {
                if (File.Exists(part.TempPath))
                {
                    using var partStream = new FileStream(part.TempPath, FileMode.Open, FileAccess.Read);
                    await partStream.CopyToAsync(outputStream);
                }
            }
        }

        // Cleanup
        await CleanupUploadSessionAsync(uploadId);

        _logger.LogInformation("Completed multipart upload {UploadId}, final path: {Path}", uploadId, finalPath);
        return finalPath;
    }

    public async Task AbortMultipartUploadAsync(string uploadId)
    {
        await CleanupUploadSessionAsync(uploadId);
        _logger.LogInformation("Aborted multipart upload {UploadId}", uploadId);
    }

    public async Task<string> UploadFileAsync(Stream content, string fileName, string contentType, string? folder = null)
    {
        var storagePath = GenerateStoragePath(fileName, folder);
        var fullPath = Path.Combine(_options.BasePath, storagePath);
        EnsureDirectoryExists(Path.GetDirectoryName(fullPath)!);

        using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        await content.CopyToAsync(fileStream);

        _logger.LogInformation("Uploaded file {FileName} to {Path}", fileName, storagePath);
        return storagePath;
    }

    public Task<Stream> GetFileStreamAsync(string path)
    {
        var fullPath = Path.Combine(_options.BasePath, path);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"File not found: {path}");

        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult(stream);
    }

    public Task<Stream?> DownloadAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_options.BasePath, path);

        if (!File.Exists(fullPath))
            return Task.FromResult<Stream?>(null);

        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult<Stream?>(stream);
    }

    public Task<StorageFileInfo> GetFileInfoAsync(string path)
    {
        var fullPath = Path.Combine(_options.BasePath, path);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"File not found: {path}");

        var fileInfo = new FileInfo(fullPath);

        return Task.FromResult(new StorageFileInfo
        {
            Path = path,
            FileName = fileInfo.Name,
            ContentType = GetContentType(fileInfo.Extension),
            Size = fileInfo.Length,
            CreatedAt = fileInfo.CreationTimeUtc,
            ModifiedAt = fileInfo.LastWriteTimeUtc
        });
    }

    public Task DeleteFileAsync(string path)
    {
        var fullPath = Path.Combine(_options.BasePath, path);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            _logger.LogInformation("Deleted file {Path}", path);
        }

        return Task.CompletedTask;
    }

    public async Task<string> CopyFileAsync(string sourcePath, string destinationPath)
    {
        var sourceFullPath = Path.Combine(_options.BasePath, sourcePath);
        var destFullPath = Path.Combine(_options.BasePath, destinationPath);

        if (!File.Exists(sourceFullPath))
            throw new FileNotFoundException($"Source file not found: {sourcePath}");

        EnsureDirectoryExists(Path.GetDirectoryName(destFullPath)!);

        using var sourceStream = new FileStream(sourceFullPath, FileMode.Open, FileAccess.Read);
        using var destStream = new FileStream(destFullPath, FileMode.Create, FileAccess.Write);
        await sourceStream.CopyToAsync(destStream);

        _logger.LogInformation("Copied file from {Source} to {Destination}", sourcePath, destinationPath);
        return destinationPath;
    }

    public async Task<string> MoveFileAsync(string sourcePath, string destinationPath)
    {
        await CopyFileAsync(sourcePath, destinationPath);
        await DeleteFileAsync(sourcePath);

        _logger.LogInformation("Moved file from {Source} to {Destination}", sourcePath, destinationPath);
        return destinationPath;
    }

    public Task<string> GenerateSecurePreviewUrlAsync(string path, TimeSpan expiry, string? contentDisposition = null)
    {
        // For local storage, generate a token-based URL
        // In production, this would be handled by a preview controller that validates tokens
        var token = GenerateSecureToken(path, expiry);
        var url = $"/api/documents/preview?path={Uri.EscapeDataString(path)}&token={token}";

        if (!string.IsNullOrEmpty(contentDisposition))
        {
            url += $"&disposition={Uri.EscapeDataString(contentDisposition)}";
        }

        return Task.FromResult(url);
    }

    public Task<bool> FileExistsAsync(string path)
    {
        var fullPath = Path.Combine(_options.BasePath, path);
        return Task.FromResult(File.Exists(fullPath));
    }

    public Task<IEnumerable<string>> ListFilesAsync(string folderPath, bool recursive = false)
    {
        var fullPath = Path.Combine(_options.BasePath, folderPath);

        if (!Directory.Exists(fullPath))
            return Task.FromResult(Enumerable.Empty<string>());

        var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var files = Directory.GetFiles(fullPath, "*", searchOption)
            .Select(f => Path.GetRelativePath(_options.BasePath, f).Replace('\\', '/'));

        return Task.FromResult(files);
    }

    public async Task<string> GetContentHashAsync(string path)
    {
        var fullPath = Path.Combine(_options.BasePath, path);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"File not found: {path}");

        using var sha256 = SHA256.Create();
        using var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        var hash = await sha256.ComputeHashAsync(stream);

        return Convert.ToBase64String(hash);
    }

    private string GenerateStoragePath(string fileName, string? folder = null)
    {
        var date = DateTime.UtcNow;
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var safeFileName = SanitizeFileName(fileName);

        var basePath = folder ?? $"{date:yyyy}/{date:MM}/{date:dd}";
        return $"{basePath}/{uniqueId}_{safeFileName}";
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Join("_", fileName.Split(invalid, StringSplitOptions.RemoveEmptyEntries));
    }

    private static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    private async Task<MultipartUploadSession?> RestoreSessionAsync(string uploadId)
    {
        var tempFolder = Path.Combine(_options.TempPath, uploadId);
        var sessionFile = Path.Combine(tempFolder, "session.json");

        if (!File.Exists(sessionFile))
            return null;

        var json = await File.ReadAllTextAsync(sessionFile);
        var session = JsonSerializer.Deserialize<MultipartUploadSession>(json);

        if (session is not null)
        {
            _uploadSessions[uploadId] = session;
        }

        return session;
    }

    private async Task CleanupUploadSessionAsync(string uploadId)
    {
        _uploadSessions.TryRemove(uploadId, out _);

        var tempFolder = Path.Combine(_options.TempPath, uploadId);
        if (Directory.Exists(tempFolder))
        {
            await Task.Run(() => Directory.Delete(tempFolder, recursive: true));
        }
    }

    private string GenerateSecureToken(string path, TimeSpan expiry)
    {
        var expiresAt = DateTime.UtcNow.Add(expiry).Ticks;
        var data = $"{path}|{expiresAt}|{_options.TokenSecret}";

        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
        return $"{expiresAt}_{Convert.ToBase64String(hash)}";
    }

    private static string GetContentType(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".txt" => "text/plain",
            ".csv" => "text/csv",
            ".json" => "application/json",
            ".xml" => "application/xml",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            ".webp" => "image/webp",
            ".mp4" => "video/mp4",
            ".webm" => "video/webm",
            ".mp3" => "audio/mpeg",
            ".wav" => "audio/wav",
            ".zip" => "application/zip",
            ".rar" => "application/vnd.rar",
            _ => "application/octet-stream"
        };
    }
}

/// <summary>
/// Configuration options for local storage.
/// </summary>
public class LocalStorageOptions
{
    public const string SectionName = "Storage:Local";

    /// <summary>
    /// Base path for file storage.
    /// </summary>
    public string BasePath { get; set; } = "/storage/documents";

    /// <summary>
    /// Path for temporary upload files.
    /// </summary>
    public string TempPath { get; set; } = "/storage/temp";

    /// <summary>
    /// Maximum file size in MB (default 5GB).
    /// </summary>
    public int MaxFileSizeMB { get; set; } = 5120;

    /// <summary>
    /// Chunk size for multipart uploads in MB.
    /// </summary>
    public int ChunkSizeMB { get; set; } = 5;

    /// <summary>
    /// Secret key for generating secure preview tokens.
    /// </summary>
    public string TokenSecret { get; set; } = "default-secret-change-in-production";
}
