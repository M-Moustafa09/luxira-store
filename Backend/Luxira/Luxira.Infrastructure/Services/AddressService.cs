using FluentValidation;
using Luxira.Application.DTOs.Address;
using Luxira.Application.Interfaces;
using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Mapster;

namespace Luxira.Infrastructure.Services;

public class AddressService : IAddressService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<SaveAddressRequest> _saveValidator;

    public AddressService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IValidator<SaveAddressRequest> saveValidator)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _saveValidator = saveValidator;
    }

    public async Task<List<AddressDto>> GetMyAddressesAsync()
    {
        var addresses = await _unitOfWork.Addresses.GetByCustomerIdAsync(_currentUser.CustomerId);
        return addresses.Adapt<List<AddressDto>>();
    }

    public async Task<AddressDto> CreateAsync(SaveAddressRequest request)
    {
        await _saveValidator.ValidateAndThrowAsync(request);

        var customerId = _currentUser.CustomerId;
        await _unitOfWork.Customers.GetOrCreateGuestAsync(customerId);

        if (request.IsDefault)
        {
            await _unitOfWork.Addresses.ClearDefaultAsync(customerId);
        }

        var address = new CustomerAddress
        {
            CustomerId = customerId,
            Label = string.IsNullOrWhiteSpace(request.Label) ? null : request.Label.Trim(),
            FullName = request.FullName.Trim(),
            Phone = request.Phone.Trim(),
            City = request.City.Trim(),
            Region = request.Region.Trim(),
            AddressDetails = request.AddressDetails.Trim(),
            IsDefault = request.IsDefault
        };

        await _unitOfWork.Addresses.AddAsync(address);
        await _unitOfWork.SaveChangesAsync();

        return address.Adapt<AddressDto>();
    }

    public async Task<AddressDto> UpdateAsync(Guid addressId, SaveAddressRequest request)
    {
        await _saveValidator.ValidateAndThrowAsync(request);

        var customerId = _currentUser.CustomerId;
        var address = await _unitOfWork.Addresses.FindAsync(customerId, addressId)
            ?? throw new KeyNotFoundException("العنوان غير موجود");

        if (request.IsDefault && !address.IsDefault)
        {
            await _unitOfWork.Addresses.ClearDefaultAsync(customerId);
        }

        address.Label = string.IsNullOrWhiteSpace(request.Label) ? null : request.Label.Trim();
        address.FullName = request.FullName.Trim();
        address.Phone = request.Phone.Trim();
        address.City = request.City.Trim();
        address.Region = request.Region.Trim();
        address.AddressDetails = request.AddressDetails.Trim();
        address.IsDefault = request.IsDefault;

        await _unitOfWork.SaveChangesAsync();

        return address.Adapt<AddressDto>();
    }

    public async Task DeleteAsync(Guid addressId)
    {
        var address = await _unitOfWork.Addresses.FindAsync(_currentUser.CustomerId, addressId)
            ?? throw new KeyNotFoundException("العنوان غير موجود");

        _unitOfWork.Addresses.Remove(address);
        await _unitOfWork.SaveChangesAsync();
    }
}
