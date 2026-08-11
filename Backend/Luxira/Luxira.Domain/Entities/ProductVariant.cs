using Luxira.Domain.Common;

namespace Luxira.Domain.Entities;

public class ProductVariant : BaseEntity
{
    public string Label { get; set; } = string.Empty;
    public string ColorHex { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
