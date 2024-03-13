using FluentAssertions;
using MediatR;
using Moq;
using RecipeSocialMediaAPI.Application.Contracts.Messages;
using RecipeSocialMediaAPI.Application.DTO.Message;
using RecipeSocialMediaAPI.Application.DTO.Users;
using RecipeSocialMediaAPI.Application.Exceptions;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Commands;
using RecipeSocialMediaAPI.Application.Handlers.Messages.Notifications;
using RecipeSocialMediaAPI.Application.Mappers.Messages.Interfaces;
using RecipeSocialMediaAPI.Application.Repositories.Messages;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
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
    private readonly Mock<IMessageMapper> _messageMapperMock;
    private readonly Mock<IRecipeQueryRepository> _recipeQueryRepositoryMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly Mock<IPublisher> _publisherMock;

    private readonly IMessageFactory _messageFactory;
private readonly UpdateMessageHandler _updateMessageHandlerSUT;

    public UpdateMessageHandlerTests()
    {
        _messagePersistenceRepositoryMock = new Mock<IMessagePersistenceRepository>();
        _messageQueryRepositoryMock = new Mock<IMessageQueryRepository>();
        _messageMapperMock = new Mock<IMessageMapper>();
        _recipeQueryRepositoryMock = new Mock<IRecipeQueryRepository>();
        _publisherMock = new Mock<IPublisher>();

        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _dateTimeProviderMock
            .Setup(provider => provider.Now)
            .Returns(TEST_DATE);
        _messageFactory = new MessageFactory(_dateTimeProviderMock.Object);

        _updateMessageHandlerSUT = new(
            _messagePersistenceRepositoryMock.Object,
            _messageQueryRepositoryMock.Object,
            _messageMapperMock.Object,
            _recipeQueryRepositoryMock.Object,
            _publisherMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenTheMessageDoesNotExist_ThrowMessageNotFoundExceptionAndDontUpdate()
    {
        // Given
        UpdateMessageCommand testCommand = new(new UpdateMessageContract("MessageId", "New Text Content", null, null));

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(testCommand.Contract.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Message?)null);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<MessageNotFoundException>();
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessageAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenTheMessageIsCorrupted_ThrowCorruptedMessageExceptionAndDontUpdate()
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
            .Setup(repo => repo.GetMessageAsync(testCommand.Contract.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testMessage);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<CorruptedMessageException>();
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessageAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenTheMessageIsATextMessageAndItChangesTheTextAndUpdateIsSuccessful_UpdateAndDontThrow()
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
        var testMessage = (TextMessage)_messageFactory.CreateTextMessage("MessageId", testSender, "Original Text", new(), new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));
        UserPreviewForMessageDTO testSenderPreview = new(
            testSender.Id,
            testSender.UserName,
            testSender.ProfileImageId
        );

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(testCommand.Contract.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessageAsync(testMessage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        MessageDTO messageDto = new(testMessage.Id, testSenderPreview, new(), testMessage.SentDate, TextContent: testMessage.TextContent);
        _messageMapperMock
            .Setup(mapper => mapper.MapMessageToMessageDTO(testMessage))
            .Returns(messageDto);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        await testAction.Should().NotThrowAsync();
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessageAsync(
                    It.Is<TextMessage>(message =>
                        message.Id == testMessage.Id
                        && message.Sender == testMessage.Sender
                        && message.TextContent == testCommand.Contract.Text
                        && message.SentDate == testMessage.SentDate
                        && message.UpdatedDate == _dateTimeProviderMock.Object.Now), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenTheMessageIsATextMessageAndItChangesTheTextButUpdateIsUnsuccessful_ThrowMessageUpdateException()
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
        var testMessage = (TextMessage)_messageFactory.CreateTextMessage("MessageId", testSender, "Original Text", new(), new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(testCommand.Contract.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessageAsync(testMessage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<MessageUpdateException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenTheMessageIsATextMessageButRequestAddsImages_ThrowTextMessageUpdateExceptionAndDontUpdate()
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
        var testMessage = (TextMessage)_messageFactory.CreateTextMessage("MessageId", testSender, "Original Text", new(), new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(testCommand.Contract.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessageAsync(testMessage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        await testAction.Should()
            .ThrowAsync<TextMessageUpdateException>()
            .WithMessage("*attempted to add images");
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessageAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenTheMessageIsATextMessageButRequestAddsRecipes_ThrowTextMessageUpdateExceptionAndDontUpdate()
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
        var testMessage = (TextMessage)_messageFactory.CreateTextMessage("MessageId", testSender, "Original Text", new(), new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(testCommand.Contract.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessageAsync(testMessage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        await testAction.Should()
            .ThrowAsync<TextMessageUpdateException>()
            .WithMessage("*attempted to add recipes");
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessageAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenTheMessageIsATextMessageButNoChangesWereMade_ThrowTextMessageUpdateExceptionAndDontUpdate()
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
        var testMessage = (TextMessage)_messageFactory.CreateTextMessage("MessageId", testSender, testCommand.Contract.Text!, new(), new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(testCommand.Contract.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessageAsync(testMessage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        await testAction.Should()
            .ThrowAsync<TextMessageUpdateException>()
            .WithMessage("*no changes made");
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessageAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public async Task Handle_WhenTheMessageIsATextMessageButRequestNullifiesTextContent_ThrowTextMessageUpdateExceptionAndDontUpdate(string? newText)
    {
        // Given
        UpdateMessageCommand testCommand = new(new UpdateMessageContract("MessageId", newText, null, null));

        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        var testMessage = (TextMessage)_messageFactory.CreateTextMessage("MessageId", testSender, "Original Text", new(), new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(testCommand.Contract.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessageAsync(testMessage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        await testAction.Should()
            .ThrowAsync<TextMessageUpdateException>()
            .WithMessage("*attempted to nullify text");
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessageAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData("New Text Content")]
    [InlineData(null)]
    public async Task Handle_WhenTheMessageIsAnImageMessageAndItChangesTextAndUpdateIsSuccessful_UpdateAndDontThrow(string? newTextContent)
    {
        // Given
        UpdateMessageCommand testCommand = new(new UpdateMessageContract("MessageId", newTextContent, null, new()));

        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        var testMessage = (ImageMessage)_messageFactory.CreateImageMessage("MessageId", testSender, new List<string>() { "ExistingImage" }, "Original Text", new(), new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));
        UserPreviewForMessageDTO testSenderPreview = new(
            testSender.Id,
            testSender.UserName,
            testSender.ProfileImageId
        );

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(testCommand.Contract.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessageAsync(testMessage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        MessageDTO messageDto = new(testMessage.Id, testSenderPreview, new(), testMessage.SentDate, TextContent: testMessage.TextContent);
        _messageMapperMock
            .Setup(mapper => mapper.MapMessageToMessageDTO(testMessage))
            .Returns(messageDto);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        await testAction.Should().NotThrowAsync();
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessageAsync(
                    It.Is<ImageMessage>(message =>
                        message.Id == testMessage.Id
                        && message.Sender == testMessage.Sender
                        && message.TextContent == testCommand.Contract.Text
                        && message.SentDate == testMessage.SentDate
                        && message.UpdatedDate == _dateTimeProviderMock.Object.Now), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_WhenTheMessageIsAnImageMessageAndItAddsImagesAndUpdateIsSuccessful_UpdateAndDontThrow(bool changeText)
    {
        // Given
        string originalText = "Original Text";
        UpdateMessageCommand testCommand = new(new UpdateMessageContract("MessageId", changeText ? "New Text Content" : originalText, null, new() { "NewImage" } ));

        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        UserPreviewForMessageDTO testSenderPreview = new(
            testSender.Id,
            testSender.UserName,
            testSender.ProfileImageId
        );
        var testMessage = (ImageMessage)_messageFactory
            .CreateImageMessage("MessageId", testSender, new List<string>() { "ExistingImage" }, originalText, new(), new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(testCommand.Contract.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessageAsync(testMessage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        MessageDTO messageDto = new(testMessage.Id, testSenderPreview, new(), testMessage.SentDate, TextContent: testMessage.TextContent);
        _messageMapperMock
            .Setup(mapper => mapper.MapMessageToMessageDTO(testMessage))
            .Returns(messageDto);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        await testAction.Should().NotThrowAsync();
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessageAsync(
                    It.Is<ImageMessage>(message =>
                        message.Id == testMessage.Id
                        && message.Sender == testMessage.Sender
                        && message.TextContent == testCommand.Contract.Text
                        && message.SentDate == testMessage.SentDate
                        && message.UpdatedDate == _dateTimeProviderMock.Object.Now
                        && message.ImageURLs.Contains(testCommand.Contract.NewImageURLs!.First())), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_WhenTheMessageIsAnImageMessageAndItAddsImagesButUpdateIsUnsuccessful_ThrowMessageUpdateException(bool changeText)
    {
        // Given
        string originalText = "Original Text";
        UpdateMessageCommand testCommand = new(new UpdateMessageContract("MessageId", changeText ? "New Text Content" : originalText, null, new() { "NewImage" }));

        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        var testMessage = (ImageMessage)_messageFactory
            .CreateImageMessage("MessageId", testSender, new List<string>() { "ExistingImage" }, originalText, new(), new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(testCommand.Contract.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessageAsync(testMessage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<MessageUpdateException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenTheMessageIsAnImageMessageButRequestAddsRecipes_ThrowImageMessageUpdateExceptionAndDontUpdate()
    {
        // Given
        string originalText = "Original Text";
        UpdateMessageCommand testCommand = new(new UpdateMessageContract("MessageId", originalText, new() { "Recipe" }, new() { "NewImage" }));

        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        var testMessage = (ImageMessage)_messageFactory
            .CreateImageMessage("MessageId", testSender, new List<string>() { "ExistingImage" }, originalText, new(), new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(testCommand.Contract.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessageAsync(testMessage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        await testAction.Should()
            .ThrowAsync<ImageMessageUpdateException>()
            .WithMessage("*attempted to add recipes");
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessageAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenTheMessageIsAnImageMessageButNoChangesWereMade_ThrowImageMessageUpdateExceptionAndDontUpdate()
    {
        // Given
        string originalText = "Original Text";
        UpdateMessageCommand testCommand = new(new UpdateMessageContract("MessageId", originalText, null, new()));

        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        var testMessage = (ImageMessage)_messageFactory
            .CreateImageMessage("MessageId", testSender, new List<string>() { "ExistingImage" }, originalText, new(), new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(testCommand.Contract.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessageAsync(testMessage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        await testAction.Should()
            .ThrowAsync<ImageMessageUpdateException>()
            .WithMessage("*no changes made");
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessageAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData("New Text Content")]
    [InlineData(null)]
    public async Task Handle_WhenTheMessageIsARecipeMessageAndItChangesTextAndUpdateIsSuccessful_UpdateAndDontThrow(string? newTextContent)
    {
        // Given
        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        Recipe existingRecipe = new(
            "Existing1",
            "Existing Recipe Title",
            new(new(), new()),
            "Description",
            testSender,
            new(2023, 10, 26, 0, 0, 0, TimeSpan.Zero),
            new(2023, 10, 26, 0, 0, 0, TimeSpan.Zero));

        UpdateMessageCommand testCommand = new(new UpdateMessageContract("MessageId", newTextContent, new(), null));

        var testMessage = (RecipeMessage)_messageFactory
            .CreateRecipeMessage("MessageId", testSender, new List<Recipe>() { existingRecipe }, "Original Text", new(), new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));

        UserPreviewForMessageDTO testSenderPreview = new(
            testSender.Id,
            testSender.UserName,
            testSender.ProfileImageId
        );

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(testCommand.Contract.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessageAsync(testMessage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        MessageDTO messageDto = new(testMessage.Id, testSenderPreview, new(), testMessage.SentDate, TextContent: testMessage.TextContent);
        _messageMapperMock
            .Setup(mapper => mapper.MapMessageToMessageDTO(testMessage))
            .Returns(messageDto);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        await testAction.Should().NotThrowAsync();
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessageAsync(
                    It.Is<RecipeMessage>(message =>
                        message.Id == testMessage.Id
                        && message.Sender == testMessage.Sender
                        && message.TextContent == testCommand.Contract.Text
                        && message.SentDate == testMessage.SentDate
                        && message.UpdatedDate == _dateTimeProviderMock.Object.Now), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_WhenTheMessageIsARecipeMessageAndItAddsRecipesAndUpdateIsSuccessful_UpdateAndDontThrow(bool changeText)
    {
        // Given
        string originalText = "Original Text";
        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        Recipe existingRecipe = new(
            "Existing1",
            "Existing Recipe Title",
            new(new(), new()),
            "Description",
            testSender,
            new(2023, 10, 26, 0, 0, 0, TimeSpan.Zero),
            new(2023, 10, 26, 0, 0, 0, TimeSpan.Zero));

        Recipe newRecipe = new(
            "New1",
            "New Recipe Title",
            new(new(), new()),
            "Description",
            testSender,
            new(2023, 10, 27, 0, 0, 0, TimeSpan.Zero),
            new(2023, 10, 27, 0, 0, 0, TimeSpan.Zero));

        UpdateMessageCommand testCommand = new(new UpdateMessageContract("MessageId", changeText ? "New Text" : originalText, new() { newRecipe.Id }, null));

        var testMessage = (RecipeMessage)_messageFactory
            .CreateRecipeMessage("MessageId", testSender, new List<Recipe>() { existingRecipe }, originalText, new(), new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));

        UserPreviewForMessageDTO testSenderPreview = new(
            testSender.Id,
            testSender.UserName,
            testSender.ProfileImageId
        );

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(testCommand.Contract.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessageAsync(testMessage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _recipeQueryRepositoryMock
            .Setup(repo => repo.GetRecipeByIdAsync(newRecipe.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newRecipe);

        MessageDTO messageDto = new(testMessage.Id, testSenderPreview, new(), testMessage.SentDate, TextContent: testMessage.TextContent);
        _messageMapperMock
            .Setup(mapper => mapper.MapMessageToMessageDTO(testMessage))
            .Returns(messageDto);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        await testAction.Should().NotThrowAsync();
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessageAsync(
                    It.Is<RecipeMessage>(message =>
                        message.Id == testMessage.Id
                        && message.Sender == testMessage.Sender
                        && message.TextContent == testCommand.Contract.Text
                        && message.SentDate == testMessage.SentDate
                        && message.UpdatedDate == _dateTimeProviderMock.Object.Now
                        && message.Recipes.Contains(newRecipe)), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_WhenTheMessageIsARecipeMessageAndItAddsRecipesThatDontExist_ThrowRecipeMessageUpdateException(bool changeText)
    {
        // Given
        string originalText = "Original Text";
        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        Recipe existingRecipe = new(
            "Existing1",
            "Existing Recipe Title",
            new(new(), new()),
            "Description",
            testSender,
            new(2023, 10, 26, 0, 0, 0, TimeSpan.Zero),
            new(2023, 10, 26, 0, 0, 0, TimeSpan.Zero));

        UpdateMessageCommand testCommand = new(new UpdateMessageContract("MessageId", changeText ? "New Text" : originalText, new() { "Inexistent Recipe" }, null));

        var testMessage = (RecipeMessage)_messageFactory
            .CreateRecipeMessage("MessageId", testSender, new List<Recipe>() { existingRecipe }, originalText, new(), new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(testCommand.Contract.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessageAsync(testMessage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        await testAction.Should()
            .ThrowAsync<RecipeMessageUpdateException>()
            .WithMessage("*attempted to add inexistent recipe*");
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessageAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_WhenTheMessageIsARecipeMessageAndItAddsRecipesButUpdateIsUnsuccessful_ThrowRecipeMessageUpdateException(bool changeText)
    {
        // Given
        string originalText = "Original Text";
        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        Recipe existingRecipe = new(
            "Existing1",
            "Existing Recipe Title",
            new(new(), new()),
            "Description",
            testSender,
            new(2023, 10, 26, 0, 0, 0, TimeSpan.Zero),
            new(2023, 10, 26, 0, 0, 0, TimeSpan.Zero));

        Recipe newRecipe = new(
            "New1",
            "New Recipe Title",
            new(new(), new()),
            "Description",
            testSender,
            new(2023, 10, 27, 0, 0, 0, TimeSpan.Zero),
            new(2023, 10, 27, 0, 0, 0, TimeSpan.Zero));

        UpdateMessageCommand testCommand = new(new UpdateMessageContract("MessageId", changeText ? "New Text" : originalText, new() { newRecipe.Id }, null));

        var testMessage = (RecipeMessage)_messageFactory
            .CreateRecipeMessage("MessageId", testSender, new List<Recipe>() { existingRecipe }, originalText, new(), new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(testCommand.Contract.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessageAsync(testMessage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _recipeQueryRepositoryMock
            .Setup(repo => repo.GetRecipeByIdAsync(newRecipe.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newRecipe);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        await testAction.Should().ThrowAsync<MessageUpdateException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenTheMessageIsARecipeMessageButRequestAddsImages_ThrowRecipeMessageUpdateExceptionAndDontUpdate()
    {
        // Given
        string originalText = "Original Text";
        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        Recipe existingRecipe = new(
            "Existing1",
            "Existing Recipe Title",
            new(new(), new()),
            "Description",
            testSender,
            new(2023, 10, 26, 0, 0, 0, TimeSpan.Zero),
            new(2023, 10, 26, 0, 0, 0, TimeSpan.Zero));

        Recipe newRecipe = new(
            "New1",
            "New Recipe Title",
            new(new(), new()),
            "Description",
            testSender,
            new(2023, 10, 27, 0, 0, 0, TimeSpan.Zero),
            new(2023, 10, 27, 0, 0, 0, TimeSpan.Zero));

        UpdateMessageCommand testCommand = new(new UpdateMessageContract("MessageId", originalText, new() { newRecipe.Id }, new() { "New Image" }));

        var testMessage = (RecipeMessage)_messageFactory
            .CreateRecipeMessage("MessageId", testSender, new List<Recipe>() { existingRecipe }, originalText, new(), new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(testCommand.Contract.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessageAsync(testMessage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _recipeQueryRepositoryMock
            .Setup(repo => repo.GetRecipeByIdAsync(newRecipe.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newRecipe);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        await testAction.Should()
            .ThrowAsync<RecipeMessageUpdateException>()
            .WithMessage("*attempted to add images");
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessageAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenTheMessageIsARecipeMessageButNoChangesWereMade_ThrowRecipeMessageUpdateExceptionAndDontUpdate()
    {
        // Given
        string originalText = "Original Text";
        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "SenderUsername",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        Recipe existingRecipe = new(
            "Existing1",
            "Existing Recipe Title",
            new(new(), new()),
            "Description",
            testSender,
            new(2023, 10, 26, 0, 0, 0, TimeSpan.Zero),
            new(2023, 10, 26, 0, 0, 0, TimeSpan.Zero));

        UpdateMessageCommand testCommand = new(new UpdateMessageContract("MessageId", originalText, new(), null));

        var testMessage = (RecipeMessage)_messageFactory
            .CreateRecipeMessage("MessageId", testSender, new List<Recipe>() { existingRecipe }, originalText, new(), new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(testCommand.Contract.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessageAsync(testMessage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var testAction = async () => await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        await testAction.Should()
            .ThrowAsync<RecipeMessageUpdateException>()
            .WithMessage("*no changes made");
        _messagePersistenceRepositoryMock
            .Verify(repo => repo.UpdateMessageAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.APPLICATION)]
    public async Task Handle_WhenTheUpdateIsSuccessful_PublishMessageUpdatedNotification()
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
        var testMessage = (TextMessage)_messageFactory.CreateTextMessage("MessageId", testSender, "Original Text", new(), new(2023, 10, 20, 0, 0, 0, TimeSpan.Zero));

        UserPreviewForMessageDTO testSenderPreview = new(
            testSender.Id,
            testSender.UserName,
            testSender.ProfileImageId
        );

        _messageQueryRepositoryMock
            .Setup(repo => repo.GetMessageAsync(testCommand.Contract.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testMessage);
        _messagePersistenceRepositoryMock
            .Setup(repo => repo.UpdateMessageAsync(testMessage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        MessageDTO messageDto = new(testMessage.Id, testSenderPreview, new(), testMessage.SentDate, TextContent: testMessage.TextContent);
        _messageMapperMock
            .Setup(mapper => mapper.MapMessageToMessageDTO(testMessage))
            .Returns(messageDto);

        // When
        await _updateMessageHandlerSUT.Handle(testCommand, CancellationToken.None);

        // Then
        _publisherMock
            .Verify(publisher => publisher.Publish(
                    It.Is<MessageUpdatedNotification>(notification
                        => notification.UpdatedMessage == messageDto),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
