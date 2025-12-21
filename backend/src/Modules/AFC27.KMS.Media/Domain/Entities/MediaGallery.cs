using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Media.Domain.Entities;

/// <summary>
/// Media gallery for organizing media items.
/// </summary>
public class MediaGallery : AuditableEntity
{
    public LocalizedString Name { get; private set; } = null!;
    public LocalizedString? Description { get; private set; }
    public GalleryType Type { get; private set; }
    public GalleryVisibility Visibility { get; private set; }
    public string? CoverImageUrl { get; private set; }
    public Guid? ParentId { get; private set; }
    public int ItemCount { get; private set; }
    public long TotalSizeBytes { get; private set; }
    public Guid OwnerId { get; private set; }
    public Guid? CommunityId { get; private set; }
    public Guid? ProjectId { get; private set; }
    public bool IsArchived { get; private set; }

    private readonly List<MediaItem> _items = new();
    public IReadOnlyCollection<MediaItem> Items => _items.AsReadOnly();

    private readonly List<GalleryTag> _tags = new();
    public IReadOnlyCollection<GalleryTag> Tags => _tags.AsReadOnly();

    private MediaGallery() { }

    public static MediaGallery Create(
        LocalizedString name,
        GalleryType type,
        GalleryVisibility visibility,
        Guid ownerId,
        LocalizedString? description = null,
        Guid? parentId = null,
        Guid? communityId = null,
        Guid? projectId = null)
    {
        var gallery = new MediaGallery
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Type = type,
            Visibility = visibility,
            OwnerId = ownerId,
            ParentId = parentId,
            CommunityId = communityId,
            ProjectId = projectId,
            ItemCount = 0,
            TotalSizeBytes = 0,
            IsArchived = false
        };

        gallery.AddDomainEvent(new GalleryCreatedEvent(gallery.Id, name.English));
        return gallery;
    }

    public void Update(LocalizedString name, LocalizedString? description, GalleryVisibility visibility)
    {
        Name = name;
        Description = description;
        Visibility = visibility;
    }

    public void SetCoverImage(string? coverImageUrl)
    {
        CoverImageUrl = coverImageUrl;
    }

    public void AddItem(MediaItem item)
    {
        _items.Add(item);
        ItemCount++;
        TotalSizeBytes += item.FileSizeBytes;
    }

    public void RemoveItem(MediaItem item)
    {
        if (_items.Remove(item))
        {
            ItemCount--;
            TotalSizeBytes -= item.FileSizeBytes;
        }
    }

    public void Archive()
    {
        IsArchived = true;
    }

    public void Restore()
    {
        IsArchived = false;
    }

    public void AddTag(string tag)
    {
        if (!_tags.Any(t => t.Tag.Equals(tag, StringComparison.OrdinalIgnoreCase)))
        {
            _tags.Add(new GalleryTag { GalleryId = Id, Tag = tag });
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
}

/// <summary>
/// Gallery tag for categorization.
/// </summary>
public class GalleryTag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid GalleryId { get; set; }
    public string Tag { get; set; } = string.Empty;
}

/// <summary>
/// Type of media gallery.
/// </summary>
public enum GalleryType
{
    General,
    Photos,
    Videos,
    Documents,
    EventMedia,
    ProjectMedia,
    TeamMedia
}

/// <summary>
/// Gallery visibility settings.
/// </summary>
public enum GalleryVisibility
{
    Private,
    Team,
    Department,
    Organization,
    Public
}

/// <summary>
/// Domain event for gallery creation.
/// </summary>
public record GalleryCreatedEvent(Guid GalleryId, string GalleryName) : DomainEvent;
