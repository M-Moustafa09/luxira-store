using FluentValidation;
using Luxira.Application.DTOs.Bundle;

namespace Luxira.Application.Validators.Bundle;

public class SaveBundleRequestValidator : AbstractValidator<SaveBundleRequest>
{
    public SaveBundleRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.MainImageUrl).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.OldPrice).GreaterThan(0).When(x => x.OldPrice.HasValue);
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);

        RuleFor(x => x.Items).NotEmpty().WithMessage("لازم تحدد منتج واحد على الأقل في الباقة");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty();
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });
    }
}
