using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.WebApi.Features.AIAnalysis.Models;
using AFC27.KMS.WebApi.Features.AIAnalysis.Services;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.WebApi.Features.AIAnalysis.Controllers;

/// <summary>
/// Controller for AI-powered document analysis
/// </summary>
[ApiController]
[Route("api/ai")]
[Authorize]
public class AIAnalysisController : ControllerBase
{
    private readonly IAIAnalysisService _analysisService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<AIAnalysisController> _logger;

    public AIAnalysisController(
        IAIAnalysisService analysisService,
        ICurrentUser currentUser,
        ILogger<AIAnalysisController> logger)
    {
        _analysisService = analysisService;
        _currentUser = currentUser;
        _logger = logger;
    }

    // Document Analysis
    [HttpPost("documents/{documentId:guid}/analyze")]
    public async Task<ActionResult<DocumentAnalysis>> AnalyzeDocument(
        Guid documentId,
        [FromBody] AnalyzeDocumentRequest? request,
        CancellationToken cancellationToken)
    {
        request ??= new AnalyzeDocumentRequest();
        var analysis = await _analysisService.AnalyzeDocumentAsync(documentId, request, cancellationToken);
        return Ok(analysis);
    }

    [HttpGet("documents/{documentId:guid}/analysis")]
    public async Task<ActionResult<DocumentAnalysis>> GetDocumentAnalysis(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var analysis = await _analysisService.GetDocumentAnalysisAsync(documentId, cancellationToken);
        return analysis == null ? NotFound() : Ok(analysis);
    }

    [HttpGet("analyses/recent")]
    public async Task<ActionResult<List<DocumentAnalysis>>> GetRecentAnalyses(
        [FromQuery] int count = 10,
        CancellationToken cancellationToken = default)
    {
        var analyses = await _analysisService.GetRecentAnalysesAsync(count, cancellationToken);
        return Ok(analyses);
    }

    // Entity Extraction
    [HttpGet("documents/{documentId:guid}/entities")]
    public async Task<ActionResult<List<ExtractedEntity>>> GetDocumentEntities(
        Guid documentId,
        [FromQuery] EntityType? type,
        CancellationToken cancellationToken)
    {
        var entities = await _analysisService.GetDocumentEntitiesAsync(documentId, type, cancellationToken);
        return Ok(entities);
    }

    [HttpPost("entities/extract")]
    public async Task<ActionResult<List<ExtractedEntity>>> ExtractEntities(
        [FromBody] ExtractEntitiesRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { message = "Text is required" });

