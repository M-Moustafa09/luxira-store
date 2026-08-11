namespace Luxira.Application.DTOs.Cart;

public class AddCartItemRequest
{
    public Guid ProductId { get; set; }
    public Guid? ProductVariantId { get; set; }
    public int Quantity { get; set; } = 1;
}
