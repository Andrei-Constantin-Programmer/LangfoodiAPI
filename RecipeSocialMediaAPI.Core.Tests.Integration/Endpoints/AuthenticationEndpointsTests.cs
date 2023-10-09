using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Contracts.Authentication;
using RecipeSocialMediaAPI.Application.Contracts.Users;
using RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Net;
using System.Net.Http.Json;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.Endpoints;

public class AuthenticationEndpointsTests : EndpointTestBase
{
    public AuthenticationEndpointsTests(WebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.AUTHENTICATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void Authenticate_WhenValidUserWithUsername_ReturnUserFromDB()
    {
        // Given
        NewUserContract userToCreate = new()
        {
            Handler = "testHandler",
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
    [Trait(Traits.DOMAIN, Traits.Domains.AUTHENTICATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void Authenticate_WhenValidUserWithEmail_ReturnUserFromDB()
    {
        // Given
        NewUserContract userToCreate = new()
        {
            Handler = "testHandler",
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
    [Trait(Traits.DOMAIN, Traits.Domains.AUTHENTICATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void Authenticate_WhenUserDoesNotExist_ReturnNotFound()
    {
        // Given
        NewUserContract userToCreate = new()
        {
            Handler = "testHandler",
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
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.AUTHENTICATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void Authenticate_WhenPasswordDoesNotMatch_ReturnBadRequest()
    {
        // Given
        NewUserContract userToCreate = new()
        {
            Handler = "testHandler",
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
