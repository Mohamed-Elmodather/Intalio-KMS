using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AFC27.KMS.Identity;

/// <summary>
/// Identity module registration.
/// </summary>
public static class IdentityModule
{
    /// <summary>
    /// Add Identity module services.
    /// </summary>
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Add module-specific services
        // services.AddScoped<IUserService, UserService>();
        // services.AddScoped<IRoleService, RoleService>();
        // services.AddScoped<IDepartmentService, DepartmentService>();
        // services.AddScoped<IGroupService, GroupService>();

        // Add authorization policies
        services.AddAuthorizationBuilder()
            .AddPolicy("CanManageUsers", policy =>
                policy.RequireClaim("permission", "users:manage"))
            .AddPolicy("CanManageRoles", policy =>
                policy.RequireClaim("permission", "roles:manage"))
            .AddPolicy("CanViewUsers", policy =>
                policy.RequireClaim("permission", "users:view"));

        return services;
    }
}
