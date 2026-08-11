using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Luxira.Infrastructure.Repositories;

public class ProductRepository : RepositoryBase<Product>, IProductRepository
{
    public ProductRepository(LuxiraDbContext context) : base(context)
    {
    }

    public async Task<(List<Product> Items, int TotalCount)> SearchAsync(ProductSearchCriteria criteria)
    {
        var query = DbSet.AsNoTracking()
            .Include(p => p.Variants)
            .Include(p => p.Brand)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(criteria.Search))
        {
            query = query.Where(p =>
                p.Name.Contains(criteria.Search) ||
                p.Subtitle.Contains(criteria.Search));
        }

        if (criteria.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == criteria.CategoryId.Value);
        }

        if (criteria.BrandId.HasValue)
        {
            query = query.Where(p => p.BrandId == criteria.BrandId.Value);
        }

        if (criteria.IsNew.HasValue)
        {
            query = query.Where(p => p.IsNew == criteria.IsNew.Value);
        }

        if (criteria.IsBestSeller.HasValue)
        {
            query = query.Where(p => p.IsBestSeller == criteria.IsBestSeller.Value);
        }

        if (criteria.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= criteria.MinPrice.Value);
        }

        if (criteria.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= criteria.MaxPrice.Value);
        }

        if (criteria.MinRating.HasValue)
        {
            query = query.Where(p => p.Rating >= criteria.MinRating.Value);
        }

        if (criteria.SkinType.HasValue)
        {
            query = query.Where(p => p.SkinType == criteria.SkinType.Value);
        }

        query = criteria.Sort switch
        {
            ProductSortOption.PriceAsc => query.OrderBy(p => p.Price),
            ProductSortOption.PriceDesc => query.OrderByDescending(p => p.Price),
            ProductSortOption.Rating => query.OrderByDescending(p => p.Rating),
            ProductSortOption.Newest => query.OrderByDescending(p => p.CreatedAt),
            _ => query.OrderBy(p => p.SortOrder).ThenBy(p => p.Name)
        };

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public Task<Product?> GetByIdWithVariantsAsync(Guid id) =>
        DbSet.AsNoTracking()
            .Include(p => p.Variants)
            .Include(p => p.Brand)
            .FirstOrDefaultAsync(p => p.Id == id);

    public Task<List<Product>> GetRelatedAsync(Guid productId, Guid categoryId, int take) =>
        DbSet.AsNoTracking()
            .Include(p => p.Variants)
            .Include(p => p.Brand)
            .Where(p => p.CategoryId == categoryId && p.Id != productId)
            .OrderBy(p => p.SortOrder)
            .Take(take)
            .ToListAsync();
}
