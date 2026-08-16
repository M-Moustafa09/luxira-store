using FluentAssertions;
using Luxira.Infrastructure.Services;
using Microsoft.Extensions.Configuration;

namespace Luxira.Tests.Services;

public class AutoReplyTextProviderTests
{
    private static AutoReplyTextProvider BuildProvider(
        string[]? positive = null, string[]? negative = null, string[]? neutral = null)
    {
        var pairs = new List<KeyValuePair<string, string?>>();
        AddPool(pairs, "AutoReply:Positive", positive);
        AddPool(pairs, "AutoReply:Negative", negative);
        AddPool(pairs, "AutoReply:Neutral", neutral);

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(pairs)
            .Build();

        return new AutoReplyTextProvider(configuration);
    }

    private static void AddPool(List<KeyValuePair<string, string?>> pairs, string sectionKey, string[]? values)
    {
        if (values is null)
        {
            return;
        }

        for (var i = 0; i < values.Length; i++)
        {
            pairs.Add(new KeyValuePair<string, string?>($"{sectionKey}:{i}", values[i]));
        }
    }

    [Theory]
    [InlineData(4)]
    [InlineData(5)]
    public void GetReplyText_DrawsFromThePositivePool_WhenRatingIsFourOrAbove(int rating)
    {
        var provider = BuildProvider(positive: ["positive one", "positive two"]);

        var result = provider.GetReplyText(rating);

        result.Should().BeOneOf("positive one", "positive two");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void GetReplyText_DrawsFromTheNegativePool_WhenRatingIsTwoOrBelow(int rating)
    {
        var provider = BuildProvider(negative: ["negative one", "negative two"]);

        var result = provider.GetReplyText(rating);

        result.Should().BeOneOf("negative one", "negative two");
    }

    [Fact]
    public void GetReplyText_DrawsFromTheNeutralPool_WhenRatingIsThree()
    {
        var provider = BuildProvider(neutral: ["neutral one"]);

        var result = provider.GetReplyText(3);

        result.Should().Be("neutral one");
    }

    [Fact]
    public void GetReplyText_ReturnsAFallback_WhenTheMatchingPoolIsEmpty()
    {
        var provider = BuildProvider();

        var result = provider.GetReplyText(5);

        result.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GetReplyText_VariesAcrossCalls_AcrossAPoolOfSeveralEntries()
    {
        var provider = BuildProvider(positive: ["a", "b", "c", "d", "e", "f", "g", "h"]);

        var seen = Enumerable.Range(0, 50)
            .Select(_ => provider.GetReplyText(5))
            .Distinct()
            .ToList();

        // With 8 options and 50 draws, seeing only ever the same single
        // value would mean the "random" pick isn't actually random.
        seen.Should().HaveCountGreaterThan(1);
    }
}
