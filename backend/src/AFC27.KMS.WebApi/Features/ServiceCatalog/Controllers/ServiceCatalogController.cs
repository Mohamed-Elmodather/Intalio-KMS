using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.WebApi.Features.ServiceCatalog.Models;
using AFC27.KMS.WebApi.Features.ServiceCatalog.Services;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.WebApi.Features.ServiceCatalog.Controllers;

/// <summary>
/// Controller for service catalog and self-service portal
/// </summary>
[ApiController]
[Route("api/services")]
[Authorize]
public class ServiceCatalogController : ControllerBase
{
    private readonly IServiceCatalogService _catalogService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<ServiceCatalogController> _logger;

    public ServiceCatalogController(
        IServiceCatalogService catalogService,
        ICurrentUser currentUser,
        ILogger<ServiceCatalogController> logger)
    {
        _catalogService = catalogService;
        _currentUser = currentUser;
        _logger = logger;
    }

    // Categories
    [HttpGet("categories")]
    public async Task<ActionResult<List<ServiceCategory>>> GetCategories(
        [FromQuery] bool includeServices = false,
        CancellationToken cancellationToken = default)
    {
        var categories = await _catalogService.GetCategoriesAsync(includeServices, cancellationToken);
        return Ok(categories);
    }

    [HttpGet("categories/{id:guid}")]
    public async Task<ActionResult<ServiceCategory>> GetCategory(Guid id, CancellationToken cancellationToken)
    {
        var category = await _catalogService.GetCategoryAsync(id, cancellationToken);
        return category == null ? NotFound() : Ok(category);
    }

