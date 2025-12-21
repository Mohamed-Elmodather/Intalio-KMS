using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Collaboration.Domain.Entities;

/// <summary>
/// Follow relationship for content and users.
/// </summary>
public class Follow : Entity
{
    public Guid FollowerId { get; private set; }
    public string FollowerName { get; private set; } = string.Empty;
    public FollowableType TargetType { get; private set; }
    public Guid TargetId { get; private set; }
    public bool NotificationsEnabled { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }

    private Follow() { }

    public static Follow Create(
        Guid followerId,
        string followerName,
        FollowableType targetType,
        Guid targetId)
    {
        return new Follow
        {
            FollowerId = followerId,
            FollowerName = followerName,
            TargetType = targetType,
            TargetId = targetId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void SetNotifications(bool enabled) => NotificationsEnabled = enabled;
}

public enum FollowableType
{
    User,
    Community,
    Discussion,
    Article,
    Document,
    Tag,
    Category
}

/// <summary>
/// Lessons Learned entry for knowledge capture.
/// </summary>
public class LessonLearned : AuditableEntity
{
    public LocalizedString Title { get; private set; } = null!;
    public string Description { get; private set; } = string.Empty;
    public string? DescriptionArabic { get; private set; }
    public string Context { get; private set; } = string.Empty;
    public string? ContextArabic { get; private set; }
    public string Challenge { get; private set; } = string.Empty;
    public string? ChallengeArabic { get; private set; }
    public string Solution { get; private set; } = string.Empty;
    public string? SolutionArabic { get; private set; }
    public string Outcome { get; private set; } = string.Empty;
    public string? OutcomeArabic { get; private set; }
    public string? Recommendations { get; private set; }
    public string? RecommendationsArabic { get; private set; }
    public LessonCategory Category { get; private set; }
    public LessonImpact Impact { get; private set; }
    public LessonStatus Status { get; private set; }
    public Guid AuthorId { get; private set; }
    public string AuthorName { get; private set; } = string.Empty;
    public Guid? CommunityId { get; private set; }
    public Guid? ProjectId { get; private set; }
    public string? ProjectName { get; private set; }
    public DateTime? OccurredAt { get; private set; }
    public int ViewCount { get; private set; }
    public int UsefulCount { get; private set; }

    // Navigation properties
    public virtual Community? Community { get; private set; }
    public virtual ICollection<LessonTag> Tags { get; private set; } = new List<LessonTag>();
    public virtual ICollection<Comment> Comments { get; private set; } = new List<Comment>();

    private LessonLearned() { }

    public static LessonLearned Create(
        LocalizedString title,
        string description,
        string context,
        string challenge,
        string solution,
        string outcome,
        LessonCategory category,
        LessonImpact impact,
        Guid authorId,
        string authorName)
    {
        return new LessonLearned
        {
            Title = title,
            Description = description,
            Context = context,
            Challenge = challenge,
            Solution = solution,
            Outcome = outcome,
            Category = category,
            Impact = impact,
            Status = LessonStatus.Draft,
            AuthorId = authorId,
            AuthorName = authorName,
            OccurredAt = DateTime.UtcNow
        };
    }

    public void Update(
        LocalizedString title,
        string description,
        string context,
        string challenge,
        string solution,
        string outcome,
        string? recommendations)
    {
        Title = title;
        Description = description;
        Context = context;
        Challenge = challenge;
        Solution = solution;
        Outcome = outcome;
        Recommendations = recommendations;
    }

    public void SetProject(Guid? projectId, string? projectName)
    {
        ProjectId = projectId;
        ProjectName = projectName;
    }

    public void SetCommunity(Guid? communityId)
    {
        CommunityId = communityId;
    }

    public void Submit()
    {
        Status = LessonStatus.PendingReview;
    }

    public void Approve()
    {
        Status = LessonStatus.Approved;
    }

    public void Reject()
    {
        Status = LessonStatus.Rejected;
    }

    public void Publish()
    {
        Status = LessonStatus.Published;
    }

    public void Archive()
    {
        Status = LessonStatus.Archived;
    }

    public void IncrementViewCount() => ViewCount++;
    public void IncrementUsefulCount() => UsefulCount++;
    public void DecrementUsefulCount() => UsefulCount = Math.Max(0, UsefulCount - 1);
}

public enum LessonCategory
{
    Process,
    Technical,
    Communication,
    TeamManagement,
    RiskManagement,
    StakeholderManagement,
    BudgetManagement,
    QualityAssurance,
    VendorManagement,
    Other
}

public enum LessonImpact
{
    Low,
    Medium,
    High,
    Critical
}

public enum LessonStatus
{
    Draft,
    PendingReview,
    Approved,
    Rejected,
    Published,
    Archived
}

/// <summary>
/// Tag for lessons learned.
/// </summary>
public class LessonTag
{
    public Guid LessonId { get; set; }
    public string Tag { get; set; } = string.Empty;
    public string? TagArabic { get; set; }

    public virtual LessonLearned Lesson { get; set; } = null!;
}
