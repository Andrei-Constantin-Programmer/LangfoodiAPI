using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Commands;

public class UpdateConnectionHandlerTests
{
    private readonly Mock<IConnectionQueryRepository> _connectionQueryRepositoryMock;
    private readonly Mock<IConnectionPersistenceRepository> _connectionPersistenceRepositoryMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;

    private readonly UpdateConnectionHandler _updateConnectionHandlerSUT;

    public UpdateConnectionHandlerTests()
    {
        _connectionQueryRepositoryMock = new Mock<IConnectionQueryRepository>();
        _connectionPersistenceRepositoryMock = new Mock<IConnectionPersistenceRepository>();
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();

        _updateConnectionHandlerSUT = new(_connectionQueryRepositoryMock.Object, _connectionPersistenceRepositoryMock.Object, _userQueryRepositoryMock.Object);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData("Blocked", ConnectionStatus.Blocked)]
    [InlineData("Muted", ConnectionStatus.Muted)]
    [InlineData("Connected", ConnectionStatus.Connected)]
    [InlineData("Favourite", ConnectionStatus.Favourite)]
    public async Task Handle_WhenUpdateContractIsValidAndUpdateIsSuccessful_UpdatesTheConnectionAndDoesNotThrow(string newConnectionStatus, ConnectionStatus connectionStatusEnumValue)
    {
        // Given
        TestUserCredentials user1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "UserId1",
                Handler = "user1",
                UserName = "Username 1",
                AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user1@mail.com",
            Password = "TestPass"
        };

        TestUserCredentials user2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "UserId2",
                Handler = "user2",
                UserName = "Username 2",
                AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user2@mail.com",
            Password = "TestPass"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(user1.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user1);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(user2.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user2);

        Connection existingConnection = new("0", user1.Account, user2.Account, ConnectionStatus.Pending);

        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnection(
                It.Is<IUserAccount>(acc => acc == user1.Account || acc == user2.Account), 
                It.Is<IUserAccount>(acc => acc == user1.Account || acc == user2.Account), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingConnection);

        _connectionPersistenceRepositoryMock
            .Setup(repo => repo.UpdateConnection(It.Is<IConnection>(
                connection => (connection.Account1 == user1.Account && connection.Account2 == user2.Account)
                           || (connection.Account1 == user2.Account && connection.Account2 == user1.Account))))
            .Returns(true);

        UpdateConnectionContract contract = new(user1.Account.Id, user2.Account.Id, newConnectionStatus);

        // When
        var testAction = async () => await _updateConnectionHandlerSUT.Handle(new UpdateConnectionCommand(contract), CancellationToken.None);

