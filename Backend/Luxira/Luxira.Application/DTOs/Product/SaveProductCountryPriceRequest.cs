namespace Luxira.Application.DTOs.Product;

public class SaveProductCountryPriceRequest
{
    public string Country { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
