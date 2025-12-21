using AFC27.KMS.AI.Application.DTOs;

namespace AFC27.KMS.AI.Application.Interfaces;

/// <summary>
/// Interface for the AI client (Intalio or mock).
/// </summary>
public interface IIntalioAIClient
{
    /// <summary>
    /// Send a chat completion request.
    /// </summary>
    Task<ChatCompletionResponse> ChatAsync(
        ChatCompletionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Send a streaming chat completion request.
    /// </summary>
    IAsyncEnumerable<ChatStreamChunk> ChatStreamAsync(
        ChatCompletionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate embeddings for text.
    /// </summary>
    Task<EmbeddingResponse> GetEmbeddingsAsync(
        EmbeddingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if the AI service is available.
    /// </summary>
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for RAG (Retrieval Augmented Generation).
/// </summary>
public interface IRAGService
{
    /// <summary>
    /// Query documents with RAG.
    /// </summary>
    Task<RAGResponse> QueryAsync(
        RAGRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream RAG response.
    /// </summary>
    IAsyncEnumerable<RAGStreamChunk> QueryStreamAsync(
        RAGRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Search for relevant documents.
    /// </summary>
    Task<IReadOnlyList<DocumentChunk>> SearchAsync(
        string query,
        Guid userId,
        int maxResults = 10,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for document embedding and indexing.
/// </summary>
public interface IEmbeddingService
{
    /// <summary>
    /// Index a document for semantic search.
    /// </summary>
    Task IndexDocumentAsync(
        Guid documentId,
        string content,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove document from index.
    /// </summary>
    Task RemoveDocumentAsync(
        Guid documentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Search for similar documents.
    /// </summary>
    Task<IReadOnlyList<SimilarDocument>> SearchSimilarAsync(
        string query,
        int maxResults = 10,
        Dictionary<string, string>? filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get embedding for text.
    /// </summary>
    Task<float[]> GetEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for chat history management.
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Create a new conversation.
    /// </summary>
    Task<ConversationDto> CreateConversationAsync(
        string? title = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get conversation by ID.
    /// </summary>
    Task<ConversationDto?> GetConversationAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List user's conversations.
    /// </summary>
    Task<IReadOnlyList<ConversationDto>> ListConversationsAsync(
        int limit = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a message to conversation.
    /// </summary>
    Task<MessageDto> AddMessageAsync(
        Guid conversationId,
        string role,
        string content,
        IReadOnlyList<Citation>? citations = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get messages for a conversation.
    /// </summary>
    Task<IReadOnlyList<MessageDto>> GetMessagesAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a conversation.
    /// </summary>
    Task<bool> DeleteConversationAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update conversation title.
    /// </summary>
    Task<bool> UpdateTitleAsync(
        Guid conversationId,
        string title,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get conversation messages as ChatMessage list for AI context.
    /// </summary>
    Task<IReadOnlyList<ChatMessage>> GetChatHistoryAsync(
        Guid conversationId,
        int maxMessages = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Auto-generate title from first message content.
    /// </summary>
    Task<string> GenerateTitleAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Similar document result.
/// </summary>
public record SimilarDocument
{
    public Guid DocumentId { get; init; }
    public string ChunkId { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public float Score { get; init; }
    public Dictionary<string, string> Metadata { get; init; } = new();
}
