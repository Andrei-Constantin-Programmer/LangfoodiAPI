using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
        var testUser = new NewUserDTO
        {
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };

        // When
        var result = await _client.PostAsJsonAsync("user/create", testUser);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = (await result.Content.ReadFromJsonAsync<UserDTO>())!;

        data.Id.Should().NotBeNull();
        data.UserName.Should().Be(testUser.UserName);
        data.Email.Should().Be(testUser.Email);
    }

    [Theory]
    [InlineData("", "test@mail.com", "Test@123")]
    [InlineData("TestUsername", "test.com", "Test@123")]
    [InlineData("TestUsername", "test@mail.com", "test")]
    [Trait(Traits.DOMAIN, "User")]
    public async void UserCreate_WhenInvalidUser_ReturnBadRequest(string username, string email, string password)
    {
        // Given
        NewUserDTO testUser = new()
        {
            UserName = username,
            Email = email,
            Password = password
        };

        // When
        var result = await _client.PostAsJsonAsync("user/create", testUser);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async void UserUsernameExists_WhenUsernameExists_ReturnTrue()
    {
        // Given
        NewUserDTO testUser = new()
        {
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };
        await (await _client
            .PostAsJsonAsync("user/create", testUser))
            .Content.ReadFromJsonAsync<UserDTO>();

        // When
        var result = await _client.PostAsync($"user/username/exists?username={Uri.EscapeDataString(testUser.UserName)}", null);
        
        // Then
        var resultContent = bool.Parse(await
            result.Content.ReadAsStringAsync());

        resultContent.Should().BeTrue();
    }

    [Fact]
    public async void UserUsernameExists_WhenUsernameDoesNotExist_ReturnFalse()
    {
        // Given
        NewUserDTO testUser = new()
        {
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };
        
        // When
        var result = await _client.PostAsync($"user/username/exists?username={Uri.EscapeDataString(testUser.UserName)}", null);

        // Then
        var resultContent = bool.Parse(await
            result.Content.ReadAsStringAsync());

        resultContent.Should().BeFalse();
    }

    [Fact]
    public async void UserEmailExists_WhenEmailExists_ReturnTrue()
    {
        // Given
        NewUserDTO testUser = new()
        {
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };
        await (await _client
            .PostAsJsonAsync("user/create", testUser))
            .Content.ReadFromJsonAsync<UserDTO>();

        // When
        var result = await _client.PostAsync($"user/email/exists?email={Uri.EscapeDataString(testUser.Email)}", null);

        // Then
        var resultContent = bool.Parse(await
            result.Content.ReadAsStringAsync());

        resultContent.Should().BeTrue();
    }

    [Fact]
    public async void UserEmailExists_WhenEmailDoesNotExist_ReturnFalse()
    {
        // Given
        NewUserDTO testUser = new()
        {
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };

        // When
        var result = await _client.PostAsync($"user/email/exists?email={Uri.EscapeDataString(testUser.Email)}", null);

        // Then
        var resultContent = bool.Parse(await
            result.Content.ReadAsStringAsync());

        resultContent.Should().BeFalse();
    }

    [Fact]
    public async void UserUpdate_WhenUserExists_UpdateUserAndReturnOk()
    {
        // Given
        NewUserDTO testUser = new()
        {
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };
        var user = (await 
            (await _client
                .PostAsJsonAsync("user/create", testUser))
            .Content
            .ReadFromJsonAsync<UserDTO>())!;

        UserDTO updatedUser = new()
        {
            Id = user.Id,
            UserName = "NewUsername",
            Password = user.Password,
            Email = user.Email,
        };

        // When
        var result = await _client.PostAsJsonAsync("user/update", updatedUser);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var oldUserExistsResult = await _client.PostAsync($"user/username/exists?username={Uri.EscapeDataString(user.UserName)}", null);
        var newUserExistsResult = await _client.PostAsync($"user/username/exists?username={Uri.EscapeDataString(updatedUser.UserName)}", null);

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
    public async void UserUpdate_WhenUserDoesNotExist_DoNotUpdateAndReturnBadRequest()
    {
        // Given
        UserDTO testUser = new()
        {
            Id = "TestId",
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };

        // When
        var result = await _client.PostAsJsonAsync("user/update", testUser);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var userExistsResult = await _client
            .PostAsync($"user/username/exists?username={Uri.EscapeDataString(testUser.UserName)}", null);

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
        NewUserDTO testUser = new()
        {
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };
        var user = (await
            (await _client
                .PostAsJsonAsync("user/create", testUser))
            .Content
            .ReadFromJsonAsync<UserDTO>())!;

        UserDTO updatedUser = new()
        {
            Id = user.Id,
            UserName = username,
            Password = email,
            Email = password,
        };

        // When
        var result = await _client.PostAsJsonAsync("user/update", updatedUser);


        // Then
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async void UserRemove_WhenUserEmailDoesExist_DeleteUserAndReturnOk()
    {
        // Given
        NewUserDTO testUser = new()
        {
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };
        await(await _client
            .PostAsJsonAsync("user/create", testUser))
            .Content.ReadFromJsonAsync<UserDTO>();

        // When
        var result = await _client.DeleteAsync($"user/remove?emailOrId={Uri.EscapeDataString(testUser.Email)}");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var userExistsResult = await _client.PostAsync($"user/username/exists?username={Uri.EscapeDataString(testUser.UserName)}", null);
        var userExists = bool.Parse(await
            userExistsResult.Content.ReadAsStringAsync());
        userExists.Should().BeFalse();
    }

    [Fact]
    public async void UserRemove_WhenUserIdDoesExist_DeleteUserAndReturnOk()
    {
        // Given
        NewUserDTO testUser = new()
        {
            UserName = "TestUsername",
            Email = "test@mail.com",
            Password = "Test@123"
        };
        var user = await (await _client
            .PostAsJsonAsync("user/create", testUser))
            .Content.ReadFromJsonAsync<UserDTO>();

        // When
        var result = await _client.DeleteAsync($"user/remove?emailOrId={Uri.EscapeDataString(user!.Id!)}");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var userExistsResult = await _client.PostAsync($"user/username/exists?username={Uri.EscapeDataString(testUser.UserName)}", null);
        var userExists = bool.Parse(await
            userExistsResult.Content.ReadAsStringAsync());
        userExists.Should().BeFalse();
    }

    [Fact]
    public async void UserRemove_WhenUserEmailDoesNotExist_ReturnBadRequest()
    {
        // Given
        var email = "test@mail.com";

        // When
        var result = await _client.DeleteAsync($"user/remove?emailOrId={Uri.EscapeDataString(email)}");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async void UserRemove_WhenUserIdDoesNotExist_ReturnBadRequest()
    {
        // Given
        var id = "1";

        // When
        var result = await _client.DeleteAsync($"user/remove?emailOrId={Uri.EscapeDataString(id)}");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
