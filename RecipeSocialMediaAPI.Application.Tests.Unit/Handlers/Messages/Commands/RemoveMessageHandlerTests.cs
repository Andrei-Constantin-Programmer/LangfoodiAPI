using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
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

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void Handle_WhenMessageDoesExistAndDeleteIsSuccessful_ReturnTaskCompleted()
    {
        // Given
        RemoveMessageCommand testCommand = new("MessageId");
        TestMessage testMessage = new(
            testCommand.Id,
            new TestUserAccount()
            {
                Id = "SenderId",
                Handler = "SenderHandler",
                UserName = "SenderUsername",
                AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new(2023, 10, 24, 0, 0, 0, TimeSpan.Zero),
            null);
        
        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessage(testCommand.Id))
            .Returns(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.DeleteMessage(testCommand.Id))
            .Returns(true);

        // When
        var testAction = async () => await _removeMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        testAction.Should().NotThrowAsync();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void Handle_WhenMessageDoesExistButDeleteIsUnsuccessful_ThrowException()
    {
        // Given
        RemoveMessageCommand testCommand = new("MessageId");
        TestMessage testMessage = new(
            testCommand.Id,
            new TestUserAccount()
            {
                Id = "SenderId",
                Handler = "SenderHandler",
                UserName = "SenderUsername",
                AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
            },
            new(2023, 10, 24, 0, 0, 0, TimeSpan.Zero),
            null);

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessage(testCommand.Id))
            .Returns(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.DeleteMessage(testCommand.Id))
            .Returns(false);

        // When
        var testAction = async () => await _removeMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        testAction.Should().ThrowAsync<Exception>().WithMessage("Could not remove message with id MessageId");
    }
}
