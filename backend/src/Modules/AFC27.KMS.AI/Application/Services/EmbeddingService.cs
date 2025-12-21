using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using AFC27.KMS.AI.Application.DTOs;
using AFC27.KMS.AI.Application.Interfaces;

namespace AFC27.KMS.AI.Application.Services;

/// <summary>
/// Service for document embedding and semantic search.
/// Uses in-memory storage for POC - production would use a vector database.
/// </summary>
public class EmbeddingService : IEmbeddingService
{
    private readonly IIntalioAIClient _aiClient;
    private readonly ILogger<EmbeddingService> _logger;

    // In-memory vector store for POC
    // In production, use Pinecone, Weaviate, Qdrant, or similar
    private static readonly Dictionary<Guid, List<DocumentEmbedding>> _documentEmbeddings = new();
    private static readonly object _lock = new();

    private const int ChunkSize = 1000; // Characters per chunk
    private const int ChunkOverlap = 200; // Overlap between chunks

    public EmbeddingService(
        IIntalioAIClient aiClient,
        ILogger<EmbeddingService> logger)
    {
        _aiClient = aiClient;
        _logger = logger;
    }

    public async Task IndexDocumentAsync(
        Guid documentId,
        string content,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Indexing document {DocumentId}", documentId);

        // Remove existing embeddings for this document
        await RemoveDocumentAsync(documentId, cancellationToken);

        // Split content into chunks
        var chunks = ChunkContent(content);
        _logger.LogDebug("Document {DocumentId} split into {ChunkCount} chunks", documentId, chunks.Count);

        // Get embeddings for all chunks
        var embeddingResponse = await _aiClient.GetEmbeddingsAsync(
            new EmbeddingRequest { Texts = chunks },
            cancellationToken);

        // Store embeddings
        var docEmbeddings = new List<DocumentEmbedding>();
        for (int i = 0; i < chunks.Count; i++)
        {
            var chunkMetadata = metadata != null
                ? new Dictionary<string, string>(metadata)
                : new Dictionary<string, string>();

            chunkMetadata["chunk_index"] = i.ToString();
            chunkMetadata["total_chunks"] = chunks.Count.ToString();

            // Try to extract page number from content
            var pageMatch = Regex.Match(chunks[i], @"page\s*(\d+)", RegexOptions.IgnoreCase);
            if (pageMatch.Success)
                chunkMetadata["page"] = pageMatch.Groups[1].Value;

            docEmbeddings.Add(new DocumentEmbedding
            {
                DocumentId = documentId,
                ChunkId = $"{documentId}:chunk_{i}",
                Content = chunks[i],
                Embedding = embeddingResponse.Embeddings[i].Embedding,
                Metadata = chunkMetadata,
                IndexedAt = DateTime.UtcNow
            });
        }

        lock (_lock)
        {
            _documentEmbeddings[documentId] = docEmbeddings;
        }

        _logger.LogInformation(
            "Document {DocumentId} indexed successfully with {ChunkCount} chunks",
            documentId, chunks.Count);
    }

