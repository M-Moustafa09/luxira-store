namespace Luxira.Application.DTOs.Product;

public class SaveProductVariantRequest
{
    public string Label { get; set; } = string.Empty;
    public string ColorHex { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
