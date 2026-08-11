using Luxira.Application.DTOs.Customer;

namespace Luxira.Application.Interfaces;

public interface ICustomerService
{
    Task<CustomerProfileDto> GetMyProfileAsync();
    Task<CustomerProfileDto> UpdateMyProfileAsync(UpdateProfileRequest request);
}
