using Luxira.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Luxira.Infrastructure.Services;

// Reads AutoReply:Positive/Negative/Neutral from configuration (same
// non-secret pattern as Moderation:NegativeKeywords) - Rating>=4 draws from
// Positive, <=2 from Negative, ==3 from Neutral. A different random entry is
// returned each call, not a fixed per-tier message.
public class AutoReplyTextProvider : IAutoReplyTextProvider
{
    private const string FallbackText = "شكراً لتقييمك، نقدر مشاركتك رأيك معنا.";

    private readonly IConfiguration _configuration;

    public AutoReplyTextProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetReplyText(int rating)
    {
        var tier = rating switch
        {
            >= 4 => "Positive",
            <= 2 => "Negative",
            _ => "Neutral"
        };

        var pool = _configuration.GetSection($"AutoReply:{tier}")
            .GetChildren()
            .Select(c => c.Value)
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .ToArray();

        if (pool.Length == 0)
        {
            return FallbackText;
        }

        return pool[Random.Shared.Next(pool.Length)]!;
    }
}
