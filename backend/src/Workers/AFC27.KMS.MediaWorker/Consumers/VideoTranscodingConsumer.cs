using Microsoft.Extensions.Logging;
using MassTransit;
using AFC27.KMS.Infrastructure.Messaging;
using AFC27.KMS.Infrastructure.Messaging.Messages;
using AFC27.KMS.MediaWorker.Services;

namespace AFC27.KMS.MediaWorker.Consumers;

/// <summary>
/// Consumer for processing video transcoding requests.
/// </summary>
public class VideoTranscodingConsumer : BaseConsumer<TranscodingRequestMessage>
{
    private readonly FFmpegService _ffmpegService;

    public VideoTranscodingConsumer(
        FFmpegService ffmpegService,
        ILogger<VideoTranscodingConsumer> logger)
        : base(logger)
    {
        _ffmpegService = ffmpegService;
    }

    protected override async Task ProcessMessageAsync(
        TranscodingRequestMessage message,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation(
            "Processing video transcoding for {MediaItemId} to {Format}",
            message.MediaItemId,
            message.TargetFormat);

        var formatString = message.TargetFormat.ToString();

        var transcodedPath = await _ffmpegService.TranscodeVideoAsync(
            message.SourcePath,
            formatString,
            message.Resolution,
            message.Bitrate,
            cancellationToken);

        Logger.LogInformation(
            "Video transcoded for {MediaItemId}: {Path}",
            message.MediaItemId,
            transcodedPath);

        // Generate thumbnail for the transcoded video
        var thumbnailPath = await _ffmpegService.GenerateVideoThumbnailAsync(
            transcodedPath,
            256,
            256,
            cancellationToken: cancellationToken);

        Logger.LogInformation(
            "Thumbnail generated for transcoded video {MediaItemId}: {Path}",
            message.MediaItemId,
            thumbnailPath);

        // TODO: Publish TranscodingCompletedEvent to update the media item
    }
}
