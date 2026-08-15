using FluentValidation;
using Luxira.Application.DTOs.Coupon;
using Luxira.Domain.Entities;

namespace Luxira.Application.Validators.Coupon;

public class SaveCouponRequestValidator : AbstractValidator<SaveCouponRequest>
{
    public SaveCouponRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);

        RuleFor(x => x.DiscountType)
            .Must(v => Enum.TryParse<CouponDiscountType>(v, out _))
            .WithMessage("نوع الخصم غير صالح");

        RuleFor(x => x.DiscountValue).GreaterThan(0);

        RuleFor(x => x.DiscountValue)
            .LessThanOrEqualTo(100)
            .WithMessage("نسبة الخصم لا يمكن أن تتجاوز 100%")
            .When(x => Enum.TryParse<CouponDiscountType>(x.DiscountType, out var type) && type == CouponDiscountType.Percentage);
    }
}
