using FluentValidation;
using Luxira.Application.DTOs.Brand;

namespace Luxira.Application.Validators.Brand;

public class SaveBrandRequestValidator : AbstractValidator<SaveBrandRequest>
{
    public SaveBrandRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.LogoUrl).MaximumLength(500);
    }
}
