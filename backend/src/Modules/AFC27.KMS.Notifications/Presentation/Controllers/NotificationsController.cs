using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AFC27.KMS.Notifications.Application.DTOs;
using AFC27.KMS.Notifications.Domain.Entities;

namespace AFC27.KMS.Notifications.Presentation.Controllers;

/// <summary>
/// Controller for user notifications
/// </summary>
[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    #region User Notifications

    /// <summary>
    /// Get user notifications
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications(
        [FromQuery] NotificationFilterRequest filter)
    {
        // TODO: Return notifications
        var notifications = new List<NotificationDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Type = NotificationType.TaskAssigned,
                Category = NotificationCategory.Workflow,
                Priority = NotificationPriority.High,
                Title = "New Task Assigned",
                TitleAr = "مهمة جديدة مسندة",
                Message = "You have been assigned a new approval task",
                MessageAr = "تم تعيين مهمة موافقة جديدة لك",
                Icon = "pi-check-square",
                IconColor = "#3B82F6",
                ActionUrl = "/workflow/tasks/123",
                ActorName = "Ahmed Hassan",
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddMinutes(-30),
                TimeAgo = "30 minutes ago"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Type = NotificationType.ContentCommented,
                Category = NotificationCategory.Content,
                Priority = NotificationPriority.Normal,
                Title = "New Comment",
                TitleAr = "تعليق جديد",
                Message = "Someone commented on your article",
                MessageAr = "علق شخص ما على مقالتك",
                Icon = "pi-comment",
                IconColor = "#10B981",
                ActionUrl = "/content/456",
                ActorName = "Sara Ali",
                ActorAvatarUrl = "/avatars/sara.jpg",
                IsRead = true,
                ReadAt = DateTime.UtcNow.AddHours(-1),
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                TimeAgo = "2 hours ago"
            }
        };
        return Ok(notifications);
    }

    /// <summary>
    /// Get notification by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotificationDto>> GetNotification(Guid id)
    {
        // TODO: Return notification
        return NotFound();
    }

    /// <summary>
    /// Get notification statistics
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(NotificationStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<NotificationStatsDto>> GetStats()
    {
        // TODO: Return stats
        var stats = new NotificationStatsDto
        {
            TotalCount = 45,
            UnreadCount = 8,
            TodayCount = 5,
            ByCategory = new Dictionary<NotificationCategory, int>
            {
                [NotificationCategory.Workflow] = 15,
                [NotificationCategory.Content] = 12,
                [NotificationCategory.Collaboration] = 10,
                [NotificationCategory.Calendar] = 8
            }
        };
        return Ok(stats);
    }

    /// <summary>
    /// Get unread count
    /// </summary>
    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        // TODO: Return unread count
        return Ok(8);
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    [HttpPost("{id:guid}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        // TODO: Mark as read
        return NoContent();
    }

    /// <summary>
    /// Mark notification as unread
    /// </summary>
    [HttpPost("{id:guid}/unread")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAsUnread(Guid id)
    {
        // TODO: Mark as unread
        return NoContent();
    }

    /// <summary>
    /// Mark multiple notifications as read
    /// </summary>
    [HttpPost("read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkMultipleAsRead([FromBody] MarkNotificationsRequest request)
    {
        // TODO: Mark multiple as read
        return NoContent();
    }

    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    [HttpPost("read-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAllAsRead([FromQuery] NotificationCategory? category = null)
    {
        // TODO: Mark all as read
        return NoContent();
    }

    /// <summary>
    /// Archive notification
    /// </summary>
    [HttpPost("{id:guid}/archive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Archive(Guid id)
    {
        // TODO: Archive notification
        return NoContent();
    }

    /// <summary>
    /// Delete notification
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        // TODO: Delete notification
        return NoContent();
    }

    /// <summary>
    /// Delete all notifications
    /// </summary>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAll(
        [FromQuery] bool onlyRead = true,
        [FromQuery] NotificationCategory? category = null)
    {
        // TODO: Delete all
        return NoContent();
    }

    #endregion

    #region Notification Types

    /// <summary>
    /// Get notification types
    /// </summary>
    [HttpGet("types")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<object>> GetNotificationTypes()
    {
        var types = Enum.GetValues<NotificationType>()
            .Select(t => new { Value = (int)t, Name = t.ToString() });
        return Ok(types);
    }

    /// <summary>
    /// Get notification categories
    /// </summary>
    [HttpGet("categories")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<object>> GetCategories()
    {
        var categories = Enum.GetValues<NotificationCategory>()
            .Select(c => new { Value = (int)c, Name = c.ToString() });
        return Ok(categories);
    }

    /// <summary>
    /// Get available channels
    /// </summary>
    [HttpGet("channels")]
    [ProducesResponseType(typeof(IEnumerable<ChannelInfoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ChannelInfoDto>>> GetChannels()
    {
        var channels = new List<ChannelInfoDto>
        {
            new()
            {
                Channel = DeliveryChannel.InApp,
                Name = "In-App",
                NameAr = "داخل التطبيق",
                Description = "Notifications within the application",
                DescriptionAr = "الإشعارات داخل التطبيق",
                Icon = "pi-bell",
                IsAvailable = true,
                IsSetUp = true
            },
            new()
            {
                Channel = DeliveryChannel.Email,
                Name = "Email",
                NameAr = "البريد الإلكتروني",
                Description = "Receive notifications via email",
                DescriptionAr = "استلام الإشعارات عبر البريد الإلكتروني",
                Icon = "pi-envelope",
                IsAvailable = true,
                IsSetUp = true
            },
            new()
            {
                Channel = DeliveryChannel.Push,
                Name = "Push Notifications",
                NameAr = "إشعارات الدفع",
                Description = "Browser and mobile push notifications",
                DescriptionAr = "إشعارات الدفع للمتصفح والجوال",
                Icon = "pi-mobile",
                IsAvailable = true,
                RequiresSetup = true,
                IsSetUp = false
            }
        };
        return Ok(channels);
    }

    #endregion
}
