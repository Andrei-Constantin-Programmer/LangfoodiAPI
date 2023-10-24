using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.TestInfrastructure;

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

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void Handle_WhenMessageDoesNotExist_ThrowMessageNotFoundException()
    {
        // Given
        RemoveMessageCommand testCommand = new("MessageId");

        // When
        var testAction = async () => await _removeMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        testAction.Should().ThrowAsync<MessageNotFoundException>();
    }
}
