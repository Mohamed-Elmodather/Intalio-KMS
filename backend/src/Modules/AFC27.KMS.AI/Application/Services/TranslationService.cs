using System.Diagnostics;
using Microsoft.Extensions.Logging;
using AFC27.KMS.AI.Application.DTOs;
using AFC27.KMS.AI.Application.Interfaces;

namespace AFC27.KMS.AI.Application.Services;

/// <summary>
/// Implementation of the translation service.
/// Uses IIntalioAIClient for LLM-based translation between EN and AR.
/// </summary>
public class TranslationService : ITranslationService
{
    private readonly IIntalioAIClient _aiClient;
    private readonly ILogger<TranslationService> _logger;

    private static class SystemPrompts
    {
        public const string TranslateText = @"You are an expert translator for the AFC Asian Cup 2027 Knowledge Management System.
Translate the provided text accurately between English and Arabic.
Maintain the original meaning, tone, and formatting.
If the text contains HTML tags, preserve them in the translation.
Use Modern Standard Arabic for Arabic translations.
Apply sports and tournament terminology where appropriate.
Return only the translated text without explanations.";

        public const string TranslateBlocks = @"You are an expert translator for the AFC Asian Cup 2027 Knowledge Management System.
Translate each content block accurately between English and Arabic.
Maintain the original meaning, tone, and formatting of each block.
If blocks contain HTML tags, preserve them in the translation.
Use Modern Standard Arabic for Arabic translations.
Apply sports and tournament terminology where appropriate.
Return the translated content for each block in the same order.";
    }

