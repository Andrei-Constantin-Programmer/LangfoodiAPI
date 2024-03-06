using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.DataAccess.Exceptions;
using RecipeSocialMediaAPI.DataAccess.Mappers;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Tests.Unit.TestHelpers;
using RecipeSocialMediaAPI.Domain.Models.Messaging.Messages;
using RecipeSocialMediaAPI.Domain.Models.Recipes;
using RecipeSocialMediaAPI.Domain.Models.Users;
using RecipeSocialMediaAPI.Domain.Services.Interfaces;
using RecipeSocialMediaAPI.Domain.Tests.Shared;
using RecipeSocialMediaAPI.TestInfrastructure;
using System.Linq.Expressions;

namespace RecipeSocialMediaAPI.DataAccess.Tests.Unit.Mappers;

public class MessageDocumentToModelMapperTests
{
    private readonly MessageDocumentToModelMapper _messageDocumentToModelMapperSUT;

    private readonly Mock<ILogger<MessageDocumentToModelMapper>> _loggerMock;
    private readonly Mock<IMessageFactory> _messageFactoryMock;
    private readonly Mock<IRecipeQueryRepository> _recipeQueryRepositoryMock;
    private readonly Mock<IUserQueryRepository> _userQueryRepositoryMock;

    public MessageDocumentToModelMapperTests()
    {
        _loggerMock = new Mock<ILogger<MessageDocumentToModelMapper>>();
        _messageFactoryMock = new Mock<IMessageFactory>();
        _recipeQueryRepositoryMock = new Mock<IRecipeQueryRepository>();
        _userQueryRepositoryMock = new Mock<IUserQueryRepository>();

        _messageDocumentToModelMapperSUT = new(_loggerMock.Object, _messageFactoryMock.Object, _recipeQueryRepositoryMock.Object, _userQueryRepositoryMock.Object);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task MapMessageFromDocument_WhenOnlyTextIsPresent_ReturnTextMessage()
    {
        // Given
        string messageId = "1";
        string senderId = "50";
        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.Id == messageId;

        MessageDocument testDocument = new(
            Id: messageId,
            MessageContent: new("Text"),
            SeenByUserIds: new() { senderId },
            SenderId: senderId,
            SentDate: new(2023, 10, 17, 0, 0, 0, TimeSpan.Zero)
        );

        TestUserAccount testSender = new()
        {
            Id = senderId,
            Handler = "Test Handler",
            UserName = "Test Username",
            AccountCreationDate = new(2020, 10, 10, 0, 0, 0, TimeSpan.Zero)
        };
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(testSender.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestUserCredentials() { Account = testSender, Email = "test@mail.com", Password = "Test@123" });

        TestTextMessage textMessage = new(
            testDocument.Id!,
            testSender,
            testDocument.MessageContent.Text!,
            testDocument.SentDate,
            null,
            null,
            new() { testSender });

        _messageFactoryMock
            .Setup(factory => factory.CreateTextMessage(
                textMessage.Id,
                testSender,
                textMessage.Text,
                It.Is<List<IUserAccount>>(list => list.Contains(testSender)),
                textMessage.SentDate,
                textMessage.UpdatedDate,
                textMessage.RepliedToMessage))
            .Returns(textMessage);

        // When
        var result = await _messageDocumentToModelMapperSUT.MapMessageFromDocumentAsync(testDocument, testSender, null);

        // Then
        result.Should().BeEquivalentTo(textMessage);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    [InlineData(true)]
    [InlineData(false)]
    public async Task MapMessageFromDocument_WhenImagesArePresentAndRecipesAreNot_ReturnImageMessage(bool hasText)
    {
        // Given
        string messageId = "1";
        string senderId = "50";
        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.Id == messageId;

        List<string> imageURLs = new()
        {
            "Image1",
            "Image2"
        };

        MessageDocument testDocument = new(
            Id: messageId,
            MessageContent: new(hasText ? "Text" : null, null, imageURLs),
            SeenByUserIds: new() { senderId },
            SenderId: senderId,
            SentDate: new(2023, 10, 17, 0, 0, 0, TimeSpan.Zero)
        );

        TestUserAccount testSender = new()
        {
            Id = senderId,
            Handler = "Test Handler",
            UserName = "Test Username",
            AccountCreationDate = new(2020, 10, 10, 0, 0, 0, TimeSpan.Zero)
        };
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(testSender.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestUserCredentials() { Account = testSender, Email = "test@mail.com", Password = "Test@123" });

        TestImageMessage imageMessage = new(
            testDocument.Id!,
            testSender,
            testDocument.MessageContent.Text!,
            testDocument.MessageContent.ImageURLs!,
            testDocument.SentDate,
            null,
            null,
            new() { testSender } );

        _messageFactoryMock
            .Setup(factory => factory.CreateImageMessage(
                imageMessage.Id,
                testSender,
                imageMessage.ImageURLs,
                imageMessage.Text,
                It.Is<List<IUserAccount>>(list => list.Contains(testSender)),
                imageMessage.SentDate,
                imageMessage.UpdatedDate,
                null))
            .Returns(imageMessage);

        // When
        var result = (TestImageMessage?) await _messageDocumentToModelMapperSUT.MapMessageFromDocumentAsync(testDocument, testSender, null);

        // Then
        result.Should().Be(imageMessage);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    [InlineData(true)]
    [InlineData(false)]
    public async Task MapMessageFromDocument_WhenRecipesArePresentAndImagesAreNot_ReturnRecipeMessage(bool hasText)
    {
        // Given
        string messageId = "1";
        string senderId = "50";
        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.Id == messageId;

        List<string> recipeIds = new()
        {
            "id1",
            "id2"
        };

        MessageDocument testDocument = new(
            Id: messageId,
            MessageContent: new(hasText ? "Text" : null, recipeIds, null),
            SeenByUserIds: new() { senderId },
            SenderId: senderId,
            SentDate: new(2023, 10, 17, 0, 0, 0, TimeSpan.Zero)
        );

        TestUserAccount testSender = new()
        {
            Id = senderId,
            Handler = "Test Handler",
            UserName = "Test Username",
            AccountCreationDate = new(2020, 10, 10, 0, 0, 0, TimeSpan.Zero)
        };
        _userQueryRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(testSender.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestUserCredentials() { Account = testSender, Email = "test@mail.com", Password = "Test@123" });

        List<RecipeAggregate> recipes = new()
        {
            new(
                recipeIds[0],
                "Recipe1",
                new(new(), new()),
                "Description1",
                testSender,
                new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
                new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)),
            new(
                recipeIds[1],
                "Recipe2",
                new(new(), new()),
                "Description2",
                testSender,
                new(2023, 1, 15, 0, 0, 0, TimeSpan.Zero),
                new(2023, 1, 15, 0, 0, 0, TimeSpan.Zero))
        };

        TestRecipeMessage recipeMessage = new(
            testDocument.Id!,
            testSender,
            testDocument.MessageContent.Text!,
            recipes,
            testDocument.SentDate,
            null,
            null,
            new() { testSender });

        _recipeQueryRepositoryMock
            .Setup(repo => repo.GetRecipeByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string id, CancellationToken _) => recipes.FirstOrDefault(recipe => recipe.Id == id));
        _messageFactoryMock
            .Setup(factory => factory.CreateRecipeMessage(
                recipeMessage.Id,
                testSender,
                recipeMessage.Recipes,
                recipeMessage.Text,
                It.Is<List<IUserAccount>>(list => list.Contains(testSender)),
                recipeMessage.SentDate,
                recipeMessage.UpdatedDate,
                null))
            .Returns(recipeMessage);

        // When
        var result = (TestRecipeMessage?) await _messageDocumentToModelMapperSUT.MapMessageFromDocumentAsync(testDocument, testSender, null);

        // Then
        result.Should().Be(recipeMessage);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task MapMessageFromDocument_WhenRecipeMessageHasNoRecipesFound_ReturnTextMessageAndLogError()
    {
        // Given
        string messageId = "1";
        string senderId = "50";
        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.Id == messageId;

        MessageDocument testDocument = new(
            Id: messageId,
            MessageContent: new("Text", new() { "r1", "r2" }, null),
            SeenByUserIds: new(),
            SenderId: senderId,
            SentDate: new(2023, 10, 17, 0, 0, 0, TimeSpan.Zero)
        );

        TestUserAccount testSender = new()
        {
            Id = senderId,
            Handler = "Test Handler",
            UserName = "Test Username",
            AccountCreationDate = new(2020, 10, 10, 0, 0, 0, TimeSpan.Zero)
        };

        TestTextMessage textMessage = new(
            testDocument.Id!,
            testSender,
            testDocument.MessageContent.Text!,
            testDocument.SentDate,
            null);

        _messageFactoryMock
            .Setup(factory => factory.CreateTextMessage(
                textMessage.Id,
                testSender,
                textMessage.Text,
                new(),
                textMessage.SentDate,
                textMessage.UpdatedDate,
                null))
            .Returns(textMessage);

        // When
        var result = (TestTextMessage?) await _messageDocumentToModelMapperSUT.MapMessageFromDocumentAsync(testDocument, testSender, null);

        // Then
        result.Should().Be(textMessage);
        _loggerMock
            .Verify(logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((@object, _) => @object.ToString()!.Contains("Malformed message found with no existing recipes")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task MapMessageFromDocument_WhenRecipeMessageHasNoRecipesFoundAndNoText_ThrowMalformedMessageDocumentException()
    {
        // Given
        string messageId = "1";
        string senderId = "50";
        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.Id == messageId;

        List<string> recipeIds = new()
        {
            "inexistent1",
            "inexistent2"
        };

        MessageDocument testDocument = new(
            Id: messageId,
            MessageContent: new(null, recipeIds, null),
            SeenByUserIds: new(),
            SenderId: senderId,
            SentDate: new(2023, 10, 17, 0, 0, 0, TimeSpan.Zero)
        );

        TestUserAccount testSender = new()
        {
            Id = senderId,
            Handler = "Test Handler",
            UserName = "Test Username",
            AccountCreationDate = new(2020, 10, 10, 0, 0, 0, TimeSpan.Zero)
        };

        List<RecipeAggregate> recipes = new() { };

        TestRecipeMessage recipeMessage = new(
            testDocument.Id!,
            testSender,
            testDocument.MessageContent.Text!,
            recipes,
            testDocument.SentDate,
            null);

        _recipeQueryRepositoryMock
            .Setup(repo => repo.GetRecipeByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string id, CancellationToken _) => recipes.FirstOrDefault(recipe => recipe.Id == id));
        
        // When
        var testAction = async () => await _messageDocumentToModelMapperSUT.MapMessageFromDocumentAsync(testDocument, testSender, null);

        // Then
        await testAction.Should().ThrowAsync<MalformedMessageDocumentException>();
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public async Task MapMessageFromDocument_WhenOnlyRecipesAndRecipeIdDoesNotExist_ReturnRecipeMessageAndLogWarningForRecipesNotFound()
    {
        // Given
        string messageId = "1";
        string senderId = "50";
        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.Id == messageId;

        List<string> recipeIds = new()
        {
            "id1",
            "id2",
            "inexistent1",
            "inexistent2"
        };

        MessageDocument testDocument = new(
            Id: messageId,
            MessageContent: new(null, recipeIds, null),
            SeenByUserIds: new(),
            SenderId: senderId,
            SentDate: new(2023, 10, 17, 0, 0, 0, TimeSpan.Zero)
        );

        TestUserAccount testSender = new()
        {
            Id = senderId,
            Handler = "Test Handler",
            UserName = "Test Username",
            AccountCreationDate = new(2020, 10, 10, 0, 0, 0, TimeSpan.Zero)
        };

        List<RecipeAggregate> recipes = new()
        {
            new(
                recipeIds[0],
                "Recipe1",
                new(new(), new()),
                "Description1",
                testSender,
                new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
                new(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)),
            new(
                recipeIds[1],
                "Recipe2",
                new(new(), new()),
                "Description2",
                testSender,
                new(2023, 1, 15, 0, 0, 0, TimeSpan.Zero),
                new(2023, 1, 15, 0, 0, 0, TimeSpan.Zero))
        };

        TestRecipeMessage recipeMessage = new(
            testDocument.Id!,
            testSender,
            testDocument.MessageContent.Text!,
            recipes,
            testDocument.SentDate,
            null);

        _recipeQueryRepositoryMock
            .Setup(repo => repo.GetRecipeByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string id, CancellationToken _) => recipes.FirstOrDefault(recipe => recipe.Id == id));
        _messageFactoryMock
            .Setup(factory => factory.CreateRecipeMessage(
                recipeMessage.Id,
                testSender,
                recipeMessage.Recipes,
                recipeMessage.Text,
                new(),
                recipeMessage.SentDate,
                recipeMessage.UpdatedDate,
                null))
            .Returns(recipeMessage);
        
        // When
        var result = (TestRecipeMessage?) await _messageDocumentToModelMapperSUT.MapMessageFromDocumentAsync(testDocument, testSender, null);

        // Then
        result.Should().Be(recipeMessage);
        _loggerMock.Verify(logger =>
            logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(2));
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    [InlineData(false, false, false)]
    [InlineData(true, false, false)]
    public async Task MapMessageFromDocument_WhenMessageContentIsMalformed_ThrowMalformedMessageDocumentException(bool isTextNull, bool isRecipeListNull, bool isImageListNull)
    {
        // Given
        string messageId = "1";
        string senderId = "50";
        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.Id == messageId;

        MessageDocument testDocument = new(
            Id: messageId,
            MessageContent: new(
                isTextNull ? null : "Test Text",
                isRecipeListNull ? null : new List<string>(),
                isImageListNull ? null : new List<string>()),
            SeenByUserIds: new(),
            SenderId: senderId,
            SentDate: new(2023, 10, 17, 0, 0, 0, TimeSpan.Zero)
        );

        TestUserAccount testSender = new()
        {
            Id = senderId,
            Handler = "Test Handler",
            UserName = "Test Username",
            AccountCreationDate = new(2020, 10, 10, 0, 0, 0, TimeSpan.Zero)
        };

        TestTextMessage textMessage = new(
            testDocument.Id!,
            testSender,
            testDocument.MessageContent.Text!,
            testDocument.SentDate,
            null);

        // When
        var action = async () => await _messageDocumentToModelMapperSUT.MapMessageFromDocumentAsync(testDocument, testSender, null);

        // Then
        await action.Should().ThrowAsync<MalformedMessageDocumentException>();
    }
}
