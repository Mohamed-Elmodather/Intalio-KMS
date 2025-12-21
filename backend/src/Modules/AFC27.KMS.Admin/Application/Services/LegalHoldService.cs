using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Admin.Domain.Entities;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Application.Interfaces;
using AFC27.KMS.SharedKernel.Interfaces;

using AFC27.KMS.Documents.Domain.Entities;
using AFC27.KMS.Identity.Domain.Entities;

namespace AFC27.KMS.Admin.Application.Services;

/// <summary>
/// Service for legal hold management.
/// </summary>
public class LegalHoldService : ILegalHoldService
{
    private readonly DbContext _dbContext;
    private readonly ICurrentUser _currentUser;
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<LegalHoldService> _logger;

    public LegalHoldService(
        DbContext dbContext,
        ICurrentUser currentUser,
        IAuditLogService auditLogService,
        ILogger<LegalHoldService> logger)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _auditLogService = auditLogService;
        _logger = logger;
    }

    public async Task<LegalHoldDto> CreateAsync(
        CreateLegalHoldRequest request,
        CancellationToken cancellationToken = default)
    {
        var legalHold = LegalHold.Create(
            request.Name,
            request.Description,
            request.CaseReference,
            _currentUser.UserId ?? Guid.Empty,
            _currentUser.DisplayName ?? "System",
            request.Notes);

        _dbContext.Set<LegalHold>().Add(legalHold);

        // Add initial documents if provided
        if (request.InitialDocumentIds?.Any() == true)
        {
            var documents = await _dbContext.Set<Document>()
                .Where(d => request.InitialDocumentIds.Contains(d.Id))
                .ToListAsync(cancellationToken);

            foreach (var doc in documents)
            {
                legalHold.AddDocument(doc.Id, doc.FileName);
            }
        }

        // Add initial custodians if provided
        if (request.InitialCustodianIds?.Any() == true)
        {
            var users = await _dbContext.Set<User>()
                .Where(u => request.InitialCustodianIds.Contains(u.Id))
                .ToListAsync(cancellationToken);

            foreach (var user in users)
            {
                legalHold.AddCustodian(user.Id, user.DisplayName, user.Email);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogActionAsync(
            AuditActions.LegalHoldApplied,
            AuditCategories.Compliance,
            "LegalHold",
            legalHold.Id,
            newValues: new { request.Name, request.CaseReference },
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "Legal hold {LegalHoldId} created: {Name}",
            legalHold.Id, request.Name);

        return MapToDto(legalHold);
    }

    public async Task<LegalHoldDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var legalHold = await _dbContext.Set<LegalHold>()
            .AsNoTracking()
            .Include(h => h.Documents)
            .Include(h => h.Custodians)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);

        return legalHold == null ? null : MapToDto(legalHold);
    }

    public async Task<IReadOnlyList<LegalHoldDto>> ListAsync(
        bool includeReleased = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<LegalHold> query = _dbContext.Set<LegalHold>()
            .AsNoTracking()
            .Include(h => h.Documents)
            .Include(h => h.Custodians);

        if (!includeReleased)
        {
            query = query.Where(h => h.Status == LegalHoldStatus.Active);
        }

        return await query
            .OrderByDescending(h => h.StartDate)
            .Select(h => MapToDto(h))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AddDocumentAsync(
        Guid legalHoldId,
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        var legalHold = await _dbContext.Set<LegalHold>()
            .Include(h => h.Documents)
            .FirstOrDefaultAsync(h => h.Id == legalHoldId, cancellationToken);

        if (legalHold == null || legalHold.Status != LegalHoldStatus.Active)
            return false;

        var document = await _dbContext.Set<Document>()
            .FirstOrDefaultAsync(d => d.Id == documentId, cancellationToken);

        if (document == null)
            return false;

        legalHold.AddDocument(documentId, document.FileName);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogActionAsync(
            AuditActions.LegalHoldApplied,
            AuditCategories.Compliance,
            "Document",
            documentId,
            additionalData: new { LegalHoldId = legalHoldId, LegalHoldName = legalHold.Name },
            cancellationToken: cancellationToken);

        return true;
    }

    public async Task<bool> RemoveDocumentAsync(
        Guid legalHoldId,
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        var legalHold = await _dbContext.Set<LegalHold>()
            .Include(h => h.Documents)
            .FirstOrDefaultAsync(h => h.Id == legalHoldId, cancellationToken);

        if (legalHold == null)
            return false;

        legalHold.RemoveDocument(documentId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> AddCustodianAsync(
        Guid legalHoldId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var legalHold = await _dbContext.Set<LegalHold>()
            .Include(h => h.Custodians)
            .FirstOrDefaultAsync(h => h.Id == legalHoldId, cancellationToken);

        if (legalHold == null || legalHold.Status != LegalHoldStatus.Active)
            return false;

        var user = await _dbContext.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            return false;

        legalHold.AddCustodian(userId, user.DisplayName, user.Email);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ReleaseAsync(
        Guid id,
        string? releaseNotes = null,
        CancellationToken cancellationToken = default)
    {
        var legalHold = await _dbContext.Set<LegalHold>()
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);

        if (legalHold == null || legalHold.Status != LegalHoldStatus.Active)
            return false;

        legalHold.Release(releaseNotes);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogActionAsync(
            AuditActions.LegalHoldReleased,
            AuditCategories.Compliance,
            "LegalHold",
            id,
            additionalData: new { LegalHoldName = legalHold.Name, ReleaseNotes = releaseNotes },
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "Legal hold {LegalHoldId} released: {Name}",
            id, legalHold.Name);

        return true;
    }

    public async Task<bool> IsDocumentUnderHoldAsync(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<LegalHoldDocument>()
            .AnyAsync(d =>
                d.DocumentId == documentId &&
                d.LegalHold.Status == LegalHoldStatus.Active,
                cancellationToken);
    }

    public async Task<IReadOnlyList<LegalHoldDto>> GetHoldsForDocumentAsync(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<LegalHold>()
            .AsNoTracking()
            .Include(h => h.Documents)
            .Include(h => h.Custodians)
            .Where(h => h.Status == LegalHoldStatus.Active &&
                        h.Documents.Any(d => d.DocumentId == documentId))
            .Select(h => MapToDto(h))
            .ToListAsync(cancellationToken);
    }

    private static LegalHoldDto MapToDto(LegalHold legalHold)
    {
        return new LegalHoldDto
        {
            Id = legalHold.Id,
            Name = legalHold.Name,
            Description = legalHold.Description,
            CaseReference = legalHold.CaseReference,
            CreatedByUserId = legalHold.CreatedByUserId,
            CreatedByName = legalHold.CreatedByName,
            StartDate = legalHold.StartDate,
            EndDate = legalHold.EndDate,
            Status = legalHold.Status.ToString(),
            Notes = legalHold.Notes,
            DocumentCount = legalHold.Documents.Count,
            CustodianCount = legalHold.Custodians.Count,
            Documents = legalHold.Documents.Select(d => new LegalHoldDocumentDto
            {
                DocumentId = d.DocumentId,
                DocumentName = d.DocumentName,
                AddedAt = d.AddedAt
            }).ToList(),
            Custodians = legalHold.Custodians.Select(c => new LegalHoldCustodianDto
            {
                UserId = c.UserId,
                UserName = c.UserName,
                Email = c.Email,
                AddedAt = c.AddedAt,
                IsNotified = c.IsNotified,
                NotifiedAt = c.NotifiedAt
            }).ToList()
        };
    }
}