        var entities = await _analysisService.ExtractEntitiesFromTextAsync(request.Text, cancellationToken);
        return Ok(entities);
    }

    // Sentiment Analysis
    [HttpPost("documents/{documentId:guid}/sentiment")]
    public async Task<ActionResult<SentimentAnalysis>> AnalyzeDocumentSentiment(
        Guid documentId,
        [FromBody] AnalyzeSentimentRequest request,
        CancellationToken cancellationToken)
    {
        var sentiment = await _analysisService.AnalyzeSentimentAsync(
            documentId, ContentType.Document, request.Text, cancellationToken);
        return Ok(sentiment);
    }

    [HttpGet("documents/{documentId:guid}/sentiment")]
    public async Task<ActionResult<SentimentAnalysis>> GetDocumentSentiment(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var sentiment = await _analysisService.GetSentimentAsync(documentId, cancellationToken);
        return sentiment == null ? NotFound() : Ok(sentiment);
    }

    // Tag Suggestions
    [HttpGet("documents/{documentId:guid}/tags/suggest")]
    public async Task<ActionResult<List<TagSuggestion>>> GetTagSuggestions(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var suggestions = await _analysisService.GetTagSuggestionsAsync(documentId, cancellationToken);
        return Ok(suggestions);
    }

    [HttpPost("tags/suggest")]
    public async Task<ActionResult<List<TagSuggestion>>> SuggestTagsFromText(
        [FromBody] SuggestTagsRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { message = "Text is required" });

        var suggestions = await _analysisService.SuggestTagsFromTextAsync(
            request.Text, request.MaxTags, cancellationToken);
        return Ok(suggestions);
    }

    [HttpPost("documents/{documentId:guid}/tags/apply")]
    public async Task<ActionResult> ApplyTagSuggestions(
        Guid documentId,
        [FromBody] ApplyTagsRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Tags == null || !request.Tags.Any())
            return BadRequest(new { message = "Tags are required" });

        await _analysisService.ApplyTagSuggestionsAsync(documentId, request.Tags, cancellationToken);
        return Ok(new { message = "Tags applied successfully" });
    }

    // Summarization
    [HttpPost("documents/{documentId:guid}/summarize")]
    public async Task<ActionResult<object>> SummarizeDocument(
        Guid documentId,
        [FromBody] SummarizeRequest? request,
        CancellationToken cancellationToken)
    {
        request ??= new SummarizeRequest();
        var summary = await _analysisService.GenerateSummaryAsync(
            documentId, request.MaxLength, request.TargetLanguage, cancellationToken);
        return Ok(new { summary });
    }

    [HttpPost("summarize")]
    public async Task<ActionResult<object>> SummarizeText(
        [FromBody] SummarizeTextRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { message = "Text is required" });

        var summary = await _analysisService.SummarizeTextAsync(request.Text, request.MaxLength, cancellationToken);
        return Ok(new { summary });
    }

    // Semantic Search
    [HttpPost("search/semantic")]
    public async Task<ActionResult<List<SemanticSearchResult>>> SemanticSearch(
        [FromBody] SemanticSearchRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return BadRequest(new { message = "Query is required" });

        var results = await _analysisService.SemanticSearchAsync(request, cancellationToken);
        return Ok(results);
    }

    [HttpGet("content/{contentId:guid}/similar")]
    public async Task<ActionResult<List<SimilarContent>>> FindSimilarContent(
        Guid contentId,
        [FromQuery] ContentType contentType = ContentType.Document,
        [FromQuery] int maxResults = 10,
        CancellationToken cancellationToken = default)
    {
        var similar = await _analysisService.FindSimilarContentAsync(contentId, contentType, maxResults, cancellationToken);
        return Ok(similar);
    }

    // Transcription
    [HttpPost("transcribe")]
    public async Task<ActionResult<TranscriptionResult>> StartTranscription(
        [FromBody] TranscribeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _analysisService.StartTranscriptionAsync(request, cancellationToken);
        return Accepted(result);
    }

    [HttpGet("transcriptions/{transcriptionId:guid}")]
    public async Task<ActionResult<TranscriptionResult>> GetTranscriptionStatus(
        Guid transcriptionId,
        CancellationToken cancellationToken)
    {
        var result = await _analysisService.GetTranscriptionStatusAsync(transcriptionId, cancellationToken);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("transcriptions/source/{sourceId:guid}")]
    public async Task<ActionResult<TranscriptionResult>> GetTranscriptionBySource(
        Guid sourceId,
        [FromQuery] TranscriptionSourceType sourceType,
        CancellationToken cancellationToken)
    {
        var result = await _analysisService.GetTranscriptionBySourceAsync(sourceId, sourceType, cancellationToken);
        return result == null ? NotFound() : Ok(result);
    }

    // Batch Processing
    [HttpPost("batch/analyze")]
    public async Task<ActionResult<object>> StartBatchAnalysis(
        [FromBody] BatchAnalysisRequest request,
        CancellationToken cancellationToken)
    {
        if (request.DocumentIds == null || !request.DocumentIds.Any())
            return BadRequest(new { message = "Document IDs are required" });

        var batchId = await _analysisService.StartBatchAnalysisAsync(request, cancellationToken);
        return Accepted(new { batchId, message = "Batch analysis started" });
    }

    [HttpGet("batch/{batchId:guid}")]
    public async Task<ActionResult<BatchAnalysisStatus>> GetBatchStatus(
        Guid batchId,
        CancellationToken cancellationToken)
    {
        var status = await _analysisService.GetBatchStatusAsync(batchId, cancellationToken);
        return status == null ? NotFound() : Ok(status);
    }

    // Recommendations
    [HttpGet("recommendations")]
    public async Task<ActionResult<List<ContentRecommendation>>> GetRecommendations(
        [FromQuery] int maxResults = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;
        var recommendations = await _analysisService.GetRecommendationsAsync(userId, maxResults, cancellationToken);
        return Ok(recommendations);
    }

    [HttpGet("content/{contentId:guid}/recommendations")]
    public async Task<ActionResult<List<ContentRecommendation>>> GetContentBasedRecommendations(
        Guid contentId,
        [FromQuery] ContentType contentType = ContentType.Document,
        [FromQuery] int maxResults = 10,
        CancellationToken cancellationToken = default)
    {
        var recommendations = await _analysisService.GetContentBasedRecommendationsAsync(
            contentId, contentType, maxResults, cancellationToken);
        return Ok(recommendations);
    }

    // Dashboard
    [HttpGet("dashboard")]
    public async Task<ActionResult<AIInsightsDashboard>> GetInsightsDashboard(CancellationToken cancellationToken)
    {
        var dashboard = await _analysisService.GetInsightsDashboardAsync(cancellationToken);
        return Ok(dashboard);
    }
}

// Request DTOs
public class ExtractEntitiesRequest
{
    public string Text { get; set; } = string.Empty;
}

public class AnalyzeSentimentRequest
{
    public string Text { get; set; } = string.Empty;
}

public class SuggestTagsRequest
{
    public string Text { get; set; } = string.Empty;
    public int MaxTags { get; set; } = 10;
}

public class ApplyTagsRequest
{
    public List<string> Tags { get; set; } = new();
}

public class SummarizeRequest
{
    public int? MaxLength { get; set; }
    public string? TargetLanguage { get; set; }
}

public class SummarizeTextRequest
{
    public string Text { get; set; } = string.Empty;
    public int? MaxLength { get; set; }
}
