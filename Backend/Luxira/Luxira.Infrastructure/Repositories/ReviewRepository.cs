using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Luxira.Infrastructure.Repositories;

public class ReviewRepository : RepositoryBase<Review>, IReviewRepository
{
    public ReviewRepository(LuxiraDbContext context) : base(context)
    {
    }

    public async Task<(List<Review> Items, int TotalCount)> GetPagedByProductAsync(Guid productId, int page, int pageSize)
    {
        var query = DbSet.AsNoTracking()
            .Include(r => r.Replies.OrderBy(reply => reply.CreatedAt))
            .Where(r => r.ProductId == productId && r.IsVisible)
            .OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(List<Review> Items, int TotalCount)> GetPagedAllAsync(int page, int pageSize)
    {
        var query = DbSet.AsNoTracking()
            .Include(r => r.Product)
            .Include(r => r.Replies.OrderBy(reply => reply.CreatedAt))
            .OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(int Count, decimal AverageRating)> GetVisibleStatsAsync(Guid productId)
    {
        var visibleRatings = await DbSet.AsNoTracking()
            .Where(r => r.ProductId == productId && r.IsVisible)
            .Select(r => r.Rating)
            .ToListAsync();

        if (visibleRatings.Count == 0)
        {
            return (0, 0m);
        }

        return (visibleRatings.Count, (decimal)visibleRatings.Average());
    }

    public async Task<(int NegativeBlocked, int Positive, int Negative)> GetDailyClassificationStatsAsync(DateTime since)
    {
        var rows = await DbSet.AsNoTracking()
            .Where(r => r.CreatedAt >= since)
            .Select(r => new { r.Rating, r.IsFlaggedNegative })
            .ToListAsync();

        var negativeBlocked = rows.Count(r => r.IsFlaggedNegative);
        var positive = rows.Count(r => r.Rating >= 4);
        var negative = rows.Count(r => r.Rating <= 2);

        return (negativeBlocked, positive, negative);
    }

    public Task<List<Review>> GetDueForAutoReplyAsync(DateTime now)
    {
        return DbSet
            .Where(r => r.AutoReplyDueAt != null && r.AutoReplyDueAt <= now && !r.AutoReplySent)
            .ToListAsync();
    }
}
