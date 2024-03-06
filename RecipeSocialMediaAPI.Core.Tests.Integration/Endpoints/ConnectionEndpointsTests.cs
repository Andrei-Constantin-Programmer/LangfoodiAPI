using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Net;
using System.Net.Http.Headers;
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
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConnection_WhenUsersExist_ReturnConnection()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));

        var existingConnection = await _fakeConnectionRepository
            .CreateConnectionAsync(user1.Account, user2.Account, ConnectionStatus.Pending);

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"connection/get/?userId1={user1.Account.Id}&userId2={user2.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<ConnectionDTO>();

        data.Should().NotBeNull();
        data!.ConnectionId.Should().Be(existingConnection.ConnectionId);
        data!.UserId1.Should().Be(user1.Account.Id);
        data!.UserId2.Should().Be(user2.Account.Id);
        data!.ConnectionStatus.Should().Be("Pending");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConnection_WhenConnectionDoesNotExist_ReturnNotFound()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"connection/get/?userId1={user1.Account.Id}&userId2={user2.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public async Task GetConnection_WhenUserDoesNotExist_ReturnNotFound(bool user1Exists, bool user2Exists)
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

        _ = user1Exists 
            ? _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero))
            : null;
        _ = user2Exists
            ? _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))
            : null;

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"connection/get/?userId1={_testUser1.Account.Id}&userId2={_testUser2.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConnection_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));

        _ = _fakeConnectionRepository
            .CreateConnectionAsync(user1.Account, user2.Account, ConnectionStatus.Pending);

        // When
        var result = await _client.PostAsync($"connection/get/?userId1={user1.Account.Id}&userId2={user2.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConnectionsByUser_WhenConnectionsFound_ReturnConnections()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));
        var user3 = await _fakeUserRepository
            .CreateUserAsync(_testUser3.Account.Handler, _testUser3.Account.UserName, _testUser3.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero));

        var connection1 = await _fakeConnectionRepository
            .CreateConnectionAsync(_testUser1.Account, _testUser2.Account, ConnectionStatus.Pending);
        var connection2 = await _fakeConnectionRepository
            .CreateConnectionAsync(_testUser1.Account, _testUser3.Account, ConnectionStatus.Connected);
        _ = _fakeConnectionRepository
            .CreateConnectionAsync(_testUser3.Account, _testUser2.Account, ConnectionStatus.Blocked);

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"connection/get-by-user/?userId={user1.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var data = await result.Content.ReadFromJsonAsync<List<ConnectionDTO>>();
        data.Should().NotBeNull();
        data.Should().HaveCount(2);
        data![0].ConnectionId.Should().Be(connection1.ConnectionId);
        data[0].UserId1.Should().Be(user1.Account.Id);
        data[0].UserId2.Should().Be(user2.Account.Id);
        data[0].ConnectionStatus.Should().Be("Pending");

        data[1].ConnectionId.Should().Be(connection2.ConnectionId);
        data[1].UserId1.Should().Be(user1.Account.Id);
        data[1].UserId2.Should().Be(user3.Account.Id);
        data[1].ConnectionStatus.Should().Be("Connected");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConnectionsByUser_WhenNoConnectionsFound_ReturnEmptyCollection()
    {
        // Given
        var user = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"connection/get-by-user/?userId={user.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<ConnectionDTO>>();
        data.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConnectionsByUser_WhenUserDoesNotExist_ReturnNotFound()
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
        var result = await _client.PostAsync($"connection/get-by-user/?userId={_testUser1.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConnectionsByUser_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        _ = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));
        _ = await _fakeUserRepository
            .CreateUserAsync(_testUser3.Account.Handler, _testUser3.Account.UserName, _testUser3.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero));

        _ = await _fakeConnectionRepository
            .CreateConnectionAsync(_testUser1.Account, _testUser2.Account, ConnectionStatus.Pending);
        _ = await _fakeConnectionRepository
            .CreateConnectionAsync(_testUser1.Account, _testUser3.Account, ConnectionStatus.Connected);
        _ = await _fakeConnectionRepository
            .CreateConnectionAsync(_testUser3.Account, _testUser2.Account, ConnectionStatus.Blocked);

        // When
        var result = await _client.PostAsync($"connection/get-by-user/?userId={user1.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task CreateConnection_WhenUsersExist_ReturnNewConnection()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));

        NewConnectionContract newConnection = new(user1.Account.Id, user2.Account.Id);

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsJsonAsync($"connection/create", newConnection);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<ConnectionDTO>();
        data!.ConnectionId.Should().Be("0");
        data.UserId1.Should().Be(user1.Account.Id);
        data.UserId2.Should().Be(user2.Account.Id);
        data.ConnectionStatus.Should().Be("Pending");
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public async Task CreateConnection_WhenUsersDoNotExist_ReturnNotFound(bool user1Exists, bool user2Exists)
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

        var user1 = user1Exists 
            ? await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero))
            : null;
        var user2 = user2Exists 
            ? await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))
            : null;

        NewConnectionContract contract = new(user1?.Account.Id ?? "user1", user2?.Account.Id ?? "user2");

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsJsonAsync($"connection/create", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task CreateConnection_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));

        NewConnectionContract newConnection = new(user1.Account.Id, user2.Account.Id);

        // When
        var result = await _client.PostAsJsonAsync($"connection/create", newConnection);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    [InlineData(ConnectionStatus.Connected)]
    [InlineData(ConnectionStatus.Blocked)]
    public async Task UpdateConnection_WhenConnectionExists_UpdateTheConnection(ConnectionStatus connectionStatus)
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));
        _ = _fakeConnectionRepository
            .CreateConnectionAsync(user1.Account, user2.Account, ConnectionStatus.Pending);

        UpdateConnectionContract contract = new(user1.Account.Id, user2.Account.Id, connectionStatus.ToString());

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PutAsJsonAsync($"connection/update", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var connection = await _fakeConnectionRepository.GetConnectionAsync(user1.Account, user2.Account);
        connection.Should().NotBeNull();
        connection!.Status.Should().Be(connectionStatus);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task UpdateConnection_WhenConnectionDoesNotExist_ReturnNotFound()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));

        UpdateConnectionContract contract = new(user1.Account.Id, user2.Account.Id, "Created");

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PutAsJsonAsync($"connection/update", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public async Task UpdateConnection_WhenUserDoesNotExist_ReturnNotFound(bool user1Exists, bool user2Exists)
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

        var user1 = user1Exists
            ? await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero))
            : new TestUserCredentials()
            {
                Account = new TestUserAccount()
                {
                    Id = "u1",
                    Handler = "user1",
                    UserName = "User 1"
                },
                Email = "u1@mail.com",
                Password = "Test@123"
            };

        var user2 = user2Exists
            ? await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))
            : new TestUserCredentials()
            {
                Account = new TestUserAccount()
                {
                    Id = "u2",
                    Handler = "user2",
                    UserName = "User 2"
                },
                Email = "u2@mail.com",
                Password = "Test@123"
            };

        _ = _fakeConnectionRepository
            .CreateConnectionAsync(user1.Account, user2.Account, ConnectionStatus.Pending);

        UpdateConnectionContract contract = new(user1.Account.Id, user2.Account.Id, "Created");

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PutAsJsonAsync($"connection/update", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task UpdateConnection_WhenConnectionStatusIsNotSupported_ReturnBadRequest()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));
        _ = await _fakeConnectionRepository
            .CreateConnectionAsync(user1.Account, user2.Account, ConnectionStatus.Pending);

        UpdateConnectionContract contract = new(user1.Account.Id, user2.Account.Id, "Unsupported Status");

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PutAsJsonAsync($"connection/update", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task UpdateConnection_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));
        _ = await _fakeConnectionRepository
            .CreateConnectionAsync(user1.Account, user2.Account, ConnectionStatus.Pending);

        UpdateConnectionContract contract = new(user1.Account.Id, user2.Account.Id, "Connected");

        // When
        var result = await _client.PutAsJsonAsync($"connection/update", contract);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
