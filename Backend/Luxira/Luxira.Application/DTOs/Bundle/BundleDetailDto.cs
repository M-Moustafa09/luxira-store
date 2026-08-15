namespace Luxira.Application.DTOs.Bundle;

// Admin-only detail shape (list/storefront still use BundleDto) - carries the
// actual product line items instead of just the aggregate ProductsCount, same
// split as ProductListItemDto vs ProductDetailDto.
public class BundleDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
    public int? Discount { get; set; }

    public string? Badge { get; set; }
    public string? BackgroundColor { get; set; }
    public int SortOrder { get; set; }

    public List<BundleItemDto> Items { get; set; } = new();
}
