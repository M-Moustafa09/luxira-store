namespace Luxira.Application.DTOs.Review;

public class CreateReviewRequest
{
    public string AuthorName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Text { get; set; } = string.Empty;
}
