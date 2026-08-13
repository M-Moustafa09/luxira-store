using Luxira.Application.Interfaces;
using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;

namespace Luxira.API.Services;

/// <summary>
/// Resolves which of the 16 supported countries the current visitor is in, once per
/// customer, and pins the result on Customer (see Customer.Country / CountryResolvedAt)
/// so pricing/currency stay stable for the rest of that customer's session even if
/// their network changes mid-cart. In Development, a "country" query string value
/// (e.g. ?country=EG) or "X-Dev-Country" header overrides IP geolocation, since local
/// dev traffic always resolves to a private IP otherwise.
/// </summary>
public class CountryResolverService : ICountryResolver
{
    private const string DevCountryQueryParam = "country";
    private const string DevCountryHeader = "X-Dev-Country";

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHostEnvironment _environment;
    private readonly IGeoIpLookup _geoIpLookup;

    public CountryResolverService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment environment,
        IGeoIpLookup geoIpLookup)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _httpContextAccessor = httpContextAccessor;
        _environment = environment;
        _geoIpLookup = geoIpLookup;
    }

    public async Task<Country?> ResolveAsync()
    {
        var customer = await _unitOfWork.Customers.GetOrCreateGuestAsync(_currentUser.CustomerId);

        if (customer.CountryResolvedAt is not null)
        {
            return customer.Country;
        }

        var resolved = TryResolveDevOverride() ?? ResolveFromIp();

        customer.Country = resolved;
        customer.CountryResolvedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        return resolved;
    }

    private Country? TryResolveDevOverride()
    {
        if (!_environment.IsDevelopment())
        {
            return null;
        }

        var context = _httpContextAccessor.HttpContext;
        var value = context?.Request.Query[DevCountryQueryParam].FirstOrDefault()
            ?? context?.Request.Headers[DevCountryHeader].FirstOrDefault();

        return Enum.TryParse<Country>(value, ignoreCase: true, out var country) ? country : null;
    }

    private Country? ResolveFromIp()
    {
        var ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        return string.IsNullOrWhiteSpace(ip) ? null : _geoIpLookup.Lookup(ip);
    }
}
