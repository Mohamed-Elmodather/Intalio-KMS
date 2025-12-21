using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Notifications.Application.DTOs;
using AFC27.KMS.Notifications.Domain.Entities;

namespace AFC27.KMS.Notifications.Presentation.Controllers;

/// <summary>
/// Controller for notification preferences
/// </summary>
[ApiController]
[Route("api/notifications/preferences")]
[Authorize]
public class PreferencesController : ControllerBase
{
    #region User Preferences

    /// <summary>
    /// Get user notification preferences
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(NotificationPreferencesDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<NotificationPreferencesDto>> GetPreferences()
    {
        // TODO: Return preferences
        var preferences = new NotificationPreferencesDto
        {
            NotificationsEnabled = true,
            EmailEnabled = true,
            PushEnabled = true,
            SmsEnabled = false,
            InAppEnabled = true,
            EmailDigestFrequency = DigestFrequency.Instant,
            DigestHour = 9,
            TimeZone = "Asia/Riyadh",
            QuietHoursEnabled = false,
            QuietHoursStart = "22:00",
            QuietHoursEnd = "07:00",
            AllowUrgentDuringQuietHours = true,
            PreferredLanguage = "en",
            CategoryPreferences = new List<CategoryPreferenceDto>
            {
                new()
                {
                    Category = NotificationCategory.Workflow,
                    CategoryName = "Workflow",
                    CategoryNameAr = "سير العمل",
                    Enabled = true,
                    Channels = new List<DeliveryChannel> { DeliveryChannel.InApp, DeliveryChannel.Email }
                },
                new()
                {
                    Category = NotificationCategory.Content,
                    CategoryName = "Content",
                    CategoryNameAr = "المحتوى",
                    Enabled = true,
                    Channels = new List<DeliveryChannel> { DeliveryChannel.InApp }
                }
            }
        };
        return Ok(preferences);
    }

    /// <summary>
    /// Update user notification preferences
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(NotificationPreferencesDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<NotificationPreferencesDto>> UpdatePreferences(
        [FromBody] UpdatePreferencesRequest request)
    {
        // TODO: Update preferences
        return Ok(new NotificationPreferencesDto());
    }

    /// <summary>
    /// Update category preference
    /// </summary>
    [HttpPut("category")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateCategoryPreference(
        [FromBody] UpdateCategoryPreferenceRequest request)
    {
        // TODO: Update category preference
        return NoContent();
    }

    /// <summary>
    /// Update type preference
    /// </summary>
    [HttpPut("type")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateTypePreference(
        [FromBody] UpdateTypePreferenceRequest request)
    {
        // TODO: Update type preference
        return NoContent();
    }

    /// <summary>
    /// Reset preferences to default
    /// </summary>
    [HttpPost("reset")]
    [ProducesResponseType(typeof(NotificationPreferencesDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<NotificationPreferencesDto>> ResetPreferences()
    {
        // TODO: Reset to default
        return Ok(new NotificationPreferencesDto());
    }

    #endregion

    #region Mute/Unmute

    /// <summary>
    /// Get muted entities
    /// </summary>
    [HttpGet("muted")]
    [ProducesResponseType(typeof(IEnumerable<MutedEntityDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MutedEntityDto>>> GetMutedEntities()
    {
        // TODO: Return muted entities
        var muted = new List<MutedEntityDto>();
        return Ok(muted);
    }

    /// <summary>
    /// Mute an entity
    /// </summary>
    [HttpPost("mute")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MuteEntity([FromBody] MuteEntityRequest request)
    {
        // TODO: Mute entity
        return NoContent();
    }

    /// <summary>
    /// Unmute an entity
    /// </summary>
    [HttpDelete("mute/{entityType}/{entityId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UnmuteEntity(string entityType, Guid entityId)
    {
        // TODO: Unmute entity
        return NoContent();
    }

    #endregion

    #region Device Registration

    /// <summary>
    /// Get registered devices
    /// </summary>
    [HttpGet("devices")]
    [ProducesResponseType(typeof(IEnumerable<DeviceRegistrationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DeviceRegistrationDto>>> GetDevices()
    {
        // TODO: Return devices
        var devices = new List<DeviceRegistrationDto>();
        return Ok(devices);
    }

    /// <summary>
    /// Register a device for push notifications
    /// </summary>
    [HttpPost("devices")]
    [ProducesResponseType(typeof(DeviceRegistrationDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<DeviceRegistrationDto>> RegisterDevice(
        [FromBody] RegisterDeviceRequest request)
    {
        // TODO: Register device
        var device = new DeviceRegistrationDto
        {
            Id = Guid.NewGuid(),
            Platform = request.Platform,
            DeviceName = request.DeviceName,
            DeviceModel = request.DeviceModel,
            OsVersion = request.OsVersion,
            BrowserInfo = request.BrowserInfo,
            IsActive = true,
            LastActiveAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        return Created($"/api/notifications/preferences/devices/{device.Id}", device);
    }

    /// <summary>
    /// Update device registration
    /// </summary>
    [HttpPut("devices/{id:guid}")]
    [ProducesResponseType(typeof(DeviceRegistrationDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DeviceRegistrationDto>> UpdateDevice(
        Guid id,
        [FromBody] RegisterDeviceRequest request)
    {
        // TODO: Update device
        return Ok(new DeviceRegistrationDto { Id = id });
    }

    /// <summary>
    /// Remove device registration
    /// </summary>
    [HttpDelete("devices/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveDevice(Guid id)
    {
        // TODO: Remove device
        return NoContent();
    }

    #endregion

    #region Subscriptions

    /// <summary>
    /// Get subscriptions
    /// </summary>
    [HttpGet("subscriptions")]
    [ProducesResponseType(typeof(IEnumerable<SubscriptionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SubscriptionDto>>> GetSubscriptions()
    {
        // TODO: Return subscriptions
        var subscriptions = new List<SubscriptionDto>();
        return Ok(subscriptions);
    }

    /// <summary>
    /// Subscribe to entity notifications
    /// </summary>
    [HttpPost("subscriptions")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<SubscriptionDto>> Subscribe([FromBody] SubscribeRequest request)
    {
        // TODO: Create subscription
        var subscription = new SubscriptionDto
        {
            Id = Guid.NewGuid(),
            EntityType = request.EntityType,
            EntityId = request.EntityId,
            Channels = request.Channels ?? new List<DeliveryChannel> { DeliveryChannel.InApp },
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        return Created($"/api/notifications/preferences/subscriptions/{subscription.Id}", subscription);
    }

    /// <summary>
    /// Unsubscribe from entity notifications
    /// </summary>
    [HttpDelete("subscriptions/{entityType}/{entityId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Unsubscribe(string entityType, Guid entityId)
    {
        // TODO: Remove subscription
        return NoContent();
    }

    #endregion
}
