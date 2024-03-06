using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Queries;

public class GetConversationByConnectionHandlerTests
{
    private readonly Mock<IConversationQueryRepository> _conversationQueryRepositoryMock;
    private readonly Mock<IConversationMapper> _conversationMapperMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;

    private readonly GetConversationByConnectionHandler _getConversationByConnectionHandlerSUT;

    public GetConversationByConnectionHandlerTests()
    {
        _conversationQueryRepositoryMock = new Mock<IConversationQueryRepository>();
        _conversationMapperMock = new Mock<IConversationMapper>();
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();

        _getConversationByConnectionHandlerSUT = new(_conversationQueryRepositoryMock.Object, _conversationMapperMock.Object, _userQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenThereAreNoMessages_ReturnConversationDtoWithNullLastMessage()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "u1",
            Handler = "user1",
            UserName = "User 1"
        };
        TestUserAccount user2 = new()
        {
            Id = "u2",
            Handler = "user2",
            UserName = "User 2",
            ProfileImageId = "img.png"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(user1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestUserCredentials()
            {
                Account = user1,
                Email = "user1@mail.com",
                Password = "Test@123"
            });

        Connection connection = new("conn1", user1, user2, ConnectionStatus.Pending);
        ConnectionConversation conversation = new(connection, "convo1", new List<Message>());

        _conversationQueryRepositoryMock
            .Setup(repo => repo.GetConversationByConnectionAsync(connection.ConnectionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        ConversationDTO conversationDto = new(conversation.ConversationId, connection.ConnectionId, false, user2.UserName, user2.ProfileImageId, null, new() { user1.Id, user2.Id });
        _conversationMapperMock
            .Setup(mapper => mapper.MapConversationToConnectionConversationDTO(user1, conversation))
            .Returns(conversationDto);

        GetConversationByConnectionQuery query = new(user1.Id, connection.ConnectionId);

        // When
        var result = await _getConversationByConnectionHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.ConnectionOrGroupId.Should().Be(connection.ConnectionId);
        result.IsGroup.Should().BeFalse();
        result.ThumbnailId.Should().Be(user2.ProfileImageId);
        result.Id.Should().Be(conversation.ConversationId);
        result.LastMessage.Should().BeNull();
        result.UserIds.Should().BeEquivalentTo(conversationDto.UserIds);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenThereAreMessages_ReturnConversationDtoWithTheLastMessage()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "u1",
            Handler = "user1",
            UserName = "User 1"
        };
        TestUserAccount user2 = new()
        {
            Id = "u2",
            Handler = "user2",
            UserName = "User 2"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(user1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestUserCredentials()
            {
                Account = user1,
                Email = "user1@mail.com",
                Password = "Test@123"
            });

        MessageDTO lastMessageDto = new("3", new(user2.Id, user1.UserName), new(), new(2023, 1, 1, 14, 35, 0, TimeSpan.Zero));

        List<Message> messages = new()
        {
            new TestMessage("1", user1, new(2023, 1, 1, 12, 45, 0, TimeSpan.Zero), null),
            new TestMessage("2", user2, new(2023, 1, 1, 13, 30, 0, TimeSpan.Zero), null),
            new TestMessage(lastMessageDto.Id, user2, lastMessageDto.SentDate!.Value, null),
            new TestMessage("4", user1, new(2023, 1, 1, 14, 10, 0, TimeSpan.Zero), null),
            new TestMessage("5", user1, new(2023, 1, 1, 14, 15, 0, TimeSpan.Zero), null),
        };

        Connection connection = new("conn1", user1, user2, ConnectionStatus.Pending);
        ConnectionConversation conversation = new(connection, "convo1", messages);

        ConversationDTO conversationDto = new(conversation.ConversationId, connection.ConnectionId, false, user2.UserName, user2.ProfileImageId, lastMessageDto, new() { user1.Id, user2.Id });
        _conversationMapperMock
            .Setup(mapper => mapper.MapConversationToConnectionConversationDTO(user1, conversation))
            .Returns(conversationDto);

        _conversationQueryRepositoryMock
            .Setup(repo => repo.GetConversationByConnectionAsync(connection.ConnectionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        GetConversationByConnectionQuery query = new(user1.Id, connection.ConnectionId);

        // When
        var result = await _getConversationByConnectionHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.ConnectionOrGroupId.Should().Be(connection.ConnectionId);
        result.Id.Should().Be(conversation.ConversationId);
        result.LastMessage.Should().Be(lastMessageDto);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenThereIsNoConversationFound_ThrowConversationNotFoundException()
    {
        // Given
        TestUserAccount user1 = new()
        {
            Id = "u1",
            Handler = "user1",
            UserName = "User 1"
        };
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(user1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestUserCredentials()
            {
                Account = user1,
                Email = "user1@mail.com",
                Password = "Test@123"
            });

        string connectionId = "conn1";
        _conversationQueryRepositoryMock
            .Setup(repo => repo.GetConversationByConnectionAsync(connectionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConnectionConversation?)null);

        GetConversationByConnectionQuery query = new(user1.Id, connectionId);

        // When
        var testAction = async () => await _getConversationByConnectionHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<ConversationNotFoundException>();
    }
}
