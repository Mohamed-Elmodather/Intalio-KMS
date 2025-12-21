using AFC27.KMS.WebApi.Features.ServiceCatalog.Models;

namespace AFC27.KMS.WebApi.Features.ServiceCatalog.Services;

/// <summary>
/// Interface for service catalog management
/// </summary>
public interface IServiceCatalogService
{
    // Categories
    Task<ServiceCategory> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<ServiceCategory?> GetCategoryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ServiceCategory>> GetCategoriesAsync(bool includeServices = false, CancellationToken cancellationToken = default);
    Task<ServiceCategory> UpdateCategoryAsync(Guid id, CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);

    // Services
    Task<CatalogService> CreateServiceAsync(CreateServiceRequest request, CancellationToken cancellationToken = default);
    Task<CatalogService?> GetServiceAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(List<CatalogService> Services, int TotalCount)> GetServicesAsync(Guid? categoryId, string? search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<CatalogService> UpdateServiceAsync(Guid id, CreateServiceRequest request, CancellationToken cancellationToken = default);
    Task DeleteServiceAsync(Guid id, CancellationToken cancellationToken = default);

    // Service Requests
    Task<ServiceRequest> SubmitRequestAsync(SubmitRequestRequest request, Guid requesterId, string requesterName, string requesterEmail, CancellationToken cancellationToken = default);
    Task<ServiceRequest?> GetRequestAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServiceRequest?> GetRequestByNumberAsync(string requestNumber, CancellationToken cancellationToken = default);
    Task<(List<ServiceRequest> Requests, int TotalCount)> GetMyRequestsAsync(Guid userId, RequestStatus? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<(List<ServiceRequest> Requests, int TotalCount)> GetPendingRequestsAsync(Guid? assigneeId, RequestStatus? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ServiceRequest> UpdateRequestStatusAsync(Guid id, UpdateRequestStatusRequest request, Guid userId, string userName, CancellationToken cancellationToken = default);
    Task<ServiceRequest> AssignRequestAsync(Guid id, Guid assigneeId, string assigneeName, CancellationToken cancellationToken = default);
    Task CancelRequestAsync(Guid id, string reason, Guid userId, CancellationToken cancellationToken = default);

    // Approvals
    Task<(List<ServiceRequest> Requests, int TotalCount)> GetPendingApprovalsAsync(Guid approverId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ServiceRequest> ProcessApprovalAsync(Guid requestId, ApprovalDecisionRequest decision, Guid approverId, string approverName, CancellationToken cancellationToken = default);

    // Comments
    Task<RequestComment> AddCommentAsync(Guid requestId, AddCommentRequest request, Guid userId, string userName, CancellationToken cancellationToken = default);
    Task<List<RequestComment>> GetCommentsAsync(Guid requestId, bool includeInternal, CancellationToken cancellationToken = default);

    // SLA
    Task<SlaReport> GetSlaReportAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default);
    Task<List<ServiceRequest>> GetSlaBreachesAsync(CancellationToken cancellationToken = default);

    // Dashboard
    Task<ServiceCatalogDashboard> GetDashboardAsync(CancellationToken cancellationToken = default);
}
