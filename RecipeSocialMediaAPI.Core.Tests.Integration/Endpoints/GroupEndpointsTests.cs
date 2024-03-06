using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.Endpoints;

public class GroupEndpointsTests : EndpointTestBase
{
    private readonly TestUserCredentials _testUser1;
    private readonly TestUserCredentials _testUser2;
    private readonly TestUserCredentials _testUser3;

    public GroupEndpointsTests(WebApplicationFactory<Program> factory) : base(factory)
    {
        _testUser1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "0",
                Handler = "testHandler1",
                UserName = "TestUsername1",
            },
            Email = "test1@mail.com",
            Password = "Test@123"
        };
        _testUser2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "1",
                Handler = "testHandler2",
                UserName = "TestUsername2",
            },
            Email = "test2@mail.com",
            Password = "Test@321"
        };
        _testUser3 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "2",
                Handler = "testHandler3",
                UserName = "TestUsername3",
            },
            Email = "test3@mail.com",
            Password = "Test@121"
        };
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task CreateGroup_WhenUsersExist_ReturnNewGroup()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));
        var user3 = await _fakeUserRepository
            .CreateUser(_testUser3.Account.Handler, _testUser3.Account.UserName, _testUser3.Email, _fakeCryptoService.Encrypt(_testUser3.Password), new(3034, 3, 3, 0, 0, 0, TimeSpan.Zero));
        
        List<string> userIds = new() { user1.Account.Id, user2.Account.Id, user3.Account.Id};
        NewGroupContract group = new("name", "Desciption", userIds);

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsJsonAsync($"group/create", group);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<GroupDTO>();
        data.Should().NotBeNull();
        data!.Name.Should().Be(group.Name);
        data.Description.Should().Be(group.Description);
        data.UserIds.Should().BeEquivalentTo(new List<string> { user1.Account.Id, user2.Account.Id, user3.Account.Id });
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    [InlineData(true, false, true)]
    [InlineData(false, true, true)]
    [InlineData(true, true, false)]
    public async Task CreateGroup_WhenUsersDoNotExist_ReturnNotFound(bool user1Exists, bool user2Exists, bool user3Exists)
    {
        // Given
        TestUserCredentials user = new()
        {
            Account = new TestUserAccount
            {
                Id = "u0",
                Handler = "user_0",
                UserName = "User 0"
            },
            Email = "u0@mail.com",
            Password = "Pass@123"
        };

        var user1 = user1Exists
            ? await _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero))
            : null;
        var user2 = user2Exists
            ? await _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))
            : null;
        var user3 = user3Exists
            ? await _fakeUserRepository
            .CreateUser(_testUser3.Account.Handler, _testUser3.Account.UserName, _testUser3.Email, _fakeCryptoService.Encrypt(_testUser3.Password), new(3034, 3, 3, 0, 0, 0, TimeSpan.Zero))
            : null;

        List<string> userIds = new() { user1?.Account.Id ?? "u1", user2?.Account.Id ?? "u2", user3?.Account.Id ?? "u3" };
        NewGroupContract group = new("name", "Desciption", userIds);

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsJsonAsync($"group/create", group);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task CreateGroup_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));
        var user3 = await _fakeUserRepository
            .CreateUser(_testUser3.Account.Handler, _testUser3.Account.UserName, _testUser3.Email, _fakeCryptoService.Encrypt(_testUser3.Password), new(3034, 3, 3, 0, 0, 0, TimeSpan.Zero));

        List<string> userIds = new() { user1.Account.Id, user2.Account.Id, user3.Account.Id };
        NewGroupContract group = new("name", "Desciption", userIds);

        // When
        var result = await _client.PostAsJsonAsync($"group/create", group);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task GetGroup_WhenGroupExists_ReturnGroup()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = (await _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))).Account;
        var user3 = (await _fakeUserRepository
            .CreateUser(_testUser3.Account.Handler, _testUser3.Account.UserName, _testUser3.Email, _fakeCryptoService.Encrypt(_testUser3.Password), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero))).Account;

        Group group = await _fakeGroupRepository
            .CreateGroup("Test Group", "Test Group Description", new() { user1.Account, user2, user3});

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"group/get/?groupId={group.GroupId}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<GroupDTO>();
        data.Should().NotBeNull();
        data!.Name.Should().Be(group.GroupName);
        data.Description.Should().Be(group.GroupDescription);
        data.UserIds.Should().BeEquivalentTo(new List<string> { user1.Account.Id, user2.Id, user3.Id });
    }
    
    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task GetGroup_WhenGroupDoesNotExist_ReturnNotFound()
    {
        // Given
        var user = await _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"group/get/?groupId=1", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task GetGroup_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        var user1 = (await _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero))).Account;
        var user2 = (await _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))).Account;
        var user3 = (await _fakeUserRepository
            .CreateUser(_testUser3.Account.Handler, _testUser3.Account.UserName, _testUser3.Email, _fakeCryptoService.Encrypt(_testUser3.Password), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero))).Account;

        Group group = await _fakeGroupRepository
            .CreateGroup("Test Group", "Test Group Description", new() { user1, user2, user3 });

        // When
        var result = await _client.PostAsync($"group/get/?groupId={group.GroupId}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task GetGroupsByUser_WhenGroupsExist_ReturnGroups()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = (await _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))).Account;
        var user3 = (await _fakeUserRepository
            .CreateUser(_testUser3.Account.Handler, _testUser3.Account.UserName, _testUser3.Email, _fakeCryptoService.Encrypt(_testUser3.Password), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero))).Account;

        Group group1 = await _fakeGroupRepository
            .CreateGroup("Test Group 1", "Test Group 1 Description", new() { user1.Account, user2 });
        Group group2 = await _fakeGroupRepository
            .CreateGroup("Test Group 2", "Test Group 2 Description", new() { user1.Account, user3 });
        _ = await _fakeGroupRepository
            .CreateGroup("Test Group 3", "Test Group 3 Description", new() { user2, user3 });

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"group/get-by-user/?userId={user1.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<GroupDTO>>();
        data.Should().NotBeNull();
        data.Should().HaveCount(2);
        data![0].Id.Should().Be(group1.GroupId);
        data![0].Name.Should().Be(group1.GroupName);
        data![0].Description.Should().Be(group1.GroupDescription);
        data![0].UserIds.Should().BeEquivalentTo(new List<string>() { user1.Account.Id, user2.Id });

        data![1].Id.Should().Be(group2.GroupId);
        data![1].Name.Should().Be(group2.GroupName);
        data![1].Description.Should().Be(group2.GroupDescription);
        data![1].UserIds.Should().BeEquivalentTo(new List<string>() { user1.Account.Id, user3.Id });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task GetGroupsByUser_WhenNoGroupsForUserExist_ReturnEmptyCollection()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = (await _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))).Account;
        var user3 = (await _fakeUserRepository
            .CreateUser(_testUser3.Account.Handler, _testUser3.Account.UserName, _testUser3.Email, _fakeCryptoService.Encrypt(_testUser3.Password), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero))).Account;

        _ = await _fakeGroupRepository
            .CreateGroup("Test Group", "Test Group Description", new() { user2, user3 });

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"group/get-by-user/?userId={user1.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<GroupDTO>>();
        data.Should().NotBeNull();
        data.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task GetGroupsByUser_WhenUserDoesNotExist_ReturnNotFound()
    {
        // Given
        TestUserCredentials user = new()
        {
            Account = new TestUserAccount
            {
                Id = "u0",
                Handler = "user_0",
                UserName = "User 0"
            },
            Email = "u0@mail.com",
            Password = "Pass@123"
        };

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"group/get-by-user/?userId=1", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task GetGroupsByUser_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        var user1 = (await _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero))).Account;
        var user2 = (await _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))).Account;
        var user3 = (await _fakeUserRepository
            .CreateUser(_testUser3.Account.Handler, _testUser3.Account.UserName, _testUser3.Email, _fakeCryptoService.Encrypt(_testUser3.Password), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero))).Account;

        _ = await _fakeGroupRepository
            .CreateGroup("Test Group 1", "Test Group 1 Description", new() { user1, user2 });
        _ = await _fakeGroupRepository
            .CreateGroup("Test Group 2", "Test Group 2 Description", new() { user1, user3 });
        _ = await _fakeGroupRepository
            .CreateGroup("Test Group 3", "Test Group 3 Description", new() { user2, user3 });

        // When
        var result = await _client.PostAsync($"group/get-by-user/?userId={user1.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task UpdateGroup_WhenGroupExists_ReturnOk()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = (await _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))).Account;
        var user3 = (await _fakeUserRepository
            .CreateUser(_testUser3.Account.Handler, _testUser3.Account.UserName, _testUser3.Email, _fakeCryptoService.Encrypt(_testUser3.Password), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero))).Account;

        Group existingGroup = await _fakeGroupRepository
            .CreateGroup("Test Group", "Test Group Description", new() { user1.Account, user2, user3 });

        UpdateGroupContract contract = new(existingGroup.GroupId, "New Group", "New Group Description", new() { user1.Account.Id, user2.Id });

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PutAsJsonAsync($"group/update", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var group = await _fakeGroupRepository.GetGroupById(existingGroup.GroupId);
        group.Should().NotBeNull();
        group!.GroupName.Should().Be(contract.GroupName);
        group.GroupDescription.Should().Be(contract.GroupDescription);
        group.Users.Should().BeEquivalentTo(new List<IUserAccount> { user1.Account, user2 });
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task UpdateGroup_WhenGroupDoesNotExist_ReturnNotFound()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = (await _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))).Account;
        
        UpdateGroupContract contract = new("1", "New Group", "New Group Description", new() { user1.Account.Id, user2.Id });

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PutAsJsonAsync($"group/update", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task UpdateGroup_WhenUserDoesNotExist_ReturnOk()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = (await _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))).Account;
        
        Group existingGroup = await _fakeGroupRepository
            .CreateGroup("Test Group", "Test Group Description", new() { user1.Account, user2 });

        UpdateGroupContract contract = new(existingGroup.GroupId, "New Group", "New Group Description", new() { user1.Account.Id, user2.Id, "nonExistentUser" });

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PutAsJsonAsync($"group/update", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task UpdateGroup_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        var user1 = (await _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero))).Account;
        var user2 = (await _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))).Account;
        var user3 = (await _fakeUserRepository
            .CreateUser(_testUser3.Account.Handler, _testUser3.Account.UserName, _testUser3.Email, _fakeCryptoService.Encrypt(_testUser3.Password), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero))).Account;

        Group existingGroup = await _fakeGroupRepository
            .CreateGroup("Test Group", "Test Group Description", new() { user1, user2, user3 });

        UpdateGroupContract contract = new(existingGroup.GroupId, "New Group", "New Group Description", new() { user1.Id, user2.Id });

        // When
        var result = await _client.PutAsJsonAsync($"group/update", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task DeleteGroup_WhenGroupExists_ReturnOk()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), UserRole.Developer);
        var user2 = (await _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))).Account;
        var user3 = (await _fakeUserRepository
            .CreateUser(_testUser3.Account.Handler, _testUser3.Account.UserName, _testUser3.Email, _fakeCryptoService.Encrypt(_testUser3.Password), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero))).Account;

        Group group = await _fakeGroupRepository
            .CreateGroup("Test Group", "Test Group Description", new() { user1.Account, user2, user3 });

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.DeleteAsync($"group/delete/?groupId={group.GroupId}");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        (await _fakeGroupRepository.GetGroupById(group.GroupId)).Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task DeleteGroup_WhenGroupDoesNotExist_ReturnNotFound()
    {
        // Given
        TestUserCredentials user = new()
        {
            Account = new TestUserAccount
            {
                Id = "u0",
                Handler = "user_0",
                UserName = "User 0",
                Role = UserRole.Developer
            },
            Email = "u0@mail.com",
            Password = "Pass@123"
        };

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.DeleteAsync($"group/delete/?groupId=1");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task DeleteGroup_WhenUserIsNotDeveloper_ReturnForbidden()
    {
        // Given
        TestUserCredentials user = new()
        {
            Account = new TestUserAccount
            {
                Id = "u0",
                Handler = "user_0",
                UserName = "User 0",
                Role = UserRole.User
            },
            Email = "u0@mail.com",
            Password = "Pass@123"
        };

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.DeleteAsync($"group/delete/?groupId=1");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async Task DeleteGroup_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        var user1 = (await _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero))).Account;
        var user2 = (await _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))).Account;
        var user3 = (await _fakeUserRepository
            .CreateUser(_testUser3.Account.Handler, _testUser3.Account.UserName, _testUser3.Email, _fakeCryptoService.Encrypt(_testUser3.Password), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero))).Account;

        Group group = await _fakeGroupRepository
            .CreateGroup("Test Group", "Test Group Description", new() { user1, user2, user3 });

        // When
        var result = await _client.DeleteAsync($"group/delete/?groupId={group.GroupId}");

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
