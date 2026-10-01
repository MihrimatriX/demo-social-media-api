using System.Net;
using FluentAssertions;

namespace DemoSocialMedia.Api.Tests;

public class HealthSwaggerIntegrationTests : IClassFixture<TestAppFactory>
{
    private readonly HttpClient _client;

    public HealthSwaggerIntegrationTests(TestAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_endpoint_returns_200()
    {
        var resp = await _client.GetAsync("/health");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // Swagger üretim hataları sadece çalışma anında görünür; upload ucu (IFormFile) dokümana girmeli.
    [Fact]
    public async Task Swagger_document_generates_and_includes_file_upload()
    {
        var resp = await _client.GetAsync("/swagger/v1/swagger.json");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var doc = await resp.Content.ReadAsStringAsync();
        doc.Should().ContainEquivalentOf("/api/files/upload"); // route'lar büyük/küçük harf duyarsız
        doc.Should().Contain("multipart/form-data");
    }
}
