using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.WebApi.Features.Polling.Models;
using AFC27.KMS.WebApi.Features.Polling.Services;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.WebApi.Features.Polling.Controllers;

/// <summary>
/// Controller for polling and voting operations
/// </summary>
[ApiController]
[Route("api/polls")]
[Authorize]
public class PollingController : ControllerBase
{
    private readonly IPollingService _pollingService;
    private readonly ICurrentUser _currentUser;

    public PollingController(IPollingService pollingService, ICurrentUser currentUser)
    {
        _pollingService = pollingService;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<ActionResult<object>> GetPolls([FromQuery] PollStatus? status, [FromQuery] string? category, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var (polls, totalCount) = await _pollingService.GetPollsAsync(status, category, _currentUser.UserId, page, pageSize, cancellationToken);
        return Ok(new { data = polls, pagination = new { page, pageSize, totalCount } });
    }

    [HttpGet("active")]
    public async Task<ActionResult<List<PollSummary>>> GetActivePolls(CancellationToken cancellationToken) =>
        Ok(await _pollingService.GetActivePollsAsync(_currentUser.UserId, cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Poll>> GetPoll(Guid id, CancellationToken cancellationToken)
    {
        var poll = await _pollingService.GetPollAsync(id, cancellationToken);
        return poll == null ? NotFound() : Ok(poll);
    }

    [HttpPost]
    public async Task<ActionResult<Poll>> CreatePoll([FromBody] CreatePollRequest request, CancellationToken cancellationToken)
    {
        var poll = await _pollingService.CreatePollAsync(request, _currentUser.UserId ?? Guid.Empty, cancellationToken);
        return CreatedAtAction(nameof(GetPoll), new { id = poll.Id }, poll);
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<ActionResult<Poll>> PublishPoll(Guid id, CancellationToken cancellationToken)
    {
        try { return Ok(await _pollingService.PublishPollAsync(id, _currentUser.UserId ?? Guid.Empty, cancellationToken)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPost("{id:guid}/close")]
    public async Task<ActionResult<Poll>> ClosePoll(Guid id, CancellationToken cancellationToken)
    {
        try { return Ok(await _pollingService.ClosePollAsync(id, _currentUser.UserId ?? Guid.Empty, cancellationToken)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPost("{id:guid}/vote")]
    public async Task<ActionResult<Vote>> CastVote(Guid id, [FromBody] CastVoteRequest request, CancellationToken cancellationToken)
    {
        request.PollId = id;
        try { return Ok(await _pollingService.CastVoteAsync(request, _currentUser.UserId, cancellationToken)); }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpGet("{id:guid}/results")]
    public async Task<ActionResult<PollResults>> GetResults(Guid id, CancellationToken cancellationToken)
    {
        try { return Ok(await _pollingService.GetResultsAsync(id, cancellationToken)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpGet("{id:guid}/my-vote")]
    public async Task<ActionResult<Vote>> GetMyVote(Guid id, CancellationToken cancellationToken)
    {
        var vote = await _pollingService.GetUserVoteAsync(id, _currentUser.UserId ?? Guid.Empty, cancellationToken);
        return vote == null ? NotFound() : Ok(vote);
    }

    [HttpGet("categories")]
    public async Task<ActionResult<List<string>>> GetCategories(CancellationToken cancellationToken) =>
        Ok(await _pollingService.GetCategoriesAsync(cancellationToken));
}
