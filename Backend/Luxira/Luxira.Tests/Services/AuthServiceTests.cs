using FluentAssertions;
using Luxira.Application.DTOs.Auth;
using Luxira.Application.Interfaces;
using Luxira.Application.Validators.Auth;
using Luxira.Domain.Interfaces;
using Luxira.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Customer = Luxira.Domain.Entities.Customer;

namespace Luxira.Tests.Services;

public class AuthServiceTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly IConfiguration _configuration = Substitute.For<IConfiguration>();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _sut = new AuthService(
            _unitOfWork,
            _currentUser,
            _configuration,
            new RegisterRequestValidator(),
            new LoginRequestValidator(),
            new RefreshRequestValidator(),
            new LogoutRequestValidator());
    }

    [Fact]
    public async Task LoginAsync_Throws_WhenCredentialsAreCorrectButCustomerIsBlocked()
    {
        var customer = new Customer
        {
            Email = "blocked@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct-password"),
            IsBlocked = true
        };
        _unitOfWork.Customers.FindByEmailAsync("blocked@example.com").Returns(customer);

        var act = () => _sut.LoginAsync(new LoginRequest { Email = "blocked@example.com", Password = "correct-password" });

        var exception = await act.Should().ThrowAsync<UnauthorizedAccessException>();
        exception.Which.Message.Should().Contain("محظور");

        // No tokens should be issued for a blocked account.
        await _unitOfWork.RefreshTokens.DidNotReceive().AddAsync(Arg.Any<Domain.Entities.RefreshToken>());
    }

    [Fact]
    public async Task LoginAsync_Throws_WrongCredentialsMessage_BeforeCheckingBlockedStatus()
    {
        // A blocked account with a WRONG password should still get the generic
        // "wrong email or password" message, never revealing it's blocked to
        // someone who hasn't proven they own the account.
        var customer = new Customer
        {
            Email = "blocked@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct-password"),
            IsBlocked = true
        };
        _unitOfWork.Customers.FindByEmailAsync("blocked@example.com").Returns(customer);

        var act = () => _sut.LoginAsync(new LoginRequest { Email = "blocked@example.com", Password = "wrong-password" });

        var exception = await act.Should().ThrowAsync<UnauthorizedAccessException>();
        exception.Which.Message.Should().NotContain("محظور");
    }
}
