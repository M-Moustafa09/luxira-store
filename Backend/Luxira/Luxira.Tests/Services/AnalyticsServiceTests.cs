using FluentAssertions;
using Luxira.Application.Interfaces;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Services;
using NSubstitute;

namespace Luxira.Tests.Services;

public class AnalyticsServiceTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly AnalyticsService _sut;

    public AnalyticsServiceTests()
    {
        _sut = new AnalyticsService(_unitOfWork, _currentUser);
    }

    [Fact]
    public async Task RecordVisitAsync_AddsAVisit_ForTheCurrentCustomer()
    {
        var customerId = Guid.NewGuid();
        _currentUser.CustomerId.Returns(customerId);

        await _sut.RecordVisitAsync();

        await _unitOfWork.SiteVisits.Received(1).AddAsync(
            Arg.Is<Domain.Entities.SiteVisit>(v => v.CustomerId == customerId));
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task GetStatsAsync_UsesCalendarBoundaries_ForTodayWeekAndMonth()
    {
        var capturedDates = new List<DateTime>();
        _unitOfWork.SiteVisits.GetCountSinceAsync(Arg.Do<DateTime>(d => capturedDates.Add(d))).Returns(0);
        _unitOfWork.SiteVisits.GetTotalCountAsync().Returns(10);
        _unitOfWork.SiteVisits.GetUniqueVisitorCountAsync().Returns(4);

        var stats = await _sut.GetStatsAsync();

        stats.TotalVisits.Should().Be(10);
        stats.TotalUniqueVisitors.Should().Be(4);
        capturedDates.Should().HaveCount(3);

        var today = DateTime.UtcNow.Date;

        // VisitsToday - exactly today's UTC midnight.
        capturedDates[0].Should().Be(today);

        // VisitsThisWeek - the most recent Monday, not a rolling 7-day window.
        capturedDates[1].DayOfWeek.Should().Be(DayOfWeek.Monday);
        capturedDates[1].Should().BeOnOrBefore(today);

        // VisitsThisMonth - the 1st of the current calendar month.
        capturedDates[2].Day.Should().Be(1);
        capturedDates[2].Month.Should().Be(today.Month);
        capturedDates[2].Year.Should().Be(today.Year);
    }

    [Fact]
    public async Task GetReviewStatsAsync_MapsTheRepositoryTuple_UsingTodaysCalendarBoundary()
    {
        DateTime? capturedSince = null;
        _unitOfWork.Reviews.GetDailyClassificationStatsAsync(Arg.Do<DateTime>(d => capturedSince = d))
            .Returns((2, 5, 3));

        var stats = await _sut.GetReviewStatsAsync();

        stats.NegativeBlockedToday.Should().Be(2);
        stats.PositiveToday.Should().Be(5);
        stats.NegativeToday.Should().Be(3);
        capturedSince.Should().Be(DateTime.UtcNow.Date);
    }
}
