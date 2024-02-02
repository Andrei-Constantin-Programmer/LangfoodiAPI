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
