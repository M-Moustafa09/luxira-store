using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Luxira.Infrastructure.Repositories;

public class CartRepository : RepositoryBase<Cart>, ICartRepository
{
    public CartRepository(LuxiraDbContext context) : base(context)
    {
    }

    public Task<Cart?> GetByCustomerIdWithItemsAsync(Guid customerId) =>
        DbSet
            .Include(c => c.Items).ThenInclude(i => i.Product)
            .Include(c => c.Items).ThenInclude(i => i.ProductVariant)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

    public Task AddItemAsync(CartItem item) => Context.Set<CartItem>().AddAsync(item).AsTask();

    public void RemoveItem(CartItem item) => Context.Set<CartItem>().Remove(item);
}
