using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Luxira.Infrastructure.Repositories;

public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
{
    public CategoryRepository(LuxiraDbContext context) : base(context)
    {
    }

    public Task<List<Category>> GetAllWithSubCategoriesAsync() =>
        DbSet.AsNoTracking()
            .Include(c => c.SubCategories)
            .ToListAsync();

    public Task<Category?> GetByIdWithSubCategoriesAsync(Guid id) =>
        DbSet.AsNoTracking()
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task ReplaceSubCategoriesAsync(Guid categoryId, List<SubCategory> subCategories)
    {
        var existing = await Context.Set<SubCategory>()
            .Where(s => s.CategoryId == categoryId)
            .ToListAsync();

        Context.Set<SubCategory>().RemoveRange(existing);
        await Context.Set<SubCategory>().AddRangeAsync(subCategories);
    }
}
