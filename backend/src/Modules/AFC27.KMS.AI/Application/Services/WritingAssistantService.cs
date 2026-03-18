using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Logging;
using AFC27.KMS.AI.Application.DTOs;
using AFC27.KMS.AI.Application.Interfaces;

namespace AFC27.KMS.AI.Application.Services;

/// <summary>
/// Implementation of the writing assistant service.
/// Uses IIntalioAIClient for LLM calls and builds appropriate system prompts for each operation.
/// </summary>
public class WritingAssistantService : IWritingAssistantService
{
    private readonly IIntalioAIClient _aiClient;
    private readonly ILogger<WritingAssistantService> _logger;

    // System prompts for each operation type
    private static class SystemPrompts
    {
        public const string Suggest = @"You are an expert writing assistant for the AFC Asian Cup 2027 Knowledge Management System.
Analyze the provided text and suggest specific improvements. For each suggestion, provide:
- The original text segment
- Your suggested replacement
- The reason for the change
- The category (grammar, clarity, style, conciseness, or consistency)

Focus on:
1. Grammar and spelling errors
2. Clarity and readability improvements
3. Professional tone consistency
4. Conciseness without losing meaning
5. Proper use of terminology

Return your response as improved text, followed by a list of specific changes you made.
If the text contains Arabic, apply Arabic-specific grammar rules.";

        public const string Improve = @"You are an expert writing editor for the AFC Asian Cup 2027 Knowledge Management System.
Improve the provided text for grammar, clarity, and structure while preserving the original meaning and intent.
Make the text more professional, clear, and well-structured.
If the text is in Arabic, apply Arabic-specific improvements.
Return only the improved text without explanations.";

        public const string Generate = @"You are a professional content writer for the AFC Asian Cup 2027 Knowledge Management System.
Generate high-quality content based on the user's prompt.
Follow any style, length, or format instructions provided.
Ensure the content is professional, accurate, and well-structured.
If writing in Arabic, use Modern Standard Arabic with proper grammar.";

        public const string Summarize = @"You are a summarization expert for the AFC Asian Cup 2027 Knowledge Management System.
Create a concise summary of the provided text that captures the key points and essential information.
Maintain the original language of the text.
If a maximum sentence count is specified, strictly adhere to it.
Focus on the most important information and actionable insights.";

        public const string Continue = @"You are a professional writing assistant for the AFC Asian Cup 2027 Knowledge Management System.
Continue writing from where the provided text ends, maintaining the same:
- Writing style and tone
- Language (English or Arabic)
- Level of formality
- Subject matter focus

Produce a natural continuation that flows seamlessly from the existing text.
If direction is provided, follow it while maintaining consistency with the existing content.";

        public static string ChangeTone(WritingTone tone) => tone switch
        {
            WritingTone.Formal => @"You are a professional editor. Rewrite the provided text in a formal, professional tone suitable for official documents, reports, and formal communications. Use proper titles, avoid contractions, and maintain a respectful, authoritative voice. Preserve the original meaning and all factual content.",
            WritingTone.Casual => @"You are a friendly editor. Rewrite the provided text in a casual, conversational tone suitable for informal communications, blog posts, and team updates. Use simpler language, shorter sentences, and a warm, approachable voice. Preserve the original meaning and all factual content.",
            WritingTone.Simplified => @"You are a plain language expert. Rewrite the provided text using simplified language that is easy to understand for a broad audience. Use short sentences, common words, and avoid jargon or technical terms. Aim for a reading level accessible to non-native speakers. Preserve the original meaning and all factual content.",
            WritingTone.Technical => @"You are a technical writer. Rewrite the provided text in a precise, technical tone suitable for technical documentation and specifications. Use accurate terminology, detailed descriptions, and structured formatting. Preserve the original meaning and all factual content.",
            WritingTone.Diplomatic => @"You are a diplomatic communications specialist. Rewrite the provided text in a balanced, diplomatic tone suitable for sensitive communications. Use inclusive language, acknowledge multiple perspectives, and maintain a neutral, respectful voice. Preserve the original meaning and all factual content.",
            WritingTone.Persuasive => @"You are a persuasive writer. Rewrite the provided text in a compelling, persuasive tone that encourages action or agreement. Use strong language, clear benefits, and logical arguments. Preserve the original meaning and all factual content.",
            _ => @"Rewrite the provided text in a professional tone. Preserve the original meaning and all factual content."
        };

