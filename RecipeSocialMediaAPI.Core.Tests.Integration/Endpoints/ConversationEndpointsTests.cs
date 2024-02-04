using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Net.Http.Json;

namespace RecipeSocialMediaAPI.Core.Tests.Integration.Endpoints;

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
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetConversationByConnection_WhenConversationExistsWithNoMessages_ReturnConversationWithoutLastMessage()
    {
        // Given
        var user1 = _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)).Account;
        var user2 = _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero)).Account;

        var connection = _fakeConnectionRepository
            .CreateConnection(user1, user2, Domain.Models.Messaging.Connections.ConnectionStatus.Connected);

        var conversation = _fakeConversationRepository
            .CreateConnectionConversation(connection);

        // When
        var result = await _client.PostAsync($"conversation/get-by-connection/?connectionId={connection.ConnectionId}", null);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<ConnectionConversationDTO>();
        data!.ConversationId.Should().Be(conversation.ConversationId);
        data.ConnectionId.Should().Be(connection.ConnectionId);
        data.LastMessage.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetConversationByConnection_WhenConversationExistsWithMessages_ReturnConversationWithLastMessage()
    {
        // Given
        var user1 = _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)).Account;
        var user2 = _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero)).Account;

        var connection = _fakeConnectionRepository
            .CreateConnection(user1, user2, Domain.Models.Messaging.Connections.ConnectionStatus.Connected);

        var conversation = _fakeConversationRepository
            .CreateConnectionConversation(connection);

        Message testMessage1 = _fakeMessageRepository
            .CreateMessage(user1, "Oldest message", new(), new(), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), null);
        Message testMessage2 = _fakeMessageRepository
            .CreateMessage(user2, "Some message", new(), new(), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero), null);
        Message testMessage3 = _fakeMessageRepository
            .CreateMessage(user1, "Last message", new(), new(), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero), null);
        
        conversation.SendMessage(testMessage1);
        conversation.SendMessage(testMessage3);
        conversation.SendMessage(testMessage2);
        
        // When
        var result = await _client.PostAsync($"conversation/get-by-connection/?connectionId={connection.ConnectionId}", null);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<ConnectionConversationDTO>();
        data!.ConversationId.Should().Be(conversation.ConversationId);
        data.ConnectionId.Should().Be(connection.ConnectionId);
        data.LastMessage.Should().NotBeNull();
        data.LastMessage!.Id.Should().Be(testMessage3.Id);
        data.LastMessage.SenderName.Should().Be(testMessage3.Sender.UserName);
        data.LastMessage.SentDate.Should().Be(testMessage3.SentDate);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetConversationByConnection_WhenConversationDoesNotExist_ReturnNotFound()
    {
        // Given
        var user1 = _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)).Account;
        var user2 = _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero)).Account;

        var connection = _fakeConnectionRepository
            .CreateConnection(user1, user2, Domain.Models.Messaging.Connections.ConnectionStatus.Connected);

        // When
        var result = await _client.PostAsync($"conversation/get-by-connection/?connectionId={connection.ConnectionId}", null);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetConversationByConnection_WhenConnectionDoesNotExist_ReturnNotFound()
    {
        // Given

        // When
        var result = await _client.PostAsync($"conversation/get-by-connection/?connectionId=1", null);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetConversationByGroup_WhenConversationExistsWithNoMessages_ReturnConversationWithoutLastMessage()
    {
        // Given
        var user1 = _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)).Account;
        var user2 = _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero)).Account;

        var group = _fakeGroupRepository
            .CreateGroup("Group", "Group Desc", new() { user1, user2 });

        var conversation = _fakeConversationRepository
            .CreateGroupConversation(group);

        // When
        var result = await _client.PostAsync($"conversation/get-by-group/?groupId={group.GroupId}", null);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<GroupConversationDTO>();
        data!.ConversationId.Should().Be(conversation.ConversationId);
        data.GroupId.Should().Be(group.GroupId);
        data.LastMessage.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetConversationByGroup_WhenConversationExistsWithMessages_ReturnConversationWithLastMessage()
    {
        // Given
        var user1 = _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)).Account;
        var user2 = _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero)).Account;

        var group = _fakeGroupRepository
            .CreateGroup("Group", "Group Desc", new() { user1, user2 });

        var conversation = _fakeConversationRepository
            .CreateGroupConversation(group);

        Message testMessage1 = _fakeMessageRepository
            .CreateMessage(user1, "Oldest message", new(), new(), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), null);
        Message testMessage2 = _fakeMessageRepository
            .CreateMessage(user2, "Some message", new(), new(), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero), null);
        Message testMessage3 = _fakeMessageRepository
            .CreateMessage(user1, "Last message", new(), new(), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero), null);

        conversation.SendMessage(testMessage1);
        conversation.SendMessage(testMessage3);
        conversation.SendMessage(testMessage2);

        // When
        var result = await _client.PostAsync($"conversation/get-by-group/?groupId={group.GroupId}", null);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<GroupConversationDTO>();
        data!.ConversationId.Should().Be(conversation.ConversationId);
        data.GroupId.Should().Be(group.GroupId);
        data.LastMessage.Should().NotBeNull();
        data.LastMessage!.Id.Should().Be(testMessage3.Id);
        data.LastMessage.SenderName.Should().Be(testMessage3.Sender.UserName);
        data.LastMessage.SentDate.Should().Be(testMessage3.SentDate);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetConversationByGroup_WhenConversationDoesNotExist_ReturnNotFound()
    {
        // Given
        var user1 = _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)).Account;
        var user2 = _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero)).Account;

        var group = _fakeGroupRepository
            .CreateGroup("Group", "Group Desc", new() { user1, user2 });

        // When
        var result = await _client.PostAsync($"conversation/get-by-group/?groupId= {group.GroupId}", null);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetConversationByGroup_WhenGroupDoesNotExist_ReturnNotFound()
    {
        // Given

        // When
        var result = await _client.PostAsync($"conversation/get-by-group/?groupId=1", null);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void CreateConnectionConversation_WhenConnectionIsValid_ReturnCreatedConnectionConversation()
    {
        // Given        
        _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        
        _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        IConnection newConnection = _fakeConnectionRepository.CreateConnection(_testUser1.Account, _testUser2.Account, Domain.Models.Messaging.Connections.ConnectionStatus.Connected);

        NewConversationContract newConversation = new(newConnection.ConnectionId);

        // When
        var result = await _client.PostAsJsonAsync("/conversation/create-by-connection", newConversation);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var data = await result.Content.ReadFromJsonAsync<ConnectionConversationDTO>();

        data.Should().NotBeNull();
        data!.ConnectionId.Should().Be(newConversation.GroupOrConnectionId);
        data.ConversationId.Should().Be("0");
        data.LastMessage.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void CreateConnectionConversation_WhenConnectionIsNotFound_ReturnNotFound()
    {
        // Given
        NewConversationContract newConversation = new("0");

        // When
        var result = await _client.PostAsJsonAsync("/conversation/create-by-connection", newConversation);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}
