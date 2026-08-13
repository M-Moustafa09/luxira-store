using Luxira.Application.Interfaces;
using Luxira.Domain.Entities;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Luxira.Infrastructure.Services;

// Backed by a self-hosted MaxMind GeoLite2 Country database (see GeoIp:DatabasePath).
// If the file isn't configured/present, falls back to always returning null (USD
// pricing for everyone) rather than throwing, since geolocation is a "nice to have
// degrade gracefully" concern, not a hard dependency for the storefront to function.
public class GeoIpLookup : IGeoIpLookup, IDisposable
{
    // MaxMind's ISO 3166-1 alpha-2 codes for the 16 supported countries.
    private static readonly Dictionary<string, Country> IsoCodeToCountry = new(StringComparer.OrdinalIgnoreCase)
    {
        ["JO"] = Country.Jordan,
        ["AE"] = Country.UAE,
        ["BH"] = Country.Bahrain,
        ["DZ"] = Country.Algeria,
        ["SA"] = Country.SaudiArabia,
        ["IQ"] = Country.Iraq,
        ["KW"] = Country.Kuwait,
        ["MA"] = Country.Morocco,
        ["TR"] = Country.Turkey,
        ["TN"] = Country.Tunisia,
        ["OM"] = Country.Oman,
        ["PS"] = Country.Palestine,
        ["QA"] = Country.Qatar,
        ["LB"] = Country.Lebanon,
        ["LY"] = Country.Libya,
        ["EG"] = Country.Egypt
    };

    private readonly ILogger<GeoIpLookup> _logger;
    private readonly DatabaseReader? _reader;

    public GeoIpLookup(IConfiguration configuration, ILogger<GeoIpLookup> logger)
    {
        _logger = logger;

        var path = configuration["GeoIp:DatabasePath"];
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            _logger.LogWarning("GeoIp:DatabasePath is not configured or the file doesn't exist; all visitors will fall back to USD pricing.");
            return;
        }

        _reader = new DatabaseReader(path);
    }

    public Country? Lookup(string ipAddress)
    {
        if (_reader is null)
        {
            return null;
        }

        try
        {
            var response = _reader.Country(ipAddress);
            var isoCode = response.Country.IsoCode;

            return isoCode is not null && IsoCodeToCountry.TryGetValue(isoCode, out var country)
                ? country
                : null;
        }
        catch (AddressNotFoundException)
        {
            // Private/reserved/unmapped IPs (e.g. localhost in dev) - expected, not an error.
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "GeoIP lookup failed for {IpAddress}", ipAddress);
            return null;
        }
    }

    public void Dispose() => _reader?.Dispose();
}
