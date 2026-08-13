using Luxira.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ICurrentUserService _currentUser;

    public AdminController(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    /// <summary>
    /// يتأكد إن الـ [Authorize(Roles = "Admin")] شغال فعلاً end-to-end فوق الـ JWT الحالي.
    /// </summary>
    [HttpGet("ping")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Ping()
    {
        return Ok(new { customerId = _currentUser.CustomerId, message = "Admin access confirmed." });
    }
}
