using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.AI.Application.DTOs;
using AFC27.KMS.AI.Domain.Entities;

namespace AFC27.KMS.AI.Presentation.Controllers;

/// <summary>
/// Admin controller for AI configuration and templates
/// </summary>
[ApiController]
[Route("api/ai/admin")]
[Authorize(Policy = "AIAdmin")]
public class AIAdminController : ControllerBase
{
    #region Prompt Templates

    /// <summary>
    /// Get all prompt templates
    /// </summary>
    [HttpGet("templates")]
    [ProducesResponseType(typeof(IEnumerable<AIPromptTemplateDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AIPromptTemplateDto>>> GetTemplates(
        [FromQuery] AIJobType? jobType = null,
        [FromQuery] bool? isActive = null)
    {
        // TODO: Return templates
        var templates = new List<AIPromptTemplateDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Key = "summarize-document",
                Name = "Document Summarization",
                Description = "Summarize documents with key points extraction",
                JobType = "DocumentSummarization",
                SystemPrompt = "You are an expert document summarizer...",
                UserPromptTemplate = "Summarize the following document:\n\n{{content}}\n\nProvide:\n1. Executive summary\n2. Key points\n3. Keywords",
                OutputFormat = "json",
                Variables = new List<PromptVariableDto>
                {
                    new() { Name = "content", Description = "Document content", Type = "Text", IsRequired = true },
                    new() { Name = "length", Description = "Summary length", Type = "Text", DefaultValue = "medium" }
                },
                IsActive = true,
                IsSystem = true,
                Version = 2,
                UsageCount = 1250,
                AverageRating = 4.5
            }
        };
        return Ok(templates);
    }

    /// <summary>
    /// Get template by ID
    /// </summary>
    [HttpGet("templates/{id:guid}")]
    [ProducesResponseType(typeof(AIPromptTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AIPromptTemplateDto>> GetTemplate(Guid id)
    {
        // TODO: Return template
        return NotFound();
    }

    /// <summary>
    /// Create prompt template
    /// </summary>
    [HttpPost("templates")]
    [ProducesResponseType(typeof(AIPromptTemplateDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<AIPromptTemplateDto>> CreateTemplate(
        [FromBody] CreatePromptTemplateRequest request)
    {
        // TODO: Create template
        var template = new AIPromptTemplateDto
        {
            Id = Guid.NewGuid(),
            Key = request.Key,
            Name = request.Name,
            Description = request.Description,
            JobType = request.JobType.ToString(),
            IsActive = true,
            Version = 1
        };
        return Created($"/api/ai/admin/templates/{template.Id}", template);
    }

    /// <summary>
    /// Update prompt template
    /// </summary>
    [HttpPut("templates/{id:guid}")]
    [ProducesResponseType(typeof(AIPromptTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AIPromptTemplateDto>> UpdateTemplate(
        Guid id,
        [FromBody] CreatePromptTemplateRequest request)
    {
        // TODO: Update template
        return Ok(new AIPromptTemplateDto { Id = id });
    }

    /// <summary>
    /// Delete prompt template
    /// </summary>
    [HttpDelete("templates/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteTemplate(Guid id)
    {
        // TODO: Delete template
        return NoContent();
    }

    /// <summary>
    /// Test prompt template
    /// </summary>
    [HttpPost("templates/{id:guid}/test")]
    [ProducesResponseType(typeof(TextGenerationResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TextGenerationResultDto>> TestTemplate(
        Guid id,
        [FromBody] ExecutePromptRequest request)
    {
        // TODO: Test template
        var result = new TextGenerationResultDto
        {
            GeneratedText = "Test output from template execution...",
            TokensUsed = 250,
            ProcessingTimeMs = 450
        };
        return Ok(result);
    }

    #endregion

    #region Model Configuration

    /// <summary>
    /// Get model configurations
    /// </summary>
    [HttpGet("models")]
    [ProducesResponseType(typeof(IEnumerable<AIModelInfoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AIModelInfoDto>>> GetModelConfigs()
    {
        // TODO: Return model configurations
        var models = new List<AIModelInfoDto>();
        return Ok(models);
    }

    /// <summary>
    /// Update model configuration
    /// </summary>
    [HttpPut("models/{modelId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateModelConfig(
        string modelId,
        [FromBody] UpdateModelConfigRequest request)
    {
        // TODO: Update model config
        return NoContent();
    }

    /// <summary>
    /// Set default model for job type
    /// </summary>
    [HttpPost("models/{modelId}/set-default")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetDefaultModel(
        string modelId,
        [FromQuery] AIJobType jobType)
    {
        // TODO: Set default model
        return NoContent();
    }

    /// <summary>
    /// Test model connectivity
    /// </summary>
    [HttpPost("models/{modelId}/test")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> TestModelConnectivity(string modelId)
    {
        // TODO: Test model connectivity
        return Ok(new { Status = "Connected", ResponseTimeMs = 125 });
    }

    #endregion

    #region Quota Management

    /// <summary>
    /// Get all quota configurations
    /// </summary>
    [HttpGet("quotas")]
    [ProducesResponseType(typeof(IEnumerable<QuotaConfigDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<QuotaConfigDto>>> GetQuotaConfigs()
    {
        // TODO: Return quota configurations
        var quotas = new List<QuotaConfigDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Default User Quota",
                JobType = AIJobType.DocumentSummarization,
                Period = "Monthly",
                MaxRequests = 500,
                MaxTokens = 100000,
                AppliesTo = "AllUsers"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Admin Unlimited",
                JobType = AIJobType.DocumentSummarization,
                Period = "Monthly",
                MaxRequests = -1,
                MaxTokens = -1,
                AppliesTo = "Role:Admin"
            }
        };
        return Ok(quotas);
    }

    /// <summary>
    /// Create quota configuration
    /// </summary>
    [HttpPost("quotas")]
    [ProducesResponseType(typeof(QuotaConfigDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<QuotaConfigDto>> CreateQuotaConfig(
        [FromBody] CreateQuotaConfigRequest request)
    {
        // TODO: Create quota config
        var quota = new QuotaConfigDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            JobType = request.JobType,
            Period = request.Period.ToString()
        };
        return Created($"/api/ai/admin/quotas/{quota.Id}", quota);
    }

    /// <summary>
    /// Update quota configuration
    /// </summary>
    [HttpPut("quotas/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateQuotaConfig(
        Guid id,
        [FromBody] CreateQuotaConfigRequest request)
    {
        // TODO: Update quota config
        return NoContent();
    }

    /// <summary>
    /// Delete quota configuration
    /// </summary>
    [HttpDelete("quotas/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteQuotaConfig(Guid id)
    {
        // TODO: Delete quota config
        return NoContent();
    }

    /// <summary>
    /// Reset user quota
    /// </summary>
    [HttpPost("quotas/reset/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResetUserQuota(Guid userId, [FromQuery] AIJobType? jobType = null)
    {
        // TODO: Reset user quota
        return NoContent();
    }

    #endregion

    #region Analytics & Reports

    /// <summary>
    /// Get AI usage analytics
    /// </summary>
    [HttpGet("analytics")]
    [ProducesResponseType(typeof(AIAnalyticsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AIAnalyticsDto>> GetAnalytics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        // TODO: Return analytics
        var analytics = new AIAnalyticsDto
        {
            PeriodStart = startDate ?? DateTime.UtcNow.AddDays(-30),
            PeriodEnd = endDate ?? DateTime.UtcNow,
            TotalJobs = 5420,
            TotalTokensUsed = 2500000,
            TotalCost = 1250.50m,
            AverageProcessingTimeMs = 850,
            SuccessRate = 97.5,
            JobsByType = new Dictionary<string, int>
            {
                ["DocumentSummarization"] = 2500,
                ["AudioTranscription"] = 1200,
                ["ContentClassification"] = 800,
                ["SemanticSearch"] = 920
            },
            TopUsers = new List<UserAIUsageDto>
            {
                new() { UserId = Guid.NewGuid(), UserName = "Ahmed Hassan", JobCount = 150, TokensUsed = 45000 },
                new() { UserId = Guid.NewGuid(), UserName = "Sara Ali", JobCount = 120, TokensUsed = 38000 }
            },
            DailyUsage = new List<DailyAIUsageDto>()
        };
        return Ok(analytics);
    }

    /// <summary>
    /// Get error report
    /// </summary>
    [HttpGet("analytics/errors")]
    [ProducesResponseType(typeof(IEnumerable<AIErrorReportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AIErrorReportDto>>> GetErrorReport(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        // TODO: Return error report
        var errors = new List<AIErrorReportDto>();
        return Ok(errors);
    }

    /// <summary>
    /// Get cost report
    /// </summary>
    [HttpGet("analytics/costs")]
    [ProducesResponseType(typeof(AICostReportDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AICostReportDto>> GetCostReport(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        // TODO: Return cost report
        var report = new AICostReportDto
        {
            TotalCost = 1250.50m,
            CostByProvider = new Dictionary<string, decimal>
            {
                ["IntalioAI"] = 950.00m,
                ["AzureOpenAI"] = 300.50m
            },
            CostByJobType = new Dictionary<string, decimal>
            {
                ["DocumentSummarization"] = 450.00m,
                ["AudioTranscription"] = 500.00m,
                ["ContentClassification"] = 300.50m
            }
        };
        return Ok(report);
    }

    #endregion

    #region Provider Configuration

    /// <summary>
    /// Get provider configuration
    /// </summary>
    [HttpGet("config/provider/{provider}")]
    [ProducesResponseType(typeof(ProviderConfigDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProviderConfigDto>> GetProviderConfig(AIProvider provider)
    {
        // TODO: Return provider config (masked secrets)
        var config = new ProviderConfigDto
        {
            Provider = provider,
            IsConfigured = true,
            IsActive = true,
            Endpoint = "https://ai.intalio.com/api/v1",
            ApiKeyConfigured = true
        };
        return Ok(config);
    }

    /// <summary>
    /// Update provider configuration
    /// </summary>
    [HttpPut("config/provider/{provider}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateProviderConfig(
        AIProvider provider,
        [FromBody] UpdateProviderConfigRequest request)
    {
        // TODO: Update provider config
        return NoContent();
    }

    /// <summary>
    /// Test provider connection
    /// </summary>
    [HttpPost("config/provider/{provider}/test")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> TestProviderConnection(AIProvider provider)
    {
        // TODO: Test provider connection
        return Ok(new { Status = "Connected", Latency = "125ms" });
    }

    #endregion
}

#region Additional DTOs

/// <summary>
/// Update model config request
/// </summary>
public class UpdateModelConfigRequest
{
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public int Priority { get; set; }
    public int? MaxTokens { get; set; }
    public double? Temperature { get; set; }
    public int? RateLimitPerMinute { get; set; }
    public Dictionary<string, object>? DefaultParameters { get; set; }
}

/// <summary>
/// Quota config DTO
/// </summary>
public class QuotaConfigDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public AIJobType JobType { get; set; }
    public string Period { get; set; } = string.Empty;
    public int MaxRequests { get; set; }
    public int? MaxTokens { get; set; }
    public int? MaxMinutes { get; set; }
    public decimal? MaxCost { get; set; }
    public string AppliesTo { get; set; } = string.Empty;
}

/// <summary>
/// Create quota config request
/// </summary>
public class CreateQuotaConfigRequest
{
    public string Name { get; set; } = string.Empty;
    public AIJobType JobType { get; set; }
    public QuotaPeriod Period { get; set; }
    public int MaxRequests { get; set; }
    public int? MaxTokens { get; set; }
    public int? MaxMinutes { get; set; }
    public decimal? MaxCost { get; set; }
    public Guid? UserId { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? Role { get; set; }
}

/// <summary>
/// AI analytics DTO
/// </summary>
public class AIAnalyticsDto
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public int TotalJobs { get; set; }
    public long TotalTokensUsed { get; set; }
    public decimal TotalCost { get; set; }
    public double AverageProcessingTimeMs { get; set; }
    public double SuccessRate { get; set; }
    public Dictionary<string, int> JobsByType { get; set; } = new();
    public List<UserAIUsageDto> TopUsers { get; set; } = new();
    public List<DailyAIUsageDto> DailyUsage { get; set; } = new();
}

/// <summary>
/// User AI usage DTO
/// </summary>
public class UserAIUsageDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int JobCount { get; set; }
    public int TokensUsed { get; set; }
}

/// <summary>
/// AI error report DTO
/// </summary>
public class AIErrorReportDto
{
    public Guid JobId { get; set; }
    public AIJobType JobType { get; set; }
    public string ErrorCode { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public int RetryCount { get; set; }
}

/// <summary>
/// AI cost report DTO
/// </summary>
public class AICostReportDto
{
    public decimal TotalCost { get; set; }
    public Dictionary<string, decimal> CostByProvider { get; set; } = new();
    public Dictionary<string, decimal> CostByJobType { get; set; } = new();
    public Dictionary<string, decimal> CostByDepartment { get; set; } = new();
}

/// <summary>
/// Provider config DTO
/// </summary>
public class ProviderConfigDto
{
    public AIProvider Provider { get; set; }
    public bool IsConfigured { get; set; }
    public bool IsActive { get; set; }
    public string? Endpoint { get; set; }
    public bool ApiKeyConfigured { get; set; }
}

/// <summary>
/// Update provider config request
/// </summary>
public class UpdateProviderConfigRequest
{
    public string? Endpoint { get; set; }
    public string? ApiKey { get; set; }
    public string? ApiVersion { get; set; }
    public bool IsActive { get; set; }
    public Dictionary<string, string>? AdditionalSettings { get; set; }
}

#endregion
