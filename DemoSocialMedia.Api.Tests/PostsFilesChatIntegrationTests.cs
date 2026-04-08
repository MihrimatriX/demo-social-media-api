using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using DemoSocialMedia.Application.Auth.Commands;
using DemoSocialMedia.Application.Posts.DTOs;
using FluentAssertions;

namespace DemoSocialMedia.Api.Tests;

public class PostsFilesChatIntegrationTests : IClassFixture<TestAppFactory>
{
    private readonly TestAppFactory _factory;

    public PostsFilesChatIntegrationTests(TestAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Posts_feed_is_public_and_create_comment_like_save_work()
    {
        var anon = _factory.CreateClient();

        // feed should work without auth
        (await anon.GetAsync("/api/posts")).StatusCode.Should().Be(HttpStatusCode.OK);

        // register a user and auth
        var regReq = TestJson.NewRegisterRequest("p1@test.local", "p1");
        var reg = await (await anon.PostAsJsonAsync("/api/auth/register", regReq)).ReadAsAsync<RegisterUserResult>();
        var authed = _factory.CreateClient().WithBearer(reg.UserId, regReq.Email, regReq.Nickname);

        // create post
        var createPostResp = await authed.PostAsJsonAsync("/api/posts", new CreatePostRequest { Content = "hello world", ImageUrl = null });
        createPostResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var post = await createPostResp.ReadAsAsync<PostDto>();
        post.Content.Should().Be("hello world");

        // detail (authed) should return same post
        var detail = await (await authed.GetAsync($"/api/posts/{post.Id}")).ReadAsAsync<PostDto>();
        detail.Id.Should().Be(post.Id);

        // comment
        var commentResp = await authed.PostAsJsonAsync($"/api/posts/{post.Id}/comments", new CreateCommentRequest { Content = "nice!" });
        commentResp.StatusCode.Should().Be(HttpStatusCode.OK);

        // like toggle true then false
        var like1 = await authed.PostAsync($"/api/posts/{post.Id}/like", null);
        like1.StatusCode.Should().Be(HttpStatusCode.OK);
        (await like1.Content.ReadAsStringAsync()).Should().Contain("liked");

        var like2 = await authed.PostAsync($"/api/posts/{post.Id}/like", null);
        like2.StatusCode.Should().Be(HttpStatusCode.OK);

        // save toggle true then false
        (await authed.PostAsync($"/api/posts/{post.Id}/save", null)).StatusCode.Should().Be(HttpStatusCode.OK);
        (await authed.PostAsync($"/api/posts/{post.Id}/save", null)).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Files_upload_returns_url_using_fake_minio()
    {
        var client = _factory.CreateClient();

        var regReq = TestJson.NewRegisterRequest("file1@test.local", "file1");
        var reg = await (await client.PostAsJsonAsync("/api/auth/register", regReq)).ReadAsAsync<RegisterUserResult>();
        client.WithBearer(reg.UserId, regReq.Email, regReq.Nickname);

        var content = new MultipartFormDataContent();
        var fileBytes = Encoding.UTF8.GetBytes("hello");
        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");
        content.Add(fileContent, "file", "hello.txt");

        var resp = await client.PostAsync("/api/files/upload", content);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await resp.Content.ReadAsStringAsync();
        body.Should().Contain("\"url\"");
        body.Should().Contain("minio.local");
    }

    [Fact]
    public async Task Chat_create_room_is_idempotent_and_messages_roundtrip()
    {
        var anon = _factory.CreateClient();

        var aReq = TestJson.NewRegisterRequest("c1@test.local", "c1");
        var bReq = TestJson.NewRegisterRequest("c2@test.local", "c2");
        var a = await (await anon.PostAsJsonAsync("/api/auth/register", aReq)).ReadAsAsync<RegisterUserResult>();
        var b = await (await anon.PostAsJsonAsync("/api/auth/register", bReq)).ReadAsAsync<RegisterUserResult>();

        var aClient = _factory.CreateClient().WithBearer(a.UserId, aReq.Email, aReq.Nickname);

        // note: controller binds CreateChatRoomCommand; it overwrites UserId from auth
        var roomBody = new
        {
            name = (string?)null,
            isGroupChat = false,
            memberIds = new[] { b.UserId },
            userId = Guid.Empty
        };

        var room1 = await (await aClient.PostAsJsonAsync("/api/chat/rooms", roomBody)).ReadAsAsync<Guid>();
        var room2 = await (await aClient.PostAsJsonAsync("/api/chat/rooms", roomBody)).ReadAsAsync<Guid>();
        room2.Should().Be(room1);

        (await aClient.PostAsJsonAsync($"/api/chat/rooms/{room1}/messages", new { content = "hi" })).StatusCode.Should().Be(HttpStatusCode.OK);

        var messagesResp = await aClient.GetAsync($"/api/chat/rooms/{room1}/messages");
        messagesResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var messagesBody = await messagesResp.Content.ReadAsStringAsync();
        messagesBody.Should().Contain("hi");
    }
}

