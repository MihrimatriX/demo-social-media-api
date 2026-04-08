using System.Net.Http.Json;
using DemoSocialMedia.Application.Auth.DTOs;
using FluentAssertions;

namespace DemoSocialMedia.Api.Tests;

public static class TestJson
{
    public static async Task<T> ReadAsAsync<T>(this HttpResponseMessage response)
    {
        response.IsSuccessStatusCode.Should().BeTrue(await response.Content.ReadAsStringAsync());
        var result = await response.Content.ReadFromJsonAsync<T>();
        result.Should().NotBeNull();
        return result!;
    }

    public static RegisterUserRequest NewRegisterRequest(string email, string nickname, string password = "P@ssw0rd-123")
        => new()
        {
            FirstName = "Test",
            LastName = "User",
            Email = email,
            Password = password,
            DateOfBirth = new DateTime(1990, 1, 1),
            NewsletterOptIn = false,
            IsAgreedKvkk = true,
            IsAgreedConsent = true,
            Country = "TR",
            Region = "34",
            ProfilePictureUrl = null,
            Gender = null,
            ReferenceCode = null,
            Nickname = nickname
        };
}

