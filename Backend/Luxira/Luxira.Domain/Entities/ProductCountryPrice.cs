using Luxira.Domain.Common;

namespace Luxira.Domain.Entities;

public class ProductCountryPrice : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Country Country { get; set; }
    public decimal Price { get; set; }
}
