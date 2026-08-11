using Luxira.Domain.Common;

namespace Luxira.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
}
