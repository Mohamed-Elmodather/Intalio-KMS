using AFC27.KMS.AI.Application.DTOs;

namespace AFC27.KMS.AI.Application.Services;

/// <summary>
/// Service interface for article and content block translation.
/// Provides bulk translation of articles, individual content blocks, and arbitrary text.
/// </summary>
public interface ITranslationService
{
    /// <summary>
    /// Translate all blocks of an article between EN and AR.
    /// </summary>
    /// <param name="articleId">The article ID to translate.</param>
    /// <param name="request">Translation parameters.</param>
    /// <param name="userId">The requesting user's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Translation result with translated blocks.</returns>
    Task<TranslationResponse> TranslateArticleAsync(
        Guid articleId,
        TranslateArticleRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Translate a set of content blocks.
    /// </summary>
    /// <param name="request">The blocks and translation parameters.</param>
    /// <param name="userId">The requesting user's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Translation result with translated blocks.</returns>
    Task<TranslationResponse> TranslateBlocksAsync(
        TranslateBlocksRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Translate arbitrary text.
    /// </summary>
    /// <param name="request">The text and translation parameters.</param>
    /// <param name="userId">The requesting user's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Translation result with translated text.</returns>
    Task<TranslationResponse> TranslateTextAsync(
        TranslateTextRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);
}
