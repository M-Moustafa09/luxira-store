using Luxira.Application.DTOs.Testimonial;
using Luxira.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestimonialsController : ControllerBase
{
    private readonly ITestimonialService _testimonialService;

    public TestimonialsController(ITestimonialService testimonialService)
    {
        _testimonialService = testimonialService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<TestimonialDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TestimonialDto>>> GetAll()
    {
        var testimonials = await _testimonialService.GetAllAsync();
        return Ok(testimonials);
    }
}
