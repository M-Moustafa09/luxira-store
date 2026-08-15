using Luxira.Application.DTOs.Common;
using Luxira.Application.DTOs.Notification;
using Luxira.Application.Interfaces;
using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Mapster;

namespace Luxira.Infrastructure.Services;

public class AdminNotificationService : IAdminNotificationService
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminNotificationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<AdminNotificationDto>> GetAllAsync(int page, int pageSize)
    {
        var (items, totalCount) = await _unitOfWork.AdminNotifications.GetPagedAsync(page, pageSize);

        return new PagedResult<AdminNotificationDto>
        {
            Items = items.Adapt<List<AdminNotificationDto>>(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<NotificationUnreadCountDto> GetUnreadCountAsync()
    {
        var count = await _unitOfWork.AdminNotifications.GetUnreadCountAsync();
        return new NotificationUnreadCountDto { Count = count };
    }

    public async Task<AdminNotificationDto> MarkReadAsync(Guid id)
    {
        var notification = await _unitOfWork.AdminNotifications.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("الإشعار غير موجود");

        notification.IsRead = true;
        await _unitOfWork.SaveChangesAsync();

        return notification.Adapt<AdminNotificationDto>();
    }

    public async Task MarkAllReadAsync()
    {
        await _unitOfWork.AdminNotifications.MarkAllReadAsync();
        await _unitOfWork.SaveChangesAsync();
    }

    public Task NotifyOrderConfirmedAsync(Guid orderId, string orderNumber, string customerName, decimal orderTotal, string orderCurrency)
    {
        var notification = new AdminNotification
        {
            Type = AdminNotificationType.OrderConfirmed,
            Message = $"طلب جديد #{orderNumber} من {customerName}",
            OrderId = orderId,
            OrderNumber = orderNumber,
            CustomerName = customerName,
            OrderTotal = orderTotal,
            OrderCurrency = orderCurrency
        };

        return _unitOfWork.AdminNotifications.AddAsync(notification);
    }
}
