using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AFC27.KMS.Admin.Application.DTOs;
using AFC27.KMS.Admin.Application.Interfaces;
using AFC27.KMS.Admin.Domain.Entities;
using AFC27.KMS.SharedKernel.Interfaces;

namespace AFC27.KMS.Admin.Application.Services;

/// <summary>
/// Service interface for webhook management.
/// </summary>
public interface IWebhookService
{
    Task<WebhookSubscriptionDto> CreateAsync(CreateWebhookRequest request, CancellationToken cancellationToken = default);
    Task<WebhookSubscriptionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WebhookSubscriptionDto>> ListAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<WebhookSubscriptionDto?> UpdateAsync(Guid id, UpdateWebhookRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ActivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WebhookDeliveryLogDto>> GetDeliveryLogsAsync(Guid webhookId, int limit = 50, CancellationToken cancellationToken = default);
    Task<bool> TestAsync(Guid id, TestWebhookRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service for managing webhook subscriptions and delivery.
/// </summary>
public class WebhookService : IWebhookService
{
    private readonly DbContext _dbContext;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<WebhookService> _logger;

    public WebhookService(
        DbContext dbContext,
        ICurrentUser currentUser,
        ILogger<WebhookService> logger)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<WebhookSubscriptionDto> CreateAsync(
        CreateWebhookRequest request,
        CancellationToken cancellationToken = default)
    {
        var webhook = WebhookSubscription.Create(
            request.Name,
            request.Url,
            request.Secret,
            request.EventFilters?.ToList() ?? new List<string>(),
            _currentUser.UserId ?? Guid.Empty,
            _currentUser.DisplayName ?? "System",
            request.Description,
            request.MaxRetries,
            request.RetryDelaySeconds,
            request.TimeoutSeconds);

        _dbContext.Set<WebhookSubscription>().Add(webhook);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created webhook subscription {WebhookId} '{Name}'", webhook.Id, webhook.Name);

        return MapToDto(webhook);
    }

    public async Task<WebhookSubscriptionDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var webhook = await _dbContext.Set<WebhookSubscription>()
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        return webhook == null ? null : MapToDto(webhook);
    }

    public async Task<IReadOnlyList<WebhookSubscriptionDto>> ListAsync(
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<WebhookSubscription>().AsNoTracking();

        if (!includeInactive)
            query = query.Where(w => w.IsActive);

        var webhooks = await query
            .OrderBy(w => w.Name)
            .ToListAsync(cancellationToken);

        return webhooks.Select(MapToDto).ToList();
    }

    public async Task<WebhookSubscriptionDto?> UpdateAsync(
        Guid id,
        UpdateWebhookRequest request,
        CancellationToken cancellationToken = default)
    {
        var webhook = await _dbContext.Set<WebhookSubscription>()
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        if (webhook == null)
            return null;

        webhook.Update(
            request.Name,
            request.Url,
            request.EventFilters?.ToList(),
            request.Description,
            request.MaxRetries,
            request.RetryDelaySeconds,
            request.TimeoutSeconds);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated webhook subscription {WebhookId}", id);

        return MapToDto(webhook);
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var webhook = await _dbContext.Set<WebhookSubscription>()
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        if (webhook == null)
            return false;

        _dbContext.Set<WebhookSubscription>().Remove(webhook);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted webhook subscription {WebhookId}", id);

        return true;
    }

    public async Task<bool> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var webhook = await _dbContext.Set<WebhookSubscription>()
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        if (webhook == null) return false;

        webhook.Activate();
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var webhook = await _dbContext.Set<WebhookSubscription>()
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        if (webhook == null) return false;

        webhook.Deactivate();
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IReadOnlyList<WebhookDeliveryLogDto>> GetDeliveryLogsAsync(
        Guid webhookId,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        var logs = await _dbContext.Set<WebhookDeliveryLog>()
            .AsNoTracking()
            .Where(l => l.WebhookSubscriptionId == webhookId)
            .OrderByDescending(l => l.DeliveredAt)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return logs.Select(l => new WebhookDeliveryLogDto
        {
            Id = l.Id,
            WebhookSubscriptionId = l.WebhookSubscriptionId,
            EventType = l.EventType,
            HttpStatusCode = l.HttpStatusCode,
            AttemptNumber = l.AttemptNumber,
            Success = l.Success,
            ErrorMessage = l.ErrorMessage,
            DurationMs = l.DurationMs,
            DeliveredAt = l.DeliveredAt
        }).ToList();
    }

    public async Task<bool> TestAsync(
        Guid id,
        TestWebhookRequest request,
        CancellationToken cancellationToken = default)
    {
        var webhook = await _dbContext.Set<WebhookSubscription>()
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        if (webhook == null) return false;

        // Record a test delivery log
        var log = WebhookDeliveryLog.Create(
            webhook.Id,
            request.EventType ?? "test.ping",
            "{\"type\":\"test.ping\",\"timestamp\":\"" + DateTime.UtcNow.ToString("O") + "\"}",
            200,
            "OK",
            1,
            true,
            null,
            0);

        _dbContext.Set<WebhookDeliveryLog>().Add(log);
        webhook.RecordDelivery(true);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Test webhook delivery for {WebhookId}", id);

        return true;
    }

    private static WebhookSubscriptionDto MapToDto(WebhookSubscription webhook)
    {
        return new WebhookSubscriptionDto
        {
            Id = webhook.Id,
            Name = webhook.Name,
            Url = webhook.Url,
            EventFilters = webhook.EventFilters,
            IsActive = webhook.IsActive,
            MaxRetries = webhook.MaxRetries,
            RetryDelaySeconds = webhook.RetryDelaySeconds,
            TimeoutSeconds = webhook.TimeoutSeconds,
            Description = webhook.Description,
            CreatedByUserId = webhook.CreatedByUserId,
            CreatedByName = webhook.CreatedByName,
            LastTriggeredAt = webhook.LastTriggeredAt,
            TotalDeliveries = webhook.TotalDeliveries,
            FailedDeliveries = webhook.FailedDeliveries,
            CreatedAt = webhook.CreatedAt
        };
    }
}
