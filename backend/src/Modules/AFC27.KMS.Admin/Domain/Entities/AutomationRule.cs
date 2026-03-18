using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Admin.Domain.Entities;

/// <summary>
/// User-configurable automation rule (Phase 8E).
/// Allows users to define trigger-condition-action automations based on domain events.
/// Leverages the existing 22+ webhook event types and notification delivery channels.
/// </summary>
public class AutomationRule : AuditableEntity
{
    public Guid OwnerId { get; private set; }
    public string OwnerName { get; private set; } = string.Empty;
    public Guid? SpaceId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    /// <summary>
    /// The domain event that triggers this rule (e.g. "article.published", "comment.created", "document.uploaded").
    /// Matches the existing webhook event type strings.
    /// </summary>
    public string TriggerEvent { get; private set; } = string.Empty;

    /// <summary>
    /// JSON array of conditions that must all be met for the rule to fire.
    /// Each condition is: { "property": "...", "operator": "...", "value": "..." }
    /// </summary>
    public string Conditions { get; private set; } = "[]";

    /// <summary>
    /// JSON array of actions to execute when the rule fires.
    /// Each action is: { "type": "...", "config": { ... } }
    /// Supported types: send-notification, assign-tag, move-to-space, send-webhook, assign-owner, etc.
    /// </summary>
    public string Actions { get; private set; } = "[]";

    /// <summary>
    /// Whether this rule is currently active and will be evaluated.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// How many times this rule has been triggered and executed.
    /// </summary>
    public int ExecutionCount { get; private set; }

    /// <summary>
    /// The last time this rule was triggered.
    /// </summary>
    public DateTime? LastTriggeredAt { get; private set; }

    /// <summary>
    /// Optional: the last error encountered when executing this rule.
    /// </summary>
    public string? LastError { get; private set; }

    /// <summary>
    /// Priority for rule evaluation order. Lower = higher priority.
    /// </summary>
    public int Priority { get; private set; }

    private AutomationRule() { }

    public static AutomationRule Create(
        Guid ownerId,
        string ownerName,
        string name,
        string triggerEvent,
        string conditions,
        string actions,
        string? description = null,
        Guid? spaceId = null,
        int priority = 100)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Rule name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(triggerEvent))
            throw new ArgumentException("Trigger event cannot be empty", nameof(triggerEvent));

        var rule = new AutomationRule
        {
            OwnerId = ownerId,
            OwnerName = ownerName,
            SpaceId = spaceId,
            Name = name,
            Description = description,
            TriggerEvent = triggerEvent,
            Conditions = conditions ?? "[]",
            Actions = actions ?? "[]",
            IsActive = true,
            ExecutionCount = 0,
            Priority = priority
        };

        rule.AddDomainEvent(new AutomationRuleCreatedEvent(rule.Id, ownerId, triggerEvent));
        return rule;
    }

    public void Update(
        string name,
        string? description,
        string triggerEvent,
        string conditions,
        string actions,
        Guid? spaceId,
        int priority)
    {
        Name = name;
        Description = description;
        TriggerEvent = triggerEvent;
        Conditions = conditions ?? "[]";
        Actions = actions ?? "[]";
        SpaceId = spaceId;
        Priority = priority;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void RecordExecution(bool success, string? error = null)
    {
        ExecutionCount++;
        LastTriggeredAt = DateTime.UtcNow;
        LastError = success ? null : error;
    }
}

// Domain Events
public record AutomationRuleCreatedEvent(Guid RuleId, Guid OwnerId, string TriggerEvent) : DomainEvent;
public record AutomationRuleTriggeredEvent(Guid RuleId, string TriggerEvent, Guid? EntityId) : DomainEvent;

/// <summary>
/// Well-known trigger event types that map to existing domain events.
/// </summary>
public static class AutomationTriggerEvents
{
    // Content events
    public const string ArticleCreated = "article.created";
    public const string ArticlePublished = "article.published";
    public const string ArticleUpdated = "article.updated";
    public const string ArticleDeleted = "article.deleted";
    public const string ArticleSubmittedForReview = "article.submitted_for_review";
    public const string ArticleApproved = "article.approved";
    public const string ArticleRejected = "article.rejected";
    public const string ArticleVerificationDue = "article.verification_due";
    public const string ArticleVerificationOverdue = "article.verification_overdue";

    // Document events
    public const string DocumentUploaded = "document.uploaded";
    public const string DocumentUpdated = "document.updated";
    public const string DocumentDeleted = "document.deleted";
    public const string DocumentCheckedOut = "document.checked_out";
    public const string DocumentCheckedIn = "document.checked_in";

    // Collaboration events
    public const string CommentCreated = "comment.created";
    public const string MentionCreated = "mention.created";
    public const string DiscussionCreated = "discussion.created";
    public const string DiscussionAnswered = "discussion.answered";

    // User events
    public const string UserCreated = "user.created";
    public const string UserDeactivated = "user.deactivated";

    // Lesson events
    public const string LessonCreated = "lesson.created";
    public const string LessonApproved = "lesson.approved";
}

/// <summary>
/// Well-known automation action types.
/// </summary>
public static class AutomationActionTypes
{
    public const string SendNotification = "send-notification";
    public const string SendEmail = "send-email";
    public const string SendWebhook = "send-webhook";
    public const string AssignTag = "assign-tag";
    public const string AssignOwner = "assign-owner";
    public const string MoveToSpace = "move-to-space";
    public const string SetStatus = "set-status";
    public const string CreateTask = "create-task";
    public const string TriggerWorkflow = "trigger-workflow";
}
