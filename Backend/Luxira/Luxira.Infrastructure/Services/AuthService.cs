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
    private readonly ICurrentUserService _currentUser;
    private readonly IConfiguration _configuration;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly IValidator<RefreshRequest> _refreshValidator;
    private readonly IValidator<LogoutRequest> _logoutValidator;

    public AuthService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IConfiguration configuration,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator,
        IValidator<RefreshRequest> refreshValidator,
        IValidator<LogoutRequest> logoutValidator)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _configuration = configuration;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _refreshValidator = refreshValidator;
        _logoutValidator = logoutValidator;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        await _registerValidator.ValidateAndThrowAsync(request);

        var email = request.Email.Trim().ToLowerInvariant();
        var existingByEmail = await _unitOfWork.Customers.FindByEmailAsync(email);
        if (existingByEmail is not null)
        {
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure(nameof(request.Email), "البريد الإلكتروني مستخدم بالفعل.")
            });
        }

        // Convert the caller's existing guest Customer (X-Guest-Id) into a
        // registered one instead of creating a new row, so their guest
        // cart/wishlist/orders carry over automatically (same CustomerId).
        var customer = await _unitOfWork.Customers.GetOrCreateGuestAsync(_currentUser.CustomerId);
        if (!customer.IsGuest)
        {
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure(nameof(request.Email), "هذا الحساب مسجل بالفعل.")
            });
        }

        customer.Name = request.Name.Trim();
        customer.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
        customer.Email = email;
        customer.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        customer.IsGuest = false;
        customer.Role = CustomerRole.Customer;

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

        // Checked after password verification (not before) so a wrong-password
        // guess never reveals whether an account is blocked.
        if (customer.IsBlocked)
        {
            throw new UnauthorizedAccessException("هذا الحساب محظور. برجاء التواصل مع خدمة العملاء لمزيد من التفاصيل.");
        }

        var response = await IssueTokensAsync(customer);
        await _unitOfWork.SaveChangesAsync();

        return response;
    }

    public async Task<AuthResponse> RefreshAsync(RefreshRequest request)
    {
        await _refreshValidator.ValidateAndThrowAsync(request);

        var token = await _unitOfWork.RefreshTokens.FindByHashAsync(HashRefreshToken(request.RefreshToken));

        if (token is null || token.RevokedAt is not null || token.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("رمز التحديث غير صالح أو منتهي الصلاحية.");
        }

        var customer = await _unitOfWork.Customers.GetByIdAsync(token.CustomerId)
            ?? throw new UnauthorizedAccessException("رمز التحديث غير صالح أو منتهي الصلاحية.");

        // Rotate: revoke the used token and issue a fresh pair, so a stolen
        // refresh token can't be replayed after the legitimate client uses it.
        token.RevokedAt = DateTime.UtcNow;

        var response = await IssueTokensAsync(customer);
        await _unitOfWork.SaveChangesAsync();

        return response;
    }

    public async Task LogoutAsync(LogoutRequest request)
    {
        await _logoutValidator.ValidateAndThrowAsync(request);

        var token = await _unitOfWork.RefreshTokens.FindByHashAsync(HashRefreshToken(request.RefreshToken));

        // Idempotent: an already-invalid/unknown token still means "logged out" from the caller's perspective.
        if (token is not null && token.RevokedAt is null)
        {
            token.RevokedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
        }
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
