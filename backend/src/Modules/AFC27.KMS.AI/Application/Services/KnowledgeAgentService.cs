using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.AI.Application.DTOs;
using AFC27.KMS.AI.Application.Interfaces;
using AFC27.KMS.AI.Domain.Entities;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.AI.Application.Services;

/// <summary>
/// Service interface for knowledge agent management and querying.
/// </summary>
public interface IKnowledgeAgentService
{
    Task<KnowledgeAgentDto> CreateAsync(CreateKnowledgeAgentRequest request, CancellationToken cancellationToken = default);
    Task<KnowledgeAgentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KnowledgeAgentDto>> ListAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<KnowledgeAgentDto?> UpdateAsync(Guid id, UpdateKnowledgeAgentRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AgentQueryResponse> QueryAsync(Guid agentId, AgentQueryRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service for managing and querying knowledge agents.
/// </summary>
public class KnowledgeAgentService : IKnowledgeAgentService
{
    private readonly DbContext _dbContext;
    private readonly IRAGService _ragService;
    private readonly IIntalioAIClient _aiClient;
    private readonly IChatService _chatService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<KnowledgeAgentService> _logger;

    public KnowledgeAgentService(
        DbContext dbContext,
        IRAGService ragService,
        IIntalioAIClient aiClient,
        IChatService chatService,
        ICurrentUser currentUser,
        ILogger<KnowledgeAgentService> logger)
    {
        _dbContext = dbContext;
        _ragService = ragService;
        _aiClient = aiClient;
        _chatService = chatService;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<KnowledgeAgentDto> CreateAsync(
        CreateKnowledgeAgentRequest request,
        CancellationToken cancellationToken = default)
    {
        var agent = KnowledgeAgent.Create(
            request.Name,
            request.Description,
            request.SystemPrompt,
            _currentUser.UserId ?? Guid.Empty,
            request.SpaceId,
            request.AllowedSources?.ToList(),
            request.Temperature,
            request.MaxTokens);

        _dbContext.Set<KnowledgeAgent>().Add(agent);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Created knowledge agent {AgentId} '{AgentName}' by user {UserId}",
            agent.Id, agent.Name, _currentUser.UserId);

        return MapToDto(agent);
    }

    public async Task<KnowledgeAgentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var agent = await _dbContext.Set<KnowledgeAgent>()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        return agent == null ? null : MapToDto(agent);
    }

    public async Task<IReadOnlyList<KnowledgeAgentDto>> ListAsync(
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<KnowledgeAgent>().AsNoTracking();

        if (!includeInactive)
            query = query.Where(a => a.IsActive);

        var agents = await query
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);

        return agents.Select(MapToDto).ToList();
    }

    public async Task<KnowledgeAgentDto?> UpdateAsync(
        Guid id,
        UpdateKnowledgeAgentRequest request,
        CancellationToken cancellationToken = default)
    {
        var agent = await _dbContext.Set<KnowledgeAgent>()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (agent == null)
            return null;

        agent.Update(
            request.Name,
            request.Description,
            request.SystemPrompt,
            request.SpaceId,
            request.AllowedSources?.ToList(),
            request.Temperature,
            request.MaxTokens);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated knowledge agent {AgentId}", id);

        return MapToDto(agent);
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var agent = await _dbContext.Set<KnowledgeAgent>()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (agent == null)
            return false;

        _dbContext.Set<KnowledgeAgent>().Remove(agent);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted knowledge agent {AgentId}", id);

        return true;
    }

    public async Task<AgentQueryResponse> QueryAsync(
        Guid agentId,
        AgentQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var agent = await _dbContext.Set<KnowledgeAgent>()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == agentId && a.IsActive, cancellationToken);

        if (agent == null)
            throw new InvalidOperationException($"Knowledge agent {agentId} not found or inactive");

        _logger.LogInformation(
            "Querying knowledge agent {AgentId} '{AgentName}' by user {UserId}",
            agentId, agent.Name, _currentUser.UserId);

        // Build the RAG request scoped to agent's allowed sources
        var ragRequest = new RAGRequest
        {
            Query = request.Message,
            ConversationId = request.ConversationId,
            MaxSources = 5,
            MinRelevanceScore = 0.7f
        };

        // Use RAG with agent's system prompt context
        var ragResponse = await _ragService.QueryAsync(ragRequest, cancellationToken);

        // Refine via agent's system prompt
        var messages = new List<ChatMessage>
        {
            new ChatMessage { Role = "system", Content = agent.SystemPrompt },
            new ChatMessage { Role = "user", Content = BuildAgentPrompt(request.Message, ragResponse) }
        };

        var chatResponse = await _aiClient.ChatAsync(
            new ChatCompletionRequest
            {
                Messages = messages,
                Temperature = agent.Temperature,
                MaxTokens = agent.MaxTokens
            },
            cancellationToken);

        return new AgentQueryResponse
        {
            AgentId = agent.Id,
            AgentName = agent.Name,
            Answer = chatResponse.Content,
            Citations = ragResponse.Citations,
            Sources = ragResponse.Sources,
            ConversationId = request.ConversationId,
            TokensUsed = chatResponse.TotalTokens
        };
    }

    private static string BuildAgentPrompt(string userMessage, RAGResponse ragResponse)
    {
        if (!ragResponse.Sources.Any())
            return userMessage;

        var contextBuilder = new System.Text.StringBuilder();
        contextBuilder.AppendLine("Use the following context to answer the user's question:");
        contextBuilder.AppendLine();

        foreach (var source in ragResponse.Sources)
        {
            contextBuilder.AppendLine($"--- Source: {source.DocumentName} (relevance: {source.RelevanceScore:F2}) ---");
            contextBuilder.AppendLine(source.Content);
            contextBuilder.AppendLine();
        }

        contextBuilder.AppendLine($"User question: {userMessage}");

        return contextBuilder.ToString();
    }

    private static KnowledgeAgentDto MapToDto(KnowledgeAgent agent)
    {
        return new KnowledgeAgentDto
        {
            Id = agent.Id,
            Name = agent.Name,
            Description = agent.Description,
            SystemPrompt = agent.SystemPrompt,
            SpaceId = agent.SpaceId,
            AllowedSources = agent.AllowedSources,
            Temperature = agent.Temperature,
            MaxTokens = agent.MaxTokens,
            IsActive = agent.IsActive,
            CreatedById = agent.CreatedById,
            CreatedAt = agent.CreatedAt,
            UpdatedAt = agent.UpdatedAt
        };
    }
}
