using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Commands;

public class CreateConnectionConversationHandlerTests
{
    private readonly Mock<IConversationPersistenceRepository> _conversationPersistenceRepositoryMock;
    private readonly Mock<IConversationMapper> _conversationMapperMock;
    private readonly Mock<IConnectionQueryRepository> _connectionQueryRepositoryMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;

    private readonly CreateConnectionConversationHandler _connectionConversationHandlerSUT;

    public CreateConnectionConversationHandlerTests()
    {
        _conversationPersistenceRepositoryMock = new Mock<IConversationPersistenceRepository>();
        _conversationMapperMock = new Mock<IConversationMapper>();
        _connectionQueryRepositoryMock = new Mock<IConnectionQueryRepository>();
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();

        _connectionConversationHandlerSUT = new(_conversationPersistenceRepositoryMock.Object, _conversationMapperMock.Object, _connectionQueryRepositoryMock.Object, _userQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenConnectionIdMatches_CreateAndReturnConnectionConversation()
    {
        // Given
        TestUserAccount userAccount1 = new()
        {
            Id = "user1",
            Handler = "user1",
            UserName = "UserName 1",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount userAccount2 = new()
        {
            Id = "user2",
            Handler = "user2",
            UserName = "UserName 2",
            AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero),
            ProfileImageId = "img.png"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(userAccount1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestUserCredentials() { Account = userAccount1, Email = "test@mail.com", Password = "Test@123" });

        Connection connection = new(
            connectionId: "connection1",
            account1: userAccount1,
            account2: userAccount2,
            status: ConnectionStatus.Connected
        );

        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnectionAsync(connection.ConnectionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(connection);

        ConnectionConversation expectedConversation = new(connection, "conversation1");

        _conversationPersistenceRepositoryMock
            .Setup(repo => repo.CreateConnectionConversationAsync(connection, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedConversation);

        ConversationDTO conversationDto = new(expectedConversation.ConversationId, connection.ConnectionId, false, userAccount2.UserName, userAccount2.ProfileImageId, null, new() { userAccount1.Id, userAccount2.Id });
        _conversationMapperMock
            .Setup(mapper => mapper.MapConversationToConnectionConversationDTO(userAccount1, expectedConversation))
            .Returns(conversationDto);

        // When
        var result = await _connectionConversationHandlerSUT.Handle(new CreateConnectionConversationCommand(userAccount1.Id, connection.ConnectionId), CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(expectedConversation.ConversationId);
        result.ConnectionOrGroupId.Should().Be(connection.ConnectionId);
        result.IsGroup.Should().BeFalse();
        result.ThumbnailId.Should().Be(userAccount2.ProfileImageId);
        result.LastMessage.Should().BeNull();
        result.MessagesUnseen.Should().Be(0);
        result.UserIds.Should().BeEquivalentTo(conversationDto.UserIds);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenConnectionIdIsNotFound_ThrowArgumentException()
    {
        // Given
        TestUserAccount userAccount1 = new()
        {
            Id = "user1",
            Handler = "user1",
            UserName = "UserName 1",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        TestUserAccount userAccount2 = new()
        {
            Id = "user2",
            Handler = "user2",
            UserName = "UserName 2",
            AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
        };
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(userAccount1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestUserCredentials() { Account = userAccount1, Email = "test@mail.com", Password = "Test@123" });

        Connection connection = new(
            connectionId: "connection1",
            account1: userAccount1,
            account2: userAccount2,
            status: ConnectionStatus.Connected
        );

        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnectionAsync(connection.ConnectionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(connection);

        ConnectionConversation expectedConversation = new(connection, "conversation1");

        _conversationPersistenceRepositoryMock
            .Setup(repo => repo.CreateConnectionConversationAsync(connection, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedConversation);

        // When
        var testAction = async() => await _connectionConversationHandlerSUT.Handle(new CreateConnectionConversationCommand(userAccount1.Id, "invalidId"), CancellationToken.None);

        // Then
        await testAction
            .Should().ThrowAsync<ConnectionNotFoundException>();
    }
}
