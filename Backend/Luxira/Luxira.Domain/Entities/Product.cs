using Luxira.Domain.Common;

namespace Luxira.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MainImageUrl { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }

    public decimal Rating { get; set; }
    public int ReviewsCount { get; set; }

    public bool IsNew { get; set; }
    public bool IsBestSeller { get; set; }
    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public Guid? BrandId { get; set; }
    public Brand? Brand { get; set; }

    public SkinType? SkinType { get; set; }

    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
}
