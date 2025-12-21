using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Integration.Application.DTOs;

namespace AFC27.KMS.Integration.Presentation.Controllers;

/// <summary>
/// Controller for webhook management
/// </summary>
[ApiController]
[Route("api/integration/webhooks")]
[Authorize(Policy = "IntegrationAdmin")]
public class WebhooksController : ControllerBase
{
    #region Webhook Configuration

    /// <summary>
    /// Get all webhook configurations
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WebhookConfigDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WebhookConfigDto>>> GetWebhooks(
        [FromQuery] Guid? connectorId = null,
        [FromQuery] bool? isActive = null)
    {
        // TODO: Return webhooks
        var webhooks = new List<WebhookConfigDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Task Assignment Webhook",
                Description = "Notifies external system when tasks are assigned",
                ConnectorId = Guid.NewGuid(),
                ConnectorName = "Intalio Case",
                TargetUrl = "https://case.intalio.com/api/webhooks/kms",
                HttpMethod = "POST",
                TriggerEvents = new List<string> { "TaskAssigned", "TaskCompleted", "TaskEscalated" },
                IsActive = true,
                LastTriggeredAt = DateTime.UtcNow.AddMinutes(-15),
                SuccessCount = 245,
                FailureCount = 3
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Document Sync Webhook",
                Description = "Syncs document changes to Intalio DMS",
                ConnectorId = Guid.NewGuid(),
                ConnectorName = "Intalio DMS",
                TargetUrl = "https://dms.intalio.com/api/webhooks/documents",
                HttpMethod = "POST",
                TriggerEvents = new List<string> { "DocumentCreated", "DocumentUpdated", "DocumentDeleted" },
                IsActive = true,
                LastTriggeredAt = DateTime.UtcNow.AddMinutes(-30),
                SuccessCount = 1250,
                FailureCount = 12
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "User Provisioning Webhook",
                Description = "Provisions new users to Intalio IAM",
                ConnectorId = Guid.NewGuid(),
                ConnectorName = "Intalio IAM",
                TargetUrl = "https://iam.intalio.com/api/webhooks/users",
                HttpMethod = "POST",
                TriggerEvents = new List<string> { "UserCreated", "UserUpdated", "UserDeactivated" },
                IsActive = true,
                LastTriggeredAt = DateTime.UtcNow.AddHours(-1),
                SuccessCount = 89,
                FailureCount = 1
            }
        };
        return Ok(webhooks);
    }

    /// <summary>
    /// Get webhook by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WebhookConfigDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WebhookConfigDto>> GetWebhook(Guid id)
    {
        // TODO: Return webhook details
        return NotFound();
    }

    /// <summary>
    /// Create webhook
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(WebhookConfigDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<WebhookConfigDto>> CreateWebhook([FromBody] CreateWebhookRequest request)
    {
        // TODO: Create webhook
        var webhook = new WebhookConfigDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            ConnectorId = request.ConnectorId,
            TargetUrl = request.TargetUrl,
            HttpMethod = request.HttpMethod,
            TriggerEvents = request.TriggerEvents,
            IsActive = true,
            SuccessCount = 0,
            FailureCount = 0
        };
        return Created($"/api/integration/webhooks/{webhook.Id}", webhook);
    }

    /// <summary>
    /// Update webhook
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(WebhookConfigDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WebhookConfigDto>> UpdateWebhook(
        Guid id,
        [FromBody] CreateWebhookRequest request)
    {
        // TODO: Update webhook
        return Ok(new WebhookConfigDto { Id = id, Name = request.Name });
    }

    /// <summary>
    /// Delete webhook
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteWebhook(Guid id)
    {
        // TODO: Delete webhook
        return NoContent();
    }

    /// <summary>
    /// Activate webhook
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ActivateWebhook(Guid id)
    {
        // TODO: Activate webhook
        return NoContent();
    }

    /// <summary>
    /// Deactivate webhook
    /// </summary>
    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeactivateWebhook(Guid id)
    {
        // TODO: Deactivate webhook
        return NoContent();
    }

    /// <summary>
    /// Test webhook
    /// </summary>
    [HttpPost("{id:guid}/test")]
    [ProducesResponseType(typeof(WebhookTestResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<WebhookTestResult>> TestWebhook(Guid id)
    {
        // TODO: Send test webhook
        var result = new WebhookTestResult
        {
            Success = true,
            StatusCode = 200,
            ResponseTimeMs = 150,
            ResponseBody = "{\"status\":\"received\"}",
            Message = "Webhook test successful"
        };
        return Ok(result);
    }

    #endregion

    #region Webhook Deliveries

    /// <summary>
    /// Get webhook deliveries
    /// </summary>
    [HttpGet("{id:guid}/deliveries")]
    [ProducesResponseType(typeof(IEnumerable<WebhookDeliveryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WebhookDeliveryDto>>> GetDeliveries(
        Guid id,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        // TODO: Return deliveries
        var deliveries = new List<WebhookDeliveryDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                WebhookId = id,
                EventType = "TaskAssigned",
                Status = "Success",
                ResponseStatusCode = 200,
                DurationMs = 125,
                AttemptNumber = 1,
                CreatedAt = DateTime.UtcNow.AddMinutes(-15)
            },
            new()
            {
                Id = Guid.NewGuid(),
                WebhookId = id,
                EventType = "TaskCompleted",
                Status = "Success",
                ResponseStatusCode = 200,
                DurationMs = 98,
                AttemptNumber = 1,
                CreatedAt = DateTime.UtcNow.AddMinutes(-30)
            },
            new()
            {
                Id = Guid.NewGuid(),
                WebhookId = id,
                EventType = "TaskEscalated",
                Status = "Failed",
                ResponseStatusCode = 500,
                DurationMs = 5000,
                AttemptNumber = 3,
                ErrorMessage = "Connection timeout",
                CreatedAt = DateTime.UtcNow.AddHours(-1)
            }
        };
        return Ok(deliveries);
    }

    /// <summary>
    /// Get delivery details
    /// </summary>
    [HttpGet("deliveries/{deliveryId:guid}")]
    [ProducesResponseType(typeof(WebhookDeliveryDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WebhookDeliveryDetailDto>> GetDelivery(Guid deliveryId)
    {
        // TODO: Return delivery details
        var delivery = new WebhookDeliveryDetailDto
        {
            Id = deliveryId,
            WebhookId = Guid.NewGuid(),
            WebhookName = "Task Assignment Webhook",
            EventType = "TaskAssigned",
            Status = "Success",
            ResponseStatusCode = 200,
            DurationMs = 125,
            AttemptNumber = 1,
            CreatedAt = DateTime.UtcNow.AddMinutes(-15),
            RequestHeaders = new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["X-Webhook-Signature"] = "sha256=abc123...",
                ["X-Webhook-Event"] = "TaskAssigned"
            },
            RequestBody = "{\"eventType\":\"TaskAssigned\",\"taskId\":\"task-001\",\"assignee\":\"user@example.com\"}",
            ResponseHeaders = new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json"
            },
            ResponseBody = "{\"status\":\"received\",\"messageId\":\"msg-001\"}"
        };
        return Ok(delivery);
    }

    /// <summary>
    /// Retry failed delivery
    /// </summary>
    [HttpPost("deliveries/{deliveryId:guid}/retry")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> RetryDelivery(Guid deliveryId)
    {
        // TODO: Retry delivery
        return Accepted(new { DeliveryId = deliveryId, Message = "Retry scheduled" });
    }

    #endregion

    #region Incoming Webhooks

    /// <summary>
    /// Receive webhook from Intalio Case
    /// </summary>
    [HttpPost("receive/intalio-case")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ReceiveIntalioCaseWebhook(
        [FromHeader(Name = "X-Intalio-Signature")] string? signature,
        [FromBody] IntalioCaseWebhookPayload payload)
    {
        // TODO: Validate signature and process webhook
        // TODO: Update task status in KMS based on Intalio Case events
        return Ok(new { Status = "Received", EventId = payload.EventId });
    }

    /// <summary>
    /// Receive webhook from Intalio IAM
    /// </summary>
    [HttpPost("receive/intalio-iam")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ReceiveIntalioIAMWebhook(
        [FromHeader(Name = "X-Intalio-Signature")] string? signature,
        [FromBody] IntalioIAMWebhookPayload payload)
    {
        // TODO: Validate signature and process webhook
        // TODO: Sync user changes from Intalio IAM
        return Ok(new { Status = "Received", EventId = payload.EventId });
    }

    /// <summary>
    /// Receive webhook from Intalio DMS
    /// </summary>
    [HttpPost("receive/intalio-dms")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ReceiveIntalioDMSWebhook(
        [FromHeader(Name = "X-Intalio-Signature")] string? signature,
        [FromBody] IntalioDMSWebhookPayload payload)
    {
        // TODO: Validate signature and process webhook
        // TODO: Sync document changes from Intalio DMS
        return Ok(new { Status = "Received", EventId = payload.EventId });
    }

    /// <summary>
    /// Receive webhook from Intalio Correspondence
    /// </summary>
    [HttpPost("receive/intalio-correspondence")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ReceiveIntalioCorrespondenceWebhook(
        [FromHeader(Name = "X-Intalio-Signature")] string? signature,
        [FromBody] IntalioCorrespondenceWebhookPayload payload)
    {
        // TODO: Validate signature and process webhook
        return Ok(new { Status = "Received", EventId = payload.EventId });
    }

    /// <summary>
    /// Receive generic webhook
    /// </summary>
    [HttpPost("receive/{connectorKey}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ReceiveGenericWebhook(
        string connectorKey,
        [FromHeader(Name = "X-Webhook-Signature")] string? signature,
        [FromBody] object payload)
    {
        // TODO: Validate signature and process webhook based on connector
        return Ok(new { Status = "Received", Connector = connectorKey });
    }

    #endregion

    #region Available Events

    /// <summary>
    /// Get available webhook events
    /// </summary>
    [HttpGet("events")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<WebhookEventType>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<WebhookEventType>> GetAvailableEvents()
    {
        var events = new List<WebhookEventType>
        {
            // Task events
            new() { Category = "Tasks", Event = "TaskCreated", Description = "Triggered when a task is created" },
            new() { Category = "Tasks", Event = "TaskAssigned", Description = "Triggered when a task is assigned" },
            new() { Category = "Tasks", Event = "TaskCompleted", Description = "Triggered when a task is completed" },
            new() { Category = "Tasks", Event = "TaskEscalated", Description = "Triggered when a task is escalated" },
            new() { Category = "Tasks", Event = "TaskCancelled", Description = "Triggered when a task is cancelled" },

            // Document events
            new() { Category = "Documents", Event = "DocumentCreated", Description = "Triggered when a document is uploaded" },
            new() { Category = "Documents", Event = "DocumentUpdated", Description = "Triggered when a document is updated" },
            new() { Category = "Documents", Event = "DocumentDeleted", Description = "Triggered when a document is deleted" },
            new() { Category = "Documents", Event = "DocumentCheckedOut", Description = "Triggered when a document is checked out" },
            new() { Category = "Documents", Event = "DocumentCheckedIn", Description = "Triggered when a document is checked in" },

            // User events
            new() { Category = "Users", Event = "UserCreated", Description = "Triggered when a user is created" },
            new() { Category = "Users", Event = "UserUpdated", Description = "Triggered when a user is updated" },
            new() { Category = "Users", Event = "UserDeactivated", Description = "Triggered when a user is deactivated" },

            // Content events
            new() { Category = "Content", Event = "ArticlePublished", Description = "Triggered when an article is published" },
            new() { Category = "Content", Event = "ArticleUpdated", Description = "Triggered when an article is updated" },

            // Workflow events
            new() { Category = "Workflow", Event = "ServiceRequestCreated", Description = "Triggered when a service request is created" },
            new() { Category = "Workflow", Event = "ServiceRequestApproved", Description = "Triggered when a service request is approved" },
            new() { Category = "Workflow", Event = "ServiceRequestRejected", Description = "Triggered when a service request is rejected" },

            // Correspondence events
            new() { Category = "Correspondence", Event = "CorrespondenceReceived", Description = "Triggered when correspondence is received" },
            new() { Category = "Correspondence", Event = "CorrespondenceSent", Description = "Triggered when correspondence is sent" },
            new() { Category = "Correspondence", Event = "CorrespondenceAssigned", Description = "Triggered when correspondence is assigned" }
        };
        return Ok(events);
    }

    #endregion
}

#region DTOs

/// <summary>
/// Webhook test result
/// </summary>
public class WebhookTestResult
{
    public bool Success { get; set; }
    public int? StatusCode { get; set; }
    public int? ResponseTimeMs { get; set; }
    public string? ResponseBody { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }
}

/// <summary>
/// Webhook delivery detail DTO
/// </summary>
public class WebhookDeliveryDetailDto : WebhookDeliveryDto
{
    public string? WebhookName { get; set; }
    public Dictionary<string, string>? RequestHeaders { get; set; }
    public string? RequestBody { get; set; }
    public Dictionary<string, string>? ResponseHeaders { get; set; }
    public string? ResponseBody { get; set; }
}

/// <summary>
/// Webhook event type
/// </summary>
public class WebhookEventType
{
    public string Category { get; set; } = string.Empty;
    public string Event { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Intalio Case webhook payload
/// </summary>
public class IntalioCaseWebhookPayload
{
    public string EventId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? ProcessInstanceId { get; set; }
    public string? TaskId { get; set; }
    public string? ProcessKey { get; set; }
    public string? AssigneeId { get; set; }
    public string? Outcome { get; set; }
    public Dictionary<string, object>? Variables { get; set; }
}

/// <summary>
/// Intalio IAM webhook payload
/// </summary>
public class IntalioIAMWebhookPayload
{
    public string EventId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? UserId { get; set; }
    public string? GroupId { get; set; }
    public string? Action { get; set; }
    public Dictionary<string, object>? ChangedAttributes { get; set; }
}

/// <summary>
/// Intalio DMS webhook payload
/// </summary>
public class IntalioDMSWebhookPayload
{
    public string EventId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? DocumentId { get; set; }
    public string? FolderId { get; set; }
    public string? LibraryId { get; set; }
    public string? Action { get; set; }
    public string? UserId { get; set; }
    public int? Version { get; set; }
}

/// <summary>
/// Intalio Correspondence webhook payload
/// </summary>
public class IntalioCorrespondenceWebhookPayload
{
    public string EventId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? CorrespondenceId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public string? AssignedToId { get; set; }
}

#endregion
