using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Application.Interfaces;

namespace AFC27.KMS.Admin.Presentation.Controllers;

/// <summary>
/// Controller for managing delegated space administration.
/// Space owners can delegate specific admin capabilities to users.
/// </summary>
[ApiController]
[Route("api/admin/delegated-admins")]
[Authorize]
public class DelegatedAdminController : ControllerBase
{
    private readonly IDelegatedAdminService _delegatedAdminService;

    public DelegatedAdminController(IDelegatedAdminService delegatedAdminService)
    {
        _delegatedAdminService = delegatedAdminService;
    }

    /// <summary>
    /// Create a delegated admin assignment.
    /// Only space owners and existing admins can delegate admin rights.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(DelegatedAdminDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DelegatedAdminDto>> CreateDelegation(
        [FromBody] CreateDelegatedAdminRequest request,
        CancellationToken ct)
    {
        // TODO: Replace with actual current user ID and name from claims
        var userId = Guid.Empty;
        var userName = "Current User";

        var result = await _delegatedAdminService.CreateAsync(request, userId, userName, ct);
        if (result == null)
            return BadRequest(new { error = "Failed to create delegation. An active delegation may already exist for this user on the specified space." });

        return CreatedAtAction(nameof(GetDelegation), new { id = result.Id }, result);
    }

    /// <summary>
    /// Get a delegated admin assignment by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DelegatedAdminDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DelegatedAdminDto>> GetDelegation(Guid id, CancellationToken ct)
    {
        var result = await _delegatedAdminService.GetByIdAsync(id, ct);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// List delegated admins for a specific space.
    /// </summary>
    [HttpGet("by-space/{spaceId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<DelegatedAdminDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DelegatedAdminDto>>> GetBySpace(
        Guid spaceId,
        [FromQuery] bool includeRevoked = false,
        CancellationToken ct = default)
    {
        var result = await _delegatedAdminService.GetBySpaceAsync(spaceId, includeRevoked, ct);
        return Ok(result);
    }

    /// <summary>
    /// List delegated admin assignments for a specific user.
    /// </summary>
    [HttpGet("by-user/{userId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<DelegatedAdminDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DelegatedAdminDto>>> GetByUser(
        Guid userId,
        CancellationToken ct)
    {
        var result = await _delegatedAdminService.GetByUserAsync(userId, ct);
        return Ok(result);
    }

    /// <summary>
    /// Get delegated admin assignments for the current user.
    /// </summary>
    [HttpGet("my-delegations")]
    [ProducesResponseType(typeof(IReadOnlyList<DelegatedAdminDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DelegatedAdminDto>>> GetMyDelegations(CancellationToken ct)
    {
        // TODO: Replace with actual current user ID from claims
        var userId = Guid.Empty;

        var result = await _delegatedAdminService.GetByUserAsync(userId, ct);
        return Ok(result);
    }

    /// <summary>
    /// Update the scopes of a delegated admin assignment.
    /// </summary>
    [HttpPut("{id:guid}/scopes")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateScopes(
        Guid id,
        [FromBody] UpdateDelegatedAdminScopesRequest request,
        CancellationToken ct)
    {
        // TODO: Replace with actual current user ID from claims
        var userId = Guid.Empty;

        var success = await _delegatedAdminService.UpdateScopesAsync(id, request, userId, ct);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Revoke a delegated admin assignment.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeDelegation(Guid id, CancellationToken ct)
    {
        // TODO: Replace with actual current user ID from claims
        var userId = Guid.Empty;

        var success = await _delegatedAdminService.RevokeAsync(id, userId, ct);
        return success ? NoContent() : NotFound();
    }
}
