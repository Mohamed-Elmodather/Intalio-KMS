namespace AFC27.KMS.Infrastructure.Storage;

/// <summary>
/// Interface for file storage operations supporting local and cloud storage providers.
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Initiates a multipart upload session for large files.
    /// </summary>
    /// <param name="fileName">Original file name</param>
    /// <param name="contentType">MIME type of the file</param>
    /// <returns>Upload session ID</returns>
    Task<string> InitiateMultipartUploadAsync(string fileName, string contentType);

    /// <summary>
    /// Uploads a single part of a multipart upload.
    /// </summary>
    /// <param name="uploadId">Upload session ID</param>
    /// <param name="partNumber">Part number (1-based)</param>
    /// <param name="content">Part content stream</param>
    /// <returns>ETag for the uploaded part</returns>
    Task<string> UploadPartAsync(string uploadId, int partNumber, Stream content);

    /// <summary>
    /// Completes a multipart upload by assembling all parts.
    /// </summary>
    /// <param name="uploadId">Upload session ID</param>
    /// <param name="partETags">List of ETags for all uploaded parts</param>
    /// <returns>Final storage path of the assembled file</returns>
    Task<string> CompleteMultipartUploadAsync(string uploadId, List<string> partETags);

    /// <summary>
    /// Aborts a multipart upload and cleans up uploaded parts.
    /// </summary>
    /// <param name="uploadId">Upload session ID</param>
    Task AbortMultipartUploadAsync(string uploadId);

    /// <summary>
    /// Uploads a file directly (for smaller files).
    /// </summary>
    /// <param name="content">File content stream</param>
    /// <param name="fileName">Original file name</param>
    /// <param name="contentType">MIME type</param>
    /// <param name="folder">Optional folder path</param>
    /// <returns>Storage path of the uploaded file</returns>
    Task<string> UploadFileAsync(Stream content, string fileName, string contentType, string? folder = null);

    /// <summary>
    /// Gets a read stream for a stored file.
    /// </summary>
    /// <param name="path">Storage path</param>
    /// <returns>File content stream</returns>
    Task<Stream> GetFileStreamAsync(string path);

    /// <summary>
    /// Gets file information without downloading the content.
    /// </summary>
    /// <param name="path">Storage path</param>
    /// <returns>File metadata</returns>
    Task<StorageFileInfo> GetFileInfoAsync(string path);

    /// <summary>
    /// Deletes a file from storage.
    /// </summary>
    /// <param name="path">Storage path</param>
    Task DeleteFileAsync(string path);

    /// <summary>
    /// Copies a file to a new location.
    /// </summary>
    /// <param name="sourcePath">Source storage path</param>
    /// <param name="destinationPath">Destination storage path</param>
    /// <returns>New storage path</returns>
    Task<string> CopyFileAsync(string sourcePath, string destinationPath);

    /// <summary>
    /// Moves a file to a new location.
    /// </summary>
    /// <param name="sourcePath">Source storage path</param>
    /// <param name="destinationPath">Destination storage path</param>
    /// <returns>New storage path</returns>
    Task<string> MoveFileAsync(string sourcePath, string destinationPath);

    /// <summary>
    /// Generates a secure, time-limited URL for file preview/download.
    /// </summary>
    /// <param name="path">Storage path</param>
    /// <param name="expiry">URL expiration time</param>
    /// <param name="contentDisposition">Optional content disposition header value</param>
    /// <returns>Secure URL</returns>
    Task<string> GenerateSecurePreviewUrlAsync(string path, TimeSpan expiry, string? contentDisposition = null);

    /// <summary>
    /// Checks if a file exists at the given path.
    /// </summary>
    /// <param name="path">Storage path</param>
    /// <returns>True if file exists</returns>
    Task<bool> FileExistsAsync(string path);

    /// <summary>
    /// Lists files in a folder.
    /// </summary>
    /// <param name="folderPath">Folder path</param>
    /// <param name="recursive">Include subfolders</param>
    /// <returns>List of file paths</returns>
    Task<IEnumerable<string>> ListFilesAsync(string folderPath, bool recursive = false);

    /// <summary>
    /// Calculates the content hash (SHA256) of a file.
    /// </summary>
    /// <param name="path">Storage path</param>
    /// <returns>Base64-encoded SHA256 hash</returns>
    Task<string> GetContentHashAsync(string path);

    /// <summary>
    /// Downloads a file from storage.
    /// </summary>
    /// <param name="path">Storage path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File stream or null if not found</returns>
    Task<Stream?> DownloadAsync(string path, CancellationToken cancellationToken = default);
}

/// <summary>
/// File metadata information from storage.
/// </summary>
public class StorageFileInfo
{
    public string Path { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string? ContentHash { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// Multipart upload session information.
/// </summary>
public class MultipartUploadSession
{
    public string UploadId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string TempFolder { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<UploadedPart> Parts { get; set; } = new();
}

/// <summary>
/// Information about an uploaded part.
/// </summary>
public class UploadedPart
{
    public int PartNumber { get; set; }
    public string ETag { get; set; } = string.Empty;
    public string TempPath { get; set; } = string.Empty;
    public long Size { get; set; }
}
