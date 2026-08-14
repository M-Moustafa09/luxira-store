using Luxira.Domain.Entities;

namespace Luxira.Domain.Interfaces;

public enum ProductSortOption
{
    Relevance,
    PriceAsc,
    PriceDesc,
    Rating,
    Newest
}

public class ProductSearchCriteria
{
    public string? Search { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public bool? IsNew { get; set; }
    public bool? IsBestSeller { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinRating { get; set; }
    public SkinType? SkinType { get; set; }
    public ProductSortOption Sort { get; set; } = ProductSortOption.Relevance;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public interface IProductRepository : IRepository<Product>
{
    Task<(List<Product> Items, int TotalCount)> SearchAsync(ProductSearchCriteria criteria);

    Task<Product?> GetByIdWithVariantsAsync(Guid id);

    Task<List<Product>> GetRelatedAsync(Guid productId, Guid categoryId, int take);

    // Upserts by Id (each incoming variant with an Id matching an existing row is
    // updated in place; without a match it's inserted) and only removes existing
    // variants that aren't present in the incoming list at all. Existing rows keep
    // their identity across an update, so a CartItem referencing an unchanged
    // variant is never affected - only a variant actually being removed can hit
    // the CartItem FK (DeleteBehavior.Restrict), and only then. New variants are
    // added directly via the DbSet rather than via product.Variants on an
    // already-tracked Product, since attaching new children to an already-tracked
    // parent's collection nav lets EF's key-default heuristic mistake the
    // client-generated (non-empty) Guid Id for an existing row (same class of bug
    // fixed for OrderStatusHistory in IOrderRepository.AddStatusHistory).
    Task UpsertVariantsAsync(Guid productId, List<ProductVariant> variants);

    Task<List<ProductCountryPrice>> GetCountryPricesAsync(Guid productId);
    Task<ProductCountryPrice?> GetCountryPriceAsync(Guid productId, Country country);
    Task<List<ProductCountryPrice>> GetCountryPricesForProductsAsync(List<Guid> productIds, Country country);
    Task ReplaceCountryPricesAsync(Guid productId, List<ProductCountryPrice> prices);

    // Tracked (not AsNoTracking) - callers decrement Stock on the returned
    // entities and rely on the caller's own SaveChangesAsync to persist it.
    Task<List<ProductVariant>> GetVariantsByIdsAsync(List<Guid> variantIds);

    // One variant per product id (lowest SortOrder) - used to resolve which
    // variant's stock a Bundle's product line consumes, since BundleItem has
    // no ProductVariantId of its own (bundles don't pin a specific shade).
    Task<List<ProductVariant>> GetDefaultVariantsByProductIdsAsync(List<Guid> productIds);
}
