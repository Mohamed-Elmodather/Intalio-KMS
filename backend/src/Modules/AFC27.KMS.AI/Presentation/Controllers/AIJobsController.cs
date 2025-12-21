using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.AI.Application.DTOs;
using AFC27.KMS.AI.Domain.Entities;

namespace AFC27.KMS.AI.Presentation.Controllers;

/// <summary>
/// Controller for AI job management
/// </summary>
[ApiController]
[Route("api/ai/jobs")]
[Authorize]
public class AIJobsController : ControllerBase
{
    #region Job Management

    /// <summary>
    /// Get AI jobs for current user
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AIJobDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AIJobDto>>> GetJobs(
        [FromQuery] AIJobType? type = null,
        [FromQuery] AIJobStatus? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // TODO: Return jobs
        var jobs = new List<AIJobDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Type = AIJobType.DocumentSummarization,
                TypeName = "Document Summarization",
                Status = AIJobStatus.Completed,
                StatusName = "Completed",
                Provider = AIProvider.IntalioAI,
                InputPreview = "Annual Report 2024...",
                OutputPreview = "The report highlights key achievements...",
                StartedAt = DateTime.UtcNow.AddMinutes(-5),
                CompletedAt = DateTime.UtcNow.AddMinutes(-3),
                ProcessingTimeMs = 2500,
                TokensUsed = 1200,
                RequestedByName = "Ahmed Hassan",
                CreatedAt = DateTime.UtcNow.AddMinutes(-5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Type = AIJobType.AudioTranscription,
                TypeName = "Audio Transcription",
                Status = AIJobStatus.Processing,
                StatusName = "Processing",
                Provider = AIProvider.IntalioAI,
                InputPreview = "Meeting recording...",
                StartedAt = DateTime.UtcNow.AddMinutes(-2),
                RequestedByName = "Ahmed Hassan",
                CreatedAt = DateTime.UtcNow.AddMinutes(-2)
            }
        };
        return Ok(jobs);
    }

    /// <summary>
    /// Get job by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AIJobDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AIJobDto>> GetJob(Guid id)
    {
        // TODO: Return job
        return NotFound();
    }

    /// <summary>
    /// Create AI job
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AIJobDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AIJobDto>> CreateJob([FromBody] CreateAIJobRequest request)
    {
        // TODO: Create and queue job
        var job = new AIJobDto
        {
            Id = Guid.NewGuid(),
            Type = request.Type,
            TypeName = request.Type.ToString(),
            Status = AIJobStatus.Queued,
            StatusName = "Queued",
            Provider = request.Provider ?? AIProvider.IntalioAI,
            CreatedAt = DateTime.UtcNow
        };
        return Created($"/api/ai/jobs/{job.Id}", job);
    }

    /// <summary>
    /// Cancel job
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelJob(Guid id)
    {
        // TODO: Cancel job
        return NoContent();
    }

    /// <summary>
    /// Retry failed job
    /// </summary>
    [HttpPost("{id:guid}/retry")]
    [ProducesResponseType(typeof(AIJobDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AIJobDto>> RetryJob(Guid id)
    {
        // TODO: Retry job
        return Ok(new AIJobDto { Id = id, Status = AIJobStatus.Queued });
    }

    /// <summary>
    /// Delete job
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteJob(Guid id)
    {
        // TODO: Delete job
        return NoContent();
    }

    #endregion

    #region Job Types & Providers

    /// <summary>
    /// Get available job types
    /// </summary>
    [HttpGet("types")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<object>> GetJobTypes()
    {
        var types = Enum.GetValues<AIJobType>()
            .Select(t => new { Value = (int)t, Name = t.ToString() });
        return Ok(types);
    }

    /// <summary>
    /// Get available providers
    /// </summary>
    [HttpGet("providers")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<object>> GetProviders()
    {
        var providers = new[]
        {
            new { Value = "IntalioAI", Name = "Intalio AI Engine", IsDefault = true, IsAvailable = true },
            new { Value = "AzureOpenAI", Name = "Azure OpenAI", IsDefault = false, IsAvailable = true },
            new { Value = "AzureCognitiveServices", Name = "Azure Cognitive Services", IsDefault = false, IsAvailable = true }
        };
        return Ok(providers);
    }

    /// <summary>
    /// Get available models
    /// </summary>
    [HttpGet("models")]
    [ProducesResponseType(typeof(IEnumerable<AIModelInfoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AIModelInfoDto>>> GetModels(
        [FromQuery] AIJobType? jobType = null,
        [FromQuery] AIProvider? provider = null)
    {
        // TODO: Return available models
        var models = new List<AIModelInfoDto>
        {
            new()
            {
                ModelId = "intalio-transcribe-v1",
                Name = "Intalio Transcribe",
                Description = "High-accuracy transcription with Arabic support",
                Provider = "IntalioAI",
                SupportedJobType = "AudioTranscription",
                SupportedLanguages = new List<string> { "ar", "en", "fr" },
                SupportedFormats = new List<string> { "mp3", "wav", "mp4", "webm" },
                MaxTokens = null,
                IsDefault = true,
                IsActive = true
            },
            new()
            {
                ModelId = "intalio-summarize-v1",
                Name = "Intalio Summarize",
                Description = "Document and text summarization",
                Provider = "IntalioAI",
                SupportedJobType = "DocumentSummarization",
                SupportedLanguages = new List<string> { "ar", "en" },
                SupportedFormats = new List<string> { "text", "pdf", "docx" },
                MaxTokens = 4096,
                IsDefault = true,
                IsActive = true
            }
        };
        return Ok(models);
    }

    #endregion

    #region Quota & Usage

    /// <summary>
    /// Get current user's quota status
    /// </summary>
    [HttpGet("quota")]
    [ProducesResponseType(typeof(IEnumerable<AIQuotaStatusDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AIQuotaStatusDto>>> GetQuotaStatus()
    {
        // TODO: Return quota status
        var quotas = new List<AIQuotaStatusDto>
        {
            new()
            {
                JobType = AIJobType.AudioTranscription,
                Period = "Monthly",
                MaxRequests = 100,
                UsedRequests = 45,
                RemainingRequests = 55,
                UsagePercentage = 45,
                MaxMinutes = 600,
                UsedMinutes = 180,
                PeriodStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
                PeriodEnd = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1).AddDays(-1),
                IsExceeded = false
            },
            new()
            {
                JobType = AIJobType.DocumentSummarization,
                Period = "Monthly",
                MaxRequests = 500,
                UsedRequests = 120,
                RemainingRequests = 380,
                UsagePercentage = 24,
                MaxTokens = 100000,
                UsedTokens = 25000,
                PeriodStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
                PeriodEnd = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1).AddDays(-1),
                IsExceeded = false
            }
        };
        return Ok(quotas);
    }

    /// <summary>
    /// Get usage statistics
    /// </summary>
    [HttpGet("usage")]
    [ProducesResponseType(typeof(AIUsageStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AIUsageStatsDto>> GetUsageStats(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        // TODO: Return usage stats
        var stats = new AIUsageStatsDto
        {
            TotalJobs = 250,
            CompletedJobs = 235,
            FailedJobs = 10,
            PendingJobs = 5,
            TotalTokensUsed = 450000,
            TotalMinutesProcessed = 180,
            TotalCost = 125.50m,
            JobsByType = new Dictionary<string, int>
            {
                ["AudioTranscription"] = 45,
                ["DocumentSummarization"] = 120,
                ["ContentClassification"] = 50,
                ["MeetingMinutesGeneration"] = 35
            },
            JobsByProvider = new Dictionary<string, int>
            {
                ["IntalioAI"] = 200,
                ["AzureOpenAI"] = 50
            }
        };
        return Ok(stats);
    }

    #endregion
}
