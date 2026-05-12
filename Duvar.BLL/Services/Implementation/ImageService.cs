using Duvar.BLL.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting; 
namespace Duvar.BLL.Services.Implementation;

public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _env;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

    public ImageService(IWebHostEnvironment env) => _env = env;

    public async Task<string> SaveImageAsync(IFormFile file, string folder)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file provided.");

        if (file.Length > MaxFileSizeBytes)
            throw new ArgumentException("File size exceeds 10 MB limit.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(ext))
            throw new ArgumentException("Unsupported file type. Use JPG, PNG, WebP, or GIF.");

        var uploadsFolder = Path.Combine(_env.WebRootPath, "images", folder);
        Directory.CreateDirectory(uploadsFolder);

        var uniqueName = $"{Guid.NewGuid():N}{ext}";
        var filePath = Path.Combine(uploadsFolder, uniqueName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/images/{folder}/{uniqueName}";
    }

    public void DeleteImage(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return;
        var fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }
}
