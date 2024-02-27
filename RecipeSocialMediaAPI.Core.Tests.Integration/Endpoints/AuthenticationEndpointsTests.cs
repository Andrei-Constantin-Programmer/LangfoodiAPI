using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Application.Contracts.Authentication;
using RecipeSocialMediaAPI.Application.Contracts.Users;
using RecipeSocialMediaAPI.Application.DTO.Users;
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
    public async void Authenticate_WhenValidUserWithEmail_ReturnUserFromDB()
    {
        // Given
        NewUserContract userToCreate = new("testHandler", "testUser", "test@mail.com", "Test@123");
        var userInDb = _fakeUserRepository
            .CreateUser(userToCreate.Handler, userToCreate.UserName, userToCreate.Email, _fakeCryptoService.Encrypt(userToCreate.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // When
        var result = await _client.PostAsJsonAsync("auth/authenticate", new AuthenticationAttemptContract(userToCreate.Email, userToCreate.Password));

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var authResult = (await result.Content.ReadFromJsonAsync<SuccessfulAuthenticationDTO>())!;

        var authenticatedUser = authResult.User;

        authenticatedUser.Id.Should().Be(userInDb.Account.Id);
        authenticatedUser.UserName.Should().Be(userInDb.Account.UserName);
        authenticatedUser.Email.Should().Be(userInDb.Email);
        authenticatedUser.Password.Should().Be(userInDb.Password);

        authResult.Token.Should().NotBeNullOrWhiteSpace();
    }
    
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.AUTHENTICATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void Authenticate_WhenUserDoesNotExist_ReturnNotFound()
    {
        // Given
        NewUserContract userToCreate = new("testHandler", "testUser", "test@mail.com", "Test@123");
        _ = _fakeUserRepository
            .CreateUser(userToCreate.Handler, userToCreate.UserName, userToCreate.Email, _fakeCryptoService.Encrypt(userToCreate.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // When
        var result = await _client.PostAsJsonAsync("auth/authenticate", new AuthenticationAttemptContract("Inexistant user", userToCreate.Password));

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.AUTHENTICATION)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void Authenticate_WhenPasswordDoesNotMatch_ReturnBadRequest()
    {
        // Given
        NewUserContract userToCreate = new("testHandler", "testUser", "test@mail.com", "Test@123");
        _ = _fakeUserRepository
            .CreateUser(userToCreate.Handler, userToCreate.UserName, userToCreate.Email, _fakeCryptoService.Encrypt(userToCreate.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // When
        var result = await _client.PostAsJsonAsync("auth/authenticate", new AuthenticationAttemptContract(userToCreate.Email, string.Empty));

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
