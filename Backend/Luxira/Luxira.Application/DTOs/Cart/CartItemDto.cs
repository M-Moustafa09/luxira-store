namespace Luxira.Application.DTOs.Cart;

public class CartItemDto
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSubtitle { get; set; } = string.Empty;
    public string ProductImageUrl { get; set; } = string.Empty;

    public Guid VariantId { get; set; }
    public string VariantLabel { get; set; } = string.Empty;
    public string VariantColorHex { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}
