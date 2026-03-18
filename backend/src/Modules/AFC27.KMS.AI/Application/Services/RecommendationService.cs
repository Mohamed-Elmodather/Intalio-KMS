using AFC27.KMS.AI.Domain.Entities;

namespace AFC27.KMS.AI.Application.Services;

/// <summary>
/// Service interface for proactive recommendation surfacing (Phase 8C).
/// Computes personalized suggestions based on user activity, leveraging the existing SmartSuggestion entity.
/// </summary>
public interface IRecommendationService
{
    /// <summary>
    /// Get personalized content suggestions for the current user based on their activity,
    /// reading history, department, role, and interests.
    /// </summary>
    Task<PersonalizedSuggestionsDto> GetSuggestionsForUserAsync(
        Guid userId, SuggestionFilterRequest? filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get related content recommendations for a specific entity (e.g. sidebar on article detail).
    /// </summary>
    Task<IReadOnlyList<RecommendationDto>> GetRelatedContentAsync(
        string entityType, Guid entityId, int maxResults = 5, CancellationToken cancellationToken = default);

    /// <summary>
    /// Record that a user accepted/dismissed a suggestion for feedback loop.
    /// </summary>
    Task RecordSuggestionFeedbackAsync(
        Guid suggestionId, bool wasAccepted, CancellationToken cancellationToken = default);

    /// <summary>
    /// Trigger a background recomputation of suggestions for a user (e.g. after significant activity).
    /// </summary>
    Task RefreshSuggestionsAsync(Guid userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of the recommendation service.
/// Uses a combination of collaborative filtering, content-based filtering, and activity signals.
/// </summary>
public class RecommendationService : IRecommendationService
{
    private readonly ILogger<RecommendationService> _logger;

    public RecommendationService(ILogger<RecommendationService> logger)
    {
        _logger = logger;
    }

    public async Task<PersonalizedSuggestionsDto> GetSuggestionsForUserAsync(
        Guid userId, SuggestionFilterRequest? filter = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Computing personalized suggestions for user {UserId}", userId);

        await Task.Delay(50, cancellationToken);

        // In a real implementation, this would:
        // 1. Fetch user activity (recently viewed, authored, bookmarked content)
        // 2. Fetch user profile (department, role, skills, interests)
        // 3. Query SmartSuggestion table for pre-computed suggestions
        // 4. Score and rank using collaborative filtering + content similarity
        // 5. Filter out already-read/dismissed content

        var suggestions = new PersonalizedSuggestionsDto
        {
            UserId = userId,
            GeneratedAt = DateTime.UtcNow,
            RecommendedForYou = new List<RecommendationDto>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    EntityType = "Article",
                    EntityId = Guid.NewGuid(),
                    Title = "Security Protocols for Large-Scale Events",
                    TitleArabic = "بروتوكولات الأمن للفعاليات الكبرى",
                    Snippet = "Comprehensive overview of security measures for stadium events with 50k+ attendees...",
                    ThumbnailUrl = "/images/articles/security-protocols.jpg",
                    AuthorName = "Sara Ali",
                    RelevanceScore = 0.95,
                    Reasoning = "Based on your recent reading about venue management and your role in operations.",
                    SuggestionType = SuggestionType.RelatedContent,
                    Tags = new List<string> { "Security", "Venues", "Operations" },
                    CreatedAt = DateTime.UtcNow.AddHours(-6)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    EntityType = "Document",
                    EntityId = Guid.NewGuid(),
                    Title = "Emergency Response Plan Template",
                    Snippet = "Template for creating venue-specific emergency response plans...",
                    AuthorName = "Ahmed Hassan",
                    RelevanceScore = 0.88,
                    Reasoning = "Documents from your department that you haven't viewed yet.",
                    SuggestionType = SuggestionType.SimilarDocument,
                    Tags = new List<string> { "Emergency", "Template" },
                    CreatedAt = DateTime.UtcNow.AddHours(-4)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    EntityType = "LessonLearned",
                    EntityId = Guid.NewGuid(),
                    Title = "Vendor coordination delays during test event",
                    Snippet = "Key lessons from the test event about managing multiple vendor timelines...",
                    AuthorName = "Mohammed Al-Rashid",
                    RelevanceScore = 0.82,
                    Reasoning = "High-impact lesson from your project area.",
                    SuggestionType = SuggestionType.RelatedContent,
                    Tags = new List<string> { "Vendor Management", "Lessons" },
                    CreatedAt = DateTime.UtcNow.AddHours(-2)
                }
            },
            TrendingInYourDepartment = new List<RecommendationDto>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    EntityType = "Article",
                    EntityId = Guid.NewGuid(),
                    Title = "New Ticketing System Integration Guide",
                    Snippet = "Step-by-step guide for the new ticketing platform integration...",
                    AuthorName = "Technology Team",
                    RelevanceScore = 0.90,
                    Reasoning = "Trending content in the Technology department this week.",
                    SuggestionType = SuggestionType.RelatedContent,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                }
            },
            RecommendedExperts = new List<RecommendedExpertDto>
            {
                new()
                {
                    UserId = Guid.NewGuid(),
                    DisplayName = "Sara Ali",
                    DisplayNameArabic = "سارة علي",
                    AvatarUrl = "/avatars/sara.jpg",
                    JobTitle = "Project Manager",
                    Department = "Operations",
                    ExpertiseAreas = new List<string> { "Venue Management", "Security", "Event Planning" },
                    RelevanceScore = 0.92,
                    Reasoning = "Expert in venue management based on authored content and activity."
                }
            },
            RecentlyPopular = new List<RecommendationDto>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    EntityType = "Discussion",
                    EntityId = Guid.NewGuid(),
                    Title = "How are we handling accessibility requirements?",
                    Snippet = "Active discussion about accessibility standards for all venues...",
                    AuthorName = "Ahmed Hassan",
                    RelevanceScore = 0.75,
                    Reasoning = "Trending discussion with high engagement this week.",
                    SuggestionType = SuggestionType.RelatedContent,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                }
            }
        };

        return suggestions;
    }

    public async Task<IReadOnlyList<RecommendationDto>> GetRelatedContentAsync(
        string entityType, Guid entityId, int maxResults = 5, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Computing related content for {EntityType} {EntityId}", entityType, entityId);

        await Task.Delay(50, cancellationToken);

        // In a real implementation, this would use vector similarity search
        // on the entity's embedding to find semantically related content.

        var related = new List<RecommendationDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                EntityType = "Article",
                EntityId = Guid.NewGuid(),
                Title = "Stadium Infrastructure Update",
                Snippet = "Latest updates on construction progress for tournament venues...",
                AuthorName = "Mohammed Al-Rashid",
                RelevanceScore = 0.91,
                Reasoning = "Semantically similar content based on topic and tags.",
                SuggestionType = SuggestionType.RelatedContent,
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                EntityType = "Document",
                EntityId = Guid.NewGuid(),
                Title = "Venue Assessment Checklist",
                Snippet = "Standardized checklist for evaluating venue readiness...",
                AuthorName = "Sara Ali",
                RelevanceScore = 0.85,
                Reasoning = "Frequently viewed together with this content.",
                SuggestionType = SuggestionType.SimilarDocument,
                CreatedAt = DateTime.UtcNow.AddDays(-8)
            }
        };

        return related.Take(maxResults).ToList();
    }

    public async Task RecordSuggestionFeedbackAsync(
        Guid suggestionId, bool wasAccepted, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Recording suggestion feedback: {SuggestionId} was {Action}",
            suggestionId, wasAccepted ? "accepted" : "dismissed");

        await Task.Delay(20, cancellationToken);

        // In a real implementation:
        // 1. Update SmartSuggestion.WasAccepted / WasDismissed
        // 2. Feed this signal back into the recommendation model
    }

    public async Task RefreshSuggestionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Triggering suggestion refresh for user {UserId}", userId);

        await Task.Delay(20, cancellationToken);

        // In a real implementation, this would enqueue a background job
        // to recompute SmartSuggestion rows for the user.
    }
}

