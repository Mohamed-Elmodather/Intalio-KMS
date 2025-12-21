using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.AI.Application.DTOs;
using AFC27.KMS.AI.Application.Interfaces;
using AFC27.KMS.SharedKernel.Interfaces;

using AFC27.KMS.Documents.Domain.Entities;

namespace AFC27.KMS.AI.Application.Services;

/// <summary>
/// ACL-aware RAG (Retrieval Augmented Generation) service.
/// Only retrieves documents the user has permission to access.
/// </summary>
public class RAGService : IRAGService
{
    private readonly IIntalioAIClient _aiClient;
    private readonly IEmbeddingService _embeddingService;
    private readonly DbContext _dbContext;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<RAGService> _logger;

    private const string SystemPrompt = @"You are the AFC Asian Cup 2027 Knowledge Assistant. Your role is to help users find information from the knowledge base.

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

    public RAGService(
        IIntalioAIClient aiClient,
        IEmbeddingService embeddingService,
        DbContext dbContext,
        ICurrentUser currentUser,
        ILogger<RAGService> logger)
    {
        _aiClient = aiClient;
        _embeddingService = embeddingService;
        _dbContext = dbContext;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<RAGResponse> QueryAsync(
        RAGRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("RAG query from user {UserId}: {Query}",
            _currentUser.UserId, request.Query);

        // 1. Search for relevant documents (ACL-filtered)
        var sources = await SearchAsync(
            request.Query,
            _currentUser.UserId ?? Guid.Empty,
            request.MaxSources,
            cancellationToken);

        if (!sources.Any())
        {
            return new RAGResponse
            {
                Answer = "I couldn't find any relevant documents in your accessible knowledge base to answer this question. Please try a different query or check if you have access to the relevant libraries.",
                Citations = Array.Empty<Citation>(),
                Sources = sources,
                TokensUsed = 0
            };
        }

        // 2. Build context from sources
        var context = BuildContext(sources);

        // 3. Build messages for AI
        var messages = BuildMessages(request, context);

        // 4. Get AI response
        var aiResponse = await _aiClient.ChatAsync(
            new ChatCompletionRequest
            {
                Messages = messages,
                Temperature = 0.3f, // Lower temperature for factual responses
                MaxTokens = 2048
            },
            cancellationToken);

        // 5. Extract citations from response
        var citations = ExtractCitations(aiResponse.Content, sources);

        return new RAGResponse
        {
            Answer = aiResponse.Content,
            Citations = citations,
            Sources = sources,
            SuggestedFollowUp = GenerateFollowUp(request.Query, sources),
            TokensUsed = aiResponse.TotalTokens
        };
    }

    public async IAsyncEnumerable<RAGStreamChunk> QueryStreamAsync(
        RAGRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("RAG streaming query from user {UserId}: {Query}",
            _currentUser.UserId, request.Query);

        // 1. Search for relevant documents (ACL-filtered)
        var sources = await SearchAsync(
            request.Query,
            _currentUser.UserId ?? Guid.Empty,
            request.MaxSources,
            cancellationToken);

        // Send sources first
        yield return new RAGStreamChunk
        {
            Delta = "",
            IsComplete = false,
            Sources = sources
        };

        if (!sources.Any())
        {
            yield return new RAGStreamChunk
            {
                Delta = "I couldn't find any relevant documents in your accessible knowledge base to answer this question.",
                IsComplete = true,
                Citations = Array.Empty<Citation>()
            };
            yield break;
        }

        // 2. Build context and messages
        var context = BuildContext(sources);
        var messages = BuildMessages(request, context);

        // 3. Stream AI response
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
                // Extract citations from complete response
                var citations = ExtractCitations(fullResponse.ToString(), sources);
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

    public async Task<IReadOnlyList<DocumentChunk>> SearchAsync(
        string query,
        Guid userId,
        int maxResults = 10,
        CancellationToken cancellationToken = default)
    {
        // Get user's accessible document IDs based on permissions
        var accessibleDocumentIds = await GetAccessibleDocumentIdsAsync(userId, cancellationToken);

        if (!accessibleDocumentIds.Any())
        {
            _logger.LogWarning("User {UserId} has no accessible documents", userId);
            return Array.Empty<DocumentChunk>();
        }

        // Search using embedding service with document filter
        var similarDocs = await _embeddingService.SearchSimilarAsync(
            query,
            maxResults * 2, // Get more to filter
            new Dictionary<string, string>
            {
                { "user_accessible", string.Join(",", accessibleDocumentIds.Take(100)) }
            },
            cancellationToken);

        // Convert to DocumentChunk and filter by accessibility
        var chunks = new List<DocumentChunk>();
        foreach (var doc in similarDocs)
        {
            if (!accessibleDocumentIds.Contains(doc.DocumentId))
                continue;

            // Get document metadata
            var document = await _dbContext.Set<Document>()
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == doc.DocumentId, cancellationToken);

            if (document == null)
                continue;

            chunks.Add(new DocumentChunk
            {
                DocumentId = doc.DocumentId,
                DocumentName = document.Name,
                ChunkId = doc.ChunkId,
                Content = doc.Content,
                RelevanceScore = doc.Score,
                PageNumber = doc.Metadata.TryGetValue("page", out var page) ? int.Parse(page) : null,
                Section = doc.Metadata.GetValueOrDefault("section"),
                Metadata = doc.Metadata
            });

            if (chunks.Count >= maxResults)
                break;
        }

        return chunks;
    }

    private async Task<HashSet<Guid>> GetAccessibleDocumentIdsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        // Get all documents where user has read permission
        // This checks:
        // 1. Documents owned by user
        // 2. Documents in libraries user has access to
        // 3. Documents shared directly with user
        // 4. Documents in public libraries

        var accessibleIds = new HashSet<Guid>();

        // 1. Documents owned by user
        var ownedDocs = await _dbContext.Set<Document>()
            .AsNoTracking()
            .Where(d => d.CreatedBy == userId && !d.IsDeleted)
            .Select(d => d.Id)
            .ToListAsync(cancellationToken);

        foreach (var id in ownedDocs)
            accessibleIds.Add(id);

        // 2. Documents in libraries where user has access
        // Get libraries with permissions for user
        var libraryPermissions = await _dbContext.Set<LibraryPermission>()
            .AsNoTracking()
            .Where(lp => lp.UserId == userId && lp.AccessLevel >= LibraryAccessLevel.Read) // Has Read permission or higher
            .Select(lp => lp.LibraryId)
            .ToListAsync(cancellationToken);

        if (libraryPermissions.Any())
        {
            var libraryDocs = await _dbContext.Set<Document>()
                .AsNoTracking()
                .Where(d => libraryPermissions.Contains(d.LibraryId) && !d.IsDeleted)
                .Select(d => d.Id)
                .ToListAsync(cancellationToken);

            foreach (var id in libraryDocs)
                accessibleIds.Add(id);
        }

        // 3. Documents shared directly with user
        // TODO: Implement DocumentShare entity when sharing feature is added
        // var sharedDocs = await _dbContext.Set<DocumentShare>()
        //     .AsNoTracking()
        //     .Where(ds => ds.SharedWithUserId == userId &&
        //                  (ds.ExpiresAt == null || ds.ExpiresAt > DateTime.UtcNow))
        //     .Select(ds => ds.DocumentId)
        //     .ToListAsync(cancellationToken);
        //
        // foreach (var id in sharedDocs)
        //     accessibleIds.Add(id);

        _logger.LogDebug("User {UserId} has access to {Count} documents", userId, accessibleIds.Count);

        return accessibleIds;
    }

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

