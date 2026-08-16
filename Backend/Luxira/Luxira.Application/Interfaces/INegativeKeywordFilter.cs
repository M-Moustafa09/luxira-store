namespace Luxira.Application.Interfaces;

// Checks review text against the admin-configured negative-keyword list
// (Moderation:NegativeKeywords in appsettings.json). Deterministic
// substring matching after Arabic normalization - no ML/sentiment model,
// per the project's no-over-engineering rule.
public interface INegativeKeywordFilter
{
    bool IsNegative(string text);
}
