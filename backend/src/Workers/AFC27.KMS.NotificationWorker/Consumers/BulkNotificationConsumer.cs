using Microsoft.Extensions.Logging;
using MassTransit;
using AFC27.KMS.Infrastructure.Messaging;
using AFC27.KMS.Infrastructure.Messaging.Messages;
using AFC27.KMS.NotificationWorker.Services;

namespace AFC27.KMS.NotificationWorker.Consumers;

/// <summary>
/// Consumer for processing bulk notification requests.
/// </summary>
public class BulkNotificationConsumer : BaseConsumer<SendBulkNotificationMessage>
{
    private readonly InAppNotificationService _inAppService;
    private readonly EmailService _emailService;
    private readonly PushNotificationService _pushService;

    public BulkNotificationConsumer(
        InAppNotificationService inAppService,
        EmailService emailService,
        PushNotificationService pushService,
        ILogger<BulkNotificationConsumer> logger)
        : base(logger)
    {
        _inAppService = inAppService;
        _emailService = emailService;
        _pushService = pushService;
    }

    protected override async Task ProcessMessageAsync(
        SendBulkNotificationMessage message,
        CancellationToken cancellationToken)
    {
        var userIds = await ResolveUserIdsAsync(message, cancellationToken);

        Logger.LogInformation(
            "Processing bulk notification to {Count} users: {Title}",
            userIds.Count,
            message.Title);

        var tasks = new List<Task>();

        // In-app notifications
        if (message.Channels.Contains(NotificationChannel.InApp))
        {
            tasks.Add(_inAppService.CreateBulkNotificationsAsync(
                userIds,
                message.Type.ToString(),
                message.Title,
                message.TitleArabic,
                message.Body,
                message.BodyArabic,
                message.RelatedEntityType,
                message.RelatedEntityId,
                message.ActionUrl,
                cancellationToken));
        }

        // Push notifications
        if (message.Channels.Contains(NotificationChannel.Push))
        {
            tasks.Add(_pushService.SendBulkPushNotificationAsync(
                userIds,
                message.Title,
                message.Body,
                cancellationToken: cancellationToken));
        }

        // Email notifications
        if (message.Channels.Contains(NotificationChannel.Email))
        {
            // In production, batch lookup user emails
            var emails = userIds.Select(id => $"user_{id}@example.com");
            tasks.Add(_emailService.SendBulkEmailAsync(
                emails,
                message.Title,
                message.Body,
                isHtml: true,
                cancellationToken));
        }

        await Task.WhenAll(tasks);

        Logger.LogInformation(
            "Bulk notification delivered to {Count} users via {ChannelCount} channels",
            userIds.Count,
            message.Channels.Count);
    }

    private Task<List<Guid>> ResolveUserIdsAsync(
        SendBulkNotificationMessage message,
        CancellationToken cancellationToken)
    {
        // If explicit user IDs provided, use those
        if (message.UserIds.Any())
        {
            return Task.FromResult(message.UserIds);
        }

        // In production:
        // - If GroupId is specified, resolve group members
        // - If DepartmentId is specified, resolve department members

        if (message.GroupId.HasValue)
        {
            Logger.LogDebug("Would resolve users from group {GroupId}", message.GroupId);
            // return await _dbContext.GroupMembers
            //     .Where(gm => gm.GroupId == message.GroupId)
            //     .Select(gm => gm.UserId)
            //     .ToListAsync(cancellationToken);
        }

        if (message.DepartmentId.HasValue)
        {
            Logger.LogDebug("Would resolve users from department {DepartmentId}", message.DepartmentId);
            // return await _dbContext.Users
            //     .Where(u => u.DepartmentId == message.DepartmentId)
            //     .Select(u => u.Id)
            //     .ToListAsync(cancellationToken);
        }

        return Task.FromResult(new List<Guid>());
    }
}
