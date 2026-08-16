using Luxira.Application.DTOs.Bundle;
using Luxira.Application.DTOs.Cart;
using Luxira.Application.DTOs.Product;
using Luxira.Application.DTOs.Review;
using Luxira.Domain.Entities;
using Mapster;

namespace Luxira.Infrastructure.Mapping;

public static class MappingConfig
{
    public static void Configure()
    {
        // CustomerAddress -> AddressDto and Customer -> CustomerProfileDto need no
        // custom mapping (property names match 1:1), so no explicit config here —
        // Mapster's convention-based mapping handles them.

        TypeAdapterConfig<Product, ProductListItemDto>.NewConfig()
            .Map(dest => dest.ImageUrl, src => src.MainImageUrl)
            .Map(dest => dest.Discount, src => CalculateDiscount(src.Price, src.OldPrice))
            .Map(dest => dest.BrandName, src => src.Brand != null ? src.Brand.Name : null)
            .Map(dest => dest.SkinType, src => src.SkinType != null ? src.SkinType.ToString() : null)
            .Map(dest => dest.Variant, src => src.Variants
                .OrderBy(v => v.SortOrder)
                .Select(v => new ProductVariantSummaryDto { Label = v.Label, ColorHex = v.ColorHex })
                .FirstOrDefault())
            .Map(dest => dest.InStock, src => src.Variants.Any(v => v.Stock > 0));

        TypeAdapterConfig<Product, ProductDetailDto>.NewConfig()
            .Map(dest => dest.Discount, src => CalculateDiscount(src.Price, src.OldPrice))
            .Map(dest => dest.BrandName, src => src.Brand != null ? src.Brand.Name : null)
            .Map(dest => dest.SkinType, src => src.SkinType != null ? src.SkinType.ToString() : null)
            .Map(dest => dest.Variants, src => src.Variants.OrderBy(v => v.SortOrder));

        TypeAdapterConfig<CartItem, CartItemDto>.NewConfig()
            .Map(dest => dest.ProductName, src => src.Product.Name)
            .Map(dest => dest.ProductSubtitle, src => src.Product.Subtitle)
            .Map(dest => dest.ProductImageUrl, src => src.Product.MainImageUrl)
            .Map(dest => dest.VariantId, src => src.ProductVariantId)
            .Map(dest => dest.VariantLabel, src => src.ProductVariant.Label)
            .Map(dest => dest.VariantColorHex, src => src.ProductVariant.ColorHex)
            .Map(dest => dest.UnitPrice, src => src.Product.Price)
            .Map(dest => dest.LineTotal, src => src.Product.Price * src.Quantity);

        TypeAdapterConfig<BundleCartItem, BundleCartItemDto>.NewConfig()
            .Map(dest => dest.BundleName, src => src.Bundle.Name)
            .Map(dest => dest.BundleImageUrl, src => src.Bundle.MainImageUrl)
            .Map(dest => dest.LineTotal, src => src.UnitPrice * src.Quantity);

        TypeAdapterConfig<Domain.Entities.Bundle, BundleDto>.NewConfig()
            .Map(dest => dest.ImageUrl, src => src.MainImageUrl)
            .Map(dest => dest.Discount, src => CalculateDiscount(src.Price, src.OldPrice))
            .Map(dest => dest.ProductsCount, src => src.Items.Count);

        TypeAdapterConfig<Domain.Entities.Bundle, BundleDetailDto>.NewConfig()
            .Map(dest => dest.ImageUrl, src => src.MainImageUrl)
            .Map(dest => dest.Discount, src => CalculateDiscount(src.Price, src.OldPrice));

        TypeAdapterConfig<BundleItem, BundleItemDto>.NewConfig()
            .Map(dest => dest.ProductName, src => src.Product.Name)
            .Map(dest => dest.ProductImageUrl, src => src.Product.MainImageUrl);

        // Product nav is only loaded for the admin moderation list
        // (IReviewRepository.GetPagedAllAsync) - null-guarded so storefront
        // queries (which don't Include Product) map safely too.
        TypeAdapterConfig<Review, ReviewDto>.NewConfig()
            .Map(dest => dest.ProductName, src => src.Product != null ? src.Product.Name : null)
            .Map(dest => dest.ProductImageUrl, src => src.Product != null ? src.Product.MainImageUrl : null);
    }

    private static int? CalculateDiscount(decimal price, decimal? oldPrice)
    {
        if (oldPrice is null or 0)
        {
            return null;
        }

        return (int)Math.Round((1 - price / oldPrice.Value) * 100);
    }
}
