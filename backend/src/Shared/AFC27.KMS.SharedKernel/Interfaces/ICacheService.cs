namespace AFC27.KMS.SharedKernel.Interfaces;

/// <summary>
/// Interface for distributed caching operations.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a cached value by key.
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a value in cache with optional expiration.
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a value from cache.
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a value from cache, or creates it using the factory if not found.
    /// </summary>
    Task<T> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all values matching a pattern.
    /// </summary>
    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a key exists in cache.
    /// </summary>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
}

/// <summary>
/// Cache key builder helper.
/// </summary>
public static class CacheKeys
{
    public static string User(Guid userId) => $"user:{userId}";
    public static string UserProfile(Guid userId) => $"user-profile:{userId}";
    public static string UserPermissions(Guid userId) => $"user-permissions:{userId}";
    public static string Article(Guid articleId) => $"article:{articleId}";
    public static string Document(Guid documentId) => $"document:{documentId}";
    public static string Search(string query, int page) => $"search:{query}:page:{page}";
}
