using FluentValidation;
using Luxira.Application.DTOs.Category;

namespace Luxira.Application.Validators.Category;

public class SaveCategoryRequestValidator : AbstractValidator<SaveCategoryRequest>
{
    public SaveCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);

        RuleForEach(x => x.SubCategories).ChildRules(subCategory =>
        {
            subCategory.RuleFor(s => s.Name).NotEmpty().MaximumLength(100);
        });
    }
}
