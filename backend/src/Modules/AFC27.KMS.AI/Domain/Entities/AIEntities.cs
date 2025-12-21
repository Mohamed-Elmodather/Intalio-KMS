namespace AFC27.KMS.AI.Domain.Entities;

/// <summary>
/// Represents a chat conversation with the AI assistant.
/// </summary>
public class Conversation
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime LastMessageAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    // Navigation
    public virtual ICollection<Message> Messages { get; private set; } = new List<Message>();

    private Conversation() { }

    public static Conversation Create(Guid userId, string title)
    {
        return new Conversation
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            CreatedAt = DateTime.UtcNow,
            LastMessageAt = DateTime.UtcNow,
            IsDeleted = false
        };
    }

    public Message AddMessage(string role, string content, string? citationsJson = null)
    {
        var message = Message.Create(Id, role, content, citationsJson);
        Messages.Add(message);
        LastMessageAt = DateTime.UtcNow;
        return message;
    }

    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        Title = title;
    }

    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Represents a message in a conversation.
/// </summary>
public class Message
{
    public Guid Id { get; private set; }
    public Guid ConversationId { get; private set; }
    public string Role { get; private set; } = string.Empty; // "user", "assistant", "system"
    public string Content { get; private set; } = string.Empty;
    public string? CitationsJson { get; private set; } // JSON array of Citation objects
    public DateTime CreatedAt { get; private set; }

    // Navigation
    public virtual Conversation Conversation { get; private set; } = null!;

    private Message() { }

    public static Message Create(Guid conversationId, string role, string content, string? citationsJson = null)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role cannot be empty", nameof(role));

        return new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            Role = role,
            Content = content ?? string.Empty,
            CitationsJson = citationsJson,
            CreatedAt = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Represents a document index entry for AI search.
/// </summary>
public class DocumentIndex
{
    public Guid Id { get; private set; }
    public Guid DocumentId { get; private set; }
    public string ChunkId { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public byte[] Embedding { get; private set; } = Array.Empty<byte>(); // Serialized float[]
    public string MetadataJson { get; private set; } = "{}";
    public DateTime IndexedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private DocumentIndex() { }

    public static DocumentIndex Create(
        Guid documentId,
        string chunkId,
        string content,
        float[] embedding,
        Dictionary<string, string>? metadata = null)
    {
        return new DocumentIndex
        {
            Id = Guid.NewGuid(),
            DocumentId = documentId,
            ChunkId = chunkId,
            Content = content,
            Embedding = SerializeEmbedding(embedding),
            MetadataJson = metadata != null
                ? System.Text.Json.JsonSerializer.Serialize(metadata)
                : "{}",
            IndexedAt = DateTime.UtcNow
        };
    }

    public void Update(string content, float[] embedding, Dictionary<string, string>? metadata = null)
    {
        Content = content;
        Embedding = SerializeEmbedding(embedding);
        if (metadata != null)
            MetadataJson = System.Text.Json.JsonSerializer.Serialize(metadata);
        UpdatedAt = DateTime.UtcNow;
    }

    public float[] GetEmbedding()
    {
        return DeserializeEmbedding(Embedding);
    }

    public Dictionary<string, string> GetMetadata()
    {
        return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(MetadataJson)
            ?? new Dictionary<string, string>();
    }

    private static byte[] SerializeEmbedding(float[] embedding)
    {
        var bytes = new byte[embedding.Length * sizeof(float)];
        Buffer.BlockCopy(embedding, 0, bytes, 0, bytes.Length);
        return bytes;
    }

    private static float[] DeserializeEmbedding(byte[] bytes)
    {
        var embedding = new float[bytes.Length / sizeof(float)];
        Buffer.BlockCopy(bytes, 0, embedding, 0, bytes.Length);
        return embedding;
    }
}

/// <summary>
/// AI usage analytics for tracking and billing.
/// </summary>
public class AIUsageLog
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? ConversationId { get; private set; }
    public string Operation { get; private set; } = string.Empty; // "chat", "rag", "embedding"
    public int PromptTokens { get; private set; }
    public int CompletionTokens { get; private set; }
    public int TotalTokens { get; private set; }
    public string Model { get; private set; } = string.Empty;
    public int ResponseTimeMs { get; private set; }
    public bool Success { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private AIUsageLog() { }

    public static AIUsageLog Create(
        Guid userId,
        string operation,
        int promptTokens,
        int completionTokens,
        string model,
        int responseTimeMs,
        bool success,
        Guid? conversationId = null,
        string? errorMessage = null)
    {
        return new AIUsageLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ConversationId = conversationId,
            Operation = operation,
            PromptTokens = promptTokens,
            CompletionTokens = completionTokens,
            TotalTokens = promptTokens + completionTokens,
            Model = model,
            ResponseTimeMs = responseTimeMs,
            Success = success,
            ErrorMessage = errorMessage,
            CreatedAt = DateTime.UtcNow
        };
    }
}
