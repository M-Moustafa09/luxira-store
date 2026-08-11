using Luxira.Domain.Common;

namespace Luxira.Domain.Entities;

public class Campaign : BaseEntity
{
    public DateTime EndsAt { get; set; }
    public int MaxDiscountPercent { get; set; }
    public bool IsActive { get; set; } = true;
}
