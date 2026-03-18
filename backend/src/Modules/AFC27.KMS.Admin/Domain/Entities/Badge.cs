using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Admin.Domain.Entities;

/// <summary>
/// A badge that users can earn for knowledge-sharing accomplishments (Phase 10 - Gamification).
/// Badges are awarded based on configurable criteria such as article count,
/// verification count, feedback quality, or manual assignment.
/// </summary>
public class Badge : AuditableEntity
{
    public LocalizedString Name { get; private set; } = new();
    public LocalizedString? Description { get; private set; }

    /// <summary>
    /// Icon identifier or URL for the badge image.
    /// </summary>
    public string IconUrl { get; private set; } = string.Empty;

    /// <summary>
    /// CSS colour code for the badge (e.g., "#D4AF37" for gold).
    /// </summary>
    public string Color { get; private set; } = "#D4AF37";

    /// <summary>
    /// Badge tier: Bronze, Silver, Gold, Platinum.
    /// </summary>
    public BadgeTier Tier { get; private set; } = BadgeTier.Bronze;

    /// <summary>
    /// Category of knowledge activity this badge recognises.
    /// </summary>
    public BadgeCategory Category { get; private set; } = BadgeCategory.Contribution;

    /// <summary>
    /// Point value awarded when this badge is earned.
    /// </summary>
    public int Points { get; private set; }

    /// <summary>
    /// Whether this badge is currently active and earnable.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Whether this badge can be earned multiple times.
    /// </summary>
    public bool IsRepeatable { get; private set; } = false;

    /// <summary>
    /// Auto-award criteria as a JSON rule.
    /// Example: {"metric":"articleCount","operator":">=","value":10}
    /// Null means the badge is awarded manually.
    /// </summary>
    public string? CriteriaJson { get; private set; }

    /// <summary>
    /// Display order for sorting in the UI.
    /// </summary>
    public int SortOrder { get; private set; }

    private Badge() { }

    public static Badge Create(
        LocalizedString name,
        string iconUrl,
        BadgeTier tier,
        BadgeCategory category,
        int points)
    {
        return new Badge
        {
            Id = Guid.NewGuid(),
            Name = name,
            IconUrl = iconUrl,
            Tier = tier,
            Category = category,
            Points = points,
            IsActive = true
        };
    }

    public void Update(LocalizedString name, LocalizedString? description)
    {
        Name = name;
        Description = description;
    }

    public void SetAppearance(string iconUrl, string color)
    {
        IconUrl = iconUrl;
        Color = color;
    }

    public void SetCriteria(string? criteriaJson) => CriteriaJson = criteriaJson;
    public void SetActive(bool active) => IsActive = active;
    public void SetRepeatable(bool repeatable) => IsRepeatable = repeatable;
    public void SetPoints(int points) => Points = points;
    public void SetSortOrder(int order) => SortOrder = order;
}

/// <summary>
/// A specific achievement earned by a user.
/// </summary>
public class Achievement : AuditableEntity
{
    /// <summary>
    /// The badge that was earned.
    /// </summary>
    public Guid BadgeId { get; private set; }
    public virtual Badge Badge { get; private set; } = null!;

    /// <summary>
    /// The user who earned the badge.
    /// </summary>
    public Guid UserId { get; private set; }
    public string UserName { get; private set; } = string.Empty;

    /// <summary>
    /// When the achievement was earned.
    /// </summary>
    public DateTime EarnedAt { get; private set; }

    /// <summary>
    /// How the badge was awarded.
    /// </summary>
    public AwardMethod AwardMethod { get; private set; }

    /// <summary>
    /// Optional note explaining why the badge was awarded (for manual awards).
    /// </summary>
    public string? Reason { get; private set; }

    /// <summary>
    /// ID of the admin who manually awarded the badge (if applicable).
    /// </summary>
    public Guid? AwardedByUserId { get; private set; }
    public string? AwardedByName { get; private set; }

    /// <summary>
    /// Points earned with this achievement (copied from Badge at award time).
    /// </summary>
    public int PointsEarned { get; private set; }

    /// <summary>
    /// Whether the user has acknowledged/viewed the achievement.
    /// </summary>
    public bool IsAcknowledged { get; private set; }
    public DateTime? AcknowledgedAt { get; private set; }

    private Achievement() { }

    public static Achievement Create(
        Guid badgeId,
        Guid userId,
        string userName,
        int points,
        AwardMethod method,
        string? reason = null,
        Guid? awardedByUserId = null,
        string? awardedByName = null)
    {
        return new Achievement
        {
            Id = Guid.NewGuid(),
            BadgeId = badgeId,
            UserId = userId,
            UserName = userName,
            EarnedAt = DateTime.UtcNow,
            PointsEarned = points,
            AwardMethod = method,
            Reason = reason,
            AwardedByUserId = awardedByUserId,
            AwardedByName = awardedByName
        };
    }

    public void Acknowledge()
    {
        IsAcknowledged = true;
        AcknowledgedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Leaderboard entry (computed/cached).
/// </summary>
public class LeaderboardEntry
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Department { get; set; }
    public int TotalPoints { get; set; }
    public int BadgeCount { get; set; }
    public int ArticleCount { get; set; }
    public int VerificationCount { get; set; }
    public int Rank { get; set; }
}

public enum BadgeTier
{
    Bronze = 0,
    Silver = 1,
    Gold = 2,
    Platinum = 3
}

public enum BadgeCategory
{
    /// <summary>Content creation (articles written).</summary>
    Contribution = 0,
    /// <summary>Knowledge verification activities.</summary>
    Verification = 1,
    /// <summary>Collaboration (edits, reviews, comments).</summary>
    Collaboration = 2,
    /// <summary>Search and knowledge consumption.</summary>
    Engagement = 3,
    /// <summary>Mentoring and helping others.</summary>
    Mentoring = 4,
    /// <summary>Special or custom achievements.</summary>
    Special = 99
}

public enum AwardMethod
{
    /// <summary>Automatically awarded by the system based on criteria.</summary>
    Automatic = 0,
    /// <summary>Manually awarded by an administrator.</summary>
    Manual = 1
}
