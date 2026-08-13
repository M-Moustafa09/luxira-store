using Luxira.Domain.Entities;

namespace Luxira.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<List<Category>> GetAllWithSubCategoriesAsync();
    Task<Category?> GetByIdWithSubCategoriesAsync(Guid id);

    // Same DbSet-direct pattern as IProductRepository.ReplaceVariantsAsync - see
    // that method's comment for why a plain category.SubCategories.Add(...) on an
    // already-tracked Category would misfire as an UPDATE instead of an INSERT.
    Task ReplaceSubCategoriesAsync(Guid categoryId, List<SubCategory> subCategories);
}
