using Moq;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Messages;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Commands;

public class CreateConnectionConversationHandlerTests
{
    private readonly Mock<IConversationPersistenceRepository> _conversationPersistenceRepositoryMock;
    private readonly Mock<IConnectionQueryRepository> _connectionQueryRepositoryMock;

    private readonly CreateConnectionConversationHandler _connectionConversationHandlerSUT;

    public CreateConnectionConversationHandlerTests()
    {
        _conversationPersistenceRepositoryMock = new Mock<IConversationPersistenceRepository>();
        _connectionQueryRepositoryMock = new Mock<IConnectionQueryRepository>();

        _connectionConversationHandlerSUT = new(_conversationPersistenceRepositoryMock.Object, _connectionQueryRepositoryMock.Object);
    }

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
            AccountCreationDate = new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero)
        };

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

        NewConversationContract testContract = new(connection.ConnectionId);


        // When


        // Then

    }

    public async Task Handle_WhenConnectionIdIsNotFound_ThrowArgumentException()
    {
        // Given


        // When


        // Then

    }
}
