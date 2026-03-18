using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AFC27.KMS.AI.Application.DTOs;
using AFC27.KMS.AI.Application.Services;

namespace AFC27.KMS.AI.Presentation.Controllers;

/// <summary>
/// API controller for managing and querying knowledge agents.
/// </summary>
[ApiController]
[Route("api/ai/agents")]
[Authorize]
public class KnowledgeAgentController : ControllerBase
{
    private readonly IKnowledgeAgentService _agentService;
    private readonly ILogger<KnowledgeAgentController> _logger;

    public KnowledgeAgentController(
        IKnowledgeAgentService agentService,
        ILogger<KnowledgeAgentController> logger)
    {
        _agentService = agentService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new knowledge agent.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AIAdmin")]
    [ProducesResponseType(typeof(KnowledgeAgentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateKnowledgeAgentRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { error = "Name is required" });
        if (string.IsNullOrWhiteSpace(request.SystemPrompt))
            return BadRequest(new { error = "System prompt is required" });

        var agent = await _agentService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = agent.Id }, agent);
    }

    /// <summary>
    /// Get a knowledge agent by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(KnowledgeAgentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var agent = await _agentService.GetByIdAsync(id, cancellationToken);
        if (agent == null)
            return NotFound();

        return Ok(agent);
    }

    /// <summary>
    /// List all knowledge agents.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<KnowledgeAgentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var agents = await _agentService.ListAsync(includeInactive, cancellationToken);
        return Ok(agents);
    }

    /// <summary>
    /// Update a knowledge agent.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "AIAdmin")]
    [ProducesResponseType(typeof(KnowledgeAgentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateKnowledgeAgentRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { error = "Name is required" });

        var agent = await _agentService.UpdateAsync(id, request, cancellationToken);
        if (agent == null)
            return NotFound();

        return Ok(agent);
    }

    /// <summary>
    /// Delete a knowledge agent.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AIAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var success = await _agentService.DeleteAsync(id, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Query a specific knowledge agent.
    /// </summary>
    [HttpPost("{id:guid}/query")]
    [ProducesResponseType(typeof(AgentQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Query(
        Guid id,
        [FromBody] AgentQueryRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new { error = "Message is required" });

        try
        {
            var response = await _agentService.QueryAsync(id, request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
