namespace Luxira.Application.Interfaces;

// Abstracts where uploaded files physically live (local disk today, swappable for
// cloud blob storage later) behind a plain stream-in/URL-out contract - no
// ASP.NET Core (IFormFile) or provider-specific types leak into this layer.
public interface IStorageService
{
    Task<string> SaveAsync(Stream content, string fileName, CancellationToken cancellationToken = default);
}