    public TranslationService(
        IIntalioAIClient aiClient,
        ILogger<TranslationService> logger)
    {
        _aiClient = aiClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TranslationResponse> TranslateArticleAsync(
        Guid articleId,
        TranslateArticleRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        _logger.LogInformation(
            "Translating article {ArticleId} to {TargetLanguage} for user {UserId}",
            articleId, request.TargetLanguage, userId);

        try
        {
            // TODO: Fetch article blocks from the content repository by articleId.
            // For now, return a placeholder response indicating the article was processed.
            var sampleBlocks = new List<ContentBlockDto>
            {
                new()
                {
                    BlockId = "block-1",
                    BlockType = "heading",
                    Content = request.TargetLanguage == "ar"
                        ? "Sample Article Heading"
                        : "عنوان المقال النموذجي"
                },
                new()
                {
                    BlockId = "block-2",
                    BlockType = "paragraph",
                    Content = request.TargetLanguage == "ar"
                        ? "This is sample article content that would be fetched from the database."
                        : "هذا هو محتوى المقال النموذجي الذي سيتم جلبه من قاعدة البيانات."
                }
            };

            var translateRequest = new TranslateBlocksRequest
            {
                Blocks = sampleBlocks,
                TargetLanguage = request.TargetLanguage,
                SourceLanguage = request.SourceLanguage,
                PreserveFormatting = request.PreserveFormatting,
                Domain = request.Domain
            };

            return await TranslateBlocksAsync(translateRequest, userId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error translating article {ArticleId}", articleId);
            sw.Stop();

            return new TranslationResponse
            {
                Success = false,
                TargetLanguage = request.TargetLanguage,
                Error = "An error occurred while translating the article.",
                ProcessingTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
    }

    /// <inheritdoc />
    public async Task<TranslationResponse> TranslateBlocksAsync(
        TranslateBlocksRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        _logger.LogInformation(
            "Translating {BlockCount} blocks to {TargetLanguage} for user {UserId}",
            request.Blocks.Count, request.TargetLanguage, userId);

        try
        {
            var translatedBlocks = new List<BlockTranslationResult>();
            var totalTokens = 0;

            foreach (var block in request.Blocks)
            {
                var domainHint = !string.IsNullOrEmpty(request.Domain)
                    ? $"\nDomain context: {request.Domain}"
                    : string.Empty;

                var userPrompt = $"Translate the following {block.BlockType} content to {GetLanguageName(request.TargetLanguage)}.{domainHint}\n\nContent:\n{block.Content}";

                var chatRequest = new ChatCompletionRequest
                {
                    Messages = new List<ChatMessage>
                    {
                        new() { Role = "system", Content = SystemPrompts.TranslateBlocks },
                        new() { Role = "user", Content = userPrompt }
                    },
                    Temperature = 0.3,
                    MaxTokens = 4096
                };

                var chatResponse = await _aiClient.ChatAsync(chatRequest, cancellationToken);

                translatedBlocks.Add(new BlockTranslationResult
                {
                    BlockId = block.BlockId,
                    BlockType = block.BlockType,
                    OriginalContent = block.Content,
                    TranslatedContent = chatResponse.Content ?? string.Empty,
                    Confidence = 0.9
                });

                totalTokens += chatResponse.TotalTokens;
            }

            sw.Stop();

            return new TranslationResponse
            {
                Success = true,
                DetectedSourceLanguage = request.SourceLanguage ?? DetectLanguage(request.Blocks.FirstOrDefault()?.Content),
                TargetLanguage = request.TargetLanguage,
                TranslatedBlocks = translatedBlocks,
                Confidence = 0.9,
                TokensUsed = totalTokens,
                ProcessingTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error translating blocks");
            sw.Stop();

            return new TranslationResponse
            {
                Success = false,
                TargetLanguage = request.TargetLanguage,
                Error = "An error occurred while translating the content blocks.",
                ProcessingTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
    }

    /// <inheritdoc />
    public async Task<TranslationResponse> TranslateTextAsync(
        TranslateTextRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        _logger.LogInformation(
            "Translating text to {TargetLanguage} for user {UserId}",
            request.TargetLanguage, userId);

        try
        {
            var domainHint = !string.IsNullOrEmpty(request.Domain)
                ? $"\nDomain context: {request.Domain}"
                : string.Empty;

            var formattingHint = request.PreserveFormatting
                ? "\nPreserve any HTML tags and formatting in the translation."
                : string.Empty;

            var userPrompt = $"Translate the following text to {GetLanguageName(request.TargetLanguage)}.{domainHint}{formattingHint}\n\nText:\n{request.Text}";

            var chatRequest = new ChatCompletionRequest
            {
                Messages = new List<ChatMessage>
                {
                    new() { Role = "system", Content = SystemPrompts.TranslateText },
                    new() { Role = "user", Content = userPrompt }
                },
                Temperature = 0.3,
                MaxTokens = 4096
            };

            var chatResponse = await _aiClient.ChatAsync(chatRequest, cancellationToken);

            sw.Stop();

            return new TranslationResponse
            {
                Success = true,
                DetectedSourceLanguage = request.SourceLanguage ?? DetectLanguage(request.Text),
                TargetLanguage = request.TargetLanguage,
                TranslatedText = chatResponse.Content ?? string.Empty,
                Confidence = 0.9,
                TokensUsed = chatResponse.TotalTokens,
                ProcessingTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error translating text");
            sw.Stop();

            return new TranslationResponse
            {
                Success = false,
                TargetLanguage = request.TargetLanguage,
                Error = "An error occurred while translating the text.",
                ProcessingTimeMs = (int)sw.ElapsedMilliseconds
            };
        }
    }

    private static string GetLanguageName(string languageCode) => languageCode.ToLowerInvariant() switch
    {
        "ar" => "Arabic (Modern Standard Arabic)",
        "en" => "English",
        "fr" => "French",
        _ => languageCode
    };

    private static string? DetectLanguage(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        // Simple heuristic: check for Arabic Unicode range characters
        var arabicCharCount = text.Count(c => c >= '\u0600' && c <= '\u06FF');
        var totalLetters = text.Count(char.IsLetter);

        if (totalLetters == 0)
            return null;

        return (double)arabicCharCount / totalLetters > 0.3 ? "ar" : "en";
    }
}
