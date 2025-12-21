using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Admin.Domain.Entities;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Application.Interfaces;
using AFC27.KMS.SharedKernel.Interfaces;

using AFC27.KMS.Documents.Domain.Entities;

namespace AFC27.KMS.Admin.Application.Services;

/// <summary>
/// Service for document quarantine management.
/// </summary>
public class QuarantineService : IQuarantineService
{
    private readonly DbContext _dbContext;
    private readonly ICurrentUser _currentUser;
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<QuarantineService> _logger;

    public QuarantineService(
        DbContext dbContext,
        ICurrentUser currentUser,
        IAuditLogService auditLogService,
        ILogger<QuarantineService> logger)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _auditLogService = auditLogService;
        _logger = logger;
    }

    public async Task<QuarantinedDocumentDto> QuarantineAsync(
        QuarantineDocumentRequest request,
        CancellationToken cancellationToken = default)
    {
        var document = await _dbContext.Set<Document>()
            .FirstOrDefaultAsync(d => d.Id == request.DocumentId, cancellationToken);

        if (document == null)
            throw new InvalidOperationException($"Document {request.DocumentId} not found");

        var quarantined = QuarantinedDocument.Create(
            request.DocumentId,
            document.FileName,
            document.StoragePath,
            request.Reason,
            request.ReasonDetails,
            _currentUser.UserId ?? Guid.Empty,
            _currentUser.DisplayName ?? "System");

        _dbContext.Set<QuarantinedDocument>().Add(quarantined);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogActionAsync(
            AuditActions.DocumentQuarantined,
            AuditCategories.SecurityEvent,
            "Document",
            request.DocumentId,
            additionalData: new { Reason = request.Reason.ToString(), request.ReasonDetails },
            severity: AuditSeverity.Warning,
            cancellationToken: cancellationToken);

        _logger.LogWarning(
            "Document {DocumentId} quarantined: {Reason}",
            request.DocumentId, request.Reason);

        return MapToDto(quarantined);
    }

    public async Task<QuarantinedDocumentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var quarantined = await _dbContext.Set<QuarantinedDocument>()
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);

        return quarantined == null ? null : MapToDto(quarantined);
    }

    public async Task<PagedResult<QuarantinedDocumentDto>> ListAsync(
        QuarantineListRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<QuarantinedDocument>().AsNoTracking();

        if (request.Status.HasValue)
            query = query.Where(q => q.Status == request.Status.Value);

        if (request.Reason.HasValue)
            query = query.Where(q => q.Reason == request.Reason.Value);

        if (request.From.HasValue)
            query = query.Where(q => q.QuarantinedAt >= request.From.Value);

        if (request.To.HasValue)
            query = query.Where(q => q.QuarantinedAt <= request.To.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(q => q.QuarantinedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(q => MapToDto(q))
            .ToListAsync(cancellationToken);

        return new PagedResult<QuarantinedDocumentDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<bool> ApproveAsync(
        Guid id,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        var quarantined = await _dbContext.Set<QuarantinedDocument>()
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);

        if (quarantined == null || quarantined.Status != QuarantineStatus.Pending)
            return false;

        quarantined.Approve(_currentUser.UserId ?? Guid.Empty, _currentUser.DisplayName ?? "System", notes);

        // Mark the original document as deleted
        var document = await _dbContext.Set<Document>()
            .FirstOrDefaultAsync(d => d.Id == quarantined.DocumentId, cancellationToken);

        if (document != null)
        {
            document.Delete();
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogActionAsync(
            "QuarantineApproved",
            AuditCategories.SecurityEvent,
            "QuarantinedDocument",
            id,
            additionalData: new { DocumentId = quarantined.DocumentId, Notes = notes },
            cancellationToken: cancellationToken);

        return true;
    }

    public async Task<bool> RejectAsync(
        Guid id,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        var quarantined = await _dbContext.Set<QuarantinedDocument>()
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);

        if (quarantined == null || quarantined.Status != QuarantineStatus.Pending)
            return false;

        quarantined.Reject(_currentUser.UserId ?? Guid.Empty, _currentUser.DisplayName ?? "System", notes);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogActionAsync(
            "QuarantineRejected",
            AuditCategories.SecurityEvent,
            "QuarantinedDocument",
            id,
            additionalData: new { DocumentId = quarantined.DocumentId, Notes = notes },
            cancellationToken: cancellationToken);

        return true;
    }

    public async Task<bool> ReleaseAsync(
        Guid id,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        var quarantined = await _dbContext.Set<QuarantinedDocument>()
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);

        if (quarantined == null || quarantined.Status != QuarantineStatus.Pending)
            return false;

        quarantined.Release(_currentUser.UserId ?? Guid.Empty, _currentUser.DisplayName ?? "System", notes);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogActionAsync(
            AuditActions.DocumentReleased,
            AuditCategories.SecurityEvent,
            "QuarantinedDocument",
            id,
            additionalData: new { DocumentId = quarantined.DocumentId, Notes = notes },
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "Document {DocumentId} released from quarantine",
            quarantined.DocumentId);

        return true;
    }

    public async Task<bool> IsQuarantinedAsync(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<QuarantinedDocument>()
            .AnyAsync(q => q.DocumentId == documentId && q.Status == QuarantineStatus.Pending,
                cancellationToken);
    }

    private static QuarantinedDocumentDto MapToDto(QuarantinedDocument q)
    {
        return new QuarantinedDocumentDto
        {
            Id = q.Id,
            DocumentId = q.DocumentId,
            DocumentName = q.DocumentName,
            OriginalPath = q.OriginalPath,
            Reason = q.Reason.ToString(),
            ReasonDetails = q.ReasonDetails,
            QuarantinedByUserId = q.QuarantinedByUserId,
            QuarantinedByName = q.QuarantinedByName,
            QuarantinedAt = q.QuarantinedAt,
            Status = q.Status.ToString(),
            ReviewedByUserId = q.ReviewedByUserId,
            ReviewedByName = q.ReviewedByName,
            ReviewedAt = q.ReviewedAt,
            ReviewNotes = q.ReviewNotes
        };
    }
}