    private static IReadOnlyList<ChatMessage> BuildMessages(RAGRequest request, string context)
    {
        var messages = new List<ChatMessage>();

        // System message with context
        messages.Add(new ChatMessage
        {
            Role = "system",
            Content = string.Format(SystemPrompt, context, request.Query)
        });

        // Add previous messages for conversation context
        if (request.PreviousMessages != null)
        {
            foreach (var msg in request.PreviousMessages.TakeLast(10))
            {
                messages.Add(msg);
            }
        }

        // Current user query
        messages.Add(new ChatMessage
        {
            Role = "user",
            Content = request.Query
        });

        return messages;
    }

    private static IReadOnlyList<Citation> ExtractCitations(
        string response,
        IReadOnlyList<DocumentChunk> sources)
    {
        var citations = new List<Citation>();

        // Find [Source N] references in the response
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

        // If no explicit citations found but sources were used, add implicit citations
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

        // Generate contextual follow-up suggestions
        var queryLower = query.ToLower();

        if (queryLower.Contains("what") || queryLower.Contains("how"))
            return "Would you like me to provide more details or examples?";

        if (queryLower.Contains("list") || queryLower.Contains("all"))
            return "Would you like me to elaborate on any specific item?";

        return "Is there anything else you'd like to know about this topic?";
    }
}
