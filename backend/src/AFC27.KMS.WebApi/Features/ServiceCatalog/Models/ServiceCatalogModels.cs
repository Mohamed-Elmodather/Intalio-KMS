namespace AFC27.KMS.WebApi.Features.ServiceCatalog.Models;

/// <summary>
/// Service category in the catalog
/// </summary>
public class ServiceCategory
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public string Icon { get; set; } = "pi pi-folder";
    public Guid? ParentCategoryId { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public int ServiceCount { get; set; }
    public List<ServiceCategory> SubCategories { get; set; } = new();
}

/// <summary>
/// Service in the catalog
/// </summary>
public class CatalogService
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string Icon { get; set; } = "pi pi-cog";
    public bool IsActive { get; set; } = true;
    public bool RequiresApproval { get; set; } = true;
    public int EstimatedDays { get; set; } = 3;
    public ServiceSlaConfig Sla { get; set; } = new();
    public List<ServiceField> Fields { get; set; } = new();
    public List<Guid> ApproverIds { get; set; } = new();
    public string? WorkflowId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int TotalRequests { get; set; }
    public double AverageCompletionDays { get; set; }
}

/// <summary>
/// Dynamic field for service request form
/// </summary>
public class ServiceField
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string LabelAr { get; set; } = string.Empty;
    public FieldType Type { get; set; }
    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }
    public string? Placeholder { get; set; }
    public string? DefaultValue { get; set; }
    public List<FieldOption>? Options { get; set; }
    public FieldValidation? Validation { get; set; }
}

public class FieldOption
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? LabelAr { get; set; }
}

public class FieldValidation
{
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public string? Pattern { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// SLA configuration for a service
/// </summary>
public class ServiceSlaConfig
{
    public int ResponseTimeHours { get; set; } = 24;
    public int ResolutionTimeDays { get; set; } = 3;
    public bool NotifyOnBreach { get; set; } = true;
    public List<string> EscalationEmails { get; set; } = new();
}

/// <summary>
/// Service request submitted by user
/// </summary>
public class ServiceRequest
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string ServiceNameAr { get; set; } = string.Empty;
    public Guid RequesterId { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public string RequesterEmail { get; set; } = string.Empty;
    public string? RequesterDepartment { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Submitted;
    public RequestPriority Priority { get; set; } = RequestPriority.Normal;
    public Dictionary<string, object> FormData { get; set; } = new();
    public string? Notes { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime? AssignedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime SlaDeadline { get; set; }
    public bool IsSlaBreached { get; set; }
    public Guid? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    public List<RequestApproval> Approvals { get; set; } = new();
    public List<RequestComment> Comments { get; set; } = new();
    public List<RequestAttachment> Attachments { get; set; } = new();
    public List<RequestStatusHistory> StatusHistory { get; set; } = new();
}

public class RequestApproval
{
    public Guid Id { get; set; }
    public Guid RequestId { get; set; }
    public Guid ApproverId { get; set; }
    public string ApproverName { get; set; } = string.Empty;
    public int ApprovalOrder { get; set; }
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
    public string? Comments { get; set; }
    public DateTime? ActionedAt { get; set; }
}

public class RequestComment
{
    public Guid Id { get; set; }
    public Guid RequestId { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class RequestAttachment
{
    public Guid Id { get; set; }
    public Guid RequestId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public Guid UploadedById { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}

public class RequestStatusHistory
{
    public Guid Id { get; set; }
    public RequestStatus FromStatus { get; set; }
    public RequestStatus ToStatus { get; set; }
    public Guid ChangedById { get; set; }
    public string ChangedByName { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}

// Enums
public enum FieldType
{
    Text,
    TextArea,
    Number,
    Email,
    Phone,
    Date,
    DateTime,
    Select,
    MultiSelect,
    Checkbox,
    Radio,
    File,
    User,
    Department
}

public enum RequestStatus
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

public enum RequestPriority
{
    Low,
    Normal,
    High,
    Urgent
}

public enum ApprovalStatus
{
    Pending,
    Approved,
    Rejected,
    Delegated
}

// Request/Response Models
public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public string Icon { get; set; } = "pi pi-folder";
    public Guid? ParentCategoryId { get; set; }
    public int SortOrder { get; set; }
}

public class CreateServiceRequest
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string Icon { get; set; } = "pi pi-cog";
    public bool RequiresApproval { get; set; } = true;
    public int EstimatedDays { get; set; } = 3;
    public ServiceSlaConfig? Sla { get; set; }
    public List<ServiceField> Fields { get; set; } = new();
    public List<Guid> ApproverIds { get; set; } = new();
}

public class SubmitRequestRequest
{
    public Guid ServiceId { get; set; }
    public Dictionary<string, object> FormData { get; set; } = new();
    public RequestPriority Priority { get; set; } = RequestPriority.Normal;
    public string? Notes { get; set; }
}

public class UpdateRequestStatusRequest
{
    public RequestStatus Status { get; set; }
    public string? Reason { get; set; }
}

public class ApprovalDecisionRequest
{
    public ApprovalStatus Decision { get; set; }
    public string? Comments { get; set; }
}

public class AddCommentRequest
{
    public string Content { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
}

public class ServiceCatalogDashboard
{
    public int TotalServices { get; set; }
    public int ActiveServices { get; set; }
    public int TotalCategories { get; set; }
    public int TotalRequests { get; set; }
    public int PendingRequests { get; set; }
    public int InProgressRequests { get; set; }
    public int CompletedThisMonth { get; set; }
    public double SlaComplianceRate { get; set; }
    public double AverageCompletionDays { get; set; }
    public List<ServiceRequestSummary> RecentRequests { get; set; } = new();
    public List<ServiceUsageStats> TopServices { get; set; } = new();
}

public class ServiceRequestSummary
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string RequesterName { get; set; } = string.Empty;
    public RequestStatus Status { get; set; }
    public DateTime SubmittedAt { get; set; }
    public bool IsSlaBreached { get; set; }
}

public class ServiceUsageStats
{
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public int RequestCount { get; set; }
    public double AverageCompletionDays { get; set; }
}

public class SlaReport
{
    public int TotalRequests { get; set; }
    public int OnTimeRequests { get; set; }
    public int BreachedRequests { get; set; }
    public double ComplianceRate { get; set; }
    public List<SlaBreachDetail> Breaches { get; set; } = new();
}

public class SlaBreachDetail
{
    public Guid RequestId { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public TimeSpan BreachDuration { get; set; }
}
