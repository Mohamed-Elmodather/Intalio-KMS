using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Content.Domain.Entities;

/// <summary>
/// Content category for organizing articles.
/// </summary>
public class Category : AuditableEntity
{
    public LocalizedString Name { get; private set; } = null!;
    public LocalizedString? Description { get; private set; }
    public string Slug { get; private set; } = string.Empty;
    public string? IconName { get; private set; }
    public string? Color { get; private set; }
    public Guid? ParentId { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    public virtual Category? Parent { get; private set; }
    public virtual ICollection<Category> Children { get; private set; } = new List<Category>();
    public virtual ICollection<Article> Articles { get; private set; } = new List<Article>();

    private Category() { }

    public static Category Create(LocalizedString name, LocalizedString? description = null, Guid? parentId = null)
    {
        return new Category
        {
            Name = name,
            Description = description,
            ParentId = parentId,
            Slug = GenerateSlug(name.English)
        };
    }

    public void Update(LocalizedString name, LocalizedString? description)
    {
        Name = name;
        Description = description;
        Slug = GenerateSlug(name.English);
    }

    public void SetParent(Guid? parentId)
    {
        ParentId = parentId;
    }

    public void SetIcon(string? iconName, string? color)
    {
        IconName = iconName;
        Color = color;
    }

    public void SetSortOrder(int order)
    {
        SortOrder = order;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    private static string GenerateSlug(string name)
    {
        return name
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "");
    }
}

/// <summary>
/// Tag for content categorization.
/// </summary>
public class Tag : AuditableEntity
{
    public LocalizedString Name { get; private set; } = null!;
    public string Slug { get; private set; } = string.Empty;
    public string? Color { get; private set; }
    public int UsageCount { get; private set; }

    // Navigation properties
    public virtual ICollection<ArticleTag> Articles { get; private set; } = new List<ArticleTag>();

    private Tag() { }

    public static Tag Create(LocalizedString name, string? color = null)
    {
        return new Tag
        {
            Name = name,
            Color = color,
            Slug = GenerateSlug(name.English)
        };
    }

    public void Update(LocalizedString name, string? color)
    {
        Name = name;
        Color = color;
        Slug = GenerateSlug(name.English);
    }

    public void IncrementUsage() => UsageCount++;
    public void DecrementUsage() => UsageCount = Math.Max(0, UsageCount - 1);

    private static string GenerateSlug(string name)
    {
        return name
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "");
    }
}

/// <summary>
/// Content template for standardized content creation.
/// </summary>
public class ContentTemplate : AuditableEntity
{
    public LocalizedString Name { get; private set; } = null!;
    public LocalizedString? Description { get; private set; }
    public ArticleType TemplateType { get; private set; }
    public string ContentTemplateEn { get; private set; } = string.Empty;
    public string? ContentTemplateAr { get; private set; }
    public string? ThumbnailUrl { get; private set; }
    public bool IsActive { get; private set; } = true;
    public int UsageCount { get; private set; }

    private ContentTemplate() { }

    public static ContentTemplate Create(
        LocalizedString name,
        ArticleType type,
        string contentTemplateEn,
        string? contentTemplateAr = null)
    {
        return new ContentTemplate
        {
            Name = name,
            TemplateType = type,
            ContentTemplateEn = contentTemplateEn,
            ContentTemplateAr = contentTemplateAr
        };
    }

    public void Update(LocalizedString name, LocalizedString? description, string contentEn, string? contentAr)
    {
        Name = name;
        Description = description;
        ContentTemplateEn = contentEn;
        ContentTemplateAr = contentAr;
    }

    public void IncrementUsage() => UsageCount++;
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
