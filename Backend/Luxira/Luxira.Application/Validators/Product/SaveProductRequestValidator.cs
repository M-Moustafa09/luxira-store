using FluentValidation;
using Luxira.Application.DTOs.Product;
using Luxira.Domain.Entities;

namespace Luxira.Application.Validators.Product;

public class SaveProductRequestValidator : AbstractValidator<SaveProductRequest>
{
    public SaveProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Subtitle).MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.MainImageUrl).NotEmpty().MaximumLength(500);

        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.OldPrice).GreaterThan(0).When(x => x.OldPrice.HasValue);

        RuleFor(x => x.CategoryId).NotEmpty();

        RuleFor(x => x.SkinType)
            .Must(v => Enum.TryParse<SkinType>(v, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.SkinType))
            .WithMessage("نوع البشرة غير صالح");

        RuleForEach(x => x.Variants).ChildRules(variant =>
        {
            variant.RuleFor(v => v.Label).NotEmpty().MaximumLength(100);
            variant.RuleFor(v => v.ColorHex).NotEmpty().MaximumLength(20);
            variant.RuleFor(v => v.ImageUrl).NotEmpty().MaximumLength(500);
        });
    }
}
