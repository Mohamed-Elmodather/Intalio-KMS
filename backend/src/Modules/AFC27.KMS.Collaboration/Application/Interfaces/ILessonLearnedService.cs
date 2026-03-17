using AFC27.KMS.Collaboration.Application.DTOs;

namespace AFC27.KMS.Collaboration.Application.Interfaces;

public interface ILessonLearnedService
{
    Task<PagedResult<LessonLearnedSummaryDto>> GetLessonsAsync(
        string? search, string? category, string? impact, string? status,
        Guid? projectId, int page, int pageSize, CancellationToken ct = default);

    Task<LessonLearnedDto?> GetLessonByIdAsync(Guid id, Guid userId, CancellationToken ct = default);

    Task<LessonLearnedDto> CreateLessonAsync(CreateLessonLearnedRequest request, Guid userId, string userName, CancellationToken ct = default);

    Task<bool> UpdateLessonAsync(Guid id, CreateLessonLearnedRequest request, Guid userId, CancellationToken ct = default);

    Task<bool> DeleteLessonAsync(Guid id, Guid userId, CancellationToken ct = default);

    Task<bool> SubmitForReviewAsync(Guid id, Guid userId, CancellationToken ct = default);

    Task<bool> ApproveAsync(Guid id, Guid userId, CancellationToken ct = default);

    Task<bool> RejectAsync(Guid id, string reason, Guid userId, CancellationToken ct = default);

    Task<bool> PublishAsync(Guid id, Guid userId, CancellationToken ct = default);

    Task<bool> MarkActionsPendingAsync(Guid id, Guid userId, CancellationToken ct = default);

    Task<bool> MarkActionsCompleteAsync(Guid id, Guid userId, CancellationToken ct = default);

    Task<bool> VerifyLessonAsync(Guid id, Guid userId, CancellationToken ct = default);

    Task<bool> ArchiveAsync(Guid id, Guid userId, CancellationToken ct = default);

    Task<bool> MarkAsUsefulAsync(Guid id, Guid userId, CancellationToken ct = default);

    Task<bool> UnmarkAsUsefulAsync(Guid id, Guid userId, CancellationToken ct = default);

    Task<List<LessonLearnedSummaryDto>> GetRelatedLessonsAsync(Guid id, int limit, CancellationToken ct = default);

    Task<bool> AssignProcessOwnerAsync(Guid id, AssignProcessOwnerRequest request, Guid userId, CancellationToken ct = default);

    Task<bool> SetRootCauseAsync(Guid id, SetRootCauseRequest request, Guid userId, CancellationToken ct = default);

    Task<LessonsAnalyticsOverviewDto> GetAnalyticsOverviewAsync(DateTime? from, DateTime? to, CancellationToken ct = default);
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
