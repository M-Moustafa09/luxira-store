using Luxira.Application.Interfaces;
using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;

namespace Luxira.Infrastructure.Services;

public class AutoReplyProcessor : IAutoReplyProcessor
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAutoReplyTextProvider _textProvider;

    public AutoReplyProcessor(IUnitOfWork unitOfWork, IAutoReplyTextProvider textProvider)
    {
        _unitOfWork = unitOfWork;
        _textProvider = textProvider;
    }

    public async Task ProcessDueRepliesAsync()
    {
        var dueReviews = await _unitOfWork.Reviews.GetDueForAutoReplyAsync(DateTime.UtcNow);
        if (dueReviews.Count == 0)
        {
            return;
        }

        foreach (var review in dueReviews)
        {
            var reply = new ReviewReply
            {
                ReviewId = review.Id,
                Text = _textProvider.GetReplyText(review.Rating),
                IsAutomated = true
            };

            // Added via the child's own repository/DbSet, not
            // review.Replies.Add(...) - review is tracked (fetched without
            // AsNoTracking so AutoReplySent can be flipped below), and EF
            // misjudges Added vs Modified for a child added onto an
            // already-tracked parent's collection (documented project-wide
            // gotcha, decision #10 in CLAUDE.md).
            await _unitOfWork.ReviewReplies.AddAsync(reply);
            review.AutoReplySent = true;
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
