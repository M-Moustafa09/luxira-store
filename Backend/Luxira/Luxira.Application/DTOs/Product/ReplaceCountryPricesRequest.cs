namespace Luxira.Application.DTOs.Product;

public class ReplaceCountryPricesRequest
{
    public List<SaveProductCountryPriceRequest> Prices { get; set; } = new();
}