    public Task RemoveDocumentAsync(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_documentEmbeddings.Remove(documentId))
            {
                _logger.LogInformation("Removed embeddings for document {DocumentId}", documentId);
            }
        }

        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<SimilarDocument>> SearchSimilarAsync(
        string query,
        int maxResults = 10,
        Dictionary<string, string>? filter = null,
        CancellationToken cancellationToken = default)
    {
        // Get query embedding
        var queryEmbedding = await GetEmbeddingAsync(query, cancellationToken);

        var results = new List<(DocumentEmbedding Embedding, float Score)>();

        // Parse filter for accessible document IDs
        HashSet<Guid>? accessibleIds = null;
        if (filter?.TryGetValue("user_accessible", out var accessibleStr) == true)
        {
            accessibleIds = accessibleStr
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => Guid.TryParse(s.Trim(), out var id) ? id : Guid.Empty)
                .Where(id => id != Guid.Empty)
                .ToHashSet();
        }

        lock (_lock)
        {
            foreach (var (docId, embeddings) in _documentEmbeddings)
            {
                // Apply access filter
                if (accessibleIds != null && !accessibleIds.Contains(docId))
                    continue;

                foreach (var embedding in embeddings)
                {
                    var score = CosineSimilarity(queryEmbedding, embedding.Embedding);
                    results.Add((embedding, score));
                }
            }
        }

        return results
            .OrderByDescending(r => r.Score)
            .Take(maxResults)
            .Select(r => new SimilarDocument
            {
                DocumentId = r.Embedding.DocumentId,
                ChunkId = r.Embedding.ChunkId,
                Content = r.Embedding.Content,
                Score = r.Score,
                Metadata = r.Embedding.Metadata
            })
            .ToList();
    }

    public async Task<float[]> GetEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        var response = await _aiClient.GetEmbeddingsAsync(
            new EmbeddingRequest { Texts = new[] { text } },
            cancellationToken);

        return response.Embeddings.First().Embedding;
    }

    /// <summary>
    /// Get indexed document count for status reporting.
    /// </summary>
    public int GetIndexedDocumentCount()
    {
        lock (_lock)
        {
            return _documentEmbeddings.Count;
        }
    }

    /// <summary>
    /// Get last index update time.
    /// </summary>
    public DateTime? GetLastIndexUpdate()
    {
        lock (_lock)
        {
            return _documentEmbeddings.Values
                .SelectMany(e => e)
                .OrderByDescending(e => e.IndexedAt)
                .FirstOrDefault()?.IndexedAt;
        }
    }

    private static List<string> ChunkContent(string content)
    {
        var chunks = new List<string>();

        if (string.IsNullOrWhiteSpace(content))
            return chunks;

        // Clean content
        content = Regex.Replace(content, @"\s+", " ").Trim();

        // Split by paragraphs first
        var paragraphs = content.Split(new[] { "\n\n", "\r\n\r\n" },
            StringSplitOptions.RemoveEmptyEntries);

        var currentChunk = new StringBuilder();

        foreach (var paragraph in paragraphs)
        {
            // If paragraph fits in current chunk
            if (currentChunk.Length + paragraph.Length <= ChunkSize)
            {
                if (currentChunk.Length > 0)
                    currentChunk.Append(' ');
                currentChunk.Append(paragraph);
            }
            else
            {
                // Save current chunk if not empty
                if (currentChunk.Length > 0)
                {
                    chunks.Add(currentChunk.ToString());

                    // Start new chunk with overlap
                    var overlap = GetOverlapText(currentChunk.ToString(), ChunkOverlap);
                    currentChunk.Clear();
                    currentChunk.Append(overlap);
                }

                // Handle large paragraphs
                if (paragraph.Length > ChunkSize)
                {
                    var words = paragraph.Split(' ');
                    foreach (var word in words)
                    {
                        if (currentChunk.Length + word.Length + 1 > ChunkSize)
                        {
                            chunks.Add(currentChunk.ToString());
                            var overlap = GetOverlapText(currentChunk.ToString(), ChunkOverlap);
                            currentChunk.Clear();
                            currentChunk.Append(overlap);
                        }

                        if (currentChunk.Length > 0)
                            currentChunk.Append(' ');
                        currentChunk.Append(word);
                    }
                }
                else
                {
                    currentChunk.Append(paragraph);
                }
            }
        }

        // Add final chunk
        if (currentChunk.Length > 0)
            chunks.Add(currentChunk.ToString());

        return chunks;
    }

    private static string GetOverlapText(string text, int maxLength)
    {
        if (text.Length <= maxLength)
            return text;

        // Try to break at word boundary
        var overlap = text.Substring(text.Length - maxLength);
        var spaceIndex = overlap.IndexOf(' ');
        if (spaceIndex > 0)
            overlap = overlap.Substring(spaceIndex + 1);

        return overlap;
    }

    private static float CosineSimilarity(float[] a, float[] b)
    {
        if (a.Length != b.Length)
            return 0;

        float dotProduct = 0;
        float magnitudeA = 0;
        float magnitudeB = 0;

        for (int i = 0; i < a.Length; i++)
        {
            dotProduct += a[i] * b[i];
            magnitudeA += a[i] * a[i];
            magnitudeB += b[i] * b[i];
        }

        magnitudeA = (float)Math.Sqrt(magnitudeA);
        magnitudeB = (float)Math.Sqrt(magnitudeB);

        if (magnitudeA == 0 || magnitudeB == 0)
            return 0;

        return dotProduct / (magnitudeA * magnitudeB);
    }

    private class DocumentEmbedding
    {
        public Guid DocumentId { get; init; }
        public string ChunkId { get; init; } = string.Empty;
        public string Content { get; init; } = string.Empty;
        public float[] Embedding { get; init; } = Array.Empty<float>();
        public Dictionary<string, string> Metadata { get; init; } = new();
        public DateTime IndexedAt { get; init; }
    }
}
