using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.WebApi.Data.Entities;

/// <summary>
/// Service category entity
/// </summary>
public class ServiceCategoryEntity : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public string Icon { get; set; } = "pi pi-folder";
    public Guid? ParentCategoryId { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ServiceCategoryEntity? ParentCategory { get; set; }
    public ICollection<ServiceCategoryEntity> SubCategories { get; set; } = new List<ServiceCategoryEntity>();
    public ICollection<CatalogServiceEntity> Services { get; set; } = new List<CatalogServiceEntity>();
}

/// <summary>
/// Catalog service entity
/// </summary>
public class CatalogServiceEntity : AuditableEntity
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string Icon { get; set; } = "pi pi-cog";
    public bool IsActive { get; set; } = true;
    public bool RequiresApproval { get; set; } = true;
    public int EstimatedDays { get; set; } = 3;

    // SLA Config (stored as separate columns for easier querying)
    public int SlaResponseTimeHours { get; set; } = 24;
    public int SlaResolutionTimeDays { get; set; } = 3;
    public bool SlaNotifyOnBreach { get; set; } = true;
    public string? SlaEscalationEmailsJson { get; set; } // JSON array

    public string? FieldsJson { get; set; } // JSON array of ServiceField
    public string? ApproverIdsJson { get; set; } // JSON array of approver GUIDs
    public string? WorkflowId { get; set; }

    // Navigation
    public ServiceCategoryEntity? Category { get; set; }
    public ICollection<ServiceRequestEntity> Requests { get; set; } = new List<ServiceRequestEntity>();
}

/// <summary>
/// Service request entity
/// </summary>
public class ServiceRequestEntity : AuditableEntity
{
    public string RequestNumber { get; set; } = string.Empty;
    public Guid ServiceId { get; set; }
    public Guid RequesterId { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public string RequesterEmail { get; set; } = string.Empty;
    public string? RequesterDepartment { get; set; }
    public ServiceRequestStatusEnum Status { get; set; } = ServiceRequestStatusEnum.Submitted;
    public ServiceRequestPriorityEnum Priority { get; set; } = ServiceRequestPriorityEnum.Normal;
    public string? FormDataJson { get; set; } // JSON object of form data
    public string? Notes { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime? AssignedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime SlaDeadline { get; set; }
    public bool IsSlaBreached { get; set; }
    public Guid? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }

    // Navigation
    public CatalogServiceEntity? Service { get; set; }
    public ICollection<RequestApprovalEntity> Approvals { get; set; } = new List<RequestApprovalEntity>();
    public ICollection<RequestCommentEntity> Comments { get; set; } = new List<RequestCommentEntity>();
    public ICollection<RequestAttachmentEntity> Attachments { get; set; } = new List<RequestAttachmentEntity>();
    public ICollection<RequestStatusHistoryEntity> StatusHistory { get; set; } = new List<RequestStatusHistoryEntity>();
}

/// <summary>
/// Request approval entity
/// </summary>
public class RequestApprovalEntity : AuditableEntity
{
    public Guid RequestId { get; set; }
    public Guid ApproverId { get; set; }
    public string ApproverName { get; set; } = string.Empty;
    public int ApprovalOrder { get; set; }
    public ApprovalStatusEnum Status { get; set; } = ApprovalStatusEnum.Pending;
    public string? Comments { get; set; }
    public DateTime? ActionedAt { get; set; }

    // Navigation
    public ServiceRequestEntity? Request { get; set; }
}

/// <summary>
/// Request comment entity
/// </summary>
public class RequestCommentEntity : AuditableEntity
{
    public Guid RequestId { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsInternal { get; set; }

    // Navigation
    public ServiceRequestEntity? Request { get; set; }
}

/// <summary>
/// Request attachment entity
/// </summary>
public class RequestAttachmentEntity : AuditableEntity
{
    public Guid RequestId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public Guid UploadedById { get; set; }

    // Navigation
    public ServiceRequestEntity? Request { get; set; }
}

/// <summary>
/// Request status history entity
/// </summary>
public class RequestStatusHistoryEntity : AuditableEntity
{
    public Guid RequestId { get; set; }
    public ServiceRequestStatusEnum FromStatus { get; set; }
    public ServiceRequestStatusEnum ToStatus { get; set; }
    public Guid ChangedById { get; set; }
    public string ChangedByName { get; set; } = string.Empty;
    public string? Reason { get; set; }

    // Navigation
    public ServiceRequestEntity? Request { get; set; }
}

// Enums
public enum ServiceRequestStatusEnum
{
    Draft,
    Submitted,
    PendingApproval,
    Approved,
    Rejected,
    InProgress,
    OnHold,
    Completed,
    Cancelled
}

public enum ServiceRequestPriorityEnum
{
    Low,
    Normal,
    High,
    Urgent
}

public enum ApprovalStatusEnum
{
    Pending,
    Approved,
    Rejected,
    Delegated
}
