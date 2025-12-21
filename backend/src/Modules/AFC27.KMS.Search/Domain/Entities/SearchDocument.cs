using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Search.Domain.Entities;

/// <summary>
/// Represents a searchable document indexed in Elasticsearch
/// </summary>
public class SearchDocument : Entity
{
    public string IndexName { get; private set; } = string.Empty;
    public SearchableContentType ContentType { get; private set; }
    public Guid SourceId { get; private set; }
    public string SourceType { get; private set; } = string.Empty;

    // Searchable content
    public string TitleEn { get; private set; } = string.Empty;
    public string TitleAr { get; private set; } = string.Empty;
    public string ContentEn { get; private set; } = string.Empty;
    public string ContentAr { get; private set; } = string.Empty;
    public string SummaryEn { get; private set; } = string.Empty;
    public string SummaryAr { get; private set; } = string.Empty;

    // Metadata for filtering
    public string? Category { get; private set; }
    public List<string> Tags { get; private set; } = new();
    public string? Author { get; private set; }
    public Guid? AuthorId { get; private set; }
    public string? Department { get; private set; }
    public Guid? DepartmentId { get; private set; }

    // Document metadata
    public string? FileType { get; private set; }
    public long? FileSize { get; private set; }
    public string? MimeType { get; private set; }

    // Status and visibility
    public DocumentVisibility Visibility { get; private set; } = DocumentVisibility.Internal;
    public bool IsPublished { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public DateTime IndexedAt { get; private set; }
    public DateTime? LastUpdatedAt { get; private set; }

    // Search analytics
    public long ViewCount { get; private set; }
    public long SearchHitCount { get; private set; }
    public double RelevanceBoost { get; private set; } = 1.0;

    // Permission context
    public List<Guid> AllowedRoleIds { get; private set; } = new();
    public List<Guid> AllowedGroupIds { get; private set; } = new();
    public List<Guid> AllowedUserIds { get; private set; } = new();

    // Full-text extracted content (for documents)
    public string? ExtractedText { get; private set; }
    public bool IsTextExtracted { get; private set; }
    public DateTime? TextExtractedAt { get; private set; }

    private SearchDocument() { }

    public static SearchDocument Create(
        string indexName,
        SearchableContentType contentType,
        Guid sourceId,
        string sourceType,
        string titleEn,
        string titleAr)
    {
        return new SearchDocument
        {
            Id = Guid.NewGuid(),
            IndexName = indexName,
            ContentType = contentType,
            SourceId = sourceId,
            SourceType = sourceType,
            TitleEn = titleEn,
            TitleAr = titleAr,
            IndexedAt = DateTime.UtcNow
        };
    }

    public void UpdateContent(string contentEn, string contentAr, string summaryEn, string summaryAr)
    {
        ContentEn = contentEn;
        ContentAr = contentAr;
        SummaryEn = summaryEn;
        SummaryAr = summaryAr;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void SetMetadata(string? category, List<string> tags, string? author, Guid? authorId)
    {
        Category = category;
        Tags = tags;
        Author = author;
        AuthorId = authorId;
    }

    public void SetDocumentInfo(string? fileType, long? fileSize, string? mimeType)
    {
        FileType = fileType;
        FileSize = fileSize;
        MimeType = mimeType;
    }

    public void SetExtractedText(string extractedText)
    {
        ExtractedText = extractedText;
        IsTextExtracted = true;
        TextExtractedAt = DateTime.UtcNow;
    }

    public void SetPermissions(List<Guid> roleIds, List<Guid> groupIds, List<Guid> userIds)
    {
        AllowedRoleIds = roleIds;
        AllowedGroupIds = groupIds;
        AllowedUserIds = userIds;
    }

    public void Publish()
    {
        IsPublished = true;
        PublishedAt = DateTime.UtcNow;
    }

    public void Unpublish()
    {
        IsPublished = false;
    }

    public void IncrementViewCount()
    {
        ViewCount++;
    }

    public void IncrementSearchHitCount()
    {
        SearchHitCount++;
    }

    public void SetRelevanceBoost(double boost)
    {
        RelevanceBoost = Math.Max(0.1, Math.Min(10.0, boost));
    }
}

public enum SearchableContentType
{
    Article,
    News,
    Blog,
    Announcement,
    Page,
    Document,
    MediaItem,
    Discussion,
    Comment,
    LessonLearned,
    User,
    Community,
    Event,
    Service
}

public enum DocumentVisibility
{
    Public,
    Internal,
    Restricted,
    Private
}

/// <summary>
/// Domain event raised when a document is indexed
/// </summary>
public record DocumentIndexedEvent(
    Guid DocumentId,
    SearchableContentType ContentType,
    Guid SourceId,
    string IndexName) : DomainEvent;

/// <summary>
/// Domain event raised when a document is removed from index
/// </summary>
public record DocumentDeindexedEvent(
    Guid DocumentId,
    SearchableContentType ContentType,
    Guid SourceId) : DomainEvent;
