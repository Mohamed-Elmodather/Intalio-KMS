using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Search.Domain.Entities;

/// <summary>
/// Represents a saved search query for a user
/// </summary>
public class SavedSearch : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Query { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }

    // Search parameters (stored as JSON)
    public string? FiltersJson { get; private set; }
    public List<SearchableContentType> ContentTypes { get; private set; } = new();
    public string? SortBy { get; private set; }
    public SortDirection SortDirection { get; private set; } = SortDirection.Descending;

    // Date range filters
    public DateTime? DateFrom { get; private set; }
    public DateTime? DateTo { get; private set; }

    // Notification settings
    public bool NotifyOnNewResults { get; private set; }
    public NotificationFrequency NotificationFrequency { get; private set; } = NotificationFrequency.Daily;
    public DateTime? LastNotifiedAt { get; private set; }
    public int LastResultCount { get; private set; }

    // Usage tracking
    public int ExecutionCount { get; private set; }
    public DateTime? LastExecutedAt { get; private set; }

    private SavedSearch() { }

    public static SavedSearch Create(
        string name,
        string query,
        Guid userId,
        List<SearchableContentType> contentTypes)
    {
        return new SavedSearch
        {
            Id = Guid.NewGuid(),
            Name = name,
            Query = query,
            UserId = userId,
            ContentTypes = contentTypes
        };
    }

    public void UpdateSearch(string name, string query, string? filtersJson)
    {
        Name = name;
        Query = query;
        FiltersJson = filtersJson;
    }

    public void SetContentTypes(List<SearchableContentType> contentTypes)
    {
        ContentTypes = contentTypes;
    }

    public void SetSorting(string sortBy, SortDirection direction)
    {
        SortBy = sortBy;
        SortDirection = direction;
    }

    public void SetDateRange(DateTime? from, DateTime? to)
    {
        DateFrom = from;
        DateTo = to;
    }

    public void EnableNotifications(NotificationFrequency frequency)
    {
        NotifyOnNewResults = true;
        NotificationFrequency = frequency;
    }

    public void DisableNotifications()
    {
        NotifyOnNewResults = false;
    }

    public void RecordNotification(int resultCount)
    {
        LastNotifiedAt = DateTime.UtcNow;
        LastResultCount = resultCount;
    }

    public void RecordExecution()
    {
        ExecutionCount++;
        LastExecutedAt = DateTime.UtcNow;
    }
}

public enum SortDirection
{
    Ascending,
    Descending
}

public enum NotificationFrequency
{
    Immediately,
    Daily,
    Weekly
}
