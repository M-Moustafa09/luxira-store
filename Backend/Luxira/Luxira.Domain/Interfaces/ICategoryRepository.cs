using Luxira.Domain.Entities;

namespace Luxira.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<List<Category>> GetAllWithSubCategoriesAsync();
}
