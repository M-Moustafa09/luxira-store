using Luxira.Application.DTOs.Common;
using Luxira.Application.DTOs.Notification;
using Luxira.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Controllers;

[ApiController]
[Route("api/admin/notifications")]
[Authorize(Roles = "Admin")]
public class AdminNotificationsController : ControllerBase
{
    private readonly IAdminNotificationService _notificationService;

    public AdminNotificationsController(IAdminNotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminNotificationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AdminNotificationDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var notifications = await _notificationService.GetAllAsync(page, pageSize);
        return Ok(notifications);
    }

    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(NotificationUnreadCountDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<NotificationUnreadCountDto>> GetUnreadCount()
    {
        var count = await _notificationService.GetUnreadCountAsync();
        return Ok(count);
    }

    [HttpPut("{id:guid}/read")]
    [ProducesResponseType(typeof(AdminNotificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminNotificationDto>> MarkRead(Guid id)
    {
        var notification = await _notificationService.MarkReadAsync(id);
        return Ok(notification);
    }

    [HttpPut("read-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAllRead()
    {
        await _notificationService.MarkAllReadAsync();
        return NoContent();
    }
}
