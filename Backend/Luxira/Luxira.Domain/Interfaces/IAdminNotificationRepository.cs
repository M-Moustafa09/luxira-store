using Luxira.Domain.Entities;

namespace Luxira.Domain.Interfaces;

public interface IAdminNotificationRepository : IRepository<AdminNotification>
{
    Task<(List<AdminNotification> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
    Task<int> GetUnreadCountAsync();
    Task MarkAllReadAsync();
}
