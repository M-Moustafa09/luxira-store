using Luxira.Domain.Entities;

namespace Luxira.Domain.Interfaces;

public interface IWishlistRepository : IRepository<WishlistItem>
{
    Task<List<WishlistItem>> GetByCustomerIdWithProductsAsync(Guid customerId);
    Task<WishlistItem?> FindAsync(Guid customerId, Guid productId);
}
