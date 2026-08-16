using FluentAssertions;
using Luxira.Application.DTOs.Review;
using Luxira.Application.Interfaces;
using Luxira.Application.Validators.Review;
using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Services;
using NSubstitute;

namespace Luxira.Tests.Services;

public class ReviewServiceTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly INegativeKeywordFilter _negativeKeywordFilter = Substitute.For<INegativeKeywordFilter>();
    private readonly ReviewService _sut;

    public ReviewServiceTests()
    {
        _sut = new ReviewService(_unitOfWork, _currentUser, new CreateReviewRequestValidator(), _negativeKeywordFilter);
    }

    private static CreateReviewRequest ValidRequest(int rating = 5) => new()
    {
        AuthorName = "زائرة",
        Rating = rating,
        Text = "منتج رائع"
    };

    [Fact]
    public async Task CreateAsync_Throws_WhenProductDoesNotExist()
    {
        var productId = Guid.NewGuid();
        _unitOfWork.Products.GetByIdAsync(productId).Returns((Product?)null);

        var act = () => _sut.CreateAsync(productId, ValidRequest());

        await act.Should().ThrowAsync<KeyNotFoundException>();
        await _unitOfWork.Reviews.DidNotReceive().AddAsync(Arg.Any<Review>());
    }

    [Fact]
    public async Task CreateAsync_TiesTheReviewToTheCurrentCustomer_AndTrimsInput()
    {
        var product = new Product();
        var customerId = Guid.NewGuid();
        _unitOfWork.Products.GetByIdAsync(product.Id).Returns(product);
        _currentUser.CustomerId.Returns(customerId);
        _unitOfWork.Reviews.GetVisibleStatsAsync(product.Id).Returns((1, 5m));

        var request = new CreateReviewRequest { AuthorName = "  زائرة  ", Rating = 5, Text = "  رائع  " };

        var result = await _sut.CreateAsync(product.Id, request);

        result.AuthorName.Should().Be("زائرة");
        result.Text.Should().Be("رائع");
        await _unitOfWork.Reviews.Received(1).AddAsync(
            Arg.Is<Review>(r => r.CustomerId == customerId && r.ProductId == product.Id));
    }

    [Fact]
    public async Task CreateAsync_AutoHidesAndFlagsTheReview_WhenTextMatchesANegativeKeyword()
    {
        var product = new Product();
        _unitOfWork.Products.GetByIdAsync(product.Id).Returns(product);
        _unitOfWork.Reviews.GetVisibleStatsAsync(product.Id).Returns((0, 0m));
        _negativeKeywordFilter.IsNegative(Arg.Any<string>()).Returns(true);

        var result = await _sut.CreateAsync(product.Id, ValidRequest());

        result.IsVisible.Should().BeFalse();
        result.IsFlaggedNegative.Should().BeTrue();
        await _unitOfWork.Reviews.Received(1).AddAsync(
            Arg.Is<Review>(r => r.IsVisible == false && r.IsFlaggedNegative == true));
    }

    [Fact]
    public async Task CreateAsync_KeepsTheReviewVisible_WhenTextDoesNotMatchAnyNegativeKeyword()
    {
        var product = new Product();
        _unitOfWork.Products.GetByIdAsync(product.Id).Returns(product);
        _unitOfWork.Reviews.GetVisibleStatsAsync(product.Id).Returns((1, 5m));
        _negativeKeywordFilter.IsNegative(Arg.Any<string>()).Returns(false);

        var result = await _sut.CreateAsync(product.Id, ValidRequest());

        result.IsVisible.Should().BeTrue();
        result.IsFlaggedNegative.Should().BeFalse();
        await _unitOfWork.Reviews.Received(1).AddAsync(
            Arg.Is<Review>(r => r.IsVisible == true && r.IsFlaggedNegative == false));
    }

    [Fact]
    public async Task CreateAsync_RecomputesProductRatingAndReviewsCount()
    {
        var product = new Product { Rating = 0, ReviewsCount = 0 };
        _unitOfWork.Products.GetByIdAsync(product.Id).Returns(product);
        // Stats queried AFTER the new review is saved - simulate 3 visible
        // reviews averaging 4.33 now that this one has been added.
        _unitOfWork.Reviews.GetVisibleStatsAsync(product.Id).Returns((3, 4.333m));

        await _sut.CreateAsync(product.Id, ValidRequest());

        product.ReviewsCount.Should().Be(3);
        product.Rating.Should().Be(4.33m);
        await _unitOfWork.Received(2).SaveChangesAsync(); // once for the review, once for the aggregate
    }

    [Fact]
    public async Task CreateAsync_ResetsRatingToZero_WhenNoVisibleReviewsRemain()
    {
        // Edge case relevant once hide/delete exist too: if the stats query
        // comes back empty, the product must not keep a stale non-zero rating.
        var product = new Product { Rating = 4.5m, ReviewsCount = 2 };
        _unitOfWork.Products.GetByIdAsync(product.Id).Returns(product);
        _unitOfWork.Reviews.GetVisibleStatsAsync(product.Id).Returns((0, 0m));

        await _sut.CreateAsync(product.Id, ValidRequest());

        product.Rating.Should().Be(0);
        product.ReviewsCount.Should().Be(0);
    }

    [Fact]
    public async Task SetVisibilityAsync_HidesTheReview_AndRecomputesTheProductAggregate()
    {
        var product = new Product { Id = Guid.NewGuid(), Rating = 5m, ReviewsCount = 1 };
        var review = new Review { Id = Guid.NewGuid(), ProductId = product.Id, IsVisible = true };
        _unitOfWork.Reviews.GetByIdAsync(review.Id).Returns(review);
        _unitOfWork.Products.GetByIdAsync(product.Id).Returns(product);
        // Hiding the only review leaves zero visible.
        _unitOfWork.Reviews.GetVisibleStatsAsync(product.Id).Returns((0, 0m));

        var result = await _sut.SetVisibilityAsync(review.Id, false);

        result.IsVisible.Should().BeFalse();
        review.IsVisible.Should().BeFalse();
        product.Rating.Should().Be(0);
        product.ReviewsCount.Should().Be(0);
    }

    [Fact]
    public async Task SetVisibilityAsync_Throws_WhenReviewDoesNotExist()
    {
        _unitOfWork.Reviews.GetByIdAsync(Arg.Any<Guid>()).Returns((Review?)null);

        var act = () => _sut.SetVisibilityAsync(Guid.NewGuid(), true);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_RemovesTheReview_AndRecomputesTheProductAggregate()
    {
        var product = new Product { Id = Guid.NewGuid(), Rating = 4m, ReviewsCount = 2 };
        var review = new Review { Id = Guid.NewGuid(), ProductId = product.Id };
        _unitOfWork.Reviews.GetByIdAsync(review.Id).Returns(review);
        _unitOfWork.Products.GetByIdAsync(product.Id).Returns(product);
        // One review remains after the delete, rated 3.
        _unitOfWork.Reviews.GetVisibleStatsAsync(product.Id).Returns((1, 3m));

        await _sut.DeleteAsync(review.Id);

        _unitOfWork.Reviews.Received(1).Remove(review);
        product.Rating.Should().Be(3);
        product.ReviewsCount.Should().Be(1);
    }

    [Fact]
    public async Task DeleteAsync_Throws_WhenReviewDoesNotExist()
    {
        _unitOfWork.Reviews.GetByIdAsync(Arg.Any<Guid>()).Returns((Review?)null);

        var act = () => _sut.DeleteAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
        _unitOfWork.Reviews.DidNotReceive().Remove(Arg.Any<Review>());
    }
}
