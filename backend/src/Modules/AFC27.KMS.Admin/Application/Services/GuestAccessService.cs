using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Application.Interfaces;
using AFC27.KMS.Admin.Domain.Entities;

namespace AFC27.KMS.Admin.Application.Services;

/// <summary>
/// Service for managing guest/external access tokens.
/// Enables sharing content with external users via time-limited access tokens.
/// </summary>
public class GuestAccessService : IGuestAccessService
{
    private readonly DbContext _dbContext;
    private readonly ILogger<GuestAccessService> _logger;

    public GuestAccessService(DbContext dbContext, ILogger<GuestAccessService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<GuestAccessDto> CreateAsync(
        CreateGuestAccessRequest request,
        Guid grantedByUserId,
        string grantedByName,
        CancellationToken ct = default)
    {
        if (!Enum.TryParse<GuestEntityType>(request.EntityType, true, out var entityType))
            throw new ArgumentException($"Invalid entity type: {request.EntityType}");

        if (!Enum.TryParse<GuestAccessLevel>(request.AccessLevel, true, out var accessLevel))
            throw new ArgumentException($"Invalid access level: {request.AccessLevel}");

        var expiresAt = DateTime.UtcNow.AddDays(request.ExpirationDays);

        var guestAccess = GuestAccess.Create(
            entityType,
            request.EntityId,
            grantedByUserId,
            grantedByName,
            request.GuestEmail,
            accessLevel,
            expiresAt,
            request.GuestName,
            request.Message);

        _dbContext.Set<GuestAccess>().Add(guestAccess);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Guest access created: {GuestEmail} to {EntityType}/{EntityId} with {AccessLevel} until {ExpiresAt} by {GrantedBy}",
            request.GuestEmail, entityType, request.EntityId, accessLevel, expiresAt, grantedByUserId);

        return MapToDto(guestAccess);
    }

    public async Task<GuestAccessDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var guestAccess = await _dbContext.Set<GuestAccess>()
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id, ct);

        return guestAccess != null ? MapToDto(guestAccess) : null;
    }

    public async Task<GuestAccessDto?> GetByTokenAsync(string token, CancellationToken ct = default)
    {
        var guestAccess = await _dbContext.Set<GuestAccess>()
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Token == token, ct);

        return guestAccess != null ? MapToDto(guestAccess) : null;
    }

    public async Task<PagedResult<GuestAccessDto>> ListAsync(
        GuestAccessFilterRequest filter, CancellationToken ct = default)
    {
        var query = _dbContext.Set<GuestAccess>()
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.EntityType) &&
            Enum.TryParse<GuestEntityType>(filter.EntityType, true, out var entityType))
        {
            query = query.Where(g => g.EntityType == entityType);
        }

        if (filter.EntityId.HasValue)
        {
            query = query.Where(g => g.EntityId == filter.EntityId.Value);
        }

        if (filter.GrantedById.HasValue)
        {
            query = query.Where(g => g.GrantedById == filter.GrantedById.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.GuestEmail))
        {
            var emailLower = filter.GuestEmail.ToLower();
            query = query.Where(g => g.GuestEmail.ToLower().Contains(emailLower));
        }

        if (filter.IsActive.HasValue)
        {
            query = query.Where(g => g.IsActive == filter.IsActive.Value);
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(g => g.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);

        return new PagedResult<GuestAccessDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<bool> RevokeAsync(Guid id, Guid revokedByUserId, CancellationToken ct = default)
    {
        var guestAccess = await _dbContext.Set<GuestAccess>()
            .FirstOrDefaultAsync(g => g.Id == id && g.IsActive, ct);

        if (guestAccess == null)
            return false;

        guestAccess.Revoke(revokedByUserId);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Guest access {Id} for {GuestEmail} revoked by {RevokedBy}",
            id, guestAccess.GuestEmail, revokedByUserId);

        return true;
    }

    public async Task<bool> ExtendAsync(
        Guid id, ExtendGuestAccessRequest request, CancellationToken ct = default)
    {
        var guestAccess = await _dbContext.Set<GuestAccess>()
            .FirstOrDefaultAsync(g => g.Id == id && g.IsActive, ct);

        if (guestAccess == null)
            return false;

        var newExpiry = guestAccess.ExpiresAt.AddDays(request.AdditionalDays);
        guestAccess.ExtendExpiration(newExpiry);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Guest access {Id} extended to {NewExpiry}", id, newExpiry);

        return true;
    }

    public async Task<bool> RecordAccessAsync(string token, CancellationToken ct = default)
    {
        var guestAccess = await _dbContext.Set<GuestAccess>()
            .FirstOrDefaultAsync(g => g.Token == token && g.IsActive, ct);

        if (guestAccess == null || !guestAccess.IsValid)
            return false;

        guestAccess.RecordAccess();
        await _dbContext.SaveChangesAsync(ct);

        return true;
    }

    public async Task<GuestAccessDto?> ValidateTokenAsync(string token, CancellationToken ct = default)
    {
        var guestAccess = await _dbContext.Set<GuestAccess>()
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Token == token, ct);

        if (guestAccess == null || !guestAccess.IsValid)
            return null;

        return MapToDto(guestAccess);
    }

    public async Task<int> DeactivateExpiredAsync(CancellationToken ct = default)
    {
        var expired = await _dbContext.Set<GuestAccess>()
            .Where(g => g.IsActive && g.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync(ct);

        foreach (var g in expired)
        {
            g.Deactivate();
        }

        if (expired.Count > 0)
        {
            await _dbContext.SaveChangesAsync(ct);
            _logger.LogInformation("Deactivated {Count} expired guest access entries", expired.Count);
        }

        return expired.Count;
    }

    // ========================================
    // Private Helpers
    // ========================================

    private static GuestAccessDto MapToDto(GuestAccess g)
    {
        return new GuestAccessDto
        {
            Id = g.Id,
            Token = g.Token,
            EntityType = g.EntityType.ToString(),
            EntityId = g.EntityId,
            GrantedById = g.GrantedById,
            GrantedByName = g.GrantedByName,
            GuestEmail = g.GuestEmail,
            GuestName = g.GuestName,
            ExpiresAt = g.ExpiresAt,
            AccessLevel = g.AccessLevel.ToString(),
            IsActive = g.IsActive,
            IsValid = g.IsValid,
            Message = g.Message,
            AccessCount = g.AccessCount,
            LastAccessedAt = g.LastAccessedAt,
            CreatedAt = g.CreatedAt,
            RevokedAt = g.RevokedAt
        };
    }
}
