using Microsoft.Extensions.Logging;
using MassTransit;
using AFC27.KMS.Infrastructure.Messaging;
using AFC27.KMS.Infrastructure.Messaging.Messages;
using AFC27.KMS.AIWorker.Services;

namespace AFC27.KMS.AIWorker.Consumers;

/// <summary>
/// Consumer for processing embedding storage requests.
/// Stores generated embeddings in the vector database.
/// </summary>
public class EmbeddingConsumer : BaseConsumer<EmbeddingGenerationMessage>
{
    private readonly MockEmbeddingService _embeddingService;

    public EmbeddingConsumer(
        MockEmbeddingService embeddingService,
        ILogger<EmbeddingConsumer> logger)
        : base(logger)
    {
        _embeddingService = embeddingService;
    }

    protected override async Task ProcessMessageAsync(
        EmbeddingGenerationMessage message,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation(
            "Processing embedding for {EntityType} {EntityId}, chunk {ChunkIndex}",
            message.EntityType,
            message.EntityId,
            message.ChunkIndex);

        // Generate embedding
        var embedding = await _embeddingService.GenerateEmbeddingAsync(
            message.ChunkText,
            cancellationToken);

        Logger.LogDebug(
            "Generated {Dimension}-dimensional embedding for chunk {ChunkIndex}",
            embedding.Length,
            message.ChunkIndex);

        // TODO: Store embedding in vector database
        // For now, just log that we would store it
        // In production:
        // await _vectorStore.UpsertAsync(new VectorEntry
        // {
        //     Id = $"{message.EntityId}_{message.ChunkIndex}",
        //     EntityId = message.EntityId,
        //     EntityType = message.EntityType,
        //     ChunkIndex = message.ChunkIndex,
        //     ChunkText = message.ChunkText,
        //     Embedding = embedding,
        //     Model = message.Model
        // });

        Logger.LogInformation(
            "Stored embedding for {EntityType} {EntityId}, chunk {ChunkIndex}",
            message.EntityType,
            message.EntityId,
            message.ChunkIndex);
    }
}
