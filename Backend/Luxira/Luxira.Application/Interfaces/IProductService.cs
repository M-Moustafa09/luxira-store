using Luxira.Application.DTOs.Common;
using Luxira.Application.DTOs.Product;

namespace Luxira.Application.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductListItemDto>> GetProductsAsync(ProductListQuery query);
    Task<ProductDetailDto?> GetByIdAsync(Guid id);
    Task<List<ProductListItemDto>> GetRelatedAsync(Guid id, int take);

    Task<ProductDetailDto> CreateAsync(SaveProductRequest request);
    Task<ProductDetailDto> UpdateAsync(Guid id, SaveProductRequest request);
    Task DeleteAsync(Guid id);

    Task<List<ProductCountryPriceDto>> GetCountryPricesAsync(Guid productId);
    Task<List<ProductCountryPriceDto>> ReplaceCountryPricesAsync(Guid productId, ReplaceCountryPricesRequest request);
}