        public static string Translate(string targetLang, string? domain) =>
            $@"You are an expert translator specializing in {(domain ?? "general")} content for the AFC Asian Cup 2027.
Translate the provided text to {GetLanguageName(targetLang)}.
Maintain the original formatting, structure, and meaning.
Use natural, fluent {GetLanguageName(targetLang)} that reads as if originally written in that language.
For Arabic translations, use Modern Standard Arabic unless the context suggests a specific dialect.
For English translations from Arabic, preserve cultural nuances appropriately.
Do not add explanations or notes - return only the translated text.";

        private static string GetLanguageName(string code) => code?.ToLower() switch
        {
            "ar" => "Arabic",
            "en" => "English",
            "fr" => "French",
            _ => code ?? "the target language"
        };
    }

    public WritingAssistantService(
        IIntalioAIClient aiClient,
        ILogger<WritingAssistantService> logger)
    {
        _aiClient = aiClient;
        _logger = logger;
    }

    public async Task<AIWritingResponse> SuggestAsync(
        AIWritingRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Writing suggest request from user {UserId}, text length: {Length}",
            userId, request.Text.Length);

        var sw = Stopwatch.StartNew();

        var instruction = string.IsNullOrWhiteSpace(request.Instruction)
            ? "Analyze and suggest improvements for the following text."
            : request.Instruction;

        var userContent = BuildUserMessage(request.Text, request.Context, instruction);

        var chatResponse = await _aiClient.ChatAsync(new ChatCompletionRequest
        {
            Messages = new List<ChatMessage>
            {
                new() { Role = "system", Content = SystemPrompts.Suggest },
                new() { Role = "user", Content = userContent }
            },
            Temperature = 0.3f,
            MaxTokens = 2048
        }, cancellationToken);

        sw.Stop();

