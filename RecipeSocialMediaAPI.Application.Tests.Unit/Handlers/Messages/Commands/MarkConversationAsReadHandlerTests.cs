using Moq;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Messages;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Commands;

public class MarkConversationAsReadHandlerTests
{
    private readonly Mock<IConversationQueryRepository> _conversationQueryRepositoryMock;
    private readonly Mock<IMessagePersistenceRepository> _messagePersistenceRepositoryMock;

    private readonly MarkConversationAsReadHandler _markConversationAsReadHandlerSUT;

    public MarkConversationAsReadHandlerTests()
    {
        _conversationQueryRepositoryMock = new Mock<IConversationQueryRepository>();
        _messagePersistenceRepositoryMock = new Mock<IMessagePersistenceRepository>();

        _markConversationAsReadHandlerSUT = new(_conversationQueryRepositoryMock.Object, _messagePersistenceRepositoryMock.Object);
    }
}
