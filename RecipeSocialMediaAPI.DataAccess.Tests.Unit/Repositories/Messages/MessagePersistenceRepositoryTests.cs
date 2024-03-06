using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Neleus.LambdaCompare;
using RecipeSocialMediaAPI.DataAccess.Mappers.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoConfiguration.Interfaces;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Repositories.Messages;
using RecipeSocialMediaAPI.DataAccess.Tests.Unit.TestHelpers;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.Domain.Utilities;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Repositories.Messages;

public class MessagePersistenceRepositoryTests
{
    private readonly MessagePersistenceRepository _messagePersistenceRepositorySUT;

    private readonly Mock<ILogger<MessagePersistenceRepository>> _loggerMock;
    private readonly Mock<IMessageDocumentToModelMapper> _messageDocumentToModelMapperMock;
    private readonly Mock<IMongoCollectionWrapper<MessageDocument>> _messageCollectionMock;
    private readonly Mock<IMongoCollectionFactory> _mongoCollectionFactoryMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

    private readonly IMessageFactory _messageFactory;

    public MessagePersistenceRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<MessagePersistenceRepository>>();
        _messageDocumentToModelMapperMock = new Mock<IMessageDocumentToModelMapper>();
        _messageCollectionMock = new Mock<IMongoCollectionWrapper<MessageDocument>>();
        _mongoCollectionFactoryMock = new Mock<IMongoCollectionFactory>();
        _mongoCollectionFactoryMock
            .Setup(factory => factory.CreateCollection<MessageDocument>())
            .Returns(_messageCollectionMock.Object);

        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _dateTimeProviderMock
            .Setup(provider => provider.Now)
            .Returns(new DateTimeOffset(2023, 10, 22, 21, 30, 0, TimeSpan.Zero));
        _messageFactory = new MessageFactory(_dateTimeProviderMock.Object);

