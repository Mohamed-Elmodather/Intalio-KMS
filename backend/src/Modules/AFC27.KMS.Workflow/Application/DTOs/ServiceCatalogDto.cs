using AFC27.KMS.Workflow.Domain.Entities;

namespace AFC27.KMS.Workflow.Application.DTOs;

/// <summary>
/// Service category DTO
/// </summary>
public record ServiceCategoryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Icon { get; init; }
    public string? Color { get; init; }
    public int SortOrder { get; init; }
    public bool IsActive { get; init; }
    public Guid? ParentCategoryId { get; init; }
    public int ServiceCount { get; init; }
    public List<ServiceCategoryDto> SubCategories { get; init; } = new();
}

/// <summary>
/// Create category request
/// </summary>
public record CreateCategoryRequest
{
    public string NameEn { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string? DescriptionEn { get; init; }
    public string? DescriptionAr { get; init; }
    public string? Icon { get; init; }
    public string? Color { get; init; }
    public Guid? ParentCategoryId { get; init; }
}

/// <summary>
/// Update category request
/// </summary>
public record UpdateCategoryRequest
{
    public string NameEn { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string? DescriptionEn { get; init; }
    public string? DescriptionAr { get; init; }
    public string? Icon { get; init; }
    public string? Color { get; init; }
}

/// <summary>
/// Service DTO
/// </summary>
public record ServiceDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? FullDescription { get; init; }
    public string? Icon { get; init; }
    public string? ImageUrl { get; init; }
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public ServiceStatus Status { get; init; }
    public ServiceType Type { get; init; }
    public ServicePriority DefaultPriority { get; init; }
    public int? SlaResponseHours { get; init; }
    public int? SlaResolutionHours { get; init; }
    public Guid? FormId { get; init; }
    public string? FormName { get; init; }
    public Guid? WorkflowDefinitionId { get; init; }
    public string? WorkflowName { get; init; }
    public bool RequiresApproval { get; init; }
    public bool IsFeatured { get; init; }
    public long RequestCount { get; init; }
    public List<string> Tags { get; init; } = new();
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Service summary for catalog listing
/// </summary>
public record ServiceSummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Icon { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public ServiceType Type { get; init; }
    public bool IsFeatured { get; init; }
    public long RequestCount { get; init; }
}

/// <summary>
/// Create service request
/// </summary>
public record CreateServiceRequest
{
    public string NameEn { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string? DescriptionEn { get; init; }
    public string? DescriptionAr { get; init; }
    public string? FullDescriptionEn { get; init; }
    public string? FullDescriptionAr { get; init; }
    public string? Icon { get; init; }
    public Guid CategoryId { get; init; }
    public ServiceType Type { get; init; } = ServiceType.Standard;
    public ServicePriority DefaultPriority { get; init; } = ServicePriority.Normal;
    public int? SlaResponseHours { get; init; }
    public int? SlaResolutionHours { get; init; }
    public Guid? FormId { get; init; }
    public Guid? WorkflowDefinitionId { get; init; }
    public bool RequiresApproval { get; init; } = true;
    public List<string> Tags { get; init; } = new();
}

/// <summary>
/// Update service request
/// </summary>
public record UpdateServiceRequest
{
    public string NameEn { get; init; } = string.Empty;
    public string NameAr { get; init; } = string.Empty;
    public string? DescriptionEn { get; init; }
    public string? DescriptionAr { get; init; }
    public string? FullDescriptionEn { get; init; }
    public string? FullDescriptionAr { get; init; }
    public string? Icon { get; init; }
    public string? ImageUrl { get; init; }
    public Guid CategoryId { get; init; }
    public ServicePriority DefaultPriority { get; init; }
    public int? SlaResponseHours { get; init; }
    public int? SlaResolutionHours { get; init; }
    public Guid? FormId { get; init; }
    public Guid? WorkflowDefinitionId { get; init; }
    public bool RequiresApproval { get; init; }
    public List<string> Tags { get; init; } = new();
}

/// <summary>
/// Service filter request
/// </summary>
public record ServiceFilterRequest
{
    public string? Search { get; init; }
    public Guid? CategoryId { get; init; }
    public ServiceType? Type { get; init; }
    public ServiceStatus? Status { get; init; }
    public bool? IsFeatured { get; init; }
    public List<string> Tags { get; init; } = new();
    public string SortBy { get; init; } = "name";
    public bool SortDescending { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

/// <summary>
/// Service access configuration
/// </summary>
public record ServiceAccessRequest
{
    public List<Guid> AllowedRoleIds { get; init; } = new();
    public List<Guid> AllowedDepartmentIds { get; init; } = new();
}
