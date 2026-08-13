using FluentValidation;
using Luxira.Application.DTOs.Product;
using Luxira.Domain.Entities;

namespace Luxira.Application.Validators.Product;

public class ReplaceCountryPricesRequestValidator : AbstractValidator<ReplaceCountryPricesRequest>
{
    public ReplaceCountryPricesRequestValidator()
    {
        RuleForEach(x => x.Prices).ChildRules(price =>
        {
            price.RuleFor(p => p.Country)
                .Must(v => Enum.TryParse<Country>(v, out _))
                .WithMessage("الدولة غير مدعومة");

            price.RuleFor(p => p.Price).GreaterThan(0);
        });

        RuleFor(x => x.Prices)
            .Must(prices => prices
                .Select(p => p.Country.Trim().ToLowerInvariant())
                .Distinct()
                .Count() == prices.Count)
            .WithMessage("لا يمكن تكرار نفس الدولة أكثر من مرة");
    }
}
