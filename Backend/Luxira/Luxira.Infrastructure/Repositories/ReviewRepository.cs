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
}
