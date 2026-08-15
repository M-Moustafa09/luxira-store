using FluentValidation;
using Luxira.Application.DTOs.Order;

namespace Luxira.Application.Validators.Order;

public class SetCustomerBlockedRequestValidator : AbstractValidator<SetCustomerBlockedRequest>
{
    public SetCustomerBlockedRequestValidator()
    {
        RuleFor(x => x.Reason).MaximumLength(500);
    }
}
