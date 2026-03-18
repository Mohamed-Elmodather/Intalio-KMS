using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Content.Domain.Entities;

/// <summary>
/// Article/News/Blog content entity.
/// </summary>
public class Article : AuditableEntity
{
    public LocalizedString Title { get; private set; } = null!;
    public LocalizedString? Summary { get; private set; }
    public LocalizedString Content { get; private set; } = null!;
    public string Slug { get; private set; } = string.Empty;
    public ArticleType Type { get; private set; }
    public ArticleStatus Status { get; private set; }
    public string? FeaturedImageUrl { get; private set; }
    public string? ThumbnailUrl { get; private set; }
    public Guid AuthorId { get; private set; }
    public string AuthorName { get; private set; } = string.Empty;
    public Guid? CategoryId { get; private set; }
    public Guid? SpaceId { get; private set; }
    public bool IsFeatured { get; private set; }
    public bool AllowComments { get; private set; } = true;
    public int ViewCount { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public DateTime? ScheduledPublishAt { get; private set; }
    public int Version { get; private set; } = 1;

    // Navigation properties
    public virtual Category? Category { get; private set; }
    public virtual Space? Space { get; private set; }
    public virtual ICollection<ArticleTag> Tags { get; private set; } = new List<ArticleTag>();
    public virtual ICollection<ArticleVersion> Versions { get; private set; } = new List<ArticleVersion>();

    private Article() { }

    public static Article Create(
        LocalizedString title,
        LocalizedString content,
        ArticleType type,
        Guid authorId,
        string authorName,
        Guid? categoryId = null)
    {
        var article = new Article
        {
            Title = title,
            Content = content,
            Type = type,
            Status = ArticleStatus.Draft,
            AuthorId = authorId,
            AuthorName = authorName,
            CategoryId = categoryId,
            Slug = GenerateSlug(title.English)
        };

        article.AddDomainEvent(new ArticleCreatedEvent(article.Id, title.English, authorId));
        return article;
    }

    public void Update(LocalizedString title, LocalizedString content, LocalizedString? summary)
    {
        Title = title;
        Content = content;
        Summary = summary;
        Slug = GenerateSlug(title.English);
        Version++;

        AddDomainEvent(new ArticleUpdatedEvent(Id, title.English));
    }

    public void SetFeaturedImage(string imageUrl, string? thumbnailUrl = null)
    {
        FeaturedImageUrl = imageUrl;
        ThumbnailUrl = thumbnailUrl;
    }

    public void SetCategory(Guid? categoryId)
    {
        CategoryId = categoryId;
    }

    public void SetSpace(Guid? spaceId)
    {
        SpaceId = spaceId;
    }

    public void SetFeatured(bool isFeatured)
    {
        IsFeatured = isFeatured;
    }

    public void Submit()
    {
        if (Status != ArticleStatus.Draft)
            throw new InvalidOperationException("Only draft articles can be submitted");

        Status = ArticleStatus.PendingReview;
        AddDomainEvent(new ArticleSubmittedEvent(Id, Title.English, AuthorId));
    }

    public void Approve()
    {
        if (Status != ArticleStatus.PendingReview)
            throw new InvalidOperationException("Only pending articles can be approved");

        Status = ArticleStatus.Approved;
    }

    public void Reject(string reason)
    {
        if (Status != ArticleStatus.PendingReview)
            throw new InvalidOperationException("Only pending articles can be rejected");

        Status = ArticleStatus.Rejected;
        AddDomainEvent(new ArticleRejectedEvent(Id, Title.English, AuthorId, reason));
    }

    public void Publish()
    {
        if (Status != ArticleStatus.Approved && Status != ArticleStatus.Draft)
            throw new InvalidOperationException("Only approved or draft articles can be published");

        Status = ArticleStatus.Published;
        PublishedAt = DateTime.UtcNow;
        AddDomainEvent(new ArticlePublishedEvent(Id, Title.English));
    }

    public void SchedulePublish(DateTime scheduledAt)
    {
        if (scheduledAt <= DateTime.UtcNow)
            throw new ArgumentException("Scheduled time must be in the future");

        Status = ArticleStatus.Scheduled;
        ScheduledPublishAt = scheduledAt;
    }

    public void Unpublish()
    {
        Status = ArticleStatus.Draft;
        PublishedAt = null;
    }

    public void Archive()
    {
        Status = ArticleStatus.Archived;
    }

    public void IncrementViewCount()
    {
        ViewCount++;
    }

    private static string GenerateSlug(string title)
    {
        return title
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "");
    }
}

public enum ArticleType
{
    Article,
    News,
    Blog,
    Announcement,
    Page
}

public enum ArticleStatus
{
    Draft,
    PendingReview,
    Approved,
    Rejected,
    Scheduled,
    Published,
    Archived
}

/// <summary>
/// Article version for history tracking.
/// </summary>
public class ArticleVersion : Entity
{
    public Guid ArticleId { get; private set; }
    public int VersionNumber { get; private set; }
    public string TitleEn { get; private set; } = string.Empty;
    public string? TitleAr { get; private set; }
    public string ContentEn { get; private set; } = string.Empty;
    public string? ContentAr { get; private set; }
    public Guid ModifiedById { get; private set; }
    public string ModifiedByName { get; private set; } = string.Empty;
    public DateTime ModifiedAt { get; private set; }
    public string? ChangeNotes { get; private set; }

    public virtual Article Article { get; private set; } = null!;
}

/// <summary>
/// Article-Tag many-to-many relationship.
/// </summary>
public class ArticleTag
{
    public Guid ArticleId { get; set; }
    public Guid TagId { get; set; }

    public virtual Article Article { get; set; } = null!;
    public virtual Tag Tag { get; set; } = null!;
}

// Domain Events
public record ArticleCreatedEvent(Guid ArticleId, string Title, Guid AuthorId) : DomainEvent;
public record ArticleUpdatedEvent(Guid ArticleId, string Title) : DomainEvent;
public record ArticleSubmittedEvent(Guid ArticleId, string Title, Guid AuthorId) : DomainEvent;
public record ArticlePublishedEvent(Guid ArticleId, string Title) : DomainEvent;
public record ArticleRejectedEvent(Guid ArticleId, string Title, Guid AuthorId, string Reason) : DomainEvent;
