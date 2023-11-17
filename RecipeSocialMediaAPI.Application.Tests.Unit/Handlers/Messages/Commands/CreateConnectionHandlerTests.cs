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

public class CreateConnectionHandlerTests
{
    private readonly Mock<IConnectionPersistenceRepository> _connectionPersistenceRepositoryMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;

    private readonly CreateConnectionHandler _connectionHandlerSUT;

    public CreateConnectionHandlerTests()
    {
        _connectionPersistenceRepositoryMock = new Mock<IConnectionPersistenceRepository>();
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();

        _connectionHandlerSUT = new(_connectionPersistenceRepositoryMock.Object, _userQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUsersExist_CreateAndReturnConnectionWithPendingStatus()
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
            .Returns(new TestUserCredentials() { Account = userAccount1, Email = "test1@mail.com", Password = "TestPass" });
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(userAccount2.Id))
            .Returns(new TestUserCredentials() { Account = userAccount2, Email = "test2@mail.com", Password = "TestPass" });

        NewConnectionContract testContract = new(userAccount1.Id, userAccount2.Id);

        Connection testConnection = new(userAccount1, userAccount2, ConnectionStatus.Pending);

        _connectionPersistenceRepositoryMock
            .Setup(repo => repo.CreateConnection(userAccount1, userAccount2, ConnectionStatus.Pending))
            .Returns(testConnection);

        // When
        var result = await _connectionHandlerSUT.Handle(new CreateConnectionCommand(testContract), CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.UserId1.Should().Be(testContract.UserId1);
        result.UserId2.Should().Be(testContract.UserId2);
        result.ConnectionStatus.Should().Be("Pending");
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task Handle_WhenUserDoesNotExist_ThrowsUserNotFoundException(bool user1Exists, bool user2Exists)
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

        if (user1Exists)
        {
            _userQueryRepositoryMock
                .Setup(repo => repo.GetUserById(userAccount1.Id))
                .Returns(new TestUserCredentials() { Account = userAccount1, Email = "test1@mail.com", Password = "TestPass" });
        }
        if (user2Exists) 
        {
            _userQueryRepositoryMock
                .Setup(repo => repo.GetUserById(userAccount2.Id))
                .Returns(new TestUserCredentials() { Account = userAccount2, Email = "test2@mail.com", Password = "TestPass" });
        }

        NewConnectionContract testContract = new(userAccount1.Id, userAccount2.Id);

        Connection testConnection = new(userAccount1, userAccount2, ConnectionStatus.Pending);

        _connectionPersistenceRepositoryMock
            .Setup(repo => repo.CreateConnection(userAccount1, userAccount2, ConnectionStatus.Pending))
            .Returns(testConnection);

        // When
        var testAction = async () => await _connectionHandlerSUT.Handle(new CreateConnectionCommand(testContract), CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<UserNotFoundException>();
    }
}
