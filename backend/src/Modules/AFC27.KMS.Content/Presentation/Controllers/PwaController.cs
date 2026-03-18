using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AFC27.KMS.Content.Presentation.Controllers;

/// <summary>
/// Controller providing PWA (Progressive Web App) support endpoints (Phase 9B).
/// Serves the web-app manifest and manages push-notification subscriptions.
/// </summary>
[ApiController]
[Route("api/content/pwa")]
public class PwaController : ControllerBase
{
    private readonly ILogger<PwaController> _logger;

    // In-memory store for push subscriptions (replace with repository in production)
    private static readonly List<PushSubscriptionDto> _subscriptions = new();

    public PwaController(ILogger<PwaController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Returns the PWA manifest (manifest.json) for the AFC27 KMS application.
    /// This enables "Add to Home Screen" on mobile browsers and desktop PWA installation.
    /// </summary>
    [HttpGet("manifest")]
    [AllowAnonymous]
    [Produces("application/manifest+json")]
    [ProducesResponseType(typeof(PwaManifestDto), StatusCodes.Status200OK)]
    public ActionResult<PwaManifestDto> GetManifest()
    {
        var manifest = new PwaManifestDto
        {
            Name = "AFC27 Knowledge Management System",
            ShortName = "AFC27 KMS",
            Description = "Official Knowledge Management System for the AFC Asian Cup Saudi Arabia 2027",
            StartUrl = "/",
            Display = "standalone",
            BackgroundColor = "#ffffff",
            ThemeColor = "#1a1a2e",
            Orientation = "any",
            Scope = "/",
            Lang = "en",
            Dir = "auto",
            Categories = new List<string> { "productivity", "business", "education" },
            Icons = new List<PwaIconDto>
            {
                new() { Src = "/icons/icon-72x72.png", Sizes = "72x72", Type = "image/png" },
                new() { Src = "/icons/icon-96x96.png", Sizes = "96x96", Type = "image/png" },
                new() { Src = "/icons/icon-128x128.png", Sizes = "128x128", Type = "image/png" },
                new() { Src = "/icons/icon-144x144.png", Sizes = "144x144", Type = "image/png" },
                new() { Src = "/icons/icon-152x152.png", Sizes = "152x152", Type = "image/png" },
                new() { Src = "/icons/icon-192x192.png", Sizes = "192x192", Type = "image/png", Purpose = "any maskable" },
                new() { Src = "/icons/icon-384x384.png", Sizes = "384x384", Type = "image/png" },
                new() { Src = "/icons/icon-512x512.png", Sizes = "512x512", Type = "image/png", Purpose = "any maskable" }
            },
            Shortcuts = new List<PwaShortcutDto>
            {
                new() { Name = "Search Knowledge", ShortName = "Search", Url = "/search", Description = "Search the knowledge base" },
                new() { Name = "My Articles", ShortName = "Articles", Url = "/my-articles", Description = "View your articles" },
                new() { Name = "Create Article", ShortName = "New", Url = "/articles/new", Description = "Create a new article" }
            },
            Screenshots = new List<PwaScreenshotDto>
            {
                new() { Src = "/screenshots/desktop-home.png", Sizes = "1920x1080", Type = "image/png", FormFactor = "wide", Label = "Home page on desktop" },
                new() { Src = "/screenshots/mobile-search.png", Sizes = "750x1334", Type = "image/png", FormFactor = "narrow", Label = "Search on mobile" }
            }
        };

        return Ok(manifest);
    }

    /// <summary>
    /// Register a push-notification subscription.
    /// The browser sends a PushSubscription object after the user grants
    /// notification permission. The server stores it for later push delivery.
    /// </summary>
    [HttpPost("push-subscription")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult RegisterPushSubscription([FromBody] PushSubscriptionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Endpoint))
            return BadRequest(new { error = "Push subscription endpoint is required" });

        // TODO: Extract current user from claims
        var userId = Guid.Empty;

        var subscription = new PushSubscriptionDto
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Endpoint = request.Endpoint,
            P256dhKey = request.Keys?.P256dh ?? string.Empty,
            AuthKey = request.Keys?.Auth ?? string.Empty,
            UserAgent = Request.Headers.UserAgent.ToString(),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        // Remove any existing subscription for the same endpoint
        _subscriptions.RemoveAll(s => s.Endpoint == request.Endpoint);
        _subscriptions.Add(subscription);

        _logger.LogInformation(
            "Push subscription registered for user {UserId}, endpoint: {Endpoint}",
            userId, request.Endpoint[..Math.Min(60, request.Endpoint.Length)] + "...");

        // TODO: Persist to database via repository

        return Created($"/api/content/pwa/push-subscription/{subscription.Id}", new
        {
            subscription.Id,
            Message = "Push subscription registered successfully"
        });
    }

    /// <summary>
    /// Remove a push-notification subscription.
    /// Called when the user revokes notification permission or unsubscribes.
    /// </summary>
    [HttpDelete("push-subscription/{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult RemovePushSubscription(Guid id)
    {
        var sub = _subscriptions.FirstOrDefault(s => s.Id == id);
        if (sub == null) return NotFound();

        _subscriptions.Remove(sub);
        _logger.LogInformation("Push subscription {Id} removed", id);
        return NoContent();
    }

    /// <summary>
    /// List active push subscriptions for the current user.
    /// </summary>
    [HttpGet("push-subscriptions")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<PushSubscriptionDto>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<PushSubscriptionDto>> ListPushSubscriptions()
    {
        // TODO: Extract current user from claims
        var userId = Guid.Empty;
        var subs = _subscriptions.Where(s => s.UserId == userId && s.IsActive);
        return Ok(subs);
    }
}

#region PWA DTOs

/// <summary>
/// PWA Web App Manifest (W3C spec).
/// </summary>
public class PwaManifestDto
{
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string StartUrl { get; set; } = "/";
    public string Display { get; set; } = "standalone";
    public string BackgroundColor { get; set; } = "#ffffff";
    public string ThemeColor { get; set; } = "#1a1a2e";
    public string? Orientation { get; set; }
    public string? Scope { get; set; }
    public string? Lang { get; set; }
    public string? Dir { get; set; }
    public List<string> Categories { get; set; } = new();
    public List<PwaIconDto> Icons { get; set; } = new();
    public List<PwaShortcutDto> Shortcuts { get; set; } = new();
    public List<PwaScreenshotDto> Screenshots { get; set; } = new();
}

public class PwaIconDto
{
    public string Src { get; set; } = string.Empty;
    public string Sizes { get; set; } = string.Empty;
    public string Type { get; set; } = "image/png";
    public string? Purpose { get; set; }
}

public class PwaShortcutDto
{
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class PwaScreenshotDto
{
    public string Src { get; set; } = string.Empty;
    public string Sizes { get; set; } = string.Empty;
    public string Type { get; set; } = "image/png";
    public string? FormFactor { get; set; }
    public string? Label { get; set; }
}

/// <summary>
/// Browser push subscription request (from the Push API).
/// </summary>
public class PushSubscriptionRequest
{
    public string Endpoint { get; set; } = string.Empty;
    public PushSubscriptionKeys? Keys { get; set; }
}

public class PushSubscriptionKeys
{
    public string P256dh { get; set; } = string.Empty;
    public string Auth { get; set; } = string.Empty;
}

/// <summary>
/// Stored push subscription.
/// </summary>
public class PushSubscriptionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Endpoint { get; set; } = string.Empty;
    public string P256dhKey { get; set; } = string.Empty;
    public string AuthKey { get; set; } = string.Empty;
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

#endregion
