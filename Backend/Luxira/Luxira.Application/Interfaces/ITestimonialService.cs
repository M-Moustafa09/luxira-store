using Luxira.Application.DTOs.Testimonial;

namespace Luxira.Application.Interfaces;

public interface ITestimonialService
{
    Task<List<TestimonialDto>> GetAllAsync();
}
