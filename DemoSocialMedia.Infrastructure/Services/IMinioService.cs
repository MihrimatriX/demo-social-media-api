using Microsoft.AspNetCore.Http;

namespace DemoSocialMedia.Infrastructure.Services;

public interface IMinioService
{
    Task<string> UploadFileAsync(IFormFile file);
    Task<string> GetImageUrlAsync(string fileName);
}