        _messagePersistenceRepositorySUT = new(_loggerMock.Object, _messageDocumentToModelMapperMock.Object, _mongoCollectionFactoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task CreateMessage_WhenMessageIsValid_AddMessageToCollectionAndReturnMappedMessageAsync()
    {
        // Given
        IUserAccount testSender = new TestUserAccount()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "TestSender",
            AccountCreationDate = new(2023, 10, 6, 0, 0, 0, TimeSpan.Zero)
        };

        TestFullMessage expectedMessage = new(
            "MessageId",
            testSender,
            "MessageText",
            new List<RecipeAggregate>() 
            { 
                new(
                    "RecipeId",
                    "RecipeTitle",
                    new Recipe(new(), new()),
                    "Recipe Description",
                    testSender,
                    new(2023, 10, 22, 0, 0, 0, TimeSpan.Zero),
                    new(2023, 10, 22, 0, 0, 0, TimeSpan.Zero)
                )
            },
            new List<string>() { "Image" },
            new(2023, 10, 22, 19, 30, 0, TimeSpan.Zero),
            null);

        _messageDocumentToModelMapperMock
            .Setup(mapper => mapper.MapMessageFromDocument(It.IsAny<MessageDocument>(), testSender, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMessage);

        // When
        var result = await _messagePersistenceRepositorySUT.CreateMessage(
            testSender,
            expectedMessage.Text,
            expectedMessage.Recipes.Select(r => r.Id).ToList(),
            expectedMessage.ImageURLs,
            expectedMessage.SentDate,
            null,
            new());

        // Then
        _messageCollectionMock
            .Verify(collection => collection.Insert(
                It.Is<MessageDocument>(doc =>
                    doc.Id == null
                    && doc.MessageContent.Text == expectedMessage.Text
                    && doc.MessageContent.RecipeIds!.SequenceEqual(expectedMessage.Recipes.Select(r => r.Id))
                    && doc.MessageContent.ImageURLs!.SequenceEqual(expectedMessage.ImageURLs)
                    && doc.SentDate == expectedMessage.SentDate
                    && doc.LastUpdatedDate == expectedMessage.UpdatedDate
                    && doc.MessageRepliedToId == null), 
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task UpdateMessage_WhenMessageIsTextMessage_UpdatesAndReturnsTrueAsync()
    {
        // Given
        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "TestSender",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        var message = (TextMessage)_messageFactory
            .CreateTextMessage("MessageId", testSender, "Test Text", new(), _dateTimeProviderMock.Object.Now);

        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.Id == message.Id;

        _messageCollectionMock
            .Setup(collection => collection.UpdateRecord(
                It.IsAny<MessageDocument>(), 
                It.IsAny<Expression<Func<MessageDocument, bool>>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var result = await _messagePersistenceRepositorySUT.UpdateMessage(message);

        // Then
        result.Should().BeTrue();
        _messageCollectionMock
            .Verify(collection => 
                collection.UpdateRecord(
                    It.Is<MessageDocument>(doc =>
                        doc.Id == message.Id
                        && doc.MessageContent.Text == message.TextContent
                        && doc.MessageContent.RecipeIds == null
                        && doc.MessageContent.ImageURLs == null
                        && doc.SentDate == message.SentDate                    
                    ),
                    It.Is<Expression<Func<MessageDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task UpdateMessage_WhenMessageIsRecipeMessage_UpdatesAndReturnsTrueAsync()
    {
        // Given
        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "TestSender",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        List<RecipeAggregate> recipes = new()
        {
            new("Recipe1", "Recipe 1", new(new(), new()), "Description", testSender, new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero), new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)),
            new("Recipe2", "Recipe 2", new(new(), new()), "Description", testSender, new(2023, 2, 2, 0, 0, 0, TimeSpan.Zero), new(2023, 4, 4, 0, 0, 0, TimeSpan.Zero)),
            new("Recipe3", "Recipe 3", new(new(), new()), "Description", testSender, new(2023, 3, 3, 0, 0, 0, TimeSpan.Zero), new(2023, 5, 5, 0, 0, 0, TimeSpan.Zero)),
        };

        var message = (RecipeMessage)_messageFactory
            .CreateRecipeMessage("MessageId", testSender, recipes, "Test Text", new(), _dateTimeProviderMock.Object.Now);

        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.Id == message.Id;

        _messageCollectionMock
            .Setup(collection => collection.UpdateRecord(
                It.IsAny<MessageDocument>(), 
                It.IsAny<Expression<Func<MessageDocument, bool>>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var result = await _messagePersistenceRepositorySUT.UpdateMessage(message);

        // Then
        result.Should().BeTrue();
        _messageCollectionMock
            .Verify(collection =>
                collection.UpdateRecord(
                    It.Is<MessageDocument>(doc =>
                        doc.Id == message.Id
                        && doc.MessageContent.Text == message.TextContent
                        && doc.MessageContent.RecipeIds!.SequenceEqual(recipes.Select(r => r.Id))
                        && doc.MessageContent.ImageURLs == null
                        && doc.SentDate == message.SentDate
                    ),
                    It.Is<Expression<Func<MessageDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task UpdateMessage_WhenMessageIsImageMessage_UpdatesAndReturnsTrueAsync()
    {
        // Given
        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "TestSender",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        List<string> imageURLs = new() { "Image1", "Image2", "Image3" };

        var message = (ImageMessage)_messageFactory
            .CreateImageMessage("MessageId", testSender, imageURLs, "Test Text", new(), _dateTimeProviderMock.Object.Now);

        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.Id == message.Id;

        _messageCollectionMock
            .Setup(collection => collection.UpdateRecord(
                It.IsAny<MessageDocument>(), 
                It.IsAny<Expression<Func<MessageDocument, bool>>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var result = await _messagePersistenceRepositorySUT.UpdateMessage(message);

        // Then
        result.Should().BeTrue();
        _messageCollectionMock
            .Verify(collection =>
                collection.UpdateRecord(
                    It.Is<MessageDocument>(doc =>
                        doc.Id == message.Id
                        && doc.MessageContent.Text == message.TextContent
                        && doc.MessageContent.RecipeIds == null
                        && doc.MessageContent.ImageURLs!.SequenceEqual(imageURLs)
                        && doc.SentDate == message.SentDate
                    ),
                    It.Is<Expression<Func<MessageDocument, bool>>>(expr => Lambda.Eq(expr, expectedExpression)), 
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task UpdateMessage_WhenMessageIsOfUnexpectedType_LogErrorAndReturnFalseAsync()
    {
        // Given
        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "TestSender",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        TestMessage message = new("MessageId", testSender, _dateTimeProviderMock.Object.Now, null);

        _messageCollectionMock
            .Setup(collection => collection.UpdateRecord(
                It.IsAny<MessageDocument>(), 
                It.IsAny<Expression<Func<MessageDocument, bool>>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var result = await _messagePersistenceRepositorySUT.UpdateMessage(message);

        // Then
        result.Should().BeFalse();
        _loggerMock.Verify(logger =>
            logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(1));
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task UpdateMessage_WhenUpdateIsUnsuccessful_ReturnFalseAsync()
    {
        // Given
        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "TestSender",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };

        var message = (TextMessage)_messageFactory
            .CreateTextMessage("MessageId", testSender, "Test Text", new(), _dateTimeProviderMock.Object.Now);

        _messageCollectionMock
            .Setup(collection => collection.UpdateRecord(
                It.IsAny<MessageDocument>(), 
                It.IsAny<Expression<Func<MessageDocument, bool>>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // When
        var result = await _messagePersistenceRepositorySUT.UpdateMessage(message);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task DeleteMessage_WhenDeleteIsSuccessful_ReturnTrueAsync()
    {
        // Given
        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "Sender",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        TestMessage message = new("TestId", testSender, _dateTimeProviderMock.Object.Now, null);

        _messageCollectionMock
            .Setup(collection => collection.Delete(
                It.IsAny<Expression<Func<MessageDocument, bool>>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var result = await _messagePersistenceRepositorySUT.DeleteMessage(message);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task DeleteMessage_WhenDeleteIsUnsuccessful_ReturnFalseAsync()
    {
        // Given
        TestUserAccount testSender = new()
        {
            Id = "SenderId",
            Handler = "SenderHandler",
            UserName = "Sender",
            AccountCreationDate = new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)
        };
        TestMessage message = new("TestId", testSender, _dateTimeProviderMock.Object.Now, null);

        _messageCollectionMock
            .Setup(collection => collection.Delete(
                It.IsAny<Expression<Func<MessageDocument, bool>>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // When
        var result = await _messagePersistenceRepositorySUT.DeleteMessage(message);

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task DeleteMessageById_WhenDeleteIsSuccessful_ReturnTrueAsync()
    {
        // Given
        _messageCollectionMock
            .Setup(collection => collection.Delete(
                It.IsAny<Expression<Func<MessageDocument, bool>>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // When
        var result = await _messagePersistenceRepositorySUT.DeleteMessage("TestId");

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task DeleteMessageById_WhenDeleteIsUnsuccessful_ReturnFalseAsync()
    {
        // Given
        _messageCollectionMock
            .Setup(collection => collection.Delete(
                It.IsAny<Expression<Func<MessageDocument, bool>>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // When
        var result = await _messagePersistenceRepositorySUT.DeleteMessage("TestId");

        // Then
        result.Should().BeFalse();
    }
}
