using FluentAssertions;
using FluentValidation;
using Luxira.Application.DTOs.Coupon;
using Luxira.Application.Validators.Coupon;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Services;
using NSubstitute;
using Coupon = Luxira.Domain.Entities.Coupon;

namespace Luxira.Tests.Services;

public class CouponServiceTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CouponService _sut;

    public CouponServiceTests()
    {
        _sut = new CouponService(_unitOfWork, new SaveCouponRequestValidator());
    }

    private static SaveCouponRequest ValidRequest(string code = "save10") => new()
    {
        Code = code,
        DiscountType = "Percentage",
        DiscountValue = 10,
        IsActive = true
    };

    [Fact]
    public async Task CreateAsync_UppercasesTheCode()
    {
        _unitOfWork.Coupons.FindByCodeAsync(Arg.Any<string>()).Returns((Coupon?)null);

        var result = await _sut.CreateAsync(ValidRequest("save10"));

        result.Code.Should().Be("SAVE10");
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenCodeAlreadyExists()
    {
        _unitOfWork.Coupons.FindByCodeAsync("SAVE10").Returns(new Coupon { Code = "SAVE10" });

        var act = () => _sut.CreateAsync(ValidRequest("save10"));

        await act.Should().ThrowAsync<ValidationException>();
        await _unitOfWork.Coupons.DidNotReceive().AddAsync(Arg.Any<Coupon>());
    }

    [Fact]
    public async Task UpdateAsync_Allows_KeepingTheSameCodeOnTheSameCoupon()
    {
        var existing = new Coupon { Code = "SAVE10" };
        _unitOfWork.Coupons.GetByIdAsync(existing.Id).Returns(existing);
        // FindByCodeAsync returning the same row (matching Id) should not be treated as a conflict.
        _unitOfWork.Coupons.FindByCodeAsync("SAVE10").Returns(existing);

        var act = () => _sut.UpdateAsync(existing.Id, ValidRequest("save10"));

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task UpdateAsync_Throws_WhenCodeBelongsToADifferentCoupon()
    {
        var existing = new Coupon { Code = "OLDCODE" };
        var other = new Coupon { Code = "SAVE10" };
        _unitOfWork.Coupons.GetByIdAsync(existing.Id).Returns(existing);
        _unitOfWork.Coupons.FindByCodeAsync("SAVE10").Returns(other);

        var act = () => _sut.UpdateAsync(existing.Id, ValidRequest("save10"));

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task DeleteAsync_RemovesTheCoupon_WithNoFkGuard()
    {
        var existing = new Coupon { Code = "SAVE10" };
        _unitOfWork.Coupons.GetByIdAsync(existing.Id).Returns(existing);

        await _sut.DeleteAsync(existing.Id);

        _unitOfWork.Coupons.Received(1).Remove(existing);
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task DeleteAsync_Throws_WhenCouponDoesNotExist()
    {
        _unitOfWork.Coupons.GetByIdAsync(Arg.Any<Guid>()).Returns((Coupon?)null);

        var act = () => _sut.DeleteAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
