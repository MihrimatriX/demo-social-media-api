using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Minio;
using Minio.DataModel.Args;

namespace DemoSocialMedia.Infrastructure.Services;

public class MinioService
{
    private readonly IMinioClient _client;
    private readonly string _bucket;
    private readonly string _endpoint;
    private readonly bool _isDevelopment;

    public MinioService(IConfiguration config, IHostEnvironment env)
    {
        _endpoint = config["Minio:Endpoint"] ?? "localhost:9000";
        var accessKey = config["Minio:AccessKey"] ?? "minioadmin";
        var secretKey = config["Minio:SecretKey"] ?? "minioadmin123";
        _bucket = config["Minio:Bucket"] ?? "media";
        _isDevelopment = env.IsDevelopment();
        _client = new MinioClient()
            .WithEndpoint(_endpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(false)
            .Build();
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        Console.WriteLine($"[MinioService] Upload başlıyor: {file.FileName}");
        var exists = await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucket));
        if (!exists)
        {
            Console.WriteLine($"[MinioService] Bucket yok, oluşturuluyor: {_bucket}");
            await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucket));
        }
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        using var stream = file.OpenReadStream();
        await _client.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_bucket)
            .WithObject(fileName)
            .WithStreamData(stream)
            .WithObjectSize(file.Length)
            .WithContentType(file.ContentType));
        Console.WriteLine($"[MinioService] Upload tamamlandı: {fileName}");
        return fileName;
    }

    public async Task<string> GetImageUrlAsync(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            Console.WriteLine("[MinioService] GetImageUrlAsync: fileName boş!");
            return string.Empty;
        }
        if (_isDevelopment)
        {
            var url = $"http://{_endpoint}/{_bucket}/{fileName}";
            Console.WriteLine($"[MinioService] Public URL döndü: {url}");
            return url;
        }
        // Production: imzalı URL
        var args = new PresignedGetObjectArgs()
            .WithBucket(_bucket)
            .WithObject(fileName)
            .WithExpiry(3600); // 1 saat
        var presignedUrl = await _client.PresignedGetObjectAsync(args);
        Console.WriteLine($"[MinioService] Presigned URL döndü: {presignedUrl}");
        return presignedUrl;
    }
}