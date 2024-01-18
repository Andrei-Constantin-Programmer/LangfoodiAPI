using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Contracts.Users;
using RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Net;
using System.Net.Http.Json;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.Endpoints;

public class UserEndpointsTests : EndpointTestBase
{
    public UserEndpointsTests(WebApplicationFactory<Program> factory) : base(factory) {}

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void UserCreate_WhenValidUser_ReturnUserWithId()
    {
        // Given
        NewUserContract contract = new("TestHandler", "TestUsername", "test@mail.com", "Test@123");

        // When
        var result = await _client.PostAsJsonAsync("user/create", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = (await result.Content.ReadFromJsonAsync<UserDTO>())!;

        data.Id.Should().NotBeNull();
        data.UserName.Should().Be(contract.UserName);
        data.Email.Should().Be(contract.Email);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    [InlineData("", "test@mail.com", "Test@123")]
    [InlineData("TestUsername", "test.com", "Test@123")]
    [InlineData("TestUsername", "test@mail.com", "test")]
    public async void UserCreate_WhenInvalidUser_ReturnBadRequest(string username, string email, string password)
    {
        // Given
        NewUserContract contract = new("handler", username, email, password);

        // When
        var result = await _client.PostAsJsonAsync("user/create", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void UserUsernameExists_WhenUsernameExists_ReturnTrue()
    {
        // Given
        NewUserContract contract = new("handler", "TestUsername", "test@mail.com", "Test@123");

        _ = _fakeUserRepository
            .CreateUser(contract.Handler, contract.UserName, contract.Email, _fakeCryptoService.Encrypt(contract.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // When
        var result = await _client.PostAsync($"user/username/exists?username={Uri.EscapeDataString(contract.UserName)}", null);
        
        // Then
        var resultContent = bool.Parse(await
            result.Content.ReadAsStringAsync());

        resultContent.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void UserUsernameExists_WhenUsernameDoesNotExist_ReturnFalse()
    {
        // Given
        NewUserContract contract = new("handler", "TestUsername", "test@mail.com", "Test@123");

        // When
        var result = await _client.PostAsync($"user/username/exists?username={Uri.EscapeDataString(contract.UserName)}", null);

        // Then
        var resultContent = bool.Parse(await
            result.Content.ReadAsStringAsync());

        resultContent.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void UserEmailExists_WhenEmailExists_ReturnTrue()
    {
        // Given
        NewUserContract contract = new("handler", "TestUsername", "test@mail.com", "Test@123");

        _ = _fakeUserRepository
            .CreateUser(contract.Handler, contract.UserName, contract.Email, _fakeCryptoService.Encrypt(contract.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // When
        var result = await _client.PostAsync($"user/email/exists?email={Uri.EscapeDataString(contract.Email)}", null);

        // Then
        var resultContent = bool.Parse(await
            result.Content.ReadAsStringAsync());

        resultContent.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void UserEmailExists_WhenEmailDoesNotExist_ReturnFalse()
    {
        // Given
        NewUserContract contract = new("handler", "TestUsername", "test@mail.com", "Test@123");

        // When
        var result = await _client.PostAsync($"user/email/exists?email={Uri.EscapeDataString(contract.Email)}", null);

        // Then
        var resultContent = bool.Parse(await
            result.Content.ReadAsStringAsync());

        resultContent.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void UserUpdate_WhenUserExists_UpdateUserAndReturnOk()
    {
        // Given
        NewUserContract createContract = new("handler", "TestUsername", "test@mail.com", "Test@123");

        var user = _fakeUserRepository
           .CreateUser(createContract.Handler, createContract.UserName, createContract.Email, _fakeCryptoService.Encrypt(createContract.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var oldUsername = user.Account.UserName;

        UpdateUserContract updateContract = new(user.Account.Id, "NewUsername", user.Email, user.Password);

        // When
        var result = await _client.PutAsJsonAsync("user/update", updateContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var oldUserExists = _fakeUserRepository.GetUserByUsername(oldUsername) is not null;
        var newUserExists = _fakeUserRepository.GetUserByUsername(updateContract.UserName) is not null;

        oldUserExists.Should().BeFalse();
        newUserExists.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void UserUpdate_WhenUserDoesNotExist_DoNotUpdateAndReturnNotFound()
    {
        // Given
        UpdateUserContract updateContract = new("TestId", "TestUsername", "test@mail.com", "Test@123");

        // When
        var result = await _client.PutAsJsonAsync("user/update", updateContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var userExists = _fakeUserRepository.GetUserByUsername(updateContract.UserName) is not null;
        userExists.Should().BeFalse();
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    [InlineData("", "test@mail.com", "Test@123")]
    [InlineData("TestUsername", "test.com", "Test@123")]
    [InlineData("TestUsername", "test@mail.com", "test")]
    public async void UserUpdate_WhenInvalidUser_ReturnBadRequest(string username, string email, string password)
    {
        // Given
        NewUserContract createContract = new("handler", "TestUsername", "test@mail.com", "Test@123");

        var user = _fakeUserRepository
            .CreateUser(createContract.Handler, createContract.UserName, createContract.Email, _fakeCryptoService.Encrypt(createContract.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        NewUserContract updateContract = new(user.Account.Id, username, email, password);

        // When
        var result = await _client.PutAsJsonAsync("user/update", updateContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void UserRemove_WhenUserEmailDoesExist_DeleteUserAndReturnOk()
    {
        // Given
        NewUserContract createContract = new("handler", "TestUsername", "test@mail.com", "Test@123");

        _ = _fakeUserRepository
           .CreateUser(createContract.Handler, createContract.UserName, createContract.Email, _fakeCryptoService.Encrypt(createContract.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // When
        var result = await _client.DeleteAsync($"user/remove?emailOrId={Uri.EscapeDataString(createContract.Email)}");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var userExists = _fakeUserRepository.GetUserByUsername(createContract.UserName) is not null;
        userExists.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void UserRemove_WhenUserIdDoesExist_DeleteUserAndReturnOk()
    {
        // Given
        NewUserContract createContract = new("handler", "TestUsername", "test@mail.com", "Test@123");

        var user = _fakeUserRepository
            .CreateUser(createContract.Handler, createContract.UserName, createContract.Email, _fakeCryptoService.Encrypt(createContract.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // When
        var result = await _client.DeleteAsync($"user/remove?emailOrId={Uri.EscapeDataString(user.Account.Id)}");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var userExists = _fakeUserRepository.GetUserByUsername(createContract.UserName) is not null;
        userExists.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void UserRemove_WhenUserEmailDoesNotExist_ReturnNotFound()
    {
        // Given
        var email = "test@mail.com";

        // When
        var result = await _client.DeleteAsync($"user/remove?emailOrId={Uri.EscapeDataString(email)}");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void UserRemove_WhenUserIdDoesNotExist_ReturnNotFound()
    {
        // Given
        var id = "1";

        // When
        var result = await _client.DeleteAsync($"user/remove?emailOrId={Uri.EscapeDataString(id)}");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
