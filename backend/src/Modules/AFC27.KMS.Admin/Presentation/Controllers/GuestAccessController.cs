using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Application.Interfaces;

namespace AFC27.KMS.Admin.Presentation.Controllers;

/// <summary>
/// Controller for managing guest/external user access.
/// Provides endpoints for creating, listing, revoking, and validating
/// time-limited access tokens for external users.
/// </summary>
[ApiController]
[Route("api/admin/guest-access")]
[Authorize]
public class GuestAccessController : ControllerBase
{
    private readonly IGuestAccessService _guestAccessService;

    public GuestAccessController(IGuestAccessService guestAccessService)
    {
        _guestAccessService = guestAccessService;
    }

    /// <summary>
    /// Create a guest access invitation.
    /// Generates a secure token that can be shared with an external user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(GuestAccessDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GuestAccessDto>> CreateGuestAccess(
        [FromBody] CreateGuestAccessRequest request,
        CancellationToken ct)
    {
        // TODO: Replace with actual current user ID and name from claims
        var userId = Guid.Empty;
        var userName = "Current User";

        try
        {
            var result = await _guestAccessService.CreateAsync(request, userId, userName, ct);
            return CreatedAtAction(nameof(GetGuestAccess), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get a guest access entry by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GuestAccessDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuestAccessDto>> GetGuestAccess(Guid id, CancellationToken ct)
    {
        var result = await _guestAccessService.GetByIdAsync(id, ct);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// List guest access entries with optional filtering.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<GuestAccessDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<GuestAccessDto>>> ListGuestAccess(
        [FromQuery] GuestAccessFilterRequest filter,
        CancellationToken ct)
    {
        var result = await _guestAccessService.ListAsync(filter, ct);
        return Ok(result);
    }

    /// <summary>
    /// Revoke a guest access entry.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeGuestAccess(Guid id, CancellationToken ct)
    {
        // TODO: Replace with actual current user ID from claims
        var userId = Guid.Empty;

        var success = await _guestAccessService.RevokeAsync(id, userId, ct);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Extend the expiration of a guest access entry.
    /// </summary>
    [HttpPost("{id:guid}/extend")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExtendGuestAccess(
        Guid id,
        [FromBody] ExtendGuestAccessRequest request,
        CancellationToken ct)
    {
        var success = await _guestAccessService.ExtendAsync(id, request, ct);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Validate a guest access token (public endpoint for guest authentication).
    /// Returns the access details if the token is valid and not expired.
    /// </summary>
    [HttpGet("validate/{token}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GuestAccessDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GuestAccessDto>> ValidateToken(string token, CancellationToken ct)
    {
        var result = await _guestAccessService.ValidateTokenAsync(token, ct);
        if (result == null)
            return Unauthorized(new { error = "Invalid or expired access token" });

        // Record access
        await _guestAccessService.RecordAccessAsync(token, ct);

        return Ok(result);
    }

    /// <summary>
    /// Manually trigger deactivation of expired guest access entries (admin only).
    /// </summary>
    [HttpPost("cleanup")]
    [Authorize(Policy = "CanManageUsers")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult> CleanupExpired(CancellationToken ct)
    {
        var count = await _guestAccessService.DeactivateExpiredAsync(ct);
        return Ok(new { deactivatedCount = count });
    }
}
