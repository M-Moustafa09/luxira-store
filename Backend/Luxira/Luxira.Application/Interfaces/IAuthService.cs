using Luxira.Application.DTOs.Auth;

namespace Luxira.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}
