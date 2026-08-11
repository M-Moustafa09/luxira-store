using Luxira.Application.DTOs.Bundle;

namespace Luxira.Application.Interfaces;

public interface IBundleService
{
    Task<List<BundleDto>> GetAllAsync();
}
