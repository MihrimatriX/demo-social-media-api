using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using DemoSocialMedia.Infrastructure.Persistence;
using DemoSocialMedia.Infrastructure.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;

namespace DemoSocialMedia.Api.Tests;

public sealed class TestAppFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:;Cache=Shared");

    protected override IHost CreateHost(IHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("Jwt__Key", TestAuth.JwtKey);
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", "");
        Environment.SetEnvironmentVariable("SKIP_DATABASE_MIGRATION", "true");
        _connection.Open();
        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development"); // swagger/minio dev paths etc.

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
            services.RemoveAll(typeof(AppDbContext));
            services.RemoveAll(typeof(IDbContextOptionsConfiguration<AppDbContext>));
            services.RemoveAll(typeof(IConfigureOptions<DbContextOptions<AppDbContext>>));

            var efSqliteProvider = new ServiceCollection()
                .AddEntityFrameworkSqlite()
                .BuildServiceProvider();

            services.AddDbContext<AppDbContext>(options =>
            {
                options
                    .UseSqlite(_connection)
                    .UseInternalServiceProvider(efSqliteProvider);
            });

            services.RemoveAll(typeof(IMinioService));
            services.AddSingleton<IMinioService, FakeMinioService>();

            using var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            Environment.SetEnvironmentVariable("SKIP_DATABASE_MIGRATION", null);
            _connection.Dispose();
        }
    }
}

public sealed class FakeMinioService : IMinioService
{
    public Task<string> UploadFileAsync(IFormFile file) => Task.FromResult($"fake-{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");
    public Task<string> GetImageUrlAsync(string fileName) => Task.FromResult($"http://minio.local/media/{fileName}");
}

public static class TestAuth
{
    public const string JwtKey = "test-jwt-key-which-is-long-enough-for-hmac-sha256-1234567890";

    public static HttpClient WithBearer(this HttpClient client, Guid userId, string email = "user@test.local", string nickname = "user")
    {
        var token = CreateJwt(userId, email, nickname);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public static string CreateJwt(Guid userId, string email, string nickname)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new("nickname", nickname),
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

