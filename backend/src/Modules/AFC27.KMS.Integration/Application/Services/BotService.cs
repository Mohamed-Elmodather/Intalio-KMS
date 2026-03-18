using System.Diagnostics;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Integration.Application.DTOs;

namespace AFC27.KMS.Integration.Application.Services;

/// <summary>
/// Service interface for processing bot messages through the RAG/AI pipeline
/// and formatting responses for Teams/Slack channels.
/// </summary>
public interface IBotService
{
    /// <summary>
    /// Process an incoming bot message: run RAG search, generate an AI answer,
    /// and return a formatted response.
    /// </summary>
    Task<BotMessageResponse> ProcessMessageAsync(
        BotMessageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Register a new bot webhook for push-based messaging.
    /// </summary>
    Task<BotRegistrationResponse> RegisterBotAsync(
        BotRegistrationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List active bot registrations.
    /// </summary>
    Task<IReadOnlyList<BotRegistrationResponse>> ListRegistrationsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove a bot registration.
    /// </summary>
    Task<bool> UnregisterBotAsync(
        Guid registrationId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Routes incoming Teams/Slack questions through the existing RAG/AI service,
/// formats the response for the target platform, and tracks bot registrations.
/// </summary>
public class BotService : IBotService
{
    private readonly ILogger<BotService> _logger;

    // In-memory store for registrations (replace with repository in production)
    private static readonly List<BotRegistrationResponse> _registrations = new();

    public BotService(ILogger<BotService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<BotMessageResponse> ProcessMessageAsync(
        BotMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        _logger.LogInformation(
            "Processing bot message from {Channel} user {UserId}: {TextPreview}",
            request.Channel,
            request.UserId,
            request.Text.Length > 80 ? request.Text[..80] + "..." : request.Text);

        // TODO: Integrate with the real AI/RAG service (AFC27.KMS.AI module).
        // The flow would be:
        //   1. Call the RAG search endpoint to find relevant knowledge articles.
        //   2. Send the user question + retrieved context to the AI completion service.
        //   3. Format the result based on the target channel.

        // Simulate RAG pipeline result
        var articleId = Guid.NewGuid();
        var answerText = $"Based on the AFC27 Knowledge Base, here is what I found regarding your question:\n\n" +
                         $"\"{request.Text}\"\n\n" +
                         $"This topic is covered in our knowledge articles. " +
                         $"Please visit the Knowledge Portal for full details.";

        var sources = new List<BotSourceReference>
        {
            new()
            {
                ArticleId = articleId,
                Title = "Related Knowledge Article",
                Url = $"/articles/{articleId}",
                Relevance = 0.87
            }
        };

        var suggestedQuestions = new List<string>
        {
            "Can you provide more details on this topic?",
            "What are the related policies?",
            "Who should I contact for further assistance?"
        };

        sw.Stop();

        var response = new BotMessageResponse
        {
            ReplyToMessageId = request.MessageId,
            Text = answerText,
            ContentFormat = FormatForChannel(request.Channel),
            Sources = sources,
            Confidence = 0.87,
            SuggestedQuestions = suggestedQuestions,
            ProcessingTimeMs = (int)sw.ElapsedMilliseconds
        };

        // Generate rich content based on channel
        response.RichContent = request.Channel.ToLowerInvariant() switch
        {
            "teams" => BuildAdaptiveCardJson(response),
            "slack" => BuildSlackBlocksJson(response),
            _ => null
        };

        _logger.LogInformation(
            "Bot response generated in {Ms}ms for {Channel}, confidence: {Confidence}",
            response.ProcessingTimeMs, request.Channel, response.Confidence);

        return response;
    }

    /// <inheritdoc />
    public Task<BotRegistrationResponse> RegisterBotAsync(
        BotRegistrationRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Registering bot for channel {Channel}, name: {Name}, webhook: {Url}",
            request.Channel, request.Name, request.WebhookUrl);

        var registration = new BotRegistrationResponse
        {
            RegistrationId = Guid.NewGuid(),
            Channel = request.Channel,
            Name = request.Name,
            Status = "Active",
            RegisteredAt = DateTime.UtcNow
        };

        _registrations.Add(registration);

        // TODO: Persist to database via repository
        // TODO: Validate webhook URL by sending a verification challenge

        return Task.FromResult(registration);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<BotRegistrationResponse>> ListRegistrationsAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<BotRegistrationResponse>>(
            _registrations.AsReadOnly());
    }

    /// <inheritdoc />
    public Task<bool> UnregisterBotAsync(
        Guid registrationId,
        CancellationToken cancellationToken = default)
    {
        var reg = _registrations.FirstOrDefault(r => r.RegistrationId == registrationId);
        if (reg == null) return Task.FromResult(false);

        _registrations.Remove(reg);
        _logger.LogInformation("Bot registration {Id} removed", registrationId);
        return Task.FromResult(true);
    }

    #region Channel Formatting Helpers

    private static string FormatForChannel(string channel) => channel.ToLowerInvariant() switch
    {
        "teams" => "adaptive-card",
        "slack" => "markdown",
        _ => "text"
    };

    private static string BuildAdaptiveCardJson(BotMessageResponse response)
    {
        // Simplified Adaptive Card JSON for Teams
        var sourcesJson = string.Join(",\n",
            response.Sources.Select(s =>
                $"{{\"type\":\"TextBlock\",\"text\":\"- [{s.Title}]({s.Url}) (relevance: {s.Relevance:P0})\",\"wrap\":true}}"));

        var suggestionsJson = string.Join(",\n",
            response.SuggestedQuestions.Select(q =>
                $"{{\"type\":\"Action.Submit\",\"title\":\"{q}\",\"data\":{{\"question\":\"{q}\"}}}}"));

        return $$"""
        {
          "type": "AdaptiveCard",
          "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
          "version": "1.4",
          "body": [
            {"type":"TextBlock","text":"{{response.Text.Replace("\"", "\\\"")}}","wrap":true},
            {"type":"TextBlock","text":"**Sources:**","weight":"Bolder"},
            {{sourcesJson}}
          ],
          "actions": [
            {{suggestionsJson}}
          ]
        }
        """;
    }

    private static string BuildSlackBlocksJson(BotMessageResponse response)
    {
        var sourcesText = string.Join("\n",
            response.Sources.Select(s => $"- <{s.Url}|{s.Title}> (relevance: {s.Relevance:P0})"));

        var suggestionsText = string.Join(", ",
            response.SuggestedQuestions.Select(q => $"`{q}`"));

        return $$"""
        {
          "blocks": [
            {"type":"section","text":{"type":"mrkdwn","text":"{{response.Text.Replace("\"", "\\\"")}}"}},
            {"type":"divider"},
            {"type":"section","text":{"type":"mrkdwn","text":"*Sources:*\n{{sourcesText.Replace("\"", "\\\"")}}"}},
            {"type":"context","elements":[{"type":"mrkdwn","text":"Suggested follow-ups: {{suggestionsText.Replace("\"", "\\\"")}}"}]}
          ]
        }
        """;
    }

    #endregion
}
