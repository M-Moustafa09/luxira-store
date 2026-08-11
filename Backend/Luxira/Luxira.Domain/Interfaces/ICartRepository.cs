using Luxira.Domain.Entities;

namespace Luxira.Domain.Interfaces;

public interface ICartRepository : IRepository<Cart>
{
    Task<Cart?> GetByCustomerIdWithItemsAsync(Guid customerId);
    Task AddItemAsync(CartItem item);
    void RemoveItem(CartItem item);
    Task AddBundleItemAsync(BundleCartItem item);
    void RemoveBundleItem(BundleCartItem item);
}
