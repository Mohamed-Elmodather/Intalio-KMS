using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AFC27.KMS.WebApi.Integration.Configuration;
using AFC27.KMS.WebApi.Integration.Core;
using AFC27.KMS.WebApi.Integration.Meeting.Models;

namespace AFC27.KMS.WebApi.Integration.Meeting;

/// <summary>
/// Implementation of meeting management integration service
/// </summary>
public class MeetingIntegrationService : ExternalServiceClientBase, IMeetingIntegrationService
{
    private readonly MeetingServiceSettings _settings;

    public override string ServiceName => "MeetingManagement";

    public MeetingIntegrationService(
        HttpClient httpClient,
        ILogger<MeetingIntegrationService> logger,
        IOptions<IntegrationSettings> settings)
        : base(httpClient, logger, settings.Value.MeetingManagement)
    {
        _settings = settings.Value.MeetingManagement;
    }

    public async Task<ServiceResponse<MeetingResponse>> CreateMeetingAsync(
        CreateMeetingRequest request,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation(
            "Creating meeting '{Title}' scheduled for {StartTime}",
            request.Title, request.StartTime);

        return await PostAsync<CreateMeetingRequest, MeetingResponse>(
            "meetings",
            request,
            cancellationToken);
    }

    public async Task<ServiceResponse<MeetingResponse>> UpdateMeetingAsync(
        string meetingId,
        CreateMeetingRequest request,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation(
            "Updating meeting {MeetingId}",
            meetingId);

        return await PutAsync<CreateMeetingRequest, MeetingResponse>(
            $"meetings/{meetingId}",
            request,
            cancellationToken);
    }

    public async Task<ServiceResponse<MeetingResponse>> GetMeetingAsync(
        string meetingId,
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<MeetingResponse>(
            $"meetings/{meetingId}",
            cancellationToken);
    }

    public async Task<ServiceResponse<bool>> CancelMeetingAsync(
        string meetingId,
        string reason,
        bool notifyParticipants = true,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation(
            "Cancelling meeting {MeetingId}: {Reason}",
            meetingId, reason);

        var response = await PostAsync<object, object>(
            $"meetings/{meetingId}/cancel",
            new { Reason = reason, NotifyParticipants = notifyParticipants },
            cancellationToken);

        return new ServiceResponse<bool>
        {
            IsSuccess = response.IsSuccess,
            Data = response.IsSuccess,
            ErrorMessage = response.ErrorMessage,
            ErrorCode = response.ErrorCode,
            HttpStatusCode = response.HttpStatusCode
        };
    }

    public async Task<ServiceResponse<MeetingRecording>> GetMeetingRecordingAsync(
        string meetingId,
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<MeetingRecording>(
            $"meetings/{meetingId}/recording",
            cancellationToken);
    }

    public async Task<ServiceResponse<MeetingMinutes>> GetMeetingMinutesAsync(
        string meetingId,
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<MeetingMinutes>(
            $"meetings/{meetingId}/minutes",
            cancellationToken);
    }

    public async Task<ServiceResponse<MeetingMinutes>> UpdateMeetingMinutesAsync(
        string meetingId,
        MeetingMinutes minutes,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation(
            "Updating minutes for meeting {MeetingId}",
            meetingId);

        return await PutAsync<MeetingMinutes, MeetingMinutes>(
            $"meetings/{meetingId}/minutes",
            minutes,
            cancellationToken);
    }

    public async Task<ServiceResponse<ActionItem>> AddActionItemAsync(
        string meetingId,
        ActionItem actionItem,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation(
            "Adding action item to meeting {MeetingId}: {Description}",
            meetingId, actionItem.Description);

        return await PostAsync<ActionItem, ActionItem>(
            $"meetings/{meetingId}/actions",
            actionItem,
            cancellationToken);
    }

    public async Task<ServiceResponse<ActionItem>> UpdateActionItemAsync(
        string meetingId,
        string actionId,
        ActionItemStatus status,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation(
            "Updating action item {ActionId} status to {Status}",
            actionId, status);

        var response = await PutAsync<object, ActionItem>(
            $"meetings/{meetingId}/actions/{actionId}",
            new { Status = status },
            cancellationToken);

        return response;
    }

    public async Task<ServiceResponse<bool>> SendInvitationsAsync(
        string meetingId,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation(
            "Sending invitations for meeting {MeetingId}",
            meetingId);

        var response = await PostAsync<object, object>(
            $"meetings/{meetingId}/invite",
            new { },
            cancellationToken);

        return new ServiceResponse<bool>
        {
            IsSuccess = response.IsSuccess,
            Data = response.IsSuccess,
            ErrorMessage = response.ErrorMessage,
            ErrorCode = response.ErrorCode,
            HttpStatusCode = response.HttpStatusCode
        };
    }

    public bool ValidateWebhookSignature(MeetingWebhookPayload payload, string secret)
    {
        if (string.IsNullOrEmpty(payload.Signature) || string.IsNullOrEmpty(secret))
            return false;

        var dataToSign = $"{payload.MeetingId}:{payload.Event}:{payload.Status}:{payload.Timestamp:O}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
        var computedSignature = Convert.ToBase64String(hash);

        return payload.Signature == computedSignature;
    }
}
