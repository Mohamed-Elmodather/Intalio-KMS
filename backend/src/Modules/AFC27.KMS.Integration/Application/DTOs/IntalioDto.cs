using AFC27.KMS.Integration.Domain.Entities;

namespace AFC27.KMS.Integration.Application.DTOs;

/// <summary>
/// Intalio Case process definition DTO
/// </summary>
public class IntalioProcessDto
{
    public string ProcessId { get; set; } = string.Empty;
    public string ProcessKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public int Version { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public Guid? MappedServiceId { get; set; }
    public string? MappedServiceName { get; set; }
}

/// <summary>
/// Intalio task DTO
/// </summary>
public class IntalioTaskDto
{
    public string TaskId { get; set; } = string.Empty;
    public string ProcessInstanceId { get; set; } = string.Empty;
    public string? ProcessKey { get; set; }
    public string? ProcessName { get; set; }

    public string TaskName { get; set; } = string.Empty;
    public string? TaskNameAr { get; set; }
    public string? Description { get; set; }
    public string? FormKey { get; set; }

    public string Status { get; set; } = string.Empty;
    public string? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public string? CandidateGroups { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ClaimedAt { get; set; }

    public int Priority { get; set; }
    public Dictionary<string, object>? Variables { get; set; }

    public Guid? KMSTaskId { get; set; }
    public bool IsSyncedToKMS { get; set; }
}

/// <summary>
/// Start Intalio process request
/// </summary>
public class StartIntalioProcessRequest
{
    public string ProcessKey { get; set; } = string.Empty;
    public string? BusinessKey { get; set; }
    public Dictionary<string, object>? Variables { get; set; }
    public Guid? InitiatorId { get; set; }
    public bool CreateKMSTask { get; set; } = true;
}

/// <summary>
/// Start process result DTO
/// </summary>
public class StartProcessResultDto
{
    public string ProcessInstanceId { get; set; } = string.Empty;
    public string ProcessKey { get; set; } = string.Empty;
    public string? BusinessKey { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<IntalioTaskDto>? ActiveTasks { get; set; }
    public Guid? KMSServiceRequestId { get; set; }
}

/// <summary>
/// Complete Intalio task request
/// </summary>
public class CompleteIntalioTaskRequest
{
    public string TaskId { get; set; } = string.Empty;
    public string? Outcome { get; set; }
    public Dictionary<string, object>? Variables { get; set; }
    public string? Comment { get; set; }
}

/// <summary>
/// Claim Intalio task request
/// </summary>
public class ClaimIntalioTaskRequest
{
    public string TaskId { get; set; } = string.Empty;
    public string? AssigneeId { get; set; }
}

/// <summary>
/// Intalio user DTO (from IAM)
/// </summary>
public class IntalioUserDto
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FirstNameAr { get; set; }
    public string? LastNameAr { get; set; }
    public string? DisplayName { get; set; }
    public string? Department { get; set; }
    public string? JobTitle { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public List<string>? Groups { get; set; }
    public List<string>? Roles { get; set; }
    public Guid? KMSUserId { get; set; }
    public bool IsSyncedToKMS { get; set; }
}

/// <summary>
/// Intalio group DTO (from IAM)
/// </summary>
public class IntalioGroupDto
{
    public string GroupId { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string? GroupNameAr { get; set; }
    public string? Description { get; set; }
    public string? ParentGroupId { get; set; }
    public int MemberCount { get; set; }
    public Guid? KMSGroupId { get; set; }
    public bool IsSyncedToKMS { get; set; }
}

/// <summary>
/// Intalio document DTO (from DMS)
/// </summary>
public class IntalioDocumentDto
{
    public string DocumentId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string? FolderId { get; set; }
    public string? FolderPath { get; set; }

    public int Version { get; set; }
    public bool IsLatestVersion { get; set; }
    public bool IsCheckedOut { get; set; }
    public string? CheckedOutBy { get; set; }

    public Dictionary<string, object>? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }

