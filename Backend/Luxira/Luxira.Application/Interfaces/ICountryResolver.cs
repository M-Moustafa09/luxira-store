using Luxira.Domain.Entities;

namespace Luxira.Application.Interfaces;

// Resolves and pins (once per customer) which of the 16 supported countries the
// current visitor is in, or null if they're outside that list - see Customer.Country
// / Customer.CountryResolvedAt for why this is pinned rather than resolved live.
public interface ICountryResolver
{
    Task<Country?> ResolveAsync();
}
