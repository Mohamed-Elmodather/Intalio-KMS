using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using AFC27.KMS.AI.Application.DTOs;
using AFC27.KMS.AI.Application.Interfaces;

namespace AFC27.KMS.AI.Infrastructure.Clients;

/// <summary>
/// Mock implementation of the Intalio AI Client for development and testing.
/// Simulates AI responses with realistic delays and streaming behavior.
/// </summary>
public class MockIntalioAIClient : IIntalioAIClient
{
    private readonly ILogger<MockIntalioAIClient> _logger;
    private readonly Random _random = new();

    // Mock responses for different types of queries
    private static readonly string[] GeneralResponses = new[]
    {
        "Based on the available information, I can help you understand this topic better.",
        "Let me analyze this question and provide a comprehensive response.",
        "This is an interesting query. Here's what I found:",
        "I'll do my best to answer your question based on the context provided."
    };

    private static readonly string[] DocumentResponses = new[]
    {
        "According to the documents in your knowledge base, {0}",
        "Based on the indexed content, I found that {0}",
        "The relevant documents indicate that {0}",
        "From my analysis of the available sources, {0}"
    };

    public MockIntalioAIClient(ILogger<MockIntalioAIClient> logger)
    {
        _logger = logger;
    }

    public async Task<ChatCompletionResponse> ChatAsync(
        ChatCompletionRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock AI: Processing chat request with {MessageCount} messages",
            request.Messages.Count);

        // Simulate processing delay
        await Task.Delay(_random.Next(500, 1500), cancellationToken);

        var userMessage = request.Messages.LastOrDefault(m => m.Role == "user")?.Content ?? "";
        var response = GenerateMockResponse(userMessage);

        var promptTokens = EstimateTokens(string.Join(" ", request.Messages.Select(m => m.Content)));
        var completionTokens = EstimateTokens(response);

