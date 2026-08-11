using Luxira.Application.DTOs.Address;

namespace Luxira.Application.Interfaces;

public interface IAddressService
{
    Task<List<AddressDto>> GetMyAddressesAsync();
    Task<AddressDto> CreateAsync(SaveAddressRequest request);
    Task<AddressDto> UpdateAsync(Guid addressId, SaveAddressRequest request);
    Task DeleteAsync(Guid addressId);
}
