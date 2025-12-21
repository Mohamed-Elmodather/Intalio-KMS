using Microsoft.Extensions.Logging;
using MassTransit;
using AFC27.KMS.Infrastructure.Messaging;
using AFC27.KMS.Infrastructure.Messaging.Messages;
using AFC27.KMS.MediaWorker.Services;

namespace AFC27.KMS.MediaWorker.Consumers;

/// <summary>
/// Consumer for processing thumbnail generation requests.
/// </summary>
public class ThumbnailGenerationConsumer : BaseConsumer<ThumbnailGenerationMessage>
{
    private readonly ImageProcessingService _imageService;
    private readonly FFmpegService _ffmpegService;

    public ThumbnailGenerationConsumer(
        ImageProcessingService imageService,
        FFmpegService ffmpegService,
        ILogger<ThumbnailGenerationConsumer> logger)
        : base(logger)
    {
        _imageService = imageService;
        _ffmpegService = ffmpegService;
    }

    protected override async Task ProcessMessageAsync(
        ThumbnailGenerationMessage message,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation(
            "Processing thumbnail generation for {EntityType} {EntityId}",
            message.EntityType,
            message.EntityId);

        string thumbnailPath;

        try
        {
            thumbnailPath = message.ContentType.ToLowerInvariant() switch
            {
                var ct when ct.StartsWith("image/") =>
                    await _imageService.GenerateThumbnailAsync(
                        message.SourcePath,
                        message.Width,
                        message.Height,
                        cancellationToken),

                var ct when ct.StartsWith("video/") =>
                    await _ffmpegService.GenerateVideoThumbnailAsync(
                        message.SourcePath,
                        message.Width,
                        message.Height,
                        cancellationToken: cancellationToken),

                "application/pdf" =>
                    await _imageService.GeneratePdfThumbnailAsync(
                        message.SourcePath,
                        message.Width,
                        message.Height,
                        cancellationToken),

                _ => throw new NotSupportedException(
                    $"Thumbnail generation not supported for content type: {message.ContentType}")
            };

            Logger.LogInformation(
                "Thumbnail generated for {EntityType} {EntityId}: {Path}",
                message.EntityType,
                message.EntityId,
                thumbnailPath);

            // TODO: Publish ThumbnailGeneratedEvent to update the entity with thumbnail path
        }
        catch (NotSupportedException ex)
        {
            Logger.LogWarning(ex,
                "Skipping unsupported content type {ContentType} for {EntityId}",
                message.ContentType,
                message.EntityId);
        }
    }
}
