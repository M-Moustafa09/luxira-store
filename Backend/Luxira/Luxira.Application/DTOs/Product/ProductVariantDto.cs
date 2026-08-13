namespace Luxira.Application.DTOs.Product;

public class ProductVariantDto
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string ColorHex { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int Stock { get; set; }
}
