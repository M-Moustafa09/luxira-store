namespace Luxira.Application.DTOs.Category;

public class SaveCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public List<SaveSubCategoryRequest> SubCategories { get; set; } = new();
}
