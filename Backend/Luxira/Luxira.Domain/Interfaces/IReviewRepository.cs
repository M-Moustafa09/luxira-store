using Luxira.Domain.Entities;

namespace Luxira.Domain.Interfaces;

public interface IReviewRepository : IRepository<Review>
{
    // Storefront - visible-only, most recent first.
    Task<(List<Review> Items, int TotalCount)> GetPagedByProductAsync(Guid productId, int page, int pageSize);

    // Admin moderation - every review regardless of visibility, most recent
    // first, with Product included so the dashboard can show which product
    // each review belongs to without a follow-up call.
    Task<(List<Review> Items, int TotalCount)> GetPagedAllAsync(int page, int pageSize);

    // Used to recompute Product.Rating/ReviewsCount after any change that
    // affects which reviews are visible (create, delete, show/hide).
    Task<(int Count, decimal AverageRating)> GetVisibleStatsAsync(Guid productId);

    // Dashboard daily stats (Module A) - one query instead of three separate
    // COUNTs, since all three numbers come from the same "reviews since X"
    // row set.
    Task<(int NegativeBlocked, int Positive, int Negative)> GetDailyClassificationStatsAsync(DateTime since);

    // Module B (auto-reply): tracked (not AsNoTracking) since
    // AutoReplyBackgroundService flips AutoReplySent on each row it
    // processes in the same unit of work.
    Task<List<Review>> GetDueForAutoReplyAsync(DateTime now);
}
