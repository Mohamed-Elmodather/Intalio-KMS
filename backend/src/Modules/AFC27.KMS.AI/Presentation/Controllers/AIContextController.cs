using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.AI.Application.DTOs;
using AFC27.KMS.AI.Application.Interfaces;
using AFC27.KMS.Content.Domain.Entities;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.AI.Presentation.Controllers;

/// <summary>
/// API controller for AI-in-context features: summarization, explanation,
/// key-point extraction, and context-aware Q&amp;A.
/// </summary>
[ApiController]
[Route("api/ai/context")]
[Authorize]
public class AIContextController : ControllerBase
{
    private readonly IIntalioAIClient _aiClient;
    private readonly DbContext _dbContext;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<AIContextController> _logger;

    private const string SummarizePrompt =
        @"You are the AFC Asian Cup 2027 Knowledge Assistant.
Summarize the following article content clearly and concisely. Produce a short summary paragraph and a bullet list of key points.
Respond in {0}.

ARTICLE TITLE: {1}

ARTICLE CONTENT:
{2}

Respond in this JSON format:
{{""summary"": ""..."", ""keyPoints"": [""..."", ""...""]}}";

    private const string ExplainPrompt =
        @"You are the AFC Asian Cup 2027 Knowledge Assistant.
Explain the following article content in simple, easy-to-understand terms. Avoid jargon. Target an audience that may be unfamiliar with the topic.
Respond in {0}.

ARTICLE TITLE: {1}

ARTICLE CONTENT:
{2}";

    private const string ExtractPointsPrompt =
        @"You are the AFC Asian Cup 2027 Knowledge Assistant.
Extract the key points from the following document content. Return them as a JSON object.
Respond in {0}.

DOCUMENT CONTENT:
{1}

Respond in this JSON format:
{{""keyPoints"": [""..."", ""...""], ""summary"": ""...""}}";

    private const string ContextQueryPrompt =
        @"You are the AFC Asian Cup 2027 Knowledge Assistant.
Answer the user's question based ONLY on the provided context. If the context does not contain enough information, say so.
Respond in {0}.

CONTEXT ({1}):
{2}

USER QUESTION: {3}";

