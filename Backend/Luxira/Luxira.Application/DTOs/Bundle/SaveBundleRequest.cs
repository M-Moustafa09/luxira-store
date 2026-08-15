namespace Luxira.Application.DTOs.Bundle;

public class SaveBundleRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MainImageUrl { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }

    public string? Badge { get; set; }
    public string? BackgroundColor { get; set; }
    public int SortOrder { get; set; }

    public List<SaveBundleItemRequest> Items { get; set; } = new();
}

public class SaveBundleItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}
