using Luxira.Application.DTOs.Coupon;

namespace Luxira.Application.Interfaces;

public interface ICouponService
{
    Task<List<CouponDto>> GetAllAsync();
    Task<CouponDto?> GetByIdAsync(Guid id);
    Task<CouponDto> CreateAsync(SaveCouponRequest request);
    Task<CouponDto> UpdateAsync(Guid id, SaveCouponRequest request);
    Task DeleteAsync(Guid id);
}
