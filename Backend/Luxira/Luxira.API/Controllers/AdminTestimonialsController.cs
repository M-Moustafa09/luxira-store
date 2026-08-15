using Luxira.Application.DTOs.Testimonial;
using Luxira.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Luxira.API.Controllers;

[ApiController]
[Route("api/admin/testimonials")]
[Authorize(Roles = "Admin")]
public class AdminTestimonialsController : ControllerBase
{
    private readonly ITestimonialService _testimonialService;

    public AdminTestimonialsController(ITestimonialService testimonialService)
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

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TestimonialDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TestimonialDto>> GetById(Guid id)
    {
        var testimonial = await _testimonialService.GetByIdAsync(id);
        if (testimonial is null)
        {
            return NotFound();
        }

        return Ok(testimonial);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TestimonialDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<TestimonialDto>> Create([FromBody] SaveTestimonialRequest request)
    {
        var testimonial = await _testimonialService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = testimonial.Id }, testimonial);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TestimonialDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TestimonialDto>> Update(Guid id, [FromBody] SaveTestimonialRequest request)
    {
        var testimonial = await _testimonialService.UpdateAsync(id, request);
        return Ok(testimonial);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _testimonialService.DeleteAsync(id);
        return NoContent();
    }
}
