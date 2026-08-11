namespace Luxira.Application.DTOs.Product;

public class ProductDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
    public int? Discount { get; set; }

    public decimal Rating { get; set; }
    public int ReviewsCount { get; set; }

    public Guid CategoryId { get; set; }

    public Guid? BrandId { get; set; }
    public string? BrandName { get; set; }
    public string? SkinType { get; set; }

    public List<ProductVariantDto> Variants { get; set; } = new();
}
