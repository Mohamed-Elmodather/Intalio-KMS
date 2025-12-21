using MassTransit;
using Microsoft.Extensions.Logging;

namespace AFC27.KMS.Infrastructure.Messaging;

/// <summary>
/// Base consumer class with common functionality for message handling.
/// Provides logging, error handling, and retry logic.
/// </summary>
/// <typeparam name="TMessage">The message type to consume</typeparam>
public abstract class BaseConsumer<TMessage> : IConsumer<TMessage>
    where TMessage : class
{
    protected readonly ILogger Logger;

    protected BaseConsumer(ILogger logger)
    {
        Logger = logger;
    }

    public async Task Consume(ConsumeContext<TMessage> context)
    {
        var messageType = typeof(TMessage).Name;
        var messageId = context.MessageId?.ToString() ?? "unknown";

        Logger.LogInformation(
            "Processing message {MessageType} with ID {MessageId}",
            messageType,
            messageId);

        try
        {
            await ProcessMessageAsync(context.Message, context.CancellationToken);

            Logger.LogInformation(
                "Successfully processed message {MessageType} with ID {MessageId}",
                messageType,
                messageId);
        }
        catch (Exception ex)
        {
            Logger.LogError(
                ex,
                "Error processing message {MessageType} with ID {MessageId}: {Error}",
                messageType,
                messageId,
                ex.Message);

            // Re-throw to let MassTransit handle retry/dead-letter
            throw;
        }
    }

    /// <summary>
    /// Process the message. Override in derived classes.
    /// </summary>
    /// <param name="message">The message to process</param>
    /// <param name="cancellationToken">Cancellation token</param>
    protected abstract Task ProcessMessageAsync(TMessage message, CancellationToken cancellationToken);
}
