using FluentValidation;
using Luxira.Application.DTOs.Customer;
using Luxira.Application.Interfaces;
using Luxira.Domain.Interfaces;
using Mapster;

namespace Luxira.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<UpdateProfileRequest> _updateProfileValidator;

    public CustomerService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IValidator<UpdateProfileRequest> updateProfileValidator)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _updateProfileValidator = updateProfileValidator;
    }

    public async Task<CustomerProfileDto> GetMyProfileAsync()
    {
        var customer = await _unitOfWork.Customers.GetOrCreateGuestAsync(_currentUser.CustomerId);
        await _unitOfWork.SaveChangesAsync();

        return customer.Adapt<CustomerProfileDto>();
    }

    public async Task<CustomerProfileDto> UpdateMyProfileAsync(UpdateProfileRequest request)
    {
        await _updateProfileValidator.ValidateAndThrowAsync(request);

        var customer = await _unitOfWork.Customers.GetOrCreateGuestAsync(_currentUser.CustomerId);

        customer.Name = string.IsNullOrWhiteSpace(request.Name) ? null : request.Name.Trim();
        customer.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
        customer.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();

        await _unitOfWork.SaveChangesAsync();

        return customer.Adapt<CustomerProfileDto>();
    }
}
