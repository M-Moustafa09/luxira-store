namespace Luxira.Application.DTOs.Coupon;

public class CouponDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
