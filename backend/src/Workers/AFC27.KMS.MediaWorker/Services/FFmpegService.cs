using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AFC27.KMS.Infrastructure.Storage;

namespace AFC27.KMS.MediaWorker.Services;

/// <summary>
/// Service for video processing operations using FFmpeg.
/// </summary>
public class FFmpegService
{
    private readonly IStorageService _storageService;
    private readonly FFmpegOptions _options;
    private readonly ILogger<FFmpegService> _logger;

    public FFmpegService(
        IStorageService storageService,
        IOptions<FFmpegOptions> options,
        ILogger<FFmpegService> logger)
    {
        _storageService = storageService;
        _options = options?.Value ?? new FFmpegOptions();
        _logger = logger;
    }

    /// <summary>
    /// Generates a thumbnail from a video file.
    /// </summary>
    public async Task<string> GenerateVideoThumbnailAsync(
        string sourcePath,
        int width,
        int height,
        int timeOffsetSeconds = 5,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Generating video thumbnail for {Source} at {Offset}s",
            sourcePath, timeOffsetSeconds);

        var outputFileName = $"{Guid.NewGuid():N}.jpg";
        var outputPath = Path.Combine(_options.TempPath, outputFileName);

        // Ensure temp directory exists
        Directory.CreateDirectory(_options.TempPath);

        // FFmpeg command to extract a frame
        var arguments = $"-i \"{sourcePath}\" -ss {timeOffsetSeconds} -vframes 1 " +
                       $"-vf \"scale={width}:{height}:force_original_aspect_ratio=decrease\" " +
                       $"-q:v 2 \"{outputPath}\"";

        await RunFFmpegAsync(arguments, cancellationToken);

        if (!File.Exists(outputPath))
        {
            throw new InvalidOperationException("FFmpeg failed to generate thumbnail");
        }

        // Upload to storage
        await using var fileStream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
        var storagePath = await _storageService.UploadFileAsync(
            fileStream, outputFileName, "image/jpeg", "thumbnails");

        // Clean up temp file
        File.Delete(outputPath);

