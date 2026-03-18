using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Application.Services;

namespace AFC27.KMS.Admin.Presentation.Controllers;

/// <summary>
/// Controller for compliance dashboard and reporting.
/// </summary>
[ApiController]
[Route("api/admin/compliance")]
[Authorize(Policy = "CanManageUsers")]
public class ComplianceController : ControllerBase
{
    private readonly IComplianceService _complianceService;

    public ComplianceController(IComplianceService complianceService)
    {
        _complianceService = complianceService;
    }

    /// <summary>
    /// Get the full compliance dashboard.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ComplianceDashboardDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ComplianceDashboardDto>> GetDashboard(
        CancellationToken cancellationToken)
    {
        var dashboard = await _complianceService.GetDashboardAsync(cancellationToken);
        return Ok(dashboard);
    }

    /// <summary>
    /// Get policy compliance details.
    /// </summary>
    [HttpGet("policies")]
    [ProducesResponseType(typeof(PolicyComplianceDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PolicyComplianceDto>> GetPolicyCompliance(
        CancellationToken cancellationToken)
    {
        var result = await _complianceService.GetPolicyComplianceAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get content review status.
    /// </summary>
    [HttpGet("content-reviews")]
    [ProducesResponseType(typeof(ContentReviewStatusDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ContentReviewStatusDto>> GetContentReviewStatus(
        CancellationToken cancellationToken)
    {
        var result = await _complianceService.GetContentReviewStatusAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get access audit summary.
    /// </summary>
    [HttpGet("access-audits")]
    [ProducesResponseType(typeof(AccessAuditSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AccessAuditSummaryDto>> GetAccessAuditSummary(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken)
    {
        var result = await _complianceService.GetAccessAuditSummaryAsync(from, to, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get data retention status.
    /// </summary>
    [HttpGet("data-retention")]
    [ProducesResponseType(typeof(DataRetentionStatusDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DataRetentionStatusDto>> GetDataRetentionStatus(
        CancellationToken cancellationToken)
    {
        var result = await _complianceService.GetDataRetentionStatusAsync(cancellationToken);
        return Ok(result);
    }
}
