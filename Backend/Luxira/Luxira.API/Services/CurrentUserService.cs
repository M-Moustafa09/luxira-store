using Luxira.Application.Interfaces;

namespace Luxira.API.Services;

/// <summary>
/// Resolves "who is making this request" from a client-generated X-Guest-Id header.
/// This is a temporary stand-in until Auth (JWT) ships — every other layer only ever
/// talks to ICurrentUserService, so swapping this implementation to read the customer
/// id from JWT claims later requires no changes anywhere else.
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
            var header = _httpContextAccessor.HttpContext?.Request.Headers[GuestIdHeader].FirstOrDefault();

            if (!Guid.TryParse(header, out var guestId))
            {
                throw new UnauthorizedAccessException($"Missing or invalid {GuestIdHeader} header.");
            }

            return guestId;
        }
    }
}
