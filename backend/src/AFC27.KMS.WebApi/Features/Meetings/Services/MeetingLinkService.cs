using Microsoft.EntityFrameworkCore;
using AFC27.KMS.WebApi.Data;
using AFC27.KMS.WebApi.Data.Entities;
using AFC27.KMS.WebApi.Features.Meetings.Models;
using AFC27.KMS.WebApi.Integration.Meeting;

namespace AFC27.KMS.WebApi.Features.Meetings.Services;

/// <summary>
/// Implementation of meeting document linking service using database persistence
/// </summary>
public class MeetingLinkService : IMeetingLinkService
{
    private readonly KmsDbContext _dbContext;
    private readonly ILogger<MeetingLinkService> _logger;
    private readonly IMeetingIntegrationService _meetingIntegration;

    public MeetingLinkService(
        KmsDbContext dbContext,
        ILogger<MeetingLinkService> logger,
        IMeetingIntegrationService meetingIntegration)
    {
        _dbContext = dbContext;
        _logger = logger;
        _meetingIntegration = meetingIntegration;
    }

    public async Task<ExternalMeetingLink> CreateMeetingLinkAsync(CreateMeetingLinkRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        var entity = new ExternalMeetingLinkEntity
        {
            ExternalMeetingId = request.ExternalMeetingId,
            ExternalSystem = "External",
            Title = request.Title,
            TitleAr = request.TitleAr ?? request.Title,
            Description = request.Description,
            StartTime = request.MeetingDate,
            EndTime = request.EndDate ?? request.MeetingDate.AddHours(1),
            Location = request.Location,
            IsOnline = !string.IsNullOrEmpty(request.TeamsLink),
            OnlineMeetingUrl = request.TeamsLink,
            Status = MeetingStatusEnum.Scheduled,
            LastSyncedAt = DateTime.UtcNow
        };

        _dbContext.ExternalMeetingLinks.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created meeting link {MeetingId} for external meeting {ExternalId}", entity.Id, request.ExternalMeetingId);
        return MapToModel(entity);
    }

