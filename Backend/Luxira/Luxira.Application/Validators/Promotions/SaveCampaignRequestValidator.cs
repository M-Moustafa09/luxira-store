using FluentValidation;
using Luxira.Application.DTOs.Promotions;

namespace Luxira.Application.Validators.Promotions;

public class SaveCampaignRequestValidator : AbstractValidator<SaveCampaignRequest>
{
    public SaveCampaignRequestValidator()
    {
        RuleFor(x => x.EndsAt).NotEmpty();
        RuleFor(x => x.MaxDiscountPercent).InclusiveBetween(0, 100);
    }
}
