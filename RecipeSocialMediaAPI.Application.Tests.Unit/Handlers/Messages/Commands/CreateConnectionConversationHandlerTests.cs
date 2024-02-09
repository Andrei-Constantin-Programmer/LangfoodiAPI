using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
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
    private readonly Mock<IConnectionQueryRepository> _connectionQueryRepositoryMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;

    private readonly CreateConnectionConversationHandler _connectionConversationHandlerSUT;

    public CreateConnectionConversationHandlerTests()
    {
        _conversationPersistenceRepositoryMock = new Mock<IConversationPersistenceRepository>();
        _connectionQueryRepositoryMock = new Mock<IConnectionQueryRepository>();
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();

        _connectionConversationHandlerSUT = new(_conversationPersistenceRepositoryMock.Object, _connectionQueryRepositoryMock.Object, _userQueryRepositoryMock.Object);
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
            .Setup(repo => repo.GetUserById(userAccount1.Id))
            .Returns(new TestUserCredentials() { Account = userAccount1, Email = "test@mail.com", Password = "Test@123" });

        Connection connection = new(
            connectionId: "connection1",
            account1: userAccount1,
            account2: userAccount2,
            status: ConnectionStatus.Connected
        );

        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnection(connection.ConnectionId))
            .Returns(connection);

        ConnectionConversation expectedConversation = new(connection, "conversation1");

        _conversationPersistenceRepositoryMock
            .Setup(repo => repo.CreateConnectionConversation(connection))
            .Returns(expectedConversation);

        // When
        var result = await _connectionConversationHandlerSUT.Handle(new CreateConnectionConversationCommand(userAccount1.Id, connection.ConnectionId), CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(expectedConversation.ConversationId);
        result.ConnectionOrGroupId.Should().Be(connection.ConnectionId);
        result.IsGroup.Should().BeFalse();
        result.ThumbnailId.Should().Be(userAccount2.ProfileImageId);
        result.LastMessage.Should().BeNull();
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
            .Setup(repo => repo.GetUserById(userAccount1.Id))
            .Returns(new TestUserCredentials() { Account = userAccount1, Email = "test@mail.com", Password = "Test@123" });

        Connection connection = new(
            connectionId: "connection1",
            account1: userAccount1,
            account2: userAccount2,
            status: ConnectionStatus.Connected
        );

        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnection(connection.ConnectionId))
            .Returns(connection);

        ConnectionConversation expectedConversation = new(connection, "conversation1");

        _conversationPersistenceRepositoryMock
            .Setup(repo => repo.CreateConnectionConversation(connection))
            .Returns(expectedConversation);

        // When
        var testAction = async() => await _connectionConversationHandlerSUT.Handle(new CreateConnectionConversationCommand(userAccount1.Id, "invalidId"), CancellationToken.None);

        // Then
        await testAction
            .Should().ThrowAsync<ConnectionNotFoundException>();
    }
}
