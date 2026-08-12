using System.IdentityModel.Tokens.Jwt;
using Luxira.Application.Interfaces;

namespace Luxira.API.Services;

/// <summary>
/// Resolves "who is making this request" - from the JWT (sub claim) when the caller
/// is logged in, otherwise from the client-generated X-Guest-Id header. This is what
/// lets Cart/Wishlist/Checkout/etc. keep working identically for guests: those
/// endpoints carry no [Authorize], so a request with no token still resolves to a
/// guest id here exactly like before Auth existed. Every other layer only ever talks
/// to ICurrentUserService, so this is the one place that needed to change.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private const string GuestIdHeader = "X-Guest-Id";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid CustomerId
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;

            var sub = context?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (sub is not null && Guid.TryParse(sub, out var customerId))
            {
                return customerId;
            }

            var header = context?.Request.Headers[GuestIdHeader].FirstOrDefault();
            if (Guid.TryParse(header, out var guestId))
            {
                return guestId;
            }

            throw new UnauthorizedAccessException($"Missing or invalid {GuestIdHeader} header.");
        }
    }
}
