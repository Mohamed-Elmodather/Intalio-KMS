using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Admin.Domain.Entities;
using AFC27.KMS.SharedKernel.Domain;

namespace AFC27.KMS.Admin.Presentation.Controllers;

/// <summary>
/// Controller for gamification features (Phase 10): badges, achievements, and leaderboards.
/// Manages badge definitions, awards badges to users, and provides leaderboard data.
/// </summary>
[ApiController]
[Route("api/admin/gamification")]
[Authorize]
public class GamificationController : ControllerBase
{
    private readonly ILogger<GamificationController> _logger;

    // In-memory stores (replace with repository in production)
    private static readonly List<Badge> _badges = new();
    private static readonly List<Achievement> _achievements = new();

    public GamificationController(ILogger<GamificationController> logger)
    {
        _logger = logger;
    }

    #region Badge CRUD

    /// <summary>
    /// List all badge definitions.
    /// </summary>
    [HttpGet("badges")]
    [ProducesResponseType(typeof(IEnumerable<BadgeDto>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<BadgeDto>> ListBadges(
        [FromQuery] string? category = null,
        [FromQuery] string? tier = null,
        [FromQuery] bool? isActive = null)
    {
        var query = _badges.AsEnumerable();

        if (!string.IsNullOrEmpty(category) && Enum.TryParse<BadgeCategory>(category, true, out var cat))
            query = query.Where(b => b.Category == cat);
        if (!string.IsNullOrEmpty(tier) && Enum.TryParse<BadgeTier>(tier, true, out var t))
            query = query.Where(b => b.Tier == t);
        if (isActive.HasValue)
            query = query.Where(b => b.IsActive == isActive.Value);

        var badges = query.OrderBy(b => b.SortOrder).Select(MapBadgeDto).ToList();
        return Ok(badges);
    }

    /// <summary>
    /// Get a badge by ID.
    /// </summary>
    [HttpGet("badges/{id:guid}")]
    [ProducesResponseType(typeof(BadgeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<BadgeDto> GetBadge(Guid id)
    {
        var badge = _badges.FirstOrDefault(b => b.Id == id);
        if (badge == null) return NotFound();
        return Ok(MapBadgeDto(badge));
    }

    /// <summary>
    /// Create a new badge definition.
    /// </summary>
    [HttpPost("badges")]
    [Authorize(Policy = "CanManageUsers")]
    [ProducesResponseType(typeof(BadgeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<BadgeDto> CreateBadge([FromBody] CreateBadgeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { error = "Badge name is required" });

        var name = LocalizedString.Create(request.Name, request.NameArabic);
        var tier = Enum.TryParse<BadgeTier>(request.Tier, true, out var t) ? t : BadgeTier.Bronze;
        var category = Enum.TryParse<BadgeCategory>(request.Category, true, out var c) ? c : BadgeCategory.Contribution;

        var badge = Badge.Create(name, request.IconUrl ?? "/badges/default.png", tier, category, request.Points);

        if (!string.IsNullOrEmpty(request.Description))
            badge.Update(name, LocalizedString.Create(request.Description, request.DescriptionArabic));

        if (!string.IsNullOrEmpty(request.Color))
            badge.SetAppearance(badge.IconUrl, request.Color);

        if (!string.IsNullOrEmpty(request.CriteriaJson))
            badge.SetCriteria(request.CriteriaJson);

        if (request.IsRepeatable.HasValue)
            badge.SetRepeatable(request.IsRepeatable.Value);

        _badges.Add(badge);

        _logger.LogInformation("Created badge {BadgeId}: {Name}", badge.Id, request.Name);

        return Created($"/api/admin/gamification/badges/{badge.Id}", MapBadgeDto(badge));
    }

    /// <summary>
    /// Update a badge definition.
    /// </summary>
    [HttpPut("badges/{id:guid}")]
    [Authorize(Policy = "CanManageUsers")]
    [ProducesResponseType(typeof(BadgeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<BadgeDto> UpdateBadge(Guid id, [FromBody] UpdateBadgeRequest request)
    {
        var badge = _badges.FirstOrDefault(b => b.Id == id);
        if (badge == null) return NotFound();

        if (!string.IsNullOrEmpty(request.Name))
        {
            var name = LocalizedString.Create(request.Name, request.NameArabic);
            var desc = !string.IsNullOrEmpty(request.Description)
                ? LocalizedString.Create(request.Description, request.DescriptionArabic)
                : null;
            badge.Update(name, desc);
        }

        if (!string.IsNullOrEmpty(request.IconUrl) || !string.IsNullOrEmpty(request.Color))
            badge.SetAppearance(request.IconUrl ?? badge.IconUrl, request.Color ?? badge.Color);

        if (request.Points.HasValue)
            badge.SetPoints(request.Points.Value);

        if (!string.IsNullOrEmpty(request.CriteriaJson))
            badge.SetCriteria(request.CriteriaJson);

        if (request.IsActive.HasValue)
            badge.SetActive(request.IsActive.Value);

        _logger.LogInformation("Updated badge {BadgeId}", id);

        return Ok(MapBadgeDto(badge));
    }

    /// <summary>
    /// Delete a badge definition (soft delete).
    /// </summary>
    [HttpDelete("badges/{id:guid}")]
    [Authorize(Policy = "CanManageUsers")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteBadge(Guid id)
    {
        var badge = _badges.FirstOrDefault(b => b.Id == id);
        if (badge == null) return NotFound();

        badge.SoftDelete(Guid.Empty);
        _logger.LogInformation("Deleted badge {BadgeId}", id);
        return NoContent();
    }

    #endregion

    #region Achievements

    /// <summary>
    /// Award a badge to a user manually.
    /// </summary>
    [HttpPost("achievements/award")]
    [Authorize(Policy = "CanManageUsers")]
    [ProducesResponseType(typeof(AchievementDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<AchievementDto> AwardBadge([FromBody] AwardBadgeRequest request)
    {
        var badge = _badges.FirstOrDefault(b => b.Id == request.BadgeId);
        if (badge == null) return NotFound(new { error = "Badge not found" });

        if (!badge.IsRepeatable)
        {
            var existing = _achievements.FirstOrDefault(
                a => a.BadgeId == request.BadgeId && a.UserId == request.UserId);
            if (existing != null)
                return BadRequest(new { error = "User already has this badge and it is not repeatable" });
        }

        // TODO: Get actual admin user from claims
        var adminUserId = Guid.Empty;
        var adminName = "Admin";

        var achievement = Achievement.Create(
            request.BadgeId,
            request.UserId,
            request.UserName,
            badge.Points,
            AwardMethod.Manual,
            request.Reason,
            adminUserId,
            adminName);

        _achievements.Add(achievement);

        _logger.LogInformation(
            "Badge {BadgeId} awarded to user {UserId} ({UserName})",
            request.BadgeId, request.UserId, request.UserName);

        return Created(
            $"/api/admin/gamification/achievements/{achievement.Id}",
            MapAchievementDto(achievement, badge));
    }

    /// <summary>
    /// List achievements for a specific user.
    /// </summary>
    [HttpGet("achievements/user/{userId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<AchievementDto>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<AchievementDto>> GetUserAchievements(Guid userId)
    {
        var achievements = _achievements
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.EarnedAt)
            .Select(a =>
            {
                var badge = _badges.FirstOrDefault(b => b.Id == a.BadgeId);
                return MapAchievementDto(a, badge);
            })
            .ToList();

        return Ok(achievements);
    }

    /// <summary>
    /// Get the current user's achievements and total points.
    /// </summary>
    [HttpGet("achievements/me")]
    [ProducesResponseType(typeof(UserGamificationProfileDto), StatusCodes.Status200OK)]
    public ActionResult<UserGamificationProfileDto> GetMyProfile()
    {
        // TODO: Get actual user from claims
        var userId = Guid.Empty;

        var userAchievements = _achievements.Where(a => a.UserId == userId).ToList();
        var totalPoints = userAchievements.Sum(a => a.PointsEarned);

        var profile = new UserGamificationProfileDto
        {
            UserId = userId,
            TotalPoints = totalPoints,
            BadgeCount = userAchievements.Count,
            Level = CalculateLevel(totalPoints),
            NextLevelPoints = CalculateNextLevelThreshold(totalPoints),
            Achievements = userAchievements
                .OrderByDescending(a => a.EarnedAt)
                .Select(a =>
                {
                    var badge = _badges.FirstOrDefault(b => b.Id == a.BadgeId);
                    return MapAchievementDto(a, badge);
                })
                .ToList()
        };

        return Ok(profile);
    }

    /// <summary>
    /// Acknowledge (mark as viewed) an achievement.
    /// </summary>
    [HttpPost("achievements/{id:guid}/acknowledge")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult AcknowledgeAchievement(Guid id)
    {
        var achievement = _achievements.FirstOrDefault(a => a.Id == id);
        if (achievement == null) return NotFound();

        achievement.Acknowledge();
        return NoContent();
    }

    #endregion

    #region Leaderboard

    /// <summary>
    /// Get the knowledge sharing leaderboard.
    /// </summary>
    [HttpGet("leaderboard")]
    [ProducesResponseType(typeof(IEnumerable<LeaderboardEntry>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<LeaderboardEntry>> GetLeaderboard(
        [FromQuery] string? period = "all-time",
        [FromQuery] string? department = null,
        [FromQuery] int limit = 20)
    {
        // TODO: Query from repository with actual user statistics
        var leaderboard = _achievements
            .GroupBy(a => new { a.UserId, a.UserName })
            .Select((g, index) => new LeaderboardEntry
            {
                UserId = g.Key.UserId,
                UserName = g.Key.UserName,
                TotalPoints = g.Sum(a => a.PointsEarned),
                BadgeCount = g.Count(),
                Rank = index + 1
            })
            .OrderByDescending(e => e.TotalPoints)
            .Take(limit)
            .ToList();

        // Re-assign ranks after sorting
        for (int i = 0; i < leaderboard.Count; i++)
            leaderboard[i].Rank = i + 1;

        return Ok(leaderboard);
    }

    #endregion

    #region Helpers

    private static int CalculateLevel(int totalPoints) => totalPoints switch
    {
        < 100 => 1,
        < 500 => 2,
        < 1000 => 3,
        < 2500 => 4,
        < 5000 => 5,
        < 10000 => 6,
        _ => 7
    };

    private static int CalculateNextLevelThreshold(int totalPoints) => totalPoints switch
    {
        < 100 => 100,
        < 500 => 500,
        < 1000 => 1000,
        < 2500 => 2500,
        < 5000 => 5000,
        < 10000 => 10000,
        _ => 25000
    };

    private static BadgeDto MapBadgeDto(Badge badge)
    {
        return new BadgeDto
        {
            Id = badge.Id,
            Name = badge.Name.English,
            NameArabic = badge.Name.Arabic,
            Description = badge.Description?.English,
            DescriptionArabic = badge.Description?.Arabic,
            IconUrl = badge.IconUrl,
            Color = badge.Color,
            Tier = badge.Tier.ToString(),
            Category = badge.Category.ToString(),
            Points = badge.Points,
            IsActive = badge.IsActive,
            IsRepeatable = badge.IsRepeatable,
            CriteriaJson = badge.CriteriaJson,
            SortOrder = badge.SortOrder,
            CreatedAt = badge.CreatedAt
        };
    }

    private static AchievementDto MapAchievementDto(Achievement achievement, Badge? badge)
    {
        return new AchievementDto
        {
            Id = achievement.Id,
            BadgeId = achievement.BadgeId,
            BadgeName = badge?.Name.English ?? "Unknown",
            BadgeIconUrl = badge?.IconUrl,
            BadgeColor = badge?.Color,
            BadgeTier = badge?.Tier.ToString(),
            UserId = achievement.UserId,
            UserName = achievement.UserName,
            EarnedAt = achievement.EarnedAt,
            PointsEarned = achievement.PointsEarned,
            AwardMethod = achievement.AwardMethod.ToString(),
            Reason = achievement.Reason,
            AwardedByName = achievement.AwardedByName,
            IsAcknowledged = achievement.IsAcknowledged
        };
    }

    #endregion
}

#region Gamification DTOs

public class BadgeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameArabic { get; set; }
    public string? Description { get; set; }
    public string? DescriptionArabic { get; set; }
    public string IconUrl { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Tier { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Points { get; set; }
    public bool IsActive { get; set; }
    public bool IsRepeatable { get; set; }
    public string? CriteriaJson { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AchievementDto
{
    public Guid Id { get; set; }
    public Guid BadgeId { get; set; }
    public string BadgeName { get; set; } = string.Empty;
    public string? BadgeIconUrl { get; set; }
    public string? BadgeColor { get; set; }
    public string? BadgeTier { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime EarnedAt { get; set; }
    public int PointsEarned { get; set; }
    public string AwardMethod { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string? AwardedByName { get; set; }
    public bool IsAcknowledged { get; set; }
}

public class UserGamificationProfileDto
{
    public Guid UserId { get; set; }
    public int TotalPoints { get; set; }
    public int BadgeCount { get; set; }
    public int Level { get; set; }
    public int NextLevelPoints { get; set; }
    public List<AchievementDto> Achievements { get; set; } = new();
}

public class CreateBadgeRequest
{
    public string Name { get; set; } = string.Empty;
    public string? NameArabic { get; set; }
    public string? Description { get; set; }
    public string? DescriptionArabic { get; set; }
    public string? IconUrl { get; set; }
    public string? Color { get; set; }
    public string? Tier { get; set; }
    public string? Category { get; set; }
    public int Points { get; set; }
    public bool? IsRepeatable { get; set; }
    public string? CriteriaJson { get; set; }
}

public class UpdateBadgeRequest : CreateBadgeRequest
{
    public bool? IsActive { get; set; }
}

public class AwardBadgeRequest
{
    public Guid BadgeId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? Reason { get; set; }
}

#endregion
