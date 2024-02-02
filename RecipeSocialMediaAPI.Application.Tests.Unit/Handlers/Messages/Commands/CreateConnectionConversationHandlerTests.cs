using Moq;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Messages;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Commands;

public class CreateConnectionConversationHandlerTests
{
    private readonly Mock<IConversationPersistenceRepository> _conversationPersistenceRepositoryMock;
    private readonly Mock<IConnectionQueryRepository> _connectionQueryRepositoryMock;
    private readonly Mock<IGroupQueryRepository> _groupQueryRepositoryMock;

    private readonly CreateConnectionConversationHandler _conversationHandlerSUT;

    public CreateConnectionConversationHandlerTests()
    {
        _conversationPersistenceRepositoryMock = new Mock<IConversationPersistenceRepository>();
        _connectionQueryRepositoryMock = new Mock<IConnectionQueryRepository>();
        _groupQueryRepositoryMock = new Mock<IGroupQueryRepository>();

        _conversationHandlerSUT = new(_conversationPersistenceRepositoryMock.Object, _connectionQueryRepositoryMock.Object);
    }

    public async Task Handle_WhenIdIsConnection_CreateAndReturnConnectionConversation()
    {
        // Given
        

        // When


        // Then

    }

    public async Task Handle_WhenIdIsNotFound_ThrowArgumentException()
    {
        // Given


        // When


        // Then

    }
}
