using FluentValidation;
using Luxira.Application.DTOs.Upload;
using Luxira.Application.Interfaces;

namespace Luxira.Infrastructure.Services;

public class UploadService : IUploadService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    private const long MaxSizeBytes = 5 * 1024 * 1024;

    private readonly IStorageService _storageService;

    public UploadService(IStorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<UploadResponse> UploadImageAsync(Stream content, string fileName, long length)
    {
        var extension = Path.GetExtension(fileName);

        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            throw new ValidationException(
            [
                new FluentValidation.Results.ValidationFailure(nameof(fileName), "صيغة الملف غير مدعومة، يُسمح فقط بـ JPG أو PNG أو WEBP")
            ]);
        }

        if (length <= 0 || length > MaxSizeBytes)
        {
            throw new ValidationException(
            [
                new FluentValidation.Results.ValidationFailure(nameof(length), "حجم الملف كبير جداً، الحد الأقصى 5 ميجابايت")
            ]);
        }

        var storedFileName = $"{Guid.NewGuid()}{extension.ToLowerInvariant()}";
        var url = await _storageService.SaveAsync(content, storedFileName);

        return new UploadResponse { Url = url };
    }
}
