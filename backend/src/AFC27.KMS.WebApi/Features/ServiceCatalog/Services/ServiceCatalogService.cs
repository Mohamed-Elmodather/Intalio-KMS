using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using AFC27.KMS.WebApi.Data;
using AFC27.KMS.WebApi.Data.Entities;
using AFC27.KMS.WebApi.Features.ServiceCatalog.Models;

namespace AFC27.KMS.WebApi.Features.ServiceCatalog.Services;

/// <summary>
/// Implementation of service catalog management using database persistence
/// </summary>
public class ServiceCatalogService : IServiceCatalogService
{
    private readonly KmsDbContext _dbContext;
    private readonly ILogger<ServiceCatalogService> _logger;
    private static int _requestCounter = 1000;

    public ServiceCatalogService(KmsDbContext dbContext, ILogger<ServiceCatalogService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    // Categories
    public async Task<ServiceCategory> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new ServiceCategoryEntity
        {
            Name = request.Name,
            NameAr = request.NameAr,
            Description = request.Description,
            DescriptionAr = request.DescriptionAr,
            Icon = request.Icon,
            ParentCategoryId = request.ParentCategoryId,
            SortOrder = request.SortOrder
        };

        _dbContext.ServiceCategories.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created service category {CategoryId}: {Name}", entity.Id, entity.Name);
        return MapCategory(entity);
    }

