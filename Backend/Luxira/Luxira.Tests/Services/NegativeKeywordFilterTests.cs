using FluentAssertions;
using Luxira.Infrastructure.Services;
using Microsoft.Extensions.Configuration;

namespace Luxira.Tests.Services;

public class NegativeKeywordFilterTests
{
    private static NegativeKeywordFilter BuildFilter(params string[] keywords)
    {
        var pairs = keywords
            .Select((k, i) => new KeyValuePair<string, string?>($"Moderation:NegativeKeywords:{i}", k));

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(pairs)
            .Build();

        return new NegativeKeywordFilter(configuration);
    }

    [Fact]
    public void IsNegative_ReturnsFalse_WhenNoKeywordsConfigured()
    {
        var filter = BuildFilter();

        filter.IsNegative("أي نص هنا").Should().BeFalse();
    }

    [Fact]
    public void IsNegative_MatchesConfiguredKeyword_Directly()
    {
        var filter = BuildFilter("سيء");

        filter.IsNegative("المنتج ده سيء جداً").Should().BeTrue();
    }

    [Fact]
    public void IsNegative_ReturnsFalse_WhenTextDoesNotContainAnyKeyword()
    {
        var filter = BuildFilter("سيء", "فظيع");

        filter.IsNegative("منتج رائع جداً والجودة ممتازة").Should().BeFalse();
    }

    [Fact]
    public void IsNegative_MatchesAcrossAlefVariants_AfterNormalization()
    {
        // Keyword configured with a plain alef but the customer typed a
        // hamza-on-alef variant - normalization should still match.
        var filter = BuildFilter("اسوأ");

        filter.IsNegative("هذا أسوأ منتج جربته").Should().BeTrue();
    }

    [Fact]
    public void IsNegative_IgnoresDiacritics_WhenMatching()
    {
        var filter = BuildFilter("سيئ");

        filter.IsNegative("مُنتَجٌ سَيِّئٌ للغاية").Should().BeTrue();
    }

    [Fact]
    public void IsNegative_IsCaseInsensitive_ForLatinText()
    {
        var filter = BuildFilter("BAD");

        filter.IsNegative("this product is bad").Should().BeTrue();
    }
}
