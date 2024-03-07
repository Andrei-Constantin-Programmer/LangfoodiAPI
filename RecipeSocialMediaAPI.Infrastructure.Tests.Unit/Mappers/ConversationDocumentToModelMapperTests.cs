using FluentAssertions;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Infrastructure.Exceptions;
using RecipeSocialMediaAPI.Infrastructure.Mappers;
using RecipeSocialMediaAPI.Infrastructure.MongoDocuments;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Infrastructure.Tests.Unit.Mappers;

public class ConversationDocumentToModelMapperTests
{
    private readonly ConversationDocumentToModelMapper _conversationDocumentToModelMapperSUT;

    public ConversationDocumentToModelMapperTests()
    {
        _conversationDocumentToModelMapperSUT = new ConversationDocumentToModelMapper();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public void MapConversationFromDocument_WhenDocumentIsValidAndIsConnectionConversation_ReturnMappedConversation()
    {
        // Given
        ConversationDocument conversationDocument = new(
            Id: "myCoversationID",
            ConnectionId: "myConnectionID",
            GroupId: null,
            Messages: new()
            {
                "message1",
                "message2",
            }
        );

        TestUserAccount testUserAccount1 = new()
        {
            Id = "user1",
            Handler = "handler1",
            UserName = "username1"
        };

        TestUserAccount testUserAccount2 = new()
        {
            Id = "user2",
            Handler = "handler2",
            UserName = "username2"
        };

        Connection connection = new("0", testUserAccount1, testUserAccount2, ConnectionStatus.Connected);

        List<Message> messages = new()
        {
            new TestMessage(conversationDocument.Messages[0],testUserAccount1,new(2023,11,6,0,0,0,TimeSpan.Zero),null),
            new TestMessage(conversationDocument.Messages[1],testUserAccount2,new(2023,11,6,0,0,0,TimeSpan.Zero),null),
        };

        // When
        var result = (ConnectionConversation)_conversationDocumentToModelMapperSUT.MapConversationFromDocument(conversationDocument,connection,null,messages);

        // Then
        result.ConversationId.Should().Be(conversationDocument.Id);
        result.Messages.Should().BeEquivalentTo(messages);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public void MapConversationFromDocument_WhenDocumentIsValidAndIsGroupConversation_ReturnMappedConversation()
    {
        // Given
        ConversationDocument conversationDocument = new(
            Id: "myCoversationID",
            ConnectionId: null,
            GroupId: "myGroupID",
            Messages: new()
            {
                "message1",
                "message2",
            }
        );

        TestUserAccount testUserAccount1 = new()
        {
            Id = "user1",
            Handler = "handler1",
            UserName = "username1"
        };

        TestUserAccount testUserAccount2 = new()
        {
            Id = "user2",
            Handler = "handler2",
            UserName = "username2"
        };

        Group group = new(
            "groupID",
            "groupName",
            "Group Description",
            new List<IUserAccount>() { testUserAccount1, testUserAccount2 });

        List<Message> messages = new()
        {
            new TestMessage(conversationDocument.Messages[0],testUserAccount1,new(2023,11,6,0,0,0,TimeSpan.Zero),null),
            new TestMessage(conversationDocument.Messages[1],testUserAccount2,new(2023,11,6,0,0,0,TimeSpan.Zero),null),
        };

        // When
        var result = (GroupConversation)_conversationDocumentToModelMapperSUT.MapConversationFromDocument(conversationDocument, null, group, messages);

        // Then
        result.ConversationId.Should().Be(conversationDocument.Id);
        result.Messages.Should().BeEquivalentTo(messages);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public void MapConversationFromDocument_WhenConversationDocumentIdIsNull_ThrowsArgumentException()
    {
        // Given
        ConversationDocument conversationDocument = new(
            Id: null,
            ConnectionId: null,
            GroupId: null,
            Messages: new()
        );

        List<Message> messages = new();

        // When
        var testAction = () => _conversationDocumentToModelMapperSUT.MapConversationFromDocument(conversationDocument, null, null, messages);

        // Then
        testAction.Should().Throw<ArgumentException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public void MapConversationFromDocument_WhenConnectionAndGroupAreNull_ThrowsMalformedConversationDocumentException()
    {
        // Given
        ConversationDocument conversationDocument = new(
            Id: "myCoversationID",
            ConnectionId: null,
            GroupId: null,
            Messages: new()
        );

        // When
        var testAction = () => _conversationDocumentToModelMapperSUT.MapConversationFromDocument(conversationDocument, null, null, new List<Message>());

        // Then
        testAction.Should().Throw<MalformedConversationDocumentException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.INFRASTRUCTURE)]
    public void MapConversationFromDocument_WhenConnectionAndGroupAreNotNull_ThrowsMalformedConversationDocumentException()
    {
        // Given
        ConversationDocument conversationDocument = new(
            Id: "myCoversationID",
            ConnectionId: "myConnectionID",
            GroupId: "myGroupID",
            Messages: new()
        );

        TestUserAccount testUserAccount1 = new()
        {
            Id = "user1",
            Handler = "handler1",
            UserName = "username1"
        };

        TestUserAccount testUserAccount2 = new()
        {
            Id = "user2",
            Handler = "handler2",
            UserName = "username2"
        };

        Group group = new(
            "groupID",
            "groupName",
            "Group Description",
            new List<IUserAccount>() { testUserAccount1, testUserAccount2 });

        Connection connection = new("0", testUserAccount1, testUserAccount2, ConnectionStatus.Connected);

        // When
        var testAction = () => _conversationDocumentToModelMapperSUT.MapConversationFromDocument(conversationDocument, connection, group, new List<Message>());

        // Then
        testAction.Should().Throw<MalformedConversationDocumentException>();
    }

}
