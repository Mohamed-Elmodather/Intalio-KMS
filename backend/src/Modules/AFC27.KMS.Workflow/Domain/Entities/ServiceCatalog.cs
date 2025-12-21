using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Workflow.Domain.Entities;

/// <summary>
/// Service category for organizing services
/// </summary>
public class ServiceCategory : AuditableEntity
{
    public LocalizedString Name { get; private set; } = new();
    public LocalizedString? Description { get; private set; }
    public string? Icon { get; private set; }
    public string? Color { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Guid? ParentCategoryId { get; private set; }
    public ServiceCategory? ParentCategory { get; private set; }
    public List<ServiceCategory> SubCategories { get; private set; } = new();
    public List<Service> Services { get; private set; } = new();

    private ServiceCategory() { }

    public static ServiceCategory Create(LocalizedString name, string? icon = null)
    {
        return new ServiceCategory
        {
            Id = Guid.NewGuid(),
            Name = name,
            Icon = icon
        };
    }

    public void Update(LocalizedString name, LocalizedString? description, string? icon, string? color)
    {
        Name = name;
        Description = description;
        Icon = icon;
        Color = color;
    }

    public void SetParent(Guid? parentId)
    {
        ParentCategoryId = parentId;
    }

    public void SetSortOrder(int order) => SortOrder = order;
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}

/// <summary>
/// Service in the service catalog
/// </summary>
public class Service : AuditableEntity
{
    public LocalizedString Name { get; private set; } = new();
    public LocalizedString? Description { get; private set; }
    public LocalizedString? FullDescription { get; private set; }
    public string? Icon { get; private set; }
    public string? ImageUrl { get; private set; }

    public Guid CategoryId { get; private set; }
    public ServiceCategory Category { get; private set; } = null!;

    public ServiceStatus Status { get; private set; } = ServiceStatus.Draft;
    public ServiceType Type { get; private set; } = ServiceType.Standard;
    public ServicePriority DefaultPriority { get; private set; } = ServicePriority.Normal;

    // SLA Configuration
    public int? SlaResponseHours { get; private set; }
    public int? SlaResolutionHours { get; private set; }

    // Form and Workflow
    public Guid? FormId { get; private set; }
    public Form? Form { get; private set; }
    public Guid? WorkflowDefinitionId { get; private set; }
    public WorkflowDefinition? WorkflowDefinition { get; private set; }

    // Access control
    public bool RequiresApproval { get; private set; } = true;
    public List<Guid> AllowedRoleIds { get; private set; } = new();
    public List<Guid> AllowedDepartmentIds { get; private set; } = new();

    // Metadata
    public int SortOrder { get; private set; }
    public long RequestCount { get; private set; }
    public List<string> Tags { get; private set; } = new();
    public bool IsFeatured { get; private set; }

    // Related services
    public List<Service> RelatedServices { get; private set; } = new();

    private Service() { }

    public static Service Create(
        LocalizedString name,
        Guid categoryId,
        ServiceType type = ServiceType.Standard)
    {
        var service = new Service
        {
            Id = Guid.NewGuid(),
            Name = name,
            CategoryId = categoryId,
            Type = type,
            Status = ServiceStatus.Draft
        };

        service.AddDomainEvent(new ServiceCreatedEvent(service.Id, name.English));
        return service;
    }

    public void Update(
        LocalizedString name,
        LocalizedString? description,
        LocalizedString? fullDescription,
        string? icon,
        string? imageUrl)
    {
        Name = name;
        Description = description;
        FullDescription = fullDescription;
        Icon = icon;
        ImageUrl = imageUrl;
    }

    public void SetCategory(Guid categoryId)
    {
        CategoryId = categoryId;
    }

    public void SetSla(int? responseHours, int? resolutionHours)
    {
        SlaResponseHours = responseHours;
        SlaResolutionHours = resolutionHours;
    }

    public void SetForm(Guid? formId)
    {
        FormId = formId;
    }

    public void SetWorkflow(Guid? workflowId)
    {
        WorkflowDefinitionId = workflowId;
    }

    public void SetAccessControl(List<Guid> roleIds, List<Guid> departmentIds)
    {
        AllowedRoleIds = roleIds;
        AllowedDepartmentIds = departmentIds;
    }

    public void Publish()
    {
        Status = ServiceStatus.Published;
        AddDomainEvent(new ServicePublishedEvent(Id));
    }

    public void Unpublish() => Status = ServiceStatus.Draft;
    public void Archive() => Status = ServiceStatus.Archived;
    public void SetFeatured(bool featured) => IsFeatured = featured;
    public void SetSortOrder(int order) => SortOrder = order;
    public void SetTags(List<string> tags) => Tags = tags;
    public void IncrementRequestCount() => RequestCount++;
}

public enum ServiceStatus
{
    Draft,
    Published,
    Archived
}

public enum ServiceType
{
    Standard,
    Express,
    Automated,
    External
}

public enum ServicePriority
{
    Low,
    Normal,
    High,
    Urgent,
    Critical
}

public record ServiceCreatedEvent(Guid ServiceId, string Name) : DomainEvent;
public record ServicePublishedEvent(Guid ServiceId) : DomainEvent;
