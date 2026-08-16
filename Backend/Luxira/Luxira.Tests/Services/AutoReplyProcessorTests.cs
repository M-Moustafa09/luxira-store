using FluentAssertions;
using Luxira.Application.Interfaces;
using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Services;
using NSubstitute;

namespace Luxira.Tests.Services;

public class AutoReplyProcessorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IAutoReplyTextProvider _textProvider = Substitute.For<IAutoReplyTextProvider>();
    private readonly AutoReplyProcessor _sut;

    public AutoReplyProcessorTests()
    {
        _sut = new AutoReplyProcessor(_unitOfWork, _textProvider);
    }

    [Fact]
    public async Task ProcessDueRepliesAsync_DoesNothing_WhenNoReviewsAreDue()
    {
        _unitOfWork.Reviews.GetDueForAutoReplyAsync(Arg.Any<DateTime>()).Returns([]);

        await _sut.ProcessDueRepliesAsync();

        await _unitOfWork.ReviewReplies.DidNotReceive().AddAsync(Arg.Any<ReviewReply>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync();
    }

    [Fact]
    public async Task ProcessDueRepliesAsync_CreatesAReply_AndMarksTheReviewSent_ForEachDueReview()
    {
        var review = new Review { Id = Guid.NewGuid(), Rating = 5, AutoReplySent = false };
        _unitOfWork.Reviews.GetDueForAutoReplyAsync(Arg.Any<DateTime>()).Returns([review]);
        _textProvider.GetReplyText(5).Returns("شكراً لتقييمك!");

        await _sut.ProcessDueRepliesAsync();

        await _unitOfWork.ReviewReplies.Received(1).AddAsync(Arg.Is<ReviewReply>(r =>
            r.ReviewId == review.Id && r.Text == "شكراً لتقييمك!" && r.IsAutomated == true));
        review.AutoReplySent.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task ProcessDueRepliesAsync_PicksTheReplyText_UsingEachReviewsOwnRating()
    {
        var positiveReview = new Review { Id = Guid.NewGuid(), Rating = 5 };
        var negativeReview = new Review { Id = Guid.NewGuid(), Rating = 1 };
        _unitOfWork.Reviews.GetDueForAutoReplyAsync(Arg.Any<DateTime>()).Returns([positiveReview, negativeReview]);
        _textProvider.GetReplyText(5).Returns("positive reply");
        _textProvider.GetReplyText(1).Returns("negative reply");

        await _sut.ProcessDueRepliesAsync();

        await _unitOfWork.ReviewReplies.Received(1).AddAsync(
            Arg.Is<ReviewReply>(r => r.ReviewId == positiveReview.Id && r.Text == "positive reply"));
        await _unitOfWork.ReviewReplies.Received(1).AddAsync(
            Arg.Is<ReviewReply>(r => r.ReviewId == negativeReview.Id && r.Text == "negative reply"));
    }
}