        return new AIWritingResponse
        {
            Result = chatResponse.Content,
            Confidence = 0.85,
            TokensUsed = chatResponse.TotalTokens,
            ProcessingTimeMs = (int)sw.ElapsedMilliseconds,
            SuggestedChanges = ParseSuggestedChanges(request.Text, chatResponse.Content)
        };
    }

    public async Task<AIWritingResponse> GenerateAsync(
        AIGenerateRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Writing generate request from user {UserId}, prompt length: {Length}",
            userId, request.Prompt.Length);

        var sw = Stopwatch.StartNew();

        var systemPrompt = SystemPrompts.Generate;
        var userContent = new StringBuilder();
        userContent.AppendLine(request.Prompt);

        if (!string.IsNullOrWhiteSpace(request.Style))
            userContent.AppendLine($"\nStyle: {request.Style}");
        if (request.MaxLength.HasValue)
            userContent.AppendLine($"\nMaximum length: approximately {request.MaxLength} words.");
        if (!string.IsNullOrWhiteSpace(request.Language))
            userContent.AppendLine($"\nWrite in: {request.Language}");
        if (!string.IsNullOrWhiteSpace(request.Context))
            userContent.AppendLine($"\nContext/Reference:\n{request.Context}");

        var chatResponse = await _aiClient.ChatAsync(new ChatCompletionRequest
        {
            Messages = new List<ChatMessage>
            {
                new() { Role = "system", Content = systemPrompt },
                new() { Role = "user", Content = userContent.ToString() }
            },
            Temperature = 0.7f,
            MaxTokens = request.MaxLength.HasValue ? Math.Min(request.MaxLength.Value * 2, 4096) : 2048
        }, cancellationToken);

        sw.Stop();

        return new AIWritingResponse
        {
            Result = chatResponse.Content,
            Confidence = 0.80,
            TokensUsed = chatResponse.TotalTokens,
            ProcessingTimeMs = (int)sw.ElapsedMilliseconds
        };
    }

    public async Task<AIWritingResponse> ImproveAsync(
        AIWritingRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Writing improve request from user {UserId}, text length: {Length}",
            userId, request.Text.Length);

        var sw = Stopwatch.StartNew();

        var instruction = string.IsNullOrWhiteSpace(request.Instruction)
            ? "Improve the following text for grammar, clarity, and structure."
            : request.Instruction;

        var userContent = BuildUserMessage(request.Text, request.Context, instruction);

        var chatResponse = await _aiClient.ChatAsync(new ChatCompletionRequest
        {
            Messages = new List<ChatMessage>
            {
                new() { Role = "system", Content = SystemPrompts.Improve },
                new() { Role = "user", Content = userContent }
            },
            Temperature = 0.3f,
            MaxTokens = 2048
        }, cancellationToken);

        sw.Stop();

        return new AIWritingResponse
        {
            Result = chatResponse.Content,
            Confidence = 0.90,
            TokensUsed = chatResponse.TotalTokens,
            ProcessingTimeMs = (int)sw.ElapsedMilliseconds
        };
    }

    public async Task<AIWritingResponse> ChangeToneAsync(
        AIToneChangeRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Writing tone-change request from user {UserId}, target tone: {Tone}",
            userId, request.TargetTone);

        var sw = Stopwatch.StartNew();

        var systemPrompt = SystemPrompts.ChangeTone(request.TargetTone);
        var userContent = BuildUserMessage(request.Text, request.Context);

        var chatResponse = await _aiClient.ChatAsync(new ChatCompletionRequest
        {
            Messages = new List<ChatMessage>
            {
                new() { Role = "system", Content = systemPrompt },
                new() { Role = "user", Content = userContent }
            },
            Temperature = 0.5f,
            MaxTokens = 2048
        }, cancellationToken);

        sw.Stop();

        return new AIWritingResponse
        {
            Result = chatResponse.Content,
            Confidence = 0.85,
            TokensUsed = chatResponse.TotalTokens,
            ProcessingTimeMs = (int)sw.ElapsedMilliseconds
        };
    }

    public async Task<AIWritingResponse> TranslateAsync(
        AITranslateRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Writing translate request from user {UserId}, {Source} -> {Target}",
            userId, request.SourceLanguage ?? "auto", request.TargetLanguage);

        var sw = Stopwatch.StartNew();

        var systemPrompt = SystemPrompts.Translate(request.TargetLanguage, request.Domain);

        var userContent = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(request.SourceLanguage))
            userContent.AppendLine($"Source language: {request.SourceLanguage}");
        userContent.AppendLine($"Translate the following text:\n\n{request.Text}");

        var chatResponse = await _aiClient.ChatAsync(new ChatCompletionRequest
        {
            Messages = new List<ChatMessage>
            {
                new() { Role = "system", Content = systemPrompt },
                new() { Role = "user", Content = userContent.ToString() }
            },
            Temperature = 0.3f,
            MaxTokens = 2048
        }, cancellationToken);

        sw.Stop();

        return new AIWritingResponse
        {
            Result = chatResponse.Content,
            Confidence = 0.88,
            TokensUsed = chatResponse.TotalTokens,
            ProcessingTimeMs = (int)sw.ElapsedMilliseconds
        };
    }

    public async Task<AIWritingResponse> SummarizeAsync(
        AISummarizeRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Writing summarize request from user {UserId}, text length: {Length}",
            userId, request.Text.Length);

        var sw = Stopwatch.StartNew();

        var userContent = new StringBuilder();
        if (request.MaxSentences.HasValue)
            userContent.AppendLine($"Summarize in at most {request.MaxSentences} sentences.");
        if (!string.IsNullOrWhiteSpace(request.FocusArea))
            userContent.AppendLine($"Focus on: {request.FocusArea}");
        userContent.AppendLine($"\nText to summarize:\n\n{request.Text}");

        var chatResponse = await _aiClient.ChatAsync(new ChatCompletionRequest
        {
            Messages = new List<ChatMessage>
            {
                new() { Role = "system", Content = SystemPrompts.Summarize },
                new() { Role = "user", Content = userContent.ToString() }
            },
            Temperature = 0.3f,
            MaxTokens = 1024
        }, cancellationToken);

        sw.Stop();

        return new AIWritingResponse
        {
            Result = chatResponse.Content,
            Confidence = 0.90,
            TokensUsed = chatResponse.TotalTokens,
            ProcessingTimeMs = (int)sw.ElapsedMilliseconds
        };
    }

    public async Task<AIWritingResponse> ContinueAsync(
        AIContinueRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Writing continue request from user {UserId}, text length: {Length}",
            userId, request.Text.Length);

        var sw = Stopwatch.StartNew();

        var userContent = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(request.Context))
            userContent.AppendLine($"Context: {request.Context}");
        if (!string.IsNullOrWhiteSpace(request.Direction))
            userContent.AppendLine($"Direction: {request.Direction}");
        if (request.MaxWords.HasValue)
            userContent.AppendLine($"Generate approximately {request.MaxWords} words.");
        userContent.AppendLine($"\nContinue from this text:\n\n{request.Text}");

        var chatResponse = await _aiClient.ChatAsync(new ChatCompletionRequest
        {
            Messages = new List<ChatMessage>
            {
                new() { Role = "system", Content = SystemPrompts.Continue },
                new() { Role = "user", Content = userContent.ToString() }
            },
            Temperature = 0.7f,
            MaxTokens = request.MaxWords.HasValue ? Math.Min(request.MaxWords.Value * 2, 2048) : 1024
        }, cancellationToken);

        sw.Stop();

        return new AIWritingResponse
        {
            Result = chatResponse.Content,
            Confidence = 0.75,
            TokensUsed = chatResponse.TotalTokens,
            ProcessingTimeMs = (int)sw.ElapsedMilliseconds
        };
    }

    public async IAsyncEnumerable<AIStreamChunk> StreamAsync(
        AIStreamWritingRequest request,
        Guid userId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Writing stream request from user {UserId}, operation: {Operation}",
            userId, request.Operation);

        var (systemPrompt, userContent, temperature) = BuildStreamPrompt(request);

        var tokenCount = 0;

        await foreach (var chunk in _aiClient.ChatStreamAsync(new ChatCompletionRequest
        {
            Messages = new List<ChatMessage>
            {
                new() { Role = "system", Content = systemPrompt },
                new() { Role = "user", Content = userContent }
            },
            Temperature = temperature,
            MaxTokens = 2048,
            Stream = true
        }, cancellationToken))
        {
            tokenCount += EstimateTokens(chunk.Delta);

            yield return new AIStreamChunk
            {
                Content = chunk.Delta,
                IsComplete = chunk.IsComplete,
                TokenCount = tokenCount
            };
        }
    }

    // --- Private helpers ---

    private static string BuildUserMessage(string text, string? context, string? instruction = null)
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(instruction))
            sb.AppendLine(instruction);
        if (!string.IsNullOrWhiteSpace(context))
            sb.AppendLine($"\nContext: {context}");
        sb.AppendLine($"\nText:\n\n{text}");
        return sb.ToString();
    }

    private static (string systemPrompt, string userContent, float temperature) BuildStreamPrompt(
        AIStreamWritingRequest request)
    {
        return request.Operation switch
        {
            WritingOperation.Generate => (
                SystemPrompts.Generate,
                BuildUserMessage(request.Text, request.Context, request.Instruction),
                0.7f),

            WritingOperation.Improve => (
                SystemPrompts.Improve,
                BuildUserMessage(request.Text, request.Context, request.Instruction),
                0.3f),

            WritingOperation.ChangeTone => (
                SystemPrompts.ChangeTone(request.TargetTone ?? WritingTone.Formal),
                BuildUserMessage(request.Text, request.Context),
                0.5f),

            WritingOperation.Translate => (
                SystemPrompts.Translate(request.TargetLanguage ?? "en", null),
                $"Translate the following text:\n\n{request.Text}",
                0.3f),

            WritingOperation.Summarize => (
                SystemPrompts.Summarize,
                $"Summarize the following text:\n\n{request.Text}",
                0.3f),

            WritingOperation.Continue => (
                SystemPrompts.Continue,
                BuildUserMessage(request.Text, request.Context, request.Instruction),
                0.7f),

            WritingOperation.Suggest => (
                SystemPrompts.Suggest,
                BuildUserMessage(request.Text, request.Context, request.Instruction),
                0.3f),

            _ => (
                SystemPrompts.Improve,
                BuildUserMessage(request.Text, request.Context, request.Instruction),
                0.5f)
        };
    }

    /// <summary>
    /// Attempt to parse suggested changes from the AI response by comparing input/output.
    /// This is a best-effort heuristic since the mock client returns general responses.
    /// </summary>
    private static IReadOnlyList<SuggestedChange>? ParseSuggestedChanges(string original, string improved)
    {
        // For a real implementation, the system prompt would instruct the AI to return
        // structured JSON with changes. For now, return a single whole-text change
        // if the texts differ.
        if (string.Equals(original.Trim(), improved.Trim(), StringComparison.Ordinal))
            return null;

        return new List<SuggestedChange>
        {
            new()
            {
                Original = original.Length > 200 ? original[..200] + "..." : original,
                Suggested = improved.Length > 200 ? improved[..200] + "..." : improved,
                Reason = "AI-suggested improvement",
                Category = "overall"
            }
        };
    }

    private static int EstimateTokens(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0;
        return (int)Math.Ceiling(text.Length / 4.0);
    }
}
