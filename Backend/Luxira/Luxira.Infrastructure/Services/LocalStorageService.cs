using Luxira.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Luxira.Infrastructure.Services;

// Saves files to local disk under Storage:RootPath (relative to the process's
// working directory, same convention as GeoIp:DatabasePath). Program.cs maps
// that same physical folder to the Storage:PublicPath request path via
// UseStaticFiles, so the URL returned here is directly usable by the frontend.
// Swappable for a cloud blob implementation later without touching callers.
public class LocalStorageService : IStorageService
{
    private readonly string _rootPath;
    private readonly string _publicPath;

    public LocalStorageService(IConfiguration configuration)
    {
        _rootPath = configuration["Storage:RootPath"] ?? "App_Data/uploads";
        _publicPath = (configuration["Storage:PublicPath"] ?? "/uploads").TrimEnd('/');

        Directory.CreateDirectory(_rootPath);
    }

    public async Task<string> SaveAsync(Stream content, string fileName, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_rootPath, fileName);

        await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await content.CopyToAsync(fileStream, cancellationToken);

        return $"{_publicPath}/{fileName}";
    }
}
