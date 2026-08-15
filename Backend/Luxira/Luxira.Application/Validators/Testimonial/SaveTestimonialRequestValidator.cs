using FluentValidation;
using Luxira.Application.DTOs.Testimonial;

namespace Luxira.Application.Validators.Testimonial;

public class SaveTestimonialRequestValidator : AbstractValidator<SaveTestimonialRequest>
{
    public SaveTestimonialRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.AvatarUrl).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Text).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
    }
}
