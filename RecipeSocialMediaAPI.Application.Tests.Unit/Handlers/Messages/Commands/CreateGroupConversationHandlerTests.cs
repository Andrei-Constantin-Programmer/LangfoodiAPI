using Moq;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Messages;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Commands;

public class CreateGroupConversationHandlerTests
{
    private readonly Mock<IConversationPersistenceRepository> _groupConversationPersistenceRepositoryMock;
    private readonly Mock<IGroupQueryRepository> _groupQueryRepositoryMock;

    private readonly CreateGroupConversationHandler _groupConversationHandlerSUT;

    public CreateGroupConversationHandlerTests()
    {
        _groupConversationPersistenceRepositoryMock = new Mock<IConversationPersistenceRepository>();
        _groupQueryRepositoryMock = new Mock<IGroupQueryRepository>();

        _groupConversationHandlerSUT = new(_groupConversationPersistenceRepositoryMock.Object, _groupQueryRepositoryMock.Object);
    }

    public async Task Handle_WhenGroupIdMatches_CreateAndReturnConnectionConversation()
    {
        // Given
        

        // When


        // Then

    }

    public async Task Handle_WhenGroupIdIsNotFound_ThrowArgumentException()
    {
        // Given


        // When


        // Then

    }
}
