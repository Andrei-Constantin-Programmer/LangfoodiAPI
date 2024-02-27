using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RecipeSocialMediaAPI.Application.Repositories.Recipes;
using RecipeSocialMediaAPI.Application.Repositories.Users;
using RecipeSocialMediaAPI.DataAccess.Exceptions;
using RecipeSocialMediaAPI.DataAccess.Mappers;
using RecipeSocialMediaAPI.DataAccess.MongoDocuments;
using RecipeSocialMediaAPI.DataAccess.Tests.Unit.TestHelpers;
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
    public void MapMessageFromDocument_WhenOnlyTextIsPresent_ReturnTextMessage()
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
            .Setup(repo => repo.GetUserById(testSender.Id))
            .Returns(new TestUserCredentials() { Account = testSender, Email = "test@mail.com", Password = "Test@123" });

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
        var result = _messageDocumentToModelMapperSUT.MapMessageFromDocument(testDocument, testSender, null);

        // Then
        result.Should().BeEquivalentTo(textMessage);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    [InlineData(true)]
    [InlineData(false)]
    public void MapMessageFromDocument_WhenImagesArePresentAndRecipesAreNot_ReturnImageMessage(bool hasText)
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
            .Setup(repo => repo.GetUserById(testSender.Id))
            .Returns(new TestUserCredentials() { Account = testSender, Email = "test@mail.com", Password = "Test@123" });

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
        var result = (TestImageMessage?)_messageDocumentToModelMapperSUT.MapMessageFromDocument(testDocument, testSender, null);

        // Then
        result.Should().Be(imageMessage);
    }

    [Theory]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    [InlineData(true)]
    [InlineData(false)]
    public void MapMessageFromDocument_WhenRecipesArePresentAndImagesAreNot_ReturnRecipeMessage(bool hasText)
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
            .Setup(repo => repo.GetUserById(testSender.Id))
            .Returns(new TestUserCredentials() { Account = testSender, Email = "test@mail.com", Password = "Test@123" });

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
            .Setup(repo => repo.GetRecipeById(It.IsAny<string>()))
            .Returns((string id) => recipes.FirstOrDefault(recipe => recipe.Id == id));
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
        var result = (TestRecipeMessage?)_messageDocumentToModelMapperSUT.MapMessageFromDocument(testDocument, testSender, null);

        // Then
        result.Should().Be(recipeMessage);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void MapMessageFromDocument_WhenNoTextAndNoRecipesFoundAndNoImageIdsExist_ReturnRemovedRecipeMessage()
    {
        // Given
        string messageId = "1";
        string senderId = "50";
        Expression<Func<MessageDocument, bool>> expectedExpression = x => x.Id == messageId;

        MessageDocument testDocument = new(
            Id: messageId,
            MessageContent: new(null, null, null),
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

        TestRemovedRecipeMessage removedRecipeMessage = new(
            testDocument.Id!,
            testSender,
            testDocument.MessageContent.Text,
            testDocument.SentDate,
            null);

        _messageFactoryMock
            .Setup(factory => factory.CreateRemovedRecipeMessage(
                removedRecipeMessage.Id,
                testSender,
                removedRecipeMessage.Text,
                new(),
                removedRecipeMessage.SentDate,
                removedRecipeMessage.UpdatedDate,
                null))
            .Returns(removedRecipeMessage);

        // When
        var result = (TestRemovedRecipeMessage?)_messageDocumentToModelMapperSUT.MapMessageFromDocument(testDocument, testSender, null);

        // Then
        result.Should().Be(removedRecipeMessage);
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void MapMessageFromDocument_WhenNoTextAndNonExistantRecipeIdsAndNoImageIds_ReturnRemovedRecipeMessageAndLogWarningForRecipesNotFound()
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

        TestRemovedRecipeMessage expectedResult = new(
            testDocument.Id!,
            testSender,
            testDocument.MessageContent.Text,
            testDocument.SentDate,
            testDocument.LastUpdatedDate
        );

        _recipeQueryRepositoryMock
            .Setup(repo => repo.GetRecipeById(It.IsAny<string>()))
            .Returns((string id) => recipes.FirstOrDefault(recipe => recipe.Id == id));
        _messageFactoryMock
            .Setup(factory => factory.CreateRemovedRecipeMessage(
                recipeMessage.Id,
                testSender,
                recipeMessage.Text,
                new(),
                recipeMessage.SentDate,
                recipeMessage.UpdatedDate,
                null))
            .Returns(expectedResult);

        // When
        var result = (TestRemovedRecipeMessage?)_messageDocumentToModelMapperSUT.MapMessageFromDocument(testDocument, testSender, null);

        // Then
        result.Should().Be(expectedResult);
        _loggerMock.Verify(logger =>
            logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(2));
    }

    [Fact]
    [Trait(Traits.DOMAIN, Traits.Domains.MESSAGING)]
    [Trait(Traits.MODULE, Traits.Modules.DATA_ACCESS)]
    public void MapMessageFromDocument_WhenOnlyRecipesAndRecipeIdDoesNotExist_ReturnRecipeMessageAndLogWarningForRecipesNotFound()
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
            .Setup(repo => repo.GetRecipeById(It.IsAny<string>()))
            .Returns((string id) => recipes.FirstOrDefault(recipe => recipe.Id == id));
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
        var result = (TestRecipeMessage?)_messageDocumentToModelMapperSUT.MapMessageFromDocument(testDocument, testSender, null);

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
    public void MapMessageFromDocument_WhenMessageContentIsMalformed_ThrowMalformedMessageDocumentException(bool isTextNull, bool isRecipeListNull, bool isImageListNull)
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
        var action = () => _messageDocumentToModelMapperSUT.MapMessageFromDocument(testDocument, testSender, null);

        // Then
        action.Should().Throw<MalformedMessageDocumentException>();
    }
}