    public async Task<ExternalMeetingLink?> GetMeetingLinkAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.ExternalMeetingLinks
            .Include(m => m.DocumentLinks)
            .Include(m => m.Attendees)
            .Include(m => m.AgendaItems)
            .Include(m => m.ActionItems)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        return entity == null ? null : MapToModel(entity);
    }

    public async Task<ExternalMeetingLink?> GetMeetingLinkByExternalIdAsync(string externalId, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.ExternalMeetingLinks
            .Include(m => m.DocumentLinks)
            .Include(m => m.AgendaItems)
            .Include(m => m.ActionItems)
            .FirstOrDefaultAsync(m => m.ExternalMeetingId == externalId, cancellationToken);

        return entity == null ? null : MapToModel(entity);
    }

    public async Task<(List<MeetingSummary> Meetings, int TotalCount)> GetMeetingLinksAsync(
        MeetingStatus? status, DateTime? fromDate, DateTime? toDate, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ExternalMeetingLinks
            .Include(m => m.DocumentLinks)
            .Include(m => m.ActionItems)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(m => m.Status == (MeetingStatusEnum)status.Value);
        if (fromDate.HasValue)
            query = query.Where(m => m.StartTime >= fromDate.Value);
        if (toDate.HasValue)
            query = query.Where(m => m.StartTime <= toDate.Value);

        var total = await query.CountAsync(cancellationToken);

        var meetings = await query
            .OrderByDescending(m => m.StartTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new MeetingSummary
            {
                Id = m.Id,
                ExternalMeetingId = m.ExternalMeetingId,
                Title = m.Title,
                MeetingDate = m.StartTime,
                Status = (MeetingStatus)m.Status,
                DocumentCount = m.DocumentLinks.Count,
                ActionItemCount = m.ActionItems.Count,
                PendingActionItems = m.ActionItems.Count(a => a.Status == ActionItemStatusEnum.Pending || a.Status == ActionItemStatusEnum.InProgress),
                OrganizerName = m.OrganizerName
            })
            .ToListAsync(cancellationToken);

        return (meetings, total);
    }

    public async Task<ExternalMeetingLink> UpdateMeetingLinkAsync(Guid id, CreateMeetingLinkRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.ExternalMeetingLinks.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new KeyNotFoundException("Meeting not found");

        entity.Title = request.Title;
        entity.TitleAr = request.TitleAr ?? request.Title;
        entity.Description = request.Description;
        entity.StartTime = request.MeetingDate;
        entity.EndTime = request.EndDate ?? request.MeetingDate.AddHours(1);
        entity.Location = request.Location;
        entity.OnlineMeetingUrl = request.TeamsLink;
        entity.LastSyncedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapToModel(entity);
    }

    public async Task DeleteMeetingLinkAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.ExternalMeetingLinks.FindAsync(new object[] { id }, cancellationToken);
        if (entity != null)
        {
            _dbContext.ExternalMeetingLinks.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    // Document Linking
    public async Task<MeetingDocumentLink> LinkDocumentAsync(Guid meetingId, LinkDocumentRequest request, Guid userId, string userName, CancellationToken cancellationToken = default)
    {
        var meeting = await _dbContext.ExternalMeetingLinks.FindAsync(new object[] { meetingId }, cancellationToken)
            ?? throw new KeyNotFoundException("Meeting not found");

        var existingLink = await _dbContext.MeetingDocumentLinks
            .FirstOrDefaultAsync(l => l.MeetingId == meetingId && l.DocumentId == request.DocumentId, cancellationToken);

        if (existingLink != null)
            throw new InvalidOperationException("Document already linked to this meeting");

        var entity = new MeetingDocumentLinkEntity
        {
            MeetingId = meetingId,
            DocumentId = request.DocumentId,
            LinkType = (DocumentLinkTypeEnum)request.LinkType,
            LinkedById = userId,
            LinkedByName = userName,
            SortOrder = await _dbContext.MeetingDocumentLinks.CountAsync(l => l.MeetingId == meetingId, cancellationToken)
        };

        _dbContext.MeetingDocumentLinks.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Linked document {DocumentId} to meeting {MeetingId}", request.DocumentId, meetingId);

        return new MeetingDocumentLink
        {
            Id = entity.Id,
            MeetingLinkId = meetingId,
            DocumentId = request.DocumentId,
            DocumentTitle = $"Document {request.DocumentId}",
            LinkType = request.LinkType,
            LinkedAt = entity.CreatedAt,
            LinkedById = userId,
            LinkedByName = userName
        };
    }

    public async Task<List<MeetingDocumentLink>> GetLinkedDocumentsAsync(Guid meetingId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MeetingDocumentLinks
            .Where(l => l.MeetingId == meetingId)
            .OrderBy(l => l.SortOrder)
            .Select(l => new MeetingDocumentLink
            {
                Id = l.Id,
                MeetingLinkId = l.MeetingId,
                DocumentId = l.DocumentId,
                DocumentTitle = $"Document {l.DocumentId}",
                LinkType = (DocumentLinkType)l.LinkType,
                LinkedAt = l.CreatedAt,
                LinkedById = l.LinkedById,
                LinkedByName = l.LinkedByName
            })
            .ToListAsync(cancellationToken);
    }

    public async Task UnlinkDocumentAsync(Guid meetingId, Guid documentId, CancellationToken cancellationToken = default)
    {
        var link = await _dbContext.MeetingDocumentLinks
            .FirstOrDefaultAsync(l => l.MeetingId == meetingId && l.DocumentId == documentId, cancellationToken);

        if (link != null)
        {
            _dbContext.MeetingDocumentLinks.Remove(link);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<List<MeetingSummary>> GetMeetingsForDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MeetingDocumentLinks
            .Where(l => l.DocumentId == documentId)
            .Include(l => l.Meeting)
                .ThenInclude(m => m!.DocumentLinks)
            .Include(l => l.Meeting)
                .ThenInclude(m => m!.ActionItems)
            .Select(l => new MeetingSummary
            {
                Id = l.Meeting!.Id,
                ExternalMeetingId = l.Meeting.ExternalMeetingId,
                Title = l.Meeting.Title,
                MeetingDate = l.Meeting.StartTime,
                Status = (MeetingStatus)l.Meeting.Status,
                DocumentCount = l.Meeting.DocumentLinks.Count,
                ActionItemCount = l.Meeting.ActionItems.Count,
                PendingActionItems = l.Meeting.ActionItems.Count(a => a.Status == ActionItemStatusEnum.Pending),
                OrganizerName = l.Meeting.OrganizerName
            })
            .ToListAsync(cancellationToken);
    }

    // Agenda Items
    public async Task<MeetingAgendaItem> AddAgendaItemAsync(Guid meetingId, CreateAgendaItemRequest request, CancellationToken cancellationToken = default)
    {
        var meeting = await _dbContext.ExternalMeetingLinks.FindAsync(new object[] { meetingId }, cancellationToken)
            ?? throw new KeyNotFoundException("Meeting not found");

        var order = await _dbContext.MeetingAgendaItems.CountAsync(a => a.MeetingId == meetingId, cancellationToken);

        var entity = new MeetingAgendaItemEntity
        {
            MeetingId = meetingId,
            Title = request.Title,
            Description = request.Description,
            DurationMinutes = request.DurationMinutes,
            PresenterName = request.PresenterName,
            SortOrder = order
        };

        _dbContext.MeetingAgendaItems.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapAgendaItem(entity);
    }

    public async Task<List<MeetingAgendaItem>> GetAgendaItemsAsync(Guid meetingId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MeetingAgendaItems
            .Where(a => a.MeetingId == meetingId)
            .OrderBy(a => a.SortOrder)
            .Select(a => MapAgendaItem(a))
            .ToListAsync(cancellationToken);
    }

    public async Task<MeetingAgendaItem> UpdateAgendaItemAsync(Guid meetingId, Guid itemId, CreateAgendaItemRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.MeetingAgendaItems
            .FirstOrDefaultAsync(a => a.Id == itemId && a.MeetingId == meetingId, cancellationToken)
            ?? throw new KeyNotFoundException("Agenda item not found");

        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.DurationMinutes = request.DurationMinutes;
        entity.PresenterName = request.PresenterName;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapAgendaItem(entity);
    }

    public async Task DeleteAgendaItemAsync(Guid meetingId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.MeetingAgendaItems
            .FirstOrDefaultAsync(a => a.Id == itemId && a.MeetingId == meetingId, cancellationToken);

        if (entity != null)
        {
            _dbContext.MeetingAgendaItems.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task ReorderAgendaItemsAsync(Guid meetingId, List<Guid> itemIds, CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.MeetingAgendaItems
            .Where(a => a.MeetingId == meetingId && itemIds.Contains(a.Id))
            .ToListAsync(cancellationToken);

        for (int i = 0; i < itemIds.Count; i++)
        {
            var item = items.FirstOrDefault(a => a.Id == itemIds[i]);
            if (item != null) item.SortOrder = i;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Action Items
    public async Task<MeetingActionItem> AddActionItemAsync(Guid meetingId, CreateActionItemRequest request, CancellationToken cancellationToken = default)
    {
        var meeting = await _dbContext.ExternalMeetingLinks.FindAsync(new object[] { meetingId }, cancellationToken)
            ?? throw new KeyNotFoundException("Meeting not found");

        var entity = new MeetingActionItemEntity
        {
            MeetingId = meetingId,
            Title = request.Description,
            Description = request.Description,
            AssignedToName = request.AssignedToName,
            DueDate = request.DueDate,
            Priority = (ActionItemPriorityEnum)request.Priority,
            Status = ActionItemStatusEnum.Pending,
            SortOrder = await _dbContext.MeetingActionItems.CountAsync(a => a.MeetingId == meetingId, cancellationToken)
        };

        _dbContext.MeetingActionItems.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Added action item to meeting {MeetingId}: {Description}", meetingId, request.Description);
        return MapActionItem(entity, request.AssignedToEmail);
    }

    public async Task<List<MeetingActionItem>> GetActionItemsAsync(Guid meetingId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MeetingActionItems
            .Where(a => a.MeetingId == meetingId)
            .OrderBy(a => a.SortOrder)
            .Select(a => new MeetingActionItem
            {
                Id = a.Id,
                MeetingLinkId = a.MeetingId,
                Description = a.Description ?? a.Title,
                AssignedToName = a.AssignedToName,
                DueDate = a.DueDate,
                Status = (ActionItemStatus)a.Status,
                Priority = (ActionItemPriority)a.Priority,
                CreatedAt = a.CreatedAt,
                CompletedAt = a.CompletedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<MeetingActionItem> UpdateActionItemStatusAsync(Guid meetingId, Guid actionId, UpdateActionItemRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.MeetingActionItems
            .FirstOrDefaultAsync(a => a.Id == actionId && a.MeetingId == meetingId, cancellationToken)
            ?? throw new KeyNotFoundException("Action item not found");

        entity.Status = (ActionItemStatusEnum)request.Status;
        if (request.Status == ActionItemStatus.Completed)
            entity.CompletedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new MeetingActionItem
        {
            Id = entity.Id,
            MeetingLinkId = entity.MeetingId,
            Description = entity.Description ?? entity.Title,
            AssignedToName = entity.AssignedToName,
            DueDate = entity.DueDate,
            Status = (ActionItemStatus)entity.Status,
            Priority = (ActionItemPriority)entity.Priority,
            CreatedAt = entity.CreatedAt,
            CompletedAt = entity.CompletedAt
        };
    }

    public async Task<List<MeetingActionItem>> GetMyActionItemsAsync(string userEmail, ActionItemStatus? status, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.MeetingActionItems.AsQueryable();

        // Filter by assignee name since we store name not email
        if (status.HasValue)
            query = query.Where(a => a.Status == (ActionItemStatusEnum)status.Value);

        return await query
            .OrderBy(a => a.DueDate)
            .Select(a => new MeetingActionItem
            {
                Id = a.Id,
                MeetingLinkId = a.MeetingId,
                Description = a.Description ?? a.Title,
                AssignedToName = a.AssignedToName,
                DueDate = a.DueDate,
                Status = (ActionItemStatus)a.Status,
                Priority = (ActionItemPriority)a.Priority,
                CreatedAt = a.CreatedAt,
                CompletedAt = a.CompletedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MeetingActionItem>> GetOverdueActionItemsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _dbContext.MeetingActionItems
            .Where(a => a.Status != ActionItemStatusEnum.Completed && a.Status != ActionItemStatusEnum.Cancelled)
            .Where(a => a.DueDate.HasValue && a.DueDate.Value < now)
            .OrderBy(a => a.DueDate)
            .Select(a => new MeetingActionItem
            {
                Id = a.Id,
                MeetingLinkId = a.MeetingId,
                Description = a.Description ?? a.Title,
                AssignedToName = a.AssignedToName,
                DueDate = a.DueDate,
                Status = (ActionItemStatus)a.Status,
                Priority = (ActionItemPriority)a.Priority,
                CreatedAt = a.CreatedAt,
                CompletedAt = a.CompletedAt
            })
            .ToListAsync(cancellationToken);
    }

    // Calendar
    public async Task<List<CalendarEvent>> GetCalendarEventsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ExternalMeetingLinks
            .Where(m => m.StartTime >= startDate && m.StartTime <= endDate)
            .Include(m => m.DocumentLinks)
            .Include(m => m.ActionItems)
            .OrderBy(m => m.StartTime)
            .Select(m => new CalendarEvent
            {
                Id = m.Id,
                Title = m.Title,
                Start = m.StartTime,
                End = m.EndTime,
                Status = (MeetingStatus)m.Status,
                Location = m.Location,
                HasDocuments = m.DocumentLinks.Any(),
                HasPendingActions = m.ActionItems.Any(a => a.Status == ActionItemStatusEnum.Pending || a.Status == ActionItemStatusEnum.InProgress)
            })
            .ToListAsync(cancellationToken);
    }

    // Sync
    public async Task<MeetingSyncResult> SyncFromExternalServiceAsync(CancellationToken cancellationToken = default)
    {
        var result = new MeetingSyncResult();
        _logger.LogInformation("Starting meeting sync from external service");

        try
        {
            // In real implementation, call the external meeting service to get meetings
            await Task.Delay(100, cancellationToken);

            var meetings = await _dbContext.ExternalMeetingLinks.ToListAsync(cancellationToken);
            result.MeetingsUpdated = meetings.Count;

            foreach (var meeting in meetings)
            {
                meeting.LastSyncedAt = DateTime.UtcNow;
            }
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Meeting sync completed: {Created} created, {Updated} updated", result.MeetingsCreated, result.MeetingsUpdated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing meetings from external service");
            result.Errors.Add(ex.Message);
        }

        return result;
    }

    public async Task UpdateFromWebhookAsync(string externalMeetingId, MeetingStatus status, string? data, CancellationToken cancellationToken = default)
    {
        var meeting = await _dbContext.ExternalMeetingLinks
            .FirstOrDefaultAsync(m => m.ExternalMeetingId == externalMeetingId, cancellationToken);

        if (meeting != null)
        {
            meeting.Status = (MeetingStatusEnum)status;
            meeting.LastSyncedAt = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(data))
                meeting.ExternalData = data;

            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Updated meeting {ExternalId} status to {Status}", externalMeetingId, status);
        }
    }

    // Mapping helpers
    private static ExternalMeetingLink MapToModel(ExternalMeetingLinkEntity entity)
    {
        return new ExternalMeetingLink
        {
            Id = entity.Id,
            ExternalMeetingId = entity.ExternalMeetingId,
            Title = entity.Title,
            TitleAr = entity.TitleAr ?? entity.Title,
            Description = entity.Description,
            MeetingDate = entity.StartTime,
            EndDate = entity.EndTime,
            Location = entity.Location,
            TeamsLink = entity.OnlineMeetingUrl,
            ExternalSystemUrl = entity.OnlineMeetingUrl ?? string.Empty,
            Status = (MeetingStatus)entity.Status,
            OrganizerEmail = entity.OrganizerEmail,
            OrganizerName = entity.OrganizerName,
            LastSyncedAt = entity.LastSyncedAt ?? DateTime.UtcNow,
            CachedData = entity.ExternalData,
            CreatedAt = entity.CreatedAt,
            Attendees = entity.Attendees?.Select(a => new MeetingAttendee
            {
                Id = a.Id,
                MeetingLinkId = a.MeetingId,
                Email = a.Email,
                Name = a.Name,
                Role = (AttendeeRole)a.Role,
                Response = (AttendeeResponse)a.ResponseStatus,
                IsOptional = a.IsOptional
            }).ToList() ?? new List<MeetingAttendee>(),
            LinkedDocuments = entity.DocumentLinks?.Select(d => new MeetingDocumentLink
            {
                Id = d.Id,
                MeetingLinkId = d.MeetingId,
                DocumentId = d.DocumentId,
                DocumentTitle = $"Document {d.DocumentId}",
                LinkType = (DocumentLinkType)d.LinkType,
                LinkedAt = d.CreatedAt,
                LinkedById = d.LinkedById,
                LinkedByName = d.LinkedByName
            }).ToList() ?? new List<MeetingDocumentLink>(),
            AgendaItems = entity.AgendaItems?.OrderBy(a => a.SortOrder).Select(MapAgendaItem).ToList() ?? new List<MeetingAgendaItem>(),
            ActionItems = entity.ActionItems?.Select(a => new MeetingActionItem
            {
                Id = a.Id,
                MeetingLinkId = a.MeetingId,
                Description = a.Description ?? a.Title,
                AssignedToName = a.AssignedToName,
                DueDate = a.DueDate,
                Status = (ActionItemStatus)a.Status,
                Priority = (ActionItemPriority)a.Priority,
                CreatedAt = a.CreatedAt,
                CompletedAt = a.CompletedAt
            }).ToList() ?? new List<MeetingActionItem>()
        };
    }

    private static MeetingAgendaItem MapAgendaItem(MeetingAgendaItemEntity entity)
    {
        return new MeetingAgendaItem
        {
            Id = entity.Id,
            MeetingLinkId = entity.MeetingId,
            Order = entity.SortOrder,
            Title = entity.Title,
            Description = entity.Description,
            DurationMinutes = entity.DurationMinutes,
            PresenterName = entity.PresenterName
        };
    }

    private static MeetingActionItem MapActionItem(MeetingActionItemEntity entity, string? email = null)
    {
        return new MeetingActionItem
        {
            Id = entity.Id,
            MeetingLinkId = entity.MeetingId,
            Description = entity.Description ?? entity.Title,
            AssignedToEmail = email,
            AssignedToName = entity.AssignedToName,
            DueDate = entity.DueDate,
            Status = (ActionItemStatus)entity.Status,
            Priority = (ActionItemPriority)entity.Priority,
            CreatedAt = entity.CreatedAt,
            CompletedAt = entity.CompletedAt
        };
    }
}
