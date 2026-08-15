using FluentValidation;
using Luxira.Application.DTOs.Coupon;
using Luxira.Application.Interfaces;
using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Mapster;

namespace Luxira.Infrastructure.Services;

public class CouponService : ICouponService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<SaveCouponRequest> _saveCouponValidator;

    public CouponService(IUnitOfWork unitOfWork, IValidator<SaveCouponRequest> saveCouponValidator)
    {
        _unitOfWork = unitOfWork;
        _saveCouponValidator = saveCouponValidator;
    }

    public async Task<List<CouponDto>> GetAllAsync()
    {
        var coupons = await _unitOfWork.Coupons.GetAllAsync();
        return coupons.OrderByDescending(c => c.CreatedAt).Adapt<List<CouponDto>>();
    }

    public async Task<CouponDto?> GetByIdAsync(Guid id)
    {
        var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);
        return coupon?.Adapt<CouponDto>();
    }

    public async Task<CouponDto> CreateAsync(SaveCouponRequest request)
    {
        await _saveCouponValidator.ValidateAndThrowAsync(request);

        var code = request.Code.Trim().ToUpperInvariant();
        await EnsureCodeIsUniqueAsync(code, excludeId: null);

        var coupon = new Domain.Entities.Coupon
        {
            Code = code,
            DiscountType = Enum.Parse<CouponDiscountType>(request.DiscountType, ignoreCase: true),
            DiscountValue = request.DiscountValue,
            IsActive = request.IsActive,
            ExpiresAt = request.ExpiresAt
        };

        await _unitOfWork.Coupons.AddAsync(coupon);
        await _unitOfWork.SaveChangesAsync();

        return coupon.Adapt<CouponDto>();
    }

    public async Task<CouponDto> UpdateAsync(Guid id, SaveCouponRequest request)
    {
        await _saveCouponValidator.ValidateAndThrowAsync(request);

        var coupon = await _unitOfWork.Coupons.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("الكوبون غير موجود");

        var code = request.Code.Trim().ToUpperInvariant();
        await EnsureCodeIsUniqueAsync(code, excludeId: id);

        coupon.Code = code;
        coupon.DiscountType = Enum.Parse<CouponDiscountType>(request.DiscountType, ignoreCase: true);
        coupon.DiscountValue = request.DiscountValue;
        coupon.IsActive = request.IsActive;
        coupon.ExpiresAt = request.ExpiresAt;

        await _unitOfWork.SaveChangesAsync();

        return coupon.Adapt<CouponDto>();
    }

    public async Task DeleteAsync(Guid id)
    {
        var coupon = await _unitOfWork.Coupons.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("الكوبون غير موجود");

        // Carts only ever store the coupon's Code (string), never a foreign key to
        // this row, so unlike Product/Category/Brand/Bundle deletion there's no FK
        // to guard against here - a deleted coupon just stops being redeemable.
        _unitOfWork.Coupons.Remove(coupon);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task EnsureCodeIsUniqueAsync(string code, Guid? excludeId)
    {
        var existing = await _unitOfWork.Coupons.FindByCodeAsync(code);
        if (existing is not null && existing.Id != excludeId)
        {
            throw new ValidationException(
            [
                new FluentValidation.Results.ValidationFailure(nameof(Domain.Entities.Coupon.Code), "يوجد كوبون آخر بنفس الكود بالفعل")
            ]);
        }
    }
}
