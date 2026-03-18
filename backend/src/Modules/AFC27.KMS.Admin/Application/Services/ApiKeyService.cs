using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Domain.Entities;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.Admin.Application.Services;

/// <summary>
/// Service interface for API key management.
/// </summary>
public interface IApiKeyService
{
    Task<ApiKeyCreatedDto> CreateAsync(CreateApiKeyRequest request, CancellationToken cancellationToken = default);
    Task<ApiKeyDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ApiKeyDto>> ListAsync(bool includeRevoked = false, CancellationToken cancellationToken = default);
    Task<ApiKeyDto?> UpdateAsync(Guid id, UpdateApiKeyRequest request, CancellationToken cancellationToken = default);
    Task<bool> RevokeAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiKeyDto?> ValidateKeyAsync(string plainTextKey, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service for managing API keys.
/// </summary>
public class ApiKeyService : IApiKeyService
{
    private readonly DbContext _dbContext;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<ApiKeyService> _logger;

    public ApiKeyService(
        DbContext dbContext,
        ICurrentUser currentUser,
        ILogger<ApiKeyService> logger)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ApiKeyCreatedDto> CreateAsync(
        CreateApiKeyRequest request,
        CancellationToken cancellationToken = default)
    {
        var (entity, plainTextKey) = ApiKey.Create(
            request.Name,
            request.Scopes?.ToList() ?? new List<string>(),
            _currentUser.UserId ?? Guid.Empty,
            _currentUser.DisplayName ?? "System",
            request.ExpiresAt,
            request.Description);

        _dbContext.Set<ApiKey>().Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Created API key {ApiKeyId} '{Name}' with prefix {Prefix}",
            entity.Id, entity.Name, entity.KeyPrefix);

        return new ApiKeyCreatedDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Key = plainTextKey,
            KeyPrefix = entity.KeyPrefix,
            Scopes = entity.Scopes,
            ExpiresAt = entity.ExpiresAt,
            CreatedAt = entity.CreatedAt
        };
    }

    public async Task<ApiKeyDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var key = await _dbContext.Set<ApiKey>()
            .AsNoTracking()
            .FirstOrDefaultAsync(k => k.Id == id, cancellationToken);

        return key == null ? null : MapToDto(key);
    }

    public async Task<IReadOnlyList<ApiKeyDto>> ListAsync(
        bool includeRevoked = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<ApiKey>().AsNoTracking();

        if (!includeRevoked)
            query = query.Where(k => k.IsActive);

        var keys = await query
            .OrderBy(k => k.Name)
            .ToListAsync(cancellationToken);

        return keys.Select(MapToDto).ToList();
    }

    public async Task<ApiKeyDto?> UpdateAsync(
        Guid id,
        UpdateApiKeyRequest request,
        CancellationToken cancellationToken = default)
    {
        var key = await _dbContext.Set<ApiKey>()
            .FirstOrDefaultAsync(k => k.Id == id, cancellationToken);

        if (key == null)
            return null;

        key.Update(
            request.Name,
            request.Scopes?.ToList(),
            request.ExpiresAt,
            request.Description);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated API key {ApiKeyId}", id);

        return MapToDto(key);
    }

    public async Task<bool> RevokeAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var key = await _dbContext.Set<ApiKey>()
            .FirstOrDefaultAsync(k => k.Id == id, cancellationToken);

        if (key == null) return false;

        key.Revoke();
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Revoked API key {ApiKeyId}", id);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var key = await _dbContext.Set<ApiKey>()
            .FirstOrDefaultAsync(k => k.Id == id, cancellationToken);

        if (key == null) return false;

        _dbContext.Set<ApiKey>().Remove(key);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted API key {ApiKeyId}", id);

        return true;
    }

    public async Task<ApiKeyDto?> ValidateKeyAsync(
        string plainTextKey,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(plainTextKey))
            return null;

        var prefix = plainTextKey.Substring(0, Math.Min(8, plainTextKey.Length));

        var candidates = await _dbContext.Set<ApiKey>()
            .Where(k => k.KeyPrefix == prefix && k.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var candidate in candidates)
        {
            if (ApiKey.VerifyKey(plainTextKey, candidate.KeyHash))
            {
                if (candidate.IsExpired)
                    return null;

                candidate.RecordUsage();
                await _dbContext.SaveChangesAsync(cancellationToken);

                return MapToDto(candidate);
            }
        }

        return null;
    }

    private static ApiKeyDto MapToDto(ApiKey key)
    {
        return new ApiKeyDto
        {
            Id = key.Id,
            Name = key.Name,
            KeyPrefix = key.KeyPrefix,
            Scopes = key.Scopes,
            ExpiresAt = key.ExpiresAt,
            IsActive = key.IsActive,
            LastUsedAt = key.LastUsedAt,
            TotalUsageCount = key.TotalUsageCount,
            CreatedByUserId = key.CreatedByUserId,
            CreatedByName = key.CreatedByName,
            Description = key.Description,
            CreatedAt = key.CreatedAt,
            IsExpired = key.IsExpired
        };
    }
}
