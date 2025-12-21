using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AFC27.KMS.AI.Application.DTOs;
using AFC27.KMS.AI.Application.Interfaces;
using AFC27.KMS.AI.Application.Services;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.AI.Presentation.Controllers;

/// <summary>
/// API controller for AI chat and RAG functionality.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AIController : ControllerBase
{
    private readonly IRAGService _ragService;
    private readonly IChatService _chatService;
    private readonly IIntalioAIClient _aiClient;
    private readonly EmbeddingService _embeddingService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<AIController> _logger;

    public AIController(
        IRAGService ragService,
        IChatService chatService,
        IIntalioAIClient aiClient,
        EmbeddingService embeddingService,
        ICurrentUser currentUser,
        ILogger<AIController> logger)
    {
        _ragService = ragService;
        _chatService = chatService;
        _aiClient = aiClient;
        _embeddingService = embeddingService;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <summary>
    /// Get AI service status.
    /// </summary>
    [HttpGet("status")]
    [ProducesResponseType(typeof(AIStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatus(CancellationToken cancellationToken)
    {
        var isAvailable = await _aiClient.IsAvailableAsync(cancellationToken);
        var isMock = _aiClient.GetType().Name.Contains("Mock");

        return Ok(new AIStatusResponse
        {
            IsAvailable = isAvailable,
            Provider = isMock ? "Mock" : "Intalio",
            Model = isMock ? "mock-gpt-4" : "gpt-4",
            IsMock = isMock,
            IndexedDocuments = _embeddingService.GetIndexedDocumentCount(),
            LastIndexUpdate = _embeddingService.GetLastIndexUpdate()
        });
    }

    /// <summary>
    /// Send a chat message (non-streaming).
    /// </summary>
    [HttpPost("chat")]
    [ProducesResponseType(typeof(RAGResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Chat(
        [FromBody] ChatRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new { error = "Message is required" });

        _logger.LogInformation("Chat request from user {UserId}", _currentUser.UserId);

        // Get or create conversation
        Guid conversationId;
        if (request.ConversationId.HasValue)
        {
            var existing = await _chatService.GetConversationAsync(
                request.ConversationId.Value, cancellationToken);

            if (existing == null)
                return BadRequest(new { error = "Conversation not found" });

            conversationId = request.ConversationId.Value;
        }
        else
        {
            var conversation = await _chatService.CreateConversationAsync(
                cancellationToken: cancellationToken);
            conversationId = conversation.Id;
        }

        // Save user message
        await _chatService.AddMessageAsync(
            conversationId, "user", request.Message, cancellationToken: cancellationToken);

        // Get chat history for context
        var history = await _chatService.GetChatHistoryAsync(
            conversationId, maxMessages: 10, cancellationToken);

        RAGResponse response;
        if (request.UseRAG)
        {
            // Use RAG to answer with document context
            response = await _ragService.QueryAsync(
                new RAGRequest
                {
                    Query = request.Message,
                    ConversationId = conversationId,
                    PreviousMessages = history,
                    RestrictToDocuments = request.RestrictToDocuments
                },
                cancellationToken);
        }
        else
        {
            // Direct chat without RAG
            var chatResponse = await _aiClient.ChatAsync(
                new ChatCompletionRequest
                {
                    Messages = history.Append(new ChatMessage
                    {
                        Role = "user",
                        Content = request.Message
                    }).ToList(),
                    Temperature = 0.7f
                },
                cancellationToken);

            response = new RAGResponse
            {
                Answer = chatResponse.Content,
                Citations = Array.Empty<Citation>(),
                Sources = Array.Empty<DocumentChunk>(),
                TokensUsed = chatResponse.TotalTokens
            };
        }

        // Save assistant response
        await _chatService.AddMessageAsync(
            conversationId, "assistant", response.Answer,
            response.Citations, cancellationToken);

        // Auto-generate title for new conversations
        if (!request.ConversationId.HasValue)
        {
            await _chatService.GenerateTitleAsync(conversationId, cancellationToken);
        }

        return Ok(new
        {
            conversationId,
            response.Answer,
            response.Citations,
            response.Sources,
            response.SuggestedFollowUp,
            response.TokensUsed
        });
    }

    /// <summary>
    /// Send a chat message with streaming response.
    /// </summary>
    [HttpPost("chat/stream")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task ChatStream(
        [FromBody] ChatRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            await Response.WriteAsync("{\"error\":\"Message is required\"}");
            return;
        }

        _logger.LogInformation("Streaming chat request from user {UserId}", _currentUser.UserId);

        // Set up SSE response
        Response.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Connection = "keep-alive";

        // Get or create conversation
        Guid conversationId;
        if (request.ConversationId.HasValue)
        {
            var existing = await _chatService.GetConversationAsync(
                request.ConversationId.Value, cancellationToken);

            if (existing == null)
            {
                await SendSSEEvent("error", new { error = "Conversation not found" });
                return;
            }

            conversationId = request.ConversationId.Value;
        }
        else
        {
            var conversation = await _chatService.CreateConversationAsync(
                cancellationToken: cancellationToken);
            conversationId = conversation.Id;

            // Send conversation ID to client
            await SendSSEEvent("conversation", new { conversationId });
        }

        // Save user message
        await _chatService.AddMessageAsync(
            conversationId, "user", request.Message, cancellationToken: cancellationToken);

        // Get chat history
        var history = await _chatService.GetChatHistoryAsync(
            conversationId, maxMessages: 10, cancellationToken);

        var fullResponse = new StringBuilder();

        if (request.UseRAG)
        {
            // Stream RAG response
            await foreach (var chunk in _ragService.QueryStreamAsync(
                new RAGRequest
                {
                    Query = request.Message,
                    ConversationId = conversationId,
                    PreviousMessages = history,
                    RestrictToDocuments = request.RestrictToDocuments,
                    Stream = true
                },
                cancellationToken))
            {
                fullResponse.Append(chunk.Delta);

                if (chunk.Sources != null && chunk.Sources.Any())
                {
                    await SendSSEEvent("sources", new { sources = chunk.Sources });
                }

                if (chunk.IsComplete)
                {
                    await SendSSEEvent("done", new
                    {
                        citations = chunk.Citations,
                        fullResponse = fullResponse.ToString()
                    });
                }
                else if (!string.IsNullOrEmpty(chunk.Delta))
                {
                    await SendSSEEvent("chunk", new { delta = chunk.Delta });
                }
            }
        }
        else
        {
            // Stream direct chat response
            await foreach (var chunk in _aiClient.ChatStreamAsync(
                new ChatCompletionRequest
                {
                    Messages = history.Append(new ChatMessage
                    {
                        Role = "user",
                        Content = request.Message
                    }).ToList(),
                    Temperature = 0.7f,
                    Stream = true
                },
                cancellationToken))
            {
                fullResponse.Append(chunk.Delta);

                if (chunk.IsComplete)
                {
                    await SendSSEEvent("done", new { fullResponse = fullResponse.ToString() });
                }
                else if (!string.IsNullOrEmpty(chunk.Delta))
                {
                    await SendSSEEvent("chunk", new { delta = chunk.Delta });
                }
            }
        }

        // Save complete response
        await _chatService.AddMessageAsync(
            conversationId, "assistant", fullResponse.ToString(),
            cancellationToken: cancellationToken);

        // Auto-generate title for new conversations
        if (!request.ConversationId.HasValue)
        {
            var title = await _chatService.GenerateTitleAsync(conversationId, cancellationToken);
            await SendSSEEvent("title", new { title });
        }
    }

    /// <summary>
    /// Search documents using semantic search.
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IReadOnlyList<DocumentChunk>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string query,
        [FromQuery] int maxResults = 10,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest(new { error = "Query is required" });

        var results = await _ragService.SearchAsync(
            query, _currentUser.UserId ?? Guid.Empty, maxResults, cancellationToken);

        return Ok(results);
    }

    /// <summary>
    /// Get user's conversations.
    /// </summary>
    [HttpGet("conversations")]
    [ProducesResponseType(typeof(IReadOnlyList<ConversationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConversations(
        [FromQuery] int limit = 50,
        CancellationToken cancellationToken = default)
    {
        var conversations = await _chatService.ListConversationsAsync(limit, cancellationToken);
        return Ok(conversations);
    }

    /// <summary>
    /// Get a specific conversation with messages.
    /// </summary>
    [HttpGet("conversations/{conversationId:guid}")]
    [ProducesResponseType(typeof(ConversationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetConversation(
        Guid conversationId,
        CancellationToken cancellationToken)
    {
        var conversation = await _chatService.GetConversationAsync(conversationId, cancellationToken);

        if (conversation == null)
            return NotFound();

        return Ok(conversation);
    }

    /// <summary>
    /// Get messages for a conversation.
    /// </summary>
    [HttpGet("conversations/{conversationId:guid}/messages")]
    [ProducesResponseType(typeof(IReadOnlyList<MessageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMessages(
        Guid conversationId,
        CancellationToken cancellationToken)
    {
        var messages = await _chatService.GetMessagesAsync(conversationId, cancellationToken);
        return Ok(messages);
    }

    /// <summary>
    /// Delete a conversation.
    /// </summary>
    [HttpDelete("conversations/{conversationId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteConversation(
        Guid conversationId,
        CancellationToken cancellationToken)
    {
        var success = await _chatService.DeleteConversationAsync(conversationId, cancellationToken);

        if (!success)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Update conversation title.
    /// </summary>
    [HttpPatch("conversations/{conversationId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateConversation(
        Guid conversationId,
        [FromBody] UpdateConversationRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest(new { error = "Title is required" });

        var success = await _chatService.UpdateTitleAsync(
            conversationId, request.Title, cancellationToken);

        if (!success)
            return NotFound();

        return NoContent();
    }

    private async Task SendSSEEvent(string eventType, object data)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(data);
        await Response.WriteAsync($"event: {eventType}\ndata: {json}\n\n");
        await Response.Body.FlushAsync();
    }
}

/// <summary>
/// Request to update conversation.
/// </summary>
public record UpdateConversationRequest
{
    public string Title { get; init; } = string.Empty;
}
