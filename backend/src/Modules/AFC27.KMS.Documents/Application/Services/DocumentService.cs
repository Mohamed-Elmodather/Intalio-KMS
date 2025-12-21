using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MassTransit;
using AFC27.KMS.Documents.Application.DTOs;
using AFC27.KMS.Documents.Application.Interfaces;
using AFC27.KMS.Documents.Domain.Entities;
using AFC27.KMS.Infrastructure.Storage;
using AFC27.KMS.Infrastructure.Messaging.Messages;
using AFC27.KMS.Contracts.Common;
using PermissionLevel = AFC27.KMS.Documents.Domain.Entities.PermissionLevel;

namespace AFC27.KMS.Documents.Application.Services;

/// <summary>
/// Service for document operations with permission checking.
/// </summary>
public class DocumentService : IDocumentService
{
    private readonly DbContext _dbContext;
    private readonly IPermissionService _permissionService;
    private readonly IStorageService _storageService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<DocumentService> _logger;

    public DocumentService(
        DbContext dbContext,
        IPermissionService permissionService,
        IStorageService storageService,
        IPublishEndpoint publishEndpoint,
        ILogger<DocumentService> logger)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
        _storageService = storageService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<PagedResult<DocumentSummaryDto>> GetDocumentsAsync(
        DocumentFilterRequest filter,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Get accessible library IDs
        var accessibleLibraryIds = await _permissionService.GetAccessibleLibraryIdsAsync(
            userId, PermissionLevel.Read, cancellationToken);

        var query = _dbContext.Set<Document>()
            .AsNoTracking()
            .Include(d => d.Library)
            .Where(d => !d.IsDeleted && accessibleLibraryIds.Contains(d.LibraryId));

        // Apply filters
        if (filter.LibraryId.HasValue)
        {
            query = query.Where(d => d.LibraryId == filter.LibraryId.Value);
        }

        if (filter.FolderId.HasValue)
        {
            query = query.Where(d => d.FolderId == filter.FolderId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var searchLower = filter.Search.ToLower();
            query = query.Where(d =>
                d.Name.ToLower().Contains(searchLower) ||
                (d.NameArabic != null && d.NameArabic.ToLower().Contains(searchLower)) ||
                d.FileName.ToLower().Contains(searchLower));
        }

        if (!string.IsNullOrWhiteSpace(filter.FileExtension))
        {
            query = query.Where(d => d.FileExtension == filter.FileExtension);
        }

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            if (Enum.TryParse<DocumentStatus>(filter.Status, true, out var status))
            {
                query = query.Where(d => d.Status == status);
            }
        }

        if (filter.CreatedById.HasValue)
        {
            query = query.Where(d => d.CreatedBy == filter.CreatedById.Value);
        }

        if (filter.FromDate.HasValue)
        {
            query = query.Where(d => d.CreatedAt >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(d => d.CreatedAt <= filter.ToDate.Value);
        }

        // Apply sorting
        query = filter.SortBy?.ToLower() switch
        {
            "name" => filter.SortDescending ? query.OrderByDescending(d => d.Name) : query.OrderBy(d => d.Name),
            "createdat" => filter.SortDescending ? query.OrderByDescending(d => d.CreatedAt) : query.OrderBy(d => d.CreatedAt),
            "filesize" => filter.SortDescending ? query.OrderByDescending(d => d.FileSize) : query.OrderBy(d => d.FileSize),
            _ => filter.SortDescending ? query.OrderByDescending(d => d.ModifiedAt ?? d.CreatedAt) : query.OrderBy(d => d.ModifiedAt ?? d.CreatedAt)
        };

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var documents = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(d => new DocumentSummaryDto
            {
                Id = d.Id,
                Name = d.Name,
                NameArabic = d.NameArabic,
                FileName = d.FileName,
                FileExtension = d.FileExtension,
                FileSize = d.FileSize,
                ThumbnailUrl = d.ThumbnailPath,
                Version = d.GetVersionString(),
                Status = d.Status.ToString(),
                IsCheckedOut = d.CheckedOutById.HasValue,
                CheckedOutByName = d.CheckedOutByName,
                CreatedByName = "", // Will be populated from User table in production
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.ModifiedAt
            })
            .ToListAsync(cancellationToken);

        return PagedResult<DocumentSummaryDto>.Create(documents, totalCount, filter.Page, filter.PageSize);
    }

    public async Task<IReadOnlyList<DocumentSummaryDto>> GetRecentDocumentsAsync(
        Guid userId,
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var accessibleLibraryIds = await _permissionService.GetAccessibleLibraryIdsAsync(
            userId, PermissionLevel.Read, cancellationToken);

        return await _dbContext.Set<Document>()
            .AsNoTracking()
            .Where(d => !d.IsDeleted && accessibleLibraryIds.Contains(d.LibraryId))
            .OrderByDescending(d => d.ModifiedAt ?? d.CreatedAt)
            .Take(limit)
            .Select(d => new DocumentSummaryDto
            {
                Id = d.Id,
                Name = d.Name,
                NameArabic = d.NameArabic,
                FileName = d.FileName,
                FileExtension = d.FileExtension,
                FileSize = d.FileSize,
                ThumbnailUrl = d.ThumbnailPath,
                Version = d.GetVersionString(),
                Status = d.Status.ToString(),
                IsCheckedOut = d.CheckedOutById.HasValue,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.ModifiedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<DocumentDto?> GetDocumentAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        // Check permission
        if (!await _permissionService.HasDocumentAccessAsync(documentId, userId, PermissionLevel.Read, cancellationToken))
        {
            _logger.LogWarning("User {UserId} denied access to document {DocumentId}", userId, documentId);
            return null;
        }

        var document = await _dbContext.Set<Document>()
            .AsNoTracking()
            .Include(d => d.Library)
            .Include(d => d.Folder)
            .Include(d => d.Metadata)
            .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted, cancellationToken);

        if (document == null)
            return null;

        // Record view
        await RecordAuditAsync(documentId, "Viewed", userId, "", cancellationToken);

        return new DocumentDto
        {
            Id = document.Id,
            Name = document.Name,
            NameArabic = document.NameArabic,
            Description = document.Description,
            DescriptionArabic = document.DescriptionArabic,
            FileName = document.FileName,
            FileExtension = document.FileExtension,
            ContentType = document.ContentType,
            FileSize = document.FileSize,
            ThumbnailUrl = document.ThumbnailPath,
            LibraryId = document.LibraryId,
            LibraryName = document.Library.Name.English,
            FolderId = document.FolderId,
            FolderPath = document.Folder?.Path,
            Version = document.GetVersionString(),
            Status = document.Status.ToString(),
            CheckedOutById = document.CheckedOutById,
            CheckedOutByName = document.CheckedOutByName,
            CheckedOutAt = document.CheckedOutAt,
            RequiresApproval = document.RequiresApproval,
            DownloadCount = document.DownloadCount,
            ViewCount = document.ViewCount,
            CreatedById = document.CreatedBy ?? Guid.Empty,
            CreatedByName = "",
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.ModifiedAt,
            Metadata = document.Metadata.Select(m => new DocumentMetadataDto
            {
                Key = m.Key,
                Value = m.Value,
                ValueArabic = m.ValueArabic,
                DataType = m.DataType
            }).ToList()
        };
    }

    public async Task<DocumentDto> CreateDocumentAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        long fileSize,
        UploadDocumentRequest request,
        Guid userId,
        string userName,
        CancellationToken cancellationToken = default)
    {
        // Check permission
        if (!await _permissionService.HasLibraryAccessAsync(request.LibraryId, userId, PermissionLevel.Write, cancellationToken))
        {
            throw new UnauthorizedAccessException("User does not have write access to this library");
        }

        // Upload file to storage
        var folder = request.FolderId.HasValue
            ? $"libraries/{request.LibraryId}/folders/{request.FolderId}"
            : $"libraries/{request.LibraryId}";

        var storagePath = await _storageService.UploadFileAsync(
            fileStream, fileName, contentType, folder);

        // Create document entity
        var document = Document.Create(
            request.Name ?? Path.GetFileNameWithoutExtension(fileName),
            fileName,
            contentType,
            fileSize,
            storagePath,
            request.LibraryId,
            request.FolderId);

        if (!string.IsNullOrEmpty(request.NameArabic))
        {
            document.Update(document.Name, request.NameArabic, request.Description, request.DescriptionArabic);
        }

        _dbContext.Set<Document>().Add(document);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Publish events for processing
        await _publishEndpoint.Publish(new DocumentUploadedMessage
        {
            DocumentId = document.Id,
            LibraryId = request.LibraryId,
            FileName = fileName,
            StoragePath = storagePath,
            ContentType = contentType,
            FileSize = fileSize,
            UploadedByUserId = userId,
            UploadedAt = DateTime.UtcNow
        }, cancellationToken);

        await _publishEndpoint.Publish(new ThumbnailGenerationMessage
        {
            EntityId = document.Id,
            EntityType = "Document",
            SourcePath = storagePath,
            ContentType = contentType,
            Priority = 5
        }, cancellationToken);

        // Record audit
        await RecordAuditAsync(document.Id, "Created", userId, userName, cancellationToken);

        _logger.LogInformation("Document {DocumentId} created by user {UserId}", document.Id, userId);

        return await GetDocumentAsync(document.Id, userId, cancellationToken) ?? throw new Exception("Failed to retrieve created document");
    }

    public async Task<bool> UpdateDocumentAsync(
        Guid documentId,
        UpdateDocumentRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _permissionService.HasDocumentAccessAsync(documentId, userId, PermissionLevel.Write, cancellationToken))
        {
            return false;
        }

        var document = await _dbContext.Set<Document>()
            .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted, cancellationToken);

