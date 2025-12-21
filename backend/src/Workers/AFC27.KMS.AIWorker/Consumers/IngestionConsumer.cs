using Microsoft.Extensions.Logging;
using MassTransit;
using AFC27.KMS.Infrastructure.Messaging;
using AFC27.KMS.Infrastructure.Messaging.Messages;
using AFC27.KMS.AIWorker.Services;

namespace AFC27.KMS.AIWorker.Consumers;

/// <summary>
/// Consumer for processing AI ingestion requests.
/// Extracts text, chunks content, and generates embeddings.
/// </summary>
public class IngestionConsumer : BaseConsumer<AIIngestionRequestMessage>
{
    private readonly TextExtractionService _textExtraction;
    private readonly ChunkingService _chunking;
    private readonly MockEmbeddingService _embedding;
    private readonly IPublishEndpoint _publishEndpoint;

    public IngestionConsumer(
        TextExtractionService textExtraction,
        ChunkingService chunking,
        MockEmbeddingService embedding,
        IPublishEndpoint publishEndpoint,
        ILogger<IngestionConsumer> logger)
        : base(logger)
    {
        _textExtraction = textExtraction;
        _chunking = chunking;
        _embedding = embedding;
        _publishEndpoint = publishEndpoint;
    }

    protected override async Task ProcessMessageAsync(
        AIIngestionRequestMessage message,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation(
            "Processing AI ingestion for {EntityType} {EntityId}",
            message.EntityType,
            message.EntityId);

        string text;

        // Extract text based on source
        if (!string.IsNullOrEmpty(message.Content))
        {
            // Direct content (e.g., Articles)
            text = message.Content;
            Logger.LogDebug("Using direct content for {EntityId}", message.EntityId);
        }
        else if (!string.IsNullOrEmpty(message.StoragePath) && !string.IsNullOrEmpty(message.ContentType))
        {
            // File-based content (e.g., Documents)
            var extractionResult = await _textExtraction.ExtractTextAsync(
                message.StoragePath,
                message.ContentType,
                cancellationToken);

            if (!extractionResult.Success)
            {
                Logger.LogWarning(
                    "Failed to extract text from {EntityId}: {Error}",
                    message.EntityId,
                    extractionResult.Error);
                return;
            }

            text = extractionResult.Text;
            Logger.LogDebug(
                "Extracted {CharCount} characters from {EntityId}",
                extractionResult.CharacterCount,
                message.EntityId);
        }
        else
        {
            Logger.LogWarning(
                "No content source provided for {EntityType} {EntityId}",
                message.EntityType,
                message.EntityId);
            return;
        }

        // Chunk the text
        var chunks = _chunking.ChunkByParagraph(text, message.Title).ToList();

        Logger.LogInformation(
            "Created {ChunkCount} chunks for {EntityId}",
            chunks.Count,
            message.EntityId);

        // Generate embeddings for each chunk
        foreach (var chunk in chunks)
        {
            var embedding = await _embedding.GenerateEmbeddingAsync(chunk.Text, cancellationToken);

            // Publish embedding generation message for storage
            await _publishEndpoint.Publish(new EmbeddingGenerationMessage
            {
                EntityId = message.EntityId,
                EntityType = message.EntityType,
                ChunkIndex = chunk.Index,
                ChunkText = chunk.Text,
                Model = "mock-embedding-v1"
            }, cancellationToken);

            Logger.LogDebug(
                "Generated embedding for chunk {ChunkIndex} of {EntityId}",
                chunk.Index,
                message.EntityId);
        }

        Logger.LogInformation(
            "Completed AI ingestion for {EntityType} {EntityId}, {ChunkCount} embeddings generated",
            message.EntityType,
            message.EntityId,
            chunks.Count);

        // TODO: Publish IngestionCompletedEvent
    }
}
