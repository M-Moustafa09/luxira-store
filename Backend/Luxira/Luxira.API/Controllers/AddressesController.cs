using Luxira.Application.DTOs.Address;
using Luxira.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Controllers;

[ApiController]
[Route("api/addresses")]
public class AddressesController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressesController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    /// <summary>
    /// يرجع عناوين العميل الحالي المحفوظة.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<AddressDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AddressDto>>> GetMyAddresses()
    {
        var addresses = await _addressService.GetMyAddressesAsync();
        return Ok(addresses);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<AddressDto>> Create([FromBody] SaveAddressRequest request)
    {
        var address = await _addressService.CreateAsync(request);
        return CreatedAtAction(nameof(GetMyAddresses), address);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AddressDto>> Update(Guid id, [FromBody] SaveAddressRequest request)
    {
        var address = await _addressService.UpdateAsync(id, request);
        return Ok(address);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _addressService.DeleteAsync(id);
        return NoContent();
    }
}
