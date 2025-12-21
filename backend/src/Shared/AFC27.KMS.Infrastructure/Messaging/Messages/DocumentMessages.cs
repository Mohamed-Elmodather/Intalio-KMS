namespace AFC27.KMS.Infrastructure.Messaging.Messages;

/// <summary>
/// Message published when a document is uploaded and ready for processing.
/// </summary>
public record DocumentUploadedMessage
{
    /// <summary>
    /// Document ID in the database.
    /// </summary>
    public Guid DocumentId { get; init; }

    /// <summary>
    /// Document library ID.
    /// </summary>
    public Guid LibraryId { get; init; }

    /// <summary>
    /// Original file name.
    /// </summary>
    public string FileName { get; init; } = string.Empty;

    /// <summary>
    /// Storage path where the file is stored.
    /// </summary>
    public string StoragePath { get; init; } = string.Empty;

    /// <summary>
    /// MIME content type.
    /// </summary>
    public string ContentType { get; init; } = string.Empty;

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public long FileSize { get; init; }

    /// <summary>
    /// User who uploaded the document.
    /// </summary>
    public Guid UploadedByUserId { get; init; }

    /// <summary>
    /// Timestamp of upload.
    /// </summary>
    public DateTime UploadedAt { get; init; }
}

/// <summary>
/// Message for thumbnail generation request.
/// </summary>
public record ThumbnailGenerationMessage
{
    /// <summary>
    /// Document or media item ID.
    /// </summary>
    public Guid EntityId { get; init; }

    /// <summary>
    /// Type of entity (Document, MediaItem, etc.).
    /// </summary>
    public string EntityType { get; init; } = string.Empty;

    /// <summary>
    /// Source file storage path.
    /// </summary>
    public string SourcePath { get; init; } = string.Empty;

    /// <summary>
    /// MIME content type of source.
    /// </summary>
    public string ContentType { get; init; } = string.Empty;

    /// <summary>
    /// Requested thumbnail width.
    /// </summary>
    public int Width { get; init; } = 256;

    /// <summary>
    /// Requested thumbnail height.
    /// </summary>
    public int Height { get; init; } = 256;

    /// <summary>
    /// Priority level (0-10, higher is more urgent).
    /// </summary>
    public int Priority { get; init; } = 5;
}

/// <summary>
/// Message for document version creation.
/// </summary>
public record DocumentVersionCreatedMessage
{
    public Guid DocumentId { get; init; }
    public Guid VersionId { get; init; }
    public string Version { get; init; } = string.Empty;
    public string StoragePath { get; init; } = string.Empty;
    public Guid CreatedByUserId { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Message for document deletion (for cleanup tasks).
/// </summary>
public record DocumentDeletedMessage
{
    public Guid DocumentId { get; init; }
    public Guid LibraryId { get; init; }
    public string StoragePath { get; init; } = string.Empty;
    public List<string> VersionPaths { get; init; } = new();
    public Guid DeletedByUserId { get; init; }
    public DateTime DeletedAt { get; init; }
}
