using Luxira.Domain.Entities;

namespace Luxira.Domain.Interfaces;

public interface ICouponRepository : IRepository<Coupon>
{
    Task<Coupon?> FindByCodeAsync(string code);
}
