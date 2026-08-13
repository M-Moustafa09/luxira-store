namespace Luxira.Application.DTOs.Product;

public class SaveProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MainImageUrl { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }

    public bool IsNew { get; set; }
    public bool IsBestSeller { get; set; }
    public int SortOrder { get; set; }

    public Guid CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public string? SkinType { get; set; }

    public List<SaveProductVariantRequest> Variants { get; set; } = new();
}
