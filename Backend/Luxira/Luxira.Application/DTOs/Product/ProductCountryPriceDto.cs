namespace Luxira.Application.DTOs.Product;

public class ProductCountryPriceDto
{
    public string Country { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
