using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.Tests.Shared.TestHelpers;
using System.Net;
using System.Net.Http.Json;

namespace RecipeSocialMediaAPI.Tests.Integration.Endpoints;

public class UserEndpointsTests : EndpointTestBase
{
    public UserEndpointsTests(WebApplicationFactory<Program> factory) : base(factory) {}

    [Fact]
    [Trait(Traits.DOMAIN, "User")]
    public async void UserCreate_WhenValidUser_ReturnUserWithId()
    {
        // Given
        var testUser = new UserDto
        {
            Id = null,
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };

        // When
        var result = await _client.PostAsJsonAsync("user/create", testUser);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = (await result.Content.ReadFromJsonAsync<UserDto>())!;

        data.Id.Should().NotBeNull();
        data.UserName.Should().Be(testUser.UserName);
        data.Email.Should().Be(testUser.Email);
    }
}
