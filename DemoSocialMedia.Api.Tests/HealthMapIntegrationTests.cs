using System.Net;
using FluentAssertions;

namespace DemoSocialMedia.Api.Tests;

public class HealthMapIntegrationTests : IClassFixture<TestAppFactory>
{
    private readonly HttpClient _client;

    public HealthMapIntegrationTests(TestAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_endpoint_returns_200()
    {
        var resp = await _client.GetAsync("/health");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Map_grid_returns_dimensions_and_cells()
    {
        var resp = await _client.GetAsync("/api/map/grid");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await resp.Content.ReadAsStringAsync();
        body.Should().Contain("width");
        body.Should().Contain("height");
        body.Should().Contain("cells");
    }
}

