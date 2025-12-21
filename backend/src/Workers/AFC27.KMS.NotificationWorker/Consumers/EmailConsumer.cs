using Microsoft.Extensions.Logging;
using MassTransit;
using AFC27.KMS.Infrastructure.Messaging;
using AFC27.KMS.Infrastructure.Messaging.Messages;
using AFC27.KMS.NotificationWorker.Services;

namespace AFC27.KMS.NotificationWorker.Consumers;

/// <summary>
/// Consumer for processing direct email requests.
/// </summary>
public class EmailConsumer : BaseConsumer<SendEmailMessage>
{
    private readonly EmailService _emailService;

    public EmailConsumer(
        EmailService emailService,
        ILogger<EmailConsumer> logger)
        : base(logger)
    {
        _emailService = emailService;
    }

    protected override async Task ProcessMessageAsync(
        SendEmailMessage message,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation(
            "Processing email to {To}: {Subject}",
            message.To,
            message.Subject);

        bool success;

        if (!string.IsNullOrEmpty(message.TemplateName))
        {
            // Use template
            success = await _emailService.SendTemplatedEmailAsync(
                message.To,
                message.TemplateName,
                message.TemplateData,
                cancellationToken);
        }
        else
        {
            // Send raw email
            success = await _emailService.SendEmailAsync(
                message.To,
                message.Subject,
                message.Body,
                message.IsHtml,
                message.Cc,
                message.Bcc,
                cancellationToken);
        }

        if (!success)
        {
            Logger.LogWarning("Failed to send email to {To}", message.To);
            throw new Exception($"Failed to send email to {message.To}");
        }

        Logger.LogInformation("Email sent to {To}", message.To);
    }
}
