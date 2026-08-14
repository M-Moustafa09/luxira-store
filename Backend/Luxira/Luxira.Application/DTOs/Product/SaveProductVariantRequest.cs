namespace Luxira.Application.DTOs.Product;

public class SaveProductVariantRequest
{
    // Present when updating an existing variant in place (echoed back from
    // ProductVariantDto.Id); omitted/null for a newly-added variant. Lets
    // UpdateAsync preserve row identity for unchanged variants instead of
    // deleting and recreating every variant on every product edit.
    public Guid? Id { get; set; }

    public string Label { get; set; } = string.Empty;
    public string ColorHex { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public int Stock { get; set; }
}
