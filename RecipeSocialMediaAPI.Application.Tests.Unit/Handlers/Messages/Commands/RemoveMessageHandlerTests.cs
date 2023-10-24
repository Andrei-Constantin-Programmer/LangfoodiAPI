using Moq;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Messages;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Commands;

public class RemoveMessageHandlerTests
{
    private readonly Mock<IMessagePersistenceRepository> _messagePersistenceRepositoryMock;
    private readonly Mock<IMessageQueryRepository> _messageQueryRepositoryMock;

    private readonly RemoveMessageHandler _removeMessageHandlerSUT;

    public RemoveMessageHandlerTests()
    {
        _messagePersistenceRepositoryMock = new Mock<IMessagePersistenceRepository>();
        _messageQueryRepositoryMock = new Mock<IMessageQueryRepository>();

        _removeMessageHandlerSUT = new(_messagePersistenceRepositoryMock.Object, _messageQueryRepositoryMock.Object);
    }
}
