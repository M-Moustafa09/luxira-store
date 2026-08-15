namespace Luxira.Application.DTOs.Coupon;

public class SaveCouponRequest
{
    public string Code { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? ExpiresAt { get; set; }
}