    public async Task<ServiceCategory?> GetCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.ServiceCategories
            .Include(c => c.SubCategories)
            .Include(c => c.Services)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        return entity == null ? null : MapCategory(entity);
    }

    public async Task<List<ServiceCategory>> GetCategoriesAsync(bool includeServices = false, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ServiceCategories
            .Where(c => c.ParentCategoryId == null && c.IsActive)
            .Include(c => c.SubCategories.Where(sc => sc.IsActive))
            .Include(c => c.Services.Where(s => s.IsActive));

        var entities = await query.OrderBy(c => c.SortOrder).ToListAsync(cancellationToken);

        return entities.Select(e =>
        {
            var cat = MapCategory(e);
            cat.ServiceCount = e.Services.Count;
            cat.SubCategories = e.SubCategories.Select(MapCategory).ToList();
            return cat;
        }).ToList();
    }

    public async Task<ServiceCategory> UpdateCategoryAsync(Guid id, CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.ServiceCategories.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new KeyNotFoundException();

        entity.Name = request.Name;
        entity.NameAr = request.NameAr;
        entity.Description = request.Description;
        entity.DescriptionAr = request.DescriptionAr;
        entity.Icon = request.Icon;
        entity.SortOrder = request.SortOrder;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapCategory(entity);
    }

    public async Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.ServiceCategories.FindAsync(new object[] { id }, cancellationToken);
        if (entity != null)
        {
            entity.IsActive = false;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    // Services
    public async Task<CatalogService> CreateServiceAsync(CreateServiceRequest request, CancellationToken cancellationToken = default)
    {
        var category = await _dbContext.ServiceCategories.FindAsync(new object[] { request.CategoryId }, cancellationToken)
            ?? throw new KeyNotFoundException("Category not found");

        var entity = new CatalogServiceEntity
        {
            CategoryId = request.CategoryId,
            Name = request.Name,
            NameAr = request.NameAr,
            Description = request.Description,
            DescriptionAr = request.DescriptionAr,
            Icon = request.Icon,
            RequiresApproval = request.RequiresApproval,
            EstimatedDays = request.EstimatedDays,
            SlaResponseTimeHours = request.Sla?.ResponseTimeHours ?? 24,
            SlaResolutionTimeDays = request.Sla?.ResolutionTimeDays ?? 3,
            SlaNotifyOnBreach = request.Sla?.NotifyOnBreach ?? true,
            SlaEscalationEmailsJson = request.Sla?.EscalationEmails != null ? JsonSerializer.Serialize(request.Sla.EscalationEmails) : null,
            FieldsJson = JsonSerializer.Serialize(request.Fields),
            ApproverIdsJson = JsonSerializer.Serialize(request.ApproverIds)
        };

        _dbContext.CatalogServices.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created service {ServiceId}: {Name}", entity.Id, entity.Name);
        return MapService(entity, category.Name);
    }

    public async Task<CatalogService?> GetServiceAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.CatalogServices
            .Include(s => s.Category)
            .Include(s => s.Requests)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        return entity == null ? null : MapService(entity, entity.Category?.Name ?? "");
    }

    public async Task<(List<CatalogService> Services, int TotalCount)> GetServicesAsync(Guid? categoryId, string? search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.CatalogServices
            .Include(s => s.Category)
            .Where(s => s.IsActive);

        if (categoryId.HasValue)
            query = query.Where(s => s.CategoryId == categoryId.Value);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(s => s.Name.Contains(search) || s.Description.Contains(search));

        var total = await query.CountAsync(cancellationToken);
        var entities = await query
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (entities.Select(e => MapService(e, e.Category?.Name ?? "")).ToList(), total);
    }

    public async Task<CatalogService> UpdateServiceAsync(Guid id, CreateServiceRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.CatalogServices
            .Include(s => s.Category)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException();

        entity.Name = request.Name;
        entity.NameAr = request.NameAr;
        entity.Description = request.Description;
        entity.DescriptionAr = request.DescriptionAr;
        entity.Icon = request.Icon;
        entity.RequiresApproval = request.RequiresApproval;
        entity.EstimatedDays = request.EstimatedDays;
        entity.FieldsJson = JsonSerializer.Serialize(request.Fields);
        entity.ApproverIdsJson = JsonSerializer.Serialize(request.ApproverIds);

        if (request.Sla != null)
        {
            entity.SlaResponseTimeHours = request.Sla.ResponseTimeHours;
            entity.SlaResolutionTimeDays = request.Sla.ResolutionTimeDays;
            entity.SlaNotifyOnBreach = request.Sla.NotifyOnBreach;
            entity.SlaEscalationEmailsJson = JsonSerializer.Serialize(request.Sla.EscalationEmails);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapService(entity, entity.Category?.Name ?? "");
    }

    public async Task DeleteServiceAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.CatalogServices.FindAsync(new object[] { id }, cancellationToken);
        if (entity != null)
        {
            entity.IsActive = false;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    // Service Requests
    public async Task<ServiceRequest> SubmitRequestAsync(SubmitRequestRequest request, Guid requesterId, string requesterName, string requesterEmail, CancellationToken cancellationToken = default)
    {
        var service = await _dbContext.CatalogServices
            .FirstOrDefaultAsync(s => s.Id == request.ServiceId, cancellationToken)
            ?? throw new KeyNotFoundException("Service not found");

        var requestNumber = $"SR-{DateTime.UtcNow:yyyyMMdd}-{Interlocked.Increment(ref _requestCounter)}";
        var approverIds = !string.IsNullOrEmpty(service.ApproverIdsJson)
            ? JsonSerializer.Deserialize<List<Guid>>(service.ApproverIdsJson) ?? new List<Guid>()
            : new List<Guid>();

        var entity = new ServiceRequestEntity
        {
            RequestNumber = requestNumber,
            ServiceId = service.Id,
            RequesterId = requesterId,
            RequesterName = requesterName,
            RequesterEmail = requesterEmail,
            Status = service.RequiresApproval ? ServiceRequestStatusEnum.PendingApproval : ServiceRequestStatusEnum.Submitted,
            Priority = (ServiceRequestPriorityEnum)request.Priority,
            FormDataJson = JsonSerializer.Serialize(request.FormData),
            Notes = request.Notes,
            SubmittedAt = DateTime.UtcNow,
            SlaDeadline = DateTime.UtcNow.AddDays(service.SlaResolutionTimeDays)
        };

        _dbContext.ServiceRequests.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Create approval records
        if (service.RequiresApproval && approverIds.Any())
        {
            for (int i = 0; i < approverIds.Count; i++)
            {
                _dbContext.RequestApprovals.Add(new RequestApprovalEntity
                {
                    RequestId = entity.Id,
                    ApproverId = approverIds[i],
                    ApproverName = $"Approver {i + 1}",
                    ApprovalOrder = i,
                    Status = ApprovalStatusEnum.Pending
                });
            }
        }

        // Add initial status history
        _dbContext.RequestStatusHistory.Add(new RequestStatusHistoryEntity
        {
            RequestId = entity.Id,
            FromStatus = ServiceRequestStatusEnum.Draft,
            ToStatus = entity.Status,
            ChangedById = requesterId,
            ChangedByName = requesterName,
            Reason = "Request submitted"
        });

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Submitted service request {RequestNumber} for service {ServiceName}", requestNumber, service.Name);

        return await GetRequestAsync(entity.Id, cancellationToken) ?? throw new InvalidOperationException("Failed to retrieve created request");
    }

    public async Task<ServiceRequest?> GetRequestAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.ServiceRequests
            .Include(r => r.Service)
            .Include(r => r.Approvals)
            .Include(r => r.Comments)
            .Include(r => r.StatusHistory)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        return entity == null ? null : MapRequest(entity);
    }

    public async Task<ServiceRequest?> GetRequestByNumberAsync(string requestNumber, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.ServiceRequests
            .Include(r => r.Service)
            .Include(r => r.Approvals)
            .Include(r => r.Comments)
            .FirstOrDefaultAsync(r => r.RequestNumber == requestNumber, cancellationToken);

        return entity == null ? null : MapRequest(entity);
    }

    public async Task<(List<ServiceRequest> Requests, int TotalCount)> GetMyRequestsAsync(Guid userId, RequestStatus? status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ServiceRequests
            .Include(r => r.Service)
            .Where(r => r.RequesterId == userId);

        if (status.HasValue)
            query = query.Where(r => r.Status == (ServiceRequestStatusEnum)status.Value);

        var total = await query.CountAsync(cancellationToken);
        var entities = await query
            .OrderByDescending(r => r.SubmittedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (entities.Select(MapRequest).ToList(), total);
    }

    public async Task<(List<ServiceRequest> Requests, int TotalCount)> GetPendingRequestsAsync(Guid? assigneeId, RequestStatus? status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ServiceRequests
            .Include(r => r.Service)
            .AsQueryable();

        if (assigneeId.HasValue)
            query = query.Where(r => r.AssignedToId == assigneeId.Value);

        if (status.HasValue)
            query = query.Where(r => r.Status == (ServiceRequestStatusEnum)status.Value);
        else
            query = query.Where(r => r.Status != ServiceRequestStatusEnum.Completed && r.Status != ServiceRequestStatusEnum.Cancelled);

        var total = await query.CountAsync(cancellationToken);
        var entities = await query
            .OrderByDescending(r => r.SubmittedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (entities.Select(MapRequest).ToList(), total);
    }

    public async Task<ServiceRequest> UpdateRequestStatusAsync(Guid id, UpdateRequestStatusRequest request, Guid userId, string userName, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.ServiceRequests
            .Include(r => r.Service)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException();

        var oldStatus = entity.Status;
        entity.Status = (ServiceRequestStatusEnum)request.Status;

        if (request.Status == RequestStatus.Completed)
            entity.CompletedAt = DateTime.UtcNow;

        // Check SLA breach
        if (entity.CompletedAt.HasValue && entity.CompletedAt > entity.SlaDeadline)
            entity.IsSlaBreached = true;

        _dbContext.RequestStatusHistory.Add(new RequestStatusHistoryEntity
        {
            RequestId = id,
            FromStatus = oldStatus,
            ToStatus = entity.Status,
            ChangedById = userId,
            ChangedByName = userName,
            Reason = request.Reason
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapRequest(entity);
    }

    public async Task<ServiceRequest> AssignRequestAsync(Guid id, Guid assigneeId, string assigneeName, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.ServiceRequests
            .Include(r => r.Service)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException();

        entity.AssignedToId = assigneeId;
        entity.AssignedToName = assigneeName;
        entity.AssignedAt = DateTime.UtcNow;

        if (entity.Status == ServiceRequestStatusEnum.Submitted || entity.Status == ServiceRequestStatusEnum.Approved)
            entity.Status = ServiceRequestStatusEnum.InProgress;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapRequest(entity);
    }

    public async Task CancelRequestAsync(Guid id, string reason, Guid userId, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.ServiceRequests.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new KeyNotFoundException();

        var oldStatus = entity.Status;
        entity.Status = ServiceRequestStatusEnum.Cancelled;
        entity.CancelledAt = DateTime.UtcNow;

        _dbContext.RequestStatusHistory.Add(new RequestStatusHistoryEntity
        {
            RequestId = id,
            FromStatus = oldStatus,
            ToStatus = ServiceRequestStatusEnum.Cancelled,
            ChangedById = userId,
            ChangedByName = "User",
            Reason = reason
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Approvals
    public async Task<(List<ServiceRequest> Requests, int TotalCount)> GetPendingApprovalsAsync(Guid approverId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var requestIds = await _dbContext.RequestApprovals
            .Where(a => a.ApproverId == approverId && a.Status == ApprovalStatusEnum.Pending)
            .Select(a => a.RequestId)
            .ToListAsync(cancellationToken);

        var query = _dbContext.ServiceRequests
            .Include(r => r.Service)
            .Include(r => r.Approvals)
            .Where(r => requestIds.Contains(r.Id) && r.Status == ServiceRequestStatusEnum.PendingApproval);

        var total = await query.CountAsync(cancellationToken);
        var entities = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (entities.Select(MapRequest).ToList(), total);
    }

    public async Task<ServiceRequest> ProcessApprovalAsync(Guid requestId, ApprovalDecisionRequest decision, Guid approverId, string approverName, CancellationToken cancellationToken = default)
    {
        var request = await _dbContext.ServiceRequests
            .Include(r => r.Service)
            .Include(r => r.Approvals)
            .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken)
            ?? throw new KeyNotFoundException();

        var approval = request.Approvals.FirstOrDefault(a => a.ApproverId == approverId && a.Status == ApprovalStatusEnum.Pending)
            ?? throw new InvalidOperationException("No pending approval found for this user");

        approval.Status = (ApprovalStatusEnum)decision.Decision;
        approval.Comments = decision.Comments;
        approval.ActionedAt = DateTime.UtcNow;
        approval.ApproverName = approverName;

        // Update request status based on approval
        if (decision.Decision == ApprovalStatus.Rejected)
        {
            request.Status = ServiceRequestStatusEnum.Rejected;
        }
        else if (decision.Decision == ApprovalStatus.Approved)
        {
            if (request.Approvals.All(a => a.Status == ApprovalStatusEnum.Approved))
                request.Status = ServiceRequestStatusEnum.Approved;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapRequest(request);
    }

    // Comments
    public async Task<RequestComment> AddCommentAsync(Guid requestId, AddCommentRequest request, Guid userId, string userName, CancellationToken cancellationToken = default)
    {
        var serviceRequest = await _dbContext.ServiceRequests.FindAsync(new object[] { requestId }, cancellationToken)
            ?? throw new KeyNotFoundException();

        var entity = new RequestCommentEntity
        {
            RequestId = requestId,
            AuthorId = userId,
            AuthorName = userName,
            Content = request.Content,
            IsInternal = request.IsInternal
        };

        _dbContext.RequestComments.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new RequestComment
        {
            Id = entity.Id,
            RequestId = requestId,
            AuthorId = userId,
            AuthorName = userName,
            Content = request.Content,
            IsInternal = request.IsInternal,
            CreatedAt = entity.CreatedAt
        };
    }

    public async Task<List<RequestComment>> GetCommentsAsync(Guid requestId, bool includeInternal, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.RequestComments.Where(c => c.RequestId == requestId);
        if (!includeInternal)
            query = query.Where(c => !c.IsInternal);

        var entities = await query.OrderByDescending(c => c.CreatedAt).ToListAsync(cancellationToken);

        return entities.Select(e => new RequestComment
        {
            Id = e.Id,
            RequestId = e.RequestId,
            AuthorId = e.AuthorId,
            AuthorName = e.AuthorName,
            Content = e.Content,
            IsInternal = e.IsInternal,
            CreatedAt = e.CreatedAt
        }).ToList();
    }

    // SLA
    public async Task<SlaReport> GetSlaReportAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ServiceRequests
            .Include(r => r.Service)
            .Where(r => r.Status == ServiceRequestStatusEnum.Completed);

        if (fromDate.HasValue)
            query = query.Where(r => r.SubmittedAt >= fromDate.Value);
        if (toDate.HasValue)
            query = query.Where(r => r.SubmittedAt <= toDate.Value);

        var requests = await query.ToListAsync(cancellationToken);

        return new SlaReport
        {
            TotalRequests = requests.Count,
            OnTimeRequests = requests.Count(r => !r.IsSlaBreached),
            BreachedRequests = requests.Count(r => r.IsSlaBreached),
            ComplianceRate = requests.Count > 0 ? (double)requests.Count(r => !r.IsSlaBreached) / requests.Count * 100 : 0,
            Breaches = requests.Where(r => r.IsSlaBreached).Select(r => new SlaBreachDetail
            {
                RequestId = r.Id,
                RequestNumber = r.RequestNumber,
                ServiceName = r.Service?.Name ?? "",
                Deadline = r.SlaDeadline,
                BreachDuration = (r.CompletedAt ?? DateTime.UtcNow) - r.SlaDeadline
            }).ToList()
        };
    }

    public async Task<List<ServiceRequest>> GetSlaBreachesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var entities = await _dbContext.ServiceRequests
            .Include(r => r.Service)
            .Where(r => r.Status != ServiceRequestStatusEnum.Completed &&
                        r.Status != ServiceRequestStatusEnum.Cancelled &&
                        now > r.SlaDeadline)
            .ToListAsync(cancellationToken);

        return entities.Select(MapRequest).ToList();
    }

    // Dashboard
    public async Task<ServiceCatalogDashboard> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        var totalServices = await _dbContext.CatalogServices.CountAsync(cancellationToken);
        var activeServices = await _dbContext.CatalogServices.CountAsync(s => s.IsActive, cancellationToken);
        var totalCategories = await _dbContext.ServiceCategories.CountAsync(cancellationToken);
        var totalRequests = await _dbContext.ServiceRequests.CountAsync(cancellationToken);
        var pendingRequests = await _dbContext.ServiceRequests.CountAsync(r => r.Status == ServiceRequestStatusEnum.PendingApproval, cancellationToken);
        var inProgressRequests = await _dbContext.ServiceRequests.CountAsync(r => r.Status == ServiceRequestStatusEnum.InProgress, cancellationToken);
        var completedThisMonth = await _dbContext.ServiceRequests.CountAsync(r => r.Status == ServiceRequestStatusEnum.Completed && r.CompletedAt >= startOfMonth, cancellationToken);

        var completedRequests = await _dbContext.ServiceRequests
            .Where(r => r.Status == ServiceRequestStatusEnum.Completed)
            .ToListAsync(cancellationToken);

        var slaCompliance = completedRequests.Any()
            ? completedRequests.Count(r => !r.IsSlaBreached) * 100.0 / completedRequests.Count
            : 100;

        var avgCompletion = completedRequests.Any(r => r.CompletedAt.HasValue)
            ? completedRequests.Where(r => r.CompletedAt.HasValue).Average(r => (r.CompletedAt!.Value - r.SubmittedAt).TotalDays)
            : 0;

        var recentRequests = await _dbContext.ServiceRequests
            .Include(r => r.Service)
            .OrderByDescending(r => r.SubmittedAt)
            .Take(10)
            .ToListAsync(cancellationToken);

        var topServices = await _dbContext.CatalogServices
            .Include(s => s.Requests)
            .OrderByDescending(s => s.Requests.Count)
            .Take(5)
            .ToListAsync(cancellationToken);

        return new ServiceCatalogDashboard
        {
            TotalServices = totalServices,
            ActiveServices = activeServices,
            TotalCategories = totalCategories,
            TotalRequests = totalRequests,
            PendingRequests = pendingRequests,
            InProgressRequests = inProgressRequests,
            CompletedThisMonth = completedThisMonth,
            SlaComplianceRate = slaCompliance,
            AverageCompletionDays = avgCompletion,
            RecentRequests = recentRequests.Select(r => new ServiceRequestSummary
            {
                Id = r.Id,
                RequestNumber = r.RequestNumber,
                ServiceName = r.Service?.Name ?? "",
                RequesterName = r.RequesterName,
                Status = (RequestStatus)r.Status,
                SubmittedAt = r.SubmittedAt,
                IsSlaBreached = r.IsSlaBreached
            }).ToList(),
            TopServices = topServices.Select(s => new ServiceUsageStats
            {
                ServiceId = s.Id,
                ServiceName = s.Name,
                RequestCount = s.Requests.Count,
                AverageCompletionDays = 0
            }).ToList()
        };
    }

    // Mapping helpers
    private static ServiceCategory MapCategory(ServiceCategoryEntity entity)
    {
        return new ServiceCategory
        {
            Id = entity.Id,
            Name = entity.Name,
            NameAr = entity.NameAr,
            Description = entity.Description,
            DescriptionAr = entity.DescriptionAr,
            Icon = entity.Icon,
            ParentCategoryId = entity.ParentCategoryId,
            SortOrder = entity.SortOrder,
            IsActive = entity.IsActive
        };
    }

    private static CatalogService MapService(CatalogServiceEntity entity, string categoryName)
    {
        return new CatalogService
        {
            Id = entity.Id,
            CategoryId = entity.CategoryId,
            CategoryName = categoryName,
            Name = entity.Name,
            NameAr = entity.NameAr,
            Description = entity.Description,
            DescriptionAr = entity.DescriptionAr,
            Icon = entity.Icon,
            IsActive = entity.IsActive,
            RequiresApproval = entity.RequiresApproval,
            EstimatedDays = entity.EstimatedDays,
            Sla = new ServiceSlaConfig
            {
                ResponseTimeHours = entity.SlaResponseTimeHours,
                ResolutionTimeDays = entity.SlaResolutionTimeDays,
                NotifyOnBreach = entity.SlaNotifyOnBreach,
                EscalationEmails = !string.IsNullOrEmpty(entity.SlaEscalationEmailsJson)
                    ? JsonSerializer.Deserialize<List<string>>(entity.SlaEscalationEmailsJson) ?? new List<string>()
                    : new List<string>()
            },
            Fields = !string.IsNullOrEmpty(entity.FieldsJson)
                ? JsonSerializer.Deserialize<List<ServiceField>>(entity.FieldsJson) ?? new List<ServiceField>()
                : new List<ServiceField>(),
            ApproverIds = !string.IsNullOrEmpty(entity.ApproverIdsJson)
                ? JsonSerializer.Deserialize<List<Guid>>(entity.ApproverIdsJson) ?? new List<Guid>()
                : new List<Guid>(),
            CreatedAt = entity.CreatedAt,
            TotalRequests = entity.Requests?.Count ?? 0
        };
    }

    private static ServiceRequest MapRequest(ServiceRequestEntity entity)
    {
        return new ServiceRequest
        {
            Id = entity.Id,
            RequestNumber = entity.RequestNumber,
            ServiceId = entity.ServiceId,
            ServiceName = entity.Service?.Name ?? "",
            ServiceNameAr = entity.Service?.NameAr ?? "",
            RequesterId = entity.RequesterId,
            RequesterName = entity.RequesterName,
            RequesterEmail = entity.RequesterEmail,
            RequesterDepartment = entity.RequesterDepartment,
            Status = (RequestStatus)entity.Status,
            Priority = (RequestPriority)entity.Priority,
            FormData = !string.IsNullOrEmpty(entity.FormDataJson)
                ? JsonSerializer.Deserialize<Dictionary<string, object>>(entity.FormDataJson) ?? new Dictionary<string, object>()
                : new Dictionary<string, object>(),
            Notes = entity.Notes,
            SubmittedAt = entity.SubmittedAt,
            AssignedAt = entity.AssignedAt,
            CompletedAt = entity.CompletedAt,
            CancelledAt = entity.CancelledAt,
            SlaDeadline = entity.SlaDeadline,
            IsSlaBreached = entity.IsSlaBreached,
            AssignedToId = entity.AssignedToId,
            AssignedToName = entity.AssignedToName,
            Approvals = entity.Approvals?.Select(a => new RequestApproval
            {
                Id = a.Id,
                RequestId = a.RequestId,
                ApproverId = a.ApproverId,
                ApproverName = a.ApproverName,
                ApprovalOrder = a.ApprovalOrder,
                Status = (ApprovalStatus)a.Status,
                Comments = a.Comments,
                ActionedAt = a.ActionedAt
            }).ToList() ?? new List<RequestApproval>(),
            Comments = entity.Comments?.Select(c => new RequestComment
            {
                Id = c.Id,
                RequestId = c.RequestId,
                AuthorId = c.AuthorId,
                AuthorName = c.AuthorName,
                Content = c.Content,
                IsInternal = c.IsInternal,
                CreatedAt = c.CreatedAt
            }).ToList() ?? new List<RequestComment>(),
            StatusHistory = entity.StatusHistory?.Select(h => new RequestStatusHistory
            {
                Id = h.Id,
                FromStatus = (RequestStatus)h.FromStatus,
                ToStatus = (RequestStatus)h.ToStatus,
                ChangedById = h.ChangedById,
                ChangedByName = h.ChangedByName,
                Reason = h.Reason,
                ChangedAt = h.CreatedAt
            }).ToList() ?? new List<RequestStatusHistory>()
        };
    }
}
