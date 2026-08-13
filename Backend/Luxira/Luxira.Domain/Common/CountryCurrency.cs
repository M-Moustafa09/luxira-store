using Luxira.Domain.Entities;

namespace Luxira.Domain.Common;

// Currency is intentionally derived from Country here rather than stored as a
// separate admin-editable field, to remove the risk of an admin saving a price
// tagged with the wrong currency (e.g. an EGP price mislabeled as SAR).
public static class CountryCurrency
{
    private static readonly Dictionary<Country, string> Map = new()
    {
        [Country.Jordan] = "JOD",
        [Country.UAE] = "AED",
        [Country.Bahrain] = "BHD",
        [Country.Algeria] = "DZD",
        [Country.SaudiArabia] = "SAR",
        [Country.Iraq] = "IQD",
        [Country.Kuwait] = "KWD",
        [Country.Morocco] = "MAD",
        [Country.Turkey] = "TRY",
        [Country.Tunisia] = "TND",
        [Country.Oman] = "OMR",
        // Palestine has no independent currency of its own; ILS is the currency
        // in most everyday circulation. Flagged as an assumption worth confirming.
        [Country.Palestine] = "ILS",
        [Country.Qatar] = "QAR",
        [Country.Lebanon] = "LBP",
        [Country.Libya] = "LYD",
        [Country.Egypt] = "EGP"
    };

    public const string FallbackCurrency = "USD";

    public static string For(Country country) => Map[country];
}