        return new ChatCompletionResponse
        {
            Id = $"mock-{Guid.NewGuid():N}",
            Content = response,
            Model = "mock-gpt-4",
            PromptTokens = promptTokens,
            CompletionTokens = completionTokens,
            TotalTokens = promptTokens + completionTokens,
            FinishReason = "stop"
        };
    }

    public async IAsyncEnumerable<ChatStreamChunk> ChatStreamAsync(
        ChatCompletionRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock AI: Starting streaming chat response");

        var userMessage = request.Messages.LastOrDefault(m => m.Role == "user")?.Content ?? "";
        var fullResponse = GenerateMockResponse(userMessage);
        var id = $"mock-stream-{Guid.NewGuid():N}";

        // Split response into words and stream them
        var words = fullResponse.Split(' ');

        for (int i = 0; i < words.Length; i++)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            // Add space before word (except first)
            var delta = i == 0 ? words[i] : " " + words[i];

            yield return new ChatStreamChunk
            {
                Id = id,
                Delta = delta,
                IsComplete = false
            };

            // Simulate typing delay (30-80ms per word)
            await Task.Delay(_random.Next(30, 80), cancellationToken);
        }

        // Final chunk
        yield return new ChatStreamChunk
        {
            Id = id,
            Delta = "",
            IsComplete = true,
            FinishReason = "stop"
        };
    }

    public async Task<EmbeddingResponse> GetEmbeddingsAsync(
        EmbeddingRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock AI: Generating embeddings for {TextCount} texts",
            request.Texts.Count);

        // Simulate processing delay
        await Task.Delay(_random.Next(100, 300) * request.Texts.Count, cancellationToken);

        var embeddings = new List<EmbeddingData>();
        var totalTokens = 0;

        for (int i = 0; i < request.Texts.Count; i++)
        {
            var text = request.Texts[i];
            var tokens = EstimateTokens(text);
            totalTokens += tokens;

            // Generate deterministic mock embedding based on text hash
            var embedding = GenerateMockEmbedding(text);

            embeddings.Add(new EmbeddingData
            {
                Index = i,
                Embedding = embedding
            });
        }

        return new EmbeddingResponse
        {
            Embeddings = embeddings,
            TotalTokens = totalTokens
        };
    }

    public Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        // Mock is always available
        return Task.FromResult(true);
    }

    private string GenerateMockResponse(string query)
    {
        // Generate contextual mock response based on query keywords
        var queryLower = query.ToLower();

        if (queryLower.Contains("hello") || queryLower.Contains("hi"))
        {
            return "Hello! I'm the AFC Asian Cup 2027 Knowledge Assistant. How can I help you today? I can answer questions about documents, articles, and general information in your knowledge base.";
        }

        if (queryLower.Contains("help"))
        {
            return "I'm here to help! You can ask me questions about:\n\n1. **Documents** - Search and summarize documents in your libraries\n2. **Articles** - Find relevant articles and content\n3. **General Knowledge** - Ask questions about any topic in your knowledge base\n\nJust type your question and I'll search the relevant sources to provide an accurate answer.";
        }

        if (queryLower.Contains("document") || queryLower.Contains("file"))
        {
            return string.Format(DocumentResponses[_random.Next(DocumentResponses.Length)],
                "the document management system allows you to organize files in libraries, set permissions, and collaborate with team members. Documents can be versioned, shared, and searched using full-text search.");
        }

        if (queryLower.Contains("asian cup") || queryLower.Contains("afc"))
        {
            return "The AFC Asian Cup 2027 is a major football tournament organized by the Asian Football Confederation. This Knowledge Management System is designed to support the organization and management of information related to the event, including:\n\n1. **Document Management** - Store and organize official documents\n2. **Content Studio** - Create and collaborate on articles\n3. **AI-Powered Search** - Find information quickly using natural language\n4. **Multi-language Support** - Full Arabic and English support";
        }

        if (queryLower.Contains("search") || queryLower.Contains("find"))
        {
            return "To search for information, you can:\n\n1. Use the search bar at the top of the page\n2. Ask me directly in this chat - I'll search relevant documents for you\n3. Browse through libraries and folders\n\nI use semantic search to understand your intent and find the most relevant results, even if they don't contain the exact words you used.";
        }

        if (queryLower.Contains("permission") || queryLower.Contains("access"))
        {
            return "The permission system supports granular access control:\n\n- **Read** - View documents and content\n- **Write** - Edit and update content\n- **Delete** - Remove documents\n- **Share** - Share with other users\n- **Manage** - Administer permissions\n\nPermissions can be set at the library, folder, or individual document level, and they inherit down the hierarchy unless overridden.";
        }

        // Default response with context
        var intro = GeneralResponses[_random.Next(GeneralResponses.Length)];
        return $"{intro}\n\nRegarding your question about \"{query}\", I would need more specific context to provide a detailed answer. You can:\n\n1. Ask a more specific question\n2. Reference particular documents or topics\n3. Use the search feature to find relevant content\n\nHow can I assist you further?";
    }

    private float[] GenerateMockEmbedding(string text)
    {
        // Generate a 1536-dimensional embedding (OpenAI ada-002 size)
        const int dimensions = 1536;
        var embedding = new float[dimensions];

        // Use text hash to generate deterministic but varied embeddings
        var hash = text.GetHashCode();
        var localRandom = new Random(hash);

        for (int i = 0; i < dimensions; i++)
        {
            // Generate values between -1 and 1
            embedding[i] = (float)(localRandom.NextDouble() * 2 - 1);
        }

        // Normalize the vector
        var magnitude = (float)Math.Sqrt(embedding.Sum(x => x * x));
        for (int i = 0; i < dimensions; i++)
        {
            embedding[i] /= magnitude;
        }

        return embedding;
    }

    private static int EstimateTokens(string text)
    {
        // Rough estimate: ~4 characters per token for English
        return (int)Math.Ceiling(text.Length / 4.0);
    }
}
