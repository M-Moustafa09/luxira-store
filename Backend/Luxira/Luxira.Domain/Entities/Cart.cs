using Luxira.Domain.Common;

namespace Luxira.Domain.Entities;

public class Cart : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public string? CouponCode { get; set; }

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
