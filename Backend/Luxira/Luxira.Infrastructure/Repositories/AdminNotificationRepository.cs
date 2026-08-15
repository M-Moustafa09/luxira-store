using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Luxira.Infrastructure.Repositories;

public class AdminNotificationRepository : RepositoryBase<AdminNotification>, IAdminNotificationRepository
{
    public AdminNotificationRepository(LuxiraDbContext context) : base(context)
    {
    }

    public async Task<(List<AdminNotification> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
    {
        var query = DbSet.AsNoTracking().OrderByDescending(n => n.CreatedAt);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public Task<int> GetUnreadCountAsync() =>
        DbSet.AsNoTracking().CountAsync(n => !n.IsRead);

    public async Task MarkAllReadAsync()
    {
        var unread = await DbSet.Where(n => !n.IsRead).ToListAsync();

        foreach (var notification in unread)
        {
            notification.IsRead = true;
        }
    }
}
