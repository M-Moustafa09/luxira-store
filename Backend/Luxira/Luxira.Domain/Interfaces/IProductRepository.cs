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
}
