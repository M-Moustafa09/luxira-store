using Luxira.Domain.Common;

namespace Luxira.Domain.Entities;

public class SubCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
