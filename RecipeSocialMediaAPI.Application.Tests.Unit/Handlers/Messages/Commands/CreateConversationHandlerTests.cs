using Moq;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Users;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Commands;

public class CreateConversationHandlerTests
{
    private readonly Mock<IConversationPersistenceRepository> _conversationPersistenceRepositoryMock;
    private readonly Mock<IConnectionQueryRepository> _connectionQueryRepositoryMock;
    private readonly Mock<IGroupQueryRepository> _groupQueryRepositoryMock;

    private readonly CreateConversationHandler _conversationHandlerSUT;

    public CreateConversationHandlerTests()
    {
        _conversationPersistenceRepositoryMock = new Mock<IConversationPersistenceRepository>();
        _connectionQueryRepositoryMock = new Mock<IConnectionQueryRepository>();
        _groupQueryRepositoryMock = new Mock<IGroupQueryRepository>();

        _conversationHandlerSUT = new(_conversationPersistenceRepositoryMock.Object, _connectionQueryRepositoryMock.Object, _groupQueryRepositoryMock.Object);
    }

    public async Task Handle_WhenIdIsConnection_CreateAndReturnConnectionConversation()
    {
        // Given
        

        // When


        // Then

    }

    public async Task Handle_WhenIdIsGroup_CreateAndReturnGroupConversation()
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
