using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Admin.Domain.Entities;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Application.Interfaces;
using AFC27.KMS.SharedKernel.Interfaces;

using AFC27.KMS.Identity.Domain.Entities;

namespace AFC27.KMS.Admin.Application.Services;

/// <summary>
/// Service for user lifecycle management.
/// </summary>
public class UserLifecycleService : IUserLifecycleService
{
    private readonly DbContext _dbContext;
    private readonly ICurrentUser _currentUser;
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<UserLifecycleService> _logger;

    public UserLifecycleService(
        DbContext dbContext,
        ICurrentUser currentUser,
        IAuditLogService auditLogService,
        ILogger<UserLifecycleService> logger)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _auditLogService = auditLogService;
        _logger = logger;
    }

    public async Task<UserDto> InviteUserAsync(
        InviteUserRequest request,
        CancellationToken cancellationToken = default)
    {
        // Check if user already exists
        var existingUser = await _dbContext.Set<User>()
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (existingUser != null)
            throw new InvalidOperationException($"User with email {request.Email} already exists");

        var user = User.Create(
            request.Email,
            request.DisplayName,
            request.DisplayNameArabic);

        if (!string.IsNullOrEmpty(request.JobTitle))
            user.UpdateProfile(request.DisplayName, request.DisplayNameArabic, request.JobTitle, null, null, "en");

        if (request.DepartmentId.HasValue)
            user.SetDepartment(request.DepartmentId.Value);

        // Activate the user (stub for Invite)
        user.Activate();

        _dbContext.Set<User>().Add(user);

        // Assign roles - stub implementation using UserRoles collection
        if (request.RoleIds?.Any() == true)
        {
            foreach (var roleId in request.RoleIds)
            {
                user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = roleId });
            }
        }

        // Assign groups - stub implementation using GroupMemberships collection
        if (request.GroupIds?.Any() == true)
        {
            foreach (var groupId in request.GroupIds)
            {
                user.GroupMemberships.Add(new GroupMember { UserId = user.Id, GroupId = groupId });
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogActionAsync(
            AuditActions.UserInvited,
            AuditCategories.UserManagement,
            "User",
            user.Id,
            newValues: new { request.Email, request.DisplayName },
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "User {UserId} invited: {Email}",
            user.Id, request.Email);

        // TODO: Send invite email if request.SendInviteEmail is true

        return await GetUserDtoAsync(user.Id, cancellationToken);
    }

    public async Task<bool> SuspendUserAsync(
        Guid userId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            return false;

        var oldStatus = user.IsActive;
        // Stub implementation - use Deactivate instead of Suspend
        user.Deactivate();

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogActionAsync(
            AuditActions.UserSuspended,
            AuditCategories.UserManagement,
            "User",
            userId,
            oldValues: new { IsActive = oldStatus },
            newValues: new { IsActive = user.IsActive, Reason = reason },
            severity: AuditSeverity.Warning,
            cancellationToken: cancellationToken);

        _logger.LogWarning(
            "User {UserId} suspended: {Reason}",
            userId, reason);

        return true;
    }

    public async Task<bool> ReactivateUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            return false;

        var oldStatus = user.IsActive;
        // Stub implementation - use Activate instead of Reactivate
        user.Activate();

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogActionAsync(
            AuditActions.UserReactivated,
            AuditCategories.UserManagement,
            "User",
            userId,
            oldValues: new { IsActive = oldStatus },
            newValues: new { IsActive = user.IsActive },
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "User {UserId} reactivated",
            userId);

        return true;
    }

    public async Task<bool> DeleteUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            return false;

        // Stub implementation - soft delete by deactivating
        user.Deactivate();
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogActionAsync(
            AuditActions.UserDeleted,
            AuditCategories.UserManagement,
            "User",
            userId,
            oldValues: new { user.Email, user.DisplayName },
            severity: AuditSeverity.Warning,
            cancellationToken: cancellationToken);

        _logger.LogWarning(
            "User {UserId} deleted: {Email}",
            userId, user.Email);

        return true;
    }

    public async Task<bool> UpdateRolesAsync(
        Guid userId,
        IReadOnlyList<Guid> roleIds,
        CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Set<User>()
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            return false;

        var oldRoleIds = user.UserRoles.Select(r => r.RoleId).ToList();

        // Remove current roles - stub implementation using UserRoles collection
        user.UserRoles.Clear();

        // Add new roles - stub implementation using UserRoles collection
        foreach (var roleId in roleIds)
        {
            user.UserRoles.Add(new UserRole { UserId = userId, RoleId = roleId });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogActionAsync(
            AuditActions.RoleAssigned,
            AuditCategories.UserManagement,
            "User",
            userId,
            oldValues: new { RoleIds = oldRoleIds },
            newValues: new { RoleIds = roleIds },
            cancellationToken: cancellationToken);

        return true;
    }

    public async Task<bool> UpdateGroupsAsync(
        Guid userId,
        IReadOnlyList<Guid> groupIds,
        CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Set<User>()
            .Include(u => u.GroupMemberships)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            return false;

        var oldGroupIds = user.GroupMemberships.Select(g => g.GroupId).ToList();

        // Remove current groups - stub implementation using GroupMemberships collection
        user.GroupMemberships.Clear();

        // Add new groups - stub implementation using GroupMemberships collection
        foreach (var groupId in groupIds)
        {
            user.GroupMemberships.Add(new GroupMember { UserId = userId, GroupId = groupId });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogActionAsync(
            "GroupsUpdated",
            AuditCategories.UserManagement,
            "User",
            userId,
            oldValues: new { GroupIds = oldGroupIds },
            newValues: new { GroupIds = groupIds },
            cancellationToken: cancellationToken);

        return true;
    }

    public async Task<UserActivitySummaryDto> GetActivitySummaryAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            throw new InvalidOperationException($"User {userId} not found");

        // Get activity from audit logs
        var auditLogs = await _dbContext.Set<AuditLogEntry>()
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .ToListAsync(cancellationToken);

        var documentsCreated = auditLogs.Count(a => a.Action == AuditActions.DocumentCreated);
        var documentsViewed = auditLogs.Count(a => a.Action == AuditActions.DocumentViewed);
        var documentsEdited = auditLogs.Count(a => a.Action == AuditActions.DocumentUpdated);
        var articlesCreated = auditLogs.Count(a => a.Action == AuditActions.ArticleCreated);

        var recentActivity = auditLogs
            .OrderByDescending(a => a.Timestamp)
            .Take(20)
            .Select(a => new RecentActivityDto
            {
                Timestamp = a.Timestamp,
                Action = a.Action,
                EntityType = a.EntityType,
                EntityName = null // Would need to look up entity name
            })
            .ToList();

        return new UserActivitySummaryDto
        {
            UserId = userId,
            UserName = user.DisplayName,
            LastLoginAt = user.LastLoginAt,
            DocumentsCreated = documentsCreated,
            DocumentsViewed = documentsViewed,
            DocumentsEdited = documentsEdited,
            ArticlesCreated = articlesCreated,
            TotalActions = auditLogs.Count,
            RecentActivity = recentActivity
        };
    }

    public async Task<BulkInviteResultDto> BulkInviteAsync(
        BulkInviteRequest request,
        CancellationToken cancellationToken = default)
    {
        var createdUsers = new List<UserDto>();
        var errors = new List<BulkInviteErrorDto>();

        foreach (var inviteRequest in request.Users)
        {
            try
            {
                var userDto = await InviteUserAsync(
                    inviteRequest with { SendInviteEmail = request.SendInviteEmails },
                    cancellationToken);
                createdUsers.Add(userDto);
            }
            catch (Exception ex)
            {
                errors.Add(new BulkInviteErrorDto
                {
                    Email = inviteRequest.Email,
                    Error = ex.Message
                });
            }
        }

        return new BulkInviteResultDto
        {
            TotalRequested = request.Users.Count,
            SuccessCount = createdUsers.Count,
            FailureCount = errors.Count,
            CreatedUsers = createdUsers,
            Errors = errors
        };
    }

    private async Task<UserDto> GetUserDtoAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Set<User>()
            .AsNoTracking()
            .Include(u => u.Department)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.GroupMemberships)
                .ThenInclude(ug => ug.Group)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            throw new InvalidOperationException($"User {userId} not found");

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            DisplayNameArabic = user.DisplayNameArabic,
            AvatarUrl = user.AvatarUrl,
            JobTitle = user.JobTitle,
            Department = user.Department?.Name,
            Status = user.IsActive ? "Active" : "Inactive",
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Roles = user.UserRoles.Select(r => r.Role.Name).ToList(),
            Groups = user.GroupMemberships.Select(g => g.Group.Name).ToList()
        };
    }
}
