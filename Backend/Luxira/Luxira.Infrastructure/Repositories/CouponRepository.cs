using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Luxira.Infrastructure.Repositories;

public class CouponRepository : RepositoryBase<Coupon>, ICouponRepository
{
    public CouponRepository(LuxiraDbContext context) : base(context)
    {
    }

    public Task<Coupon?> FindByCodeAsync(string code) =>
        DbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Code == code);
}