        if (document == null)
            return false;

        document.Update(request.Name, request.NameArabic, request.Description, request.DescriptionArabic);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await RecordAuditAsync(documentId, "Updated", userId, "", cancellationToken);

        _logger.LogInformation("Document {DocumentId} updated by user {UserId}", documentId, userId);
        return true;
    }

    public async Task<bool> DeleteDocumentAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _permissionService.HasDocumentAccessAsync(documentId, userId, PermissionLevel.Delete, cancellationToken))
        {
            return false;
        }

        var document = await _dbContext.Set<Document>()
            .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted, cancellationToken);

        if (document == null)
            return false;

        document.SoftDelete(userId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await RecordAuditAsync(documentId, "Deleted", userId, "", cancellationToken);

        _logger.LogInformation("Document {DocumentId} deleted by user {UserId}", documentId, userId);
        return true;
    }

    public async Task<(Stream Stream, string FileName, string ContentType)?> DownloadDocumentAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _permissionService.HasDocumentAccessAsync(documentId, userId, PermissionLevel.Read, cancellationToken))
        {
            return null;
        }

        var document = await _dbContext.Set<Document>()
            .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted, cancellationToken);

        if (document == null)
            return null;

        var stream = await _storageService.GetFileStreamAsync(document.StoragePath);

        // Increment download count
        document.IncrementDownloadCount();
        await _dbContext.SaveChangesAsync(cancellationToken);

        await RecordAuditAsync(documentId, "Downloaded", userId, "", cancellationToken);

        return (stream, document.FileName, document.ContentType);
    }

    public async Task<bool> CheckOutDocumentAsync(
        Guid documentId,
        Guid userId,
        string userName,
        CancellationToken cancellationToken = default)
    {
        if (!await _permissionService.HasDocumentAccessAsync(documentId, userId, PermissionLevel.Write, cancellationToken))
        {
            return false;
        }

        var document = await _dbContext.Set<Document>()
            .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted, cancellationToken);

        if (document == null || document.CheckedOutById.HasValue)
            return false;

        document.CheckOut(userId, userName);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await RecordAuditAsync(documentId, "Checked Out", userId, userName, cancellationToken);

        _logger.LogInformation("Document {DocumentId} checked out by {UserName}", documentId, userName);
        return true;
    }

    public async Task<bool> CheckInDocumentAsync(
        Guid documentId,
        Stream? fileStream,
        string? fileName,
        long? fileSize,
        CheckInDocumentRequest request,
        Guid userId,
        string userName,
        CancellationToken cancellationToken = default)
    {
        var document = await _dbContext.Set<Document>()
            .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted, cancellationToken);

        if (document == null || document.CheckedOutById != userId)
            return false;

        // If new file provided, upload it
        if (fileStream != null && !string.IsNullOrEmpty(fileName) && fileSize.HasValue)
        {
            // Create version record for current version
            var version = DocumentVersion.Create(
                documentId,
                document.MajorVersion,
                document.MinorVersion,
                document.FileName,
                document.FileSize,
                document.StoragePath,
                userId,
                userName,
                request.ChangeNotes);

            _dbContext.Set<DocumentVersion>().Add(version);

            // Upload new version
            var folder = $"libraries/{document.LibraryId}/documents/{documentId}";
            var storagePath = await _storageService.UploadFileAsync(
                fileStream, fileName, document.ContentType, folder);

            // Update document with new storage path
            // Note: This would require adding a method to Document entity
        }

        document.CheckIn(request.ChangeNotes, request.IsMajorVersion);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var versionStr = document.GetVersionString();
        await RecordAuditAsync(documentId, $"Checked In (v{versionStr})", userId, userName, cancellationToken);

        _logger.LogInformation("Document {DocumentId} checked in as v{Version} by {UserName}", documentId, versionStr, userName);
        return true;
    }

    public async Task<bool> DiscardCheckoutAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var document = await _dbContext.Set<Document>()
            .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted, cancellationToken);

        if (document == null || document.CheckedOutById != userId)
            return false;

        document.DiscardCheckout();
        await _dbContext.SaveChangesAsync(cancellationToken);

        await RecordAuditAsync(documentId, "Checkout Discarded", userId, "", cancellationToken);

        return true;
    }

    public async Task<IReadOnlyList<DocumentVersionDto>> GetVersionsAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _permissionService.HasDocumentAccessAsync(documentId, userId, PermissionLevel.Read, cancellationToken))
        {
            return Array.Empty<DocumentVersionDto>();
        }

        return await _dbContext.Set<DocumentVersion>()
            .AsNoTracking()
            .Where(v => v.DocumentId == documentId)
            .OrderByDescending(v => v.MajorVersion)
            .ThenByDescending(v => v.MinorVersion)
            .Select(v => new DocumentVersionDto
            {
                Id = v.Id,
                Version = v.GetVersionString(),
                FileName = v.FileName,
                FileSize = v.FileSize,
                CreatedByName = v.CreatedByName,
                CreatedAt = v.CreatedAt,
                ChangeNotes = v.ChangeNotes
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> RestoreVersionAsync(
        Guid documentId,
        Guid versionId,
        Guid userId,
        string userName,
        CancellationToken cancellationToken = default)
    {
        if (!await _permissionService.HasDocumentAccessAsync(documentId, userId, PermissionLevel.Write, cancellationToken))
        {
            return false;
        }

        var document = await _dbContext.Set<Document>()
            .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted, cancellationToken);

        var version = await _dbContext.Set<DocumentVersion>()
            .FirstOrDefaultAsync(v => v.Id == versionId && v.DocumentId == documentId, cancellationToken);

        if (document == null || version == null)
            return false;

        // Create backup of current version
        var currentVersion = DocumentVersion.Create(
            documentId,
            document.MajorVersion,
            document.MinorVersion,
            document.FileName,
            document.FileSize,
            document.StoragePath,
            userId,
            userName,
            $"Backup before restore to v{version.GetVersionString()}");

        _dbContext.Set<DocumentVersion>().Add(currentVersion);

        await _dbContext.SaveChangesAsync(cancellationToken);

        await RecordAuditAsync(documentId, $"Restored to v{version.GetVersionString()}", userId, userName, cancellationToken);

        return true;
    }

    public async Task<IReadOnlyList<DocumentAuditDto>> GetAuditTrailAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _permissionService.HasDocumentAccessAsync(documentId, userId, PermissionLevel.Read, cancellationToken))
        {
            return Array.Empty<DocumentAuditDto>();
        }

        return await _dbContext.Set<DocumentAudit>()
            .AsNoTracking()
            .Where(a => a.DocumentId == documentId)
            .OrderByDescending(a => a.PerformedAt)
            .Take(100)
            .Select(a => new DocumentAuditDto
            {
                Id = a.Id,
                Action = a.Action,
                Details = a.Details,
                PerformedByName = a.PerformedByName,
                PerformedAt = a.PerformedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> MoveDocumentAsync(
        Guid documentId,
        Guid? targetFolderId,
        Guid? targetLibraryId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _permissionService.HasDocumentAccessAsync(documentId, userId, PermissionLevel.Write, cancellationToken))
        {
            return false;
        }

        var document = await _dbContext.Set<Document>()
            .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted, cancellationToken);

        if (document == null)
            return false;

        // Check target permissions
        var targetLibrary = targetLibraryId ?? document.LibraryId;
        if (!await _permissionService.HasLibraryAccessAsync(targetLibrary, userId, PermissionLevel.Write, cancellationToken))
        {
            return false;
        }

        document.MoveTo(targetFolderId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await RecordAuditAsync(documentId, "Moved", userId, "", cancellationToken);

        return true;
    }

    public async Task<DocumentDto?> CopyDocumentAsync(
        Guid documentId,
        Guid? targetFolderId,
        Guid? targetLibraryId,
        Guid userId,
        string userName,
        CancellationToken cancellationToken = default)
    {
        var sourceDoc = await GetDocumentAsync(documentId, userId, cancellationToken);
        if (sourceDoc == null)
            return null;

        var targetLibrary = targetLibraryId ?? sourceDoc.LibraryId;
        if (!await _permissionService.HasLibraryAccessAsync(targetLibrary, userId, PermissionLevel.Write, cancellationToken))
        {
            return null;
        }

        // Copy file in storage
        var document = await _dbContext.Set<Document>()
            .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted, cancellationToken);

        if (document == null)
            return null;

        var newStoragePath = await _storageService.CopyFileAsync(
            document.StoragePath,
            $"libraries/{targetLibrary}/documents/{Guid.NewGuid()}/{document.FileName}");

        // Create new document
        var newDocument = Document.Create(
            $"{document.Name} (Copy)",
            document.FileName,
            document.ContentType,
            document.FileSize,
            newStoragePath,
            targetLibrary,
            targetFolderId);

        _dbContext.Set<Document>().Add(newDocument);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await RecordAuditAsync(newDocument.Id, $"Copied from {documentId}", userId, userName, cancellationToken);

        return await GetDocumentAsync(newDocument.Id, userId, cancellationToken);
    }

    public async Task<bool> PublishDocumentAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _permissionService.HasDocumentAccessAsync(documentId, userId, PermissionLevel.Manage, cancellationToken))
        {
            return false;
        }

        var document = await _dbContext.Set<Document>()
            .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted, cancellationToken);

        if (document == null)
            return false;

        document.Publish();
        await _dbContext.SaveChangesAsync(cancellationToken);

        await RecordAuditAsync(documentId, "Published", userId, "", cancellationToken);

        // Trigger AI ingestion
        await _publishEndpoint.Publish(new AIIngestionRequestMessage
        {
            EntityId = documentId,
            EntityType = "Document",
            StoragePath = document.StoragePath,
            ContentType = document.ContentType,
            Title = document.Name,
            ContainerId = document.LibraryId,
            RequestedByUserId = userId,
            RequestedAt = DateTime.UtcNow
        }, cancellationToken);

        return true;
    }

    public async Task<bool> ArchiveDocumentAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _permissionService.HasDocumentAccessAsync(documentId, userId, PermissionLevel.Write, cancellationToken))
        {
            return false;
        }

        var document = await _dbContext.Set<Document>()
            .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted, cancellationToken);

        if (document == null)
            return false;

        document.Archive();
        await _dbContext.SaveChangesAsync(cancellationToken);

        await RecordAuditAsync(documentId, "Archived", userId, "", cancellationToken);

        return true;
    }

    public async Task<string?> GetPreviewUrlAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _permissionService.HasDocumentAccessAsync(documentId, userId, PermissionLevel.Read, cancellationToken))
        {
            return null;
        }

        var document = await _dbContext.Set<Document>()
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted, cancellationToken);

        if (document == null)
            return null;

        return await _storageService.GenerateSecurePreviewUrlAsync(
            document.StoragePath,
            TimeSpan.FromHours(1));
    }

    private async Task RecordAuditAsync(
        Guid documentId,
        string action,
        Guid userId,
        string userName,
        CancellationToken cancellationToken)
    {
        var audit = DocumentAudit.Create(
            documentId,
            action,
            userId,
            userName);

        _dbContext.Set<DocumentAudit>().Add(audit);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResult<DocumentSummaryDto>> SearchDocumentsAsync(
        Guid userId,
        Guid? libraryId,
        Guid? folderId,
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var filter = new DocumentFilterRequest
        {
            LibraryId = libraryId,
            FolderId = folderId,
            Search = searchTerm,
            Page = page,
            PageSize = pageSize
        };

        return await GetDocumentsAsync(filter, userId, cancellationToken);
    }

    public async Task<DocumentDto> UploadDocumentAsync(
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
        CancellationToken cancellationToken = default)
    {
        var request = new UploadDocumentRequest
        {
            LibraryId = libraryId,
            FolderId = folderId,
            Name = name,
            NameArabic = nameArabic,
            Description = description,
            DescriptionArabic = descriptionArabic,
            Tags = tags ?? Array.Empty<string>()
        };

        return await CreateDocumentAsync(
            fileStream, fileName, contentType, fileStream.Length,
            request, userId, userName, cancellationToken);
    }

    public async Task<bool> UpdateDocumentAsync(
        Guid documentId,
        string name,
        string? nameArabic,
        string? description,
        string? descriptionArabic,
        string[]? tags,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var request = new UpdateDocumentRequest
        {
            Name = name,
            NameArabic = nameArabic,
            Description = description,
            DescriptionArabic = descriptionArabic,
            Tags = tags ?? Array.Empty<string>()
        };

        return await UpdateDocumentAsync(documentId, request, userId, cancellationToken);
    }

    public async Task LogDownloadAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await RecordAuditAsync(documentId, "Downloaded", userId, "", cancellationToken);
    }

    public async Task LogViewAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await RecordAuditAsync(documentId, "Viewed", userId, "", cancellationToken);
    }

    public async Task<IReadOnlyList<DocumentVersionDto>?> GetDocumentVersionsAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _permissionService.HasDocumentAccessAsync(documentId, userId, PermissionLevel.Read, cancellationToken))
        {
            return null;
        }

        return await GetVersionsAsync(documentId, userId, cancellationToken);
    }

    public async Task<(Stream Stream, string ContentType, string FileName)?> DownloadVersionAsync(
        Guid documentId,
        Guid versionId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _permissionService.HasDocumentAccessAsync(documentId, userId, PermissionLevel.Read, cancellationToken))
        {
            return null;
        }

        var version = await _dbContext.Set<DocumentVersion>()
            .FirstOrDefaultAsync(v => v.Id == versionId && v.DocumentId == documentId, cancellationToken);

        if (version == null)
            return null;

        var stream = await _storageService.GetFileStreamAsync(version.StoragePath);
        return (stream, "application/octet-stream", version.FileName);
    }

    public async Task<bool> CheckInDocumentAsync(
        Guid documentId,
        Stream? fileStream,
        string? fileName,
        string? contentType,
        bool isMajorVersion,
        string? comments,
        Guid userId,
        string userName,
        CancellationToken cancellationToken = default)
    {
        var request = new CheckInDocumentRequest
        {
            IsMajorVersion = isMajorVersion,
            ChangeNotes = comments,
            Comments = comments
        };

        return await CheckInDocumentAsync(
            documentId, fileStream, fileName,
            fileStream?.Length, request, userId, userName, cancellationToken);
    }

    public async Task<bool> MoveDocumentAsync(
        Guid documentId,
        Guid? targetFolderId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await MoveDocumentAsync(documentId, targetFolderId, null, userId, cancellationToken);
    }

    public async Task<DocumentDto?> CopyDocumentAsync(
        Guid documentId,
        Guid? targetFolderId,
        Guid userId,
        string userName,
        CancellationToken cancellationToken = default)
    {
        return await CopyDocumentAsync(documentId, targetFolderId, null, userId, userName, cancellationToken);
    }
}
