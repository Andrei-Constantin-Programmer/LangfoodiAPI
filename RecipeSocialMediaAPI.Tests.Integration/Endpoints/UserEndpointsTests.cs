using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Contracts;
using RecipeSocialMediaAPI.DTO;
using RecipeSocialMediaAPI.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.Tests.Shared.Traits;
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
        NewUserContract contract = new()
        {
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };

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
    [InlineData("", "test@mail.com", "Test@123")]
    [InlineData("TestUsername", "test.com", "Test@123")]
    [InlineData("TestUsername", "test@mail.com", "test")]
    [Trait(Traits.DOMAIN, "User")]
    public async void UserCreate_WhenInvalidUser_ReturnBadRequest(string username, string email, string password)
    {
        // Given
        NewUserContract contract = new()
        {
            UserName = username,
            Email = email,
            Password = password
        };

        // When
        var result = await _client.PostAsJsonAsync("user/create", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async void UserUsernameExists_WhenUsernameExists_ReturnTrue()
    {
        // Given
        NewUserContract contract = new()
        {
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };
        await (await _client
            .PostAsJsonAsync("user/create", contract))
            .Content.ReadFromJsonAsync<UserDTO>();

        // When
        var result = await _client.PostAsync($"user/username/exists?username={Uri.EscapeDataString(contract.UserName)}", null);
        
        // Then
        var resultContent = bool.Parse(await
            result.Content.ReadAsStringAsync());

        resultContent.Should().BeTrue();
    }

    [Fact]
    public async void UserUsernameExists_WhenUsernameDoesNotExist_ReturnFalse()
    {
        // Given
        NewUserContract contract = new()
        {
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };
        
        // When
        var result = await _client.PostAsync($"user/username/exists?username={Uri.EscapeDataString(contract.UserName)}", null);

        // Then
        var resultContent = bool.Parse(await
            result.Content.ReadAsStringAsync());

        resultContent.Should().BeFalse();
    }

    [Fact]
    public async void UserEmailExists_WhenEmailExists_ReturnTrue()
    {
        // Given
        NewUserContract contract = new()
        {
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };
        await (await _client
            .PostAsJsonAsync("user/create", contract))
            .Content.ReadFromJsonAsync<UserDTO>();

        // When
        var result = await _client.PostAsync($"user/email/exists?email={Uri.EscapeDataString(contract.Email)}", null);

        // Then
        var resultContent = bool.Parse(await
            result.Content.ReadAsStringAsync());

        resultContent.Should().BeTrue();
    }

    [Fact]
    public async void UserEmailExists_WhenEmailDoesNotExist_ReturnFalse()
    {
        // Given
        NewUserContract contract = new()
        {
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };

        // When
        var result = await _client.PostAsync($"user/email/exists?email={Uri.EscapeDataString(contract.Email)}", null);

        // Then
        var resultContent = bool.Parse(await
            result.Content.ReadAsStringAsync());

        resultContent.Should().BeFalse();
    }

    [Fact]
    public async void UserUpdate_WhenUserExists_UpdateUserAndReturnOk()
    {
        // Given
        NewUserContract createContract = new()
        {
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };
        var user = (await 
            (await _client
                .PostAsJsonAsync("user/create", createContract))
            .Content
            .ReadFromJsonAsync<UserDTO>())!;

        UpdateUserContract updateContract = new()
        {
            Id = user.Id,
            UserName = "NewUsername",
            Password = user.Password,
            Email = user.Email,
        };

        // When
        var result = await _client.PostAsJsonAsync("user/update", updateContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var oldUserExistsResult = await _client.PostAsync($"user/username/exists?username={Uri.EscapeDataString(user.UserName)}", null);
        var newUserExistsResult = await _client.PostAsync($"user/username/exists?username={Uri.EscapeDataString(updateContract.UserName)}", null);

        var oldUserExists = bool.Parse(await 
            oldUserExistsResult.Content.ReadAsStringAsync());

        var newUserExists = bool.Parse(
            await(
                newUserExistsResult)
            .Content.ReadAsStringAsync());

        oldUserExists.Should().BeFalse();
        newUserExists.Should().BeTrue();
    }

    [Fact]
    public async void UserUpdate_WhenUserDoesNotExist_DoNotUpdateAndReturnNotFound()
    {
        // Given
        UpdateUserContract updateContract = new()
        {
            Id = "TestId",
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };

        // When
        var result = await _client.PostAsJsonAsync("user/update", updateContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var userExistsResult = await _client
            .PostAsync($"user/username/exists?username={Uri.EscapeDataString(updateContract.UserName)}", null);

        var userExists = bool.Parse(await
            userExistsResult.Content
            .ReadAsStringAsync());

        userExists.Should().BeFalse();
    }

    [Theory]
    [InlineData("", "test@mail.com", "Test@123")]
    [InlineData("TestUsername", "test.com", "Test@123")]
    [InlineData("TestUsername", "test@mail.com", "test")]
    [Trait(Traits.DOMAIN, "User")]
    public async void UserUpdate_WhenInvalidUser_ReturnBadRequest(string username, string email, string password)
    {
        // Given
        NewUserContract createContract = new()
        {
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };
        var user = (await
            (await _client
                .PostAsJsonAsync("user/create", createContract))
            .Content
            .ReadFromJsonAsync<UserDTO>())!;

        UpdateUserContract updateContract = new()
        {
            Id = user.Id,
            UserName = username,
            Password = email,
            Email = password,
        };

        // When
        var result = await _client.PostAsJsonAsync("user/update", updateContract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async void UserRemove_WhenUserEmailDoesExist_DeleteUserAndReturnOk()
    {
        // Given
        NewUserContract createContract = new()
        {
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };
        await(await _client
            .PostAsJsonAsync("user/create", createContract))
            .Content.ReadFromJsonAsync<UserDTO>();

        // When
        var result = await _client.DeleteAsync($"user/remove?emailOrId={Uri.EscapeDataString(createContract.Email)}");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var userExistsResult = await _client.PostAsync($"user/username/exists?username={Uri.EscapeDataString(createContract.UserName)}", null);
        var userExists = bool.Parse(await
            userExistsResult.Content.ReadAsStringAsync());
        userExists.Should().BeFalse();
    }

    [Fact]
    public async void UserRemove_WhenUserIdDoesExist_DeleteUserAndReturnOk()
    {
        // Given
        NewUserContract createContract = new()
        {
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };
        var user = await (await _client
            .PostAsJsonAsync("user/create", createContract))
            .Content.ReadFromJsonAsync<UserDTO>();

        // When
        var result = await _client.DeleteAsync($"user/remove?emailOrId={Uri.EscapeDataString(user!.Id!)}");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var userExistsResult = await _client.PostAsync($"user/username/exists?username={Uri.EscapeDataString(createContract.UserName)}", null);
        var userExists = bool.Parse(await
            userExistsResult.Content.ReadAsStringAsync());
        userExists.Should().BeFalse();
    }

    [Fact]
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
