using Luxira.Domain.Common;

namespace Luxira.Domain.Entities;

public class BuyMoreOffer : BaseEntity
{
    public int Quantity { get; set; }
    public int DiscountPercent { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
