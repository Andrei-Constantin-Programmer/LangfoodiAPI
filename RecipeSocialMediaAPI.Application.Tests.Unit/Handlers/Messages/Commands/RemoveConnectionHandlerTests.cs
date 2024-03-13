using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Connections;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Commands;

public class RemoveConnectionHandlerTests
{
    private readonly Mock<IConnectionPersistenceRepository> _connectionPersistenceRepositoryMock;
    private readonly Mock<IConnectionQueryRepository> _connectionQueryRepositoryMock;

    private readonly RemoveConnectionHandler _removeConnectionHandlerSUT;

    public RemoveConnectionHandlerTests()
    {
        _connectionPersistenceRepositoryMock = new Mock<IConnectionPersistenceRepository>();
        _connectionQueryRepositoryMock = new Mock<IConnectionQueryRepository>();

        _removeConnectionHandlerSUT = new(_connectionPersistenceRepositoryMock.Object, _connectionQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenConnectionDoesNotExist_ThrowConnectionNotFoundException()
    {
        // Given
        RemoveConnectionCommand command = new("connId");

        // When
        var testAction = async () => await _removeConnectionHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<ConnectionNotFoundException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenConnectionDoesExist_DeleteConnection()
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

        IConnection connection = new Connection("connId", user1, user2, ConnectionStatus.Connected);

        _connectionQueryRepositoryMock
            .Setup(repo => repo.GetConnectionAsync(connection.ConnectionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(connection);

        RemoveConnectionCommand command = new(connection.ConnectionId);

        // When
        await _removeConnectionHandlerSUT.Handle(command, CancellationToken.None);

        // Then
        _connectionPersistenceRepositoryMock
            .Verify(repo => repo.DeleteConnectionAsync(connection, It.IsAny<CancellationToken>()), Times.Once);
    }
}
