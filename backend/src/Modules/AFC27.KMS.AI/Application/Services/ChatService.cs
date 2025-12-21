using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.AI.Application.DTOs;
using AFC27.KMS.AI.Application.Interfaces;
using AFC27.KMS.AI.Domain.Entities;
using AFC27.KMS.SharedKernel.Interfaces;


namespace AFC27.KMS.AI.Application.Services;

/// <summary>
/// Service for managing chat conversations and messages.
/// </summary>
public class ChatService : IChatService
{
    private readonly DbContext _dbContext;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<ChatService> _logger;

    public ChatService(
        DbContext dbContext,
        ICurrentUser currentUser,
        ILogger<ChatService> logger)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ConversationDto> CreateConversationAsync(
        string? title = null,
        CancellationToken cancellationToken = default)
    {
        var conversation = Conversation.Create(
            _currentUser.UserId ?? Guid.Empty,
            title ?? "New Conversation");

        _dbContext.Set<Conversation>().Add(conversation);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Created conversation {ConversationId} for user {UserId}",
            conversation.Id, _currentUser.UserId);

        return MapToDto(conversation);
    }

    public async Task<ConversationDto?> GetConversationAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        var conversation = await _dbContext.Set<Conversation>()
            .AsNoTracking()
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == _currentUser.UserId,
                cancellationToken);

        if (conversation == null)
            return null;

        return MapToDto(conversation);
    }

    public async Task<IReadOnlyList<ConversationDto>> ListConversationsAsync(
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        var conversations = await _dbContext.Set<Conversation>()
            .AsNoTracking()
            .Where(c => c.UserId == _currentUser.UserId && !c.IsDeleted)
            .OrderByDescending(c => c.LastMessageAt)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return conversations.Select(MapToDto).ToList();
    }

    public async Task<MessageDto> AddMessageAsync(
        Guid conversationId,
        string role,
        string content,
        IReadOnlyList<Citation>? citations = null,
        CancellationToken cancellationToken = default)
    {
        var conversation = await _dbContext.Set<Conversation>()
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == _currentUser.UserId,
                cancellationToken);

        if (conversation == null)
            throw new InvalidOperationException($"Conversation {conversationId} not found");

        string? citationsJson = null;
        if (citations != null && citations.Any())
        {
            citationsJson = JsonSerializer.Serialize(citations);
        }

        var message = conversation.AddMessage(role, content, citationsJson);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogDebug(
            "Added {Role} message to conversation {ConversationId}",
            role, conversationId);

        return MapToMessageDto(message);
    }

    public async Task<IReadOnlyList<MessageDto>> GetMessagesAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        var messages = await _dbContext.Set<Message>()
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);

        // Verify user has access to this conversation
        if (messages.Any())
        {
            var conversation = await _dbContext.Set<Conversation>()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == conversationId, cancellationToken);

            if (conversation?.UserId != _currentUser.UserId)
                return Array.Empty<MessageDto>();
        }

        return messages.Select(MapToMessageDto).ToList();
    }

    public async Task<bool> DeleteConversationAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        var conversation = await _dbContext.Set<Conversation>()
            .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == _currentUser.UserId,
                cancellationToken);

        if (conversation == null)
            return false;

        conversation.Delete();
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted conversation {ConversationId}", conversationId);

        return true;
    }

    public async Task<bool> UpdateTitleAsync(
        Guid conversationId,
        string title,
        CancellationToken cancellationToken = default)
    {
        var conversation = await _dbContext.Set<Conversation>()
            .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == _currentUser.UserId,
                cancellationToken);

        if (conversation == null)
            return false;

        conversation.UpdateTitle(title);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Updated title for conversation {ConversationId} to '{Title}'",
            conversationId, title);

        return true;
    }

    /// <summary>
    /// Auto-generate title from first message content.
    /// </summary>
    public async Task<string> GenerateTitleAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        var firstMessage = await _dbContext.Set<Message>()
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId && m.Role == "user")
            .OrderBy(m => m.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (firstMessage == null)
            return "New Conversation";

        // Take first 50 characters as title
        var title = firstMessage.Content.Length > 50
            ? firstMessage.Content.Substring(0, 47) + "..."
            : firstMessage.Content;

        await UpdateTitleAsync(conversationId, title, cancellationToken);

        return title;
    }

    /// <summary>
    /// Get conversation messages as ChatMessage list for AI context.
    /// </summary>
    public async Task<IReadOnlyList<ChatMessage>> GetChatHistoryAsync(
        Guid conversationId,
        int maxMessages = 20,
        CancellationToken cancellationToken = default)
    {
        var messages = await _dbContext.Set<Message>()
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.CreatedAt)
            .Take(maxMessages)
            .OrderBy(m => m.CreatedAt) // Re-order chronologically
            .ToListAsync(cancellationToken);

        return messages.Select(m => new ChatMessage
        {
            Role = m.Role,
            Content = m.Content
        }).ToList();
    }

    private static ConversationDto MapToDto(Conversation conversation)
    {
        return new ConversationDto
        {
            Id = conversation.Id,
            Title = conversation.Title,
            UserId = conversation.UserId,
            CreatedAt = conversation.CreatedAt,
            LastMessageAt = conversation.LastMessageAt,
            MessageCount = conversation.Messages?.Count ?? 0
        };
    }

    private static MessageDto MapToMessageDto(Message message)
    {
        IReadOnlyList<Citation>? citations = null;
        if (!string.IsNullOrEmpty(message.CitationsJson))
        {
            try
            {
                citations = JsonSerializer.Deserialize<List<Citation>>(message.CitationsJson);
            }
            catch { }
        }

        return new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            Role = message.Role,
            Content = message.Content,
            Citations = citations,
            CreatedAt = message.CreatedAt
        };
    }
}
