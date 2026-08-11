using Luxira.Application.DTOs.Common;
using Luxira.Application.DTOs.Product;

namespace Luxira.Application.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductListItemDto>> GetProductsAsync(ProductListQuery query);
    Task<ProductDetailDto?> GetByIdAsync(Guid id);
    Task<List<ProductListItemDto>> GetRelatedAsync(Guid id, int take);
}
