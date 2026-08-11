using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Luxira.Infrastructure.Repositories;

public class WishlistRepository : RepositoryBase<WishlistItem>, IWishlistRepository
{
    public WishlistRepository(LuxiraDbContext context) : base(context)
    {
    }

    public Task<List<WishlistItem>> GetByCustomerIdWithProductsAsync(Guid customerId) =>
        DbSet.AsNoTracking()
            .Include(w => w.Product).ThenInclude(p => p.Variants)
            .Where(w => w.CustomerId == customerId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();

    public Task<WishlistItem?> FindAsync(Guid customerId, Guid productId) =>
        DbSet.FirstOrDefaultAsync(w => w.CustomerId == customerId && w.ProductId == productId);
}
