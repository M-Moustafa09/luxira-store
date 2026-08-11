using Luxira.Application.DTOs.Common;
using Luxira.Application.DTOs.Product;
using Luxira.Application.Interfaces;
using Luxira.Domain.Interfaces;
using Mapster;

namespace Luxira.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<ProductListItemDto>> GetProductsAsync(ProductListQuery query)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        Enum.TryParse<Domain.Entities.SkinType>(query.SkinType, ignoreCase: true, out var skinType);

        var criteria = new ProductSearchCriteria
        {
            Search = query.Search,
            CategoryId = query.CategoryId,
            BrandId = query.BrandId,
            IsNew = query.IsNew,
            IsBestSeller = query.IsBestSeller,
            MinPrice = query.MinPrice,
            MaxPrice = query.MaxPrice,
            MinRating = query.MinRating,
            SkinType = string.IsNullOrWhiteSpace(query.SkinType) ? null : skinType,
            Sort = (ProductSortOption)(int)query.Sort,
            Page = page,
            PageSize = pageSize
        };

        var (items, totalCount) = await _unitOfWork.Products.SearchAsync(criteria);

        return new PagedResult<ProductListItemDto>
        {
            Items = items.Adapt<List<ProductListItemDto>>(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ProductDetailDto?> GetByIdAsync(Guid id)
    {
        var product = await _unitOfWork.Products.GetByIdWithVariantsAsync(id);
        return product?.Adapt<ProductDetailDto>();
    }

    public async Task<List<ProductListItemDto>> GetRelatedAsync(Guid id, int take)
    {
        var product = await _unitOfWork.Products.GetByIdWithVariantsAsync(id);
        if (product is null)
        {
            return new List<ProductListItemDto>();
        }

        var related = await _unitOfWork.Products.GetRelatedAsync(id, product.CategoryId, take);
        return related.Adapt<List<ProductListItemDto>>();
    }
}
