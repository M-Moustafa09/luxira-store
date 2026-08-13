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

    public async Task ReplaceVariantsAsync(Guid productId, List<ProductVariant> variants)
    {
        var existing = await Context.Set<ProductVariant>()
            .Where(v => v.ProductId == productId)
            .ToListAsync();

        Context.Set<ProductVariant>().RemoveRange(existing);
        await Context.Set<ProductVariant>().AddRangeAsync(variants);
    }

    public Task<List<ProductCountryPrice>> GetCountryPricesAsync(Guid productId) =>
        Context.Set<ProductCountryPrice>()
            .AsNoTracking()
            .Where(p => p.ProductId == productId)
            .OrderBy(p => p.Country)
            .ToListAsync();

    public Task<ProductCountryPrice?> GetCountryPriceAsync(Guid productId, Country country) =>
        Context.Set<ProductCountryPrice>()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProductId == productId && p.Country == country);

    public Task<List<ProductCountryPrice>> GetCountryPricesForProductsAsync(List<Guid> productIds, Country country) =>
        Context.Set<ProductCountryPrice>()
            .AsNoTracking()
            .Where(p => productIds.Contains(p.ProductId) && p.Country == country)
            .ToListAsync();

    public async Task ReplaceCountryPricesAsync(Guid productId, List<ProductCountryPrice> prices)
    {
        var existing = await Context.Set<ProductCountryPrice>()
            .Where(p => p.ProductId == productId)
            .ToListAsync();

        Context.Set<ProductCountryPrice>().RemoveRange(existing);
        await Context.Set<ProductCountryPrice>().AddRangeAsync(prices);
    }

    public Task<List<ProductVariant>> GetVariantsByIdsAsync(List<Guid> variantIds) =>
        Context.Set<ProductVariant>()
            .Include(v => v.Product)
            .Where(v => variantIds.Contains(v.Id))
            .ToListAsync();

    public async Task<List<ProductVariant>> GetDefaultVariantsByProductIdsAsync(List<Guid> productIds)
    {
        var variants = await Context.Set<ProductVariant>()
            .Include(v => v.Product)
            .Where(v => productIds.Contains(v.ProductId))
            .OrderBy(v => v.SortOrder)
            .ToListAsync();

        return variants
            .GroupBy(v => v.ProductId)
            .Select(g => g.First())
            .ToList();
    }
}
