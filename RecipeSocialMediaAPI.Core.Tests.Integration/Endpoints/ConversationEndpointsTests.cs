using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Core.Tests.Integration.IntegrationHelpers;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;
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
        var result = await _client.PostAsync($"conversation/get-by-connection/?connectionId={connection.ConnectionId}&userId={user1.Id}", null);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<ConversationDTO>();
        data!.Id.Should().Be(conversation.ConversationId);
        data.ConnectionOrGroupId.Should().Be(connection.ConnectionId);
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
            .CreateMessage(user1, "Oldest message", new(), new(), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), null, new());
        Message testMessage2 = _fakeMessageRepository
            .CreateMessage(user2, "Some message", new(), new(), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero), null, new());
        Message testMessage3 = _fakeMessageRepository
            .CreateMessage(user1, "Last message", new(), new(), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero), null, new());
        
        conversation.SendMessage(testMessage1);
        conversation.SendMessage(testMessage3);
        conversation.SendMessage(testMessage2);
        
        // When
        var result = await _client.PostAsync($"conversation/get-by-connection/?connectionId={connection.ConnectionId}&userId={user1.Id}", null);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<ConversationDTO>();
        data!.Id.Should().Be(conversation.ConversationId);
        data.ConnectionOrGroupId.Should().Be(connection.ConnectionId);
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
        var result = await _client.PostAsync($"conversation/get-by-connection/?connectionId={connection.ConnectionId}&userId={user1.Id}", null);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetConversationByConnection_WhenConnectionDoesNotExist_ReturnNotFound()
    {
        // Given
        var user1 = _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)).Account;

        // When
        var result = await _client.PostAsync($"conversation/get-by-connection/?connectionId=1&userId={user1.Id}", null);

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
        var result = await _client.PostAsync($"conversation/get-by-group/?groupId={group.GroupId}&userId={user1.Id}", null);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<ConversationDTO>();
        data!.Id.Should().Be(conversation.ConversationId);
        data.ConnectionOrGroupId.Should().Be(group.GroupId);
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
            .CreateMessage(user1, "Oldest message", new(), new(), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), null, new());
        Message testMessage2 = _fakeMessageRepository
            .CreateMessage(user2, "Some message", new(), new(), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero), null, new());
        Message testMessage3 = _fakeMessageRepository
            .CreateMessage(user1, "Last message", new(), new(), new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero), null, new());

        conversation.SendMessage(testMessage1);
        conversation.SendMessage(testMessage3);
        conversation.SendMessage(testMessage2);

        // When
        var result = await _client.PostAsync($"conversation/get-by-group/?groupId={group.GroupId}&userId={user1.Id}", null);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var data = await result.Content.ReadFromJsonAsync<ConversationDTO>();
        data!.Id.Should().Be(conversation.ConversationId);
        data.ConnectionOrGroupId.Should().Be(group.GroupId);
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
        var result = await _client.PostAsync($"conversation/get-by-group/?groupId={group.GroupId}&userId={user1.Id}", null);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void GetConversationByGroup_WhenGroupDoesNotExist_ReturnNotFound()
    {
        // Given
        var user1 = _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)).Account;

        // When
        var result = await _client.PostAsync($"conversation/get-by-group/?groupId=1&userId={user1.Id}", null);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void CreateConnectionConversation_WhenConnectionIsValid_ReturnCreatedConnectionConversation()
    {
        // Given
        var user1 = _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        
        var user2= _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        IConnection newConnection = _fakeConnectionRepository.CreateConnection(user1.Account, user2.Account, ConnectionStatus.Connected);

        // When
        var result = await _client.PostAsync($"/conversation/create-by-connection/?userId={user1.Account.Id}&connectionId={newConnection.ConnectionId}", null);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var data = await result.Content.ReadFromJsonAsync<ConversationDTO>();

        data.Should().NotBeNull();
        data!.ConnectionOrGroupId.Should().Be(newConnection.ConnectionId);
        data.Id.Should().Be("0");
        data.LastMessage.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void CreateConnectionConversation_WhenConnectionIsNotFound_ReturnNotFound()
    {
        // Given
        var user1 = _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // When
        var result = await _client.PostAsync($"/conversation/create-by-connection/?userId={user1.Account.Id}&connectionId=0", null);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }



    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void CreateGroupConversation_WhenGroupIsValid_ReturnCreatedGroupConversation()
    {
        // Given        
        var user1 = _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var user2 = _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        Group newGroup = _fakeGroupRepository.CreateGroup("testGroup","This is a test group.",new List<IUserAccount> { user1.Account, user2.Account});

        // When
        var result = await _client.PostAsync($"/conversation/create-by-group/?userId={user1.Account.Id}&groupId={newGroup.GroupId}", null);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var data = await result.Content.ReadFromJsonAsync<ConversationDTO>();

        data.Should().NotBeNull();
        data!.ConnectionOrGroupId.Should().Be(newGroup.GroupId);
        data.Id.Should().Be("0");
        data.LastMessage.Should().BeNull();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void CreateGroupConversation_WhenGroupIsNotFound_ReturnNotFound()
    {
        // Given
        var user = _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // When
        var result = await _client.PostAsync($"/conversation/create-by-group/?userId={user.Account.Id}&groupId=0", null);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.CORE)]
    public async void MarkConversationAsRead_WhenConversationExists_MarkAllConversationsAsRead()
    {
        // Given
        var user1 = _fakeUserRepository
            .CreateUser(_testUser1.Account.Handler, _testUser1.Account.UserName, _testUser1.Email, _fakeCryptoService.Encrypt(_testUser1.Password), new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var user2 = _fakeUserRepository
            .CreateUser(_testUser2.Account.Handler, _testUser2.Account.UserName, _testUser2.Email, _fakeCryptoService.Encrypt(_testUser2.Password), new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero));
        var connection = _fakeConnectionRepository
            .CreateConnection(user1.Account, user2.Account, ConnectionStatus.Connected);
        var conversation = _fakeConversationRepository
            .CreateConnectionConversation(connection);

        var message1 = _fakeMessageRepository
            .CreateMessage(user1.Account, "Sent by user", new(), new(), new(2024, 2, 2, 12, 0, 0, TimeSpan.Zero), null, new() { user1.Account.Id, user2.Account.Id });
        var message2 = _fakeMessageRepository
            .CreateMessage(user2.Account, "Seen by user", new(), new(), new(2024, 2, 2, 12, 30, 0, TimeSpan.Zero), null, new() { user1.Account.Id, user2.Account.Id });
        var message3 = _fakeMessageRepository
            .CreateMessage(user1.Account, "Not seen 1", new(), new(), new(2024, 2, 2, 13, 1, 20, TimeSpan.Zero), null, new() { user2.Account.Id });
        var message4 = _fakeMessageRepository
            .CreateMessage(user1.Account, "Not seen 2", new(), new(), new(2024, 2, 2, 13, 2, 34, TimeSpan.Zero), null, new() { user2.Account.Id });

        conversation.SendMessage(message1);
        conversation.SendMessage(message2);
        conversation.SendMessage(message3);
        conversation.SendMessage(message4);

        // When
        var result = await _client.PutAsync($"/conversation/mark-as-read/?userId={user1.Account.Id}&conversationId={conversation.ConversationId}", null);

        // Then
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        message3.SeenBy.Should().Contain(user1.Account);
        message4.SeenBy.Should().Contain(user1.Account);
    }
}
