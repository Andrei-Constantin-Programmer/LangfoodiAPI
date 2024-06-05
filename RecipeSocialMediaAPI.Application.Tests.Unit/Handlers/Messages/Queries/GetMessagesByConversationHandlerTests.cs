using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Queries;

public class GetMessagesByConversationHandlerTests
{
    private readonly Mock<IConversationQueryRepository> _conversationQueryRepositoryMock;
    private readonly Mock<IMessageMapper> _messageMapperMock;

    private readonly GetMessagesByConversationHandler _getMessagesByConversationHandlerSUT;

    public GetMessagesByConversationHandlerTests()
    {
        _conversationQueryRepositoryMock = new Mock<IConversationQueryRepository>();
        _messageMapperMock = new Mock<IMessageMapper>();

        _getMessagesByConversationHandlerSUT = new(_conversationQueryRepositoryMock.Object, _messageMapperMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenConversationExistsAndThereAreNoMessages_ReturnEmptyList()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "u1",
            Handler = "user_1",
            UserName = "User 1"
        };
        TestUserAccount user2 = new()
        {
            Id = "u2",
            Handler = "user_2",
            UserName = "User 2"
        };

        Connection connection = new("conn1", user1, user2, ConnectionStatus.Connected);
        ConnectionConversation conversation = new(connection, "convo1", new List<Message>());
        _conversationQueryRepositoryMock
            .Setup(repo => repo.GetConversationByIdAsync(conversation.ConversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        GetMessagesByConversationQuery query = new(conversation.ConversationId);

        // When
        var result = await _getMessagesByConversationHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenConversationExistsAndThereAreMessages_ReturnListWithMappedMessages()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "u1",
            Handler = "user_1",
            UserName = "User 1"
        };
        TestUserAccount user2 = new()
        {
            Id = "u2",
            Handler = "user_2",
            UserName = "User 2"
        };

        List<Message> messages = new()
        {
            new TestMessage("m1", user1, new(2024, 1, 1, 12, 45, 01, TimeSpan.Zero), null),
            new TestMessage("m2", user2, new(2024, 1, 1, 12, 47, 12, TimeSpan.Zero), null),
            new TestMessage("m3", user1, new(2024, 1, 2, 13, 32, 15, TimeSpan.Zero), null),
            new TestMessage("m4", user2, new(2024, 1, 2, 13, 35, 42, TimeSpan.Zero), null),
        };
        List<MessageDto> expectedDtos = new()
        {
            new(messages[0].Id, new(messages[0].Sender.Id, messages[0].Sender.UserName), new(), messages[0].SentDate),
            new(messages[1].Id, new(messages[1].Sender.Id, messages[1].Sender.UserName), new(), messages[1].SentDate),
            new(messages[2].Id, new(messages[2].Sender.Id, messages[2].Sender.UserName), new(), messages[2].SentDate),
            new(messages[3].Id, new(messages[3].Sender.Id, messages[3].Sender.UserName), new(), messages[3].SentDate)
        };

        _messageMapperMock
            .Setup(mapper => mapper.MapMessageToMessageDTO(messages[0]))
            .Returns(expectedDtos[0]);
        _messageMapperMock
            .Setup(mapper => mapper.MapMessageToMessageDTO(messages[1]))
            .Returns(expectedDtos[1]);
        _messageMapperMock
            .Setup(mapper => mapper.MapMessageToMessageDTO(messages[2]))
            .Returns(expectedDtos[2]);
        _messageMapperMock
            .Setup(mapper => mapper.MapMessageToMessageDTO(messages[3]))
            .Returns(expectedDtos[3]);

        Connection connection = new("conn1", user1, user2, ConnectionStatus.Connected);
        ConnectionConversation conversation = new(connection, "convo1", messages);
        _conversationQueryRepositoryMock
            .Setup(repo => repo.GetConversationByIdAsync(conversation.ConversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        GetMessagesByConversationQuery query = new(conversation.ConversationId);

        // When
        var result = await _getMessagesByConversationHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        result.Should().HaveCount(4);
        result.Should().BeEquivalentTo(expectedDtos);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenConversationDoesNotExist_ThrowConversationNotFoundException()
    {
        // Given
        _conversationQueryRepositoryMock
            .Setup(repo => repo.GetConversationByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conversation?)null);

        GetMessagesByConversationQuery query = new("convo0");

        // When
        var testAction = async() => await _getMessagesByConversationHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        await testAction.Should()
            .ThrowAsync<ConversationNotFoundException>()
            .WithMessage($"*{query.ConversationId}*");
    }
}
