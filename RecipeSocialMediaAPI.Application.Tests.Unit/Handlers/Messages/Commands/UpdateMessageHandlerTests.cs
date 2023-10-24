using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Commands;

public class UpdateMessageHandlerTests
{
    private readonly Mock<IMessagePersistenceRepository> _messagePersistenceRepositoryMock;
    private readonly Mock<IMessageQueryRepository> _messageQueryRepositoryMock;
    private readonly Mock<IRecipeQueryRepository> _recipeQueryRepositoryMock;

    private readonly UpdateMessageHandler _updateMessageHandlerSUT;

    public UpdateMessageHandlerTests()
    {
        _messagePersistenceRepositoryMock = new Mock<IMessagePersistenceRepository>();
        _messageQueryRepositoryMock = new Mock<IMessageQueryRepository>();
        _recipeQueryRepositoryMock = new Mock<IRecipeQueryRepository>();

        _updateMessageHandlerSUT = new(_messagePersistenceRepositoryMock.Object, _messageQueryRepositoryMock.Object, _recipeQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void Handle_WhenTheMessageDoesNotExist_ThrowMessageNotFoundException()
    {
        // Given
        UpdateMessageCommand testCommand = new(new UpdateMessageContract("MessageId", "New Text Content", null, null));

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessage(testCommand.UpdateMessageContract.Id))
            .Returns((Message?)null);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        testAction.Should().ThrowAsync<MessageNotFoundException>();
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessage(It.IsAny<Message>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void Handle_WhenTheMessageIsCorrupted_ThrowCorruptedMessageException()
    {
        // Given
        UpdateMessageCommand testCommand = new(new UpdateMessageContract("MessageId", "New Text Content", null, null));

        TestMessage testMessage = new(
            "TestId",
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
            .Setup(repo => repo.GetMessage(testCommand.UpdateMessageContract.Id))
            .Returns(testMessage);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        testAction.Should().ThrowAsync<CorruptedMessageException>();
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessage(It.IsAny<Message>()), Times.Never);
    }
}
