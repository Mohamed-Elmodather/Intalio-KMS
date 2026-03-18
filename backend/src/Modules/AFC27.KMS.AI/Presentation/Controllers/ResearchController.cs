using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AFC27.KMS.AI.Application.DTOs;
using AFC27.KMS.AI.Application.Services;

namespace AFC27.KMS.AI.Presentation.Controllers;

/// <summary>
/// API controller for multi-step AI research.
/// </summary>
[ApiController]
[Route("api/ai/research")]
[Authorize]
public class ResearchController : ControllerBase
{
    private readonly IResearchService _researchService;
    private readonly ILogger<ResearchController> _logger;

    public ResearchController(
        IResearchService researchService,
        ILogger<ResearchController> logger)
    {
        _researchService = researchService;
        _logger = logger;
    }

    /// <summary>
    /// Execute a multi-step AI research query.
    /// Searches multiple sources, synthesizes findings, and produces a structured report.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ResearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Research(
        [FromBody] ResearchRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
            return BadRequest(new { error = "Question is required" });

        var validDepths = new[] { "quick", "standard", "deep" };
        if (!validDepths.Contains(request.Depth?.ToLowerInvariant()))
            return BadRequest(new { error = "Depth must be 'quick', 'standard', or 'deep'" });

        _logger.LogInformation(
            "Research request: depth={Depth}, question='{Question}'",
            request.Depth, request.Question);

        var response = await _researchService.ResearchAsync(request, cancellationToken);

        return Ok(response);
    }
}
