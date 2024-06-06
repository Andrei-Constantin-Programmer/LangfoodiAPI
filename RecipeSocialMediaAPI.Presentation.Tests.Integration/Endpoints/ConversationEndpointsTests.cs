using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Presentation.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RecipeSocialMediaAPI.Presentation.Tests.Integration.Endpoints;

public class ConversationEndpointsTests : EndpointTestBase
{
    private readonly TestUserCredentials _testUser1;
    private readonly TestUserCredentials _testUser2;

    public ConversationEndpointsTests(WebApplicationFactory<Program> factory) : base(factory)
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
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConversationsByUser_WhenConversationsExist_ReturnConversations()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = (await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakePasswordCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))).Account;

        var connection = await _fakeConnectionRepository
            .CreateConnectionAsync(user1.Account, user2, ConnectionStatus.Connected);
        var conversation1 = await _fakeConversationRepository
            .CreateConnectionConversationAsync(connection);

        var group = await _fakeGroupRepository
            .CreateGroupAsync("Group", "Group Desc", new() { user1.Account, user2 });
        var conversation2 = await _fakeConversationRepository
            .CreateGroupConversationAsync(group);

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"conversation/get-by-user/?userId={user1.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<ConversationDto>>();

        data![0].Id.Should().Be(conversation1.ConversationId);
        data[0].ConnectionOrGroupId.Should().Be(connection.ConnectionId);
        data[0].IsGroup.Should().BeFalse();

        data[1].Id.Should().Be(conversation2.ConversationId);
        data[1].ConnectionOrGroupId.Should().Be(group.GroupId);
        data[1].IsGroup.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConversationsByUser_WhenNoConversationsExistForUser_ReturnEmptyList()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"conversation/get-by-user/?userId={user1.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<List<ConversationDto>>();
        data.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConversationsByUser_WhenUserDoesNotExist_ReturnNotFound()
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
        var result = await _client.PostAsync("conversation/get-by-user/?userId=0", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConversationsByUser_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = (await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakePasswordCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))).Account;

        var connection = await _fakeConnectionRepository
            .CreateConnectionAsync(user1.Account, user2, ConnectionStatus.Connected);
        _ = await _fakeConversationRepository
            .CreateConnectionConversationAsync(connection);

        var group = await _fakeGroupRepository
            .CreateGroupAsync("Group", "Group Desc", new() { user1.Account, user2 });
        _ = await _fakeConversationRepository
            .CreateGroupConversationAsync(group);

        // When
        var result = await _client.PostAsync($"conversation/get-by-user/?userId={user1.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConversationByConnection_WhenConversationExistsWithNoMessages_ReturnConversationWithoutLastMessage()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = (await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakePasswordCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))).Account;

        var connection = await _fakeConnectionRepository
            .CreateConnectionAsync(user1.Account, user2, ConnectionStatus.Connected);
        var conversation = await _fakeConversationRepository
            .CreateConnectionConversationAsync(connection);

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"conversation/get-by-connection/?connectionId={connection.ConnectionId}&userId={user1.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<ConversationDto>();
        data!.Id.Should().Be(conversation.ConversationId);
        data.ConnectionOrGroupId.Should().Be(connection.ConnectionId);
        data.LastMessage.Should().BeNull();
        data.IsGroup.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConversationByConnection_WhenConversationExistsWithMessages_ReturnConversationWithLastMessage()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = (await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakePasswordCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))).Account;

        var connection = await _fakeConnectionRepository
            .CreateConnectionAsync(user1.Account, user2, ConnectionStatus.Connected);
        var conversation = await _fakeConversationRepository
            .CreateConnectionConversationAsync(connection);

        Message testMessage1 = await _fakeMessageRepository
            .CreateMessageAsync(user1.Account, "Oldest message", new(), new(), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), null, new());
        Message testMessage2 = await _fakeMessageRepository
            .CreateMessageAsync(user2, "Some message", new(), new(), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero), null, new());
        Message testMessage3 = await _fakeMessageRepository
            .CreateMessageAsync(user1.Account, "Last message", new(), new(), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero), null, new());

        conversation.SendMessage(testMessage1);
        conversation.SendMessage(testMessage3);
        conversation.SendMessage(testMessage2);

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"conversation/get-by-connection/?connectionId={connection.ConnectionId}&userId={user1.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<ConversationDto>();
        data!.Id.Should().Be(conversation.ConversationId);
        data.ConnectionOrGroupId.Should().Be(connection.ConnectionId);
        data.LastMessage.Should().NotBeNull();
        data.LastMessage!.Id.Should().Be(testMessage3.Id);
        data.LastMessage.UserPreview.Username.Should().Be(testMessage3.Sender.UserName);
        data.LastMessage.SentDate.Should().Be(testMessage3.SentDate);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConversationByConnection_WhenConversationDoesNotExist_ReturnNotFound()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = (await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakePasswordCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))).Account;

        var connection = await _fakeConnectionRepository
            .CreateConnectionAsync(user1.Account, user2, ConnectionStatus.Connected);

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"conversation/get-by-connection/?connectionId={connection.ConnectionId}&userId={user1.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConversationByConnection_WhenConnectionDoesNotExist_ReturnNotFound()
    {
        // Given
        var user = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"conversation/get-by-connection/?connectionId=1&userId={user.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConversationByConnection_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = (await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakePasswordCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))).Account;

        var connection = await _fakeConnectionRepository
            .CreateConnectionAsync(user1.Account, user2, ConnectionStatus.Connected);
        _ = await _fakeConversationRepository
            .CreateConnectionConversationAsync(connection);

        // When
        var result = await _client.PostAsync($"conversation/get-by-connection/?connectionId={connection.ConnectionId}&userId={user1.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConversationByGroup_WhenConversationExistsWithNoMessages_ReturnConversationWithoutLastMessage()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = (await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakePasswordCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))).Account;

        var group = await _fakeGroupRepository
            .CreateGroupAsync("Group", "Group Desc", new() { user1.Account, user2 });
        var conversation = await _fakeConversationRepository
            .CreateGroupConversationAsync(group);

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"conversation/get-by-group/?groupId={group.GroupId}&userId={user1.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<ConversationDto>();
        data!.Id.Should().Be(conversation.ConversationId);
        data.ConnectionOrGroupId.Should().Be(group.GroupId);
        data.LastMessage.Should().BeNull();
        data.IsGroup.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConversationByGroup_WhenConversationExistsWithMessages_ReturnConversationWithLastMessage()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = (await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakePasswordCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))).Account;

        var group = await _fakeGroupRepository
            .CreateGroupAsync("Group", "Group Desc", new() { user1.Account, user2 });

        var conversation = await _fakeConversationRepository
            .CreateGroupConversationAsync(group);

        Message testMessage1 = await _fakeMessageRepository
            .CreateMessageAsync(user1.Account, "Oldest message", new(), new(), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), null, new());
        Message testMessage2 = await _fakeMessageRepository
            .CreateMessageAsync(user2, "Some message", new(), new(), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero), null, new());
        Message testMessage3 = await _fakeMessageRepository
            .CreateMessageAsync(user1.Account, "Last message", new(), new(), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero), null, new());

        conversation.SendMessage(testMessage1);
        conversation.SendMessage(testMessage3);
        conversation.SendMessage(testMessage2);

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"conversation/get-by-group/?groupId={group.GroupId}&userId={user1.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<ConversationDto>();
        data!.Id.Should().Be(conversation.ConversationId);
        data.ConnectionOrGroupId.Should().Be(group.GroupId);
        data.LastMessage.Should().NotBeNull();
        data.LastMessage!.Id.Should().Be(testMessage3.Id);
        data.LastMessage.UserPreview.Username.Should().Be(testMessage3.Sender.UserName);
        data.LastMessage.SentDate.Should().Be(testMessage3.SentDate);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConversationByGroup_WhenConversationDoesNotExist_ReturnNotFound()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = (await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakePasswordCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))).Account;

        var group = await _fakeGroupRepository
            .CreateGroupAsync("Group", "Group Desc", new() { user1.Account, user2 });

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"conversation/get-by-group/?groupId={group.GroupId}&userId={user1.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConversationByGroup_WhenGroupDoesNotExist_ReturnNotFound()
    {
        // Given
        var user = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"conversation/get-by-group/?groupId=1&userId={user.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task GetConversationByGroup_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = (await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakePasswordCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero))).Account;

        var group = await _fakeGroupRepository
            .CreateGroupAsync("Group", "Group Desc", new() { user1.Account, user2 });

        _ = await _fakeConversationRepository
            .CreateGroupConversationAsync(group);

        // When
        var result = await _client.PostAsync($"conversation/get-by-group/?groupId={group.GroupId}&userId={user1.Account.Id}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task CreateConnectionConversation_WhenConnectionIsValid_ReturnCreatedConnectionConversation()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var user2 = await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakePasswordCryptoService.Encrypt(_testUser2.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        IConnection newConnection = await _fakeConnectionRepository.CreateConnectionAsync(user1.Account, user2.Account, ConnectionStatus.Connected);

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"/conversation/create-by-connection/?userId={user1.Account.Id}&connectionId={newConnection.ConnectionId}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var data = await result.Content.ReadFromJsonAsync<ConversationDto>();

        data.Should().NotBeNull();
        data!.ConnectionOrGroupId.Should().Be(newConnection.ConnectionId);
        data.Id.Should().Be("0");
        data.LastMessage.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task CreateConnectionConversation_WhenConnectionIsNotFound_ReturnNotFound()
    {
        // Given
        var user = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"/conversation/create-by-connection/?userId={user.Account.Id}&connectionId=0", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task CreateConnectionConversation_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var user2 = await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakePasswordCryptoService.Encrypt(_testUser2.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        IConnection newConnection = await _fakeConnectionRepository.CreateConnectionAsync(user1.Account, user2.Account, ConnectionStatus.Connected);

        // When
        var result = await _client.PostAsync($"/conversation/create-by-connection/?userId={user1.Account.Id}&connectionId={newConnection.ConnectionId}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task CreateGroupConversation_WhenGroupIsValid_ReturnCreatedGroupConversation()
    {
        // Given        
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var user2 = await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakePasswordCryptoService.Encrypt(_testUser2.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        Group newGroup = await _fakeGroupRepository.CreateGroupAsync("testGroup", "This is a test group.", new List<IUserAccount> { user1.Account, user2.Account });

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"/conversation/create-by-group/?userId={user1.Account.Id}&groupId={newGroup.GroupId}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var data = await result.Content.ReadFromJsonAsync<ConversationDto>();

        data.Should().NotBeNull();
        data!.ConnectionOrGroupId.Should().Be(newGroup.GroupId);
        data.Id.Should().Be("0");
        data.LastMessage.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task CreateGroupConversation_WhenGroupIsNotFound_ReturnNotFound()
    {
        // Given
        var user = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var token = _bearerTokenGeneratorService.GenerateToken(user);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PostAsync($"/conversation/create-by-group/?userId={user.Account.Id}&groupId=0", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task CreateGroupConversation_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given        
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var user2 = await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakePasswordCryptoService.Encrypt(_testUser2.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        Group newGroup = await _fakeGroupRepository.CreateGroupAsync("testGroup", "This is a test group.", new List<IUserAccount> { user1.Account, user2.Account });

        // When
        var result = await _client.PostAsync($"/conversation/create-by-group/?userId={user1.Account.Id}&groupId={newGroup.GroupId}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task MarkConversationAsRead_WhenConversationExists_MarkAllConversationsAsRead()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakePasswordCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));
        var connection = await _fakeConnectionRepository
            .CreateConnectionAsync(user1.Account, user2.Account, ConnectionStatus.Connected);
        var conversation = await _fakeConversationRepository
            .CreateConnectionConversationAsync(connection);

        var message1 = await _fakeMessageRepository
            .CreateMessageAsync(user1.Account, "Sent by user", new(), new(), new(2024, 2, 2, 12, 0, 0, TimeSpan.Zero), null, new() { user1.Account.Id, user2.Account.Id });
        var message2 = await _fakeMessageRepository
            .CreateMessageAsync(user2.Account, "Seen by user", new(), new(), new(2024, 2, 2, 12, 30, 0, TimeSpan.Zero), null, new() { user1.Account.Id, user2.Account.Id });
        var message3 = await _fakeMessageRepository
            .CreateMessageAsync(user1.Account, "Not seen 1", new(), new(), new(2024, 2, 2, 13, 1, 20, TimeSpan.Zero), null, new() { user2.Account.Id });
        var message4 = await _fakeMessageRepository
            .CreateMessageAsync(user1.Account, "Not seen 2", new(), new(), new(2024, 2, 2, 13, 2, 34, TimeSpan.Zero), null, new() { user2.Account.Id });

        conversation.SendMessage(message1);
        conversation.SendMessage(message2);
        conversation.SendMessage(message3);
        conversation.SendMessage(message4);

        var token = _bearerTokenGeneratorService.GenerateToken(user1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // When
        var result = await _client.PutAsync($"/conversation/mark-as-read/?userId={user1.Account.Id}&conversationId={conversation.ConversationId}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        message3.GetSeenBy().Should().Contain(user1.Account);
        message4.GetSeenBy().Should().Contain(user1.Account);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.PRESENTATION)]
    public async Task MarkConversationAsRead_WhenNoTokenIsUsed_ReturnUnauthorised()
    {
        // Given
        var user1 = await _fakeUserRepository
            .CreateUserAsync(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakePasswordCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = await _fakeUserRepository
            .CreateUserAsync(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakePasswordCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));
        var connection = await _fakeConnectionRepository
            .CreateConnectionAsync(user1.Account, user2.Account, ConnectionStatus.Connected);
        var conversation = await _fakeConversationRepository
            .CreateConnectionConversationAsync(connection);

        var message1 = await _fakeMessageRepository
            .CreateMessageAsync(user1.Account, "Sent by user", new(), new(), new(2024, 2, 2, 12, 0, 0, TimeSpan.Zero), null, new() { user1.Account.Id, user2.Account.Id });
        var message2 = await _fakeMessageRepository
            .CreateMessageAsync(user2.Account, "Seen by user", new(), new(), new(2024, 2, 2, 12, 30, 0, TimeSpan.Zero), null, new() { user1.Account.Id, user2.Account.Id });
        var message3 = await _fakeMessageRepository
            .CreateMessageAsync(user1.Account, "Not seen 1", new(), new(), new(2024, 2, 2, 13, 1, 20, TimeSpan.Zero), null, new() { user2.Account.Id });
        var message4 = await _fakeMessageRepository
            .CreateMessageAsync(user1.Account, "Not seen 2", new(), new(), new(2024, 2, 2, 13, 2, 34, TimeSpan.Zero), null, new() { user2.Account.Id });

        conversation.SendMessage(message1);
        conversation.SendMessage(message2);
        conversation.SendMessage(message3);
        conversation.SendMessage(message4);

        // When
        var result = await _client.PutAsync($"/conversation/mark-as-read/?userId={user1.Account.Id}&conversationId={conversation.ConversationId}", null);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