    public Guid? KMSDocumentId { get; set; }
    public bool IsSyncedToKMS { get; set; }
}

/// <summary>
/// Upload to Intalio DMS request
/// </summary>
public class UploadToIntalioDMSRequest
{
    public string FileName { get; set; } = string.Empty;
    public string? FolderId { get; set; }
    public string? ContentType { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public bool CreateNewVersion { get; set; }
    public string? ExistingDocumentId { get; set; }
}

/// <summary>
/// Intalio correspondence DTO
/// </summary>
public class IntalioCorrespondenceDto
{
    public string CorrespondenceId { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public CorrespondenceType Type { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? SubjectAr { get; set; }

    public string? FromOrganization { get; set; }
    public string? FromPerson { get; set; }
    public string? ToOrganization { get; set; }
    public string? ToPerson { get; set; }

    public string Category { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public DateTime ReceivedDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ClosedDate { get; set; }

    public List<string>? AttachmentIds { get; set; }
    public string? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }

    public Dictionary<string, object>? CustomFields { get; set; }
    public Guid? KMSEntityId { get; set; }
    public bool IsSyncedToKMS { get; set; }
}

/// <summary>
/// Correspondence type
/// </summary>
public enum CorrespondenceType
{
    Incoming,
    Outgoing,
    Internal
}

/// <summary>
/// Create correspondence request
/// </summary>
public class CreateCorrespondenceRequest
{
    public CorrespondenceType Type { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? SubjectAr { get; set; }
    public string? Body { get; set; }

    public string? FromOrganization { get; set; }
    public string? FromPerson { get; set; }
    public string? ToOrganization { get; set; }
    public string? ToPerson { get; set; }

    public string Category { get; set; } = string.Empty;
    public string Priority { get; set; } = "Normal";
    public DateTime? DueDate { get; set; }

    public List<Guid>? AttachmentIds { get; set; }
    public Guid? AssignToUserId { get; set; }
    public Dictionary<string, object>? CustomFields { get; set; }

    public bool TriggerWorkflow { get; set; } = true;
}

/// <summary>
/// Microsoft 365 sync status DTO
/// </summary>
public class M365SyncStatusDto
{
    public bool SharePointEnabled { get; set; }
    public bool ExchangeEnabled { get; set; }
    public bool TeamsEnabled { get; set; }
    public bool OneDriveEnabled { get; set; }

    public int SharePointLibrariesSynced { get; set; }
    public int DocumentsSynced { get; set; }
    public int CalendarEventsSynced { get; set; }
    public int UsersFromAzureAD { get; set; }

    public DateTime? LastSharePointSync { get; set; }
    public DateTime? LastCalendarSync { get; set; }
    public DateTime? LastAzureADSync { get; set; }

    public List<string>? Errors { get; set; }
}

/// <summary>
/// SharePoint document DTO
/// </summary>
public class SharePointDocumentDto
{
    public string DriveItemId { get; set; } = string.Empty;
    public string DriveId { get; set; } = string.Empty;
    public string SiteId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string? WebUrl { get; set; }
    public long Size { get; set; }
    public string? ContentType { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }

    public Guid? KMSDocumentId { get; set; }
    public bool IsSyncedToKMS { get; set; }
}

/// <summary>
/// ERP employee DTO
/// </summary>
public class ERPEmployeeDto
{
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? FirstNameAr { get; set; }
    public string? LastNameAr { get; set; }

    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public string? DepartmentCode { get; set; }
    public string? CostCenter { get; set; }
    public string? JobTitle { get; set; }
    public string? ManagerId { get; set; }

    public bool IsActive { get; set; }
    public DateTime? HireDate { get; set; }

    public Guid? KMSUserId { get; set; }
    public bool IsSyncedToKMS { get; set; }
}

/// <summary>
/// ERP organization unit DTO
/// </summary>
public class ERPOrganizationUnitDto
{
    public string UnitId { get; set; } = string.Empty;
    public string UnitCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? ParentUnitId { get; set; }
    public string? ManagerId { get; set; }
    public string? CostCenter { get; set; }
    public bool IsActive { get; set; }

    public Guid? KMSDepartmentId { get; set; }
    public bool IsSyncedToKMS { get; set; }
}
