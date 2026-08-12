using Luxira.Domain.Entities;

namespace Luxira.Domain.Interfaces;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> FindByHashAsync(string tokenHash);
}
