using AFC27.KMS.AI.Domain.Entities;

namespace AFC27.KMS.AI.Application.DTOs;

/// <summary>
/// AI job DTO
/// </summary>
public class AIJobDto
{
    public Guid Id { get; set; }
    public AIJobType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public AIJobStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public AIProvider Provider { get; set; }
    public string? ModelId { get; set; }

    public Guid? SourceEntityId { get; set; }
    public string? SourceEntityType { get; set; }
    public string? InputPreview { get; set; }

    public string? OutputPreview { get; set; }
    public string? OutputFileUrl { get; set; }

    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? ProcessingTimeMs { get; set; }
    public int? TokensUsed { get; set; }

    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }

    public Guid RequestedById { get; set; }
    public string? RequestedByName { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Create AI job request
/// </summary>
public class CreateAIJobRequest
{
    public AIJobType Type { get; set; }
    public AIProvider? Provider { get; set; }
    public string? ModelId { get; set; }

    public Guid? SourceEntityId { get; set; }
    public string? SourceEntityType { get; set; }
    public string? InputText { get; set; }
    public string? InputFileUrl { get; set; }
    public string? InputLanguage { get; set; }

    public string? OutputLanguage { get; set; }
    public string? OutputFormat { get; set; }

    public Dictionary<string, object>? Parameters { get; set; }
    public string? CallbackUrl { get; set; }
    public bool WaitForCompletion { get; set; }
}

/// <summary>
/// Transcription request
/// </summary>
public class TranscriptionRequest
{
    public Guid MediaId { get; set; }
    public string? MediaUrl { get; set; }
    public string? MediaType { get; set; }

    public bool EnableSpeakerDiarization { get; set; }
    public bool EnableWordTimestamps { get; set; }
    public bool EnablePunctuation { get; set; } = true;
    public bool EnableProfanityFilter { get; set; }

    public string? Language { get; set; }
    public List<string>? ExpectedSpeakers { get; set; }
    public string? VocabularyHints { get; set; }

    public bool AutoTranslate { get; set; }
    public string? TranslateToLanguage { get; set; }
    public string? CallbackUrl { get; set; }
}

/// <summary>
/// Transcription result DTO
/// </summary>
public class TranscriptionResultDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Guid SourceMediaId { get; set; }

    public string FullText { get; set; } = string.Empty;
    public string? FullTextAr { get; set; }
    public string DetectedLanguage { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public TimeSpan Duration { get; set; }

    public List<TranscriptionSegmentDto> Segments { get; set; } = new();
    public List<TranscriptionSpeakerDto> Speakers { get; set; } = new();

    public bool HasSpeakerDiarization { get; set; }
    public bool HasTimestamps { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Transcription segment DTO
/// </summary>
public class TranscriptionSegmentDto
{
    public int SequenceNumber { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string? SpeakerId { get; set; }
    public string? SpeakerName { get; set; }
    public double ConfidenceScore { get; set; }
}

/// <summary>
/// Transcription speaker DTO
/// </summary>
public class TranscriptionSpeakerDto
{
    public string SpeakerId { get; set; } = string.Empty;
    public string? SpeakerName { get; set; }
    public Guid? UserId { get; set; }
    public string TotalSpeakingTime { get; set; } = string.Empty;
    public int SegmentCount { get; set; }
}

/// <summary>
/// Summarization request
/// </summary>
public class SummarizationRequest
{
    public Guid? DocumentId { get; set; }
    public string? Text { get; set; }
    public string? TextUrl { get; set; }
    public string? SourceType { get; set; }

    public SummaryLength Length { get; set; } = SummaryLength.Medium;
    public string? TargetLanguage { get; set; }
    public bool ExtractKeyPoints { get; set; } = true;
    public bool ExtractKeywords { get; set; } = true;
    public bool ExtractEntities { get; set; }
    public int? MaxSentences { get; set; }
    public int? MaxWords { get; set; }
    public string? FocusArea { get; set; }
    public string? CallbackUrl { get; set; }
}

/// <summary>
/// Document summary DTO
/// </summary>
public class DocumentSummaryDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Guid SourceDocumentId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string SummaryAr { get; set; } = string.Empty;
    public string? ExecutiveSummary { get; set; }
    public string? ExecutiveSummaryAr { get; set; }

    public List<string> KeyPoints { get; set; } = new();
    public List<string> KeyPointsAr { get; set; } = new();
    public List<string> Keywords { get; set; } = new();
    public List<ExtractedEntityDto> Entities { get; set; } = new();
    public List<string> Topics { get; set; } = new();

    public SummaryLength Length { get; set; }
    public int WordCount { get; set; }
    public int OriginalWordCount { get; set; }
    public double CompressionRatio { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Extracted entity DTO
/// </summary>
public class ExtractedEntityDto
{
    public string Text { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? NormalizedValue { get; set; }
    public double ConfidenceScore { get; set; }
}

/// <summary>
/// Meeting minutes generation request
/// </summary>
public class GenerateMeetingMinutesRequest
{
    public Guid? EventId { get; set; }
    public Guid? RecordingId { get; set; }
    public Guid? TranscriptionId { get; set; }

    public string? MeetingTitle { get; set; }
    public DateTime? MeetingDate { get; set; }
    public string? MeetingLocation { get; set; }

    public List<MeetingParticipantInput>? Participants { get; set; }
    public List<string>? AgendaItems { get; set; }

    public bool ExtractActionItems { get; set; } = true;
    public bool ExtractDecisions { get; set; } = true;
    public bool GenerateExecutiveSummary { get; set; } = true;
    public string? TargetLanguage { get; set; }
    public string? Template { get; set; }
    public string? CallbackUrl { get; set; }
}

/// <summary>
/// Meeting participant input
/// </summary>
public class MeetingParticipantInput
{
    public Guid? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Role { get; set; }
}

/// <summary>
/// Meeting minutes DTO
/// </summary>
public class MeetingMinutesDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public DateTime MeetingDate { get; set; }
    public string Duration { get; set; } = string.Empty;
    public string? Location { get; set; }

    public List<MeetingParticipantDto> Participants { get; set; } = new();
    public List<MeetingAgendaItemDto> AgendaItems { get; set; } = new();
    public List<MeetingActionItemDto> ActionItems { get; set; } = new();
    public List<MeetingDecisionDto> Decisions { get; set; } = new();

    public string? ExecutiveSummary { get; set; }
    public string? ExecutiveSummaryAr { get; set; }
    public string? DetailedNotes { get; set; }
    public string? DetailedNotesAr { get; set; }

    public string Status { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Meeting participant DTO
/// </summary>
public class MeetingParticipantDto
{
    public Guid? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string ParticipantType { get; set; } = string.Empty;
    public bool WasPresent { get; set; }
}

/// <summary>
/// Meeting agenda item DTO
/// </summary>
public class MeetingAgendaItemDto
{
    public int SequenceNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string? Discussion { get; set; }
    public string? DiscussionAr { get; set; }
    public string? Presenter { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Meeting action item DTO
/// </summary>
public class MeetingActionItemDto
{
    public string Description { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public Guid? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public DateTime? DueDate { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Meeting decision DTO
/// </summary>
public class MeetingDecisionDto
{
    public string Description { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? Context { get; set; }
    public List<string>? ApprovedBy { get; set; }
}

/// <summary>
/// Content classification request
/// </summary>
public class ClassifyContentRequest
{
    public Guid? EntityId { get; set; }
    public string? EntityType { get; set; }
    public string? Text { get; set; }
    public string? Title { get; set; }

    public bool ClassifyTopics { get; set; } = true;
    public bool SuggestCategories { get; set; } = true;
    public bool SuggestTags { get; set; } = true;
    public bool AnalyzeSentiment { get; set; }
    public bool CheckContentSafety { get; set; }
    public bool DetectLanguage { get; set; } = true;

    public List<string>? AvailableCategories { get; set; }
    public List<string>? AvailableTags { get; set; }
}

/// <summary>
/// Content classification result DTO
/// </summary>
public class ContentClassificationDto
{
    public Guid Id { get; set; }
    public Guid SourceEntityId { get; set; }
    public string SourceEntityType { get; set; } = string.Empty;

    public List<ClassificationLabelDto> Labels { get; set; } = new();
    public List<string> SuggestedCategories { get; set; } = new();
    public List<string> SuggestedTags { get; set; } = new();
    public string? DetectedLanguage { get; set; }
    public SentimentResultDto? Sentiment { get; set; }
    public ContentSafetyResultDto? ContentSafety { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Classification label DTO
/// </summary>
public class ClassificationLabelDto
{
    public string Label { get; set; } = string.Empty;
    public string? LabelAr { get; set; }
    public double ConfidenceScore { get; set; }
    public string? Category { get; set; }
}

/// <summary>
/// Sentiment result DTO
/// </summary>
public class SentimentResultDto
{
    public string Overall { get; set; } = string.Empty;
    public double PositiveScore { get; set; }
    public double NeutralScore { get; set; }
    public double NegativeScore { get; set; }
    public double ConfidenceScore { get; set; }
}

/// <summary>
/// Content safety result DTO
/// </summary>
public class ContentSafetyResultDto
{
    public bool IsSafe { get; set; }
    public double HateSpeechScore { get; set; }
    public double ViolenceScore { get; set; }
    public double SexualContentScore { get; set; }
    public double SelfHarmScore { get; set; }
    public List<string>? FlaggedContent { get; set; }
    public List<string>? Recommendations { get; set; }
}

/// <summary>
/// Translation request
/// </summary>
public class TranslationRequest
{
    public string Text { get; set; } = string.Empty;
    public string? SourceLanguage { get; set; }
    public string TargetLanguage { get; set; } = string.Empty;
    public bool PreserveFormatting { get; set; } = true;
    public string? Domain { get; set; }
    public string? Glossary { get; set; }
}

/// <summary>
/// Translation result DTO
/// </summary>
public class TranslationResultDto
{
    public string SourceText { get; set; } = string.Empty;
    public string TranslatedText { get; set; } = string.Empty;
    public string SourceLanguage { get; set; } = string.Empty;
    public string TargetLanguage { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public int TokensUsed { get; set; }
}

/// <summary>
/// Text generation request
/// </summary>
public class TextGenerationRequest
{
    public string Prompt { get; set; } = string.Empty;
    public string? SystemPrompt { get; set; }
    public GenerationType Type { get; set; }

    public string? Language { get; set; }
    public string? Tone { get; set; }
    public int? MaxTokens { get; set; }
    public double? Temperature { get; set; }
    public string? Format { get; set; }

    public Dictionary<string, object>? Context { get; set; }
    public Guid? TemplateId { get; set; }
}

/// <summary>
/// Generation type
/// </summary>
public enum GenerationType
{
    FreeForm,
    Article,
    Summary,
    Email,
    Report,
    SocialPost,
    Description,
    FAQ,
    Custom
}

/// <summary>
/// Text generation result DTO
/// </summary>
public class TextGenerationResultDto
{
    public string GeneratedText { get; set; } = string.Empty;
    public int TokensUsed { get; set; }
    public string? Language { get; set; }
    public int ProcessingTimeMs { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// AI usage statistics DTO
/// </summary>
public class AIUsageStatsDto
{
    public int TotalJobs { get; set; }
    public int CompletedJobs { get; set; }
    public int FailedJobs { get; set; }
    public int PendingJobs { get; set; }

    public int TotalTokensUsed { get; set; }
    public int TotalMinutesProcessed { get; set; }
    public decimal TotalCost { get; set; }

    public Dictionary<string, int> JobsByType { get; set; } = new();
    public Dictionary<string, int> JobsByProvider { get; set; } = new();
    public List<DailyAIUsageDto> DailyUsage { get; set; } = new();
}

/// <summary>
/// Daily AI usage DTO
/// </summary>
public class DailyAIUsageDto
{
    public DateTime Date { get; set; }
    public int JobCount { get; set; }
    public int TokensUsed { get; set; }
    public decimal Cost { get; set; }
}

/// <summary>
/// AI quota status DTO
/// </summary>
public class AIQuotaStatusDto
{
    public AIJobType JobType { get; set; }
    public string Period { get; set; } = string.Empty;

    public int MaxRequests { get; set; }
    public int UsedRequests { get; set; }
    public int RemainingRequests { get; set; }
    public double UsagePercentage { get; set; }

    public int? MaxTokens { get; set; }
    public int? UsedTokens { get; set; }
    public decimal? MaxCost { get; set; }
    public decimal? UsedCost { get; set; }

    public int? MaxMinutes { get; set; }
    public int? UsedMinutes { get; set; }

    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public bool IsExceeded { get; set; }
}

/// <summary>
/// AI model info DTO
/// </summary>
public class AIModelInfoDto
{
    public string ModelId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string SupportedJobType { get; set; } = string.Empty;

    public List<string> SupportedLanguages { get; set; } = new();
    public List<string> SupportedFormats { get; set; } = new();

    public int? MaxTokens { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}
