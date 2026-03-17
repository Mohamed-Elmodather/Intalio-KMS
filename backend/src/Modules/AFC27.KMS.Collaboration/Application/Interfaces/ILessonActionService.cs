using AFC27.KMS.Collaboration.Application.DTOs;

namespace AFC27.KMS.Collaboration.Application.Interfaces;

public interface ILessonActionService
{
    Task<List<LessonActionDto>> GetActionsAsync(Guid lessonId, CancellationToken ct = default);

    Task<LessonActionDto> CreateActionAsync(Guid lessonId, CreateLessonActionRequest request, Guid userId, CancellationToken ct = default);

    Task<bool> UpdateActionAsync(Guid lessonId, Guid actionId, UpdateLessonActionRequest request, Guid userId, CancellationToken ct = default);

    Task<bool> StartActionAsync(Guid lessonId, Guid actionId, Guid userId, CancellationToken ct = default);

    Task<bool> CompleteActionAsync(Guid lessonId, Guid actionId, CompleteActionRequest request, Guid userId, CancellationToken ct = default);

    Task<bool> VerifyActionAsync(Guid lessonId, Guid actionId, VerifyActionRequest request, Guid userId, string verifierName, CancellationToken ct = default);

    Task<bool> CancelActionAsync(Guid lessonId, Guid actionId, Guid userId, CancellationToken ct = default);

    Task<bool> DelegateActionAsync(Guid lessonId, Guid actionId, DelegateActionRequest request, Guid userId, CancellationToken ct = default);

    Task<bool> LinkDocumentAsync(Guid lessonId, Guid actionId, LinkDocumentRequest request, Guid userId, CancellationToken ct = default);
}
