using DemoSocialMedia.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoSocialMedia.Api.Controllers;

[Authorize]
public class FilesController : BaseController
{
    private const long MaxBytes = 5 * 1024 * 1024;
    private static readonly HashSet<string> AllowedTypes = ["image/jpeg", "image/png", "image/gif", "image/webp"];

    private readonly IMinioService _minioService;
    public FilesController(IMinioService minioService) => _minioService = minioService;

    // Dönen objectName, POST /api/posts gövdesindeki imageUrl alanına verilir.
    // ponytail: Content-Type istemcinin beyanı; gerekirse magic-byte kontrolü ekle.
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file.Length == 0 || file.Length > MaxBytes)
            return BadRequest(new { message = "Dosya boş olamaz ve 5 MB'ı aşamaz." });
        if (!AllowedTypes.Contains(file.ContentType))
            return BadRequest(new { message = "Sadece JPEG, PNG, GIF veya WebP yüklenebilir." });

        var objectName = await _minioService.UploadFileAsync(file);
        return Ok(new { url = await _minioService.GetImageUrlAsync(objectName), objectName });
    }
}
