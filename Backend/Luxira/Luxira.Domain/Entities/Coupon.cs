using Luxira.Domain.Common;

namespace Luxira.Domain.Entities;

public enum CouponDiscountType
{
    Percentage,
    FixedAmount
}

public class Coupon : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public CouponDiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime? ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
