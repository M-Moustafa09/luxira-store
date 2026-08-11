using Luxira.Application.DTOs.Product;

namespace Luxira.Application.Interfaces;

public interface IWishlistService
{
    Task<List<ProductListItemDto>> GetWishlistAsync();
    Task<List<ProductListItemDto>> AddAsync(Guid productId);
    Task<List<ProductListItemDto>> RemoveAsync(Guid productId);
}
