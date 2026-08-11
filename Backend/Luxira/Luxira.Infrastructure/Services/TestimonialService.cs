using Luxira.Application.DTOs.Testimonial;
using Luxira.Application.Interfaces;
using Luxira.Domain.Interfaces;
using Mapster;

namespace Luxira.Infrastructure.Services;

public class TestimonialService : ITestimonialService
{
    private readonly IUnitOfWork _unitOfWork;

    public TestimonialService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TestimonialDto>> GetAllAsync()
    {
        var testimonials = await _unitOfWork.Testimonials.GetAllAsync();
        return testimonials.OrderBy(t => t.SortOrder).Adapt<List<TestimonialDto>>();
    }
}
