using Luxira.Domain.Entities;

namespace Luxira.Application.Interfaces;

// Resolves an IP address to one of the 16 supported countries, or null if the IP
// is outside that list / can't be resolved (private IP, lookup failure, etc.) - both
// cases are treated identically by callers (USD fallback), so this stays a simple
// nullable return rather than a richer "ambiguous vs failed" result type.
public interface IGeoIpLookup
{
    Country? Lookup(string ipAddress);
}
