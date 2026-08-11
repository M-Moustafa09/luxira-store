using FluentValidation;
using Luxira.Application.DTOs.Cart;

namespace Luxira.Application.Validators.Cart;

public class UpdateCartItemRequestValidator : AbstractValidator<UpdateCartItemRequest>
{
    public UpdateCartItemRequestValidator()
    {
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(1);
    }
}