    [HttpPost("categories")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<ServiceCategory>> CreateCategory(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var category = await _catalogService.CreateCategoryAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    [HttpPut("categories/{id:guid}")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<ServiceCategory>> UpdateCategory(
        Guid id,
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var category = await _catalogService.UpdateCategoryAsync(id, request, cancellationToken);
            return Ok(category);
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpDelete("categories/{id:guid}")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        await _catalogService.DeleteCategoryAsync(id, cancellationToken);
        return NoContent();
    }

    // Catalog Services
    [HttpGet("catalog")]
    public async Task<ActionResult<object>> GetCatalog(
        [FromQuery] Guid? categoryId,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (services, total) = await _catalogService.GetServicesAsync(categoryId, search, page, pageSize, cancellationToken);
        return Ok(new { data = services, pagination = new { page, pageSize, totalCount = total } });
    }

    [HttpGet("catalog/{id:guid}")]
    public async Task<ActionResult<CatalogService>> GetService(Guid id, CancellationToken cancellationToken)
    {
        var service = await _catalogService.GetServiceAsync(id, cancellationToken);
        return service == null ? NotFound() : Ok(service);
    }

    [HttpPost("catalog")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<CatalogService>> CreateService(
        [FromBody] CreateServiceRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var service = await _catalogService.CreateServiceAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetService), new { id = service.Id }, service);
        }
        catch (KeyNotFoundException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("catalog/{id:guid}")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<CatalogService>> UpdateService(
        Guid id,
        [FromBody] CreateServiceRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var service = await _catalogService.UpdateServiceAsync(id, request, cancellationToken);
            return Ok(service);
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpDelete("catalog/{id:guid}")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult> DeleteService(Guid id, CancellationToken cancellationToken)
    {
        await _catalogService.DeleteServiceAsync(id, cancellationToken);
        return NoContent();
    }

    // Service Requests
    [HttpPost("{serviceId:guid}/request")]
    public async Task<ActionResult<ServiceRequest>> SubmitRequest(
        Guid serviceId,
        [FromBody] SubmitRequestRequest request,
        CancellationToken cancellationToken)
    {
        request.ServiceId = serviceId;
        try
        {
            var serviceRequest = await _catalogService.SubmitRequestAsync(
                request,
                _currentUser.UserId ?? Guid.Empty,
                _currentUser.DisplayName ?? "Unknown",
                _currentUser.Email ?? "",
                cancellationToken);
            return CreatedAtAction(nameof(GetRequest), new { id = serviceRequest.Id }, serviceRequest);
        }
        catch (KeyNotFoundException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpGet("requests/{id:guid}")]
    public async Task<ActionResult<ServiceRequest>> GetRequest(Guid id, CancellationToken cancellationToken)
    {
        var request = await _catalogService.GetRequestAsync(id, cancellationToken);
        return request == null ? NotFound() : Ok(request);
    }

    [HttpGet("requests/number/{requestNumber}")]
    public async Task<ActionResult<ServiceRequest>> GetRequestByNumber(string requestNumber, CancellationToken cancellationToken)
    {
        var request = await _catalogService.GetRequestByNumberAsync(requestNumber, cancellationToken);
        return request == null ? NotFound() : Ok(request);
    }

    [HttpGet("my-requests")]
    public async Task<ActionResult<object>> GetMyRequests(
        [FromQuery] RequestStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (requests, total) = await _catalogService.GetMyRequestsAsync(
            _currentUser.UserId ?? Guid.Empty, status, page, pageSize, cancellationToken);
        return Ok(new { data = requests, pagination = new { page, pageSize, totalCount = total } });
    }

    [HttpGet("requests/pending")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<object>> GetPendingRequests(
        [FromQuery] Guid? assigneeId,
        [FromQuery] RequestStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (requests, total) = await _catalogService.GetPendingRequestsAsync(assigneeId, status, page, pageSize, cancellationToken);
        return Ok(new { data = requests, pagination = new { page, pageSize, totalCount = total } });
    }

    [HttpPut("requests/{id:guid}/status")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<ServiceRequest>> UpdateRequestStatus(
        Guid id,
        [FromBody] UpdateRequestStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var serviceRequest = await _catalogService.UpdateRequestStatusAsync(
                id, request, _currentUser.UserId ?? Guid.Empty, _currentUser.DisplayName ?? "Unknown", cancellationToken);
            return Ok(serviceRequest);
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPut("requests/{id:guid}/assign")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<ServiceRequest>> AssignRequest(
        Guid id,
        [FromBody] AssignRequestRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var serviceRequest = await _catalogService.AssignRequestAsync(id, request.AssigneeId, request.AssigneeName, cancellationToken);
            return Ok(serviceRequest);
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPost("requests/{id:guid}/cancel")]
    public async Task<ActionResult> CancelRequest(
        Guid id,
        [FromBody] CancelRequestRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _catalogService.CancelRequestAsync(id, request.Reason, _currentUser.UserId ?? Guid.Empty, cancellationToken);
            return Ok(new { message = "Request cancelled" });
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    // Approvals
    [HttpGet("approvals/pending")]
    public async Task<ActionResult<object>> GetPendingApprovals(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (requests, total) = await _catalogService.GetPendingApprovalsAsync(
            _currentUser.UserId ?? Guid.Empty, page, pageSize, cancellationToken);
        return Ok(new { data = requests, pagination = new { page, pageSize, totalCount = total } });
    }

    [HttpPost("requests/{id:guid}/approve")]
    public async Task<ActionResult<ServiceRequest>> ProcessApproval(
        Guid id,
        [FromBody] ApprovalDecisionRequest decision,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = await _catalogService.ProcessApprovalAsync(
                id, decision, _currentUser.UserId ?? Guid.Empty, _currentUser.DisplayName ?? "Unknown", cancellationToken);
            return Ok(request);
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    // Comments
    [HttpPost("requests/{id:guid}/comments")]
    public async Task<ActionResult<RequestComment>> AddComment(
        Guid id,
        [FromBody] AddCommentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var comment = await _catalogService.AddCommentAsync(
                id, request, _currentUser.UserId ?? Guid.Empty, _currentUser.DisplayName ?? "Unknown", cancellationToken);
            return Ok(comment);
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpGet("requests/{id:guid}/comments")]
    public async Task<ActionResult<List<RequestComment>>> GetComments(
        Guid id,
        [FromQuery] bool includeInternal = false,
        CancellationToken cancellationToken = default)
    {
        var comments = await _catalogService.GetCommentsAsync(id, includeInternal, cancellationToken);
        return Ok(comments);
    }

    // SLA
    [HttpGet("sla/report")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<SlaReport>> GetSlaReport(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        var report = await _catalogService.GetSlaReportAsync(fromDate, toDate, cancellationToken);
        return Ok(report);
    }

    [HttpGet("sla/breaches")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<List<ServiceRequest>>> GetSlaBreaches(CancellationToken cancellationToken)
    {
        var breaches = await _catalogService.GetSlaBreachesAsync(cancellationToken);
        return Ok(breaches);
    }

    // Dashboard
    [HttpGet("dashboard")]
    public async Task<ActionResult<ServiceCatalogDashboard>> GetDashboard(CancellationToken cancellationToken)
    {
        var dashboard = await _catalogService.GetDashboardAsync(cancellationToken);
        return Ok(dashboard);
    }
}

// Request DTOs
public class AssignRequestRequest
{
    public Guid AssigneeId { get; set; }
    public string AssigneeName { get; set; } = string.Empty;
}

public class CancelRequestRequest
{
    public string Reason { get; set; } = string.Empty;
}
