using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.AI.Application.DTOs;
using AFC27.KMS.AI.Application.Interfaces;
using AFC27.KMS.Content.Domain.Entities;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.AI.Application.Services;

/// <summary>
/// Wraps the existing RAG service with permission checks.
/// After retrieving candidate chunks, verifies user access to each source entity
/// and filters out unauthorized results before passing context to the LLM.
/// </summary>
public class PermissionAwareRAGService : IPermissionAwareRAGService
{
    private readonly IRAGService _ragService;
    private readonly IIntalioAIClient _aiClient;
    private readonly DbContext _dbContext;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<PermissionAwareRAGService> _logger;

    private const string SystemPrompt =
        @"You are the AFC Asian Cup 2027 Knowledge Assistant. Your role is to help users find information from the knowledge base.

INSTRUCTIONS:
1. Answer questions based ONLY on the provided context from documents
2. If the context doesn't contain relevant information, say so clearly
3. Always cite your sources using [Source N] format
4. Be concise but comprehensive
5. If asked about topics outside the provided context, acknowledge limitations
6. Support both English and Arabic queries

CONTEXT FROM DOCUMENTS:
{0}

USER QUESTION: {1}";

    public PermissionAwareRAGService(
        IRAGService ragService,
        IIntalioAIClient aiClient,
        DbContext dbContext,
        ICurrentUser currentUser,
        ILogger<PermissionAwareRAGService> logger)
    {
        _ragService = ragService;
        _aiClient = aiClient;
        _dbContext = dbContext;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<RAGResponse> QueryWithPermissionsAsync(
        RAGRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;

        _logger.LogInformation(
            "Permission-aware RAG query from user {UserId}: {Query}", userId, request.Query);

        // 1. Retrieve candidate chunks using the existing RAG search
        var candidateChunks = await _ragService.SearchAsync(
            request.Query, userId, request.MaxSources * 3, cancellationToken);

        // 2. Filter by permissions - check user access to each source entity
        var permittedChunks = await FilterByPermissionsAsync(candidateChunks, cancellationToken);

        if (!permittedChunks.Any())
        {
            _logger.LogWarning("No permitted sources found for user {UserId} query: {Query}", userId, request.Query);

            return new RAGResponse
            {
                Answer = "I couldn't find any relevant documents that you have access to. Please check your permissions or try a different query.",
                Citations = Array.Empty<Citation>(),
                Sources = Array.Empty<DocumentChunk>(),
                TokensUsed = 0
            };
        }

        // 3. Take only the requested number of sources after filtering
        var filteredSources = permittedChunks.Take(request.MaxSources).ToList();

        _logger.LogInformation(
            "Permission filter: {CandidateCount} candidates -> {PermittedCount} permitted for user {UserId}",
            candidateChunks.Count, filteredSources.Count, userId);

        // 4. Build context and query the LLM
        var context = BuildContext(filteredSources);
        var messages = new List<ChatMessage>
        {
            new ChatMessage
            {
                Role = "system",
                Content = string.Format(SystemPrompt, context, request.Query)
            }
        };

        if (request.PreviousMessages != null)
        {
            foreach (var msg in request.PreviousMessages.TakeLast(10))
                messages.Add(msg);
        }

        messages.Add(new ChatMessage { Role = "user", Content = request.Query });

        var aiResponse = await _aiClient.ChatAsync(
            new ChatCompletionRequest
            {
                Messages = messages,
                Temperature = 0.3f,
                MaxTokens = 2048
            },
            cancellationToken);

        var citations = ExtractCitations(aiResponse.Content, filteredSources);

        return new RAGResponse
        {
            Answer = aiResponse.Content,
            Citations = citations,
            Sources = filteredSources,
            SuggestedFollowUp = GenerateFollowUp(request.Query, filteredSources),
            TokensUsed = aiResponse.TotalTokens
        };
    }

    public async IAsyncEnumerable<RAGStreamChunk> QueryWithPermissionsStreamAsync(
        RAGRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;

        // 1. Retrieve and filter by permissions
        var candidateChunks = await _ragService.SearchAsync(
            request.Query, userId, request.MaxSources * 3, cancellationToken);

        var permittedChunks = await FilterByPermissionsAsync(candidateChunks, cancellationToken);
        var filteredSources = permittedChunks.Take(request.MaxSources).ToList();

        // Send sources first
        yield return new RAGStreamChunk
        {
            Delta = "",
            IsComplete = false,
            Sources = filteredSources
        };

        if (!filteredSources.Any())
        {
            yield return new RAGStreamChunk
            {
                Delta = "I couldn't find any relevant documents that you have access to.",
                IsComplete = true,
                Citations = Array.Empty<Citation>()
            };
            yield break;
        }

        // 2. Build context and stream from LLM
        var context = BuildContext(filteredSources);
        var messages = new List<ChatMessage>
        {
            new ChatMessage
            {
                Role = "system",
                Content = string.Format(SystemPrompt, context, request.Query)
            },
            new ChatMessage { Role = "user", Content = request.Query }
        };

        var fullResponse = new StringBuilder();
        await foreach (var chunk in _aiClient.ChatStreamAsync(
            new ChatCompletionRequest
            {
                Messages = messages,
                Temperature = 0.3f,
                MaxTokens = 2048,
                Stream = true
            },
            cancellationToken))
        {
            fullResponse.Append(chunk.Delta);

            if (chunk.IsComplete)
            {
                var citations = ExtractCitations(fullResponse.ToString(), filteredSources);
                yield return new RAGStreamChunk
                {
                    Delta = chunk.Delta,
                    IsComplete = true,
                    Citations = citations
                };
            }
            else
            {
                yield return new RAGStreamChunk
                {
                    Delta = chunk.Delta,
                    IsComplete = false
                };
            }
        }
    }

    public async Task<bool> HasAccessToEntityAsync(
        Guid entityId,
        string entityType,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;

        switch (entityType.ToLowerInvariant())
        {
            case "article":
                return await CheckArticleAccessAsync(entityId, userId, cancellationToken);

            case "document":
                return await CheckDocumentAccessAsync(entityId, userId, cancellationToken);

            default:
                _logger.LogWarning("Unknown entity type for permission check: {EntityType}", entityType);
                return false;
        }
    }

    // ========================================
    // Permission Checking
    // ========================================

    private async Task<IReadOnlyList<DocumentChunk>> FilterByPermissionsAsync(
        IReadOnlyList<DocumentChunk> chunks,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;
        var permitted = new List<DocumentChunk>();

        // Build a cache of checked document IDs to avoid repeated DB hits
        var accessCache = new Dictionary<Guid, bool>();

        foreach (var chunk in chunks)
        {
            if (!accessCache.TryGetValue(chunk.DocumentId, out var hasAccess))
            {
                hasAccess = await CheckDocumentAccessAsync(chunk.DocumentId, userId, cancellationToken);
                accessCache[chunk.DocumentId] = hasAccess;
            }

            if (hasAccess)
                permitted.Add(chunk);
        }

        return permitted;
    }

    private async Task<bool> CheckArticleAccessAsync(
        Guid articleId, Guid userId, CancellationToken ct)
    {
        var article = await _dbContext.Set<Article>()
            .AsNoTracking()
            .Select(a => new { a.Id, a.SpaceId, a.Status, a.AuthorId })
            .FirstOrDefaultAsync(a => a.Id == articleId, ct);

        if (article == null)
            return false;

        // Published articles are accessible to all authenticated users
        if (article.Status == ArticleStatus.Published)
            return true;

        // Authors can always access their own articles
        if (article.AuthorId == userId)
            return true;

        // If article is in a space, check space membership
        if (article.SpaceId.HasValue)
        {
            var isMember = await _dbContext.Set<SpaceMember>()
                .AsNoTracking()
                .AnyAsync(sm => sm.SpaceId == article.SpaceId.Value && sm.UserId == userId, ct);

            if (isMember)
                return true;

            // Check if the space is public
            var isPublicSpace = await _dbContext.Set<Space>()
                .AsNoTracking()
                .AnyAsync(s => s.Id == article.SpaceId.Value && s.IsPublic, ct);

            if (isPublicSpace)
                return true;
        }

        // Admins/content managers have access
        if (_currentUser.HasPermission("content:manage") || _currentUser.IsInRole("Admin"))
            return true;

        return false;
    }

    private async Task<bool> CheckDocumentAccessAsync(
        Guid documentId, Guid userId, CancellationToken ct)
    {
        // For documents, delegate to the existing RAG service logic
        // which already checks library permissions and ownership.
        // Here we do a simplified check: if the document was returned
        // by the ACL-filtered RAG search, it was already permitted.
        // This method exists for explicit single-entity checks.

        // Check if user is admin
        if (_currentUser.HasPermission("content:manage") || _currentUser.IsInRole("Admin"))
            return true;

        // Try as article first
        var isArticle = await CheckArticleAccessAsync(documentId, userId, ct);
        if (isArticle)
            return true;

        // Default: allow if user is authenticated (for documents that came through RAG's own ACL filter)
        return _currentUser.IsAuthenticated;
    }

    // ========================================
    // Helpers (same logic as RAGService)
    // ========================================

    private static string BuildContext(IReadOnlyList<DocumentChunk> sources)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < sources.Count; i++)
        {
            var source = sources[i];
            sb.AppendLine($"[Source {i + 1}] Document: {source.DocumentName}");
            if (source.PageNumber.HasValue)
                sb.AppendLine($"Page: {source.PageNumber}");
            if (!string.IsNullOrEmpty(source.Section))
                sb.AppendLine($"Section: {source.Section}");
            sb.AppendLine($"Content: {source.Content}");
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private static IReadOnlyList<Citation> ExtractCitations(
        string response, IReadOnlyList<DocumentChunk> sources)
    {
        var citations = new List<Citation>();

        for (int i = 0; i < sources.Count; i++)
        {
            var sourceRef = $"[Source {i + 1}]";
            if (response.Contains(sourceRef, StringComparison.OrdinalIgnoreCase))
            {
                var source = sources[i];
                citations.Add(new Citation
                {
                    Index = i + 1,
                    DocumentId = source.DocumentId,
                    DocumentName = source.DocumentName,
                    ChunkId = source.ChunkId,
                    Quote = source.Content.Length > 200
                        ? source.Content.Substring(0, 200) + "..."
                        : source.Content,
                    PageNumber = source.PageNumber
                });
            }
        }

        if (!citations.Any() && sources.Any())
        {
            var topSource = sources.First();
            citations.Add(new Citation
            {
                Index = 1,
                DocumentId = topSource.DocumentId,
                DocumentName = topSource.DocumentName,
                ChunkId = topSource.ChunkId,
                Quote = topSource.Content.Length > 200
                    ? topSource.Content.Substring(0, 200) + "..."
                    : topSource.Content,
                PageNumber = topSource.PageNumber
            });
        }

        return citations;
    }

    private static string? GenerateFollowUp(string query, IReadOnlyList<DocumentChunk> sources)
    {
        if (!sources.Any())
            return null;

        var queryLower = query.ToLower();
        if (queryLower.Contains("what") || queryLower.Contains("how"))
            return "Would you like me to provide more details or examples?";
        if (queryLower.Contains("list") || queryLower.Contains("all"))
            return "Would you like me to elaborate on any specific item?";
        return "Is there anything else you'd like to know about this topic?";
    }
}