        _logger.LogInformation("Video thumbnail generated: {Path}", storagePath);
        return storagePath;
    }

    /// <summary>
    /// Transcodes a video to the specified format.
    /// </summary>
    public async Task<string> TranscodeVideoAsync(
        string sourcePath,
        string targetFormat,
        string? resolution = null,
        int? bitrate = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Transcoding video {Source} to {Format}",
            sourcePath, targetFormat);

        var outputFileName = $"{Guid.NewGuid():N}.{GetExtension(targetFormat)}";
        var outputPath = Path.Combine(_options.TempPath, outputFileName);

        Directory.CreateDirectory(_options.TempPath);

        var arguments = BuildTranscodeArguments(sourcePath, outputPath, targetFormat, resolution, bitrate);

        await RunFFmpegAsync(arguments, cancellationToken);

        if (!File.Exists(outputPath))
        {
            throw new InvalidOperationException("FFmpeg failed to transcode video");
        }

        // Upload to storage
        await using var fileStream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
        var contentType = GetContentType(targetFormat);
        var storagePath = await _storageService.UploadFileAsync(
            fileStream, outputFileName, contentType, "transcoded");

        // Clean up temp file
        File.Delete(outputPath);

        _logger.LogInformation("Video transcoded: {Path}", storagePath);
        return storagePath;
    }

    /// <summary>
    /// Extracts audio from a video file.
    /// </summary>
    public async Task<string> ExtractAudioAsync(
        string sourcePath,
        string outputFormat = "mp3",
        int? bitrate = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Extracting audio from {Source}", sourcePath);

        var outputFileName = $"{Guid.NewGuid():N}.{outputFormat}";
        var outputPath = Path.Combine(_options.TempPath, outputFileName);

        Directory.CreateDirectory(_options.TempPath);

        var bitrateArg = bitrate.HasValue ? $"-b:a {bitrate}k" : "";
        var arguments = $"-i \"{sourcePath}\" -vn -acodec libmp3lame {bitrateArg} \"{outputPath}\"";

        await RunFFmpegAsync(arguments, cancellationToken);

        if (!File.Exists(outputPath))
        {
            throw new InvalidOperationException("FFmpeg failed to extract audio");
        }

        await using var fileStream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
        var storagePath = await _storageService.UploadFileAsync(
            fileStream, outputFileName, "audio/mpeg", "audio");

        File.Delete(outputPath);

        _logger.LogInformation("Audio extracted: {Path}", storagePath);
        return storagePath;
    }

    /// <summary>
    /// Gets video metadata (duration, resolution, codec, etc.).
    /// </summary>
    public async Task<VideoMetadata> GetVideoMetadataAsync(
        string sourcePath,
        CancellationToken cancellationToken = default)
    {
        var arguments = $"-i \"{sourcePath}\" -hide_banner";

        try
        {
            var output = await RunFFprobeAsync(arguments, cancellationToken);
            return ParseVideoMetadata(output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get video metadata for {Path}", sourcePath);
            return new VideoMetadata();
        }
    }

    private async Task RunFFmpegAsync(string arguments, CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = _options.FFmpegPath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _logger.LogDebug("Running FFmpeg: {Arguments}", arguments);

        using var process = new Process { StartInfo = startInfo };
        process.Start();

        var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            var error = await errorTask;
            _logger.LogError("FFmpeg error: {Error}", error);
            throw new InvalidOperationException($"FFmpeg failed with exit code {process.ExitCode}: {error}");
        }
    }

    private async Task<string> RunFFprobeAsync(string arguments, CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = _options.FFprobePath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();

        var output = await process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);

        return output;
    }

    private static string BuildTranscodeArguments(
        string input,
        string output,
        string format,
        string? resolution,
        int? bitrate)
    {
        var args = $"-i \"{input}\" ";

        // Video codec based on format
        args += format.ToLowerInvariant() switch
        {
            "mp4h264" => "-c:v libx264 -preset medium -crf 23 ",
            "mp4h265" => "-c:v libx265 -preset medium -crf 28 ",
            "webm" => "-c:v libvpx-vp9 -crf 30 -b:v 0 ",
            _ => "-c:v libx264 -preset medium -crf 23 "
        };

        // Resolution
        if (!string.IsNullOrEmpty(resolution))
        {
            args += $"-vf scale={resolution} ";
        }

        // Bitrate
        if (bitrate.HasValue)
        {
            args += $"-b:v {bitrate}k ";
        }

        // Audio codec
        args += "-c:a aac -b:a 128k ";

        args += $"\"{output}\"";
        return args;
    }

    private static string GetExtension(string format) => format.ToLowerInvariant() switch
    {
        "mp4h264" or "mp4h265" => "mp4",
        "webm" => "webm",
        _ => "mp4"
    };

    private static string GetContentType(string format) => format.ToLowerInvariant() switch
    {
        "mp4h264" or "mp4h265" => "video/mp4",
        "webm" => "video/webm",
        _ => "video/mp4"
    };

    private static VideoMetadata ParseVideoMetadata(string ffprobeOutput)
    {
        // Basic parsing - in production use JSON output from ffprobe
        var metadata = new VideoMetadata();

        // Parse duration
        var durationMatch = System.Text.RegularExpressions.Regex.Match(
            ffprobeOutput, @"Duration: (\d{2}):(\d{2}):(\d{2})\.(\d{2})");
        if (durationMatch.Success)
        {
            metadata.Duration = TimeSpan.FromHours(int.Parse(durationMatch.Groups[1].Value))
                + TimeSpan.FromMinutes(int.Parse(durationMatch.Groups[2].Value))
                + TimeSpan.FromSeconds(int.Parse(durationMatch.Groups[3].Value));
        }

        // Parse resolution
        var resolutionMatch = System.Text.RegularExpressions.Regex.Match(
            ffprobeOutput, @"(\d{3,4})x(\d{3,4})");
        if (resolutionMatch.Success)
        {
            metadata.Width = int.Parse(resolutionMatch.Groups[1].Value);
            metadata.Height = int.Parse(resolutionMatch.Groups[2].Value);
        }

        return metadata;
    }
}

/// <summary>
/// FFmpeg configuration options.
/// </summary>
public class FFmpegOptions
{
    public const string SectionName = "FFmpeg";

    public string FFmpegPath { get; set; } = "ffmpeg";
    public string FFprobePath { get; set; } = "ffprobe";
    public string TempPath { get; set; } = "/tmp/ffmpeg";
}

/// <summary>
/// Video metadata information.
/// </summary>
public class VideoMetadata
{
    public TimeSpan Duration { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string? Codec { get; set; }
    public string? AudioCodec { get; set; }
    public int? Bitrate { get; set; }
    public double? FrameRate { get; set; }
}
