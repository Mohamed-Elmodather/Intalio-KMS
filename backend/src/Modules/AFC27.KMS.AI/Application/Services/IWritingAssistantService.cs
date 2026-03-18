using AFC27.KMS.AI.Application.DTOs;

namespace AFC27.KMS.AI.Application.Services;

/// <summary>
/// Service interface for inline AI writing assistance.
/// Provides text improvement, generation, translation, tone-change, and streaming capabilities.
/// </summary>
public interface IWritingAssistantService
{
    /// <summary>
    /// Suggest improvements for the given text (grammar, clarity, style).
    /// </summary>
    Task<AIWritingResponse> SuggestAsync(
        AIWritingRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate content from a prompt.
    /// </summary>
    Task<AIWritingResponse> GenerateAsync(
        AIGenerateRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Improve text grammar, clarity, and structure.
    /// </summary>
    Task<AIWritingResponse> ImproveAsync(
        AIWritingRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Change the tone of text (formal, casual, simplified, technical).
    /// </summary>
    Task<AIWritingResponse> ChangeToneAsync(
        AIToneChangeRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Translate text between languages (EN/AR).
    /// </summary>
    Task<AIWritingResponse> TranslateAsync(
        AITranslateRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Summarize text into a shorter version.
    /// </summary>
    Task<AIWritingResponse> SummarizeAsync(
        AISummarizeRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Continue writing from the given text.
    /// </summary>
    Task<AIWritingResponse> ContinueAsync(
        AIContinueRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream a writing operation result via async enumerable.
    /// </summary>
    IAsyncEnumerable<AIStreamChunk> StreamAsync(
        AIStreamWritingRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);
}
