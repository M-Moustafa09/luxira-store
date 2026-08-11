using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Luxira.Infrastructure.Repositories;

public class OrderRepository : RepositoryBase<Order>, IOrderRepository
{
    public OrderRepository(LuxiraDbContext context) : base(context)
    {
    }

    public Task<Order?> GetByIdWithDetailsAsync(Guid id) =>
        DbSet.AsNoTracking()
            .Include(o => o.Items)
            .Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.Id == id);

    public Task<Order?> FindByOrderNumberAndPhoneAsync(string orderNumber, string phone) =>
        DbSet.AsNoTracking()
            .Include(o => o.Items)
            .Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber && o.Phone == phone);

    public Task<bool> OrderNumberExistsAsync(string orderNumber) =>
        DbSet.AsNoTracking().AnyAsync(o => o.OrderNumber == orderNumber);

    public async Task<(List<Order> Items, int TotalCount)> GetByCustomerAsync(Guid customerId, int page, int pageSize)
    {
        var query = DbSet.AsNoTracking()
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(o => o.Items)
            .Include(o => o.StatusHistory)
            .ToListAsync();

        return (items, totalCount);
    }
}
