using Luxira.Domain.Common;

namespace Luxira.Domain.Entities;

public class BundleCartItem : BaseEntity
{
    public Guid CartId { get; set; }
    public Cart Cart { get; set; } = null!;

    public Guid BundleId { get; set; }
    public Bundle Bundle { get; set; } = null!;

    // Snapshotted at add-time so the cart isn't affected if the bundle's
    // authored price changes later (same philosophy as OrderItem.UnitPrice).
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; } = 1;
}
