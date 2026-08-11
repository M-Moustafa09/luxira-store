namespace Luxira.Application.DTOs.Cart;

public class BundleCartItemDto
{
    public Guid Id { get; set; }

    public Guid BundleId { get; set; }
    public string BundleName { get; set; } = string.Empty;
    public string BundleImageUrl { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}
