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
}
