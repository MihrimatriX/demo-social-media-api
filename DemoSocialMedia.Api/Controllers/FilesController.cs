using DemoSocialMedia.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoSocialMedia.Api.Controllers;

[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase
{
    private readonly MinioService _minioService;
    public FilesController(MinioService minioService)
    {
        _minioService = minioService;
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPost("upload")]
    [Authorize]
    public async Task<IActionResult> Upload([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Dosya seçilmedi.");
        var objectName = await _minioService.UploadFileAsync(file);
        // Geliştirme ortamında public URL; prod'da presigned URL
        var publicUrl = await _minioService.GetImageUrlAsync(objectName);
        return Ok(new { url = publicUrl, objectName });
    }
} 