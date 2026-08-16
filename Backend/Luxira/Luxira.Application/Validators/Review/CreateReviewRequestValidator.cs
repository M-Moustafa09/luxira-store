using FluentValidation;
using Luxira.Application.DTOs.Review;

namespace Luxira.Application.Validators.Review;

public class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequest>
{
    public CreateReviewRequestValidator()
    {
        RuleFor(x => x.AuthorName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Text).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
    }
}
