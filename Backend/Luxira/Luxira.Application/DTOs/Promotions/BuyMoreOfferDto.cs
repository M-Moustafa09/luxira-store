namespace Luxira.Application.DTOs.Promotions;

public class BuyMoreOfferDto
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public int DiscountPercent { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}
