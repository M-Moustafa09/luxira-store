using Luxira.Application.DTOs.Customer;

namespace Luxira.Application.DTOs.Auth;

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public CustomerProfileDto Customer { get; set; } = null!;
}
