using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using AFC27.KMS.AI.Application.DTOs;
using AFC27.KMS.AI.Application.Interfaces;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.AI.Application.Services;

/// <summary>
/// Service interface for multi-step AI research.
/// </summary>
public interface IResearchService
{
    /// <summary>
    /// Execute a multi-step research query, searching multiple sources, synthesizing findings, and producing a structured report.
    /// </summary>
    Task<ResearchResponse> ResearchAsync(ResearchRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Multi-step AI research service that searches multiple sources, synthesizes information, and produces a report.
/// </summary>
public class ResearchService : IResearchService
{
    private readonly IRAGService _ragService;
    private readonly IIntalioAIClient _aiClient;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<ResearchService> _logger;

    public ResearchService(
        IRAGService ragService,
        IIntalioAIClient aiClient,
        ICurrentUser currentUser,
        ILogger<ResearchService> logger)
    {
        _ragService = ragService;
        _aiClient = aiClient;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ResearchResponse> ResearchAsync(
        ResearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var researchId = Guid.NewGuid();

        _logger.LogInformation(
            "Starting research {ResearchId} for user {UserId}: depth={Depth}, question='{Question}'",
            researchId, _currentUser.UserId, request.Depth, request.Question);

        var passCount = GetPassCount(request.Depth);
        var allSources = new List<DocumentChunk>();
        var allCitations = new List<Citation>();
        var totalTokens = 0;

        // --- Pass 1: Initial broad search ---
        var initialResults = await _ragService.SearchAsync(
            request.Question,
            _currentUser.UserId ?? Guid.Empty,
            request.MaxSources,
            cancellationToken);

        allSources.AddRange(initialResults);

        // --- Pass 2+: Refine queries based on initial findings ---
        if (passCount >= 2 && initialResults.Any())
        {
            var refinedQueries = await GenerateRefinedQueriesAsync(
                request.Question, initialResults, cancellationToken);

            totalTokens += refinedQueries.TokensUsed;

            foreach (var refinedQuery in refinedQueries.Queries)
            {
                var refinedResults = await _ragService.SearchAsync(
                    refinedQuery,
                    _currentUser.UserId ?? Guid.Empty,
                    Math.Max(3, request.MaxSources / 3),
                    cancellationToken);

                // Add unique sources only
                foreach (var result in refinedResults)
                {
                    if (!allSources.Any(s => s.DocumentId == result.DocumentId && s.ChunkId == result.ChunkId))
                    {
                        allSources.Add(result);
                    }
                }
            }
        }

        // --- Pass 3: Deep analysis with cross-referencing ---
        if (passCount >= 3 && allSources.Count > 1)
        {
            var crossRefQuery = await GenerateCrossReferenceQueryAsync(
                request.Question, allSources, cancellationToken);

            totalTokens += crossRefQuery.TokensUsed;

            if (!string.IsNullOrWhiteSpace(crossRefQuery.Query))
            {
                var deepResults = await _ragService.SearchAsync(
                    crossRefQuery.Query,
                    _currentUser.UserId ?? Guid.Empty,
                    5,
                    cancellationToken);

                foreach (var result in deepResults)
                {
                    if (!allSources.Any(s => s.DocumentId == result.DocumentId && s.ChunkId == result.ChunkId))
                    {
                        allSources.Add(result);
                    }
                }
            }
        }

        // --- Synthesis: Generate structured report ---
        var synthesisResult = await SynthesizeReportAsync(
            request.Question, allSources, request.Language, cancellationToken);

        totalTokens += synthesisResult.TokensUsed;

        // Build citations from sources
        for (var i = 0; i < allSources.Count; i++)
        {
            var source = allSources[i];
            allCitations.Add(new Citation
            {
                Index = i + 1,
                DocumentId = source.DocumentId,
                DocumentName = source.DocumentName,
                ChunkId = source.ChunkId,
                Quote = source.Content.Length > 200 ? source.Content.Substring(0, 197) + "..." : source.Content,
                PageNumber = source.PageNumber
            });
        }

        stopwatch.Stop();

        _logger.LogInformation(
            "Completed research {ResearchId} in {DurationMs}ms: {SourceCount} sources, {PassCount} passes",
            researchId, stopwatch.ElapsedMilliseconds, allSources.Count, passCount);

        return new ResearchResponse
        {
            ResearchId = researchId,
            Question = request.Question,
            Summary = synthesisResult.Summary,
            Findings = synthesisResult.Findings,
            Citations = allCitations,
            Conclusion = synthesisResult.Conclusion,
            SuggestedFollowUps = synthesisResult.SuggestedFollowUps,
            Metadata = new ResearchMetadata
            {
                Depth = request.Depth,
                SourcesSearched = allSources.Count,
                SourcesUsed = allSources.Count(s => synthesisResult.UsedSourceIds.Contains(s.ChunkId)),
                TotalTokensUsed = totalTokens,
                SearchPasses = passCount,
                DurationMs = (int)stopwatch.ElapsedMilliseconds,
                CompletedAt = DateTime.UtcNow
            }
        };
    }

    private static int GetPassCount(string depth)
    {
        return depth?.ToLowerInvariant() switch
        {
            "quick" => 1,
            "standard" => 2,
            "deep" => 3,
            _ => 2
        };
    }

    private async Task<(IReadOnlyList<string> Queries, int TokensUsed)> GenerateRefinedQueriesAsync(
        string originalQuestion,
        IReadOnlyList<DocumentChunk> initialResults,
        CancellationToken cancellationToken)
    {
        var contextSummary = new StringBuilder();
        foreach (var result in initialResults.Take(5))
        {
            contextSummary.AppendLine($"- {result.DocumentName}: {result.Content.Substring(0, Math.Min(150, result.Content.Length))}");
        }

        var response = await _aiClient.ChatAsync(
            new ChatCompletionRequest
            {
                Messages = new List<ChatMessage>
                {
                    new ChatMessage
                    {
                        Role = "system",
                        Content = "You are a research assistant. Given an initial question and some preliminary findings, generate 2-3 refined follow-up search queries that would help find additional relevant information. Return only the queries, one per line."
                    },
                    new ChatMessage
                    {
                        Role = "user",
                        Content = $"Original question: {originalQuestion}\n\nPreliminary findings:\n{contextSummary}\n\nGenerate refined follow-up search queries:"
                    }
                },
                Temperature = 0.3f,
                MaxTokens = 256
            },
            cancellationToken);

        var queries = response.Content
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(q => q.Length > 5)
            .Take(3)
            .ToList();

        return (queries, response.TotalTokens);
    }

    private async Task<(string Query, int TokensUsed)> GenerateCrossReferenceQueryAsync(
        string originalQuestion,
        IReadOnlyList<DocumentChunk> allSources,
        CancellationToken cancellationToken)
    {
        var sourceSummary = new StringBuilder();
        foreach (var source in allSources.Take(8))
        {
            sourceSummary.AppendLine($"- {source.DocumentName}: {source.Content.Substring(0, Math.Min(100, source.Content.Length))}");
        }

        var response = await _aiClient.ChatAsync(
            new ChatCompletionRequest
            {
                Messages = new List<ChatMessage>
                {
                    new ChatMessage
                    {
                        Role = "system",
                        Content = "You are a research assistant. Given a question and collected sources, generate ONE cross-reference search query that could fill gaps or find contradictions. Return only the query."
                    },
                    new ChatMessage
                    {
                        Role = "user",
                        Content = $"Question: {originalQuestion}\n\nCollected sources:\n{sourceSummary}\n\nCross-reference query:"
                    }
                },
                Temperature = 0.2f,
                MaxTokens = 128
            },
            cancellationToken);

        return (response.Content.Trim(), response.TotalTokens);
    }

    private async Task<SynthesisResult> SynthesizeReportAsync(
        string question,
        IReadOnlyList<DocumentChunk> sources,
        string language,
        CancellationToken cancellationToken)
    {
        var contextBuilder = new StringBuilder();
        var sourceIndex = 0;
        foreach (var source in sources)
        {
            sourceIndex++;
            contextBuilder.AppendLine($"[Source {sourceIndex}: {source.DocumentName} ({source.ChunkId})]");
            contextBuilder.AppendLine(source.Content);
            contextBuilder.AppendLine();
        }

        var languageInstruction = language == "ar"
            ? "Write the report in Arabic."
            : "Write the report in English.";

        var response = await _aiClient.ChatAsync(
            new ChatCompletionRequest
            {
                Messages = new List<ChatMessage>
                {
                    new ChatMessage
                    {
                        Role = "system",
                        Content = $@"You are a research analyst. Synthesize the provided sources into a structured research report.
{languageInstruction}

Respond in this exact JSON format:
{{
  ""summary"": ""A concise 2-3 sentence summary of findings"",
  ""findings"": [
    {{
      ""title"": ""Finding title"",
      ""content"": ""Detailed finding description"",
      ""relevance"": 0.95,
      ""sourceIds"": [""chunkId1""]
    }}
  ],
  ""conclusion"": ""Overall conclusion and recommendations"",
  ""suggestedFollowUps"": [""Follow-up question 1"", ""Follow-up question 2""],
  ""usedSourceIds"": [""chunkId1"", ""chunkId2""]
}}"
                    },
                    new ChatMessage
                    {
                        Role = "user",
                        Content = $"Research question: {question}\n\nSources:\n{contextBuilder}"
                    }
                },
                Temperature = 0.3f,
                MaxTokens = 4096
            },
            cancellationToken);

        try
        {
            var parsed = JsonSerializer.Deserialize<SynthesisJsonResponse>(
                response.Content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (parsed != null)
            {
                return new SynthesisResult
                {
                    Summary = parsed.Summary ?? string.Empty,
                    Findings = parsed.Findings?.Select(f => new ResearchFinding
                    {
                        Title = f.Title ?? string.Empty,
                        Content = f.Content ?? string.Empty,
                        Relevance = f.Relevance,
                        SupportingCitations = Array.Empty<Citation>()
                    }).ToList() ?? new List<ResearchFinding>(),
                    Conclusion = parsed.Conclusion ?? string.Empty,
                    SuggestedFollowUps = parsed.SuggestedFollowUps ?? Array.Empty<string>(),
                    UsedSourceIds = parsed.UsedSourceIds ?? Array.Empty<string>(),
                    TokensUsed = response.TotalTokens
                };
            }
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to parse synthesis JSON, using raw response");
        }

        // Fallback: use raw response as summary
        return new SynthesisResult
        {
            Summary = response.Content,
            Findings = new List<ResearchFinding>(),
            Conclusion = string.Empty,
            SuggestedFollowUps = Array.Empty<string>(),
            UsedSourceIds = Array.Empty<string>(),
            TokensUsed = response.TotalTokens
        };
    }

    // Internal types for JSON deserialization
    private class SynthesisJsonResponse
    {
        public string? Summary { get; set; }
        public List<SynthesisFinding>? Findings { get; set; }
        public string? Conclusion { get; set; }
        public List<string>? SuggestedFollowUps { get; set; }
        public List<string>? UsedSourceIds { get; set; }
    }

    private class SynthesisFinding
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public float Relevance { get; set; }
        public List<string>? SourceIds { get; set; }
    }

    private class SynthesisResult
    {
        public string Summary { get; set; } = string.Empty;
        public IReadOnlyList<ResearchFinding> Findings { get; set; } = Array.Empty<ResearchFinding>();
        public string Conclusion { get; set; } = string.Empty;
        public IReadOnlyList<string> SuggestedFollowUps { get; set; } = Array.Empty<string>();
        public IReadOnlyList<string> UsedSourceIds { get; set; } = Array.Empty<string>();
        public int TokensUsed { get; set; }
    }
}
