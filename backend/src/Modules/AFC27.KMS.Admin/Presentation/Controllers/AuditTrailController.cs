using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Application.Services;

namespace AFC27.KMS.Admin.Presentation.Controllers;

/// <summary>
/// Controller for enhanced audit trail with rich filtering and export.
/// </summary>
[ApiController]
[Route("api/admin/audit-trail")]
[Authorize(Policy = "CanManageUsers")]
public class AuditTrailController : ControllerBase
{
    private readonly IAuditTrailService _auditTrailService;

    public AuditTrailController(IAuditTrailService auditTrailService)
    {
        _auditTrailService = auditTrailService;
    }

    /// <summary>
    /// Query audit trail with rich filtering, pagination, and sorting.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AuditTrailEntryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AuditTrailEntryDto>>> Query(
        [FromQuery] AuditTrailQueryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _auditTrailService.QueryAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get audit trail summary with aggregations for a time range.
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(AuditTrailSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuditTrailSummaryDto>> GetSummary(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken)
    {
        var result = await _auditTrailService.GetSummaryAsync(from, to, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Export audit trail entries. Supports CSV and JSON formats.
    /// </summary>
    [HttpPost("export")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> Export(
        [FromBody] AuditTrailQueryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _auditTrailService.ExportAsync(request, cancellationToken);
        return File(result.Data, result.ContentType, result.FileName);
    }
}