    public AIContextController(
        IIntalioAIClient aiClient,
        DbContext dbContext,
        ICurrentUser currentUser,
        ILogger<AIContextController> logger)
    {
        _aiClient = aiClient;
        _dbContext = dbContext;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <summary>
    /// Summarize an article's content using AI.
    /// </summary>
    [HttpPost("article/{id:guid}/summarize")]
    [ProducesResponseType(typeof(ArticleSummarizeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SummarizeArticle(
        Guid id,
        CancellationToken cancellationToken)
    {
        var article = await _dbContext.Set<Article>()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (article == null)
            return NotFound(new { error = "Article not found" });

        var language = _currentUser.Language == "ar" ? "Arabic" : "English";
        var content = _currentUser.Language == "ar" && !string.IsNullOrEmpty(article.Content.Arabic)
            ? article.Content.Arabic
            : article.Content.English;
        var title = _currentUser.Language == "ar" && !string.IsNullOrEmpty(article.Title.Arabic)
            ? article.Title.Arabic
            : article.Title.English;

        _logger.LogInformation("Summarizing article {ArticleId} for user {UserId}", id, _currentUser.UserId);

        var prompt = string.Format(SummarizePrompt, language, title, content);

        var aiResponse = await _aiClient.ChatAsync(
            new ChatCompletionRequest
            {
                Messages = new[]
                {
                    new ChatMessage { Role = "user", Content = prompt }
                },
                Temperature = 0.3f,
                MaxTokens = 1024
            },
            cancellationToken);

        // Parse key points from AI response (best-effort JSON extraction)
        var keyPoints = ExtractKeyPointsFromResponse(aiResponse.Content);
        var summary = ExtractSummaryFromResponse(aiResponse.Content);

        return Ok(new ArticleSummarizeResponse
        {
            ArticleId = id,
            Summary = summary ?? aiResponse.Content,
            KeyPoints = keyPoints,
            TokensUsed = aiResponse.TotalTokens
        });
    }

    /// <summary>
    /// Explain an article's content in simpler terms using AI.
    /// </summary>
    [HttpPost("article/{id:guid}/explain")]
    [ProducesResponseType(typeof(ArticleExplainResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExplainArticle(
        Guid id,
        CancellationToken cancellationToken)
    {
        var article = await _dbContext.Set<Article>()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (article == null)
            return NotFound(new { error = "Article not found" });

        var language = _currentUser.Language == "ar" ? "Arabic" : "English";
        var content = _currentUser.Language == "ar" && !string.IsNullOrEmpty(article.Content.Arabic)
            ? article.Content.Arabic
            : article.Content.English;
        var title = _currentUser.Language == "ar" && !string.IsNullOrEmpty(article.Title.Arabic)
            ? article.Title.Arabic
            : article.Title.English;

        _logger.LogInformation("Explaining article {ArticleId} for user {UserId}", id, _currentUser.UserId);

        var prompt = string.Format(ExplainPrompt, language, title, content);

        var aiResponse = await _aiClient.ChatAsync(
            new ChatCompletionRequest
            {
                Messages = new[]
                {
                    new ChatMessage { Role = "user", Content = prompt }
                },
                Temperature = 0.5f,
                MaxTokens = 1536
            },
            cancellationToken);

        return Ok(new ArticleExplainResponse
        {
            ArticleId = id,
            Explanation = aiResponse.Content,
            TokensUsed = aiResponse.TotalTokens
        });
    }

    /// <summary>
    /// Extract key points from a document.
    /// </summary>
    [HttpPost("document/{id:guid}/extract-points")]
    [ProducesResponseType(typeof(DocumentExtractPointsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExtractKeyPoints(
        Guid id,
        CancellationToken cancellationToken)
    {
        // Try to load as an article first (documents module may resolve differently)
        var article = await _dbContext.Set<Article>()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        string documentContent;
        if (article != null)
        {
            documentContent = article.Content.English;
        }
        else
        {
            return NotFound(new { error = "Document not found" });
        }

        var language = _currentUser.Language == "ar" ? "Arabic" : "English";

        _logger.LogInformation("Extracting key points from document {DocumentId} for user {UserId}", id, _currentUser.UserId);

        var prompt = string.Format(ExtractPointsPrompt, language, documentContent);

        var aiResponse = await _aiClient.ChatAsync(
            new ChatCompletionRequest
            {
                Messages = new[]
                {
                    new ChatMessage { Role = "user", Content = prompt }
                },
                Temperature = 0.3f,
                MaxTokens = 1024
            },
            cancellationToken);

        var keyPoints = ExtractKeyPointsFromResponse(aiResponse.Content);
        var summary = ExtractSummaryFromResponse(aiResponse.Content);

        return Ok(new DocumentExtractPointsResponse
        {
            DocumentId = id,
            KeyPoints = keyPoints,
            Summary = summary,
            TokensUsed = aiResponse.TotalTokens
        });
    }

    /// <summary>
    /// Answer a question with injected page/entity context.
    /// </summary>
    [HttpPost("context-query")]
    [ProducesResponseType(typeof(ContextQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ContextQuery(
        [FromBody] ContextQueryRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return BadRequest(new { error = "Query is required" });

        _logger.LogInformation(
            "Context query from user {UserId}: ContextType={ContextType}, EntityId={EntityId}",
            _currentUser.UserId, request.ContextType, request.ContextEntityId);

        // Resolve context text from entity if not provided directly
        var contextText = request.ContextText;
        var sources = new List<ContextSource>();

        if (string.IsNullOrWhiteSpace(contextText) && request.ContextEntityId.HasValue)
        {
            var resolved = await ResolveContextAsync(
                request.ContextType, request.ContextEntityId.Value, cancellationToken);

            contextText = resolved.content;
            if (resolved.source != null)
                sources.Add(resolved.source);
        }

        if (string.IsNullOrWhiteSpace(contextText))
            return BadRequest(new { error = "No context could be resolved. Provide contextText or a valid contextEntityId." });

        var language = (request.Language ?? _currentUser.Language) == "ar" ? "Arabic" : "English";
        var prompt = string.Format(ContextQueryPrompt, language, request.ContextType, contextText, request.Query);

        var aiResponse = await _aiClient.ChatAsync(
            new ChatCompletionRequest
            {
                Messages = new[]
                {
                    new ChatMessage { Role = "user", Content = prompt }
                },
                Temperature = 0.4f,
                MaxTokens = 1536
            },
            cancellationToken);

        // Simple confidence heuristic based on response characteristics
        var confidence = EstimateConfidence(aiResponse.Content, contextText);

        return Ok(new ContextQueryResponse
        {
            Answer = aiResponse.Content,
            Confidence = confidence,
            Sources = sources,
            TokensUsed = aiResponse.TotalTokens
        });
    }

    // ========================================
    // Private Helpers
    // ========================================

    private async Task<(string content, ContextSource? source)> ResolveContextAsync(
        string contextType, Guid entityId, CancellationToken ct)
    {
        switch (contextType.ToLowerInvariant())
        {
            case "article":
                var article = await _dbContext.Set<Article>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id == entityId, ct);

                if (article != null)
                {
                    var content = $"Title: {article.Title.English}\n\n{article.Content.English}";
                    if (!string.IsNullOrEmpty(article.Summary?.English))
                        content = $"Title: {article.Title.English}\nSummary: {article.Summary.English}\n\n{article.Content.English}";

                    return (content, new ContextSource
                    {
                        EntityType = "Article",
                        EntityId = entityId,
                        Title = article.Title.English,
                        Excerpt = article.Summary?.English
                    });
                }
                break;

            default:
                _logger.LogWarning("Unsupported context type: {ContextType}", contextType);
                break;
        }

        return (string.Empty, null);
    }

    private static double EstimateConfidence(string answer, string context)
    {
        if (string.IsNullOrWhiteSpace(answer))
            return 0.0;

        // If the answer explicitly states it cannot answer, low confidence
        var lowerAnswer = answer.ToLowerInvariant();
        if (lowerAnswer.Contains("i don't have") || lowerAnswer.Contains("not enough information") ||
            lowerAnswer.Contains("cannot determine") || lowerAnswer.Contains("no information"))
            return 0.2;

        // Longer, more detailed answers with context overlap get higher confidence
        var words = answer.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var contextWords = context.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.ToLowerInvariant())
            .ToHashSet();

        var overlapCount = words.Count(w => contextWords.Contains(w.ToLowerInvariant()));
        var overlapRatio = words.Length > 0 ? (double)overlapCount / words.Length : 0;

        // Base confidence + overlap bonus
        return Math.Min(1.0, 0.5 + overlapRatio * 0.5);
    }

    private static IReadOnlyList<string> ExtractKeyPointsFromResponse(string response)
    {
        try
        {
            // Try to extract JSON array of key points
            var keyPointsStart = response.IndexOf("\"keyPoints\"", StringComparison.OrdinalIgnoreCase);
            if (keyPointsStart >= 0)
            {
                var arrayStart = response.IndexOf('[', keyPointsStart);
                var arrayEnd = response.IndexOf(']', arrayStart);
                if (arrayStart >= 0 && arrayEnd > arrayStart)
                {
                    var arrayJson = response.Substring(arrayStart, arrayEnd - arrayStart + 1);
                    var points = System.Text.Json.JsonSerializer.Deserialize<List<string>>(arrayJson);
                    if (points != null && points.Count > 0)
                        return points;
                }
            }
        }
        catch
        {
            // Fall through to line-based extraction
        }

        // Fallback: extract bullet points or numbered items
        var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var keyPoints = new List<string>();
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("- ") || trimmed.StartsWith("* ") ||
                (trimmed.Length > 2 && char.IsDigit(trimmed[0]) && trimmed[1] == '.'))
            {
                keyPoints.Add(trimmed.TrimStart('-', '*', ' ').TrimStart('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.', ' '));
            }
        }

        return keyPoints;
    }

    private static string? ExtractSummaryFromResponse(string response)
    {
        try
        {
            var summaryStart = response.IndexOf("\"summary\"", StringComparison.OrdinalIgnoreCase);
            if (summaryStart >= 0)
            {
                var valueStart = response.IndexOf('"', summaryStart + 9);
                if (valueStart >= 0)
                {
                    valueStart++; // skip opening quote
                    var valueEnd = response.IndexOf('"', valueStart);
                    // Handle escaped quotes
                    while (valueEnd > 0 && response[valueEnd - 1] == '\\')
                        valueEnd = response.IndexOf('"', valueEnd + 1);

                    if (valueEnd > valueStart)
                        return response.Substring(valueStart, valueEnd - valueStart);
                }
            }
        }
        catch
        {
            // Ignore parsing errors
        }

        return null;
    }
}
