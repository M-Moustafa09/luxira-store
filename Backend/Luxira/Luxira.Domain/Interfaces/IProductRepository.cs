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

    // Replaces the full variant set directly via the DbSet rather than mutating
    // product.Variants on an already-tracked Product, since attaching new children
    // to an already-tracked parent's collection nav lets EF's key-default heuristic
    // mistake the client-generated (non-empty) Guid Id for an existing row (same
    // class of bug fixed for OrderStatusHistory in IOrderRepository.AddStatusHistory).
    Task ReplaceVariantsAsync(Guid productId, List<ProductVariant> variants);

    Task<List<ProductCountryPrice>> GetCountryPricesAsync(Guid productId);
    Task<ProductCountryPrice?> GetCountryPriceAsync(Guid productId, Country country);
    Task<List<ProductCountryPrice>> GetCountryPricesForProductsAsync(List<Guid> productIds, Country country);
    Task ReplaceCountryPricesAsync(Guid productId, List<ProductCountryPrice> prices);
}
