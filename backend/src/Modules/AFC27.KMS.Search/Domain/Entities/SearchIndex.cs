using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Search.Domain.Entities;

/// <summary>
/// Represents an Elasticsearch index configuration
/// </summary>
public class SearchIndex : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string DisplayNameEn { get; private set; } = string.Empty;
    public string DisplayNameAr { get; private set; } = string.Empty;
    public string? DescriptionEn { get; private set; }
    public string? DescriptionAr { get; private set; }

    public SearchableContentType ContentType { get; private set; }
    public IndexStatus Status { get; private set; } = IndexStatus.Active;

    // Index settings
    public int NumberOfShards { get; private set; } = 1;
    public int NumberOfReplicas { get; private set; } = 1;
    public string? Analyzer { get; private set; } = "standard";
    public string? ArabicAnalyzer { get; private set; } = "arabic";

    // Index statistics
    public long DocumentCount { get; private set; }
    public long SizeInBytes { get; private set; }
    public DateTime? LastIndexedAt { get; private set; }
    public DateTime? LastReindexedAt { get; private set; }

    // Reindexing
    public bool IsReindexing { get; private set; }
    public int ReindexProgress { get; private set; }
    public DateTime? ReindexStartedAt { get; private set; }

    // Mapping configuration (JSON)
    public string? MappingJson { get; private set; }

    private SearchIndex() { }

    public static SearchIndex Create(
        string name,
        SearchableContentType contentType,
        string displayNameEn,
        string displayNameAr)
    {
        return new SearchIndex
        {
            Id = Guid.NewGuid(),
            Name = name.ToLowerInvariant().Replace(" ", "_"),
            ContentType = contentType,
            DisplayNameEn = displayNameEn,
            DisplayNameAr = displayNameAr,
            Status = IndexStatus.Active
        };
    }

    public void UpdateSettings(int shards, int replicas, string? analyzer, string? arabicAnalyzer)
    {
        NumberOfShards = Math.Max(1, shards);
        NumberOfReplicas = Math.Max(0, replicas);
        Analyzer = analyzer ?? "standard";
        ArabicAnalyzer = arabicAnalyzer ?? "arabic";
    }

    public void UpdateStatistics(long documentCount, long sizeInBytes)
    {
        DocumentCount = documentCount;
        SizeInBytes = sizeInBytes;
        LastIndexedAt = DateTime.UtcNow;
    }

    public void SetMapping(string mappingJson)
    {
        MappingJson = mappingJson;
    }

    public void StartReindexing()
    {
        IsReindexing = true;
        ReindexProgress = 0;
        ReindexStartedAt = DateTime.UtcNow;
    }

    public void UpdateReindexProgress(int progress)
    {
        ReindexProgress = Math.Min(100, Math.Max(0, progress));
    }

    public void CompleteReindexing()
    {
        IsReindexing = false;
        ReindexProgress = 100;
        LastReindexedAt = DateTime.UtcNow;
    }

    public void Activate() => Status = IndexStatus.Active;
    public void Deactivate() => Status = IndexStatus.Inactive;
    public void MarkForRebuild() => Status = IndexStatus.Rebuilding;
}

public enum IndexStatus
{
    Active,
    Inactive,
    Rebuilding,
    Error
}
