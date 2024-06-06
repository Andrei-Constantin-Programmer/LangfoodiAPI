using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Application.Contracts.Users;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Presentation.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.Intrinsics.X86;

namespace RecipeSocialMediaAPI.Presentation.Tests.Integration.Endpoints;

public class UserEndpointsTests : EndpointTestBase
{
    public UserEndpointsTests(WebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task UserCreate_WhenValidUser_ReturnUserWithId()
    {
        // Given
        NewUserContract contract = new("test_handle", "TestUsername", "test@mail.com", "Test@123");

        // When
        var result = await _client.PostAsJsonAsync("user/create", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = (await result.Content.ReadFromJsonAsync<SuccessfulAuthenticationDto>())!;

        data.User.Id.Should().NotBeNull();
        data.User.UserName.Should().Be(contract.UserName);
        data.User.Email.Should().Be(contract.Email);
        data.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    [InlineData("", "test@mail.com", "Test@123")]
    [InlineData("TestUsername", "test.com", "Test@123")]
    [InlineData("TestUsername", "test@mail.com", "test")]
    public async Task UserCreate_WhenInvalidUser_ReturnBadRequest(string username, string email, string password)
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
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task UserUsernameExists_WhenUsernameExists_ReturnTrue()
    {
        // Given
        NewUserContract contract = new("handler", "TestUsername", "test@mail.com", "Test@123");

        _ = await _fakeUserRepository
            .CreateUserAsync(contract.Handler, contract.UserName, contract.Email, _fakePasswordCryptoService.Encrypt(contract.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // When
        var result = await _client.PostAsync($"user/username/exists?username={Uri.EscapeDataString(contract.UserName)}", null);
        
        // Then
        var resultContent = bool.Parse(await
            result.Content.ReadAsStringAsync());

        resultContent.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task UserUsernameExists_WhenUsernameDoesNotExist_ReturnFalse()
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
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task UserEmailExists_WhenEmailExists_ReturnTrue()
    {
        // Given
        NewUserContract contract = new("handler", "TestUsername", "test@mail.com", "Test@123");

        _ = await _fakeUserRepository
            .CreateUserAsync(contract.Handler, contract.UserName, contract.Email, _fakePasswordCryptoService.Encrypt(contract.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // When
        var result = await _client.PostAsync($"user/email/exists?email={Uri.EscapeDataString(contract.Email)}", null);

        // Then
        var resultContent = bool.Parse(await
            result.Content.ReadAsStringAsync());

        resultContent.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task UserEmailExists_WhenEmailDoesNotExist_ReturnFalse()
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
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task UserUpdate_WhenUserExists_UpdateUserAndReturnOk()
    {
        // Given
        var user = await _fakeUserRepository
           .CreateUserAsync("handle", "user_1", "u1@mail.com", _fakePasswordCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var oldUsername = user.Account.UserName;

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        UpdateUserContract updateContract = new(user.Account.Id, "TestImageId", "NewUsername", user.Email, user.Password);

        // When
        var result = await _client.PutAsJsonAsync("user/update", updateContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var oldUserExists = await _fakeUserRepository.GetUserByUsernameAsync(oldUsername) is not null;
        var newUserExists = await _fakeUserRepository.GetUserByUsernameAsync(updateContract.UserName!) is not null;

        oldUserExists.Should().BeFalse();
        newUserExists.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task UserUpdate_WhenUserDoesNotExist_DoNotUpdateAndReturnNotFound()
    {
        // Given
        TestUserCredentials user = new()
        {
            Account = new TestUserAccount
            {
                Id = "u1",
                Handler = "user_1",
                UserName = "User 1"
            },
            Email = "u1@mail.com",
            Password = "Pass@123"
        };

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        UpdateUserContract updateContract = new("TestId", "TestImageId", "TestUsername", "test@mail.com", "Test@123");

        // When
        var result = await _client.PutAsJsonAsync("user/update", updateContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var userExists = await _fakeUserRepository.GetUserByUsernameAsync(updateContract.UserName!) is not null;
        userExists.Should().BeFalse();
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    [InlineData("", "test@mail.com", "Test@123")]
    [InlineData("TestUsername", "test.com", "Test@123")]
    [InlineData("TestUsername", "test@mail.com", "test")]
    public async Task UserUpdate_WhenInvalidUser_ReturnBadRequest(string username, string email, string password)
    {
        // Given
        var user = await _fakeUserRepository
           .CreateUserAsync("handle", "user_1", "u1@mail.com", _fakePasswordCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        NewUserContract updateContract = new(user.Account.Id, username, email, password);

        // When
        var result = await _client.PutAsJsonAsync("user/update", updateContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task UserUpdate_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        var user = await _fakeUserRepository
           .CreateUserAsync("handle", "user_1", "u1@mail.com", _fakePasswordCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        UpdateUserContract updateContract = new(user.Account.Id, "TestImageId", "NewUsername", user.Email, user.Password);

        // When
        var result = await _client.PutAsJsonAsync("user/update", updateContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task UserRemove_WhenUserEmailDoesExist_DeleteUserAndReturnOk()
    {
        // Given
        var user = await _fakeUserRepository
           .CreateUserAsync("handle", "user_1", "u1@mail.com", _fakePasswordCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.DeleteAsync($"user/remove?emailOrId={Uri.EscapeDataString(user.Email)}");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var userExists = await _fakeUserRepository.GetUserByUsernameAsync(user.Account.UserName) is not null;
        userExists.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task UserRemove_WhenUserIdDoesExist_DeleteUserAndReturnOk()
    {
        // Given
        var user = await _fakeUserRepository
           .CreateUserAsync("handle", "user_1", "u1@mail.com", _fakePasswordCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.DeleteAsync($"user/remove?emailOrId={Uri.EscapeDataString(user.Account.Id)}");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var userExists = await _fakeUserRepository.GetUserByUsernameAsync(user.Account.UserName) is not null;
        userExists.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task UserRemove_WhenUserEmailDoesNotExist_ReturnNotFound()
    {
        // Given
        TestUserCredentials user = new()
        {
            Account = new TestUserAccount
            {
                Id = "u1",
                Handler = "user_1",
                UserName = "User 1"
            },
            Email = "u1@mail.com",
            Password = "Pass@123"
        };

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.DeleteAsync($"user/remove?emailOrId={Uri.EscapeDataString(user.Email)}");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task UserRemove_WhenUserIdDoesNotExist_ReturnNotFound()
    {
        // Given
        TestUserCredentials user = new()
        {
            Account = new TestUserAccount
            {
                Id = "u1",
                Handler = "user_1",
                UserName = "User 1"
            },
            Email = "u1@mail.com",
            Password = "Pass@123"
        };

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.DeleteAsync($"user/remove?emailOrId={Uri.EscapeDataString(user.Account.Id)}");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task UserRemove_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        var user = await _fakeUserRepository
           .CreateUserAsync("handle", "user_1", "u1@mail.com", _fakePasswordCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // When
        var result = await _client.DeleteAsync($"user/remove?emailOrId={Uri.EscapeDataString(user.Account.Id)}");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetAll_WhenThereAreNoUsers_ReturnEmptyList(bool containSelf)
    {
        // Given
        var user = await _fakeUserRepository
            .CreateUserAsync($"handle", "UserName 1", "email1@mail.com", _fakePasswordCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"user/get-all/?containedString=notContaining&userId={user.Account.Id}&containSelf={containSelf}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<UserAccountDto>>();
        data.Should().NotBeNull();
        data.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetAll_WhenThereAreUsersAndChoseSelf_ReturnUsersWithSelf()
    {
        // Given
        string containedString = "test";
        var user1 = await _fakeUserRepository
            .CreateUserAsync($"handle_{containedString}", "UserName 1", "email1@mail.com", _fakePasswordCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
            .CreateUserAsync("Handle 2", $"{containedString.ToUpper()} 2", "email2@mail.com", _fakePasswordCryptoService.Encrypt("Test@321"), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));
        var _ = await _fakeUserRepository
            .CreateUserAsync("not_found_handle", "Not Found User", "email3@mail.com", _fakePasswordCryptoService.Encrypt("Test@987"), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"user/get-all/?containedString={containedString}&userId={user1.Account.Id}&containSelf=true", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<UserAccountDto>>();
        data.Should().NotBeNull();
        data.Should().HaveCount(2);

        data![0].Id.Should().Be(user1.Account.Id);
        data[0].Handler.Should().Be(user1.Account.Handler);
        data[0].UserName.Should().Be(user1.Account.UserName);

        data[1].Id.Should().Be(user2.Account.Id);
        data[1].Handler.Should().Be(user2.Account.Handler);
        data[1].UserName.Should().Be(user2.Account.UserName);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetAll_WhenThereAreUsersAndChoseNonSelf_ReturnUsersWithoutQueryingUser()
    {
        // Given
        string containedString = "test";
        var user1 = await _fakeUserRepository
            .CreateUserAsync($"handle_{containedString}", "UserName 1", "email1@mail.com", _fakePasswordCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
            .CreateUserAsync("Handle 2", $"{containedString.ToUpper()} 2", "email2@mail.com", _fakePasswordCryptoService.Encrypt("Test@321"), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));
        _ = await _fakeUserRepository
            .CreateUserAsync("not_found_handle", "Not Found User", "email3@mail.com", _fakePasswordCryptoService.Encrypt("Test@987"), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"user/get-all/?containedString={containedString}&userId={user1.Account.Id}&containSelf=false", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<UserAccountDto>>();
        data.Should().NotBeNull();
        data.Should().HaveCount(1);

        data![0].Id.Should().Be(user2.Account.Id);
        data[0].Handler.Should().Be(user2.Account.Handler);
        data[0].UserName.Should().Be(user2.Account.UserName);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConnected_WhenThereAreNoUsers_ReturnEmptyList()
    {
        // Given
        var user = await _fakeUserRepository
            .CreateUserAsync($"handle", "UserName 1", "email1@mail.com", _fakePasswordCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"user/get-connected/?containedString=notContaining&userId={user.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<UserAccountDto>>();
        data.Should().NotBeNull();
        data.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConnected_WhenThereAreUsers_ReturnOnlyConnectedUsers()
    {
        // Given
        string containedString = "test";
        var user1 = await _fakeUserRepository
            .CreateUserAsync($"handle_{containedString}", "UserName 1", "email1@mail.com", _fakePasswordCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
            .CreateUserAsync("Handle 2", $"{containedString.ToUpper()} 2", "email2@mail.com", _fakePasswordCryptoService.Encrypt("Test@321"), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));
        _ = await _fakeUserRepository
            .CreateUserAsync($"{containedString}_handle", "User 3", "email3@mail.com", _fakePasswordCryptoService.Encrypt("Test@987"), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero));

        _ = await _fakeConnectionRepository
            .CreateConnectionAsync(user1.Account, user2.Account, ConnectionStatus.Connected);

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"user/get-connected/?containedString={containedString}&userId={user1.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<UserAccountDto>>();
        data.Should().NotBeNull();
        data.Should().HaveCount(1);

        data![0].Id.Should().Be(user2.Account.Id);
        data[0].Handler.Should().Be(user2.Account.Handler);
        data[0].UserName.Should().Be(user2.Account.UserName);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConnected_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        string containedString = "test";
        var user1 = await _fakeUserRepository
            .CreateUserAsync($"handle_{containedString}", "UserName 1", "email1@mail.com", _fakePasswordCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
            .CreateUserAsync("Handle 2", $"{containedString.ToUpper()} 2", "email2@mail.com", _fakePasswordCryptoService.Encrypt("Test@321"), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));
        _ = await _fakeUserRepository
            .CreateUserAsync($"{containedString}_handle", "User 3", "email3@mail.com", _fakePasswordCryptoService.Encrypt("Test@987"), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero));

        _ = await _fakeConnectionRepository
            .CreateConnectionAsync(user1.Account, user2.Account, ConnectionStatus.Connected);

        // When
        var result = await _client.PostAsync($"user/get-connected/?containedString={containedString}&userId={user1.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetUnconnected_WhenThereAreNoUsers_ReturnEmptyList()
    {
        // Given
        var user = await _fakeUserRepository
            .CreateUserAsync($"handle", "UserName 1", "email1@mail.com", _fakePasswordCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"user/get-unconnected/?containedString=notContaining&userId={user.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<UserAccountDto>>();
        data.Should().NotBeNull();
        data.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetUnconnected_WhenThereAreUsers_ReturnOnlyUnconnectedUsers()
    {
        // Given
        string containedString = "test";
        var user1 = await _fakeUserRepository
            .CreateUserAsync($"handle_{containedString}", "UserName 1", "email1@mail.com", _fakePasswordCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
            .CreateUserAsync("Handle 2", $"{containedString.ToUpper()} 2", "email2@mail.com", _fakePasswordCryptoService.Encrypt("Test@321"), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));
        var user3 = await _fakeUserRepository
            .CreateUserAsync($"{containedString}_handle", "User 3", "email3@mail.com", _fakePasswordCryptoService.Encrypt("Test@987"), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero));

        _ = await _fakeConnectionRepository
            .CreateConnectionAsync(user1.Account, user2.Account, ConnectionStatus.Connected);

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"user/get-unconnected/?containedString={containedString}&userId={user1.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<UserAccountDto>>();
        data.Should().NotBeNull();
        data.Should().HaveCount(1);

        data![0].Id.Should().Be(user3.Account.Id);
        data[0].Handler.Should().Be(user3.Account.Handler);
        data[0].UserName.Should().Be(user3.Account.UserName);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetUnconnected_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        string containedString = "test";
        var user1 = await _fakeUserRepository
            .CreateUserAsync($"handle_{containedString}", "UserName 1", "email1@mail.com", _fakePasswordCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
            .CreateUserAsync("Handle 2", $"{containedString.ToUpper()} 2", "email2@mail.com", _fakePasswordCryptoService.Encrypt("Test@321"), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));
        _ = await _fakeUserRepository
            .CreateUserAsync($"{containedString}_handle", "User 3", "email3@mail.com", _fakePasswordCryptoService.Encrypt("Test@987"), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero));

        _ = await _fakeConnectionRepository
            .CreateConnectionAsync(user1.Account, user2.Account, ConnectionStatus.Connected);

        // When
        var result = await _client.PostAsync($"user/get-unconnected/?containedString={containedString}&userId={user1.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task ChangeRole_WhenUserDoesExist_ChangeRole()
    {
        // Given
        var admin = await _fakeUserRepository
            .CreateUserAsync($"handle", "UserName 1", "email1@mail.com", _fakePasswordCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), UserRole.Admin);

        var user = await _fakeUserRepository
            .CreateUserAsync($"handle_2", "UserName 2", "email2@mail.com", _fakePasswordCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), UserRole.User);

        var token = _bearerTokenGeneratorService.GenerateToken(admin);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PutAsync($"user/change-role?userId={user.Account.Id}&newRole=Developer", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        (await _fakeUserRepository.GetUserByIdAsync(user.Account.Id))?.Account.Role.Should().Be(UserRole.Developer);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task ChangeRole_WhenRoleIsInvalid_ReturnsBadRequest()
    {
        // Given
        var admin = await _fakeUserRepository
            .CreateUserAsync($"handle", "UserName 1", "email1@mail.com", _fakePasswordCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), UserRole.Admin);

        var user = await _fakeUserRepository
            .CreateUserAsync($"handle_2", "UserName 2", "email2@mail.com", _fakePasswordCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), UserRole.User);

        var token = _bearerTokenGeneratorService.GenerateToken(admin);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PutAsync($"user/change-role?userId={user.Account.Id}&newRole=BadRole", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task ChangeRole_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Given
        var admin = await _fakeUserRepository
            .CreateUserAsync($"handle", "UserName 1", "email1@mail.com", _fakePasswordCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), UserRole.Admin);

        var token = _bearerTokenGeneratorService.GenerateToken(admin);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PutAsync($"user/change-role?userId=u1&newRole=role", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    [InlineData(UserRole.Developer)]
    [InlineData(UserRole.User)]
    public async Task ChangeRole_WhenNonAdminTokenIsUsed_ReturnsForbidden(UserRole role)
    {
        // Given
        var user = await _fakeUserRepository
            .CreateUserAsync($"handle", "UserName 1", "email1@mail.com", _fakePasswordCryptoService.Encrypt("Test@123"), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PutAsync($"user/change-role?userId=1&newRole={role}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task ChangeRole_WhenNoTokenIsUsed_ReturnsUnauthorised()
    {
        // Given

        // When
        var result = await _client.PutAsync($"user/change-role?userId=1&newRole=role", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
