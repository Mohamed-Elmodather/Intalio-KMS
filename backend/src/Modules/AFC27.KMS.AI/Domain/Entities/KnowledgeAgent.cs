namespace AFC27.KMS.AI.Domain.Entities;

/// <summary>
/// Configurable AI agent for a specific knowledge domain.
/// Agents can be scoped to a Space and restricted to particular data sources.
/// </summary>
public class KnowledgeAgent
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string SystemPrompt { get; private set; } = string.Empty;
    public Guid? SpaceId { get; private set; }
    public List<string> AllowedSources { get; private set; } = new();
    public float Temperature { get; private set; } = 0.7f;
    public int MaxTokens { get; private set; } = 2048;
    public bool IsActive { get; private set; } = true;
    public Guid CreatedById { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private KnowledgeAgent() { }

    public static KnowledgeAgent Create(
        string name,
        string description,
        string systemPrompt,
        Guid createdById,
        Guid? spaceId = null,
        List<string>? allowedSources = null,
        float temperature = 0.7f,
        int maxTokens = 2048)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(systemPrompt))
            throw new ArgumentException("System prompt cannot be empty", nameof(systemPrompt));
        if (temperature < 0f || temperature > 2f)
            throw new ArgumentOutOfRangeException(nameof(temperature), "Temperature must be between 0 and 2");
        if (maxTokens < 1)
            throw new ArgumentOutOfRangeException(nameof(maxTokens), "MaxTokens must be positive");

        return new KnowledgeAgent
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            SystemPrompt = systemPrompt,
            SpaceId = spaceId,
            AllowedSources = allowedSources ?? new List<string>(),
            Temperature = temperature,
            MaxTokens = maxTokens,
            IsActive = true,
            CreatedById = createdById,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string name,
        string description,
        string systemPrompt,
        Guid? spaceId,
        List<string>? allowedSources,
        float temperature,
        int maxTokens)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(systemPrompt))
            throw new ArgumentException("System prompt cannot be empty", nameof(systemPrompt));

        Name = name;
        Description = description;
        SystemPrompt = systemPrompt;
        SpaceId = spaceId;
        AllowedSources = allowedSources ?? new List<string>();
        Temperature = Math.Clamp(temperature, 0f, 2f);
        MaxTokens = Math.Max(1, maxTokens);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
