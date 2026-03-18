namespace AFC27.KMS.AI.Application.DTOs;

/// <summary>
/// Request for multi-step AI research.
/// </summary>
public record ResearchRequest
{
    /// <summary>
    /// The research question to investigate.
    /// </summary>
    public string Question { get; init; } = string.Empty;

    /// <summary>
    /// Research depth: "quick" (1 pass), "standard" (2 passes), "deep" (3+ passes with synthesis).
    /// </summary>
    public string Depth { get; init; } = "standard";

    /// <summary>
    /// Optional list of source IDs (document or library GUIDs) to restrict the search scope.
    /// </summary>
    public IReadOnlyList<Guid>? Sources { get; init; }

    /// <summary>
    /// Maximum number of sources to include in the report.
    /// </summary>
    public int MaxSources { get; init; } = 10;

    /// <summary>
    /// Optional language for the output report (en, ar).
    /// </summary>
    public string Language { get; init; } = "en";
}

/// <summary>
/// Response from the AI research pipeline.
/// </summary>
public record ResearchResponse
{
    public Guid ResearchId { get; init; }
    public string Question { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public IReadOnlyList<ResearchFinding> Findings { get; init; } = Array.Empty<ResearchFinding>();
    public IReadOnlyList<Citation> Citations { get; init; } = Array.Empty<Citation>();
    public string Conclusion { get; init; } = string.Empty;
    public IReadOnlyList<string> SuggestedFollowUps { get; init; } = Array.Empty<string>();
    public ResearchMetadata Metadata { get; init; } = new();
}

/// <summary>
/// An individual finding from the research process.
/// </summary>
public record ResearchFinding
{
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public float Relevance { get; init; }
    public IReadOnlyList<Citation> SupportingCitations { get; init; } = Array.Empty<Citation>();
}

/// <summary>
/// Metadata about the research process.
/// </summary>
public record ResearchMetadata
{
    public string Depth { get; init; } = string.Empty;
    public int SourcesSearched { get; init; }
    public int SourcesUsed { get; init; }
    public int TotalTokensUsed { get; init; }
    public int SearchPasses { get; init; }
    public int DurationMs { get; init; }
    public DateTime CompletedAt { get; init; }
}
