using Microsoft.Extensions.Logging;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Domain.Entities;

namespace AFC27.KMS.Admin.Application.Services;

/// <summary>
/// Service interface for automation rules (Phase 8E).
/// </summary>
public interface IAutomationRuleService
{
    Task<AutomationRuleDto> CreateAsync(CreateAutomationRuleRequest request, Guid userId, string userName, CancellationToken cancellationToken = default);
    Task<AutomationRuleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AutomationRuleSummaryDto>> ListAsync(Guid? ownerId = null, Guid? spaceId = null, bool? isActive = null, CancellationToken cancellationToken = default);
    Task<AutomationRuleDto?> UpdateAsync(Guid id, UpdateAutomationRuleRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ActivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AutomationRuleExecutionDto>> GetExecutionHistoryAsync(Guid ruleId, int maxResults = 50, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TriggerEventMetadataDto>> GetAvailableTriggerEventsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of the automation rule management service.
/// </summary>
public class AutomationRuleService : IAutomationRuleService
{
    private readonly ILogger<AutomationRuleService> _logger;

    public AutomationRuleService(ILogger<AutomationRuleService> logger)
    {
        _logger = logger;
    }

    public async Task<AutomationRuleDto> CreateAsync(
        CreateAutomationRuleRequest request, Guid userId, string userName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating automation rule '{Name}' for user {UserId}", request.Name, userId);

        await Task.Delay(50, cancellationToken);

        // In a real implementation: create AutomationRule entity and persist via repository
        var rule = AutomationRule.Create(
            userId, userName, request.Name, request.TriggerEvent,
            request.Conditions, request.Actions, request.Description,
            request.SpaceId, request.Priority);

        return MapToDto(rule);
    }

    public async Task<AutomationRuleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await Task.Delay(20, cancellationToken);

        // Mock response
        return new AutomationRuleDto
        {
            Id = id,
            OwnerId = Guid.NewGuid(),
            OwnerName = "Mohammed Al-Rashid",
            Name = "Notify on article publish",
            Description = "Send a notification to the content team when any article is published.",
            TriggerEvent = AutomationTriggerEvents.ArticlePublished,
            Conditions = "[{\"property\":\"categoryName\",\"operator\":\"equals\",\"value\":\"Tournament News\"}]",
            Actions = "[{\"type\":\"send-notification\",\"config\":{\"channel\":\"email\",\"recipients\":\"content-team\",\"template\":\"article-published\"}}]",
            IsActive = true,
            ExecutionCount = 42,
            LastTriggeredAt = DateTime.UtcNow.AddHours(-3),
            Priority = 50,
            CreatedAt = DateTime.UtcNow.AddMonths(-2)
        };
    }

    public async Task<IReadOnlyList<AutomationRuleSummaryDto>> ListAsync(
        Guid? ownerId = null, Guid? spaceId = null, bool? isActive = null, CancellationToken cancellationToken = default)
    {
        await Task.Delay(30, cancellationToken);

        var rules = new List<AutomationRuleSummaryDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Notify on article publish",
                TriggerEvent = AutomationTriggerEvents.ArticlePublished,
                IsActive = true,
                ExecutionCount = 42,
                LastTriggeredAt = DateTime.UtcNow.AddHours(-3),
                OwnerName = "Mohammed Al-Rashid",
                CreatedAt = DateTime.UtcNow.AddMonths(-2)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Auto-tag security documents",
                TriggerEvent = AutomationTriggerEvents.DocumentUploaded,
                IsActive = true,
                ExecutionCount = 15,
                LastTriggeredAt = DateTime.UtcNow.AddDays(-1),
                OwnerName = "Sara Ali",
                CreatedAt = DateTime.UtcNow.AddMonths(-1)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Escalate overdue verifications",
                TriggerEvent = AutomationTriggerEvents.ArticleVerificationOverdue,
                IsActive = false,
                ExecutionCount = 8,
                LastTriggeredAt = DateTime.UtcNow.AddDays(-7),
                OwnerName = "Ahmed Hassan",
                CreatedAt = DateTime.UtcNow.AddWeeks(-3)
            }
        };

        if (isActive.HasValue)
        {
            rules = rules.Where(r => r.IsActive == isActive.Value).ToList();
        }

        return rules;
    }

    public async Task<AutomationRuleDto?> UpdateAsync(
        Guid id, UpdateAutomationRuleRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating automation rule {RuleId}", id);

        await Task.Delay(50, cancellationToken);

        return new AutomationRuleDto
        {
            Id = id,
            OwnerId = Guid.NewGuid(),
            OwnerName = "Mohammed Al-Rashid",
            Name = request.Name,
            Description = request.Description,
            TriggerEvent = request.TriggerEvent,
            Conditions = request.Conditions,
            Actions = request.Actions,
            SpaceId = request.SpaceId,
            IsActive = true,
            ExecutionCount = 42,
            Priority = request.Priority,
            CreatedAt = DateTime.UtcNow.AddMonths(-2),
            ModifiedAt = DateTime.UtcNow
        };
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting automation rule {RuleId}", id);
        await Task.Delay(30, cancellationToken);
        return true;
    }

