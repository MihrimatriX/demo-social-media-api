using System.Net;
using System.Net.Http.Json;
using DemoSocialMedia.Application.Auth.Commands;
using DemoSocialMedia.Application.Auth.DTOs;
using FluentAssertions;

namespace DemoSocialMedia.Api.Tests;

public class UsersFriendsIntegrationTests : IClassFixture<TestAppFactory>
{
    private readonly TestAppFactory _factory;

    public UsersFriendsIntegrationTests(TestAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Users_search_excludes_self_and_friends()
    {
        var client = _factory.CreateClient();

        var u1 = TestJson.NewRegisterRequest("u1@test.local", "u1");
        var u2 = TestJson.NewRegisterRequest("u2@test.local", "u2");
        var u3 = TestJson.NewRegisterRequest("u3@test.local", "u3");

        var r1 = await (await client.PostAsJsonAsync("/api/auth/register", u1)).ReadAsAsync<RegisterUserResult>();
        var r2 = await (await client.PostAsJsonAsync("/api/auth/register", u2)).ReadAsAsync<RegisterUserResult>();
        var r3 = await (await client.PostAsJsonAsync("/api/auth/register", u3)).ReadAsAsync<RegisterUserResult>();

        // make u2 friend of u1 so it should be excluded from search results
        var u1Client = _factory.CreateClient().WithBearer(r1.UserId, u1.Email, u1.Nickname);
        var u2Client = _factory.CreateClient().WithBearer(r2.UserId, u2.Email, u2.Nickname);

        (await u1Client.PostAsJsonAsync("/api/friends/requests", new SendFriendRequestRequest { ReceiverId = r2.UserId }))
            .StatusCode.Should().Be(HttpStatusCode.OK);

        var incoming = await (await u2Client.GetAsync("/api/friends/requests?incoming=true")).Content.ReadAsStringAsync();
        incoming.Should().Contain(r1.UserId.ToString());

        // accept from u2
        var reqList = await u2Client.GetFromJsonAsync<List<FriendRequestDto>>("/api/friends/requests?incoming=true");
        reqList.Should().NotBeNull();
        reqList!.Count.Should().BeGreaterThan(0);
        var requestId = reqList[0].Id;

        (await u2Client.PutAsync($"/api/friends/requests/{requestId}/accept", null)).StatusCode.Should().Be(HttpStatusCode.OK);

        // now u1 searches "u" should find u3 but not u1 (self) and not u2 (friend)
        var searchResp = await u1Client.GetAsync("/api/users/search?query=u");
        searchResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var searchBody = await searchResp.Content.ReadAsStringAsync();
        searchBody.Should().Contain(r3.UserId.ToString());
        searchBody.Should().NotContain(r1.UserId.ToString());
        searchBody.Should().NotContain(r2.UserId.ToString());
    }

    [Fact]
    public async Task Friend_request_flow_send_list_accept_and_friends_list()
    {
        var client = _factory.CreateClient();

        var u1 = TestJson.NewRegisterRequest("f1@test.local", "f1");
        var u2 = TestJson.NewRegisterRequest("f2@test.local", "f2");

        var r1 = await (await client.PostAsJsonAsync("/api/auth/register", u1)).ReadAsAsync<RegisterUserResult>();
        var r2 = await (await client.PostAsJsonAsync("/api/auth/register", u2)).ReadAsAsync<RegisterUserResult>();

        var u1Client = _factory.CreateClient().WithBearer(r1.UserId, u1.Email, u1.Nickname);
        var u2Client = _factory.CreateClient().WithBearer(r2.UserId, u2.Email, u2.Nickname);

        (await u1Client.PostAsJsonAsync("/api/friends/requests", new SendFriendRequestRequest { ReceiverId = r2.UserId }))
            .StatusCode.Should().Be(HttpStatusCode.OK);

        var outgoing = await u1Client.GetAsync("/api/friends/requests?incoming=false");
        outgoing.StatusCode.Should().Be(HttpStatusCode.OK);

        var incomingList = await u2Client.GetFromJsonAsync<List<FriendRequestDto>>("/api/friends/requests?incoming=true");
        incomingList.Should().NotBeNull();
        incomingList!.Count.Should().Be(1);

        (await u2Client.PutAsync($"/api/friends/requests/{incomingList[0].Id}/accept", null))
            .StatusCode.Should().Be(HttpStatusCode.OK);

        var friends1 = await u1Client.GetAsync("/api/friends");
        friends1.StatusCode.Should().Be(HttpStatusCode.OK);
        (await friends1.Content.ReadAsStringAsync()).Should().Contain(r2.UserId.ToString());

        var friends2 = await u2Client.GetAsync("/api/friends");
        friends2.StatusCode.Should().Be(HttpStatusCode.OK);
        (await friends2.Content.ReadAsStringAsync()).Should().Contain(r1.UserId.ToString());
    }

    private sealed class FriendRequestDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Status { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}

