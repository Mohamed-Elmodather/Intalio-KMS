using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Content.Application.DTOs;
using AFC27.KMS.Content.Application.Interfaces;
using AFC27.KMS.Contracts.Common;

namespace AFC27.KMS.Content.Presentation.Controllers;

/// <summary>
/// Spaces/Teamspaces management controller.
/// </summary>
[ApiController]
[Route("api/content/spaces")]
[Authorize]
public class SpacesController : ControllerBase
{
    private readonly ISpaceService _spaceService;
    private readonly ILogger<SpacesController> _logger;

    public SpacesController(ISpaceService spaceService, ILogger<SpacesController> logger)
    {
        _spaceService = spaceService;
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of spaces.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SpaceSummaryDto>>>> GetSpaces(
        [FromQuery] SpaceFilterRequest filter, CancellationToken ct)
    {
        var result = await _spaceService.GetSpacesAsync(filter, ct);
        return Ok(ApiResponse<PagedResult<SpaceSummaryDto>>.Ok(result));
    }

    /// <summary>
    /// Get spaces the current user is a member of.
    /// </summary>
    [HttpGet("my-spaces")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<SpaceSummaryDto>>>> GetMySpaces(CancellationToken ct)
    {
        // TODO: Replace with actual current user ID from claims
        var userId = Guid.Empty;
        var result = await _spaceService.GetMySpacesAsync(userId, ct);
        return Ok(ApiResponse<IReadOnlyList<SpaceSummaryDto>>.Ok(result));
    }

    /// <summary>
    /// Get space by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<SpaceDto>>> GetSpace(Guid id, CancellationToken ct)
    {
        var space = await _spaceService.GetSpaceByIdAsync(id, ct);
        if (space == null)
            return NotFound(ApiResponse<SpaceDto>.Fail("Space not found"));

        return Ok(ApiResponse<SpaceDto>.Ok(space));
    }

    /// <summary>
    /// Create a new space.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<SpaceDto>>> CreateSpace(
        [FromBody] CreateSpaceRequest request, CancellationToken ct)
    {
        // TODO: Replace with actual current user ID and name from claims
        var userId = Guid.Empty;
        var userName = "Current User";

        var space = await _spaceService.CreateSpaceAsync(request, userId, userName, ct);
        return CreatedAtAction(nameof(GetSpace), new { id = space.Id }, ApiResponse<SpaceDto>.Ok(space));
    }

    /// <summary>
    /// Update a space.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> UpdateSpace(
        Guid id, [FromBody] UpdateSpaceRequest request, CancellationToken ct)
    {
        // TODO: Replace with actual current user ID from claims
        var userId = Guid.Empty;

        var success = await _spaceService.UpdateSpaceAsync(id, request, userId, ct);
        if (!success)
            return NotFound(ApiResponse.Fail("Space not found"));

        return Ok(ApiResponse.Ok("Space updated successfully"));
    }

    /// <summary>
    /// Delete a space (soft delete).
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> DeleteSpace(Guid id, CancellationToken ct)
    {
        // TODO: Replace with actual current user ID from claims
        var userId = Guid.Empty;

        var success = await _spaceService.DeleteSpaceAsync(id, userId, ct);
        if (!success)
            return NotFound(ApiResponse.Fail("Space not found"));

        return Ok(ApiResponse.Ok("Space deleted successfully"));
    }

    /// <summary>
    /// Archive a space.
    /// </summary>
    [HttpPost("{id:guid}/archive")]
    public async Task<ActionResult<ApiResponse>> ArchiveSpace(Guid id, CancellationToken ct)
    {
        // TODO: Replace with actual current user ID from claims
        var userId = Guid.Empty;

        var success = await _spaceService.ArchiveSpaceAsync(id, userId, ct);
        if (!success)
            return NotFound(ApiResponse.Fail("Space not found"));

        return Ok(ApiResponse.Ok("Space archived successfully"));
    }

    /// <summary>
    /// Unarchive a space.
    /// </summary>
    [HttpPost("{id:guid}/unarchive")]
    public async Task<ActionResult<ApiResponse>> UnarchiveSpace(Guid id, CancellationToken ct)
    {
        // TODO: Replace with actual current user ID from claims
        var userId = Guid.Empty;

        var success = await _spaceService.UnarchiveSpaceAsync(id, userId, ct);
        if (!success)
            return NotFound(ApiResponse.Fail("Space not found"));

        return Ok(ApiResponse.Ok("Space unarchived successfully"));
    }

    /// <summary>
    /// Get space members.
    /// </summary>
    [HttpGet("{id:guid}/members")]
    public async Task<ActionResult<ApiResponse<SpaceDto>>> GetSpaceMembers(Guid id, CancellationToken ct)
    {
        var space = await _spaceService.GetSpaceByIdAsync(id, ct);
        if (space == null)
            return NotFound(ApiResponse<SpaceDto>.Fail("Space not found"));

        return Ok(ApiResponse<SpaceDto>.Ok(space));
    }

    /// <summary>
    /// Add a member to a space.
    /// </summary>
    [HttpPost("{id:guid}/members")]
    public async Task<ActionResult<ApiResponse<SpaceMemberDto>>> AddMember(
        Guid id, [FromBody] AddSpaceMemberRequest request, CancellationToken ct)
    {
        // TODO: Replace with actual current user ID from claims
        var userId = Guid.Empty;

        var member = await _spaceService.AddMemberAsync(id, request, userId, ct);
        if (member == null)
            return BadRequest(ApiResponse<SpaceMemberDto>.Fail("Failed to add member. Space not found or user is already a member."));

        return Ok(ApiResponse<SpaceMemberDto>.Ok(member, "Member added successfully"));
    }

    /// <summary>
    /// Update a member's role in a space.
    /// </summary>
    [HttpPut("{id:guid}/members/{memberUserId:guid}")]
    public async Task<ActionResult<ApiResponse>> ChangeMemberRole(
        Guid id, Guid memberUserId, [FromBody] ChangeSpaceMemberRoleRequest request, CancellationToken ct)
    {
        // TODO: Replace with actual current user ID from claims
        var userId = Guid.Empty;

        var success = await _spaceService.ChangeMemberRoleAsync(id, memberUserId, request, userId, ct);
        if (!success)
            return NotFound(ApiResponse.Fail("Space or member not found"));

        return Ok(ApiResponse.Ok("Member role updated"));
    }

    /// <summary>
    /// Remove a member from a space.
    /// </summary>
    [HttpDelete("{id:guid}/members/{memberUserId:guid}")]
    public async Task<ActionResult<ApiResponse>> RemoveMember(
        Guid id, Guid memberUserId, CancellationToken ct)
    {
        // TODO: Replace with actual current user ID from claims
        var userId = Guid.Empty;

        var success = await _spaceService.RemoveMemberAsync(id, memberUserId, userId, ct);
        if (!success)
            return BadRequest(ApiResponse.Fail("Failed to remove member. Space/member not found or cannot remove owner."));

        return Ok(ApiResponse.Ok("Member removed successfully"));
    }

    /// <summary>
    /// Get content (articles) within a space.
    /// </summary>
    [HttpGet("{id:guid}/content")]
    public async Task<ActionResult<ApiResponse<PagedResult<ArticleSummaryDto>>>> GetSpaceContent(
        Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _spaceService.GetSpaceContentAsync(id, page, pageSize, ct);
        return Ok(ApiResponse<PagedResult<ArticleSummaryDto>>.Ok(result));
    }
}
