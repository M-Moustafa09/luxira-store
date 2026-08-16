namespace Luxira.Application.DTOs.Review;

public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }

    // Populated only when the source query included Product (the admin
    // moderation list) - null on storefront responses, where the client is
    // already on that product's page and the context is redundant.
    public string? ProductName { get; set; }
    public string? ProductImageUrl { get; set; }

    public string AuthorName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsVisible { get; set; }
    public DateTime CreatedAt { get; set; }
}