// ========================================
// Phase 8C: Recommendation DTOs
// ========================================

/// <summary>
/// Container for all personalized suggestions for a user.
/// </summary>
public record PersonalizedSuggestionsDto
{
    public Guid UserId { get; init; }
    public DateTime GeneratedAt { get; init; }
    public IReadOnlyList<RecommendationDto> RecommendedForYou { get; init; } = Array.Empty<RecommendationDto>();
    public IReadOnlyList<RecommendationDto> TrendingInYourDepartment { get; init; } = Array.Empty<RecommendationDto>();
    public IReadOnlyList<RecommendedExpertDto> RecommendedExperts { get; init; } = Array.Empty<RecommendedExpertDto>();
    public IReadOnlyList<RecommendationDto> RecentlyPopular { get; init; } = Array.Empty<RecommendationDto>();
}

/// <summary>
/// A single content recommendation.
/// </summary>
public record RecommendationDto
{
    public Guid Id { get; init; }
    public string EntityType { get; init; } = string.Empty;
    public Guid EntityId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? TitleArabic { get; init; }
    public string? Snippet { get; init; }
    public string? ThumbnailUrl { get; init; }
    public string? AuthorName { get; init; }
    public double RelevanceScore { get; init; }
    public string? Reasoning { get; init; }
    public SuggestionType SuggestionType { get; init; }
    public IReadOnlyList<string>? Tags { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// A recommended expert/colleague.
/// </summary>
public record RecommendedExpertDto
{
    public Guid UserId { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string? DisplayNameArabic { get; init; }
    public string? AvatarUrl { get; init; }
    public string? JobTitle { get; init; }
    public string? Department { get; init; }
    public IReadOnlyList<string> ExpertiseAreas { get; init; } = Array.Empty<string>();
    public double RelevanceScore { get; init; }
    public string? Reasoning { get; init; }
}

/// <summary>
/// Filter options for suggestion requests.
/// </summary>
public record SuggestionFilterRequest
{
    public string? EntityType { get; init; }
    public SuggestionType? SuggestionType { get; init; }
    public int MaxResults { get; init; } = 10;
    public bool IncludeDismissed { get; init; }
}
