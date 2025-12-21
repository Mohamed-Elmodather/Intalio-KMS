using Microsoft.Extensions.Logging;
using MassTransit;
using AFC27.KMS.Infrastructure.Messaging;
using AFC27.KMS.Infrastructure.Messaging.Messages;
using AFC27.KMS.NotificationWorker.Services;

namespace AFC27.KMS.NotificationWorker.Consumers;

/// <summary>
/// Consumer for processing notification requests.
/// Dispatches notifications to appropriate channels (in-app, email, push).
/// </summary>
public class NotificationConsumer : BaseConsumer<SendNotificationMessage>
{
    private readonly InAppNotificationService _inAppService;
    private readonly EmailService _emailService;
    private readonly PushNotificationService _pushService;

    public NotificationConsumer(
        InAppNotificationService inAppService,
        EmailService emailService,
        PushNotificationService pushService,
        ILogger<NotificationConsumer> logger)
        : base(logger)
    {
        _inAppService = inAppService;
        _emailService = emailService;
        _pushService = pushService;
    }

    protected override async Task ProcessMessageAsync(
        SendNotificationMessage message,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation(
            "Processing notification for user {UserId}: {Title}",
            message.UserId,
            message.Title);

        // Check if notification is scheduled for future
        if (message.ScheduledAt.HasValue && message.ScheduledAt.Value > DateTime.UtcNow)
        {
            Logger.LogInformation(
                "Notification scheduled for {ScheduledAt}, skipping for now",
                message.ScheduledAt.Value);
            // In production, re-queue with delay or use scheduler
            return;
        }

        // Check if notification has expired
        if (message.ExpiresAt.HasValue && message.ExpiresAt.Value < DateTime.UtcNow)
        {
            Logger.LogInformation(
                "Notification expired at {ExpiresAt}, not delivering",
                message.ExpiresAt.Value);
            return;
        }

        // Process each channel
        var tasks = new List<Task>();

        if (message.Channels.Contains(NotificationChannel.InApp))
        {
            tasks.Add(SendInAppNotificationAsync(message, cancellationToken));
        }

        if (message.Channels.Contains(NotificationChannel.Email))
        {
            tasks.Add(SendEmailNotificationAsync(message, cancellationToken));
        }

        if (message.Channels.Contains(NotificationChannel.Push))
        {
            tasks.Add(SendPushNotificationAsync(message, cancellationToken));
        }

        await Task.WhenAll(tasks);

        Logger.LogInformation(
            "Notification delivered to user {UserId} via {ChannelCount} channels",
            message.UserId,
            message.Channels.Count);
    }

    private async Task SendInAppNotificationAsync(
        SendNotificationMessage message,
        CancellationToken cancellationToken)
    {
        await _inAppService.CreateNotificationAsync(
            message.UserId,
            message.Type.ToString(),
            message.Title,
            message.TitleArabic,
            message.Body,
            message.BodyArabic,
            message.RelatedEntityType,
            message.RelatedEntityId,
            message.ActionUrl,
            message.Data,
            cancellationToken);
    }

    private async Task SendEmailNotificationAsync(
        SendNotificationMessage message,
        CancellationToken cancellationToken)
    {
        // In production, look up user's email from database
        var userEmail = $"user_{message.UserId}@example.com"; // Placeholder

        await _emailService.SendEmailAsync(
            userEmail,
            message.Title,
            message.Body,
            isHtml: true,
            cancellationToken: cancellationToken);
    }

    private async Task SendPushNotificationAsync(
        SendNotificationMessage message,
        CancellationToken cancellationToken)
    {
        await _pushService.SendPushNotificationAsync(
            message.UserId,
            message.Title,
            message.Body,
            message.Data,
            cancellationToken);
    }
}
