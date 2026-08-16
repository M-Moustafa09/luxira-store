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

    // Distinguishes an auto-blocked review (negative-keyword match) from one
    // an admin hid manually - both share IsVisible=false, but only this one
    // was the negative-keyword filter's doing.
    public bool IsFlaggedNegative { get; set; }

    public DateTime CreatedAt { get; set; }

    // Oldest first - populated whenever the source query included Replies
    // (both storefront and admin queries do), empty otherwise (e.g. the
    // create-response mapping, which maps a freshly-added Review with no
    // replies yet).
    public List<ReviewReplyDto> Replies { get; set; } = [];
}
