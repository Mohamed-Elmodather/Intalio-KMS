using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.WebApi.Features.Meetings.Models;
using AFC27.KMS.WebApi.Features.Meetings.Services;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.WebApi.Features.Meetings.Controllers;

/// <summary>
/// Controller for meeting management and document linking
/// </summary>
[ApiController]
[Route("api/meetings")]
[Authorize]
public class MeetingsController : ControllerBase
{
    private readonly IMeetingLinkService _meetingService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<MeetingsController> _logger;

    public MeetingsController(
        IMeetingLinkService meetingService,
        ICurrentUser currentUser,
        ILogger<MeetingsController> logger)
    {
        _meetingService = meetingService;
        _currentUser = currentUser;
        _logger = logger;
    }

    // Meeting Links
    [HttpGet]
    public async Task<ActionResult<object>> GetMeetings(
        [FromQuery] MeetingStatus? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (meetings, total) = await _meetingService.GetMeetingLinksAsync(status, fromDate, toDate, page, pageSize, cancellationToken);
        return Ok(new { data = meetings, pagination = new { page, pageSize, totalCount = total } });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExternalMeetingLink>> GetMeeting(Guid id, CancellationToken cancellationToken)
    {
        var meeting = await _meetingService.GetMeetingLinkAsync(id, cancellationToken);
        return meeting == null ? NotFound() : Ok(meeting);
    }

    [HttpPost]
    public async Task<ActionResult<ExternalMeetingLink>> CreateMeetingLink(
        [FromBody] CreateMeetingLinkRequest request,
        CancellationToken cancellationToken)
    {
        var meeting = await _meetingService.CreateMeetingLinkAsync(request, _currentUser.UserId ?? Guid.Empty, cancellationToken);
        return CreatedAtAction(nameof(GetMeeting), new { id = meeting.Id }, meeting);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ExternalMeetingLink>> UpdateMeetingLink(
        Guid id,
        [FromBody] CreateMeetingLinkRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var meeting = await _meetingService.UpdateMeetingLinkAsync(id, request, cancellationToken);
            return Ok(meeting);
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteMeetingLink(Guid id, CancellationToken cancellationToken)
    {
        await _meetingService.DeleteMeetingLinkAsync(id, cancellationToken);
        return NoContent();
    }

    // Document Linking
    [HttpPost("{id:guid}/documents")]
    public async Task<ActionResult<MeetingDocumentLink>> LinkDocument(
        Guid id,
        [FromBody] LinkDocumentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var link = await _meetingService.LinkDocumentAsync(
                id, request, _currentUser.UserId ?? Guid.Empty, _currentUser.DisplayName ?? "Unknown", cancellationToken);
            return Ok(link);
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpGet("{id:guid}/documents")]
    public async Task<ActionResult<List<MeetingDocumentLink>>> GetLinkedDocuments(Guid id, CancellationToken cancellationToken)
    {
        var documents = await _meetingService.GetLinkedDocumentsAsync(id, cancellationToken);
        return Ok(documents);
    }

    [HttpDelete("{id:guid}/documents/{documentId:guid}")]
    public async Task<ActionResult> UnlinkDocument(Guid id, Guid documentId, CancellationToken cancellationToken)
    {
        try
        {
            await _meetingService.UnlinkDocumentAsync(id, documentId, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpGet("for-document/{documentId:guid}")]
    public async Task<ActionResult<List<MeetingSummary>>> GetMeetingsForDocument(Guid documentId, CancellationToken cancellationToken)
    {
        var meetings = await _meetingService.GetMeetingsForDocumentAsync(documentId, cancellationToken);
        return Ok(meetings);
    }

    // Agenda Items
    [HttpPost("{id:guid}/agenda")]
    public async Task<ActionResult<MeetingAgendaItem>> AddAgendaItem(
        Guid id,
        [FromBody] CreateAgendaItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _meetingService.AddAgendaItemAsync(id, request, cancellationToken);
            return Ok(item);
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpGet("{id:guid}/agenda")]
    public async Task<ActionResult<List<MeetingAgendaItem>>> GetAgendaItems(Guid id, CancellationToken cancellationToken)
    {
        var items = await _meetingService.GetAgendaItemsAsync(id, cancellationToken);
        return Ok(items);
    }

    [HttpPut("{id:guid}/agenda/{itemId:guid}")]
    public async Task<ActionResult<MeetingAgendaItem>> UpdateAgendaItem(
        Guid id,
        Guid itemId,
        [FromBody] CreateAgendaItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _meetingService.UpdateAgendaItemAsync(id, itemId, request, cancellationToken);
            return Ok(item);
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpDelete("{id:guid}/agenda/{itemId:guid}")]
    public async Task<ActionResult> DeleteAgendaItem(Guid id, Guid itemId, CancellationToken cancellationToken)
    {
        try
        {
            await _meetingService.DeleteAgendaItemAsync(id, itemId, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPost("{id:guid}/agenda/reorder")]
    public async Task<ActionResult> ReorderAgendaItems(Guid id, [FromBody] List<Guid> itemIds, CancellationToken cancellationToken)
    {
        try
        {
            await _meetingService.ReorderAgendaItemsAsync(id, itemIds, cancellationToken);
            return Ok();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    // Action Items
    [HttpPost("{id:guid}/actions")]
    public async Task<ActionResult<MeetingActionItem>> AddActionItem(
        Guid id,
        [FromBody] CreateActionItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var action = await _meetingService.AddActionItemAsync(id, request, cancellationToken);
            return Ok(action);
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpGet("{id:guid}/actions")]
    public async Task<ActionResult<List<MeetingActionItem>>> GetActionItems(Guid id, CancellationToken cancellationToken)
    {
        var actions = await _meetingService.GetActionItemsAsync(id, cancellationToken);
        return Ok(actions);
    }

    [HttpPut("{id:guid}/actions/{actionId:guid}")]
    public async Task<ActionResult<MeetingActionItem>> UpdateActionItemStatus(
        Guid id,
        Guid actionId,
        [FromBody] UpdateActionItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var action = await _meetingService.UpdateActionItemStatusAsync(id, actionId, request, cancellationToken);
            return Ok(action);
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpGet("my-actions")]
    public async Task<ActionResult<List<MeetingActionItem>>> GetMyActionItems(
        [FromQuery] ActionItemStatus? status,
        CancellationToken cancellationToken)
    {
        var email = _currentUser.Email ?? string.Empty;
        var actions = await _meetingService.GetMyActionItemsAsync(email, status, cancellationToken);
        return Ok(actions);
    }

    [HttpGet("actions/overdue")]
    public async Task<ActionResult<List<MeetingActionItem>>> GetOverdueActionItems(CancellationToken cancellationToken)
    {
        var actions = await _meetingService.GetOverdueActionItemsAsync(cancellationToken);
        return Ok(actions);
    }

    // Calendar
    [HttpGet("calendar")]
    public async Task<ActionResult<List<CalendarEvent>>> GetCalendarEvents(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken)
    {
        var events = await _meetingService.GetCalendarEventsAsync(startDate, endDate, cancellationToken);
        return Ok(events);
    }

    // Sync
    [HttpPost("sync")]
    [Authorize(Policy = "CanManageContent")]
    public async Task<ActionResult<MeetingSyncResult>> SyncMeetings(CancellationToken cancellationToken)
    {
        var result = await _meetingService.SyncFromExternalServiceAsync(cancellationToken);
        return Ok(result);
    }
}
