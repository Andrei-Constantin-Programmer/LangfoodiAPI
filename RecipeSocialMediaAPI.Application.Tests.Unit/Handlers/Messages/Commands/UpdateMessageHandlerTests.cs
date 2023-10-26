using FluentAssertions;
using Moq;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Services;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.TestInfrastructure;

namespace RecipeSocialMediaAPI.Application.Tests.Unit.Handlers.Messages.Commands;

public class UpdateMessageHandlerTests
{
    public static readonly DateTimeOffset TEST_DATE = new(2023, 10, 24, 0, 0, 0, TimeSpan.Zero);

    private readonly Mock<IMessagePersistenceRepository> _messagePersistenceRepositoryMock;
    private readonly Mock<IMessageQueryRepository> _messageQueryRepositoryMock;
    private readonly Mock<IRecipeQueryRepository> _recipeQueryRepositoryMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

    private readonly IMessageFactory _messageFactory;

    private readonly UpdateMessageHandler _updateMessageHandlerSUT;

    public UpdateMessageHandlerTests()
    {
        _messagePersistenceRepositoryMock = new Mock<IMessagePersistenceRepository>();
        _messageQueryRepositoryMock = new Mock<IMessageQueryRepository>();
        _recipeQueryRepositoryMock = new Mock<IRecipeQueryRepository>();

        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _dateTimeProviderMock
            .Setup(provider => provider.Now)
            .Returns(TEST_DATE);
        _messageFactory = new MessageFactory(_dateTimeProviderMock.Object);

        _updateMessageHandlerSUT = new(_messagePersistenceRepositoryMock.Object, _messageQueryRepositoryMock.Object, _recipeQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void Handle_WhenTheMessageDoesNotExist_ThrowMessageNotFoundExceptionAndDontUpdate()
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
    public void Handle_WhenTheMessageIsCorrupted_ThrowCorruptedMessageExceptionAndDontUpdate()
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

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void Handle_WhenTheMessageIsATextMessageAndItChangesTheTextAndUpdateIsSuccessful_UpdateAndDontThrow()
    {
        // Given
        UpdateMessageCommand testCommand = new(new UpdateMessageContract("MessageId", "New Text Content", null, null));

        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        var testMessage = (TextMessage)_messageFactory.CreateTextMessage("MessageId", testSender, "Original Text", new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessage(testCommand.UpdateMessageContract.Id))
            .Returns(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessage(testMessage))
            .Returns(true);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        testAction.Should().NotThrowAsync();
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessage(It.Is<TextMessage>(message =>
                    message.Id == testMessage.Id
                    && message.Sender == testMessage.Sender
                    && message.TextContent == testCommand.UpdateMessageContract.Text
                    && message.SentDate == testMessage.SentDate
                    && message.UpdatedDate == _dateTimeProviderMock.Object.Now
                )), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void Handle_WhenTheMessageIsATextMessageAndItChangesTheTextButUpdateIsUnsuccessful_ThrowMessageUpdateException()
    {
        // Given
        UpdateMessageCommand testCommand = new(new UpdateMessageContract("MessageId", "New Text Content", null, null));

        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        var testMessage = (TextMessage)_messageFactory.CreateTextMessage("MessageId", testSender, "Original Text", new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessage(testCommand.UpdateMessageContract.Id))
            .Returns(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessage(testMessage))
            .Returns(false);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        testAction.Should().ThrowAsync<MessageUpdateException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void Handle_WhenTheMessageIsATextMessageButRequestAddsImages_ThrowMessageUpdateExceptionAndDontUpdate()
    {
        // Given
        UpdateMessageCommand testCommand = new(new UpdateMessageContract("MessageId", "New Text Content", null, new() { "Image" }));

        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        var testMessage = (TextMessage)_messageFactory.CreateTextMessage("MessageId", testSender, "Original Text", new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessage(testCommand.UpdateMessageContract.Id))
            .Returns(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessage(testMessage))
            .Returns(true);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        testAction.Should()
            .ThrowAsync<MessageUpdateException>()
            .WithMessage("*attempted to add images");
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessage(It.Is<TextMessage>(message =>
                    message.Id == testMessage.Id
                    && message.Sender == testMessage.Sender
                    && message.TextContent == testCommand.UpdateMessageContract.Text
                    && message.SentDate == testMessage.SentDate
                    && message.UpdatedDate == _dateTimeProviderMock.Object.Now
                )), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void Handle_WhenTheMessageIsATextMessageButRequestAddsRecipes_ThrowMessageUpdateExceptionAndDontUpdate()
    {
        // Given
        UpdateMessageCommand testCommand = new(new UpdateMessageContract("MessageId", "New Text Content", new() { "Recipe" }, null));

        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        var testMessage = (TextMessage)_messageFactory.CreateTextMessage("MessageId", testSender, "Original Text", new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessage(testCommand.UpdateMessageContract.Id))
            .Returns(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessage(testMessage))
            .Returns(true);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        testAction.Should()
            .ThrowAsync<MessageUpdateException>()
            .WithMessage("*attempted to add recipes");
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessage(It.Is<TextMessage>(message =>
                    message.Id == testMessage.Id
                    && message.Sender == testMessage.Sender
                    && message.TextContent == testCommand.UpdateMessageContract.Text
                    && message.SentDate == testMessage.SentDate
                    && message.UpdatedDate == _dateTimeProviderMock.Object.Now
                )), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public void Handle_WhenTheMessageIsATextMessageButNoChangesWereMade_ThrowMessageUpdateExceptionAndDontUpdate()
    {
        // Given
        UpdateMessageCommand testCommand = new(new UpdateMessageContract("MessageId", "Original Text", null, null));

        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        var testMessage = (TextMessage)_messageFactory.CreateTextMessage("MessageId", testSender, testCommand.UpdateMessageContract.Text!, new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessage(testCommand.UpdateMessageContract.Id))
            .Returns(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessage(testMessage))
            .Returns(true);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        testAction.Should()
            .ThrowAsync<MessageUpdateException>()
            .WithMessage("*no changes made");
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessage(It.Is<TextMessage>(message =>
                    message.Id == testMessage.Id
                    && message.Sender == testMessage.Sender
                    && message.TextContent == testCommand.UpdateMessageContract.Text
                    && message.SentDate == testMessage.SentDate
                    && message.UpdatedDate == _dateTimeProviderMock.Object.Now
                )), Times.Never);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData("")]
    [InlineData(null)]
    public void Handle_WhenTheMessageIsATextMessageButRequestNullifiesTextContent_ThrowMessageUpdateExceptionAndDontUpdate(string? newText)
    {
        // Given
        UpdateMessageCommand testCommand = new(new UpdateMessageContract("MessageId", newText, new() { "Recipe" }, null));

        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        var testMessage = (TextMessage)_messageFactory.CreateTextMessage("MessageId", testSender, "Original Text", new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessage(testCommand.UpdateMessageContract.Id))
            .Returns(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessage(testMessage))
            .Returns(true);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        testAction.Should().ThrowAsync<MessageUpdateException>().WithMessage("*attempted to nullify text");
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessage(It.Is<TextMessage>(message =>
                    message.Id == testMessage.Id
                    && message.Sender == testMessage.Sender
                    && message.TextContent == testCommand.UpdateMessageContract.Text
                    && message.SentDate == testMessage.SentDate
                    && message.UpdatedDate == _dateTimeProviderMock.Object.Now
                )), Times.Never);
    }
}
