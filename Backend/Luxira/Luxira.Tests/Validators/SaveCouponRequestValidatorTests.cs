using FluentValidation.TestHelper;
using Luxira.Application.DTOs.Coupon;
using Luxira.Application.Validators.Coupon;

namespace Luxira.Tests.Validators;

public class SaveCouponRequestValidatorTests
{
    private readonly SaveCouponRequestValidator _validator = new();

    private static SaveCouponRequest ValidRequest() => new()
    {
        Code = "SAVE10",
        DiscountType = "Percentage",
        DiscountValue = 10,
        IsActive = true
    };

    [Fact]
    public void Passes_ForAValidPercentageCoupon()
    {
        var result = _validator.TestValidate(ValidRequest());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_WhenPercentageDiscountExceeds100()
    {
        var request = ValidRequest();
        request.DiscountValue = 150;

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.DiscountValue);
    }

    [Fact]
    public void Allows_FixedAmountDiscountAbove100()
    {
        // The 100% cap only makes sense for Percentage discounts - a fixed
        // amount coupon worth more than 100 (currency units) is legitimate.
        var request = ValidRequest();
        request.DiscountType = "FixedAmount";
        request.DiscountValue = 150;

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Fails_WhenDiscountTypeIsNotARealEnumValue()
    {
        var request = ValidRequest();
        request.DiscountType = "NotARealType";

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.DiscountType);
    }

    [Fact]
    public void Fails_WhenDiscountValueIsZeroOrNegative()
    {
        var request = ValidRequest();
        request.DiscountValue = 0;

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.DiscountValue);
    }
}
