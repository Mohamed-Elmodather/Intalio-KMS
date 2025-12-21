using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.WebApi.Features.KpiManagement.Models;
using AFC27.KMS.WebApi.Features.KpiManagement.Services;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.WebApi.Features.KpiManagement.Controllers;

/// <summary>
/// Controller for KPI management operations
/// </summary>
[ApiController]
[Route("api/kpis")]
[Authorize]
public class KpiController : ControllerBase
{
    private readonly IKpiService _kpiService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<KpiController> _logger;

    public KpiController(
        IKpiService kpiService,
        ICurrentUser currentUser,
        ILogger<KpiController> logger)
    {
        _kpiService = kpiService;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <summary>
    /// Gets all KPI definitions
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<KpiDefinition>>> GetKpis(
        [FromQuery] string? category,
        [FromQuery] KpiScope? scope,
        CancellationToken cancellationToken)
    {
        var kpis = await _kpiService.GetKpisAsync(category, scope, true, cancellationToken);
        return Ok(kpis);
    }

    /// <summary>
    /// Gets a specific KPI definition
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<KpiDefinition>> GetKpi(
        Guid id,
        CancellationToken cancellationToken)
    {
        var kpi = await _kpiService.GetKpiAsync(id, cancellationToken);
        if (kpi == null)
            return NotFound(new { message = $"KPI {id} not found" });

        return Ok(kpi);
    }

    /// <summary>
    /// Creates a new KPI definition
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<KpiDefinition>> CreateKpi(
        [FromBody] CreateKpiRequest request,
        CancellationToken cancellationToken)
    {
        var kpi = await _kpiService.CreateKpiAsync(
            request,
            _currentUser.UserId ?? Guid.Empty,
            cancellationToken);

        return CreatedAtAction(nameof(GetKpi), new { id = kpi.Id }, kpi);
    }

    /// <summary>
    /// Updates a KPI definition
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<KpiDefinition>> UpdateKpi(
        Guid id,
        [FromBody] CreateKpiRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var kpi = await _kpiService.UpdateKpiAsync(id, request, cancellationToken);
            return Ok(kpi);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"KPI {id} not found" });
        }
    }

    /// <summary>
    /// Records a KPI value
    /// </summary>
    [HttpPost("values")]
    public async Task<ActionResult<KpiValue>> RecordValue(
        [FromBody] RecordKpiValueRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var value = await _kpiService.RecordValueAsync(
                request,
                _currentUser.UserId ?? Guid.Empty,
                cancellationToken);

            return Ok(value);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"KPI {request.KpiDefinitionId} not found" });
        }
    }

    /// <summary>
    /// Gets KPI values history
    /// </summary>
    [HttpGet("{id:guid}/values")]
    public async Task<ActionResult<List<KpiValue>>> GetValues(
        Guid id,
        [FromQuery] Guid? userId,
        [FromQuery] Guid? teamId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var values = await _kpiService.GetValuesAsync(
            id, userId, teamId, fromDate, toDate, cancellationToken);

        return Ok(values);
    }

    /// <summary>
    /// Assigns a KPI to a user or team
    /// </summary>
    [HttpPost("assignments")]
    [Authorize(Policy = "CanManageUsers")]
    public async Task<ActionResult<KpiAssignment>> AssignKpi(
        [FromBody] AssignKpiRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var assignment = await _kpiService.AssignKpiAsync(
                request,
                _currentUser.UserId ?? Guid.Empty,
                cancellationToken);

            return Ok(assignment);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"KPI {request.KpiDefinitionId} not found" });
        }
    }

    /// <summary>
    /// Gets my KPI assignments
    /// </summary>
    [HttpGet("my-assignments")]
    public async Task<ActionResult<List<KpiAssignment>>> GetMyAssignments(
        CancellationToken cancellationToken)
    {
        var assignments = await _kpiService.GetUserAssignmentsAsync(
            _currentUser.UserId ?? Guid.Empty,
            true,
            cancellationToken);

        return Ok(assignments);
    }

    /// <summary>
    /// Gets personal KPI summary
    /// </summary>
    [HttpGet("my-summary")]
    public async Task<ActionResult<PersonalKpiSummary>> GetPersonalSummary(
        CancellationToken cancellationToken)
    {
        var summary = await _kpiService.GetPersonalSummaryAsync(
            _currentUser.UserId ?? Guid.Empty,
            cancellationToken);

        return Ok(summary);
    }

    /// <summary>
    /// Gets team KPI summary
    /// </summary>
    [HttpGet("teams/{teamId:guid}/summary")]
    public async Task<ActionResult<TeamKpiSummary>> GetTeamSummary(
        Guid teamId,
        CancellationToken cancellationToken)
    {
        var summary = await _kpiService.GetTeamSummaryAsync(teamId, cancellationToken);
        return Ok(summary);
    }

    /// <summary>
    /// Gets dashboard cards for user or team
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult<List<KpiDashboardCard>>> GetDashboard(
        [FromQuery] Guid? teamId,
        CancellationToken cancellationToken)
    {
        var userId = teamId.HasValue ? null : _currentUser.UserId;
        var cards = await _kpiService.GetDashboardCardsAsync(userId, teamId, cancellationToken);
        return Ok(cards);
    }

    /// <summary>
    /// Generates KPI report
    /// </summary>
    [HttpGet("report")]
    public async Task<ActionResult<KpiReport>> GenerateReport(
        [FromQuery] Guid? teamId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var userId = teamId.HasValue ? null : _currentUser.UserId;
        var report = await _kpiService.GenerateReportAsync(
            userId, teamId, fromDate, toDate, cancellationToken);

        return Ok(report);
    }

    /// <summary>
    /// Gets KPI categories
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<List<string>>> GetCategories(
        CancellationToken cancellationToken)
    {
        var categories = await _kpiService.GetCategoriesAsync(cancellationToken);
        return Ok(categories);
    }
}
