using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FluentValidation;
using Luxira.Application.DTOs.Auth;
using Luxira.Application.DTOs.Customer;
using Luxira.Application.Interfaces;
using Luxira.Domain.Entities;
using Luxira.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Luxira.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthService(
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        await _registerValidator.ValidateAndThrowAsync(request);

        var email = request.Email.Trim().ToLowerInvariant();
        var existing = await _unitOfWork.Customers.FindByEmailAsync(email);
        if (existing is not null)
        {
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure(nameof(request.Email), "البريد الإلكتروني مستخدم بالفعل.")
            });
        }

        var customer = new Customer
        {
            Name = request.Name.Trim(),
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsGuest = false,
            Role = CustomerRole.Customer
        };

        await _unitOfWork.Customers.AddAsync(customer);

        var response = await IssueTokensAsync(customer);
        await _unitOfWork.SaveChangesAsync();

        return response;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        await _loginValidator.ValidateAndThrowAsync(request);

        var email = request.Email.Trim().ToLowerInvariant();
        var customer = await _unitOfWork.Customers.FindByEmailAsync(email);

        if (customer?.PasswordHash is null || !BCrypt.Net.BCrypt.Verify(request.Password, customer.PasswordHash))
        {
            throw new UnauthorizedAccessException("البريد الإلكتروني أو كلمة المرور غير صحيحة.");
        }

        var response = await IssueTokensAsync(customer);
        await _unitOfWork.SaveChangesAsync();

        return response;
    }

    private async Task<AuthResponse> IssueTokensAsync(Customer customer)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var secret = jwtSection["Secret"]
            ?? throw new InvalidOperationException("Jwt:Secret is not configured.");

        var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSection["AccessTokenMinutes"]!));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
            new Claim(ClaimTypes.Role, customer.Role.ToString())
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: signingCredentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        var refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        await _unitOfWork.RefreshTokens.AddAsync(new RefreshToken
        {
            CustomerId = customer.Id,
            TokenHash = HashRefreshToken(refreshTokenValue),
            ExpiresAt = DateTime.UtcNow.AddDays(int.Parse(jwtSection["RefreshTokenDays"]!))
        });

        return new AuthResponse
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = expires,
            RefreshToken = refreshTokenValue,
            Customer = new CustomerProfileDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Phone = customer.Phone,
                Email = customer.Email
            }
        };
    }

    private static string HashRefreshToken(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
