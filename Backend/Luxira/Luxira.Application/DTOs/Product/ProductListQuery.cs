namespace Luxira.Application.DTOs.Product;

public enum ProductSort
{
    Relevance,
    PriceAsc,
    PriceDesc,
    Rating,
    Newest
}

public class ProductListQuery
{
    public string? Search { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public bool? IsNew { get; set; }
    public bool? IsBestSeller { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinRating { get; set; }
    public string? SkinType { get; set; }
    public ProductSort Sort { get; set; } = ProductSort.Relevance;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
