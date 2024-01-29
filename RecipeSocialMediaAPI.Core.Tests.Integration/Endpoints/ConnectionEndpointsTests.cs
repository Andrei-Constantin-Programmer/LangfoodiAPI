using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.DTO.Recipes;
using RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Net;
using System.Net.Http.Json;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.Endpoints;

public class ConnectionEndpointsTests : EndpointTestBase
{
    private readonly TestUserCredentials _testUser1;
    private readonly TestUserCredentials _testUser2;
    private readonly TestUserCredentials _testUser3;

    public ConnectionEndpointsTests(WebApplicationFactory<Program> factory) : base(factory) 
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
            Password = "Test@123"
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
            Password = "Test@123"
        };
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetConnection_WhenUsersExist_ReturnConnection()
    {
        // Given
        var user1 = _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));

        _ = _fakeConnectionRepository
            .CreateConnection(user1.Account, user2.Account, ConnectionStatus.Pending);

        // When
        var result = await _client.PostAsync($"connection/get/?userId1={user1.Account.Id}&userId2={user2.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<ConnectionDTO>();

        data.Should().NotBeNull();
        data!.UserId1.Should().Be(user1.Account.Id);
        data!.UserId2.Should().Be(user2.Account.Id);
        data!.ConnectionStatus.Should().Be("Pending");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetConnection_WhenConnectionDoesNotExist_ReturnNotFound()
    {
        // Given
        var user1 = _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));

        // When
        var result = await _client.PostAsync($"connection/get/?userId1={user1.Account.Id}&userId2={user2.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public async void GetConnection_WhenUserDoesNotExist_ReturnNotFound(bool user1Exists, bool user2Exists)
    {
        // Given
        _ = user1Exists 
            ? _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero))
            : null;
        _ = user2Exists
            ? _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))
            : null;

        // When
        var result = await _client.PostAsync($"connection/get/?userId1={_testUser1.Account.Id}&userId2={_testUser2.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetConnectionsByUser_WhenConnectionsFound_ReturnConnections()
    {
        // Given
        var user1 = _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));
        var user3 = _fakeUserRepository
            .CreateUser(_testUser3.Account.Handler, _testUser3.Account.UserName, _testUser3.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero));

        _ = _fakeConnectionRepository
            .CreateConnection(_testUser1.Account, _testUser2.Account, ConnectionStatus.Pending);
        _ = _fakeConnectionRepository
            .CreateConnection(_testUser1.Account, _testUser3.Account, ConnectionStatus.Connected);
        _ = _fakeConnectionRepository
            .CreateConnection(_testUser3.Account, _testUser2.Account, ConnectionStatus.Blocked);

        // When
        var result = await _client.PostAsync($"connection/get-by-user/?userId={user1.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<ConnectionDTO>>();
        data.Should().NotBeNull();
        data.Should().HaveCount(2);
        data![0].UserId1.Should().Be(user1.Account.Id);
        data[0].UserId2.Should().Be(user2.Account.Id);
        data[0].ConnectionStatus.Should().Be("Pending");

        data[1].UserId1.Should().Be(user1.Account.Id);
        data[1].UserId2.Should().Be(user3.Account.Id);
        data[1].ConnectionStatus.Should().Be("Connected");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetConnectionsByUser_WhenNoConnectionsFound_ReturnEmptyCollection()
    {
        // Given
        var user = _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // When
        var result = await _client.PostAsync($"connection/get-by-user/?userId={user.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<ConnectionDTO>>();
        data.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetConnectionsByUser_WhenUserDoesNotExist_ReturnNotFound()
    {
        // Given

        // When
        var result = await _client.PostAsync($"connection/get-by-user/?userId={_testUser1.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void CreateConnection_WhenUsersExist_ReturnNewConnection()
    {
        // Given
        var user1 = _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));

        NewConnectionContract newConnection = new(user1.Account.Id, user2.Account.Id);

        // When
        var result = await _client.PostAsJsonAsync($"connection/create", newConnection);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<ConnectionDTO>();
        data!.UserId1.Should().Be(user1.Account.Id);
        data.UserId2.Should().Be(user2.Account.Id);
        data.ConnectionStatus.Should().Be("Pending");
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public async void CreateConnection_WhenUsersDoNotExist_ReturnNewConnection(bool user1Exists, bool user2Exists)
    {
        // Given
        var user1 = user1Exists 
            ? _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero))
            : null;
        var user2 = user2Exists 
            ? _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))
            : null;

        NewConnectionContract newConnection = new(user1?.Account.Id ?? "user1", user2?.Account.Id ?? "user2");

        // When
        var result = await _client.PostAsJsonAsync($"connection/create", newConnection);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
