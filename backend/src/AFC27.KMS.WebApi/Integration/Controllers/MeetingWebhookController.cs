using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AFC27.KMS.WebApi.Integration.Configuration;
using AFC27.KMS.WebApi.Integration.Meeting;
using AFC27.KMS.WebApi.Integration.Meeting.Models;

namespace AFC27.KMS.WebApi.Integration.Controllers;

/// <summary>
/// Controller for handling meeting service webhooks
/// </summary>
[ApiController]
[Route("api/webhooks/meeting")]
public class MeetingWebhookController : ControllerBase
{
    private readonly IMeetingIntegrationService _meetingService;
    private readonly ILogger<MeetingWebhookController> _logger;
    private readonly IntegrationSettings _settings;

    public MeetingWebhookController(
        IMeetingIntegrationService meetingService,
        ILogger<MeetingWebhookController> logger,
        IOptions<IntegrationSettings> settings)
    {
        _meetingService = meetingService;
        _logger = logger;
        _settings = settings.Value;
    }

    /// <summary>
    /// Receives meeting event updates from external meeting service
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> HandleMeetingWebhook([FromBody] MeetingWebhookPayload payload)
    {
        _logger.LogInformation(
            "Received meeting webhook for meeting {MeetingId}, event: {Event}",
            payload.MeetingId, payload.Event);

        // Validate webhook signature
        var webhookSecret = _settings.MeetingManagement?.WebhookSecret;
        if (!string.IsNullOrEmpty(webhookSecret))
        {
            if (!_meetingService.ValidateWebhookSignature(payload, webhookSecret))
            {
                _logger.LogWarning(
                    "Invalid webhook signature for meeting {MeetingId}",
                    payload.MeetingId);
                return Unauthorized(new { error = "Invalid signature" });
            }
        }

        try
        {
            // Process the webhook based on event type
            switch (payload.Event.ToLowerInvariant())
            {
                case "meeting.started":
                    await HandleMeetingStartedAsync(payload);
                    break;

                case "meeting.ended":
                    await HandleMeetingEndedAsync(payload);
                    break;

                case "participant.joined":
                    await HandleParticipantJoinedAsync(payload);
                    break;

                case "participant.left":
                    await HandleParticipantLeftAsync(payload);
                    break;

                case "recording.ready":
                    await HandleRecordingReadyAsync(payload);
                    break;

                case "transcript.ready":
                    await HandleTranscriptReadyAsync(payload);
                    break;

                default:
                    _logger.LogInformation(
                        "Meeting {MeetingId} event: {Event}",
                        payload.MeetingId, payload.Event);
                    break;
            }

            return Ok(new { received = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing meeting webhook for meeting {MeetingId}",
                payload.MeetingId);
            return Ok(new { received = true, error = "Processing failed" });
        }
    }

    private Task HandleMeetingStartedAsync(MeetingWebhookPayload payload)
    {
        _logger.LogInformation(
            "Meeting {MeetingId} started at {Timestamp}",
            payload.MeetingId, payload.Timestamp);

        // TODO: Update meeting status in database
        // TODO: Notify relevant users

        return Task.CompletedTask;
    }

    private async Task HandleMeetingEndedAsync(MeetingWebhookPayload payload)
    {
        _logger.LogInformation(
            "Meeting {MeetingId} ended at {Timestamp}",
            payload.MeetingId, payload.Timestamp);

        // TODO: Update meeting status in database
        // TODO: Trigger minutes generation if auto-minutes is enabled

        // Get meeting minutes
        var minutesResult = await _meetingService.GetMeetingMinutesAsync(payload.MeetingId);
        if (minutesResult.IsSuccess)
        {
            _logger.LogInformation(
                "Retrieved minutes for meeting {MeetingId}: {ItemCount} items, {ActionCount} action items",
                payload.MeetingId,
                minutesResult.Data?.Items.Count ?? 0,
                minutesResult.Data?.ActionItems.Count ?? 0);
        }
    }

    private Task HandleParticipantJoinedAsync(MeetingWebhookPayload payload)
    {
        _logger.LogInformation(
            "Participant {ParticipantEmail} joined meeting {MeetingId}",
            payload.Participant?.Email, payload.MeetingId);

        // TODO: Track attendance

        return Task.CompletedTask;
    }

    private Task HandleParticipantLeftAsync(MeetingWebhookPayload payload)
    {
        _logger.LogInformation(
            "Participant {ParticipantEmail} left meeting {MeetingId}",
            payload.Participant?.Email, payload.MeetingId);

        // TODO: Update attendance duration

        return Task.CompletedTask;
    }

    private async Task HandleRecordingReadyAsync(MeetingWebhookPayload payload)
    {
        _logger.LogInformation(
            "Recording ready for meeting {MeetingId}",
            payload.MeetingId);

        // Get the recording details
        var recordingResult = await _meetingService.GetMeetingRecordingAsync(payload.MeetingId);
        if (recordingResult.IsSuccess && recordingResult.Data != null)
        {
            // TODO: Store recording reference
            // TODO: Link to meeting record
            // TODO: Notify meeting organizer

            _logger.LogInformation(
                "Recording retrieved for meeting {MeetingId}: {Duration} seconds, {Size} bytes",
                payload.MeetingId,
                recordingResult.Data.DurationSeconds,
                recordingResult.Data.FileSizeBytes);
        }
    }

    private Task HandleTranscriptReadyAsync(MeetingWebhookPayload payload)
    {
        _logger.LogInformation(
            "Transcript ready for meeting {MeetingId}",
            payload.MeetingId);

        // TODO: Download and store transcript
        // TODO: Process transcript for AI analysis
        // TODO: Extract action items using AI

        return Task.CompletedTask;
    }
}
