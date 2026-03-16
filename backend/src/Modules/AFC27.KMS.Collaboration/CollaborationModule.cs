using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AFC27.KMS.Collaboration;

/// <summary>
/// Collaboration module registration.
/// </summary>
public static class CollaborationModule
{
    /// <summary>
    /// Add Collaboration module services.
    /// </summary>
    public static IServiceCollection AddCollaborationModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Add module-specific services
        // services.AddScoped<ICommunityService, CommunityService>();
        // services.AddScoped<IDiscussionService, DiscussionService>();
        // services.AddScoped<ICommentService, CommentService>();
        // services.AddScoped<IFollowService, FollowService>();
        // services.AddScoped<ILessonLearnedService, LessonLearnedService>();
        // services.AddScoped<ILessonActionService, LessonActionService>();
        // services.AddScoped<IMentionService, MentionService>();

        // Add authorization policies
        services.AddAuthorizationBuilder()
            .AddPolicy("CanCreateCommunity", policy =>
                policy.RequireClaim("permission", "communities:create"))
            .AddPolicy("CanManageCommunity", policy =>
                policy.RequireClaim("permission", "communities:manage"))
            .AddPolicy("CanModerateDiscussions", policy =>
                policy.RequireClaim("permission", "discussions:moderate"))
            .AddPolicy("CanApproveLessons", policy =>
                policy.RequireClaim("permission", "lessons:approve"))
            .AddPolicy("CanManageKnowledgeAssets", policy =>
                policy.RequireClaim("permission", "knowledge:manage"));

        return services;
    }
}
