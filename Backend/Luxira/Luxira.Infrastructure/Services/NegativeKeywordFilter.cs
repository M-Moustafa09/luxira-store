using System.Text;
using Luxira.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Luxira.Infrastructure.Services;

// Reads Moderation:NegativeKeywords from configuration (non-secret, same
// pattern as Email:*) and does a normalized substring match - not
// word-boundary matching, since Arabic attaches prefixes/suffixes (ال,
// pronouns) directly to words, which would make exact word-boundary
// matching unreliable. Normalization strips diacritics/tatweel and folds
// letter variants (أإآ -> ا, ى -> ي, ة -> ه)
// so a keyword matches regardless of which form the customer typed.
// Known limitation: hamza-on-yeh/waw (ئ/ؤ) are NOT folded to a bare hamza -
// e.g. "سيئ" (bad) would not match a keyword spelled "سيء". Folding those
// too would catch more variants but risks more false positives; left out
// deliberately, matching the "keyword list needs occasional upkeep"
// trade-off already agreed for this feature.
public class NegativeKeywordFilter : INegativeKeywordFilter
{
    // Arabic diacritics (tashkeel) - fathatan..sukun, plus superscript alef.
    private const char DiacriticRangeStart = 'ً';
    private const char DiacriticRangeEnd = 'ْ';
    private const char SuperscriptAlef = 'ٰ';
    private const char Tatweel = 'ـ';

    private readonly IConfiguration _configuration;

    public NegativeKeywordFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool IsNegative(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var keywords = _configuration.GetSection("Moderation:NegativeKeywords")
            .GetChildren()
            .Select(c => c.Value)
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .ToArray();

        if (keywords.Length == 0)
        {
            return false;
        }

        var normalizedText = Normalize(text);

        return keywords
            .Select(k => Normalize(k!))
            .Where(k => k.Length > 0)
            .Any(normalizedText.Contains);
    }

    private static string Normalize(string text)
    {
        var builder = new StringBuilder(text.Length);

        foreach (var ch in text)
        {
            if (IsDiacritic(ch) || ch == Tatweel)
            {
                continue;
            }

            builder.Append(FoldLetterVariant(ch));
        }

        return builder.ToString().Trim().ToLowerInvariant();
    }

    private static bool IsDiacritic(char ch) =>
        (ch >= DiacriticRangeStart && ch <= DiacriticRangeEnd) || ch == SuperscriptAlef;

    private static char FoldLetterVariant(char ch) => ch switch
    {
        'أ' or 'إ' or 'آ' => 'ا', // أ/إ/آ -> ا (alef variants)
        'ى' => 'ي',                          // ى -> ي (alef maksura -> yeh)
        'ة' => 'ه',                          // ة -> ه (teh marbuta -> heh)
        _ => ch
    };
}
