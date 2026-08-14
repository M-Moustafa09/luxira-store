using FluentValidation;
using Luxira.Application.DTOs.Common;
using Luxira.Application.DTOs.Product;
using Luxira.Application.Interfaces;
using Luxira.Domain.Common;
using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Luxira.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICountryResolver _countryResolver;
    private readonly IValidator<SaveProductRequest> _saveProductValidator;
    private readonly IValidator<ReplaceCountryPricesRequest> _replaceCountryPricesValidator;

    public ProductService(
        IUnitOfWork unitOfWork,
        ICountryResolver countryResolver,
        IValidator<SaveProductRequest> saveProductValidator,
        IValidator<ReplaceCountryPricesRequest> replaceCountryPricesValidator)
    {
        _unitOfWork = unitOfWork;
        _countryResolver = countryResolver;
        _saveProductValidator = saveProductValidator;
        _replaceCountryPricesValidator = replaceCountryPricesValidator;
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

        var dtos = items.Adapt<List<ProductListItemDto>>();
        await ApplyCountryPricingAsync(dtos, items.Select(p => p.Id).ToList());

        return new PagedResult<ProductListItemDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ProductDetailDto?> GetByIdAsync(Guid id)
    {
        var product = await _unitOfWork.Products.GetByIdWithVariantsAsync(id);
        if (product is null)
        {
            return null;
        }

        var dto = product.Adapt<ProductDetailDto>();
        await ApplyCountryPricingAsync(dto, id);
        return dto;
    }

    public async Task<List<ProductListItemDto>> GetRelatedAsync(Guid id, int take)
    {
        var product = await _unitOfWork.Products.GetByIdWithVariantsAsync(id);
        if (product is null)
        {
            return new List<ProductListItemDto>();
        }

        var related = await _unitOfWork.Products.GetRelatedAsync(id, product.CategoryId, take);
        var dtos = related.Adapt<List<ProductListItemDto>>();
        await ApplyCountryPricingAsync(dtos, related.Select(p => p.Id).ToList());

        return dtos;
    }

    // Country pricing display is currently scoped to product listing/detail only
    // (not price-range filtering/sorting, which still operates on the base USD
    // Product.Price - a known, deliberate gap flagged for a later pass).
    private async Task ApplyCountryPricingAsync(ProductDetailDto dto, Guid productId)
    {
        var country = await _countryResolver.ResolveAsync();
        if (country is null)
        {
            dto.Currency = CountryCurrency.FallbackCurrency;
            return;
        }

        var countryPrice = await _unitOfWork.Products.GetCountryPriceAsync(productId, country.Value);
        if (countryPrice is null)
        {
            dto.Currency = CountryCurrency.FallbackCurrency;
            return;
        }

        dto.Price = countryPrice.Price;
        dto.OldPrice = null;
        dto.Discount = null;
        dto.Currency = CountryCurrency.For(country.Value);
    }

    private async Task ApplyCountryPricingAsync(List<ProductListItemDto> dtos, List<Guid> productIds)
    {
        foreach (var dto in dtos)
        {
            dto.Currency = CountryCurrency.FallbackCurrency;
        }

        var country = await _countryResolver.ResolveAsync();
        if (country is null || productIds.Count == 0)
        {
            return;
        }

        var countryPrices = await _unitOfWork.Products.GetCountryPricesForProductsAsync(productIds, country.Value);
        if (countryPrices.Count == 0)
        {
            return;
        }

        var priceByProductId = countryPrices.ToDictionary(p => p.ProductId, p => p.Price);
        var currency = CountryCurrency.For(country.Value);

        foreach (var dto in dtos)
        {
            if (priceByProductId.TryGetValue(dto.Id, out var price))
            {
                dto.Price = price;
                dto.OldPrice = null;
                dto.Discount = null;
                dto.Currency = currency;
            }
        }
    }

    public async Task<ProductDetailDto> CreateAsync(SaveProductRequest request)
    {
        await _saveProductValidator.ValidateAndThrowAsync(request);
        await EnsureCategoryAndBrandExistAsync(request.CategoryId, request.BrandId);

        Enum.TryParse<SkinType>(request.SkinType, ignoreCase: true, out var skinType);

        var product = new Domain.Entities.Product
        {
            Name = request.Name.Trim(),
            Subtitle = request.Subtitle.Trim(),
            Description = request.Description.Trim(),
            MainImageUrl = request.MainImageUrl.Trim(),
            Price = request.Price,
            OldPrice = request.OldPrice,
            IsNew = request.IsNew,
            IsBestSeller = request.IsBestSeller,
            SortOrder = request.SortOrder,
            CategoryId = request.CategoryId,
            BrandId = request.BrandId,
            SkinType = string.IsNullOrWhiteSpace(request.SkinType) ? null : skinType,
            Variants = request.Variants.Select(v => new ProductVariant
            {
                Label = v.Label.Trim(),
                ColorHex = v.ColorHex.Trim(),
                ImageUrl = v.ImageUrl.Trim(),
                SortOrder = v.SortOrder,
                Stock = v.Stock
            }).ToList()
        };

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        var created = await _unitOfWork.Products.GetByIdWithVariantsAsync(product.Id)
            ?? throw new KeyNotFoundException("المنتج غير موجود");

        return created.Adapt<ProductDetailDto>();
    }

    public async Task<ProductDetailDto> UpdateAsync(Guid id, SaveProductRequest request)
    {
        await _saveProductValidator.ValidateAndThrowAsync(request);
        await EnsureCategoryAndBrandExistAsync(request.CategoryId, request.BrandId);

        var product = await _unitOfWork.Products.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("المنتج غير موجود");

        Enum.TryParse<SkinType>(request.SkinType, ignoreCase: true, out var skinType);

        product.Name = request.Name.Trim();
        product.Subtitle = request.Subtitle.Trim();
        product.Description = request.Description.Trim();
        product.MainImageUrl = request.MainImageUrl.Trim();
        product.Price = request.Price;
        product.OldPrice = request.OldPrice;
        product.IsNew = request.IsNew;
        product.IsBestSeller = request.IsBestSeller;
        product.SortOrder = request.SortOrder;
        product.CategoryId = request.CategoryId;
        product.BrandId = request.BrandId;
        product.SkinType = string.IsNullOrWhiteSpace(request.SkinType) ? null : skinType;

        var newVariants = request.Variants.Select(v => new ProductVariant
        {
            Id = v.Id ?? Guid.NewGuid(),
            ProductId = id,
            Label = v.Label.Trim(),
            ColorHex = v.ColorHex.Trim(),
            ImageUrl = v.ImageUrl.Trim(),
            SortOrder = v.SortOrder,
            Stock = v.Stock
        }).ToList();

        await _unitOfWork.Products.UpsertVariantsAsync(id, newVariants);

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new ValidationException(
            [
                new FluentValidation.Results.ValidationFailure(nameof(request.Variants), "لا يمكن حذف بعض الدرجات لأنها مستخدمة في سلة حالية")
            ]);
        }

        var updated = await _unitOfWork.Products.GetByIdWithVariantsAsync(id)
            ?? throw new KeyNotFoundException("المنتج غير موجود");

        return updated.Adapt<ProductDetailDto>();
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("المنتج غير موجود");

        try
        {
            _unitOfWork.Products.Remove(product);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new ValidationException(
            [
                new FluentValidation.Results.ValidationFailure(nameof(id), "لا يمكن حذف هذا المنتج لأنه مستخدم في سلة أو باقة حالية")
            ]);
        }
    }

    public async Task<List<ProductCountryPriceDto>> GetCountryPricesAsync(Guid productId)
    {
        await EnsureProductExistsAsync(productId);

        var prices = await _unitOfWork.Products.GetCountryPricesAsync(productId);
        return prices.Select(ToCountryPriceDto).ToList();
    }

    public async Task<List<ProductCountryPriceDto>> ReplaceCountryPricesAsync(Guid productId, ReplaceCountryPricesRequest request)
    {
        await _replaceCountryPricesValidator.ValidateAndThrowAsync(request);
        await EnsureProductExistsAsync(productId);

        var entities = request.Prices.Select(p => new ProductCountryPrice
        {
            ProductId = productId,
            Country = Enum.Parse<Country>(p.Country, ignoreCase: true),
            Price = p.Price
        }).ToList();

        await _unitOfWork.Products.ReplaceCountryPricesAsync(productId, entities);
        await _unitOfWork.SaveChangesAsync();

        var saved = await _unitOfWork.Products.GetCountryPricesAsync(productId);
        return saved.Select(ToCountryPriceDto).ToList();
    }

    private static ProductCountryPriceDto ToCountryPriceDto(ProductCountryPrice p) => new()
    {
        Country = p.Country.ToString(),
        Currency = CountryCurrency.For(p.Country),
        Price = p.Price
    };

    private async Task EnsureProductExistsAsync(Guid productId)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId);
        if (product is null)
        {
            throw new KeyNotFoundException("المنتج غير موجود");
        }
    }

    private async Task EnsureCategoryAndBrandExistAsync(Guid categoryId, Guid? brandId)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
        if (category is null)
        {
            throw new ValidationException(
            [
                new FluentValidation.Results.ValidationFailure(nameof(categoryId), "التصنيف غير موجود")
            ]);
        }

        if (brandId.HasValue)
        {
            var brand = await _unitOfWork.Brands.GetByIdAsync(brandId.Value);
            if (brand is null)
            {
                throw new ValidationException(
                [
                    new FluentValidation.Results.ValidationFailure(nameof(brandId), "العلامة التجارية غير موجودة")
                ]);
            }
        }
    }
}
