using AFC27.KMS.Documents.Application.DTOs;
using AFC27.KMS.Contracts.Common;

namespace AFC27.KMS.Documents.Application.Interfaces;

/// <summary>
/// Service interface for document operations.
/// </summary>
public interface IDocumentService
{
    /// <summary>
    /// Gets a paginated list of documents based on filter criteria.
    /// </summary>
    Task<PagedResult<DocumentSummaryDto>> GetDocumentsAsync(
        DocumentFilterRequest filter,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recent documents for a user.
    /// </summary>
    Task<IReadOnlyList<DocumentSummaryDto>> GetRecentDocumentsAsync(
        Guid userId,
        int limit = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a document by ID with permission check.
    /// </summary>
    Task<DocumentDto?> GetDocumentAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new document from an uploaded file.
    /// </summary>
    Task<DocumentDto> CreateDocumentAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        long fileSize,
        UploadDocumentRequest request,
        Guid userId,
        string userName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a document (soft delete).
    /// </summary>
    Task<bool> DeleteDocumentAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a document's file stream for download.
    /// </summary>
    Task<(Stream Stream, string FileName, string ContentType)?> DownloadDocumentAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks out a document for editing.
    /// </summary>
    Task<bool> CheckOutDocumentAsync(
        Guid documentId,
        Guid userId,
        string userName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Discards a checkout without creating a new version.
    /// </summary>
    Task<bool> DiscardCheckoutAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all versions of a document.
    /// </summary>
    Task<IReadOnlyList<DocumentVersionDto>> GetVersionsAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores a specific version as the current version.
    /// </summary>
    Task<bool> RestoreVersionAsync(
        Guid documentId,
        Guid versionId,
        Guid userId,
        string userName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the audit trail for a document.
    /// </summary>
    Task<IReadOnlyList<DocumentAuditDto>> GetAuditTrailAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a document.
    /// </summary>
    Task<bool> PublishDocumentAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Archives a document.
    /// </summary>
    Task<bool> ArchiveDocumentAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a secure preview URL for a document.
    /// </summary>
    Task<string?> GetPreviewUrlAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Search documents with pagination.
    /// </summary>
    Task<PagedResult<DocumentSummaryDto>> SearchDocumentsAsync(
        Guid userId,
        Guid? libraryId,
        Guid? folderId,
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Upload a document.
    /// </summary>
    Task<DocumentDto> UploadDocumentAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        Guid libraryId,
        Guid? folderId,
        string name,
        string? nameArabic,
        string? description,
        string? descriptionArabic,
        string[]? tags,
        Guid userId,
        string userName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update document metadata.
    /// </summary>
    Task<bool> UpdateDocumentAsync(
        Guid documentId,
        string name,
        string? nameArabic,
        string? description,
        string? descriptionArabic,
        string[]? tags,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Log a document download.
    /// </summary>
    Task LogDownloadAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Log a document view.
    /// </summary>
    Task LogViewAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get document versions.
    /// </summary>
    Task<IReadOnlyList<DocumentVersionDto>?> GetDocumentVersionsAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Download a specific document version.
    /// </summary>
    Task<(Stream Stream, string ContentType, string FileName)?> DownloadVersionAsync(
        Guid documentId,
        Guid versionId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check in a document with optional new file.
    /// </summary>
    Task<bool> CheckInDocumentAsync(
        Guid documentId,
        Stream? fileStream,
        string? fileName,
        string? contentType,
        bool isMajorVersion,
        string? comments,
        Guid userId,
        string userName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Move a document to another folder.
    /// </summary>
    Task<bool> MoveDocumentAsync(
        Guid documentId,
        Guid? targetFolderId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Copy a document to another folder.
    /// </summary>
    Task<DocumentDto?> CopyDocumentAsync(
        Guid documentId,
        Guid? targetFolderId,
        Guid userId,
        string userName,
        CancellationToken cancellationToken = default);
}
