using Luxira.Application.DTOs.Upload;

namespace Luxira.Application.Interfaces;

public interface IUploadService
{
    // Validates extension/content-type and size before delegating to IStorageService.
    // fileName is used only to recover the extension; the stored file gets a
    // generated name, so the original name is never trusted as-is.
    Task<UploadResponse> UploadImageAsync(Stream content, string fileName, long length);
}