    public async Task<bool> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Activating automation rule {RuleId}", id);
        await Task.Delay(30, cancellationToken);
        return true;
    }

    public async Task<bool> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deactivating automation rule {RuleId}", id);
        await Task.Delay(30, cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<AutomationRuleExecutionDto>> GetExecutionHistoryAsync(
        Guid ruleId, int maxResults = 50, CancellationToken cancellationToken = default)
    {
        await Task.Delay(30, cancellationToken);

        return new List<AutomationRuleExecutionDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                RuleId = ruleId,
                RuleName = "Notify on article publish",
                TriggerEvent = AutomationTriggerEvents.ArticlePublished,
                TriggerEntityId = Guid.NewGuid(),
                TriggerEntityType = "Article",
                Success = true,
                DurationMs = 120,
                ExecutedAt = DateTime.UtcNow.AddHours(-3)
            },
            new()
            {
                Id = Guid.NewGuid(),
                RuleId = ruleId,
                RuleName = "Notify on article publish",
                TriggerEvent = AutomationTriggerEvents.ArticlePublished,
                TriggerEntityId = Guid.NewGuid(),
                TriggerEntityType = "Article",
                Success = true,
                DurationMs = 95,
                ExecutedAt = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                Id = Guid.NewGuid(),
                RuleId = ruleId,
                RuleName = "Notify on article publish",
                TriggerEvent = AutomationTriggerEvents.ArticlePublished,
                TriggerEntityId = Guid.NewGuid(),
                TriggerEntityType = "Article",
                Success = false,
                ErrorMessage = "Notification channel 'email' returned 503",
                DurationMs = 5200,
                ExecutedAt = DateTime.UtcNow.AddDays(-3)
            }
        };
    }

    public async Task<IReadOnlyList<TriggerEventMetadataDto>> GetAvailableTriggerEventsAsync(
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(10, cancellationToken);

        return new List<TriggerEventMetadataDto>
        {
            new() { EventName = AutomationTriggerEvents.ArticleCreated, DisplayName = "Article Created", Category = "Content", AvailableProperties = new[] { "title", "authorId", "categoryName", "type", "spaceId" } },
            new() { EventName = AutomationTriggerEvents.ArticlePublished, DisplayName = "Article Published", Category = "Content", AvailableProperties = new[] { "title", "authorId", "categoryName", "type", "spaceId" } },
            new() { EventName = AutomationTriggerEvents.ArticleUpdated, DisplayName = "Article Updated", Category = "Content", AvailableProperties = new[] { "title", "authorId", "categoryName", "type", "spaceId" } },
            new() { EventName = AutomationTriggerEvents.ArticleSubmittedForReview, DisplayName = "Article Submitted for Review", Category = "Content", AvailableProperties = new[] { "title", "authorId", "categoryName" } },
            new() { EventName = AutomationTriggerEvents.ArticleApproved, DisplayName = "Article Approved", Category = "Content", AvailableProperties = new[] { "title", "authorId", "approvedById" } },
            new() { EventName = AutomationTriggerEvents.ArticleVerificationDue, DisplayName = "Article Verification Due", Category = "Content", AvailableProperties = new[] { "title", "ownerId", "daysUntilDue" } },
            new() { EventName = AutomationTriggerEvents.ArticleVerificationOverdue, DisplayName = "Article Verification Overdue", Category = "Content", AvailableProperties = new[] { "title", "ownerId", "daysOverdue" } },
            new() { EventName = AutomationTriggerEvents.DocumentUploaded, DisplayName = "Document Uploaded", Category = "Documents", AvailableProperties = new[] { "fileName", "fileType", "uploaderId", "libraryId" } },
            new() { EventName = AutomationTriggerEvents.DocumentUpdated, DisplayName = "Document Updated", Category = "Documents", AvailableProperties = new[] { "fileName", "fileType", "updatedById" } },
            new() { EventName = AutomationTriggerEvents.CommentCreated, DisplayName = "Comment Created", Category = "Collaboration", AvailableProperties = new[] { "targetType", "targetId", "authorId", "content" } },
            new() { EventName = AutomationTriggerEvents.MentionCreated, DisplayName = "Mention Created", Category = "Collaboration", AvailableProperties = new[] { "mentionedUserId", "targetEntityType", "mentionContext" } },
            new() { EventName = AutomationTriggerEvents.DiscussionCreated, DisplayName = "Discussion Created", Category = "Collaboration", AvailableProperties = new[] { "title", "communityId", "authorId" } },
            new() { EventName = AutomationTriggerEvents.UserCreated, DisplayName = "User Created", Category = "Identity", AvailableProperties = new[] { "email", "departmentId", "role" } },
            new() { EventName = AutomationTriggerEvents.LessonCreated, DisplayName = "Lesson Created", Category = "Collaboration", AvailableProperties = new[] { "title", "category", "impact", "authorId" } },
            new() { EventName = AutomationTriggerEvents.LessonApproved, DisplayName = "Lesson Approved", Category = "Collaboration", AvailableProperties = new[] { "title", "category", "approvedById" } }
        };
    }

    private static AutomationRuleDto MapToDto(AutomationRule rule)
    {
        return new AutomationRuleDto
        {
            Id = rule.Id,
            OwnerId = rule.OwnerId,
            OwnerName = rule.OwnerName,
            Name = rule.Name,
            Description = rule.Description,
            TriggerEvent = rule.TriggerEvent,
            Conditions = rule.Conditions,
            Actions = rule.Actions,
            IsActive = rule.IsActive,
            ExecutionCount = rule.ExecutionCount,
            Priority = rule.Priority,
            CreatedAt = rule.CreatedAt
        };
    }
}
