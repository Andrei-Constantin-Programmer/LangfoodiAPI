using FluentAssertions;
using MediatR;
using Moq;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Commands;

public class RemoveMessageHandlerTests
{
    private readonly Mock<IMessagePersistenceRepository> _messagePersistenceRepositoryMock;
    private readonly Mock<IMessageQueryRepository> _messageQueryRepositoryMock;
    private readonly Mock<IPublisher> _publisherMock;

    private readonly RemoveMessageHandler _removeMessageHandlerSUT;

    public RemoveMessageHandlerTests()
    {
        _messagePersistenceRepositoryMock = new Mock<IMessagePersistenceRepository>();
        _messageQueryRepositoryMock = new Mock<IMessageQueryRepository>();
        _publisherMock = new Mock<IPublisher>();

        _removeMessageHandlerSUT = new(_messagePersistenceRepositoryMock.Object, _messageQueryRepositoryMock.Object, _publisherMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenMessageDoesNotExist_ThrowMessageNotFoundException()
    {
        // Given
        RemoveMessageCommand testCommand = new("MessageId");

        // When
        var testAction = async () => await _removeMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<MessageNotFoundException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenMessageDoesExistAndDeleteIsSuccessful_DontThrow()
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
        await testAction.Should().NotThrowAsync();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenMessageDoesExistButDeleteIsUnsuccessful_ThrowMessageRemovalException()
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
        await testAction.Should().ThrowAsync<MessageRemovalException>().WithMessage($"*{testMessage.Id}*");
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenDeleteIsSuccessful_PublishMessageDeletedNotificationAsync()
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
        await _removeMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        _publisherMock
            .Verify(publisher => publisher.Publish(
                    It.Is<MessageDeletedNotification>(notification 
                        => notification.MessageId == testCommand.Id), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
    }
}
