using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Data.DTO;
using RecipeSocialMediaAPI.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.Tests.Shared.TestHelpers;
using System.Net;
using System.Net.Http.Json;

namespace RecipeSocialMediaAPI.Tests.Integration.Endpoints;

public class AuthenticationEndpointsTests : EndpointTestBase
{
    public AuthenticationEndpointsTests(WebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    [Trait(Traits.DOMAIN, "Authentication")]
    public async void Authenticate_WhenValidUserWithUsername_ReturnUserFromDB()
    {
        // Given
        UserDto userToCreate = new()
        {
            Id = null,
            UserName = "testUser",
            Email = "test@mail.com",
            Password = "Test@123"
        };
        var userInDb = (await
            (await _client
                .PostAsJsonAsync("user/create", userToCreate))
            .Content
            .ReadFromJsonAsync<UserDto>())!;

        // When
        var result = await _client.PostAsJsonAsync("auth/authenticate", new AuthenticationAttemptDTO() 
        { 
            UsernameOrEmail = userToCreate.UserName, 
            Password = userToCreate.Password 
        });

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        UserDto authenticatedUser = (await result.Content.ReadFromJsonAsync<UserDto>())!;

        authenticatedUser.Id.Should().Be(userInDb.Id);
        authenticatedUser.UserName.Should().Be(userInDb.UserName);
        authenticatedUser.Email.Should().Be(userInDb.Email);
        authenticatedUser.Password.Should().Be(userInDb.Password);
    }

    [Fact]
    [Trait(Traits.DOMAIN, "Authentication")]
    public async void Authenticate_WhenValidUserWithEmail_ReturnUserFromDB()
    {
        // Given
        UserDto userToCreate = new()
        {
            Id = null,
            UserName = "testUser",
            Email = "test@mail.com",
            Password = "Test@123"
        };
        var userInDb = (await
            (await _client
                .PostAsJsonAsync("user/create", userToCreate))
            .Content
            .ReadFromJsonAsync<UserDto>())!;

        // When
        var result = await _client.PostAsJsonAsync("auth/authenticate", new AuthenticationAttemptDTO()
        {
            UsernameOrEmail = userToCreate.Email,
            Password = userToCreate.Password
        });

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        UserDto authenticatedUser = (await result.Content.ReadFromJsonAsync<UserDto>())!;

        authenticatedUser.Id.Should().Be(userInDb.Id);
        authenticatedUser.UserName.Should().Be(userInDb.UserName);
        authenticatedUser.Email.Should().Be(userInDb.Email);
        authenticatedUser.Password.Should().Be(userInDb.Password);
    }


}
