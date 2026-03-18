using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Application.Interfaces;
using AFC27.KMS.Admin.Domain.Entities;

namespace AFC27.KMS.Admin.Application.Services;

/// <summary>
/// Service for managing delegated space administration.
/// Space owners can delegate specific admin capabilities to users,
/// restricted to the space scope only.
/// </summary>
public class DelegatedAdminService : IDelegatedAdminService
{
    private readonly DbContext _dbContext;
    private readonly ILogger<DelegatedAdminService> _logger;

    public DelegatedAdminService(DbContext dbContext, ILogger<DelegatedAdminService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<DelegatedAdminDto?> CreateAsync(
        CreateDelegatedAdminRequest request,
        Guid grantedByUserId,
        string grantedByUserName,
        CancellationToken ct = default)
    {
        // Check for existing active delegation for this user+space
        var existing = await _dbContext.Set<DelegatedAdmin>()
            .FirstOrDefaultAsync(d =>
                d.SpaceId == request.SpaceId
                && d.DelegateUserId == request.DelegateUserId
                && d.Status == DelegatedAdminStatus.Active,
                ct);

        if (existing != null)
        {
            _logger.LogWarning(
                "Active delegation already exists for User {UserId} on Space {SpaceId}",
                request.DelegateUserId, request.SpaceId);
            return null;
        }

        var scopes = ParseScopes(request.Scopes);

        var delegation = DelegatedAdmin.Create(
            request.SpaceId,
            request.DelegateUserId,
            request.DelegateUserName,
            grantedByUserId,
            grantedByUserName,
            scopes,
            request.Reason,
            request.ExpiresAt);

        _dbContext.Set<DelegatedAdmin>().Add(delegation);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Delegated admin created: User {DelegateUserId} on Space {SpaceId} with scopes {Scopes} by {GrantedBy}",
            request.DelegateUserId, request.SpaceId, scopes, grantedByUserId);

        return MapToDto(delegation);
    }

    public async Task<DelegatedAdminDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var delegation = await _dbContext.Set<DelegatedAdmin>()
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id, ct);

        return delegation != null ? MapToDto(delegation) : null;
    }

    public async Task<IReadOnlyList<DelegatedAdminDto>> GetBySpaceAsync(
        Guid spaceId, bool includeRevoked = false, CancellationToken ct = default)
    {
        var query = _dbContext.Set<DelegatedAdmin>()
            .AsNoTracking()
            .Where(d => d.SpaceId == spaceId);

        if (!includeRevoked)
        {
            query = query.Where(d => d.Status == DelegatedAdminStatus.Active);
        }

        var delegations = await query
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(ct);

        return delegations.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<DelegatedAdminDto>> GetByUserAsync(
        Guid userId, CancellationToken ct = default)
    {
        var delegations = await _dbContext.Set<DelegatedAdmin>()
            .AsNoTracking()
            .Where(d => d.DelegateUserId == userId && d.Status == DelegatedAdminStatus.Active)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(ct);

        return delegations.Select(MapToDto).ToList();
    }

    public async Task<bool> UpdateScopesAsync(
        Guid id, UpdateDelegatedAdminScopesRequest request, Guid updatedByUserId, CancellationToken ct = default)
    {
        var delegation = await _dbContext.Set<DelegatedAdmin>()
            .FirstOrDefaultAsync(d => d.Id == id && d.Status == DelegatedAdminStatus.Active, ct);

        if (delegation == null)
            return false;

        var newScopes = ParseScopes(request.Scopes);
        delegation.UpdateScopes(newScopes);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Delegated admin {Id} scopes updated to {Scopes} by {UpdatedBy}",
            id, newScopes, updatedByUserId);

        return true;
    }

    public async Task<bool> RevokeAsync(Guid id, Guid revokedByUserId, CancellationToken ct = default)
    {
        var delegation = await _dbContext.Set<DelegatedAdmin>()
            .FirstOrDefaultAsync(d => d.Id == id && d.Status == DelegatedAdminStatus.Active, ct);

        if (delegation == null)
            return false;

        delegation.Revoke(revokedByUserId);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Delegated admin {Id} revoked by {RevokedBy}", id, revokedByUserId);

        return true;
    }

    public async Task<bool> HasDelegatedScopeAsync(
        Guid spaceId, Guid userId, DelegatedAdminScope scope, CancellationToken ct = default)
    {
        var delegation = await _dbContext.Set<DelegatedAdmin>()
            .AsNoTracking()
            .FirstOrDefaultAsync(d =>
                d.SpaceId == spaceId
                && d.DelegateUserId == userId
                && d.Status == DelegatedAdminStatus.Active,
                ct);

        return delegation?.HasScope(scope) ?? false;
    }

    // ========================================
    // Private Helpers
    // ========================================

    private static DelegatedAdminDto MapToDto(DelegatedAdmin d)
    {
        return new DelegatedAdminDto
        {
            Id = d.Id,
            SpaceId = d.SpaceId,
            DelegateUserId = d.DelegateUserId,
            DelegateUserName = d.DelegateUserName,
            GrantedByUserId = d.GrantedByUserId,
            GrantedByUserName = d.GrantedByUserName,
            Scopes = FormatScopes(d.Scopes),
            Reason = d.Reason,
            Status = d.Status.ToString(),
            ExpiresAt = d.ExpiresAt,
            IsEffective = d.IsEffective,
            CreatedAt = d.CreatedAt,
            RevokedAt = d.RevokedAt
        };
    }

    private static DelegatedAdminScope ParseScopes(IReadOnlyList<string> scopeStrings)
    {
        var result = DelegatedAdminScope.None;
        foreach (var s in scopeStrings)
        {
            if (Enum.TryParse<DelegatedAdminScope>(s, true, out var scope))
                result |= scope;
        }
        return result;
    }

    private static IReadOnlyList<string> FormatScopes(DelegatedAdminScope scopes)
    {
        var result = new List<string>();
        foreach (DelegatedAdminScope value in Enum.GetValues(typeof(DelegatedAdminScope)))
        {
            if (value != DelegatedAdminScope.None && value != DelegatedAdminScope.All && scopes.HasFlag(value))
                result.Add(value.ToString());
        }
        return result;
    }
}
