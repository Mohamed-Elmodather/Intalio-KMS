using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Content.Domain.Entities;

/// <summary>
/// Reusable content template with predefined structure and blocks.
/// Supports categorization, visibility control, and review cycles.
/// </summary>
public class ContentTemplate : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? NameArabic { get; private set; }
    public string? Description { get; private set; }
    public string? DescriptionArabic { get; private set; }

    /// <summary>
    /// JSON-serialized template structure defining the blocks and layout.
    /// </summary>
    public string Structure { get; private set; } = "{}";

    /// <summary>
    /// Template category for organization (e.g., "Meeting Notes", "Project Report", "SOP").
    /// </summary>
    public string Category { get; private set; } = "General";

    /// <summary>
    /// Whether this template is visible to all users or only the creator.
    /// </summary>
    public bool IsPublic { get; private set; }

    /// <summary>
    /// Whether this template is a system-provided default.
    /// </summary>
    public bool IsSystem { get; private set; }

    /// <summary>
    /// The user who created this template.
    /// </summary>
    public Guid CreatorId { get; private set; }
    public string CreatorName { get; private set; } = string.Empty;

    /// <summary>
    /// Optional thumbnail/preview image URL.
    /// </summary>
    public string? ThumbnailUrl { get; private set; }

    /// <summary>
    /// Number of times this template has been used to create articles.
    /// </summary>
    public int UsageCount { get; private set; }

    /// <summary>
    /// Tags for discoverability.
    /// </summary>
    public string? Tags { get; private set; }

    // Phase 3E: Template Review Cycles
    /// <summary>
    /// How often this template should be reviewed (in days). 0 means no scheduled review.
    /// </summary>
    public int ReviewIntervalDays { get; private set; }

    /// <summary>
    /// When this template was last reviewed for accuracy/relevance.
    /// </summary>
    public DateTime? LastReviewedAt { get; private set; }

    /// <summary>
    /// When the next review is due.
    /// </summary>
    public DateTime? NextReviewDue { get; private set; }

    /// <summary>
    /// Who last reviewed the template.
    /// </summary>
    public Guid? LastReviewedById { get; private set; }
    public string? LastReviewedByName { get; private set; }

    /// <summary>
    /// Current review status.
    /// </summary>
    public TemplateReviewStatus ReviewStatus { get; private set; } = TemplateReviewStatus.NotScheduled;

    private ContentTemplate() { }

    public static ContentTemplate Create(
        string name,
        string structure,
        string category,
        bool isPublic,
        Guid creatorId,
        string creatorName,
        string? nameArabic = null,
        string? description = null,
        string? descriptionArabic = null,
        string? thumbnailUrl = null,
        string? tags = null,
        int reviewIntervalDays = 0)
    {
        var template = new ContentTemplate
        {
            Name = name,
            NameArabic = nameArabic,
            Description = description,
            DescriptionArabic = descriptionArabic,
            Structure = structure,
            Category = category,
            IsPublic = isPublic,
            CreatorId = creatorId,
            CreatorName = creatorName,
            ThumbnailUrl = thumbnailUrl,
            Tags = tags,
            ReviewIntervalDays = reviewIntervalDays
        };

        if (reviewIntervalDays > 0)
        {
            template.ReviewStatus = TemplateReviewStatus.Current;
            template.NextReviewDue = DateTime.UtcNow.AddDays(reviewIntervalDays);
        }

        template.AddDomainEvent(new ContentTemplateCreatedEvent(template.Id, name, creatorId));
        return template;
    }

    public void Update(
        string name,
        string? nameArabic,
        string? description,
        string? descriptionArabic,
        string structure,
        string category,
        bool isPublic,
        string? thumbnailUrl,
        string? tags)
    {
        Name = name;
        NameArabic = nameArabic;
        Description = description;
        DescriptionArabic = descriptionArabic;
        Structure = structure;
        Category = category;
        IsPublic = isPublic;
        ThumbnailUrl = thumbnailUrl;
        Tags = tags;

        AddDomainEvent(new ContentTemplateUpdatedEvent(Id, name));
    }

    public void SetPublic(bool isPublic)
    {
        IsPublic = isPublic;
    }

    public void IncrementUsageCount()
    {
        UsageCount++;
    }

    // Phase 3E: Review cycle methods

    public void SetReviewInterval(int intervalDays)
    {
        ReviewIntervalDays = intervalDays;

        if (intervalDays > 0)
        {
            if (ReviewStatus == TemplateReviewStatus.NotScheduled)
                ReviewStatus = TemplateReviewStatus.Current;

            NextReviewDue = (LastReviewedAt ?? DateTime.UtcNow).AddDays(intervalDays);
        }
        else
        {
            ReviewStatus = TemplateReviewStatus.NotScheduled;
            NextReviewDue = null;
        }
    }

    public void MarkReviewed(Guid reviewedById, string reviewedByName, int? newIntervalDays = null)
    {
        LastReviewedAt = DateTime.UtcNow;
        LastReviewedById = reviewedById;
        LastReviewedByName = reviewedByName;
        ReviewStatus = TemplateReviewStatus.Current;

        if (newIntervalDays.HasValue)
            ReviewIntervalDays = newIntervalDays.Value;

        if (ReviewIntervalDays > 0)
            NextReviewDue = DateTime.UtcNow.AddDays(ReviewIntervalDays);
        else
            NextReviewDue = null;

        AddDomainEvent(new ContentTemplateReviewedEvent(Id, reviewedById, reviewedByName));
    }

    public void MarkReviewDueSoon()
    {
        if (ReviewStatus == TemplateReviewStatus.Current)
        {
            ReviewStatus = TemplateReviewStatus.DueSoon;
            AddDomainEvent(new ContentTemplateReviewDueEvent(Id, CreatorId, NextReviewDue));
        }
    }

    public void MarkReviewOverdue()
    {
        if (ReviewStatus != TemplateReviewStatus.Overdue)
        {
            ReviewStatus = TemplateReviewStatus.Overdue;
            AddDomainEvent(new ContentTemplateReviewOverdueEvent(Id, CreatorId, NextReviewDue));
        }
    }
}

/// <summary>
/// Review status for content templates.
/// </summary>
public enum TemplateReviewStatus
{
    /// <summary>No scheduled review.</summary>
    NotScheduled,

    /// <summary>Template is current and up to date.</summary>
    Current,

    /// <summary>Review is due soon (within threshold).</summary>
    DueSoon,

    /// <summary>Review is overdue.</summary>
    Overdue
}

// Domain Events
public record ContentTemplateCreatedEvent(Guid TemplateId, string Name, Guid CreatorId) : DomainEvent;
public record ContentTemplateUpdatedEvent(Guid TemplateId, string Name) : DomainEvent;
public record ContentTemplateReviewedEvent(Guid TemplateId, Guid ReviewedById, string ReviewedByName) : DomainEvent;
public record ContentTemplateReviewDueEvent(Guid TemplateId, Guid CreatorId, DateTime? NextReviewDue) : DomainEvent;
public record ContentTemplateReviewOverdueEvent(Guid TemplateId, Guid CreatorId, DateTime? NextReviewDue) : DomainEvent;
