using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Identity.Application.DTOs;
using AFC27.KMS.Contracts.Common;

namespace AFC27.KMS.Identity.Presentation.Controllers;

/// <summary>
/// Role and permission management controller.
/// </summary>
[ApiController]
[Route("api/identity/roles")]
[Authorize(Policy = "CanManageRoles")]
public class RolesController : ControllerBase
{
    private readonly ILogger<RolesController> _logger;

    public RolesController(ILogger<RolesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all roles.
    /// </summary>
    [HttpGet]
    public ActionResult<ApiResponse<IReadOnlyList<RoleDto>>> GetRoles()
    {
        var roles = new List<RoleDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Administrator",
                NameArabic = "مدير النظام",
                Description = "Full system access",
                DescriptionArabic = "صلاحيات كاملة على النظام",
                IsSystemRole = true,
                UserCount = 5,
                Permissions = GetAdminPermissions(),
                CreatedAt = DateTime.UtcNow.AddYears(-1)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Content Manager",
                NameArabic = "مدير المحتوى",
                Description = "Manage all content",
                DescriptionArabic = "إدارة جميع المحتويات",
                IsSystemRole = true,
                UserCount = 15,
                Permissions = GetContentManagerPermissions(),
                CreatedAt = DateTime.UtcNow.AddMonths(-6)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "User",
                NameArabic = "مستخدم",
                Description = "Standard user access",
                DescriptionArabic = "صلاحيات المستخدم العادي",
                IsSystemRole = true,
                UserCount = 500,
                Permissions = GetUserPermissions(),
                CreatedAt = DateTime.UtcNow.AddYears(-1)
            }
        };

