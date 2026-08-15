namespace Luxira.Application.DTOs.Testimonial;

public class SaveTestimonialRequest
{
    public string Name { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Text { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
