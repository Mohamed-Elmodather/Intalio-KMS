using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Content.Application.DTOs;
using AFC27.KMS.Content.Application.Interfaces;
using AFC27.KMS.Content.Domain.Entities;
using AFC27.KMS.Contracts.Common;
using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Content.Application.Services;

/// <summary>
/// Service implementation for Space/Teamspace CRUD and membership management.
/// </summary>
public class SpaceService : ISpaceService
{
    private readonly DbContext _dbContext;
    private readonly ILogger<SpaceService> _logger;

    public SpaceService(DbContext dbContext, ILogger<SpaceService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    // ========================================
    // Queries
    // ========================================

    public async Task<PagedResult<SpaceSummaryDto>> GetSpacesAsync(
        SpaceFilterRequest filter, CancellationToken ct = default)
    {
        var query = _dbContext.Set<Space>()
            .AsNoTracking()
            .Include(s => s.Members)
            .AsQueryable();

        // Search filter
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var searchLower = filter.Search.ToLower();
            query = query.Where(s =>
                s.Name.English.ToLower().Contains(searchLower) ||
                s.Name.Arabic.ToLower().Contains(searchLower) ||
                (s.Description != null && s.Description.English.ToLower().Contains(searchLower)));
        }

        // SpaceType filter
        if (!string.IsNullOrWhiteSpace(filter.SpaceType) && Enum.TryParse<SpaceType>(filter.SpaceType, true, out var spaceType))
        {
            query = query.Where(s => s.SpaceType == spaceType);
        }

        // Public filter
        if (filter.IsPublic.HasValue)
        {
            query = query.Where(s => s.IsPublic == filter.IsPublic.Value);
        }

        // Archived filter
        if (filter.IsArchived.HasValue)
        {
            query = query.Where(s => s.IsArchived == filter.IsArchived.Value);
        }
        else
        {
            // By default, exclude archived spaces
            query = query.Where(s => !s.IsArchived);
        }

        // Parent space filter
        if (filter.ParentSpaceId.HasValue)
        {
            query = query.Where(s => s.ParentSpaceId == filter.ParentSpaceId.Value);
        }

        // Owner filter
        if (filter.OwnerId.HasValue)
        {
            query = query.Where(s => s.OwnerId == filter.OwnerId.Value);
        }

        // Sorting
        query = filter.SortBy?.ToLower() switch
        {
            "name" => filter.SortDescending
                ? query.OrderByDescending(s => s.Name.English)
                : query.OrderBy(s => s.Name.English),
            "membercount" => filter.SortDescending
                ? query.OrderByDescending(s => s.Members.Count)
                : query.OrderBy(s => s.Members.Count),
            _ => filter.SortDescending
                ? query.OrderByDescending(s => s.CreatedAt)
                : query.OrderBy(s => s.CreatedAt)
        };

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(s => new SpaceSummaryDto
            {
                Id = s.Id,
                Name = s.Name.English,
                NameArabic = s.Name.Arabic,
                Description = s.Description != null ? s.Description.English : null,
                DescriptionArabic = s.Description != null ? s.Description.Arabic : null,
                SpaceType = s.SpaceType.ToString(),
                OwnerName = s.OwnerName,
                IconName = s.IconName,
                Color = s.Color,
                IsPublic = s.IsPublic,
                IsArchived = s.IsArchived,
                MemberCount = s.Members.Count,
                ChildSpaceCount = s.ChildSpaces.Count,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync(ct);

        return PagedResult<SpaceSummaryDto>.Create(items, totalCount, filter.Page, filter.PageSize);
    }

    public async Task<SpaceDto?> GetSpaceByIdAsync(Guid id, CancellationToken ct = default)
    {
        var space = await _dbContext.Set<Space>()
            .AsNoTracking()
            .Include(s => s.Members)
            .Include(s => s.ChildSpaces)
            .Include(s => s.ParentSpace)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

        if (space == null)
            return null;

        // Count articles in this space
        var contentCount = await _dbContext.Set<Article>()
            .AsNoTracking()
            .Where(a => a.SpaceId == id)
            .CountAsync(ct);

        return MapToDto(space, contentCount);
    }

    public async Task<IReadOnlyList<SpaceSummaryDto>> GetMySpacesAsync(
        Guid userId, CancellationToken ct = default)
    {
        var spaces = await _dbContext.Set<Space>()
            .AsNoTracking()
            .Include(s => s.Members)
            .Where(s => !s.IsArchived && s.Members.Any(m => m.UserId == userId))
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SpaceSummaryDto
            {
                Id = s.Id,
                Name = s.Name.English,
                NameArabic = s.Name.Arabic,
                Description = s.Description != null ? s.Description.English : null,
                DescriptionArabic = s.Description != null ? s.Description.Arabic : null,
                SpaceType = s.SpaceType.ToString(),
                OwnerName = s.OwnerName,
                IconName = s.IconName,
                Color = s.Color,
                IsPublic = s.IsPublic,
                IsArchived = s.IsArchived,
                MemberCount = s.Members.Count,
                ChildSpaceCount = s.ChildSpaces.Count,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync(ct);

        return spaces;
    }

    // ========================================
    // CRUD
    // ========================================

    public async Task<SpaceDto> CreateSpaceAsync(
        CreateSpaceRequest request, Guid userId, string userName, CancellationToken ct = default)
    {
        var name = LocalizedString.Create(request.Name, request.NameArabic);
        var description = !string.IsNullOrWhiteSpace(request.Description)
            ? LocalizedString.Create(request.Description, request.DescriptionArabic)
            : null;

        Enum.TryParse<SpaceType>(request.SpaceType, true, out var spaceType);

        var space = Space.Create(name, spaceType, userId, userName, request.IsPublic, description);

        if (!string.IsNullOrWhiteSpace(request.IconName) || !string.IsNullOrWhiteSpace(request.Color))
        {
            space.Update(name, description, request.IconName, request.Color, request.IsPublic);
        }

        if (request.ParentSpaceId.HasValue)
        {
            space.SetParent(request.ParentSpaceId);
        }

        _dbContext.Set<Space>().Add(space);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Space {SpaceId} '{SpaceName}' created by user {UserId}",
            space.Id, request.Name, userId);

        return MapToDto(space, 0);
    }

    public async Task<bool> UpdateSpaceAsync(
        Guid id, UpdateSpaceRequest request, Guid userId, CancellationToken ct = default)
    {
        var space = await _dbContext.Set<Space>()
            .Include(s => s.Members)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

        if (space == null)
            return false;

        var name = LocalizedString.Create(request.Name, request.NameArabic);
        var description = !string.IsNullOrWhiteSpace(request.Description)
            ? LocalizedString.Create(request.Description, request.DescriptionArabic)
            : null;

        space.Update(name, description, request.IconName, request.Color, request.IsPublic);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Space {SpaceId} updated by user {UserId}", id, userId);
        return true;
    }

    public async Task<bool> DeleteSpaceAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var space = await _dbContext.Set<Space>()
            .FirstOrDefaultAsync(s => s.Id == id, ct);

        if (space == null)
            return false;

        space.SoftDelete(userId);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Space {SpaceId} deleted by user {UserId}", id, userId);
        return true;
    }

    public async Task<bool> ArchiveSpaceAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var space = await _dbContext.Set<Space>()
            .FirstOrDefaultAsync(s => s.Id == id, ct);

        if (space == null)
            return false;

        space.Archive();
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Space {SpaceId} archived by user {UserId}", id, userId);
        return true;
    }

