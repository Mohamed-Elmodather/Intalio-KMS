using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Application.Interfaces;
using AFC27.KMS.Admin.Domain.Entities;

namespace AFC27.KMS.Admin.Presentation.Controllers;

/// <summary>
/// Controller for admin operations - audit logs, legal holds, quarantine, user management.
/// </summary>
[ApiController]
[Route("api/admin")]
[Authorize(Policy = "CanManageUsers")]
public class AdminController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;
    private readonly ILegalHoldService _legalHoldService;
    private readonly IQuarantineService _quarantineService;
    private readonly IUserLifecycleService _userLifecycleService;
    private readonly IImpersonationService _impersonationService;

    public AdminController(
        IAuditLogService auditLogService,
        ILegalHoldService legalHoldService,
        IQuarantineService quarantineService,
        IUserLifecycleService userLifecycleService,
        IImpersonationService impersonationService)
    {
        _auditLogService = auditLogService;
        _legalHoldService = legalHoldService;
        _quarantineService = quarantineService;
        _userLifecycleService = userLifecycleService;
        _impersonationService = impersonationService;
    }

    #region Audit Logs

    /// <summary>
    /// Search audit logs.
    /// </summary>
    [HttpPost("audit-logs/search")]
    [ProducesResponseType(typeof(PagedResult<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AuditLogDto>>> SearchAuditLogs(
        [FromBody] AuditLogSearchRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _auditLogService.SearchAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get audit history for an entity.
    /// </summary>
    [HttpGet("audit-logs/entity/{entityType}/{entityId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AuditLogDto>>> GetEntityAuditHistory(
        string entityType,
        Guid entityId,
        CancellationToken cancellationToken)
    {
        var history = await _auditLogService.GetEntityHistoryAsync(entityType, entityId, cancellationToken);
        return Ok(history);
    }

    /// <summary>
    /// Get audit history for a user.
    /// </summary>
    [HttpGet("audit-logs/user/{userId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AuditLogDto>>> GetUserAuditHistory(
        Guid userId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken cancellationToken)
    {
        var history = await _auditLogService.GetUserActivityAsync(userId, from, to, cancellationToken);
        return Ok(history);
    }

    /// <summary>
    /// Export audit logs to CSV.
    /// </summary>
    [HttpPost("audit-logs/export")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportAuditLogs(
        [FromBody] AuditLogSearchRequest request,
        CancellationToken cancellationToken)
    {
        var csvBytes = await _auditLogService.ExportToCsvAsync(request, cancellationToken);
        return File(csvBytes, "text/csv", $"audit-logs-{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
    }

    /// <summary>
    /// Get audit statistics.
    /// </summary>
    [HttpGet("audit-logs/statistics")]
    [ProducesResponseType(typeof(AuditStatisticsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuditStatisticsDto>> GetAuditStatistics(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken)
    {
        var stats = await _auditLogService.GetStatisticsAsync(from, to, cancellationToken);
        return Ok(stats);
    }

    #endregion

    #region Legal Holds

    /// <summary>
    /// Create a legal hold.
    /// </summary>
    [HttpPost("legal-holds")]
    [ProducesResponseType(typeof(LegalHoldDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<LegalHoldDto>> CreateLegalHold(
        [FromBody] CreateLegalHoldRequest request,
        CancellationToken cancellationToken)
    {
        var legalHold = await _legalHoldService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetLegalHold), new { id = legalHold.Id }, legalHold);
    }

    /// <summary>
    /// Get a legal hold by ID.
    /// </summary>
    [HttpGet("legal-holds/{id:guid}")]
    [ProducesResponseType(typeof(LegalHoldDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LegalHoldDto>> GetLegalHold(
        Guid id,
        CancellationToken cancellationToken)
    {
        var legalHold = await _legalHoldService.GetByIdAsync(id, cancellationToken);
        if (legalHold == null)
            return NotFound();
        return Ok(legalHold);
    }

    /// <summary>
    /// List all legal holds.
    /// </summary>
    [HttpGet("legal-holds")]
    [ProducesResponseType(typeof(IReadOnlyList<LegalHoldDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LegalHoldDto>>> ListLegalHolds(
        [FromQuery] bool includeReleased = false,
        CancellationToken cancellationToken = default)
    {
        var holds = await _legalHoldService.ListAsync(includeReleased, cancellationToken);
        return Ok(holds);
    }

    /// <summary>
    /// Add document to legal hold.
    /// </summary>
    [HttpPost("legal-holds/{id:guid}/documents/{documentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddDocumentToLegalHold(
        Guid id,
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var success = await _legalHoldService.AddDocumentAsync(id, documentId, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Remove document from legal hold.
    /// </summary>
    [HttpDelete("legal-holds/{id:guid}/documents/{documentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveDocumentFromLegalHold(
        Guid id,
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var success = await _legalHoldService.RemoveDocumentAsync(id, documentId, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Release a legal hold.
    /// </summary>
    [HttpPost("legal-holds/{id:guid}/release")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReleaseLegalHold(
        Guid id,
        [FromBody] ReleaseRequest? request,
        CancellationToken cancellationToken)
    {
        var success = await _legalHoldService.ReleaseAsync(id, request?.Notes, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    #endregion

    #region Quarantine

    /// <summary>
    /// Quarantine a document.
    /// </summary>
    [HttpPost("quarantine")]
    [ProducesResponseType(typeof(QuarantinedDocumentDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<QuarantinedDocumentDto>> QuarantineDocument(
        [FromBody] QuarantineDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var quarantined = await _quarantineService.QuarantineAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetQuarantinedDocument), new { id = quarantined.Id }, quarantined);
    }

    /// <summary>
    /// Get quarantined document.
    /// </summary>
    [HttpGet("quarantine/{id:guid}")]
    [ProducesResponseType(typeof(QuarantinedDocumentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuarantinedDocumentDto>> GetQuarantinedDocument(
        Guid id,
        CancellationToken cancellationToken)
    {
        var doc = await _quarantineService.GetByIdAsync(id, cancellationToken);
        if (doc == null)
            return NotFound();
        return Ok(doc);
    }

    /// <summary>
    /// List quarantined documents.
    /// </summary>
    [HttpGet("quarantine")]
    [ProducesResponseType(typeof(PagedResult<QuarantinedDocumentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<QuarantinedDocumentDto>>> ListQuarantined(
        [FromQuery] QuarantineListRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _quarantineService.ListAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Approve quarantine (delete document).
    /// </summary>
    [HttpPost("quarantine/{id:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveQuarantine(
        Guid id,
        [FromBody] ReviewRequest? request,
        CancellationToken cancellationToken)
    {
        var success = await _quarantineService.ApproveAsync(id, request?.Notes, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Reject quarantine.
    /// </summary>
    [HttpPost("quarantine/{id:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectQuarantine(
        Guid id,
        [FromBody] ReviewRequest? request,
        CancellationToken cancellationToken)
    {
        var success = await _quarantineService.RejectAsync(id, request?.Notes, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Release document from quarantine.
    /// </summary>
    [HttpPost("quarantine/{id:guid}/release")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReleaseFromQuarantine(
        Guid id,
        [FromBody] ReviewRequest? request,
        CancellationToken cancellationToken)
    {
        var success = await _quarantineService.ReleaseAsync(id, request?.Notes, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    #endregion

    #region User Lifecycle

    /// <summary>
    /// Invite a new user.
    /// </summary>
    [HttpPost("users/invite")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<UserDto>> InviteUser(
        [FromBody] InviteUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _userLifecycleService.InviteUserAsync(request, cancellationToken);
        return Created($"/api/admin/users/{user.Id}", user);
    }

    /// <summary>
    /// Bulk invite users.
    /// </summary>
    [HttpPost("users/bulk-invite")]
    [ProducesResponseType(typeof(BulkInviteResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BulkInviteResultDto>> BulkInviteUsers(
        [FromBody] BulkInviteRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _userLifecycleService.BulkInviteAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Suspend a user.
    /// </summary>
    [HttpPost("users/{userId:guid}/suspend")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SuspendUser(
        Guid userId,
        [FromBody] SuspendRequest request,
        CancellationToken cancellationToken)
    {
        var success = await _userLifecycleService.SuspendUserAsync(userId, request.Reason, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Reactivate a suspended user.
    /// </summary>
    [HttpPost("users/{userId:guid}/reactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReactivateUser(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var success = await _userLifecycleService.ReactivateUserAsync(userId, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Delete a user.
    /// </summary>
    [HttpDelete("users/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var success = await _userLifecycleService.DeleteUserAsync(userId, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Update user roles.
    /// </summary>
    [HttpPut("users/{userId:guid}/roles")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserRoles(
        Guid userId,
        [FromBody] UpdateRolesRequest request,
        CancellationToken cancellationToken)
    {
        var success = await _userLifecycleService.UpdateRolesAsync(userId, request.RoleIds, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Update user groups.
    /// </summary>
    [HttpPut("users/{userId:guid}/groups")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserGroups(
        Guid userId,
        [FromBody] UpdateGroupsRequest request,
        CancellationToken cancellationToken)
    {
        var success = await _userLifecycleService.UpdateGroupsAsync(userId, request.GroupIds, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Get user activity summary.
    /// </summary>
    [HttpGet("users/{userId:guid}/activity")]
    [ProducesResponseType(typeof(UserActivitySummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserActivitySummaryDto>> GetUserActivity(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var summary = await _userLifecycleService.GetActivitySummaryAsync(userId, cancellationToken);
        return Ok(summary);
    }

    #endregion

    #region Impersonation

    /// <summary>
    /// Start impersonating a user.
    /// </summary>
    [HttpPost("impersonation/start")]
    [ProducesResponseType(typeof(ImpersonationSessionDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ImpersonationSessionDto>> StartImpersonation(
        [FromBody] StartImpersonationRequest request,
        CancellationToken cancellationToken)
    {
        var session = await _impersonationService.StartAsync(request.TargetUserId, request.Reason, cancellationToken);
        return Created($"/api/admin/impersonation/{session.Id}", session);
    }

    /// <summary>
    /// End impersonation session.
    /// </summary>
    [HttpPost("impersonation/{sessionId:guid}/end")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EndImpersonation(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        var success = await _impersonationService.EndAsync(sessionId, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Get impersonation history.
    /// </summary>
    [HttpGet("impersonation/history")]
    [ProducesResponseType(typeof(IReadOnlyList<ImpersonationSessionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ImpersonationSessionDto>>> GetImpersonationHistory(
        [FromQuery] Guid? adminUserId,
        [FromQuery] Guid? targetUserId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken cancellationToken)
    {
        var history = await _impersonationService.GetHistoryAsync(adminUserId, targetUserId, from, to, cancellationToken);
        return Ok(history);
    }

    #endregion
}

// Request DTOs
public record ReleaseRequest { public string? Notes { get; init; } }
public record ReviewRequest { public string? Notes { get; init; } }
public record SuspendRequest { public string Reason { get; init; } = string.Empty; }
public record UpdateRolesRequest { public IReadOnlyList<Guid> RoleIds { get; init; } = Array.Empty<Guid>(); }
public record UpdateGroupsRequest { public IReadOnlyList<Guid> GroupIds { get; init; } = Array.Empty<Guid>(); }
public record StartImpersonationRequest
{
    public Guid TargetUserId { get; init; }
    public string Reason { get; init; } = string.Empty;
}
