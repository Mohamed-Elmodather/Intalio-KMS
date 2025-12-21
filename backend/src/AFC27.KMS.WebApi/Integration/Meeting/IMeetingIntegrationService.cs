using System;
using System.Threading;
using System.Threading.Tasks;
using AFC27.KMS.WebApi.Integration.Core;
using AFC27.KMS.WebApi.Integration.Meeting.Models;

namespace AFC27.KMS.WebApi.Integration.Meeting;

/// <summary>
/// Interface for meeting management integration service
/// </summary>
public interface IMeetingIntegrationService : IExternalServiceClient
{
    /// <summary>
    /// Creates a new meeting
    /// </summary>
    Task<ServiceResponse<MeetingResponse>> CreateMeetingAsync(
        CreateMeetingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing meeting
    /// </summary>
    Task<ServiceResponse<MeetingResponse>> UpdateMeetingAsync(
        string meetingId,
        CreateMeetingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets meeting details
    /// </summary>
    Task<ServiceResponse<MeetingResponse>> GetMeetingAsync(
        string meetingId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels a meeting
    /// </summary>
    Task<ServiceResponse<bool>> CancelMeetingAsync(
        string meetingId,
        string reason,
        bool notifyParticipants = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets meeting recording
    /// </summary>
    Task<ServiceResponse<MeetingRecording>> GetMeetingRecordingAsync(
        string meetingId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or creates meeting minutes
    /// </summary>
    Task<ServiceResponse<MeetingMinutes>> GetMeetingMinutesAsync(
        string meetingId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates meeting minutes
    /// </summary>
    Task<ServiceResponse<MeetingMinutes>> UpdateMeetingMinutesAsync(
        string meetingId,
        MeetingMinutes minutes,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds action item to meeting
    /// </summary>
    Task<ServiceResponse<ActionItem>> AddActionItemAsync(
        string meetingId,
        ActionItem actionItem,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates action item status
    /// </summary>
    Task<ServiceResponse<ActionItem>> UpdateActionItemAsync(
        string meetingId,
        string actionId,
        ActionItemStatus status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends meeting invitations to participants
    /// </summary>
    Task<ServiceResponse<bool>> SendInvitationsAsync(
        string meetingId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates webhook signature
    /// </summary>
    bool ValidateWebhookSignature(MeetingWebhookPayload payload, string secret);
}
