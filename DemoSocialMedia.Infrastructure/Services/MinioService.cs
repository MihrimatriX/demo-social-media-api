using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Minio;
using Minio.DataModel.Args;

namespace DemoSocialMedia.Infrastructure.Services;

// Singleton: MinioClient içinde HttpClient tutar, istek başına yeniden kurulmamalı.
public class MinioService : IMinioService
{
    private readonly IMinioClient _client;
    private readonly string _bucket;
    private readonly string _endpoint;
    private readonly string? _publicBaseUrl;
    private readonly bool _isDevelopment;

    public MinioService(IConfiguration config, IHostEnvironment env)
    {
        _endpoint = config["Minio:Endpoint"] ?? "localhost:9000";
        _bucket = config["Minio:Bucket"] ?? "media";
        _publicBaseUrl = config["Minio:PublicBaseUrl"];
        _isDevelopment = env.IsDevelopment();
        _client = new MinioClient()
            .WithEndpoint(_endpoint)
            .WithCredentials(
                config["Minio:AccessKey"] ?? throw new InvalidOperationException("'Minio:AccessKey' eksik."),
                config["Minio:SecretKey"] ?? throw new InvalidOperationException("'Minio:SecretKey' eksik."))
            .WithSSL(false)
            .Build();
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        if (!await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucket)))
        {
            await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucket));
            // Dev'de GetImageUrlAsync düz URL döndürüyor; bucket private kalırsa tarayıcı 403 alır.
            if (_isDevelopment)
                await _client.SetPolicyAsync(new SetPolicyArgs().WithBucket(_bucket).WithPolicy(PublicReadPolicy()));
        }

        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        using var stream = file.OpenReadStream();
        await _client.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_bucket)
            .WithObject(fileName)
            .WithStreamData(stream)
            .WithObjectSize(file.Length)
            .WithContentType(file.ContentType));
        return fileName;
    }

    public async Task<string> GetImageUrlAsync(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return string.Empty;
        if (_isDevelopment)
        {
            var baseUrl = string.IsNullOrWhiteSpace(_publicBaseUrl) ? $"http://{_endpoint}" : _publicBaseUrl.TrimEnd('/');
            return $"{baseUrl}/{_bucket}/{fileName}";
        }
        // Production: 1 saatlik imzalı URL (imza yerelde hesaplanır, MinIO'ya istek atılmaz).
        return await _client.PresignedGetObjectAsync(new PresignedGetObjectArgs()
            .WithBucket(_bucket)
            .WithObject(fileName)
            .WithExpiry(3600));
    }

    private string PublicReadPolicy() =>
        $$"""{"Version":"2012-10-17","Statement":[{"Effect":"Allow","Principal":{"AWS":["*"]},"Action":["s3:GetObject"],"Resource":["arn:aws:s3:::{{_bucket}}/*"]}]}""";
}
