namespace Luxira.Application.DTOs.Promotions;

public class SaveCampaignRequest
{
    public DateTime EndsAt { get; set; }
    public int MaxDiscountPercent { get; set; }
    public bool IsActive { get; set; } = true;
}
