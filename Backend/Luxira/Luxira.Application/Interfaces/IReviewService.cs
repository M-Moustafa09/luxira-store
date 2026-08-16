using Luxira.Application.DTOs.Common;
using Luxira.Application.DTOs.Review;

namespace Luxira.Application.Interfaces;

public interface IReviewService
{
    Task<PagedResult<ReviewDto>> GetByProductAsync(Guid productId, int page, int pageSize);
    Task<ReviewDto> CreateAsync(Guid productId, CreateReviewRequest request);

    // Admin moderation (Tasks 2 & 3): global list regardless of visibility,
    // hide/show without deleting, and hard delete. Both mutations recompute
    // the owning product's Rating/ReviewsCount afterward, same as CreateAsync.
    Task<PagedResult<ReviewDto>> GetAllAsync(int page, int pageSize);
    Task<ReviewDto> SetVisibilityAsync(Guid id, bool isVisible);
    Task DeleteAsync(Guid id);
}
