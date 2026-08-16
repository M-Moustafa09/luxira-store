using FluentValidation;
using Luxira.Application.DTOs.Common;
using Luxira.Application.DTOs.Review;
using Luxira.Application.Interfaces;
using Luxira.Domain.Interfaces;
using Mapster;

namespace Luxira.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<CreateReviewRequest> _createReviewValidator;

    public ReviewService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IValidator<CreateReviewRequest> createReviewValidator)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _createReviewValidator = createReviewValidator;
    }

    public async Task<PagedResult<ReviewDto>> GetByProductAsync(Guid productId, int page, int pageSize)
    {
        var (items, totalCount) = await _unitOfWork.Reviews.GetPagedByProductAsync(productId, page, pageSize);

        return new PagedResult<ReviewDto>
        {
            Items = items.Adapt<List<ReviewDto>>(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ReviewDto> CreateAsync(Guid productId, CreateReviewRequest request)
    {
        await _createReviewValidator.ValidateAndThrowAsync(request);

        var product = await _unitOfWork.Products.GetByIdAsync(productId)
            ?? throw new KeyNotFoundException("المنتج غير موجود");

        var review = new Domain.Entities.Review
        {
            ProductId = productId,
            CustomerId = _currentUser.CustomerId,
            AuthorName = request.AuthorName.Trim(),
            Rating = request.Rating,
            Text = request.Text.Trim()
        };

        await _unitOfWork.Reviews.AddAsync(review);
        await _unitOfWork.SaveChangesAsync();

        await RecomputeProductAggregateAsync(productId);

        return review.Adapt<ReviewDto>();
    }

    public async Task<PagedResult<ReviewDto>> GetAllAsync(int page, int pageSize)
    {
        var (items, totalCount) = await _unitOfWork.Reviews.GetPagedAllAsync(page, pageSize);

        return new PagedResult<ReviewDto>
        {
            Items = items.Adapt<List<ReviewDto>>(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ReviewDto> SetVisibilityAsync(Guid id, bool isVisible)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("التقييم غير موجود");

        review.IsVisible = isVisible;
        await _unitOfWork.SaveChangesAsync();

        await RecomputeProductAggregateAsync(review.ProductId);

        return review.Adapt<ReviewDto>();
    }

    public async Task DeleteAsync(Guid id)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("التقييم غير موجود");

        var productId = review.ProductId;

        // No FK guard needed here (unlike Product/Category/Brand/Bundle
        // deletion) - nothing references a Review by FK.
        _unitOfWork.Reviews.Remove(review);
        await _unitOfWork.SaveChangesAsync();

        await RecomputeProductAggregateAsync(productId);
    }

    // Re-queries visible-review stats from the DB fresh (after whatever
    // mutation just happened has already been saved) rather than adjusting
    // Product.Rating/ReviewsCount with in-memory math - simpler to reason
    // about correctly for create/delete/hide/show alike than hand-rolling a
    // running-average update per operation type.
    private async Task RecomputeProductAggregateAsync(Guid productId)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId);
        if (product is null)
        {
            return;
        }

        var (count, average) = await _unitOfWork.Reviews.GetVisibleStatsAsync(productId);
        product.Rating = count > 0 ? Math.Round(average, 2) : 0;
        product.ReviewsCount = count;

        await _unitOfWork.SaveChangesAsync();
    }
}
