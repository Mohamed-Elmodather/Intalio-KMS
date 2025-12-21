namespace AFC27.KMS.Content.Application.DTOs;

/// <summary>
/// Category response DTO.
/// </summary>
public record CategoryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string Slug { get; init; } = string.Empty;
    public string? IconName { get; init; }
    public string? Color { get; init; }
    public Guid? ParentId { get; init; }
    public string? ParentName { get; init; }
    public int SortOrder { get; init; }
    public bool IsActive { get; init; }
    public int ArticleCount { get; init; }
    public IReadOnlyList<CategorySummaryDto> Children { get; init; } = Array.Empty<CategorySummaryDto>();
}

/// <summary>
/// Category summary for lists.
/// </summary>
public record CategorySummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string Slug { get; init; } = string.Empty;
    public string? IconName { get; init; }
    public string? Color { get; init; }
    public int ArticleCount { get; init; }
}

/// <summary>
/// Create category request.
/// </summary>
public record CreateCategoryRequest
{
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string? IconName { get; init; }
    public string? Color { get; init; }
    public Guid? ParentId { get; init; }
}

/// <summary>
/// Update category request.
/// </summary>
public record UpdateCategoryRequest
{
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string? IconName { get; init; }
    public string? Color { get; init; }
    public Guid? ParentId { get; init; }
    public int SortOrder { get; init; }
}

/// <summary>
/// Tag response DTO.
/// </summary>
public record TagDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string Slug { get; init; } = string.Empty;
    public string? Color { get; init; }
    public int UsageCount { get; init; }
}

/// <summary>
/// Tag summary for embedding in articles.
/// </summary>
public record TagSummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string Slug { get; init; } = string.Empty;
    public string? Color { get; init; }
}

/// <summary>
/// Create tag request.
/// </summary>
public record CreateTagRequest
{
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Color { get; init; }
}

/// <summary>
/// Content template DTO.
/// </summary>
public record ContentTemplateDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string TemplateType { get; init; } = string.Empty;
    public string ContentTemplateEn { get; init; } = string.Empty;
    public string? ContentTemplateAr { get; init; }
    public string? ThumbnailUrl { get; init; }
    public bool IsActive { get; init; }
    public int UsageCount { get; init; }
}

/// <summary>
/// Create template request.
/// </summary>
public record CreateTemplateRequest
{
    public string Name { get; init; } = string.Empty;
    public string? NameArabic { get; init; }
    public string? Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public string TemplateType { get; init; } = "Article";
    public string ContentTemplateEn { get; init; } = string.Empty;
    public string? ContentTemplateAr { get; init; }
}
