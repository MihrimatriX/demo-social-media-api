using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace DemoSocialMedia.Api.Tests;

public class AuthIntegrationTests : IClassFixture<TestAppFactory>
{
    private readonly TestAppFactory _factory;

    public AuthIntegrationTests(TestAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_then_login_sets_cookie_and_me_works_via_cookie_bridge()
    {
        var client = _factory.CreateClient();

        var register = TestJson.NewRegisterRequest("a1@test.local", "a1");
        var registerResp = await client.PostAsJsonAsync("/api/auth/register", register);
        registerResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginResp = await client.PostAsJsonAsync("/api/auth/login", new { email = register.Email, password = register.Password });
        loginResp.StatusCode.Should().Be(HttpStatusCode.OK);
        loginResp.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        cookies!.Any(c => c.StartsWith("token=", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();

        // Cookie should be sent automatically by HttpClient handler; middleware bridges it into Authorization header
        var meResp = await client.GetAsync("/api/auth/me");
        meResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var meBody = await meResp.Content.ReadAsStringAsync();
        meBody.Should().Contain("userId");
        meBody.Should().Contain("email");
        meBody.Should().Contain("nickname");
    }
}

