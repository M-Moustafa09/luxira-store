using Luxira.Application.DTOs.Upload;
using Luxira.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Controllers;

[ApiController]
[Route("api/admin/uploads")]
[Authorize(Roles = "Admin")]
public class AdminUploadsController : ControllerBase
{
    private readonly IUploadService _uploadService;

    public AdminUploadsController(IUploadService uploadService)
    {
        _uploadService = uploadService;
    }

    /// <summary>
    /// يرفع صورة (منتج أو درجة) ويرجع الرابط اللي يتحط في MainImageUrl/ImageUrl.
    /// </summary>
    [HttpPost("images")]
    [RequestSizeLimit(5_242_880)]
    [ProducesResponseType(typeof(UploadResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UploadResponse>> UploadImage([FromForm] IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        var result = await _uploadService.UploadImageAsync(stream, file.FileName, file.Length);
        return Ok(result);
    }
}
