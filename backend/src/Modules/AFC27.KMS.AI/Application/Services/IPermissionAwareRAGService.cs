using AFC27.KMS.AI.Application.DTOs;

namespace AFC27.KMS.AI.Application.Services;

/// <summary>
/// Permission-aware RAG service that filters retrieval results by the
/// current user's access level before passing context to the LLM.
/// </summary>
public interface IPermissionAwareRAGService
{
    /// <summary>
    /// Query documents with RAG, filtering results by user permissions.
    /// After retrieving candidate chunks, each source entity's accessibility
    /// is verified before inclusion in the LLM context.
    /// </summary>
    Task<RAGResponse> QueryWithPermissionsAsync(
        RAGRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream a permission-filtered RAG response.
    /// </summary>
    IAsyncEnumerable<RAGStreamChunk> QueryWithPermissionsStreamAsync(
        RAGRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check whether the current user has access to a specific entity
    /// (Article, Document, Space content, etc.).
    /// </summary>
    Task<bool> HasAccessToEntityAsync(
        Guid entityId,
        string entityType,
        CancellationToken cancellationToken = default);
}