    public async Task<bool> UnarchiveSpaceAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var space = await _dbContext.Set<Space>()
            .FirstOrDefaultAsync(s => s.Id == id, ct);

        if (space == null)
            return false;

        space.Unarchive();
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Space {SpaceId} unarchived by user {UserId}", id, userId);
        return true;
    }

    // ========================================
    // Membership
    // ========================================

    public async Task<SpaceMemberDto?> AddMemberAsync(
        Guid spaceId, AddSpaceMemberRequest request, Guid userId, CancellationToken ct = default)
    {
        var space = await _dbContext.Set<Space>()
            .Include(s => s.Members)
            .FirstOrDefaultAsync(s => s.Id == spaceId, ct);

        if (space == null)
            return null;

        // Check if already a member
        if (space.Members.Any(m => m.UserId == request.UserId))
        {
            _logger.LogWarning("User {MemberUserId} is already a member of space {SpaceId}",
                request.UserId, spaceId);
            return null;
        }

        Enum.TryParse<SpaceMemberRole>(request.Role, true, out var role);
        var member = space.AddMember(request.UserId, request.UserName, role);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("User {MemberUserId} added to space {SpaceId} with role {Role} by {UserId}",
            request.UserId, spaceId, role, userId);

        return new SpaceMemberDto
        {
            Id = member.Id,
            UserId = member.UserId,
            UserName = member.UserName,
            Role = member.Role.ToString(),
            JoinedAt = member.JoinedAt
        };
    }

    public async Task<bool> RemoveMemberAsync(
        Guid spaceId, Guid memberUserId, Guid userId, CancellationToken ct = default)
    {
        var space = await _dbContext.Set<Space>()
            .Include(s => s.Members)
            .FirstOrDefaultAsync(s => s.Id == spaceId, ct);

        if (space == null)
            return false;

        var member = space.Members.FirstOrDefault(m => m.UserId == memberUserId);
        if (member == null)
            return false;

        // Prevent removing the owner
        if (member.Role == SpaceMemberRole.Owner)
        {
            _logger.LogWarning("Cannot remove owner {MemberUserId} from space {SpaceId}", memberUserId, spaceId);
            return false;
        }

        space.RemoveMember(memberUserId);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("User {MemberUserId} removed from space {SpaceId} by {UserId}",
            memberUserId, spaceId, userId);
        return true;
    }

    public async Task<bool> ChangeMemberRoleAsync(
        Guid spaceId, Guid memberUserId, ChangeSpaceMemberRoleRequest request, Guid userId, CancellationToken ct = default)
    {
        var space = await _dbContext.Set<Space>()
            .Include(s => s.Members)
            .FirstOrDefaultAsync(s => s.Id == spaceId, ct);

        if (space == null)
            return false;

        var member = space.Members.FirstOrDefault(m => m.UserId == memberUserId);
        if (member == null)
            return false;

        if (!Enum.TryParse<SpaceMemberRole>(request.Role, true, out var newRole))
            return false;

        member.ChangeRole(newRole);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("User {MemberUserId} role changed to {Role} in space {SpaceId} by {UserId}",
            memberUserId, newRole, spaceId, userId);
        return true;
    }

    // ========================================
    // Content
    // ========================================

    public async Task<PagedResult<ArticleSummaryDto>> GetSpaceContentAsync(
        Guid spaceId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _dbContext.Set<Article>()
            .AsNoTracking()
            .Where(a => a.SpaceId == spaceId)
            .OrderByDescending(a => a.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new ArticleSummaryDto
            {
                Id = a.Id,
                Title = a.Title.English,
                TitleArabic = a.Title.Arabic,
                Summary = a.Summary != null ? a.Summary.English : null,
                SummaryArabic = a.Summary != null ? a.Summary.Arabic : null,
                Slug = a.Slug,
                Type = a.Type.ToString(),
                Status = a.Status.ToString(),
                ThumbnailUrl = a.ThumbnailUrl,
                AuthorName = a.AuthorName,
                IsFeatured = a.IsFeatured,
                ViewCount = a.ViewCount,
                PublishedAt = a.PublishedAt
            })
            .ToListAsync(ct);

        return PagedResult<ArticleSummaryDto>.Create(items, totalCount, page, pageSize);
    }

    // ========================================
    // Personal Workspace
    // ========================================

    public async Task<SpaceDto> GetOrCreateMyWorkspaceAsync(
        Guid userId, string userName, CancellationToken ct = default)
    {
        // Try to find existing personal workspace
        var personalSpace = await _dbContext.Set<Space>()
            .Include(s => s.Members)
            .Include(s => s.ChildSpaces)
            .FirstOrDefaultAsync(s =>
                s.SpaceType == SpaceType.Personal
                && s.OwnerId == userId
                && !s.IsDeleted,
                ct);

        if (personalSpace != null)
        {
            var contentCount = await _dbContext.Set<Article>()
                .AsNoTracking()
                .Where(a => a.SpaceId == personalSpace.Id)
                .CountAsync(ct);

            return MapToDto(personalSpace, contentCount);
        }

        // Create personal workspace
        var name = LocalizedString.Create($"{userName}'s Workspace", $"مساحة عمل {userName}");
        var description = LocalizedString.Create(
            "Personal workspace for drafts, notes, and bookmarked items",
            "مساحة عمل شخصية للمسودات والملاحظات والعناصر المحفوظة");

        personalSpace = Space.Create(name, SpaceType.Personal, userId, userName, isPublic: false, description);

        _dbContext.Set<Space>().Add(personalSpace);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Personal workspace {SpaceId} created for user {UserId} '{UserName}'",
            personalSpace.Id, userId, userName);

        return MapToDto(personalSpace, 0);
    }

    // ========================================
    // Private Helpers
    // ========================================

    private static SpaceDto MapToDto(Space space, int contentCount)
    {
        return new SpaceDto
        {
            Id = space.Id,
            Name = space.Name.English,
            NameArabic = space.Name.Arabic,
            Description = space.Description?.English,
            DescriptionArabic = space.Description?.Arabic,
            SpaceType = space.SpaceType.ToString(),
            OwnerId = space.OwnerId,
            OwnerName = space.OwnerName,
            IconName = space.IconName,
            Color = space.Color,
            IsPublic = space.IsPublic,
            IsArchived = space.IsArchived,
            ParentSpaceId = space.ParentSpaceId,
            ParentSpaceName = space.ParentSpace?.Name?.English,
            MemberCount = space.Members.Count,
            ContentCount = contentCount,
            ChildSpaceCount = space.ChildSpaces.Count,
            Members = space.Members.Select(m => new SpaceMemberDto
            {
                Id = m.Id,
                UserId = m.UserId,
                UserName = m.UserName,
                Role = m.Role.ToString(),
                JoinedAt = m.JoinedAt
            }).ToList(),
            ChildSpaces = space.ChildSpaces.Select(c => new SpaceSummaryDto
            {
                Id = c.Id,
                Name = c.Name.English,
                NameArabic = c.Name.Arabic,
                SpaceType = c.SpaceType.ToString(),
                OwnerName = c.OwnerName,
                IconName = c.IconName,
                Color = c.Color,
                IsPublic = c.IsPublic,
                IsArchived = c.IsArchived,
                CreatedAt = c.CreatedAt
            }).ToList(),
            CreatedAt = space.CreatedAt,
            UpdatedAt = space.ModifiedAt
        };
    }
}
