using AFC27.KMS.Workflow.Domain.Entities;

namespace AFC27.KMS.Workflow.Application.DTOs;

/// <summary>
/// Service request DTO
/// </summary>
public record ServiceRequestDto
{
    public Guid Id { get; init; }
    public string RequestNumber { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }

    public Guid ServiceId { get; init; }
    public string ServiceName { get; init; } = string.Empty;
    public string? ServiceIcon { get; init; }

    public Guid RequesterId { get; init; }
    public string RequesterName { get; init; } = string.Empty;
    public string? RequesterEmail { get; init; }
    public string? RequesterDepartment { get; init; }

    public RequestStatus Status { get; init; }
    public ServicePriority Priority { get; init; }

    public Guid? AssignedToId { get; init; }
    public string? AssignedToName { get; init; }
    public string? AssignedGroupName { get; init; }
    public DateTime? AssignedAt { get; init; }

    public string? CurrentStepName { get; init; }
    public int CurrentStepOrder { get; init; }

    public DateTime? SlaResponseDue { get; init; }
    public DateTime? SlaResolutionDue { get; init; }
    public bool IsSlaResponseBreached { get; init; }
    public bool IsSlaResolutionBreached { get; init; }

    public DateTime? FirstResponseAt { get; init; }
    public DateTime? ResolvedAt { get; init; }
    public DateTime? ClosedAt { get; init; }
    public string? ResolutionNotes { get; init; }
    public ResolutionType? Resolution { get; init; }

    public int CommentCount { get; init; }
    public int AttachmentCount { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
}

/// <summary>
/// Service request summary
/// </summary>
public record ServiceRequestSummaryDto
{
    public Guid Id { get; init; }
    public string RequestNumber { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string ServiceName { get; init; } = string.Empty;
    public string RequesterName { get; init; } = string.Empty;
    public RequestStatus Status { get; init; }
    public ServicePriority Priority { get; init; }
    public string? AssignedToName { get; init; }
    public bool IsSlaBreached { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Create service request
/// </summary>
public record CreateServiceRequestDto
{
    public Guid ServiceId { get; init; }
    public string TitleEn { get; init; } = string.Empty;
    public string TitleAr { get; init; } = string.Empty;
    public string? DescriptionEn { get; init; }
    public string? DescriptionAr { get; init; }
    public ServicePriority Priority { get; init; } = ServicePriority.Normal;
    public string? FormDataJson { get; init; }
    public bool SubmitImmediately { get; init; } = true;
}

/// <summary>
/// Update service request
/// </summary>
public record UpdateServiceRequestDto
{
    public string TitleEn { get; init; } = string.Empty;
    public string TitleAr { get; init; } = string.Empty;
    public string? DescriptionEn { get; init; }
    public string? DescriptionAr { get; init; }
    public ServicePriority Priority { get; init; }
}

/// <summary>
/// Assign request to user
/// </summary>
public record AssignRequestDto
{
    public Guid UserId { get; init; }
    public Guid? GroupId { get; init; }
    public string? Notes { get; init; }
}

/// <summary>
/// Resolve request
/// </summary>
public record ResolveRequestDto
{
    public ResolutionType Resolution { get; init; }
    public string? Notes { get; init; }
}

/// <summary>
/// Add comment to request
/// </summary>
public record AddCommentDto
{
    public string Content { get; init; } = string.Empty;
    public bool IsInternal { get; init; }
}

/// <summary>
/// Comment DTO
/// </summary>
public record RequestCommentDto
{
    public Guid Id { get; init; }
    public string Content { get; init; } = string.Empty;
    public Guid AuthorId { get; init; }
    public string AuthorName { get; init; } = string.Empty;
    public string? AuthorAvatar { get; init; }
    public bool IsInternal { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? EditedAt { get; init; }
}

/// <summary>
/// Attachment DTO
/// </summary>
public record RequestAttachmentDto
{
    public Guid Id { get; init; }
    public string FileName { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public string MimeType { get; init; } = string.Empty;
    public string DownloadUrl { get; init; } = string.Empty;
    public DateTime UploadedAt { get; init; }
}

/// <summary>
/// Activity DTO
/// </summary>
public record RequestActivityDto
{
    public Guid Id { get; init; }
    public RequestActivityType Type { get; init; }
    public string Description { get; init; } = string.Empty;
    public string? Details { get; init; }
    public DateTime OccurredAt { get; init; }
}

/// <summary>
/// Service request filter
/// </summary>
public record ServiceRequestFilterDto
{
    public string? Search { get; init; }
    public Guid? ServiceId { get; init; }
    public Guid? CategoryId { get; init; }
    public RequestStatus? Status { get; init; }
    public ServicePriority? Priority { get; init; }
    public Guid? RequesterId { get; init; }
    public Guid? AssignedToId { get; init; }
    public bool? IsOverdue { get; init; }
    public DateTime? CreatedFrom { get; init; }
    public DateTime? CreatedTo { get; init; }
    public string SortBy { get; init; } = "createdAt";
    public bool SortDescending { get; init; } = true;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

/// <summary>
/// Dashboard statistics
/// </summary>
public record RequestStatsDto
{
    public int TotalRequests { get; init; }
    public int PendingRequests { get; init; }
    public int InProgressRequests { get; init; }
    public int ResolvedRequests { get; init; }
    public int OverdueRequests { get; init; }
    public double AverageResolutionTimeHours { get; init; }
    public double SlaComplianceRate { get; init; }
    public Dictionary<RequestStatus, int> ByStatus { get; init; } = new();
    public Dictionary<ServicePriority, int> ByPriority { get; init; } = new();
}
