using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Queries;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.Domain.Models.Messaging;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Conversations;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Queries;

public class GetConnectionsByUserHandlerTests
{
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;
    private readonly Mock<IConnectionQueryRepository> _connectionQueryRepositoryMock;

    private readonly GetConnectionsByUserHandler _getConnectionsByUserHandlerSUT;

    public GetConnectionsByUserHandlerTests()
    {
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();
        _connectionQueryRepositoryMock = new Mock<IConnectionQueryRepository>();

        _getConnectionsByUserHandlerSUT = new(_userQueryRepositoryMock.Object, _connectionQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserHasNoConversations_ReturnsEmptyCollection()
    {
        // Given
        TestUserCredentials testUser = new()
        {
            Account = new TestUserAccount()
            {
                Id = "UserId",
                Handler = "handle",
                UserName = "User",
                AccountCreationDate = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user@mail.com",
            Password = "password",
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(testUser.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testUser);

        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnectionsForUser(It.IsAny<IUserAccount>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<IConnection>());

        GetConnectionsByUserQuery query = new(testUser.Account.Id);

        // When
        var result = await _getConnectionsByUserHandlerSUT.Handle(query, CancellationToken.None);

        // Then
        result.Should().BeEmpty();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenUserHasConversations_ReturnsMappedConversations()
    {
        // Given
        TestUserCredentials testUser = new()
        {
            Account = new TestUserAccount()
            {
                Id = "UserId",
                Handler = "handle",
                UserName = "User",
                AccountCreationDate = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            Email = "user@mail.com",
            Password = "password",
        };

        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserById(testUser.Account.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testUser);

        Connection connection1 = new(
            "0",
            testUser.Account, 
            new TestUserAccount()
            {
                Id = "User2",
                Handler = "handle2",
                UserName = "User2",
                AccountCreationDate = new(2024, 2, 2, 0, 0, 0, TimeSpan.Zero)
            }, 
            ConnectionStatus.Pending);
        
        Connection connection2 = new(
            "1",
            testUser.Account, 
            new TestUserAccount()
            {
                Id = "User3",
                Handler = "handle3",
                UserName = "User3",
                AccountCreationDate = new(2024, 3, 3, 0, 0, 0, TimeSpan.Zero)
            }, 
            ConnectionStatus.Connected);

        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnectionsForUser(testUser.Account, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<IConnection>() { connection1, connection2 });

        GetConnectionsByUserQuery query = new(testUser.Account.Id);

        // When
        var result = (await _getConnectionsByUserHandlerSUT.Handle(query, CancellationToken.None)).ToList();

        // Then
        result.Should().HaveCount(2);

        result[0].ConnectionId.Should().Be(connection1.ConnectionId);
        result[0].UserId1.Should().Be(connection1.Account1.Id);
        result[0].UserId2.Should().Be(connection1.Account2.Id);
        result[0].ConnectionStatus.Should().Be("Pending");

        result[1].ConnectionId.Should().Be(connection2.ConnectionId);
        result[1].UserId1.Should().Be(connection2.Account1.Id);
        result[1].UserId2.Should().Be(connection2.Account2.Id);
        result[1].ConnectionStatus.Should().Be("Connected");
    }
}
