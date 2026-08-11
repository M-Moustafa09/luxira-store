using Luxira.Application.DTOs.Customer;
using Luxira.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// يرجع الملف الشخصي للعميل الحالي.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(CustomerProfileDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CustomerProfileDto>> GetMe()
    {
        var profile = await _customerService.GetMyProfileAsync();
        return Ok(profile);
    }

    [HttpPut("me")]
    [ProducesResponseType(typeof(CustomerProfileDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CustomerProfileDto>> UpdateMe([FromBody] UpdateProfileRequest request)
    {
        var profile = await _customerService.UpdateMyProfileAsync(request);
        return Ok(profile);
    }
}
