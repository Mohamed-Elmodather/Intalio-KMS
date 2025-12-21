using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Media.Domain.Entities;

/// <summary>
/// Individual media item (image, video, audio, etc.).
/// </summary>
public class MediaItem : AuditableEntity
{
    public string FileName { get; private set; } = string.Empty;
    public string OriginalFileName { get; private set; } = string.Empty;
    public LocalizedString? Title { get; private set; }
    public LocalizedString? Description { get; private set; }
    public LocalizedString? AltText { get; private set; }
    public MediaType Type { get; private set; }
    public string MimeType { get; private set; } = string.Empty;
    public string Extension { get; private set; } = string.Empty;
    public long FileSizeBytes { get; private set; }
    public string StoragePath { get; private set; } = string.Empty;
    public string? Url { get; private set; }
    public string? ThumbnailUrl { get; private set; }
    public string? PreviewUrl { get; private set; }
    public Guid GalleryId { get; private set; }
    public MediaGallery Gallery { get; private set; } = null!;
    public Guid UploadedById { get; private set; }
    public MediaStatus Status { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int? DurationSeconds { get; private set; }
    public string? Metadata { get; private set; }
    public int ViewCount { get; private set; }
    public int DownloadCount { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsFeatured { get; private set; }

    private readonly List<MediaThumbnail> _thumbnails = new();
    public IReadOnlyCollection<MediaThumbnail> Thumbnails => _thumbnails.AsReadOnly();

    private readonly List<MediaTag> _tags = new();
    public IReadOnlyCollection<MediaTag> Tags => _tags.AsReadOnly();

    private readonly List<MediaTranscoding> _transcodings = new();
    public IReadOnlyCollection<MediaTranscoding> Transcodings => _transcodings.AsReadOnly();

    private MediaItem() { }

    public static MediaItem Create(
        string fileName,
        string originalFileName,
        MediaType type,
        string mimeType,
        string extension,
        long fileSizeBytes,
        string storagePath,
        Guid galleryId,
        Guid uploadedById,
        int width = 0,
        int height = 0,
        int? durationSeconds = null)
    {
        var item = new MediaItem
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
            OriginalFileName = originalFileName,
            Type = type,
            MimeType = mimeType,
            Extension = extension.TrimStart('.').ToLowerInvariant(),
            FileSizeBytes = fileSizeBytes,
            StoragePath = storagePath,
            GalleryId = galleryId,
            UploadedById = uploadedById,
            Status = MediaStatus.Processing,
            Width = width,
            Height = height,
            DurationSeconds = durationSeconds,
            ViewCount = 0,
            DownloadCount = 0,
            SortOrder = 0,
            IsFeatured = false
        };

        item.AddDomainEvent(new MediaItemUploadedEvent(item.Id, item.Type, item.FileSizeBytes));
        return item;
    }

    public void SetUrls(string url, string? thumbnailUrl = null, string? previewUrl = null)
    {
        Url = url;
        ThumbnailUrl = thumbnailUrl;
        PreviewUrl = previewUrl;
    }

    public void UpdateDetails(LocalizedString? title, LocalizedString? description, LocalizedString? altText)
    {
        Title = title;
        Description = description;
        AltText = altText;
    }

    public void SetDimensions(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public void SetDuration(int durationSeconds)
    {
        DurationSeconds = durationSeconds;
    }

    public void SetMetadata(string metadata)
    {
        Metadata = metadata;
    }

    public void MarkAsReady()
    {
        Status = MediaStatus.Ready;
        AddDomainEvent(new MediaItemReadyEvent(Id, Type));
    }

    public void MarkAsFailed(string? reason = null)
    {
        Status = MediaStatus.Failed;
    }

    public void MarkAsArchived()
    {
        Status = MediaStatus.Archived;
    }

    public void IncrementViewCount()
    {
        ViewCount++;
    }

    public void IncrementDownloadCount()
    {
        DownloadCount++;
    }

    public void SetSortOrder(int order)
    {
        SortOrder = order;
    }

    public void SetFeatured(bool featured)
    {
        IsFeatured = featured;
    }

    public void AddThumbnail(MediaThumbnail thumbnail)
    {
        _thumbnails.Add(thumbnail);
    }

    public void AddTranscoding(MediaTranscoding transcoding)
    {
        _transcodings.Add(transcoding);
    }

    public void AddTag(string tag)
    {
        if (!_tags.Any(t => t.Tag.Equals(tag, StringComparison.OrdinalIgnoreCase)))
        {
            _tags.Add(new MediaTag { MediaItemId = Id, Tag = tag });
        }
    }

    public void RemoveTag(string tag)
    {
        var existingTag = _tags.FirstOrDefault(t => t.Tag.Equals(tag, StringComparison.OrdinalIgnoreCase));
        if (existingTag != null)
        {
            _tags.Remove(existingTag);
        }
    }

    public void ClearTags()
    {
        _tags.Clear();
    }
}

/// <summary>
/// Media tag for categorization.
/// </summary>
public class MediaTag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MediaItemId { get; set; }
    public string Tag { get; set; } = string.Empty;
}

/// <summary>
/// Type of media.
/// </summary>
public enum MediaType
{
    Image,
    Video,
    Audio,
    Document,
    Other
}

/// <summary>
/// Processing status of media.
/// </summary>
public enum MediaStatus
{
    Uploading,
    Processing,
    Ready,
    Failed,
    Archived
}

/// <summary>
/// Domain event for media upload.
/// </summary>
public record MediaItemUploadedEvent(Guid MediaItemId, MediaType Type, long FileSizeBytes) : DomainEvent;

/// <summary>
/// Domain event for media ready.
/// </summary>
public record MediaItemReadyEvent(Guid MediaItemId, MediaType Type) : DomainEvent;
