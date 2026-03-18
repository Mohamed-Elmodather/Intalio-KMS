using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AFC27.KMS.AI.Application.DTOs;
using AFC27.KMS.AI.Application.Services;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.AI.Presentation.Controllers;

/// <summary>
/// API controller for inline AI writing assistance.
/// Provides suggest, generate, improve, tone-change, translate, summarize, continue, and streaming endpoints.
/// </summary>
[ApiController]
[Route("api/ai/writing")]
[Authorize]
public class AIWritingAssistantController : ControllerBase
{
    private readonly IWritingAssistantService _writingService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<AIWritingAssistantController> _logger;

    public AIWritingAssistantController(
        IWritingAssistantService writingService,
        ICurrentUser currentUser,
        ILogger<AIWritingAssistantController> logger)
    {
        _writingService = writingService;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <summary>
    /// Suggest improvements for the given text.
    /// Returns the improved text along with a list of specific suggested changes.
    /// </summary>
    [HttpPost("suggest")]
    [ProducesResponseType(typeof(AIWritingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Suggest(
        [FromBody] AIWritingRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { error = "Text is required" });

        _logger.LogInformation("Writing suggest request from user {UserId}", _currentUser.UserId);

        var response = await _writingService.SuggestAsync(
            request, _currentUser.UserId ?? Guid.Empty, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Generate content from a prompt.
    /// Supports optional style, max length, language, and template ID parameters.
    /// </summary>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(AIWritingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Generate(
        [FromBody] AIGenerateRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
            return BadRequest(new { error = "Prompt is required" });

        _logger.LogInformation("Writing generate request from user {UserId}", _currentUser.UserId);

        var response = await _writingService.GenerateAsync(
            request, _currentUser.UserId ?? Guid.Empty, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Improve text grammar, clarity, and structure.
    /// Returns the improved version of the provided text.
    /// </summary>
    [HttpPost("improve")]
    [ProducesResponseType(typeof(AIWritingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Improve(
        [FromBody] AIWritingRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { error = "Text is required" });

        _logger.LogInformation("Writing improve request from user {UserId}", _currentUser.UserId);

        var response = await _writingService.ImproveAsync(
            request, _currentUser.UserId ?? Guid.Empty, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Change the tone of text to match a target audience.
    /// Supported tones: Formal, Casual, Simplified, Technical, Diplomatic, Persuasive.
    /// </summary>
    [HttpPost("change-tone")]
    [ProducesResponseType(typeof(AIWritingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeTone(
        [FromBody] AIToneChangeRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { error = "Text is required" });

        _logger.LogInformation("Writing change-tone request from user {UserId}, tone: {Tone}",
            _currentUser.UserId, request.TargetTone);

        var response = await _writingService.ChangeToneAsync(
            request, _currentUser.UserId ?? Guid.Empty, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Translate text between languages (EN/AR and others).
    /// Auto-detects source language if not provided.
    /// </summary>
    [HttpPost("translate")]
    [ProducesResponseType(typeof(AIWritingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Translate(
        [FromBody] AITranslateRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { error = "Text is required" });

        if (string.IsNullOrWhiteSpace(request.TargetLanguage))
            return BadRequest(new { error = "TargetLanguage is required" });

        _logger.LogInformation("Writing translate request from user {UserId}, target: {Target}",
            _currentUser.UserId, request.TargetLanguage);

        var response = await _writingService.TranslateAsync(
            request, _currentUser.UserId ?? Guid.Empty, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Summarize text into a shorter version.
    /// Supports optional max sentence count and focus area.
    /// </summary>
    [HttpPost("summarize")]
    [ProducesResponseType(typeof(AIWritingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Summarize(
        [FromBody] AISummarizeRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { error = "Text is required" });

        _logger.LogInformation("Writing summarize request from user {UserId}", _currentUser.UserId);

        var response = await _writingService.SummarizeAsync(
            request, _currentUser.UserId ?? Guid.Empty, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Continue writing from the given text position.
    /// Maintains the style, tone, and language of the existing content.
    /// </summary>
    [HttpPost("continue")]
    [ProducesResponseType(typeof(AIWritingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Continue(
        [FromBody] AIContinueRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { error = "Text is required" });

        _logger.LogInformation("Writing continue request from user {UserId}", _currentUser.UserId);

        var response = await _writingService.ContinueAsync(
            request, _currentUser.UserId ?? Guid.Empty, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Stream a writing operation result via Server-Sent Events (SSE).
    /// Supports all writing operations: Generate, Improve, ChangeTone, Translate, Summarize, Continue, Suggest.
    /// </summary>
    [HttpPost("stream")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task Stream(
        [FromBody] AIStreamWritingRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            await Response.WriteAsync("{\"error\":\"Text is required\"}");
            return;
        }

        _logger.LogInformation(
            "Writing stream request from user {UserId}, operation: {Operation}",
            _currentUser.UserId, request.Operation);

        // Set up SSE response
        Response.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Connection = "keep-alive";

        var fullContent = new StringBuilder();

        try
        {
            await foreach (var chunk in _writingService.StreamAsync(
                request, _currentUser.UserId ?? Guid.Empty, cancellationToken))
            {
                fullContent.Append(chunk.Content);

                if (chunk.IsComplete)
                {
                    await SendSSEEvent("done", new
                    {
                        fullContent = fullContent.ToString(),
                        tokenCount = chunk.TokenCount
                    });
                }
                else if (!string.IsNullOrEmpty(chunk.Content))
                {
                    await SendSSEEvent("chunk", new
                    {
                        content = chunk.Content,
                        tokenCount = chunk.TokenCount
                    });
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Writing stream cancelled by user {UserId}", _currentUser.UserId);
            await SendSSEEvent("cancelled", new { message = "Stream cancelled by client" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during writing stream for user {UserId}", _currentUser.UserId);
            await SendSSEEvent("error", new { error = "An error occurred during processing" });
        }
    }

    /// <summary>
    /// Generate a complete article from a topic, prompt, and optional outline.
    /// Returns a structured article with title, sections, summary, and suggested tags.
    /// </summary>
    [HttpPost("generate-article")]
    [ProducesResponseType(typeof(GenerateArticleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerateArticle(
        [FromBody] GenerateArticleRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Topic))
            return BadRequest(new { error = "Topic is required" });

        if (request.MaxSections < 1 || request.MaxSections > 20)
            return BadRequest(new { error = "MaxSections must be between 1 and 20" });

        _logger.LogInformation(
            "Generate article request from user {UserId}, topic: {Topic}, maxSections: {MaxSections}",
            _currentUser.UserId, request.Topic, request.MaxSections);

        var sw = System.Diagnostics.Stopwatch.StartNew();

        // Build the generation prompt
        var promptBuilder = new System.Text.StringBuilder();
        promptBuilder.AppendLine($"Generate a complete, well-structured article about: {request.Topic}");

        if (!string.IsNullOrWhiteSpace(request.Prompt))
            promptBuilder.AppendLine($"\nAdditional instructions: {request.Prompt}");

        if (!string.IsNullOrWhiteSpace(request.Style))
            promptBuilder.AppendLine($"\nWriting style: {request.Style}");

        if (!string.IsNullOrWhiteSpace(request.TargetAudience))
            promptBuilder.AppendLine($"\nTarget audience: {request.TargetAudience}");

        if (!string.IsNullOrWhiteSpace(request.Language))
            promptBuilder.AppendLine($"\nLanguage: {(request.Language == "ar" ? "Arabic (Modern Standard Arabic)" : "English")}");

        if (request.TargetWordCount.HasValue)
            promptBuilder.AppendLine($"\nTarget word count: approximately {request.TargetWordCount} words");

        promptBuilder.AppendLine($"\nMaximum sections: {request.MaxSections}");

        if (request.Outline is { Count: > 0 })
        {
            promptBuilder.AppendLine("\nArticle outline:");
            foreach (var section in request.Outline.OrderBy(s => s.Order))
            {
                promptBuilder.AppendLine($"- {section.Heading}");
                if (!string.IsNullOrWhiteSpace(section.Description))
                    promptBuilder.AppendLine($"  ({section.Description})");
            }
        }

        if (!string.IsNullOrWhiteSpace(request.Context))
            promptBuilder.AppendLine($"\nReference context:\n{request.Context}");

        var generateRequest = new AIGenerateRequest
        {
            Prompt = promptBuilder.ToString(),
            Style = request.Style,
            Language = request.Language,
            TemplateId = request.TemplateId,
            Context = request.Context
        };

        var aiResponse = await _writingService.GenerateAsync(
            generateRequest, _currentUser.UserId ?? Guid.Empty, cancellationToken);

        sw.Stop();

        // Parse the generated content into sections
        var sections = ParseSections(aiResponse.Result, request.MaxSections);
        var title = ExtractTitle(aiResponse.Result, request.Topic);
        var summary = GenerateSummaryFromSections(sections);
        var totalWordCount = sections.Sum(s => s.WordCount);

        var response = new GenerateArticleResponse
        {
            Title = title,
            Sections = sections,
            Summary = summary,
            SuggestedTags = ExtractSuggestedTags(request.Topic, aiResponse.Result),
            SuggestedCategory = InferCategory(request.Topic),
            WordCount = totalWordCount,
            TokensUsed = aiResponse.TokensUsed,
            ProcessingTimeMs = (int)sw.ElapsedMilliseconds
        };

        return Ok(response);
    }

    private static IReadOnlyList<GeneratedSection> ParseSections(string content, int maxSections)
    {
        var sections = new List<GeneratedSection>();
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var currentHeading = string.Empty;
        var currentContent = new System.Text.StringBuilder();
        var order = 0;

        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            // Detect headings (lines starting with # or all-caps short lines)
            if (trimmed.StartsWith('#') || (trimmed.Length < 100 && trimmed == trimmed.ToUpperInvariant() && trimmed.Length > 3))
            {
                // Save previous section
                if (!string.IsNullOrEmpty(currentHeading) && currentContent.Length > 0)
                {
                    var sectionText = currentContent.ToString().Trim();
                    sections.Add(new GeneratedSection
                    {
                        Heading = currentHeading,
                        Content = sectionText,
                        Order = order,
                        WordCount = sectionText.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length
                    });
                    order++;
                    currentContent.Clear();
                }

                currentHeading = trimmed.TrimStart('#').Trim();
            }
            else
            {
                currentContent.AppendLine(trimmed);
            }
        }

        // Add the last section
        if (!string.IsNullOrEmpty(currentHeading) && currentContent.Length > 0)
        {
            var sectionText = currentContent.ToString().Trim();
            sections.Add(new GeneratedSection
            {
                Heading = currentHeading,
                Content = sectionText,
                Order = order,
                WordCount = sectionText.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length
            });
        }

        // If no sections were detected, treat the entire content as one section
        if (sections.Count == 0 && !string.IsNullOrWhiteSpace(content))
        {
            sections.Add(new GeneratedSection
            {
                Heading = "Content",
                Content = content.Trim(),
                Order = 0,
                WordCount = content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length
            });
        }

        return sections.Take(maxSections).ToList();
    }

    private static string ExtractTitle(string content, string fallbackTopic)
    {
        var firstLine = content.Split('\n', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim();
        if (!string.IsNullOrEmpty(firstLine) && firstLine.StartsWith('#'))
            return firstLine.TrimStart('#').Trim();

        return fallbackTopic;
    }

    private static string GenerateSummaryFromSections(IReadOnlyList<GeneratedSection> sections)
    {
        if (sections.Count == 0)
            return string.Empty;

        // Take the first sentence from each section as a summary
        var summaryParts = sections
            .Select(s =>
            {
                var firstSentenceEnd = s.Content.IndexOfAny(new[] { '.', '!', '?' });
                return firstSentenceEnd > 0 ? s.Content[..(firstSentenceEnd + 1)] : s.Content;
            })
            .Take(3);

        return string.Join(" ", summaryParts);
    }

    private static IReadOnlyList<string> ExtractSuggestedTags(string topic, string content)
    {
        var tags = new List<string>();

        // Add topic words as base tags
        var topicWords = topic.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(w => w.Length > 3)
            .Select(w => w.ToLowerInvariant())
            .Distinct()
            .Take(5);

        tags.AddRange(topicWords);

        // Add common AFC/sports tags if relevant
        var lowerContent = content.ToLowerInvariant();
        if (lowerContent.Contains("tournament") || lowerContent.Contains("cup"))
            tags.Add("tournament");
        if (lowerContent.Contains("stadium") || lowerContent.Contains("venue"))
            tags.Add("venues");
        if (lowerContent.Contains("team") || lowerContent.Contains("player"))
            tags.Add("teams");
        if (lowerContent.Contains("volunteer"))
            tags.Add("volunteers");

        return tags.Distinct().Take(10).ToList();
    }

    private static string? InferCategory(string topic)
    {
        var lower = topic.ToLowerInvariant();
        if (lower.Contains("stadium") || lower.Contains("venue") || lower.Contains("infrastructure"))
            return "Infrastructure";
        if (lower.Contains("team") || lower.Contains("match") || lower.Contains("tournament"))
            return "Tournament";
        if (lower.Contains("volunteer") || lower.Contains("community"))
            return "Community Engagement";
        if (lower.Contains("partner") || lower.Contains("sponsor"))
            return "Partnerships";
        if (lower.Contains("security") || lower.Contains("safety"))
            return "Security";
        if (lower.Contains("transport") || lower.Contains("logistics"))
            return "Operations";

        return "General";
    }

    private async Task SendSSEEvent(string eventType, object data)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(data);
        await Response.WriteAsync($"event: {eventType}\ndata: {json}\n\n");
        await Response.Body.FlushAsync();
    }
}
