using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Contracts;
using RecipeSocialMediaAPI.DTO;
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
        NewUserContract userToCreate = new()
        {
            UserName = "testUser",
            Email = "test@mail.com",
            Password = "Test@123"
        };
        var userInDb = (await
            (await _client
                .PostAsJsonAsync("user/create", userToCreate))
            .Content
            .ReadFromJsonAsync<UserDTO>())!;

        // When
        var result = await _client.PostAsJsonAsync("auth/authenticate", new AuthenticationAttemptContract() 
        { 
            UsernameOrEmail = userToCreate.UserName, 
            Password = userToCreate.Password 
        });

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        UserDTO authenticatedUser = (await result.Content.ReadFromJsonAsync<UserDTO>())!;

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
        NewUserContract userToCreate = new()
        {
            UserName = "testUser",
            Email = "test@mail.com",
            Password = "Test@123"
        };
        var userInDb = (await
            (await _client
                .PostAsJsonAsync("user/create", userToCreate))
            .Content
            .ReadFromJsonAsync<UserDTO>())!;

        // When
        var result = await _client.PostAsJsonAsync("auth/authenticate", new AuthenticationAttemptContract()
        {
            UsernameOrEmail = userToCreate.Email,
            Password = userToCreate.Password
        });

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        UserDTO authenticatedUser = (await result.Content.ReadFromJsonAsync<UserDTO>())!;

        authenticatedUser.Id.Should().Be(userInDb.Id);
        authenticatedUser.UserName.Should().Be(userInDb.UserName);
        authenticatedUser.Email.Should().Be(userInDb.Email);
        authenticatedUser.Password.Should().Be(userInDb.Password);
    }

    [Fact]
    [Trait(Traits.DOMAIN, "Authentication")]
    public async void Authenticate_WhenUserDoesNotExist_ReturnBadRequest()
    {
        // Given
        NewUserContract userToCreate = new()
        {
            UserName = "testUser",
            Email = "test@mail.com",
            Password = "Test@123"
        };
        await
            (await _client
                .PostAsJsonAsync("user/create", userToCreate))
            .Content
            .ReadFromJsonAsync<UserDTO>();

        // When
        var result = await _client.PostAsJsonAsync("auth/authenticate", new AuthenticationAttemptContract()
        {
            UsernameOrEmail = "Inexistant user",
            Password = userToCreate.Password
        });

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [Trait(Traits.DOMAIN, "Authentication")]
    public async void Authenticate_WhenPasswordDoesNotMatch_ReturnBadRequest()
    {
        // Given
        NewUserContract userToCreate = new()
        {
            UserName = "testUser",
            Email = "test@mail.com",
            Password = "Test@123"
        };
        await
            (await _client
                .PostAsJsonAsync("user/create", userToCreate))
            .Content
            .ReadFromJsonAsync<UserDTO>();

        // When
        var result = await _client.PostAsJsonAsync("auth/authenticate", new AuthenticationAttemptContract()
        {
            UsernameOrEmail = userToCreate.UserName,
            Password = ""
        });

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
