using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Luxira.Infrastructure.Repositories;

public class RefreshTokenRepository : RepositoryBase<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(LuxiraDbContext context) : base(context)
    {
    }

    public Task<RefreshToken?> FindByHashAsync(string tokenHash) =>
        DbSet.FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
}
