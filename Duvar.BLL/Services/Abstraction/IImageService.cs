namespace Duvar.BLL.Services.Abstraction;
using Microsoft.AspNetCore.Http;
public interface IImageService
{
    Task<string> SaveImageAsync(IFormFile file, string folder);
    void DeleteImage(string relativePath);
}
