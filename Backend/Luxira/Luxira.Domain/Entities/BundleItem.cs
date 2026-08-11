using Luxira.Domain.Common;

namespace Luxira.Domain.Entities;

public class BundleItem : BaseEntity
{
    public Guid BundleId { get; set; }
    public Bundle Bundle { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; } = 1;
}
