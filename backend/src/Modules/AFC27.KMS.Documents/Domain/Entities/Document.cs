using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Documents.Domain.Entities;

/// <summary>
/// Document entity representing a file in the document library.
/// </summary>
public class Document : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? NameArabic { get; private set; }
    public string? Description { get; private set; }
    public string? DescriptionArabic { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string FileExtension { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public string StoragePath { get; private set; } = string.Empty;
    public string? ThumbnailPath { get; private set; }
    public Guid LibraryId { get; private set; }
    public Guid? FolderId { get; private set; }
    public int MajorVersion { get; private set; } = 1;
    public int MinorVersion { get; private set; } = 0;
    public DocumentStatus Status { get; private set; }
    public Guid? CheckedOutById { get; private set; }
    public string? CheckedOutByName { get; private set; }
    public DateTime? CheckedOutAt { get; private set; }
    public bool RequiresApproval { get; private set; }
    public int DownloadCount { get; private set; }
    public int ViewCount { get; private set; }
    public string? ContentHash { get; private set; }

    // Navigation properties
    public virtual DocumentLibrary Library { get; private set; } = null!;
    public virtual Folder? Folder { get; private set; }
    public virtual ICollection<DocumentVersion> Versions { get; private set; } = new List<DocumentVersion>();
    public virtual ICollection<DocumentMetadata> Metadata { get; private set; } = new List<DocumentMetadata>();
    public virtual ICollection<DocumentAudit> AuditTrail { get; private set; } = new List<DocumentAudit>();

    private Document() { }

    public static Document Create(
        string name,
        string fileName,
        string contentType,
        long fileSize,
        string storagePath,
        Guid libraryId,
        Guid? folderId = null)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        var document = new Document
        {
            Name = name,
            FileName = fileName,
            FileExtension = extension,
            ContentType = contentType,
            FileSize = fileSize,
            StoragePath = storagePath,
            LibraryId = libraryId,
            FolderId = folderId,
            Status = DocumentStatus.Draft
        };

        document.AddDomainEvent(new DocumentCreatedEvent(document.Id, name, libraryId));
        return document;
    }

    public void Update(string name, string? nameArabic, string? description, string? descriptionArabic)
    {
        Name = name;
        NameArabic = nameArabic;
        Description = description;
        DescriptionArabic = descriptionArabic;

        AddDomainEvent(new DocumentUpdatedEvent(Id, name));
    }

    public void CheckOut(Guid userId, string userName)
    {
        if (CheckedOutById.HasValue)
            throw new InvalidOperationException("Document is already checked out");

        CheckedOutById = userId;
        CheckedOutByName = userName;
        CheckedOutAt = DateTime.UtcNow;
        Status = DocumentStatus.CheckedOut;

        AddDomainEvent(new DocumentCheckedOutEvent(Id, userId));
    }

    public void CheckIn(string? changeNotes, bool isMajorVersion)
    {
        if (!CheckedOutById.HasValue)
            throw new InvalidOperationException("Document is not checked out");

        if (isMajorVersion)
        {
            MajorVersion++;
            MinorVersion = 0;
        }
        else
        {
            MinorVersion++;
        }

        var previousCheckOutBy = CheckedOutById.Value;
        CheckedOutById = null;
        CheckedOutByName = null;
        CheckedOutAt = null;
        Status = RequiresApproval ? DocumentStatus.PendingApproval : DocumentStatus.Published;

        AddDomainEvent(new DocumentCheckedInEvent(Id, previousCheckOutBy, $"{MajorVersion}.{MinorVersion}"));
    }

    public void DiscardCheckout()
    {
        if (!CheckedOutById.HasValue)
            throw new InvalidOperationException("Document is not checked out");

        CheckedOutById = null;
        CheckedOutByName = null;
        CheckedOutAt = null;
        Status = DocumentStatus.Published;
    }

    public void Publish()
    {
        Status = DocumentStatus.Published;
        AddDomainEvent(new DocumentPublishedEvent(Id, Name));
    }

    public void Archive()
    {
        Status = DocumentStatus.Archived;
    }

    public void MoveTo(Guid? folderId)
    {
        FolderId = folderId;
    }

    public void SetThumbnail(string thumbnailPath)
    {
        ThumbnailPath = thumbnailPath;
    }

    public void IncrementDownloadCount() => DownloadCount++;
    public void IncrementViewCount() => ViewCount++;

    public string GetVersionString() => $"{MajorVersion}.{MinorVersion}";

    public void Delete()
    {
        Status = DocumentStatus.Archived;
        SoftDelete(Guid.Empty);
    }
}

public enum DocumentStatus
{
    Draft,
    CheckedOut,
    PendingApproval,
    Published,
    Archived
}

/// <summary>
/// Document version for tracking file history.
/// </summary>
public class DocumentVersion : Entity
{
    public Guid DocumentId { get; private set; }
    public int MajorVersion { get; private set; }
    public int MinorVersion { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public string StoragePath { get; private set; } = string.Empty;
    public string? ContentHash { get; private set; }
    public Guid CreatedById { get; private set; }
    public string CreatedByName { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public string? ChangeNotes { get; private set; }

    public virtual Document Document { get; private set; } = null!;

    private DocumentVersion() { }

    public static DocumentVersion Create(
        Guid documentId,
        int majorVersion,
        int minorVersion,
        string fileName,
        long fileSize,
        string storagePath,
        Guid createdById,
        string createdByName,
        string? changeNotes = null)
    {
        return new DocumentVersion
        {
            DocumentId = documentId,
            MajorVersion = majorVersion,
            MinorVersion = minorVersion,
            FileName = fileName,
            FileSize = fileSize,
            StoragePath = storagePath,
            CreatedById = createdById,
            CreatedByName = createdByName,
            CreatedAt = DateTime.UtcNow,
            ChangeNotes = changeNotes
        };
    }

    public string GetVersionString() => $"{MajorVersion}.{MinorVersion}";
}

/// <summary>
/// Document metadata for custom properties.
/// </summary>
public class DocumentMetadata : Entity
{
    public Guid DocumentId { get; private set; }
    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public string? ValueArabic { get; private set; }
    public string DataType { get; private set; } = "String";

    public virtual Document Document { get; private set; } = null!;
}

/// <summary>
/// Document audit trail entry.
/// </summary>
public class DocumentAudit : Entity
{
    public Guid DocumentId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string? Details { get; private set; }
    public Guid PerformedById { get; private set; }
    public string PerformedByName { get; private set; } = string.Empty;
    public DateTime PerformedAt { get; private set; }
    public string? IpAddress { get; private set; }

    public virtual Document Document { get; private set; } = null!;

    private DocumentAudit() { }

    public static DocumentAudit Create(
        Guid documentId,
        string action,
        Guid performedById,
        string performedByName,
        string? details = null,
        string? ipAddress = null)
    {
        return new DocumentAudit
        {
            DocumentId = documentId,
            Action = action,
            Details = details,
            PerformedById = performedById,
            PerformedByName = performedByName,
            PerformedAt = DateTime.UtcNow,
            IpAddress = ipAddress
        };
    }
}

// Domain Events
public record DocumentCreatedEvent(Guid DocumentId, string Name, Guid LibraryId) : DomainEvent;
public record DocumentUpdatedEvent(Guid DocumentId, string Name) : DomainEvent;
public record DocumentCheckedOutEvent(Guid DocumentId, Guid UserId) : DomainEvent;
public record DocumentCheckedInEvent(Guid DocumentId, Guid UserId, string Version) : DomainEvent;
public record DocumentPublishedEvent(Guid DocumentId, string Name) : DomainEvent;
