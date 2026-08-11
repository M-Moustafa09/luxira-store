namespace Luxira.Application.DTOs.Bundle;

public class BundleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
    public int? Discount { get; set; }

    public int ProductsCount { get; set; }
    public string? Badge { get; set; }
    public string? BackgroundColor { get; set; }
}
