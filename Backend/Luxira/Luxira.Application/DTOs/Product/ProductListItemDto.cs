namespace Luxira.Application.DTOs.Product;

public class ProductListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsNew { get; set; }

    public decimal Rating { get; set; }
    public int ReviewsCount { get; set; }

    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
    public int? Discount { get; set; }

    public ProductVariantSummaryDto? Variant { get; set; }

    public Guid? BrandId { get; set; }
    public string? BrandName { get; set; }
    public string? SkinType { get; set; }
}
