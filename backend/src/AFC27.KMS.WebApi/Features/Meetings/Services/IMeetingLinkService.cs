using AFC27.KMS.WebApi.Features.Meetings.Models;

namespace AFC27.KMS.WebApi.Features.Meetings.Services;

/// <summary>
/// Interface for meeting document linking service
/// </summary>
public interface IMeetingLinkService
{
    // Meeting Links
    Task<ExternalMeetingLink> CreateMeetingLinkAsync(CreateMeetingLinkRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<ExternalMeetingLink?> GetMeetingLinkAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ExternalMeetingLink?> GetMeetingLinkByExternalIdAsync(string externalId, CancellationToken cancellationToken = default);
    Task<(List<MeetingSummary> Meetings, int TotalCount)> GetMeetingLinksAsync(MeetingStatus? status, DateTime? fromDate, DateTime? toDate, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ExternalMeetingLink> UpdateMeetingLinkAsync(Guid id, CreateMeetingLinkRequest request, CancellationToken cancellationToken = default);
    Task DeleteMeetingLinkAsync(Guid id, CancellationToken cancellationToken = default);

    // Document Linking
    Task<MeetingDocumentLink> LinkDocumentAsync(Guid meetingId, LinkDocumentRequest request, Guid userId, string userName, CancellationToken cancellationToken = default);
    Task<List<MeetingDocumentLink>> GetLinkedDocumentsAsync(Guid meetingId, CancellationToken cancellationToken = default);
    Task UnlinkDocumentAsync(Guid meetingId, Guid documentId, CancellationToken cancellationToken = default);
    Task<List<MeetingSummary>> GetMeetingsForDocumentAsync(Guid documentId, CancellationToken cancellationToken = default);

    // Agenda Items
    Task<MeetingAgendaItem> AddAgendaItemAsync(Guid meetingId, CreateAgendaItemRequest request, CancellationToken cancellationToken = default);
    Task<List<MeetingAgendaItem>> GetAgendaItemsAsync(Guid meetingId, CancellationToken cancellationToken = default);
    Task<MeetingAgendaItem> UpdateAgendaItemAsync(Guid meetingId, Guid itemId, CreateAgendaItemRequest request, CancellationToken cancellationToken = default);
    Task DeleteAgendaItemAsync(Guid meetingId, Guid itemId, CancellationToken cancellationToken = default);
    Task ReorderAgendaItemsAsync(Guid meetingId, List<Guid> itemIds, CancellationToken cancellationToken = default);

    // Action Items
    Task<MeetingActionItem> AddActionItemAsync(Guid meetingId, CreateActionItemRequest request, CancellationToken cancellationToken = default);
    Task<List<MeetingActionItem>> GetActionItemsAsync(Guid meetingId, CancellationToken cancellationToken = default);
    Task<MeetingActionItem> UpdateActionItemStatusAsync(Guid meetingId, Guid actionId, UpdateActionItemRequest request, CancellationToken cancellationToken = default);
    Task<List<MeetingActionItem>> GetMyActionItemsAsync(string userEmail, ActionItemStatus? status, CancellationToken cancellationToken = default);
    Task<List<MeetingActionItem>> GetOverdueActionItemsAsync(CancellationToken cancellationToken = default);

    // Calendar
    Task<List<CalendarEvent>> GetCalendarEventsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    // Sync
    Task<MeetingSyncResult> SyncFromExternalServiceAsync(CancellationToken cancellationToken = default);
    Task UpdateFromWebhookAsync(string externalMeetingId, MeetingStatus status, string? data, CancellationToken cancellationToken = default);
}