        return Ok(ApiResponse<IReadOnlyList<RoleDto>>.Ok(roles));
    }

    /// <summary>
    /// Get role by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public ActionResult<ApiResponse<RoleDto>> GetRole(Guid id)
    {
        var role = new RoleDto
        {
            Id = id,
            Name = "Content Manager",
            NameArabic = "مدير المحتوى",
            Description = "Manage all content",
            IsSystemRole = false,
            UserCount = 15,
            Permissions = GetContentManagerPermissions(),
            CreatedAt = DateTime.UtcNow.AddMonths(-6)
        };

        return Ok(ApiResponse<RoleDto>.Ok(role));
    }

    /// <summary>
    /// Create a new role.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<RoleDto>>> CreateRole([FromBody] CreateRoleRequest request)
    {
        _logger.LogInformation("Creating role {RoleName}", request.Name);

        await Task.Delay(100);

        var role = new RoleDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            NameArabic = request.NameArabic,
            Description = request.Description,
            DescriptionArabic = request.DescriptionArabic,
            IsSystemRole = false,
            UserCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        return CreatedAtAction(nameof(GetRole), new { id = role.Id }, ApiResponse<RoleDto>.Ok(role));
    }

    /// <summary>
    /// Update a role.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> UpdateRole(Guid id, [FromBody] UpdateRoleRequest request)
    {
        _logger.LogInformation("Updating role {RoleId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Role updated successfully"));
    }

    /// <summary>
    /// Delete a role.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> DeleteRole(Guid id)
    {
        _logger.LogInformation("Deleting role {RoleId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Role deleted successfully"));
    }

    /// <summary>
    /// Get all permissions.
    /// </summary>
    [HttpGet("permissions")]
    public ActionResult<ApiResponse<IReadOnlyList<PermissionDto>>> GetPermissions()
    {
        var permissions = new List<PermissionDto>
        {
            // Content permissions
            new() { Id = Guid.NewGuid(), Name = "content:view", Resource = "Content", Action = "View", Description = "View content" },
            new() { Id = Guid.NewGuid(), Name = "content:create", Resource = "Content", Action = "Create", Description = "Create content" },
            new() { Id = Guid.NewGuid(), Name = "content:edit", Resource = "Content", Action = "Edit", Description = "Edit content" },
            new() { Id = Guid.NewGuid(), Name = "content:delete", Resource = "Content", Action = "Delete", Description = "Delete content" },
            new() { Id = Guid.NewGuid(), Name = "content:publish", Resource = "Content", Action = "Publish", Description = "Publish content" },

            // Document permissions
            new() { Id = Guid.NewGuid(), Name = "documents:view", Resource = "Documents", Action = "View", Description = "View documents" },
            new() { Id = Guid.NewGuid(), Name = "documents:upload", Resource = "Documents", Action = "Upload", Description = "Upload documents" },
            new() { Id = Guid.NewGuid(), Name = "documents:edit", Resource = "Documents", Action = "Edit", Description = "Edit documents" },
            new() { Id = Guid.NewGuid(), Name = "documents:delete", Resource = "Documents", Action = "Delete", Description = "Delete documents" },

            // User permissions
            new() { Id = Guid.NewGuid(), Name = "users:view", Resource = "Users", Action = "View", Description = "View users" },
            new() { Id = Guid.NewGuid(), Name = "users:manage", Resource = "Users", Action = "Manage", Description = "Manage users" },

            // Role permissions
            new() { Id = Guid.NewGuid(), Name = "roles:view", Resource = "Roles", Action = "View", Description = "View roles" },
            new() { Id = Guid.NewGuid(), Name = "roles:manage", Resource = "Roles", Action = "Manage", Description = "Manage roles" },

            // Media permissions
            new() { Id = Guid.NewGuid(), Name = "media:view", Resource = "Media", Action = "View", Description = "View media" },
            new() { Id = Guid.NewGuid(), Name = "media:upload", Resource = "Media", Action = "Upload", Description = "Upload media" },
            new() { Id = Guid.NewGuid(), Name = "media:edit", Resource = "Media", Action = "Edit", Description = "Edit media" },

            // Workflow permissions
            new() { Id = Guid.NewGuid(), Name = "workflow:view", Resource = "Workflow", Action = "View", Description = "View workflows" },
            new() { Id = Guid.NewGuid(), Name = "workflow:manage", Resource = "Workflow", Action = "Manage", Description = "Manage workflows" },

            // Admin permissions
            new() { Id = Guid.NewGuid(), Name = "admin:access", Resource = "Admin", Action = "Access", Description = "Access admin area" },
            new() { Id = Guid.NewGuid(), Name = "admin:settings", Resource = "Admin", Action = "Settings", Description = "Manage settings" }
        };

        return Ok(ApiResponse<IReadOnlyList<PermissionDto>>.Ok(permissions));
    }

    /// <summary>
    /// Update role permissions.
    /// </summary>
    [HttpPut("{id:guid}/permissions")]
    public async Task<ActionResult<ApiResponse>> UpdateRolePermissions(Guid id, [FromBody] UpdateRolePermissionsRequest request)
    {
        _logger.LogInformation("Updating permissions for role {RoleId}", id);

        await Task.Delay(100);

        return Ok(ApiResponse.Ok("Role permissions updated successfully"));
    }

    /// <summary>
    /// Get users with a specific role.
    /// </summary>
    [HttpGet("{id:guid}/users")]
    public ActionResult<ApiResponse<IReadOnlyList<UserSummaryDto>>> GetRoleUsers(Guid id)
    {
        var users = new List<UserSummaryDto>
        {
            new() { Id = Guid.NewGuid(), DisplayName = "Ahmed Hassan", Email = "ahmed.hassan@afc27.com" },
            new() { Id = Guid.NewGuid(), DisplayName = "Sara Ali", Email = "sara.ali@afc27.com" }
        };

        return Ok(ApiResponse<IReadOnlyList<UserSummaryDto>>.Ok(users));
    }

    private static IReadOnlyList<PermissionDto> GetAdminPermissions()
    {
        return new List<PermissionDto>
        {
            new() { Id = Guid.NewGuid(), Name = "admin:access", Resource = "Admin", Action = "Access" },
            new() { Id = Guid.NewGuid(), Name = "admin:settings", Resource = "Admin", Action = "Settings" },
            new() { Id = Guid.NewGuid(), Name = "users:manage", Resource = "Users", Action = "Manage" },
            new() { Id = Guid.NewGuid(), Name = "roles:manage", Resource = "Roles", Action = "Manage" }
        };
    }

    private static IReadOnlyList<PermissionDto> GetContentManagerPermissions()
    {
        return new List<PermissionDto>
        {
            new() { Id = Guid.NewGuid(), Name = "content:view", Resource = "Content", Action = "View" },
            new() { Id = Guid.NewGuid(), Name = "content:create", Resource = "Content", Action = "Create" },
            new() { Id = Guid.NewGuid(), Name = "content:edit", Resource = "Content", Action = "Edit" },
            new() { Id = Guid.NewGuid(), Name = "content:publish", Resource = "Content", Action = "Publish" }
        };
    }

    private static IReadOnlyList<PermissionDto> GetUserPermissions()
    {
        return new List<PermissionDto>
        {
            new() { Id = Guid.NewGuid(), Name = "content:view", Resource = "Content", Action = "View" },
            new() { Id = Guid.NewGuid(), Name = "documents:view", Resource = "Documents", Action = "View" }
        };
    }
}
