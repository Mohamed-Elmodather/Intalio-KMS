using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.AI.Application.DTOs;
using AFC27.KMS.AI.Domain.Entities;

namespace AFC27.KMS.AI.Presentation.Controllers;

/// <summary>
/// Controller for audio/video transcription
/// </summary>
[ApiController]
[Route("api/ai/transcription")]
[Authorize]
public class TranscriptionController : ControllerBase
{
    /// <summary>
    /// Create transcription job
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AIJobDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AIJobDto>> CreateTranscription([FromBody] TranscriptionRequest request)
    {
        // TODO: Create transcription job
        var job = new AIJobDto
        {
            Id = Guid.NewGuid(),
            Type = AIJobType.AudioTranscription,
            TypeName = "Audio Transcription",
            Status = AIJobStatus.Queued,
            StatusName = "Queued",
            Provider = AIProvider.IntalioAI,
            SourceEntityId = request.MediaId,
            CreatedAt = DateTime.UtcNow
        };
        return Created($"/api/ai/jobs/{job.Id}", job);
    }

    /// <summary>
    /// Get transcription result
    /// </summary>
    [HttpGet("{jobId:guid}")]
    [ProducesResponseType(typeof(TranscriptionResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TranscriptionResultDto>> GetTranscription(Guid jobId)
    {
        // TODO: Return transcription result
        var result = new TranscriptionResultDto
        {
            Id = Guid.NewGuid(),
            JobId = jobId,
            SourceMediaId = Guid.NewGuid(),
            FullText = "Welcome everyone to today's meeting. We'll be discussing the tournament preparations...",
            FullTextAr = "مرحباً بالجميع في اجتماع اليوم. سنناقش استعدادات البطولة...",
            DetectedLanguage = "en",
            ConfidenceScore = 0.95,
            Duration = TimeSpan.FromMinutes(45),
            HasSpeakerDiarization = true,
            HasTimestamps = true,
            Segments = new List<TranscriptionSegmentDto>
            {
                new()
                {
                    SequenceNumber = 1,
                    StartTime = "00:00:00",
                    EndTime = "00:00:05",
                    Text = "Welcome everyone to today's meeting.",
                    SpeakerId = "speaker_1",
                    SpeakerName = "Ahmed",
                    ConfidenceScore = 0.97
                },
                new()
                {
                    SequenceNumber = 2,
                    StartTime = "00:00:05",
                    EndTime = "00:00:12",
                    Text = "We'll be discussing the tournament preparations.",
                    SpeakerId = "speaker_1",
                    SpeakerName = "Ahmed",
                    ConfidenceScore = 0.94
                }
            },
            Speakers = new List<TranscriptionSpeakerDto>
            {
                new()
                {
                    SpeakerId = "speaker_1",
                    SpeakerName = "Ahmed",
                    TotalSpeakingTime = "00:25:30",
                    SegmentCount = 45
                },
                new()
                {
                    SpeakerId = "speaker_2",
                    SpeakerName = "Sara",
                    TotalSpeakingTime = "00:15:20",
                    SegmentCount = 28
                }
            },
            CreatedAt = DateTime.UtcNow
        };
        return Ok(result);
    }

    /// <summary>
    /// Get transcription by media ID
    /// </summary>
    [HttpGet("media/{mediaId:guid}")]
    [ProducesResponseType(typeof(TranscriptionResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TranscriptionResultDto>> GetTranscriptionByMedia(Guid mediaId)
    {
        // TODO: Return transcription by media ID
        return NotFound();
    }

    /// <summary>
    /// Update transcription (manual corrections)
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TranscriptionResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TranscriptionResultDto>> UpdateTranscription(
        Guid id,
        [FromBody] UpdateTranscriptionRequest request)
    {
        // TODO: Update transcription
        return Ok(new TranscriptionResultDto { Id = id });
    }

    /// <summary>
    /// Update speaker identification
    /// </summary>
    [HttpPut("{id:guid}/speakers")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateSpeakers(
        Guid id,
        [FromBody] UpdateSpeakersRequest request)
    {
        // TODO: Update speaker identification
        return NoContent();
    }

    /// <summary>
    /// Export transcription
    /// </summary>
    [HttpGet("{id:guid}/export")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportTranscription(
        Guid id,
        [FromQuery] string format = "txt",
        [FromQuery] bool includeTimestamps = true,
        [FromQuery] bool includeSpeakers = true)
    {
        // TODO: Export transcription
        var content = "Transcription export...";
        var contentType = format switch
        {
            "srt" => "application/x-subrip",
            "vtt" => "text/vtt",
            "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "json" => "application/json",
            _ => "text/plain"
        };
        return File(System.Text.Encoding.UTF8.GetBytes(content), contentType, $"transcription.{format}");
    }

    /// <summary>
    /// Translate transcription
    /// </summary>
    [HttpPost("{id:guid}/translate")]
    [ProducesResponseType(typeof(TranscriptionResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TranscriptionResultDto>> TranslateTranscription(
        Guid id,
        [FromQuery] string targetLanguage)
    {
        // TODO: Translate transcription
        return Ok(new TranscriptionResultDto { Id = id });
    }

    /// <summary>
    /// Get supported audio/video formats
    /// </summary>
    [HttpGet("formats")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public ActionResult<object> GetSupportedFormats()
    {
        var formats = new
        {
            Audio = new[] { "mp3", "wav", "m4a", "ogg", "flac", "aac" },
            Video = new[] { "mp4", "webm", "mov", "avi", "mkv" },
            MaxFileSizeMB = 500,
            MaxDurationMinutes = 180,
            SupportedLanguages = new[]
            {
                new { Code = "ar", Name = "Arabic", NameAr = "العربية" },
                new { Code = "en", Name = "English", NameAr = "الإنجليزية" },
                new { Code = "fr", Name = "French", NameAr = "الفرنسية" }
            }
        };
        return Ok(formats);
    }
}

/// <summary>
/// Update transcription request
/// </summary>
public class UpdateTranscriptionRequest
{
    public string? FullText { get; set; }
    public List<TranscriptionSegmentDto>? Segments { get; set; }
}

/// <summary>
/// Update speakers request
/// </summary>
public class UpdateSpeakersRequest
{
    public List<SpeakerMapping> Speakers { get; set; } = new();
}

/// <summary>
/// Speaker mapping
/// </summary>
public class SpeakerMapping
{
    public string SpeakerId { get; set; } = string.Empty;
    public string? SpeakerName { get; set; }
    public Guid? UserId { get; set; }
}
