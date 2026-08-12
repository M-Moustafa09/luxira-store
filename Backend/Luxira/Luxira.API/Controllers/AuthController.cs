using Luxira.Application.DTOs.Auth;
using Luxira.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// ينشئ حساب عميل جديد ويرجع access/refresh token.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// تسجيل دخول بالبريد الإلكتروني وكلمة المرور، يرجع access/refresh token.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }
}
