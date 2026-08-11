using Luxira.Domain.Common;

namespace Luxira.Domain.Entities;

public class Bundle : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MainImageUrl { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }

    public string? Badge { get; set; }
    public string? BackgroundColor { get; set; }
    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<BundleItem> Items { get; set; } = new List<BundleItem>();
}
