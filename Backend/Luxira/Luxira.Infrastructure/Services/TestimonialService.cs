using FluentValidation;
using Luxira.Application.DTOs.Testimonial;
using Luxira.Application.Interfaces;
using Luxira.Domain.Interfaces;
using Mapster;

namespace Luxira.Infrastructure.Services;

public class TestimonialService : ITestimonialService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<SaveTestimonialRequest> _saveTestimonialValidator;

    public TestimonialService(IUnitOfWork unitOfWork, IValidator<SaveTestimonialRequest> saveTestimonialValidator)
    {
        _unitOfWork = unitOfWork;
        _saveTestimonialValidator = saveTestimonialValidator;
    }

    public async Task<List<TestimonialDto>> GetAllAsync()
    {
        var testimonials = await _unitOfWork.Testimonials.GetAllAsync();
        return testimonials.OrderBy(t => t.SortOrder).Adapt<List<TestimonialDto>>();
    }

    public async Task<TestimonialDto?> GetByIdAsync(Guid id)
    {
        var testimonial = await _unitOfWork.Testimonials.GetByIdAsync(id);
        return testimonial?.Adapt<TestimonialDto>();
    }

    public async Task<TestimonialDto> CreateAsync(SaveTestimonialRequest request)
    {
        await _saveTestimonialValidator.ValidateAndThrowAsync(request);

        var testimonial = new Domain.Entities.Testimonial
        {
            Name = request.Name.Trim(),
            AvatarUrl = request.AvatarUrl.Trim(),
            Rating = request.Rating,
            Text = request.Text.Trim(),
            SortOrder = request.SortOrder
        };

        await _unitOfWork.Testimonials.AddAsync(testimonial);
        await _unitOfWork.SaveChangesAsync();

        return testimonial.Adapt<TestimonialDto>();
    }

    public async Task<TestimonialDto> UpdateAsync(Guid id, SaveTestimonialRequest request)
    {
        await _saveTestimonialValidator.ValidateAndThrowAsync(request);

        var testimonial = await _unitOfWork.Testimonials.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("رأي العميل غير موجود");

        testimonial.Name = request.Name.Trim();
        testimonial.AvatarUrl = request.AvatarUrl.Trim();
        testimonial.Rating = request.Rating;
        testimonial.Text = request.Text.Trim();
        testimonial.SortOrder = request.SortOrder;

        await _unitOfWork.SaveChangesAsync();

        return testimonial.Adapt<TestimonialDto>();
    }

    public async Task DeleteAsync(Guid id)
    {
        var testimonial = await _unitOfWork.Testimonials.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("رأي العميل غير موجود");

        // Nothing references Testimonial by FK, unlike Product/Category/Brand/Bundle -
        // no DbUpdateException guard needed here.
        _unitOfWork.Testimonials.Remove(testimonial);
        await _unitOfWork.SaveChangesAsync();
    }
}
