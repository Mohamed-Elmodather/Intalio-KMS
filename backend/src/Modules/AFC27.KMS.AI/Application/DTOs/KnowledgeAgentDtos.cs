namespace AFC27.KMS.AI.Application.DTOs;

/// <summary>
/// Knowledge agent response DTO.
/// </summary>
public record KnowledgeAgentDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string SystemPrompt { get; init; } = string.Empty;
    public Guid? SpaceId { get; init; }
    public IReadOnlyList<string> AllowedSources { get; init; } = Array.Empty<string>();
    public float Temperature { get; init; }
    public int MaxTokens { get; init; }
    public bool IsActive { get; init; }
    public Guid CreatedById { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// Request to create a knowledge agent.
/// </summary>
public record CreateKnowledgeAgentRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string SystemPrompt { get; init; } = string.Empty;
    public Guid? SpaceId { get; init; }
    public IReadOnlyList<string>? AllowedSources { get; init; }
    public float Temperature { get; init; } = 0.7f;
    public int MaxTokens { get; init; } = 2048;
}

/// <summary>
/// Request to update a knowledge agent.
/// </summary>
public record UpdateKnowledgeAgentRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string SystemPrompt { get; init; } = string.Empty;
    public Guid? SpaceId { get; init; }
    public IReadOnlyList<string>? AllowedSources { get; init; }
    public float Temperature { get; init; } = 0.7f;
    public int MaxTokens { get; init; } = 2048;
}

/// <summary>
/// Request to query a specific knowledge agent.
/// </summary>
public record AgentQueryRequest
{
    public string Message { get; init; } = string.Empty;
    public Guid? ConversationId { get; init; }
    public bool Stream { get; init; } = false;
}

/// <summary>
/// Response from a knowledge agent query.
/// </summary>
public record AgentQueryResponse
{
    public Guid AgentId { get; init; }
    public string AgentName { get; init; } = string.Empty;
    public string Answer { get; init; } = string.Empty;
    public IReadOnlyList<Citation> Citations { get; init; } = Array.Empty<Citation>();
    public IReadOnlyList<DocumentChunk> Sources { get; init; } = Array.Empty<DocumentChunk>();
    public Guid? ConversationId { get; init; }
    public int TokensUsed { get; init; }
}
