using Luxira.Application.DTOs.Common;
using Luxira.Application.DTOs.Notification;

namespace Luxira.Application.Interfaces;

public interface IAdminNotificationService
{
    Task<PagedResult<AdminNotificationDto>> GetAllAsync(int page, int pageSize);
    Task<NotificationUnreadCountDto> GetUnreadCountAsync();
    Task<AdminNotificationDto> MarkReadAsync(Guid id);
    Task MarkAllReadAsync();

    // Stages the notification row only (no SaveChangesAsync) so the caller
    // (OrderService.CreateAsync) persists it atomically together with the
    // order itself in its own single SaveChangesAsync call.
    Task NotifyOrderConfirmedAsync(Guid orderId, string orderNumber, string customerName, decimal orderTotal, string orderCurrency);
}
