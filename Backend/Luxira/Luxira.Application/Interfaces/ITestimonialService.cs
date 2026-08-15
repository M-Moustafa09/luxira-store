using Luxira.Application.DTOs.Testimonial;

namespace Luxira.Application.Interfaces;

public interface ITestimonialService
{
    Task<List<TestimonialDto>> GetAllAsync();
    Task<TestimonialDto?> GetByIdAsync(Guid id);
    Task<TestimonialDto> CreateAsync(SaveTestimonialRequest request);
    Task<TestimonialDto> UpdateAsync(Guid id, SaveTestimonialRequest request);
    Task DeleteAsync(Guid id);
}
