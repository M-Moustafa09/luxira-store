namespace Luxira.Application.DTOs.Review;

public class ReviewReplyDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsAutomated { get; set; }
    public DateTime CreatedAt { get; set; }
}
