using FluentValidation;
using Luxira.Application.DTOs.Order;
using Luxira.Domain.Entities;

namespace Luxira.Application.Validators.Order;

public class UpdateOrderStatusRequestValidator : AbstractValidator<UpdateOrderStatusRequest>
{
    public UpdateOrderStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(v => Enum.TryParse<OrderStatus>(v, out _))
            .WithMessage("حالة الطلب غير صالحة");
    }
}
