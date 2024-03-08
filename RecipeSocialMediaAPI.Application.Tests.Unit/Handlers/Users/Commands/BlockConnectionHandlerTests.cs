using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Users.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Users.Commands;

public class BlockConnectionHandlerTests
{
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;
    private readonly Mock<IUserPersistenceRepository> _userPersistenceRepositoryMock;
    private readonly Mock<IConnectionQueryRepository> _connectionQueryRepositoryMock;

    private readonly BlockConnectionHandler _blockConnectionHandlerSUT;

    public BlockConnectionHandlerTests()
    {
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();
        _userPersistenceRepositoryMock = new Mock<IUserPersistenceRepository>();
        _connectionQueryRepositoryMock = new Mock<IConnectionQueryRepository>();

        _blockConnectionHandlerSUT = new(
            _userQueryRepositoryMock.Object,
            _userPersistenceRepositoryMock.Object,
            _connectionQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.DOMAIN, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserDoesNotExist_ThrowUserNotFoundException()
    {
        // Given
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IUserCredentials?)null);

        BlockConnectionCommand command = new("nonExistantUserId", "conn1");

        // When
        var testAction = async () => await _blockConnectionHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<UserNotFoundException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.DOMAIN, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenConnectionDoesNotExist_ThrowConnectionNotFoundException()
    {
        // Given
        TestUserCredentials user = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u1",
                UserName = "User 1",
                Handler = "user_1"
            },
            Email = "user1@mail.com",
            Password = "Pass@123"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(user.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        BlockConnectionCommand command = new(user.Account.Id, "conn1");

        // When
        var testAction = async () => await _blockConnectionHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<ConnectionNotFoundException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.DOMAIN, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenBlockIsSuccessful_UpdateUser()
    {
        // Given
        TestUserCredentials user1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u1",
                UserName = "User 1",
                Handler = "user_1"
            },
            Email = "user1@mail.com",
            Password = "Pass@123"
        };
        TestUserCredentials user2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u2",
                UserName = "User 2",
                Handler = "user_2"
            },
            Email = "user2@mail.com",
            Password = "Pass@123"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(user1.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user1);

        Connection connection = new("conn1", user1.Account, user2.Account, ConnectionStatus.Connected);

        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnectionAsync(connection.ConnectionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(connection);

        BlockConnectionCommand command = new(user1.Account.Id, connection.ConnectionId);

        // When
        await _blockConnectionHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        user1.Account.Id.Should().Be("u1");
        user1.Account.UserName.Should().Be("User 1");
        user1.Account.Handler.Should().Be("user_1");
        user1.Email.Should().Be("user1@mail.com");
        user1.Password.Should().Be("Pass@123");
        user1.Account.BlockedConnectionIds.Should().Contain(connection.ConnectionId);
        _userPersistenceRepositoryMock
            .Verify(repo => repo.UpdateUserAsync(user1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.USER)]
    [Trait(Traits.DOMAIN, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenBlockIsUnsuccessful_DoNotUpdateUser()
    {
        // Given
        TestUserCredentials user1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u1",
                UserName = "User 1",
                Handler = "user_1"
            },
            Email = "user1@mail.com",
            Password = "Pass@123"
        };
        TestUserCredentials user2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "u2",
                UserName = "User 2",
                Handler = "user_2"
            },
            Email = "user2@mail.com",
            Password = "Pass@123"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(user1.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user1);

        Connection connection = new("conn1", user1.Account, user2.Account, ConnectionStatus.Connected);
        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnectionAsync(connection.ConnectionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(connection);

        user1.Account.BlockConnection(connection.ConnectionId);

        BlockConnectionCommand command = new(user1.Account.Id, connection.ConnectionId);

        // When
        await _blockConnectionHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        user1.Account.Id.Should().Be("u1");
        user1.Account.UserName.Should().Be("User 1");
        user1.Account.Handler.Should().Be("user_1");
        user1.Email.Should().Be("user1@mail.com");
        user1.Password.Should().Be("Pass@123");
        user1.Account.BlockedConnectionIds.Should().Contain(connection.ConnectionId);
        _userPersistenceRepositoryMock
            .Verify(repo => repo.UpdateUserAsync(user1, It.IsAny<CancellationToken>()), Times.Never);
    }
}
