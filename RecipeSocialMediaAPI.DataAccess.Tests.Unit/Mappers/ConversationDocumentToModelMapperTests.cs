using FluentAssertions;
using RecipeSocialMediaAPI.DataAccess.Exceptions;
using RecipeSocialMediaAPI.DataAccess.Mappers;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Mappers;

public class ConversationDocumentToModelMapperTests
{
    private readonly ConversationDocumentToModelMapper _conversationDocumentToModelMapperSUT;

    public ConversationDocumentToModelMapperTests()
    {
        _conversationDocumentToModelMapperSUT = new ConversationDocumentToModelMapper();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void MapConversationFromDocument_WhenDocumentIsValidAndIsConnectionConversation_ReturnMappedConversation()
    {
        // Given
        ConversationDocument conversationDocument = new()
        {
            Id = "myCoversationID",
            ConnectionId = "myConnectionID",
            GroupId = null,
            Messages = new()
            {
                "message1",
                "message2",
            }
        };

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

        Connection connection = new(testUserAccount1, testUserAccount2, ConnectionStatus.Connected);

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

}
