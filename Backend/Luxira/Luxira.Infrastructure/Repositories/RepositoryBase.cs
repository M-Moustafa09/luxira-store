using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Luxira.Infrastructure.Repositories;

public class RepositoryBase<T> : IRepository<T> where T : class
{
    protected readonly LuxiraDbContext Context;
    protected readonly DbSet<T> DbSet;

    public RepositoryBase(LuxiraDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public Task<T?> GetByIdAsync(Guid id) => DbSet.FindAsync(id).AsTask();

    public Task<List<T>> GetAllAsync() => DbSet.AsNoTracking().ToListAsync();

    public Task AddAsync(T entity) => DbSet.AddAsync(entity).AsTask();

    public void Update(T entity) => DbSet.Update(entity);

    public void Remove(T entity) => DbSet.Remove(entity);
}