        // Then
        await testAction.Should().NotThrowAsync();
        _connectionPersistenceRepositoryMock
            .Verify(repo => repo.UpdateConnection(It.Is<IConnection>(connection => connection.Status == connectionStatusEnumValue)), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenContractIsValidButUpdateIsUnsuccessful_ThrowsConnectionUpdateException()
    {
        // Given
        TestUserCredentials user1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "UserId1",
                Handler = "user1",
                UserName = "Username 1",
                AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user1@mail.com",
            Password = "TestPass"
        };

        TestUserCredentials user2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "UserId2",
                Handler = "user2",
                UserName = "Username 2",
                AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user2@mail.com",
            Password = "TestPass"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(user1.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user1);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(user2.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user2);

        Connection existingConnection = new("0", user1.Account, user2.Account, ConnectionStatus.Pending);

        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnection(
                It.Is<IUserAccount>(acc => acc == user1.Account || acc == user2.Account),
                It.Is<IUserAccount>(acc => acc == user1.Account || acc == user2.Account), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingConnection);

        _connectionPersistenceRepositoryMock
            .Setup(repo => repo.UpdateConnection(It.IsAny<IConnection>()))
            .Returns(false);

        UpdateConnectionContract contract = new(user1.Account.Id, user2.Account.Id, "Connected");

        // When
        var testAction = async () => await _updateConnectionHandlerSUT.Handle(new UpdateConnectionCommand(contract), CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<ConnectionUpdateException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUpdateContractIsValidButConnectionStatusHasNotChanged_DoesNotUpdateTheConnectionAndDoesNotThrow()
    {
        // Given
        TestUserCredentials user1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "UserId1",
                Handler = "user1",
                UserName = "Username 1",
                AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user1@mail.com",
            Password = "TestPass"
        };

        TestUserCredentials user2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "UserId2",
                Handler = "user2",
                UserName = "Username 2",
                AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user2@mail.com",
            Password = "TestPass"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(user1.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user1);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(user2.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user2);

        Connection existingConnection = new("0", user1.Account, user2.Account, ConnectionStatus.Pending);

        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnection(
                It.Is<IUserAccount>(acc => acc == user1.Account || acc == user2.Account),
                It.Is<IUserAccount>(acc => acc == user1.Account || acc == user2.Account), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingConnection);

        UpdateConnectionContract contract = new(user1.Account.Id, user2.Account.Id, "Pending");

        // When
        var testAction = async () => await _updateConnectionHandlerSUT.Handle(new UpdateConnectionCommand(contract), CancellationToken.None);

        // Then
        await testAction.Should().NotThrowAsync();
        _connectionPersistenceRepositoryMock
            .Verify(repo => repo.UpdateConnection(It.IsAny<IConnection>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenConnectionStatusIsInvalid_DoesNotUpdateAndThrowsConnectionUpdateException()
    {
        // Given
        TestUserCredentials user1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "UserId1",
                Handler = "user1",
                UserName = "Username 1",
                AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user1@mail.com",
            Password = "TestPass"
        };

        TestUserCredentials user2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "UserId2",
                Handler = "user2",
                UserName = "Username 2",
                AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user2@mail.com",
            Password = "TestPass"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(user1.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user1);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(user2.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user2);

        Connection existingConnection = new("0", user1.Account, user2.Account, ConnectionStatus.Pending);

        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnection(
                It.Is<IUserAccount>(acc => acc == user1.Account || acc == user2.Account),
                It.Is<IUserAccount>(acc => acc == user1.Account || acc == user2.Account), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingConnection);

        UpdateConnectionContract contract = new(user1.Account.Id, user2.Account.Id, "Unsupported Status");

        // When
        var testAction = async () => await _updateConnectionHandlerSUT.Handle(new UpdateConnectionCommand(contract), CancellationToken.None);

        // Then
        await testAction.Should()
            .ThrowAsync<UnsupportedConnectionStatusException>();
        _connectionPersistenceRepositoryMock
            .Verify(repo => repo.UpdateConnection(It.IsAny<IConnection>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenConnectionDoesNotExist_DoesNotUpdateAndThrowConnectionNotFoundException()
    {
        // Given
        TestUserCredentials user1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "UserId1",
                Handler = "user1",
                UserName = "Username 1",
                AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user1@mail.com",
            Password = "TestPass"
        };

        TestUserCredentials user2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "UserId2",
                Handler = "user2",
                UserName = "Username 2",
                AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user2@mail.com",
            Password = "TestPass"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(user1.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user1);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(user2.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user2);

        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnection(
                It.IsAny<IUserAccount>(),
                It.IsAny<IUserAccount>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IConnection?)null);

        UpdateConnectionContract contract = new(user1.Account.Id, user2.Account.Id, "Connected");

        // When
        var testAction = async () => await _updateConnectionHandlerSUT.Handle(new UpdateConnectionCommand(contract), CancellationToken.None);

        // Thens
        await testAction.Should().ThrowAsync<ConnectionNotFoundException>();
        _connectionPersistenceRepositoryMock
            .Verify(repo => repo.UpdateConnection(It.IsAny<IConnection>()), Times.Never);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public async Task Handle_WhenUsersDoNotExist_DoesNotUpdateAndThrowsUserNotFoundException(bool user1Exists, bool user2Exists)
    {
        // Given
        TestUserCredentials user1 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "UserId1",
                Handler = "user1",
                UserName = "Username 1",
                AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user1@mail.com",
            Password = "TestPass"
        };

        TestUserCredentials user2 = new()
        {
            Account = new TestUserAccount()
            {
                Id = "UserId2",
                Handler = "user2",
                UserName = "Username 2",
                AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user2@mail.com",
            Password = "TestPass"
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(user1.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user1Exists ? user1 : null);
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(user2.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user2Exists ? user2 : null);

        UpdateConnectionContract contract = new(user1.Account.Id, user2.Account.Id, "Connected");

        // When
        var testAction = async () => await _updateConnectionHandlerSUT.Handle(new UpdateConnectionCommand(contract), CancellationToken.None);

        // Then
        await testAction.Should()
            .ThrowAsync<UserNotFoundException>()
            .WithMessage($"No user found with id {(!user1Exists ? user1.Account.Id : user2.Account.Id)}");

        _connectionPersistenceRepositoryMock
            .Verify(repo => repo.UpdateConnection(It.IsAny<IConnection>()), Times.Never);
    }
}
