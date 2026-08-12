using Luxira.Application.DTOs.Auth;

namespace Luxira.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshAsync(RefreshRequest request);
    Task LogoutAsync(LogoutRequest request);
}
