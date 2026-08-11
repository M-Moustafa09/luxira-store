using FluentValidation;
using Luxira.Application.DTOs.Cart;

namespace Luxira.Application.Validators.Cart;

public class ApplyCouponRequestValidator : AbstractValidator<ApplyCouponRequest>
{
    public ApplyCouponRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
    }
}
