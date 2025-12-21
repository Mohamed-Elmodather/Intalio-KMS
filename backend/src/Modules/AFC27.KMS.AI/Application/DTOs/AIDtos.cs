namespace AFC27.KMS.AI.Application.DTOs;

/// <summary>
/// Chat completion request.
/// </summary>
public record ChatCompletionRequest
{
    public IReadOnlyList<ChatMessage> Messages { get; init; } = Array.Empty<ChatMessage>();
    public string Model { get; init; } = "gpt-4";
    public float Temperature { get; init; } = 0.7f;
    public int MaxTokens { get; init; } = 2048;
    public bool Stream { get; init; } = false;
    public string? SystemPrompt { get; init; }
}

/// <summary>
/// Chat message.
/// </summary>
public record ChatMessage
{
    public string Role { get; init; } = "user"; // "system", "user", "assistant"
    public string Content { get; init; } = string.Empty;
}

/// <summary>
/// Chat completion response.
/// </summary>
public record ChatCompletionResponse
{
    public string Id { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public int PromptTokens { get; init; }
    public int CompletionTokens { get; init; }
    public int TotalTokens { get; init; }
    public string FinishReason { get; init; } = string.Empty;
}

/// <summary>
/// Streaming chat chunk.
/// </summary>
public record ChatStreamChunk
{
    public string Id { get; init; } = string.Empty;
    public string Delta { get; init; } = string.Empty;
    public bool IsComplete { get; init; }
    public string? FinishReason { get; init; }
}

/// <summary>
/// Embedding request.
/// </summary>
public record EmbeddingRequest
{
    public IReadOnlyList<string> Texts { get; init; } = Array.Empty<string>();
    public string Model { get; init; } = "text-embedding-ada-002";
}

/// <summary>
/// Embedding response.
/// </summary>
public record EmbeddingResponse
{
    public IReadOnlyList<EmbeddingData> Embeddings { get; init; } = Array.Empty<EmbeddingData>();
    public int TotalTokens { get; init; }
}

/// <summary>
/// Single embedding data.
/// </summary>
public record EmbeddingData
{
    public int Index { get; init; }
    public float[] Embedding { get; init; } = Array.Empty<float>();
}

/// <summary>
/// RAG request.
/// </summary>
public record RAGRequest
{
    public string Query { get; init; } = string.Empty;
    public Guid? ConversationId { get; init; }
    public IReadOnlyList<ChatMessage>? PreviousMessages { get; init; }
    public bool Stream { get; init; } = false;
    public int MaxSources { get; init; } = 5;
    public float MinRelevanceScore { get; init; } = 0.7f;
    public IReadOnlyList<Guid>? RestrictToDocuments { get; init; }
    public IReadOnlyList<Guid>? RestrictToLibraries { get; init; }
}

/// <summary>
/// RAG response.
/// </summary>
public record RAGResponse
{
    public string Answer { get; init; } = string.Empty;
    public IReadOnlyList<Citation> Citations { get; init; } = Array.Empty<Citation>();
    public IReadOnlyList<DocumentChunk> Sources { get; init; } = Array.Empty<DocumentChunk>();
    public string? SuggestedFollowUp { get; init; }
    public int TokensUsed { get; init; }
}

/// <summary>
/// RAG streaming chunk.
/// </summary>
public record RAGStreamChunk
{
    public string Delta { get; init; } = string.Empty;
    public bool IsComplete { get; init; }
    public IReadOnlyList<Citation>? Citations { get; init; }
    public IReadOnlyList<DocumentChunk>? Sources { get; init; }
}

/// <summary>
/// Citation reference.
/// </summary>
public record Citation
{
    public int Index { get; init; }
    public Guid DocumentId { get; init; }
    public string DocumentName { get; init; } = string.Empty;
    public string ChunkId { get; init; } = string.Empty;
    public string Quote { get; init; } = string.Empty;
    public int? PageNumber { get; init; }
}

/// <summary>
/// Document chunk from semantic search.
/// </summary>
public record DocumentChunk
{
    public Guid DocumentId { get; init; }
    public string DocumentName { get; init; } = string.Empty;
    public string ChunkId { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public float RelevanceScore { get; init; }
    public int? PageNumber { get; init; }
    public string? Section { get; init; }
    public Dictionary<string, string> Metadata { get; init; } = new();
}

/// <summary>
/// Conversation DTO.
/// </summary>
public record ConversationDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime LastMessageAt { get; init; }
    public int MessageCount { get; init; }
}

/// <summary>
/// Message DTO.
/// </summary>
public record MessageDto
{
    public Guid Id { get; init; }
    public Guid ConversationId { get; init; }
    public string Role { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public IReadOnlyList<Citation>? Citations { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Chat request from frontend.
/// </summary>
public record ChatRequest
{
    public string Message { get; init; } = string.Empty;
    public Guid? ConversationId { get; init; }
    public bool UseRAG { get; init; } = true;
    public bool Stream { get; init; } = true;
    public IReadOnlyList<Guid>? RestrictToDocuments { get; init; }
}

/// <summary>
/// AI status response.
/// </summary>
public record AIStatusResponse
{
    public bool IsAvailable { get; init; }
    public string Provider { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public bool IsMock { get; init; }
    public int IndexedDocuments { get; init; }
    public DateTime? LastIndexUpdate { get; init; }
}
